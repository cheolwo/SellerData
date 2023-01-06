using BusinessData.ofDataAccessLayer.ofModelExtenstions;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SellerCommon.SellerData.Data;
using SellerCommon.SellerData.Model;
using SellerCommon.SellerData.ofRepository.ofMarket;
using SellerCommon.SellerData.Services;
using SellerData.ofDataAccessLayer.ofDataContext;
using SellerData.Services.ofConfiguring;
using System.Text;
using System.Text.RegularExpressions;

namespace SellerData.Services.ofCollecitng
{
    public interface ICommodityScarppingService
    {
        Task Scrapping();
        Task Scrapping(string searchKwd);
        Task CollectingAccessPath(string searchKwd);
        Task DetailImageUrlScrapping(List<SellerMCommodity> sellerMCommodities);

    }
    public class Och3DataCenterCommodityScrappingService : ICommodityScarppingService
    {
        private readonly SellerMarketDataContext _sellerMarketDataContext;
        private readonly ISellerMCommodityRepository _sellerMCommodityRepositoiry;
        private readonly IOnChannalXPathInfoService _onChannalXPathInfoService;
        private readonly IOnChannalMappingService _OnChannalMappingService;
        private readonly IConfiguration _configuration;
        private readonly ICommodityOriginCodeService _commodityOriginCodeService;
        private readonly ICommodityDeliveryCodeService _commodityDeliveryCodeService;
        private readonly ICommodityCategoryCodeService _commodityCategoryCodeService;
        private readonly string Id;
        private readonly string Password;
        private readonly string LoginPath;
        protected ChromeDriver _driver = null;
        public List<string> AccessPaths = new();

        public Och3DataCenterCommodityScrappingService(SellerMarketDataContext sellerMarketDataContext,
            IConfiguration configuration,
            ISellerMCommodityRepository sellerMCommodityRepository,
            IOnChannalXPathInfoService onChannalXPathInfoService,
            IOnChannalMappingService onChannalMappingService,
            ICommodityOriginCodeService commodityOriginCodeService,
            ICommodityDeliveryCodeService commodityDeliveryCodeService,
            ICommodityCategoryCodeService commodityCategoryCodeService)
        {
            _sellerMarketDataContext = sellerMarketDataContext;
            _sellerMCommodityRepositoiry = sellerMCommodityRepository;
            _configuration = configuration;
            _onChannalXPathInfoService = onChannalXPathInfoService;
            _OnChannalMappingService = onChannalMappingService;
            Id = _configuration.GetSection("Och3")["Id"];
            Password = _configuration.GetSection("Och3")["Password"];
            LoginPath = _configuration.GetSection("Och3")["LoginPath"];
            _commodityCategoryCodeService = commodityCategoryCodeService;
            _commodityDeliveryCodeService = commodityDeliveryCodeService;
            _commodityOriginCodeService = commodityOriginCodeService;
        }
        public async Task Scrapping(string searchKwd)
        {
            throw new NotImplementedException();
        }
        public async Task Scrapping()
        {
            var marketvalue = await _sellerMarketDataContext.GetByNameAsync<SellerMarket>("Onch3");
            var SMCommodities = await _sellerMCommodityRepositoiry.GetToListByNoNameAsync();
            using (IWebDriver _driver = new ChromeDriver())
            {
                _driver.Navigate().GoToUrl("https://www.onch3.co.kr/index.html");
                Thread.Sleep(3000);
                var loginelement = _driver.FindElement(By.XPath("/html/body/div[1]/div[1]/div[1]/div/ul[2]/li[2]/a"));
                loginelement.Click();
                _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

                Thread.Sleep(1000);
                ////*[@id="login"]
                var element = _driver.FindElement(By.XPath("//*[@id='login']"));
                element.SendKeys(Id);
                element = _driver.FindElement(By.XPath("//*[@id='password']"));
                element.SendKeys(Password);

                element = _driver.FindElement(By.XPath("//*[@id='flogin']/div[5]/div[1]/button"));
                element.Click();
                CommodityPageXPathInfo commodityPageXPathInfo;
                foreach (var value in SMCommodities)
                {
                    try
                    {
                        _driver.Navigate().GoToUrl(value.CommodityPageUrl);
                        Thread.Sleep(1000);
                        commodityPageXPathInfo = _onChannalXPathInfoService.GetCommodityPageXPathInfo(value.CommodityPageUrl);
                        var CommodityCollectInfo = await CommodityCollectInfoScrapping(commodityPageXPathInfo, value, _driver);
                        var DetailCommodityCollectInfo = DetailCommodityInfoScrapping(_driver);
                        var Options = CommodityOptionInfoBySelenium(_driver);

                        value.DetailImages = CommodityCollectInfo.DetailImages;
                        value.DeliveryContent = CommodityCollectInfo.DeliveryInfo;
                        value.Code = CommodityCollectInfo.Code;
                        value.Keywords = CommodityCollectInfo.KeyWords;
                        value.Name = CommodityCollectInfo.Name;
                        value.CommodityOptions = Options;
                        value.CreateTime = DateTime.Now;
                        value.DetailCommodityInfo = DetailCommodityCollectInfo;
                        value.CenterId = marketvalue.Id;
                        value.RepresentativeImageUrl = CommodityCollectInfo.RepresentativeImageUrl;
                        await value.PutAsync(_sellerMarketDataContext);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        continue;
                    }
                }
            }
        }
        public async Task DetailImageUrlScrapping(List<SellerMCommodity> sellerMCommodities)
        {
            using (IWebDriver _driver = new ChromeDriver())
            {
                _driver.Navigate().GoToUrl("https://www.onch3.co.kr/index.html");
                Thread.Sleep(3000);
                var loginelement = _driver.FindElement(By.XPath("/html/body/div[1]/div[1]/div[1]/div/ul[2]/li[2]/a"));
                loginelement.Click();
                _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

                Thread.Sleep(1000);
                ////*[@id="login"]
                var element = _driver.FindElement(By.XPath("//*[@id='login']"));
                element.SendKeys(Id);
                element = _driver.FindElement(By.XPath("//*[@id='password']"));
                element.SendKeys(Password);

                element = _driver.FindElement(By.XPath("//*[@id='flogin']/div[5]/div[1]/button"));
                element.Click();
                foreach(var commodity in sellerMCommodities)
                {
                    if(commodity.DetailImages.Count > 0) { continue ; }
                    if(commodity.CommodityPageUrl == null) { continue; }
                    _driver.Navigate().GoToUrl(commodity.CommodityPageUrl);
                    Thread.Sleep(1000);
                    var node = _driver.FindElement(By.XPath("//div[contains(@class, 'content_section')]"));
                    var imgtags = node.FindElements(By.TagName("img")); 
                    foreach(var img in imgtags)
                    {
                        var srcvalue = img.GetAttribute("src");
                        Console.WriteLine(srcvalue);
                        commodity.DetailImages.Add(srcvalue);
                    }
                    await commodity.PutAsync(_sellerMarketDataContext);
                }
            }


        }
        public async Task<MCommodityCollectInfo> CommodityCollectInfoScrapping(CommodityPageXPathInfo xPathInfo, SellerMCommodity sellerMCommodity, IWebDriver _driver)
        {
            var NameNode = _driver.FindElement(By.XPath(xPathInfo.Name));
            var KeywordNode = _driver.FindElement(By.XPath(xPathInfo.KeyWords));
            var RepresentativeImageNode = _driver.FindElement(By.XPath(xPathInfo.RepresentativeImageUrl));
            var DetailImageNodes = _driver.FindElement(By.XPath(xPathInfo.DetailImages));
            var Status = _driver.FindElement(By.XPath(xPathInfo.Status));
            var CodeNode = _driver.FindElement(By.XPath(xPathInfo.Code));
            var OrderEndDateNode = _driver.FindElement(By.XPath(xPathInfo.OrderEndDate));
            var DeliveryInfoNode = _driver.FindElement(By.XPath(xPathInfo.DeliveryInfo));

            if (!Status.GetAttribute("innerText").Equals("정상판매"))
            {
                await sellerMCommodity.DeleteAsync(_sellerMarketDataContext);
                throw new Exception("판매X");
            }
            MCommodityCollectInfo mCommodityCollectInfo = new();
            var Name = NameNode.GetAttribute("innerText");
            var Keyword = KeywordNode.GetAttribute("innerText");
            var srcRepresentativeImage = RepresentativeImageNode.GetAttribute("src");
            var Code = CodeNode.GetAttribute("innerText");
            var OrderEndDate = OrderEndDateNode.GetAttribute("innerText");
            var DeliveryInfo = DeliveryInfoNode.GetAttribute("innerText");

            Name = Regex.Replace(Name, @"\t|\n|\r", "");
            Code = Regex.Replace(Code, @"\t|\n|\r", "");
            Keyword = Regex.Replace(Keyword, @"\t|\n|\r", "");
            OrderEndDate = Regex.Replace(OrderEndDate, @"\t|\n|\r", "");
            DeliveryInfo = Regex.Replace(DeliveryInfo, @"\t|\n|\r", "");

            mCommodityCollectInfo.Name = Name;
            //int index = 4;
            //StringBuilder CodeBuilder = new();
            //do
            //{
            //    CodeBuilder.Append(Code[index]);
            //    index++;
            //}
            //while (Code[index] != ' ');
            mCommodityCollectInfo.Code = Code;
            mCommodityCollectInfo.KeyWords = Keyword;
            mCommodityCollectInfo.OrderEndDate = OrderEndDate;
            mCommodityCollectInfo.DeliveryInfo = DeliveryInfo;
            mCommodityCollectInfo.RepresentativeImageUrl = srcRepresentativeImage;
            var imgtags = DetailImageNodes.FindElements(By.TagName("img"));
            foreach (var img in imgtags)
            {
                var srcvalue = img.GetAttribute("src");
                Console.WriteLine(srcvalue);
                mCommodityCollectInfo.DetailImages.Add(srcvalue);
            }
            return mCommodityCollectInfo;
        }
        public Dictionary<string, string> DetailCommodityInfoScrapping(IWebDriver _driver)
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
        public string CommodityDeliveryInfoScrapping(IWebDriver _driver)
        {
            var element = _driver.FindElement(By.XPath("//div[contains(@class, 'prod_detail_content')]"));
            Console.WriteLine(element.Text);
            var Result = Regex.Replace(element.Text, @"\t|\n|\r", "");
            return Result;
        }

        public async Task CollectingAccessPath(string searchKwd)
        {
            SellerMarket sellerMarket = await _sellerMarketDataContext.GetByNameAsync<SellerMarket>("Onch3");
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
                _driver.Navigate().GoToUrl("https://www.onch3.co.kr/dbcenter_renewal/excel_download_center.html?cate=rule1");
                SearchByKwd(searchKwd, _driver);
                for (int i = 100; i <= 700; i++)
                {
                    string Url = CollectingUrl(i);
                    _driver.Navigate().GoToUrl(Url);
                    Collecting(_driver);
                    await StoringCollectedAccessPaths(AccessPaths, sellerMarket);
                }
            }
        }
        private string CollectingUrl(int number)
        {
            return $"https://www.onch3.co.kr/dbcenter_renewal/excel_download_center.html?cate=rule1&cate_f=&cate_s=&cate_t=&cate_fr=&cate_nm=&stxt=&page={number}";
        }

        private void SearchByKwd(string searchKwd, IWebDriver _driver)
        {
            // 검색창에 kwd 입력
            // 클릭
            // /html/body/header/div[2]/div/div[2]/div[1]/div/input
            if (searchKwd == null || searchKwd == "")
            {
                return;
            }
            var element = _driver.FindElement(By.XPath("/html/body/header/div[2]/div/div[2]/div[1]/div/input"));
            element.SendKeys(searchKwd);
            element = _driver.FindElement(By.XPath("/html/body/header/div[2]/div/div[2]/div[1]/a"));
            element.Click();
            Thread.Sleep(1000);
        }
        private void Collecting(IWebDriver _driver)
        {
            var ImgElements = _driver.FindElements(By.XPath("//dd[contains(@class, 'product_img')]"));
            if (ImgElements != null)
            {
                foreach (var element in ImgElements)
                {
                    var AElement = element.FindElement(By.XPath("a"));
                    var Secondhref = AElement.GetAttribute("href");
                    AccessPaths.Add(Secondhref);
                }
            }
        }
        private async Task StoringCollectedAccessPaths(List<string> AccessPaths, SellerMarket sellerMarket)
        {
            SellerMCommodity sellerMCommodity = new();
            foreach (var value in AccessPaths)
            {
                var FindValue = await _sellerMCommodityRepositoiry.GetByCommodityPageUrl(value);
                if (FindValue == null)
                {
                    sellerMCommodity.CommodityPageUrl = value;
                    await sellerMCommodity.PostAsync(_sellerMarketDataContext);
                    sellerMCommodity.Id = null;
                }
            }
        }
    }
}
