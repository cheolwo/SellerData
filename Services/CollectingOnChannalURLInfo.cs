using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Text;

namespace SellerData.Services
{
    public interface ICollectingOnChannalURLInfo
    {
        Task CollectCommmodityURLInfo(List<string> Path, string Url);
        Task CollectOnch3ExcellCommodity(List<string> Path);
        Task CollectOnch3ExcellentCommodityPathByYear(List<string> Path, string Year);
        List<string> GetExcellentCommodityPageUrlByYear(string year);
        List<string> CollectCommodityURLInfo(IWebDriver webDriver);
        List<string> CollectDataCenterCommodityURLInfo(IWebDriver webDriver);
    }
    public class CollectingOnChannalURLInfo : ICollectingOnChannalURLInfo
    {
        private HttpClient _HttpClient;
        private HtmlDocument _HtmlDocument;
        public List<string> Urls = new();
        private Dictionary<string, List<string>> DicYearUrls;
        private const string Och3 = "https://www.onch3.co.kr/";
        public CollectingOnChannalURLInfo()
        {
            _HttpClient = new();
            _HtmlDocument = new();
            DicYearUrls = CreateExcellentPageInputURLByYear();
        }
        private const string prdListXPath = "/html/body/div[1]/div[2]/div/div[3]/div/div[4]/div[1]/div[2]/form/ul";
        /// <summary>
        /// 여기서 Path는 ViewModel 에서 전달받은 Path
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public List<string> GetExcellentCommodityPageUrlByYear(string year)
        {
            return DicYearUrls[year];
        }
        public async Task CollectOnch3ExcellCommodity(List<string> Path)
        {
            var Urls = CreateExcellentPageInputURL();
            foreach (var url in Urls)
            {
                await CollectCommmodityURLInfo(Path, url);
            }
        }
        public List<string> CollectCommodityURLInfo(IWebDriver webDriver)
        {
            var prd_img_Nodes = webDriver.FindElements(By.XPath("//li[contains(@class, 'prd_img')]"));
            List<string> Paths = new();
            if(prd_img_Nodes != null)
            {
                foreach(var imgNode in prd_img_Nodes)
                {
                    var node = imgNode.FindElement(By.XPath("a"));
                    var href = node.GetAttribute("href");
                    if(href != null && href.Contains("onch_view"))
                    {
                        Paths.Add(href);
                        Console.WriteLine(href);
                    }
                }
            }
            return Paths;
        }
        public List<string> CollectDataCenterCommodityURLInfo(IWebDriver webDriver)
        {
            var prd_img_Nodes = webDriver.FindElements(By.XPath("//li[contains(@class, 'product_img')]"));
            List<string> Paths = new();
            if (prd_img_Nodes != null)
            {
                foreach (var imgNode in prd_img_Nodes)
                {
                    var node = imgNode.FindElement(By.XPath("a"));
                    var href = node.GetAttribute("href");
                    if (href != null && href.Contains("dbcenter"))
                    {
                        Paths.Add(href);
                        Console.WriteLine(href);
                    }
                }
            }
            return Paths;
        }
        public async Task CollectCommmodityURLInfo(List<string> Path, string Url)
        {
            var StreamInfo = await _HttpClient.GetStreamAsync(Url);
            _HtmlDocument.Load(StreamInfo, Encoding.UTF8);
            ////span[contains(@class, 'detail_page_name')]
            var prd_img_Nodes = _HtmlDocument.DocumentNode.SelectNodes("//li[contains(@class, 'btn_sale')]");
            if (prd_img_Nodes != null)
            {
                foreach (var imgNode in prd_img_Nodes)
                {
                    var node = imgNode.SelectSingleNode("a");
                    var href = node.GetAttributeValue("href", "");
                    var value = Path.Find(e => e.Equals(href));
                    if (value == null)
                    {
                        bool IsAdd = await IsSelling(href);
                        if(IsAdd)
                        {
                            Path.Add(href);
                            Console.WriteLine(href);
                        }
                    }
                }
            }
        }
        private async Task<bool> IsSelling(string linkUrl)
        {
            string Och3 = "https://www.onch3.co.kr/";
            var Och3RequestUrl = Och3 + linkUrl;
            var ResultStream = await _HttpClient.GetStreamAsync(Och3RequestUrl);
            _HtmlDocument.Load(ResultStream, Encoding.Default);
            Console.Write($"IsSelling { linkUrl }");
            ///html/body/div[1]/div[2]/div[1]/div[3]/div[2]/ul/li[6]/ul/li[1]/text()
            var State = _HtmlDocument.DocumentNode.SelectNodes("/html/body/div[1]/div[2]/div[1]/div[3]/div[2]/ul/li[6]/ul/li[1]");
            if(State == null) { return false; }
            if (State[0].InnerText.Contains("정상판매"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task CollectOnch3ExcellentCommodityPathByYear(List<string> Path, string Year)
        {
            var YearUrls = DicYearUrls[Year];
            foreach (var url in YearUrls)
            {
                await CollectCommmodityURLInfo(Path, url);
            }
        }
        private string DetailPrdXPathInfo(int cnt)
        {
            var PrdDetailListIndex = $"/li[{cnt}]/ul/li[8]";
            return PrdDetailListIndex;
        }
        private List<string> CreateExcellentPageInputURL()
        {
            List<string> strings = new();
            for (int i = 1; i <= 25; i++)
            {
                var InputString = $"https://www.onch3.co.kr/onch_hall_of_fame.html?sec=best&page={i}&sear_txt=&type=&catef=&cates=&catet=";
                strings.Add(InputString);
            }
            return strings;
        }
        //https://www.onch3.co.kr/onch_hall_of_fame.html?sec=2021&page=1&sear_txt=&type=&catef=&cates=&catet=
        //https://www.onch3.co.kr/onch_hall_of_fame.html?sec=2020&page=1&sear_txt=&type=&catef=&cates=&catet=

        private Dictionary<string, List<string>> CreateExcellentPageInputURLByYear()
        {
            Dictionary<string, List<string>> DicYearUrls = new();
            var Url2012 = CreateExcellentPageInputURL2012();
            var Url2013 = CreateExcellentPageInputURL2013();
            var Url2014 = CreateExcellentPageInputURL2014();
            var Url2015 = CreateExcellentPageInputURL2015();
            var Url2016 = CreateExcellentPageInputURL2016();
            var Url2017 = CreateExcellentPageInputURL2017();
            var Url2018 = CreateExcellentPageInputURL2018();
            var Url2019 = CreateExcellentPageInputURL2019();
            var Url2020 = CreateExcellentPageInputURL2020();
            var Url2021 = CreateExcellentPageInputURL2021();

            DicYearUrls.Add("2012", Url2012);
            DicYearUrls.Add("2013", Url2013);
            DicYearUrls.Add("2014", Url2014);
            DicYearUrls.Add("2015", Url2015);
            DicYearUrls.Add("2016", Url2016);
            DicYearUrls.Add("2017", Url2017);
            DicYearUrls.Add("2018", Url2018);
            DicYearUrls.Add("2019", Url2019);
            DicYearUrls.Add("2020", Url2020);
            DicYearUrls.Add("2021", Url2021);
            return DicYearUrls;
        }
        private List<string> CreateExcellentPageInputURL2012()
        {
            List<string> Urls = new();
            for (int i = 1; i <= 13; i++)
            {
                var InputString = $"https://www.onch3.co.kr/onch_hall_of_fame.html?sec=2012&page={i}&sear_txt=&type=&catef=&cates=&catet=";
                Urls.Add(InputString);
            }
            return Urls;
        }
        private List<string> CreateExcellentPageInputURL2013()
        {
            List<string> Urls = new();
            for (int i = 1; i <= 15; i++)
            {
                var InputString = $"https://www.onch3.co.kr/onch_hall_of_fame.html?sec=2013&page={i}&sear_txt=&type=&catef=&cates=&catet=";
                Urls.Add(InputString);
            }
            return Urls;
        }
        private List<string> CreateExcellentPageInputURL2014()
        {
            List<string> Urls = new();
            for (int i = 1; i <= 15; i++)
            {
                var InputString = $"https://www.onch3.co.kr/onch_hall_of_fame.html?sec=2014&page={i}&sear_txt=&type=&catef=&cates=&catet=";
                Urls.Add(InputString);
            }
            return Urls;
        }
        private List<string> CreateExcellentPageInputURL2015()
        {
            List<string> Urls = new();
            for (int i = 1; i <= 15; i++)
            {
                var InputString = $"https://www.onch3.co.kr/onch_hall_of_fame.html?sec=2015&page={i}&sear_txt=&type=&catef=&cates=&catet=";
                Urls.Add(InputString);
            }
            return Urls;
        }
        private List<string> CreateExcellentPageInputURL2016()
        {
            List<string> Urls = new();
            for (int i = 1; i <= 15; i++)
            {
                var InputString = $"https://www.onch3.co.kr/onch_hall_of_fame.html?sec=2016&page={i}&sear_txt=&type=&catef=&cates=&catet=";
                Urls.Add(InputString);
            }
            return Urls;
        }
        private List<string> CreateExcellentPageInputURL2017()
        {
            List<string> Urls = new();
            for (int i = 1; i <= 11; i++)
            {
                var InputString = $"https://www.onch3.co.kr/onch_hall_of_fame.html?sec=2017&page={i}&sear_txt=&type=&catef=&cates=&catet=";
                Urls.Add(InputString);
            }
            return Urls;
        }
        private List<string> CreateExcellentPageInputURL2018()
        {
            List<string> Urls = new();
            for (int i = 1; i <= 11; i++)
            {
                var InputString = $"https://www.onch3.co.kr/onch_hall_of_fame.html?sec=2018&page={i}&sear_txt=&type=&catef=&cates=&catet=";
                Urls.Add(InputString);
            }
            return Urls;
        }
        private List<string> CreateExcellentPageInputURL2019()
        {
            List<string> Urls = new();
            for (int i = 1; i <= 14; i++)
            {
                var InputString = $"https://www.onch3.co.kr/onch_hall_of_fame.html?sec=2019&page={i}&sear_txt=&type=&catef=&cates=&catet=";
                Urls.Add(InputString);
            }
            return Urls;
        }
        private List<string> CreateExcellentPageInputURL2020()
        {
            List<string> Urls = new();
            for (int i = 1; i <= 17; i++)
            {
                var InputString = $"https://www.onch3.co.kr/onch_hall_of_fame.html?sec=2020&page={i}&sear_txt=&type=&catef=&cates=&catet=";
                Urls.Add(InputString);
            }
            return Urls;
        }
        private List<string> CreateExcellentPageInputURL2021()
        {
            List<string> Urls = new();
            for (int i = 1; i <= 22; i++)
            {
                var InputString = $"https://www.onch3.co.kr/onch_hall_of_fame.html?sec=2021&page={i}&sear_txt=&type=&catef=&cates=&catet=";
                Urls.Add(InputString);
            }
            return Urls;
        }
    }
}
