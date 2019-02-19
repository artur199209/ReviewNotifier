﻿using System;
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
        private int _codeReviewCount;
        private static HttpClient client;

        public TfsDataConnector(Settings settings, ILoginBuilder loginBuilder)
        {
            _url = settings.TfsUrl;
            _loginBuilder = loginBuilder;
            _lastId = lastId;
            _codeReviewCount = settings.CodeReviewCount;

            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format(":{0}", settings.PersonalAccessTokenToTFS))));
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
            var wiql = PrepareWiqlQuery(lastId);
            Console.WriteLine($"wiql {wiql}");
            var codeReviews = new List<CodeReview>();

            try
            {
                var wiUrl = $"{_url}/_apis/wit/wiql?$top=15&api-version=3.0";
                var response = client.PostWithResponse<WorkItemResults>(wiUrl, wiql);
                Console.WriteLine(wiUrl);
                if (!response.workItems.Any()) return codeReviews;
                Console.WriteLine($"Got {response.workItems.Count()} responses");
                var joinedWorkItemIds = string.Join(",", response.workItems.Select(x => x.id).Distinct().ToList());
                var qwe = $"{_url}/_apis/wit/WorkItems?ids={joinedWorkItemIds}&fields=Microsoft.VSTS.CodeReview.Context,Microsoft.VSTS.CodeReview.ContextOwner,System.CreatedBy,System.Title,System.Id&api-version=3.0";
                Console.WriteLine("Downloaded work itemdata");
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