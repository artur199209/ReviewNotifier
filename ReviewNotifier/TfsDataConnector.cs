using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using ReviewNotifier.Models;

namespace ReviewNotifier
{
    class TfsDataConnector
    {
        private readonly VssConnection _connection;
        private WorkItemTrackingHttpClient _witClient;
        private ILoginBuilder _loginBuilder;
        private string _shelvesetUrl;
        private int _lastId;

        public TfsDataConnector(Settings settings, ILoginBuilder loginBuilder, int lastId)
        {
            _shelvesetUrl = $"{settings.TfsUrl}_versionControl/shelvesets";
            var tfsUri = new Uri(settings.TfsUrl);
            _connection = new VssConnection(tfsUri, new VssBasicCredential(string.Empty, settings.PersonalAccessTokenToTFS));
            _loginBuilder = loginBuilder;
            _lastId = lastId;
        }

        private Wiql PrepareWiqlQuery()
        {
            var createdByQuery = _loginBuilder.GetCreateByQuery();

            var wiql = new Wiql
            {
                Query = "Select [ID], [State], [Title], [Work Item Type] From WorkItems " +
                        "Where [Work Item Type] = 'Code Review Request' " +
                        "And [System.TeamProject] = 'FenergoCore' " +
                        "And [State] = 'Requested' " +
                        "And [System.CreatedBy] in " + createdByQuery +
                        "And [ID] > " + _lastId +
                        "Order By [Created Date]"
            };

            return wiql;
        }

        public List<CodeReview> GetReviewData()
        {
            var wiql = PrepareWiqlQuery();
            var reviewInfos = new List<CodeReview>();

            try
            {
                _witClient = _connection.GetClient<WorkItemTrackingHttpClient>();

                var workItemQueryResult = _witClient.QueryByWiqlAsync(wiql, true).Result;

                if (workItemQueryResult?.WorkItems != null && workItemQueryResult.WorkItems.Any())
                {
                    var ids = workItemQueryResult.WorkItems.Select(x => x.Id).ToList();
                    var newCodeReviewItems = _witClient.GetWorkItemsAsync(ids, expand: WorkItemExpand.Links).Result;

                    reviewInfos = newCodeReviewItems.Select(item => new CodeReview
                    {
                        Id = item.Id.GetValueOrDefault(0),
                        CreatedBy = item.Fields["System.CreatedBy"].ToString(),
                        Title = item.Fields["System.Title"].ToString(),
                        WorkItemUrl = BuildUrl(item.Fields["Microsoft.VSTS.CodeReview.Context"].ToString(), item.Fields["Microsoft.VSTS.CodeReview.ContextOwner"].ToString())
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            return reviewInfos;
        }

        private string BuildUrl(string context, string createdBy)
        {
            return $"{_shelvesetUrl}?ss={context};{createdBy}";
        }
    }
}