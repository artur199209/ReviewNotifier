using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using ReviewNotifier.Helpers;
using ReviewNotifier.Interfaces;
using ReviewNotifier.Models;

namespace ReviewNotifier
{
    class TfsDataConnector
    {
        private readonly ILoginBuilder _loginBuilder;
        private readonly string _url;
        private readonly int _codeReviewCount;
        private static HttpClient _client;

        public TfsDataConnector(Settings settings, ILoginBuilder loginBuilder)
        {
            _url = settings.TfsUrl;
            _loginBuilder = loginBuilder;
            _codeReviewCount = settings.CodeReviewCount;

            _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(
                $":{settings.PersonalAccessTokenToTFS}")));
        }

        private string PrepareWiqlQuery(int lastId)
        {
            var createdByQuery = _loginBuilder.GetCreateByQuery();
            var query = "Select [ID] From WorkItems " +
                        " Where [Work Item Type] = 'Code Review Request' " +
                        " And [State] = 'Requested' " +
                        " And [System.CreatedBy] in " + createdByQuery +
                        " And [ID] > " + lastId +
                        " Order By [Created Date] DESC";
            return query;
        }

        public List<CodeReview> GetReviewData(int lastId)
        {
            Logger.SaveLog("Preparing query...");
            var wiql = PrepareWiqlQuery(lastId);
            Logger.SaveLog($"wiql {wiql}");
            Console.WriteLine($"wiql {wiql}");
            var codeReviews = new List<CodeReview>();

            try
            {
                var wiUrl = $"{_url}/_apis/wit/wiql?$top=15&api-version=3.0";
                var response = _client.PostWithResponse<WorkItemResults>(wiUrl, wiql);
                Logger.SaveLog(wiUrl);
                Console.WriteLine(wiUrl);
                if (!response.WorkItems.Any()) return codeReviews;
                Logger.SaveLog($"Got {response.WorkItems.Length} responses");
                Console.WriteLine($"Got {response.WorkItems.Length} responses");
                var joinedWorkItemIds = string.Join(",", response.WorkItems.Select(x => x.Id).Distinct().Take(_codeReviewCount).ToList());
                var witUrl = $"{_url}/_apis/wit/WorkItems?ids={joinedWorkItemIds}&fields=Microsoft.VSTS.CodeReview.Context,Microsoft.VSTS.CodeReview.ContextOwner,System.CreatedBy,System.Title,System.Id&api-version=3.0";
                Logger.SaveLog("Downloaded work itemdata");
                Console.WriteLine("Downloaded work itemdata");
                codeReviews = _client.GetWithResponse<WorkItemResults>(witUrl).Value.Select(x => x.Fields).ToList();
                codeReviews.ForEach(x => x.BuildUrl(_url));
            }
            catch (Exception ex)
            {
                Logger.SaveLog(ex.Message);
                Console.WriteLine(ex.Message);
            }
            
            return codeReviews;
        }

    }
}