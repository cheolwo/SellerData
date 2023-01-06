using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using SellerCommon.SellerData.Model;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace SellerCommon.SellerData.Services
{
    public interface INaverSerachRelKwdAPIService
    {
        Task<NaverRelKwdStatResult> GetRelKwdStat(List<string> HintKwds);
        Task<NaverRelKwdStatResult> GetRelKwdStat(string SingleHintKwd);
        Task<List<CommodityAnalyzedInfo>> GetRelKwdStat(List<string> HintKwds, string sellerMCommodityId, int FilteryByMobileQc);
    }
    public class NaverSerachRelKwdAPIService : INaverSerachRelKwdAPIService
    {
        private const string BaseUrl = "https://api.naver.com";
        private readonly long CustomerId;
        private readonly string ApiKey;
        private readonly string SecretKey;
        private readonly IConfiguration _Configuration;
        public NaverSerachRelKwdAPIService(IConfiguration configuration)
        {
            _Configuration = configuration;
            ApiKey = _Configuration.GetSection("NaverSerachAdAPIService")["ApiKey"];
            SecretKey = _Configuration.GetSection("NaverSerachAdAPIService")["SecretKey"];
            CustomerId = long.Parse(_Configuration.GetSection("NaverSerachAdAPIService")["CustomerId"]);

            Console.WriteLine(ApiKey);
            Console.WriteLine(SecretKey);
            Console.Write(CustomerId);
        }
        public async Task<NaverRelKwdStatResult> GetRelKwdStat(List<string> HintKwds)
        {
            var HintKwdString = CreateHintKwds(HintKwds);
            var rest = new SearchAdApi(BaseUrl, ApiKey, SecretKey);
            var request = CreateRestRequest(HintKwdString);
            var result = await rest.ExecuteAsync<NaverRelKwdStatResult>(request, CustomerId);
            return result;
        }
        public async Task<List<CommodityAnalyzedInfo>> GetRelKwdStat(List<string> HintKwds, string sellerMCommodityId, int FilteryByMobileQc)
        {
            List<CommodityAnalyzedInfo> commodityAnalyzedInfos = new();
            var HintKwdString = CreateHintKwds(HintKwds);
            var rest = new SearchAdApi(BaseUrl, ApiKey, SecretKey);
            var request = CreateRestRequest(HintKwdString);
            var result = await rest.ExecuteAsync<NaverRelKwdStatResult>(request, CustomerId);
            ConfigureRelKwdInfosWithFilterFilterByQc(result, sellerMCommodityId, FilteryByMobileQc, commodityAnalyzedInfos);
            return commodityAnalyzedInfos;
        }
        public async Task<NaverRelKwdStatResult> GetRelKwdStat(string SingleHintKwd)
        {
            var rest = new SearchAdApi(BaseUrl, ApiKey, SecretKey);
            var request = CreateRestRequest(SingleHintKwd);
            var result = await rest.ExecuteAsync<NaverRelKwdStatResult>(request, CustomerId);
            return result;
        }
        public void ConfigureRelKwdInfosWithFilterFilterByQc(NaverRelKwdStatResult naverRelKwdStatResult, string sellerMCommodityId, int FilteryByMobileQc, List<CommodityAnalyzedInfo> commodityAnalyzedInfos)
        {
            var valuelist = naverRelKwdStatResult.keywordList;
            foreach (var value in valuelist)
            {
                if (value.monthlyPcQcCnt.Contains('<'))
                {
                    var index = value.monthlyPcQcCnt.IndexOf('<');
                    value.monthlyPcQcCnt = value.monthlyPcQcCnt.Substring(index + 1, value.monthlyPcQcCnt.Length - 1);
                }
                if (value.monthlyMobileQcCnt.Contains('<'))
                {
                    var index = value.monthlyMobileQcCnt.IndexOf('<');
                    value.monthlyMobileQcCnt = value.monthlyMobileQcCnt.Substring(index + 1, value.monthlyMobileQcCnt.Length - 1);
                }
                int monthlyPcQcCnt = int.Parse(value.monthlyPcQcCnt);
                int monthlyMobileQcCnt = int.Parse(value.monthlyMobileQcCnt);
                FilterByQc(value, commodityAnalyzedInfos, monthlyMobileQcCnt, monthlyPcQcCnt, FilteryByMobileQc, sellerMCommodityId);
            }
        }
        private void FilterByQc(KeywordInfo value, List<CommodityAnalyzedInfo> commodityAnalyzedInfos, int monthlyMobileQcCnt, int monthlyPcQcCnt, int FilteryByMobileQc, string sellerMCommodityId)
        {
            if (monthlyMobileQcCnt > FilteryByMobileQc)
            {
                CommodityAnalyzedInfo commodityAnalyzedInfo = new();
                commodityAnalyzedInfo.MonthlyPcQcCnt = monthlyPcQcCnt;
                commodityAnalyzedInfo.MonthlyMobileQcCnt = monthlyMobileQcCnt;
                commodityAnalyzedInfo.SeletedKeywords = value.relKeyword;
                commodityAnalyzedInfo.SellerMCommodityId = sellerMCommodityId;
                commodityAnalyzedInfos.Add(commodityAnalyzedInfo);
            }
        }
        private RestRequest CreateRestRequest(string HintKwds)
        {
            var request = new RestRequest("/keywordstool", Method.GET);
            request.AddQueryParameter("hintKeywords", HintKwds);
            request.AddQueryParameter("includeHintKeywords", "1");
            request.AddQueryParameter("showDetail", "1");
            return request;
        }
        private string CreateHintKwds(List<string> HintKwds)
        {
            StringBuilder stringBuilder = new();
            for (int i = 0; i < HintKwds.Count; i++)
            {
                stringBuilder.Append(HintKwds[i]);
                if (HintKwds[i] == HintKwds[HintKwds.Count - 1]) { continue; }
                else { stringBuilder.Append(','); }
            }
            return stringBuilder.ToString();
        }
    }
    public class NaverRelKwdStatResult
    {
        public KeywordInfo[] keywordList { get; set; }
    }

    public class KeywordInfo
    {
        //public string relKeyword { get; set; }
        //public string monthlyPcQcCnt { get; set; }
        //public string monthlyMobileQcCnt { get; set; }
        //public string monthlyAvePcClkCnt { get; set; }
        //public string monthlyAveMobileClkCnt { get; set; }
        //public string monthlyAvePcCtr { get; set; }
        //public string monthlyAveMobileCtr { get; set; }
        //public string plAvgDepth { get; set; }
        //public string compIdx { get; set; }
        public string relKeyword { get; set; }
        public string monthlyPcQcCnt { get; set; }
        public string monthlyMobileQcCnt { get; set; }
        public float monthlyAvePcClkCnt { get; set; }
        public float monthlyAveMobileClkCnt { get; set; }
        public float monthlyAvePcCtr { get; set; }
        public float monthlyAveMobileCtr { get; set; }
        public int plAvgDepth { get; set; }
        public string compIdx { get; set; }
    }
    public class SearchAdApi
    {
        private readonly string BaseUrl;
        private readonly string ApiKey;
        private readonly string SecretKey;
        private readonly HMACSHA256 HMAC;

        public SearchAdApi(string baseUrl, string apiKey, string secretKey)
        {
            this.BaseUrl = baseUrl;
            this.ApiKey = apiKey;
            this.SecretKey = secretKey;
            this.HMAC = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
        }

        public async Task<T> ExecuteAsync<T>(RestRequest request, long customerId) where T : new()
        {
            var client = new RestClient(BaseUrl);

            var timestamp = getTimestamp().ToString();
            var signature = generateSignature(timestamp, request.Method.ToString(), request.Resource);

            request.AddHeader("X-API-KEY", ApiKey);
            request.AddHeader("X-Customer", customerId.ToString());
            request.AddHeader("X-Timestamp", timestamp);
            request.AddHeader("X-Signature", signature);

            printRequest(request);

            var response = await client.ExecuteAsync<T>(request);

            printResponse(response);

            return JsonConvert.DeserializeObject<T>(response.Content);
        }

        private long getTimestamp()
        {
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        private string generateSignature(string timestamp, string method, string resource)
        {
            return Convert.ToBase64String(HMAC.ComputeHash(Encoding.UTF8.GetBytes(timestamp + "." + method + "." + resource)));
        }

        private static readonly Func<Parameter, bool> IS_HEADER = param => param.Type == ParameterType.HttpHeader;
        private static readonly Func<Parameter, bool> IS_QUERYPARAM = param => param.Type == ParameterType.QueryString;

        private void printRequest(RestRequest request)
        {
            Console.WriteLine("============ Request =============");
            Console.Write(request.Method + " " + request.Resource);

            var queryParams = request.Parameters.Where(IS_QUERYPARAM).ToList();
            Console.WriteLine(queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "");

            request.Parameters.Where(IS_HEADER).ToList().ForEach(Console.WriteLine);
            Console.WriteLine();
        }

        private void printResponse(IRestResponse response)
        {
            Console.WriteLine("============ Response ============");
            Console.WriteLine(((int)response.StatusCode) + " " + response.StatusCode);

            foreach (Parameter param in response.Headers)
            {
                Console.WriteLine(param);
            }

            Console.WriteLine();
            Console.WriteLine(response.Content);
            Console.WriteLine();
        }
    }
}