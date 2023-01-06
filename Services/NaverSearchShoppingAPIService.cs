using BusinessData.ofDataAccessLayer.ofModelExtenstions;
using Microsoft.Extensions.Logging;
using SellerCommon.SellerData.Model;
using SellerData.ofDataAccessLayer.ofDataContext;
using System.Text.RegularExpressions;
using System.Xml;

namespace SellerCommon.SellerData.Services
{
    public interface INaverSearchShoppingAPIService
    {
        Task<CommodityAnalyzedInfo> SearchingByKeywords(string Keywords);
        Task SetttingNameByImportantKwds(List<SellerSMCommodity> sellerSMCommodites);
    }
    public class NaverSearchShoppingService : INaverSearchShoppingAPIService
    {
        private HttpClient HttpClient = new();
        private readonly SellerMarketDataContext _sellerMarketDataContext;
        private readonly ILogger<NaverSearchShoppingService> _logger;
        private const string Url = "https://openapi.naver.com/v1/search/shop.xml";
        private string queryurl = "";
        public NaverSearchShoppingService(SellerMarketDataContext sellerMarketDataContext, ILogger<NaverSearchShoppingService> logger)
        {
            HttpClient.DefaultRequestHeaders.Add("X-Naver-Client-Id", "A4WcT6iYNnPs7_NbkMhi");
            HttpClient.DefaultRequestHeaders.Add("X-Naver-Client-Secret", "AQM69CB_6z");
            _sellerMarketDataContext = sellerMarketDataContext;
            _logger = logger;
        }
        public async Task<CommodityAnalyzedInfo> SearchingByKeywords(string Keywords)
        {
            CommodityAnalyzedInfo commodityAnalyzedInfo = new();
            commodityAnalyzedInfo.SeletedKeywords = Keywords;

            var encodingvalue = EncodingByUtf8(Keywords);
            Console.WriteLine(encodingvalue);
            var url = AddQuery(encodingvalue);
            var value = await HttpClient.GetAsync(url);
            var stringvalue = await value.Content.ReadAsStringAsync();

            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(stringvalue);
            var total = xmldoc.SelectSingleNode("//total");
            var category1 = xmldoc.SelectSingleNode("//category1");
            var category2 = xmldoc.SelectSingleNode("//category2");
            var category3 = xmldoc.SelectSingleNode("//category3");
            var category4 = xmldoc.SelectSingleNode("//category4");

            commodityAnalyzedInfo.TotalCommodityCount = int.Parse(total?.InnerText??"0");
            commodityAnalyzedInfo.Category1 = category1?.InnerText?? "";
            commodityAnalyzedInfo.Category2 = category2?.InnerText?? "";
            commodityAnalyzedInfo.Category3 = category3?.InnerText?? "";
            commodityAnalyzedInfo.Category4 = category4?.InnerText?? "";

            return commodityAnalyzedInfo;
        }
        public async Task SetttingNameByImportantKwds(List<SellerSMCommodity> sellerSMCommodites)
        {
            int CurrentStateCount = 1;
            foreach(var sellerSMCommodity in sellerSMCommodites)
            {
                _logger.LogInformation(sellerSMCommodites.Count.ToString());
                _logger.LogInformation(CurrentStateCount.ToString());
                CurrentStateCount++;

                if (sellerSMCommodity.SellerMCommodity == null) { sellerSMCommodity.SellerMCommodity = await _sellerMarketDataContext.GetByIdAsync<SellerMCommodity>(sellerSMCommodity.SellerMCommodityId); }
                var encodingvalue = EncodingByUtf8(sellerSMCommodity.SellerMCommodity.ToStringExceptForIndex0());
                Console.WriteLine(encodingvalue);
                var url = AddQuery(encodingvalue);
                var value = await HttpClient.GetAsync(url);
                var stringvalue = await value.Content.ReadAsStringAsync();

                XmlDocument xmldoc = new XmlDocument();
                xmldoc.LoadXml(stringvalue);
                var mallname = xmldoc.SelectSingleNode("//mallName");
                var titles = xmldoc.SelectNodes("//title");
                var mallnames = xmldoc.SelectNodes("//mallName");
                Dictionary<string, string> DicMallTitle = new();
                int cnt = 1;
                foreach (var mall in mallnames)
                {
                    if(cnt == 5) { break; }
                    if (mall != null && titles[cnt] != null)
                    {
                        XmlNode mallNode = (XmlNode)mall;
                        if(DicMallTitle.Keys.Contains(mallNode.InnerText)) { continue; }
                        XmlNode titleNode = titles[cnt];
                        Console.WriteLine(mallNode.InnerText);
                        var title = Regex.Replace(titleNode.InnerText, "<b>|</b>", "");
                        DicMallTitle.Add(mallNode.InnerText, title);
                        cnt++;
                    }
                }
                foreach (var key in DicMallTitle.Keys)
                {
                    if (!DicMallTitle[key].Contains(key))
                    {
                        sellerSMCommodity.Name = DicMallTitle[key];
                        await sellerSMCommodity.PutAsync(_sellerMarketDataContext);
                        break;
                    }
                }
            }
            
        }
        private string AddQuery(string value)
        {
            queryurl = Url + $"?query={value}";
            return queryurl;
        }
        private string EncodingByUtf8(string s)
        {
            byte[] utf8Bytes = System.Text.Encoding.UTF8.GetBytes(s);
            string urlEncodingText = "";
            foreach (byte b in utf8Bytes)
            {
                string addText = Convert.ToString(b, 16);
                urlEncodingText = urlEncodingText + "%" + addText;
            }
            return Convert.ToString(urlEncodingText);
        }
    }
}