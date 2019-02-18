using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using ReviewNotifier.Helpers;
using ReviewNotifier.Models;

namespace ReviewNotifier
{
    class TfsDataConnector
    {
        private ILoginBuilder _loginBuilder;
        private string _url;
        private int _lastId;
        private static HttpClient client;

        public TfsDataConnector(Settings settings, ILoginBuilder loginBuilder, int lastId)
        {
            _url = settings.TfsUrl;
            _loginBuilder = loginBuilder;
            _lastId = lastId;

            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format(":{0}", settings.PersonalAccessTokenToTFS))));
        }

        private string PrepareWiqlQuery()
        {
            var createdByQuery = _loginBuilder.GetCreateByQuery();
            var query = "Select [ID] From WorkItems " +
                        " Where [Work Item Type] = 'Code Review Request' " +
                        " And [State] = 'Requested' " +
                        " And [System.CreatedBy] in " + createdByQuery +
                        " And [ID] > " + _lastId +
                        " Order By [Created Date] DESC";
            return query;
        }

        public List<CodeReview> GetReviewData()
        {
            var wiql = PrepareWiqlQuery();
            var codeReviews = new List<CodeReview>();

            try
            {

                var response = client.PostWithResponse<WorkItemResults>($"{_url}/_apis/wit/wiql?$top=15&api-version=3.0", wiql);
                if (!response.workItems.Any()) return codeReviews;
                var joinedWorkItemIds = string.Join(",", response.workItems.Select(x => x.id).Distinct().ToList());
                var qwe = $"{_url}/_apis/wit/WorkItems?ids={joinedWorkItemIds}&fields=Microsoft.VSTS.CodeReview.Context,System.CreatedBy,System.Title,System.Id&api-version=3.0";
                codeReviews = client.GetWithResponse<WorkItemResults>(qwe).value.Select(x => x.fields).ToList();
                codeReviews.ForEach(x => x.BuildUrl(_url));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            return codeReviews;
        }

    }
}