using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using ReviewNotifier.Helpers;
using ReviewNotifier.Interfaces;
using ReviewNotifier.Models;
using log4net;

namespace ReviewNotifier
{
    class TfsDataConnector
    {
        private readonly ILoginBuilder _loginBuilder;
        private readonly string _tfsUrl;
        private readonly string _adoUrl;
        private readonly int _codeReviewCount;
        private static HttpClient _tfs;
        private static HttpClient _ado;
        private readonly ILog _logger;
        public static Regex ChangesetRegex = new Regex(@".*\[(.*)\].*");

        public TfsDataConnector(Settings settings, ILoginBuilder loginBuilder)
        {
            _logger = Log4NetConfig.GetLogger();
            _tfsUrl = settings.TfsUrl;
            _adoUrl = settings.AdoUrl;
            _loginBuilder = loginBuilder;
            _codeReviewCount = settings.CodeReviewCount;
            _tfs = SetupClient(settings.TfsUrl,settings.PersonalAccessTokenToTFS);
            _ado = SetupClient(settings.AdoUrl,settings.AdoToken);
        }
        

        public HttpClient SetupClient(string url, string key)
        {
            var client = new HttpClient {BaseAddress = new Uri(url)};
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes($":{key}")));
            return client;
        }


        private string PrepareWiqlQuery(int lastId)
        {
            var createdByQuery = _loginBuilder.GetCreateByQuery();
            var query = "Select [ID] From WorkItems " +
                        " Where [Work Item Type] = 'Code Review Request' " +
                        " And [State] = 'Requested' ";

            if (!string.IsNullOrEmpty(createdByQuery))
            {
                query += " And [System.CreatedBy] in " + createdByQuery;
            }
            
            query += " And [ID] > " + lastId +
                     " Order By [Created Date] DESC";

            return query;
        }

        public List<WorkItemData> GetReviewData(int lastId)
        {
            _logger.Info("Preparing query...");
            var wiql = PrepareWiqlQuery(lastId);

            _logger.Info($"Wiql query: {wiql}");
            var codeReviews = new List<WorkItemData>();

            try
            {
                var wiUrl = "_apis/wit/wiql?$top=15&api-version=3.0";
                var response = _tfs.PostWithResponse<WorkItemResults>(wiUrl, wiql);
                if (!response.WorkItems.Any()) return codeReviews;
                _logger.Info($"Got {response.WorkItems.Length} responses");
                var joinedWorkItemIds = string.Join(",", response.WorkItems.Select(x => x.Id).Distinct().Take(_codeReviewCount).ToList());
                var fields = "Microsoft.VSTS.CodeReview.Context,Microsoft.VSTS.CodeReview.ContextOwner,System.CreatedBy,System.Title,System.Id";
                codeReviews = DownloadWorkItems(_tfs,joinedWorkItemIds, fields);
                codeReviews.ForEach(x => x.BuildShelvesetUrl(_tfsUrl));
                foreach (var codeReview in codeReviews)
                {
                    codeReview.WorkItems = new List<WorkItemData>();
                    var idMatch = ChangesetRegex.Match(codeReview.Title);
                    if (!idMatch.Success) continue;
                    var matchedWorkItemIds = idMatch.Groups[1].Value;
                    fields = "System.Title,System.Id";
                    var codeReviewWorkItems = DownloadWorkItems(_ado, matchedWorkItemIds, fields);
                    codeReview.WorkItems = codeReviewWorkItems?? new List<WorkItemData>();
                    codeReview.WorkItems.ForEach(x => x.BuildWorkItemUrl(_adoUrl));
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.InnerException);
                _logger.Error(ex.StackTrace);
            }
            
            return codeReviews;
        }

        private List<WorkItemData> DownloadWorkItems(HttpClient client, string ids, string fields)
        {
            var witUrl = $"_apis/wit/WorkItems?ids={ids}&fields={fields}&api-version=3.0";
            _logger.Info("Downloaded work itemdata");
            var codeReviews = client.GetWithResponse<WorkItemResults>(witUrl).Value.Select(x => x.Fields).ToList();
            return codeReviews;
        }
    }
}