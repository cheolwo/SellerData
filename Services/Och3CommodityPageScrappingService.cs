using BusinessData.ofDataAccessLayer.ofModelExtenstions;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SellerCommon.SellerData.Data;
using SellerCommon.SellerData.Model;
using SellerCommon.SellerData.ofRepository.ofMarket;
using SellerCommon.SellerData.ViewModel.ofWholeSale;
using SellerData.ofDataAccessLayer.ofDataContext;
using SellerData.Services;
using System.Text;
using System.Text.RegularExpressions;

namespace SellerCommon.SellerData.Services
{
    public class ExcellentCommoditPageXPathInfo
    {
        public string Name = "/html/body/div[1]/div[2]/div[1]/div[3]/div[2]/ul/li[3]/ul/li[1]/div[2]";
        public string KeyWords = "/html/body/div[1]/div[2]/div[1]/div[3]/div[2]/ul/li[2]/div[2]";
        public string OrderEndDate = "/html/body/div[1]/div[2]/div[1]/div[3]/div[2]/ul/li[6]/ul/li[2]";
        public string NameofDeliveryCenter = "/html/body/div[1]/div[2]/div[1]/div[3]/div[2]/ul/li[5]/ul/li[1]/div[2]";
        public string Code = "/html/body/div[1]/div[2]/div[1]/div[3]/div[2]/ul/li[3]/ul/li[2]";
        public string RepresentativeImageUrl = "/html/body/div[1]/div[2]/div[1]/div[3]/div[2]/div[2]/div[1]/img";
                                                               ///html/body/div[1]/div[2]/div[1]/div[3]/div[2]/div[2]/div[1]/img
        public string DetailImages = "/html/body/div[1]/div[2]/div[1]/div[3]/div[6]/div[2]";

    }
    public class DataCenterCommodityPageXPathInfo
    {
        public string Name = "/html/body/div/section/div/div[1]/div[3]/ul/li[2]/ul/li[2]/div[2]";
        public string KeyWords = "//div[contains(@class, 'prod_keyword')]";
        public string CategoryNumber = "//span[contains(@class, 'ss_cate_num')]";
        public string RepresentativeImageUrl = "//div[contains(@class, 'prod_detail_img')]/img";
        public string DetailImages = "//div[contains(@class, 'content_section')]";
        public string Status = "/html/body/div/section/div/div[1]/div[3]/ul/li[5]/ul/li[1]/div[2]";
        public string OrderEndDate = "/html/body/div/section/div/div[1]/div[3]/ul/li[5]/ul/li[2]/div[2]";
        public string DeliveryInfo = "/html/body/div/section/div/div[1]/div[3]/ul/li[4]/ul/li[1]/div[2]";
        public string Code = "/html/body/div/section/div/div[1]/div[3]/ul/li[2]/ul/li[3]/div[2]";
    }
    public interface IOnChannalXPathInfoService
    {
        string GetDetailCommodityXPathInfo();
        string GetAccessPageXPathInfoByCode(string code);
        CommodityPageXPathInfo GetCommodityPageXPathInfo(string url);
    }
    public class OnChannalXPathInfoService : IOnChannalXPathInfoService
    {
        public const string XPathAccessPageToDataCenterCommodity = "/html/body/div/section/div/form/ul";
        public const string XPathAccessPageToExcellentCommodity = "/html/body/div[1]/div[2]/div/div[3]/div/div[4]/div[1]/div[2]/form/ul";
        private const string XPathDetailCommodity = "/html/body/div[1]/div[2]/div[1]/div[3]/div[6]/div[2]/div/div[1]";
        private readonly IOnChannalMappingService _OnChannalMappingService;
        public OnChannalXPathInfoService(IOnChannalMappingService onChannalMappingService)
        {
            _OnChannalMappingService = onChannalMappingService;
        }
        public string GetAccessPageXPathInfoByCode(string code)
        {
            if(OnChannalPageCode.AccessPageToDataCenterCommodity.ToString().Equals(code))
            {
                return XPathAccessPageToDataCenterCommodity;
            }
            if(OnChannalPageCode.AccessPageToExcellentCommodity.ToString().Equals(code))
            {
                return XPathAccessPageToExcellentCommodity;
            }
            throw new ArgumentException(nameof(GetAccessPageXPathInfoByCode) + "Not Support Code");
        }
        public CommodityPageXPathInfo GetCommodityPageXPathInfo(string url)
        {
            if (url.Contains("onch_view"))
            {
                return _OnChannalMappingService.ExcellentPageXPathToCommodityPageXPathInfo(new ExcellentCommoditPageXPathInfo());
            }
            if (url.Contains("dbcenter_view"))
            {
                return _OnChannalMappingService.DataCenterPageXPathInfoToCommodityPageXPathInfo(new DataCenterCommodityPageXPathInfo());
            }
            throw new ArgumentNullException("Not Support Page");
        }

        public string GetDetailCommodityXPathInfo()
        {
            return XPathDetailCommodity;
        }
    }
    public enum PlatformCode { OnChannal = 0 }
    public enum OnChannalPageCode { AccessPageToExcellentCommodity = 1, AccessPageToDataCenterCommodity, ExcellentCommodityPage, DataCenterCommodityPage }
    public interface IRecognizeCommodityPageService
    {
        CommodityPageXPathInfo GetXPathInfoByUrl(string url);
        string RecognizePageByUrl(string url);
        string RecognizePlatformPage(PlatformCode code, string url);
        PlatformCode RecognizePlatformByUrl(string url);
    }
    public interface IOch3CommodityPageScrappingService
    {
        Task<MCommodityCollectInfo> Scrapping(string requestUrl);
        Task<List<SellerMCommodity>> Scrapping(string pageCode, string requestUrl);
        Task<Dictionary<string, string>> DetailCommodityInfoScrapping(string requestUrl);
        Task CommodityOptionScrapping(List<SellerMCommodity> sellerMCommodities);
        Task<List<SellerMCommodity>> ExcellentCommodityPageScrappingByYear(string Year);
        Task CommodityDeliveryInfoScrapping(List<SellerMCommodity> sellerMCommodities);
    }
    public class Och3CommodityPageScrappingService : IOch3CommodityPageScrappingService
    {
        private readonly ISellerMCommodityRepository _sellerMCommodityRepository;
        private readonly ICollectingOnChannalURLInfo _collectingOnChannalURLInfo;
        private readonly IOnChannalXPathInfoService _onChannalXPathInfoService;
        private readonly IOnChannalMappingService _OnChannalMappingService;
        private readonly IConfiguration _Configuration;
        private readonly SellerMarketDataContext _SellerMarketDataContext;
        private readonly string Id;
        private readonly string Password;
        private readonly string LoginPath;
        protected ChromeDriver _driver = null;
        public Och3CommodityPageScrappingService(SellerMarketDataContext sellerMarketDataContext, ISellerMCommodityRepository sellerMCommodityRepository,
            IOnChannalXPathInfoService onChannalXPathInfoService, 
            IOnChannalMappingService onChannalMappingService,
            IConfiguration configuration,
            ICollectingOnChannalURLInfo collectingOnChannalURLInfo)
        {
            _SellerMarketDataContext = sellerMarketDataContext;
            _OnChannalMappingService = onChannalMappingService;
            _Configuration = configuration;
            _onChannalXPathInfoService = onChannalXPathInfoService;
            _sellerMCommodityRepository = sellerMCommodityRepository;
            Id = _Configuration.GetSection("Och3")["Id"];
            Password = _Configuration.GetSection("Och3")["Password"];
            LoginPath = _Configuration.GetSection("Och3")["LoginPath"];
            _collectingOnChannalURLInfo = collectingOnChannalURLInfo;
        }
        private HttpClient _httpClient = new();
        private HtmlDocument Document = new();
        private const string Och3 = "https://www.onch3.co.kr/";
        private SellerMarket Onch3Market = new();
        public async Task<List<SellerMCommodity>> ExcellentCommodityPageScrappingByYear(string Year)
        {
            Onch3Market.Name = OnChannal.OnChannalName;
            var marketvalue = await Onch3Market.GetByNameAsync(_SellerMarketDataContext);
            if (marketvalue == null)
            {
                marketvalue = await Onch3Market.PostAsync(_SellerMarketDataContext);
            }
            // 22
            List<SellerMCommodity> sellerMCommodities = new();
            var UrlByYear = _collectingOnChannalURLInfo.GetExcellentCommodityPageUrlByYear(Year);
            List<string> Paths = new();
            using (IWebDriver _driver = new ChromeDriver())
            {
                _driver.Navigate().GoToUrl("https://www.onch3.co.kr/index.html");
                Thread.Sleep(3000);
                var loginelement = _driver.FindElement(By.XPath("/html/body/div[1]/div[1]/div[1]/div/ul[2]/li[2]/a"));
                loginelement.Click();
                _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

                Thread.Sleep(1000);
                //*[@id="login"]
                var element = _driver.FindElement(By.XPath("//*[@id='login']"));
                element.SendKeys(Id);
                //#password
                //*[@id="password"]
                element = _driver.FindElement(By.XPath("//*[@id='password']"));
                element.SendKeys(Password);

                element = _driver.FindElement(By.XPath("//*[@id='flogin']/div[5]/div[1]/button"));
                element.Click();
                foreach(var urlvalue in UrlByYear)
                {
                    _driver.Navigate().GoToUrl(urlvalue);
                    Thread.Sleep(1000);
                    var PathValues = _collectingOnChannalURLInfo.CollectCommodityURLInfo(_driver);
                    Console.WriteLine($"PathValues Count : {PathValues.Count}");
                    foreach(var value in PathValues)
                    {
                        Paths.Add(value);
                    }
                }
                CommodityPageXPathInfo commodityPageXPathInfo;
                Console.WriteLine($"Total Path Count : {Paths.Count}");
                int Count = 1;
                foreach(var PathValue in Paths)
                {
                    Console.WriteLine($"Count :  {Count}");
                    commodityPageXPathInfo = _onChannalXPathInfoService.GetCommodityPageXPathInfo(PathValue);
                    _driver.Navigate().GoToUrl(PathValue);
                    // 옵션
                    // 상품정보
                    // 디테일정보
                    var Options = CommodityOptionInfoBySelenium(_driver);
                    var CommodityCollectInfo = CommodityCollectInfoScrapping(commodityPageXPathInfo, _driver);
                    var DetailCommodityCollectInfo = DetailCommodityInfoScrapping(_driver);

                    var SellerMCommodity = _OnChannalMappingService.MCommodityCollectInfoToSellerMCommodity(CommodityCollectInfo);
                    SellerMCommodity.CommodityOptions = Options;
                    SellerMCommodity.DetailCommodityInfo = DetailCommodityCollectInfo;
                    SellerMCommodity.CommodityPageUrl = PathValue;
                    SellerMCommodity.CenterId = marketvalue.Id;
                    var value = await _sellerMCommodityRepository.GetByCommodityPageUrl(PathValue);
                    if(value == null)
                    {
                        await SellerMCommodity.PostAsync(_SellerMarketDataContext);
                    }
                    Count++;
                }
                return sellerMCommodities;
            }
        }
        public async Task CommodityOptionScrapping(List<SellerMCommodity> sellerMCommodities)
        {
            using (IWebDriver _driver = new ChromeDriver())
            {
                _driver.Navigate().GoToUrl("https://www.onch3.co.kr/index.html");
                Thread.Sleep(3000);
                var loginelement = _driver.FindElement(By.XPath("/html/body/div[2]/div[1]/div[1]/div/ul[2]/li[2]/a"));
                loginelement.Click();
                _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

                Thread.Sleep(1000);
                //*[@id="login"]
                var element = _driver.FindElement(By.XPath("//*[@id='login']"));
                element.SendKeys(Id);
                //#password
                //*[@id="password"]
                element = _driver.FindElement(By.XPath("//*[@id='password']"));
                element.SendKeys(Password);

                element = _driver.FindElement(By.XPath("//*[@id='flogin']/div[5]/div[1]/button"));
                element.Click();
                
                foreach(var commodity in sellerMCommodities)
                {
                    var Och3RequestUrl = Och3 + commodity.CommodityPageUrl;
                    _driver.Navigate().GoToUrl(Och3RequestUrl);
                    Thread.Sleep(1000);
                    var options = CommodityOptionInfoBySelenium(_driver);
                    commodity.CommodityOptions = options;
                    await _sellerMCommodityRepository.UpdateAsync(commodity);
                }
            }
        }
        public virtual async Task CommodityDeliveryInfoScrapping(List<SellerMCommodity> sellerMCommodities)
        {
            using (IWebDriver _driver = new ChromeDriver())
            {
                _driver.Navigate().GoToUrl("https://www.onch3.co.kr/index.html");
                Thread.Sleep(3000);
                var loginelement = _driver.FindElement(By.XPath("/html/body/div[2]/div[1]/div[1]/div/ul[2]/li[2]/a"));
                loginelement.Click();
                _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

                Thread.Sleep(1000);
                //*[@id="login"]
                var element = _driver.FindElement(By.XPath("//*[@id='login']"));
                element.SendKeys(Id);
                //#password
                //*[@id="password"]
                element = _driver.FindElement(By.XPath("//*[@id='password']"));
                element.SendKeys(Password);

                element = _driver.FindElement(By.XPath("//*[@id='flogin']/div[5]/div[1]/button"));
                element.Click();
                foreach(var commodity in sellerMCommodities)
                {
                    var Och3RequestUrl = commodity.CommodityPageUrl;
                    _driver.Navigate().GoToUrl(Och3RequestUrl);
                    Thread.Sleep(1000);
                    element = _driver.FindElement(By.XPath("//div[contains(@class, 'prod_detail_content')]"));
                    Console.WriteLine(element.Text);
                    commodity.DeliveryContent = Regex.Replace(element.Text, @"\t|\n|\r", "");
                    await commodity.PutAsync(_SellerMarketDataContext);
                }
            }
        }
      
        public virtual Dictionary<string, string> DetailCommodityInfoScrapping(IWebDriver _driver)
        {
            Dictionary<string, string> DicDetailCommodityInfos = new();
            var titleNodes = _driver.FindElements(By.XPath("//div[contains(@class, 'pg_title')]"));
            var contentNodes = _driver.FindElements(By.XPath("//div[contains(@class, 'pg_content')]"));
            int count = 0;
            if (titleNodes != null)
            {
                foreach (var titleNode in titleNodes)
                {
                    var value = DicDetailCommodityInfos.Keys.FirstOrDefault(e => e.Equals(titleNode.GetAttribute("innerText")));
                    if (value != null) { continue; }
                    DicDetailCommodityInfos.Add(titleNode.GetAttribute("innerText"), contentNodes[count].GetAttribute("innerText"));
                    count++;
                }
            }
            return DicDetailCommodityInfos;
        }
        public async Task<Dictionary<string, string>> DetailCommodityInfoScrapping(string requestUrl)
        {
            var Och3RequestUrl = Och3 + requestUrl;
            var ResultStream = await _httpClient.GetStreamAsync(Och3RequestUrl);
            Document.Load(ResultStream, Encoding.Default);
            var CollectInfo = CollectDetailCommodityInfo(Document);
            return CollectInfo;
        }
        private List<CommodityOption> CommodityOptionInfoBySelenium(IWebDriver driver)
        {
            var detail_page_names = driver.FindElements(By.XPath("//span[contains(@class, 'detail_page_name')]"));
            var detail_page_price_1s = driver.FindElements(By.XPath("//span[contains(@class, 'detail_page_price_1')]"));
            var detail_page_price_2s = driver.FindElements(By.XPath("//span[contains(@class, 'detail_page_price_2')]"));
            var detail_page_price_3s = driver.FindElements(By.XPath("//span[contains(@class, 'detail_page_price_3')]"));
            int cnt = 0;
            List<CommodityOption> commodityOptions = new();
            foreach (var node in detail_page_names)
            {
                CommodityOption commodityOption = new();
                if (cnt == 0) { cnt++; continue; }
                commodityOption.OptionName = node.GetAttribute("innerText");
                commodityOption.OptionName = Regex.Replace(commodityOption.OptionName, @"\t|\n|\r", "");

                commodityOption.SalesType = detail_page_price_1s[cnt].GetAttribute("innerText");
                commodityOption.SalesType = Regex.Replace(commodityOption.SalesType, @"\t|\n|\r", "");

                commodityOption.ConsumerPrice = detail_page_price_2s[cnt].GetAttribute("innerText");
                commodityOption.ConsumerPrice = Regex.Replace(commodityOption.ConsumerPrice, @"\t|\n|\r", "");

                commodityOption.SellerPrice = detail_page_price_3s[cnt].GetAttribute("innerText");
                commodityOption.SellerPrice = Regex.Replace(commodityOption.SellerPrice, @"\t|\n|\r", "");
                cnt++;

                commodityOptions.Add(commodityOption);
            }
            return commodityOptions;
        }
        private Dictionary<string, string> CollectDetailCommodityInfo(HtmlDocument document)
        {
            Dictionary<string, string> DicDetailCommodityInfos = new();
            var titleNodes = document.DocumentNode.SelectNodes("//div[contains(@class, 'pg_title')]");
            var contentNodes = document.DocumentNode.SelectNodes("//div[contains(@class, 'pg_content')]");

            int count = 0;
            if(titleNodes != null)
            {
                foreach (var titleNode in titleNodes)
                {
                    var value = DicDetailCommodityInfos.Keys.FirstOrDefault(e => e.Equals(titleNode.InnerText));
                    if(value != null) { continue; }
                    DicDetailCommodityInfos.Add(titleNode.InnerText, contentNodes[count].InnerText);
                    count++;
                }
            }
            return DicDetailCommodityInfos;
        }
        // 스크래핑
        public async Task<MCommodityCollectInfo> Scrapping(string requestUrl)
        {
            var XPathInfo = _onChannalXPathInfoService.GetCommodityPageXPathInfo(requestUrl);
            var Och3RequestUrl = Och3 + requestUrl;
            var ResultStream = await _httpClient.GetStreamAsync(Och3RequestUrl);
            Document.Load(ResultStream, Encoding.Default);

            var CollectInfo = CollectXPathInfo(XPathInfo);
            return CollectInfo;
        }
        public virtual MCommodityCollectInfo CommodityCollectInfoScrapping(CommodityPageXPathInfo xPathInfo, IWebDriver _driver)
        {
            var NameNode = _driver.FindElement(By.XPath(xPathInfo.Name));
            var CodeNode = _driver.FindElement(By.XPath(xPathInfo.Code));
            var KeywordNode = _driver.FindElement(By.XPath(xPathInfo.KeyWords));
            var OrderEndDateNode = _driver.FindElement(By.XPath(xPathInfo.OrderEndDate));
            var NameofDeliveryCenterNode = _driver.FindElement(By.XPath(xPathInfo.DeliveryInfo));
            var RepresentativeImageNode = _driver.FindElement(By.XPath(xPathInfo.RepresentativeImageUrl));
            var DetailImageNode = _driver.FindElements(By.XPath("//div[contains(@class, 'content_section')]/p/img"));

            MCommodityCollectInfo mCommodityCollectInfo = new();
            var Name = NameNode.GetAttribute("innerText");
            var Code = CodeNode.GetAttribute("innerText");
            var Keyword = KeywordNode.GetAttribute("innerText");
            var OrderEndDate = OrderEndDateNode.GetAttribute("innerText");
            var DeliveryCenter = NameofDeliveryCenterNode.GetAttribute("innerText");
            var srcRepresentativeImage = RepresentativeImageNode.GetAttribute("src");

            Name = Regex.Replace(Name, @"\t|\n|\r", "");
            Code = Regex.Replace(Code, @"\t|\n|\r", "");
            Keyword = Regex.Replace(Keyword, @"\t|\n|\r", "");
            OrderEndDate = Regex.Replace(OrderEndDate, @"\t|\n|\r", "");
            DeliveryCenter = Regex.Replace(DeliveryCenter, @"\t|\n|\r", "");

            mCommodityCollectInfo.Name = Name;
            int index = 4;
            StringBuilder CodeBuilder = new();
            do
            {
                CodeBuilder.Append(Code[index]);
                index++;
            }
            while (Code[index] != ' ');
            mCommodityCollectInfo.Code = CodeBuilder.ToString();
            mCommodityCollectInfo.KeyWords = Keyword;
            StringBuilder OrderDataBuilder = new();
            int OrderDataBuilderIndex = 4;
            do
            {
                OrderDataBuilder.Append(OrderEndDate[OrderDataBuilderIndex]);
                OrderDataBuilderIndex++;
            } while (OrderDataBuilderIndex < OrderEndDate.Length);
            mCommodityCollectInfo.OrderEndDate = OrderDataBuilder.ToString();
            mCommodityCollectInfo.DeliveryInfo = DeliveryCenter;
            mCommodityCollectInfo.RepresentativeImageUrl = srcRepresentativeImage;
            foreach(var detailImageValue in DetailImageNode)
            {
                var srcvalue = detailImageValue.GetAttribute("src");
                if(srcvalue != null)
                {
                    Console.WriteLine(srcvalue);
                    mCommodityCollectInfo.DetailImages.Add(srcvalue);
                }
            }
            return mCommodityCollectInfo;
        }
        private MCommodityCollectInfo CollectXPathInfo(CommodityPageXPathInfo XPathInfo)
        {
            MCommodityCollectInfo mCommodityCollectInfo = new();
            HtmlNode NameNode = Document.DocumentNode.SelectSingleNode(XPathInfo.Name);
            HtmlNode CodeNode = Document.DocumentNode.SelectSingleNode(XPathInfo.Code);
            HtmlNode KeyWordsNode = Document.DocumentNode.SelectSingleNode(XPathInfo.KeyWords);
            HtmlNode OrderEndDateNode = Document.DocumentNode.SelectSingleNode(XPathInfo.OrderEndDate);
            HtmlNode DeliveryCenterNode = Document.DocumentNode.SelectSingleNode(XPathInfo.DeliveryInfo);
            HtmlNode RepresentativeImageNode = Document.DocumentNode.SelectSingleNode(XPathInfo.RepresentativeImageUrl);
            HtmlNode DetailImageNode = Document.DocumentNode.SelectSingleNode(XPathInfo.DetailImages);
            if(DetailImageNode == null)
            {
                XPathInfo.DetailImages = "/html/body/div[1]/div[2]/div[1]/div[3]/div[7]/div[2]";
                DetailImageNode = Document.DocumentNode.SelectSingleNode(XPathInfo.DetailImages);
            }
            var Nametext = NameNode.InnerText;
            string NameReplacement = Regex.Replace(Nametext, @"\t|\n|\r", "");
            mCommodityCollectInfo.Name = NameReplacement;

            var CodeText = CodeNode.InnerText;
            string CodeReplacement = Regex.Replace(CodeText, @"\t|\n|\r", "");
            int index = 4;
            StringBuilder CodeBuilder = new();

            do
            {
                CodeBuilder.Append(CodeReplacement[index]);
                index++;
            }
            while (CodeReplacement[index] != ' ');
            mCommodityCollectInfo.Code = CodeBuilder.ToString();

            var KeywordText = KeyWordsNode.InnerText;
            string KeywordReplacement = Regex.Replace(KeywordText, @"\t|\n|\r", "");
            mCommodityCollectInfo.KeyWords = KeywordReplacement;

            var OrderEndDateText = OrderEndDateNode.InnerText;
            string OrderEndDateReplacement = Regex.Replace(OrderEndDateText, @"\t|\n|\r", "");

            StringBuilder OrderDataBuilder = new();
            int OrderDataBuilderIndex = 4;
            do
            {
                OrderDataBuilder.Append(OrderEndDateReplacement[OrderDataBuilderIndex]);
                OrderDataBuilderIndex++;
            } while (OrderDataBuilderIndex < OrderEndDateReplacement.Length);
            mCommodityCollectInfo.OrderEndDate = OrderDataBuilder.ToString();

            var DeliveryCenterText = DeliveryCenterNode.InnerText;
            string DeliveryCenterReplacement = Regex.Replace(DeliveryCenterText, @"\t|\n|\r", "");
            mCommodityCollectInfo.DeliveryInfo = DeliveryCenterReplacement;

            var src = RepresentativeImageNode.GetAttributeValue("src", "");
            mCommodityCollectInfo.RepresentativeImageUrl = src;

            var DetailChiledNode = DetailImageNode.SelectNodes("//img");
            foreach (var chiledNode in DetailChiledNode)
            {
                var srcvalue = chiledNode.GetAttributeValue("src", "");
                if (srcvalue.Contains("http"))
                {
                    if(srcvalue.Contains("svg")) { continue; }
                    if (srcvalue.Contains(":8080")) { continue; }
                    var srcReplacement = Regex.Replace(srcvalue, @"\\", "");
                    var duplicatedValue = mCommodityCollectInfo.DetailImages.Find(e => e.Equals(srcReplacement));
                    if(duplicatedValue == null)
                    {
                        Console.WriteLine(srcReplacement);
                        mCommodityCollectInfo.DetailImages.Add(srcReplacement);
                    }
                }
            }
            return mCommodityCollectInfo;
        }

        /// <summary>
        /// 1. Onchannal Main Page로 Selenium을 작동하는 단계
        /// 2. Main Page에서 로그인 페이지로 이동하는 단계
        /// 3. 상기 페이지에서 Id 및 패스워드를 입력하여 로그인하는 단계
        /// 4. 입력 URL 로 이동하는 단계
        /// 5. 스크래핑하는 단계를 포함한다.
        /// </summary>
        /// <param name="requestUrl"></param>
        /// <returns></returns>
        public async Task<List<SellerMCommodity>> Scrapping(string pageCode, string requestUrl)
        {
            List<SellerMCommodity> sellerMCommodities = new();
            if(pageCode.Equals(OnChannalPageCode.AccessPageToExcellentCommodity.ToString()))
            {
                return sellerMCommodities;
            }
            if(pageCode.Equals(OnChannalPageCode.AccessPageToDataCenterCommodity.ToString()))
            {
                return sellerMCommodities;
            }
            if(pageCode.Equals(OnChannalPageCode.ExcellentCommodityPage.ToString()))
            {
                return sellerMCommodities;
            }
            if(pageCode.Equals(OnChannalPageCode.DataCenterCommodityPage.ToString()))
            {
                return sellerMCommodities;
            }
            throw new ArgumentException("Not Support PageCode");
        }
        private async Task<List<SellerMCommodity>> ScrappingAccessPageofExcellentCommodity(string requestUrl)
        {
            List<SellerMCommodity> sellerMCommodities = new();
            return sellerMCommodities;
        }
        private async Task<List<SellerMCommodity>> ScrappingAccesPageofDataCenterCommodity(string requestUrl)
        {
            List<SellerMCommodity> sellerMCommodities = new();
            return sellerMCommodities;
        }
        private async Task<List<SellerMCommodity>> ScrappingExcellntCommodityPage(string requestUrl)
        {
            List<SellerMCommodity> sellerMCommodities = new();
            return sellerMCommodities;
        }
        private async Task<List<SellerMCommodity>> ScrappingDataCenterCommodityPage(string requestUrl)
        {
            List<SellerMCommodity> sellerMCommodities = new();
            return sellerMCommodities;
        }
    }
}