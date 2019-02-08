using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using ReviewNotifier.Config;
using ReviewNotifier.Models;

namespace ReviewNotifier.Helpers
{
    class CodeReview
    {
        private readonly VssConnection _connection;
        private WorkItemTrackingHttpClient _witClient;
        private readonly LastIdSaver _lastIdSaver;

        public CodeReview()
        {
            var configuration = Configuration.ConfigInstance;
            var tfsUrl = configuration.GetSection("tfsUrl").Value;
            var tfsUri = new Uri(tfsUrl);
            _connection = new VssConnection(tfsUri, new VssCredentials());
            _lastIdSaver = new LastIdSaver();
        }

        private Wiql PrepareWiqlQuery()
        {
            var loginBuilder = new LoginBuilder();
            var lastId = _lastIdSaver.GetValueFromFile();

            var createdByQuery = loginBuilder.GetCreateByQuery();
            var dateTime = DateTimeHelper.GetCurrentDateTime().ToUniversalTime().AddMinutes(-1);

            var wiql = new Wiql()
            {
                Query = "Select [ID], [State], [Title], [Work Item Type] From WorkItems Where [Work Item Type] = 'Code Review Request' " +
                        "And [System.TeamProject] = 'FenergoCore' And [State] = 'Requested' And [System.CreatedBy] in " +
                        createdByQuery + " And [Created Date] > " + DateTimeHelper.GetDateTimeInFormattedType(dateTime) +
                        " And [ID] > " + lastId + 
                        " Order By [Created Date]"
            };

            return wiql;
        }

        public List<ReviewInfo> ExecuteWiqlQuery()
        {
            var wiql = PrepareWiqlQuery();
            var reviewInfos = new List<ReviewInfo>();

            try
            {
                _witClient = _connection.GetClient<WorkItemTrackingHttpClient>();

                var workItemQueryResult = _witClient.QueryByWiqlAsync(wiql, true).Result;
                var ids = workItemQueryResult?.WorkItems.Select(x => x.Id).ToList();

                if (workItemQueryResult.WorkItems != null && workItemQueryResult.WorkItems.Any())
                {
                    var newCodeReviewItems = _witClient.GetWorkItemsAsync(ids, expand: WorkItemExpand.Links).Result;
                  
                    foreach (var item in newCodeReviewItems)
                    {
                        var info = new ReviewInfo
                        {
                            CreatedBy = item.Fields["System.CreatedBy"].ToString(),
                            Title = item.Fields["System.Title"].ToString(),
                            WorkItemUrl = BuildUrl(item.Fields["Microsoft.VSTS.CodeReview.Context"].ToString(), item.Fields["Microsoft.VSTS.CodeReview.ContextOwner"].ToString())
                        };

                        reviewInfos.Add(info);
                    }
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
            return $"https://secure.fenergo.com/tfs/Product/FenergoCore/_versionControl/shelveset?ss={context};{createdBy}";
        }
    }
}