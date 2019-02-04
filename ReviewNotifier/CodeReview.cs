using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace ReviewNotifier
{
    class CodeReview
    {
        private readonly VssConnection _connection;
        private WorkItemTrackingHttpClient _witClient;
        private readonly IConfigurationRoot _configuration;

        public CodeReview()
        {
            _configuration = Configuration.configInstance;
            var tfsUrl = _configuration.GetSection("tfsUrl").Value;
            var tfsUri = new Uri(tfsUrl);
            _connection = new VssConnection(tfsUri, new VssCredentials());
        }

        private Wiql PrepareWiqlQuery()
        {
            var loginBuilder = new LoginBuilder();

            var createdByQuery = loginBuilder.GetCreateByQuery();
            var dateTime = DateTimeHelper.GetCurrentDateTime().ToUniversalTime().AddMinutes(-1);

            var wiql = new Wiql()
            {
                Query = "Select [ID], [State], [Title], [Work Item Type] From WorkItems Where [Work Item Type] = 'Code Review Request' " +
                        "And [System.TeamProject] = 'ProjectName' And [State] = 'Requested' And [System.CreatedBy] in " +
                        createdByQuery + " And [Created Date] > " + DateTimeHelper.GetDateTimeInFormattedType(dateTime) +
                        " Order By [Created Date]"
            };

            return wiql;
        }

        public List<ReviewInfo> ExecuteWiqlQuery()
        {
            var wiql = PrepareWiqlQuery();
            
            WorkItemQueryResult workItemQueryResult;
            List<ReviewInfo> reviewInfos = new List<ReviewInfo>();

            try
            {
                _witClient = _connection.GetClient<WorkItemTrackingHttpClient>();
                workItemQueryResult = _witClient.QueryByWiqlAsync(wiql, true).Result;
                var Ids = workItemQueryResult?.WorkItems.Select(x => x.Id).ToList();

                if (workItemQueryResult.WorkItems != null && workItemQueryResult.WorkItems.Any())
                {
                    var newCodeReviewItems = _witClient.GetWorkItemsAsync(Ids, expand: WorkItemExpand.All).Result;

                    foreach (var item in newCodeReviewItems)
                    {
                        var info = new ReviewInfo
                        {
                            CreatedBy = item.Fields["System.CreatedBy"].ToString(),
                            Title = item.Fields["System.Title"].ToString()
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
    }
}