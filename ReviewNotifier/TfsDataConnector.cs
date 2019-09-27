using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using ReviewNotifier.Helpers;
using ReviewNotifier.Interfaces;
using ReviewNotifier.Models;
using log4net;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace ReviewNotifier
{
    class TfsDataConnector
    {
        private readonly ILoginBuilder _loginBuilder;
        private readonly string _url;
        private readonly int _codeReviewCount;
        private static HttpClient _client;
        private readonly ILog _logger;
        private readonly TfvcHttpClient _tfvcHttpClient;
        private readonly VssConnection _vssConnection;

        public TfsDataConnector(Settings settings, ILoginBuilder loginBuilder)
        {
            _logger = Log4NetConfig.GetLogger();
            _url = settings.TfsUrl;
            _loginBuilder = loginBuilder;
            _codeReviewCount = settings.CodeReviewCount;
            _client = new HttpClient();
           // VssConnection connection = new VssConnection(new Uri(_url), new VssBasicCredential(string.Empty, Convert.ToBase64String(Encoding.ASCII.GetBytes($":umjy7kf5duv4euyh4scz7txrz3opdem4trliitsqahtbr7pssg2a"))));
            _vssConnection = new VssConnection(new Uri(_url), new VssCredentials());
            _tfvcHttpClient = _vssConnection.GetClient<TfvcHttpClient>();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($":{settings.PersonalAccessTokenToTFS}")));
        }

        private string PrepareWiqlQuery(int lastId)
        {
            var createdByQuery = _loginBuilder.GetCreateByQuery();
            var query = "Select* From WorkItems " +
                        " Where [Work Item Type] = 'Code Review Request' " +
                        " And [State] = 'Closed' ";

            if (!string.IsNullOrEmpty(createdByQuery))
            {
                query += " And [System.CreatedBy] in " + createdByQuery;
            }

            query += " And [ID] > " + lastId +
                     " Order By [Created Date] DESC";

            return query;
        }

        public List<CodeReview> GetReviewData(int lastId)
        {
            _logger.Info("Preparing query...");
            var wiql = PrepareWiqlQuery(lastId);

            _logger.Info($"Wiql query: {wiql}");
            var codeReviews = new List<CodeReview>();

            try
            {
                var wiUrl = $"{_url}/_apis/wit/wiql?$top=15&api-version=3.0";
                var response = _client.PostWithResponse<WorkItemResults>(wiUrl, wiql);
                if (!response.WorkItems.Any()) return codeReviews;
                _logger.Info($"Got {response.WorkItems.Length} responses");
                var joinedWorkItemIds = string.Join(",", response.WorkItems.Select(x => x.Id).Distinct().Take(_codeReviewCount).ToList());
                var witUrl = $"{_url}/_apis/wit/WorkItems?ids={joinedWorkItemIds}&fields=Microsoft.VSTS.CodeReview.Context,Microsoft.VSTS.CodeReview.ContextOwner,System.CreatedBy,System.TeamProject,System.Title,System.Id&api-version=3.0";
                _logger.Info("Downloaded work itemdata");
                codeReviews = _client.GetWithResponse<WorkItemResults>(witUrl).Value.Select(x => x.Fields).ToList();
                codeReviews.ForEach(x => x.BuildUrl(_url));
                
                foreach (var codeReview in codeReviews)
                {
                    var context = codeReview.Context;
                    var contextOwner = codeReview.ContextOwner;
                    var shelvesetId = $"{context};{contextOwner}";
                    var shelvesetsChanges = _tfvcHttpClient.GetShelvesetChangesAsync(shelvesetId).Result;
                    var changedFileList = shelvesetsChanges.Select(x => x.Item.Path).ToList();

                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex.InnerException);
                _logger.Error(ex.StackTrace);
            }

            return codeReviews;
        }
        

    }
}