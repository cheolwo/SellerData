using BusinessData.ofDataAccessLayer.ofModelExtenstions;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SellerCommon.SellerData.Model;
using SellerCommon.SellerData.ofRepository.ofMarket;
using SellerCommon.SellerData.Services;
using SellerData.ofDataAccessLayer.ofDataContext;
using SellerData.ofDataAccessLayer.ofModel;
using SellerData.ofDataAccessLayer.ofRepository.ofMarket;
using SellerData.Services.ofCollecitng;
using System.Text;

namespace SellerData.Services.ofAnalyzing
{
    public interface ICommodityNamingService
    {
        Task SetNameofCommodity(SellerSMCommodity sellerSMCommodity);
        Task SetNameofCommodityByNaverShopping(List<SellerSMCommodity> sellerSMCommodities);
        Task SetNameofCommodityByRelKwdService(SellerSMCommodity sellerSMCommodity, int FilterMonthlyMobileCount);
    }
    public class SettingNamingService : ICommodityNamingService
    {
        private readonly SellerMarketDataContext _sellerMarketDataContext;
        private readonly IOpenMarketCommonCategoryRepostiory _openMarketCommonCategoryRepostiory;
        private readonly ICommodityAnalyzedInfoRepository _commodityAnalyzedInfoRepository;
        private readonly IImportantKwdSettingService _importantKwdSettingService;
        private readonly IRelKwdSettingService _relKwdSettingService;
        private readonly ILogger<SettingNamingService> _logger;
        public SettingNamingService(SellerMarketDataContext sellerMarketDataContext,
                                                        ILogger<SettingNamingService> logger,
                                                        IOpenMarketCommonCategoryRepostiory openMarketCommonCategoryRepostiory,
                                                         ICommodityAnalyzedInfoRepository commodityAnalyzedInfoRepository,
                                                         IImportantKwdSettingService importantKwdSettingService,
                                                         IRelKwdSettingService relKwdSettingService)
        {
            _sellerMarketDataContext = sellerMarketDataContext;
            _logger = logger;
            _openMarketCommonCategoryRepostiory = openMarketCommonCategoryRepostiory;
            _commodityAnalyzedInfoRepository = commodityAnalyzedInfoRepository;
            _importantKwdSettingService = importantKwdSettingService;
            _relKwdSettingService = relKwdSettingService;
        }
        public async Task SetNameofCommodityByNaverShopping(List<SellerSMCommodity> sellerSMCommodities)
        {
            using (IWebDriver _driver = new ChromeDriver())
            {
                _driver.Navigate().GoToUrl("https://search.shopping.naver.com/search/all?query=%EB%93%B1%EC%82%B0%EA%B0%80%EB%B0%A9&cat_id=&frm=NVSHATC");
                Thread.Sleep(3000);
                int count = 1;
                int SleepCount = 0;
                foreach (var sellerSMCommodity in sellerSMCommodities)
                {
                    if (SleepCount == 10) { Thread.Sleep(10000); SleepCount = 0; }
                    _logger.LogInformation(sellerSMCommodities.Count.ToString());
                    _logger.LogInformation(count.ToString());
                    count++;
                    if (sellerSMCommodity.SellerMCommodity == null) { sellerSMCommodity.SellerMCommodity = await _sellerMarketDataContext.GetByIdAsync<SellerMCommodity>(sellerSMCommodity.SellerMCommodityId); }
                    if (sellerSMCommodity.SellerMCommodity.ImportantKwds == null || sellerSMCommodity.SellerMCommodity.ImportantKwds.Count == 0) { _logger.LogInformation("ImportantKwds 의 조건 미충족"); continue; }
                    if (sellerSMCommodity.Name != null) { _logger.LogInformation("이름이 이미 지어져있습니다."); continue; }
                    var NameofSearching = sellerSMCommodity.SellerMCommodity.ToStringImportantKwds();
                    var loginelement = _driver.FindElement(By.XPath("//*[@id='__next']/div/div[1]/div/div[2]/div/div[2]/form/fieldset/div[1]/input"));
                    while (loginelement == null) {
                        _driver.Navigate().GoToUrl("https://search.shopping.naver.com/search/all?query=%EB%93%B1%EC%82%B0%EA%B0%80%EB%B0%A9&cat_id=&frm=NVSHATC");
                        Thread.Sleep(1000);
                        loginelement = _driver.FindElement(By.XPath("//*[@id='__next']/div/div[1]/div/div[2]/div/div[2]/form/fieldset/div[1]/input")); 
                    }
                    loginelement.Clear();
                    loginelement.SendKeys(NameofSearching);
                    var ClickElement = _driver.FindElement(By.XPath("//*[@id='__next']/div/div[1]/div/div[2]/div/div[2]/form/fieldset/div[1]/button[2]"));
                    ClickElement.Click();

                    var CommodityListElement = _driver.FindElements(By.XPath("//ul[contains(@class,'list_basis')]/div/div"));
                    int cnt = 1;
                    foreach (var commodity in CommodityListElement)
                    {
                        var NameofCommodity = commodity.FindElement(By.XPath($"//div[{cnt}]/li/div/div[2]/div[1]/a"));
                        var Center = commodity.FindElement(By.XPath($"//div[{cnt}]/li/div/div[3]/div[1]/a[1]"));
                        if (!NameofCommodity.Text.Contains(Center.Text))
                        {
                            sellerSMCommodity.Name = NameofCommodity.Text;
                            _logger.LogInformation(sellerSMCommodity.Name);
                            await sellerSMCommodity.PutAsync(_sellerMarketDataContext);
                            break;
                        }
                        cnt++;
                    }
                    Thread.Sleep(1000);
                    SleepCount++;
                }
            }
        }
        public async Task SetNameofCommodityByRelKwdService(SellerSMCommodity sellerSMCommodity, int FilterMonthlyMobileCount)
        {
            // 파라미터 조건 확인
            if (sellerSMCommodity.SellerMCommodity == null) { sellerSMCommodity.SellerMCommodity = await _sellerMarketDataContext.GetByIdAsync<SellerMCommodity>(sellerSMCommodity.SellerMCommodityId); }
            var mcommodity = sellerSMCommodity.SellerMCommodity;
            if (mcommodity.ImportantKwds == null) { return; }
            try
            {
                var commoidtyAnlayzeInfos = await _commodityAnalyzedInfoRepository.GetToListByMCommodityIdAsync(mcommodity.Id);
                if (commoidtyAnlayzeInfos.Count == 0) { return; }
                //카테고리 확인
                var Category = await _openMarketCommonCategoryRepostiory.GetByCodeAsync(sellerSMCommodity.Code);
                var CommodityAnalyzedInfos = await _relKwdSettingService.GetRelKwd(mcommodity.ImportantKwds, sellerSMCommodity.ImportantKwdType, sellerSMCommodity.Id, FilterMonthlyMobileCount, 30, Category);
                if (CommodityAnalyzedInfos.Where(e => e.SeletedKeywords.Contains(mcommodity.ImportantKwds.FirstOrDefault())).ToList().Count == 0) { throw new ArgumentException("직접분석"); }
                CommodityAnalyzedInfos = FilterByAvg(CommodityAnalyzedInfos);
                await StoringAnalyzedInfo(CommodityAnalyzedInfos, mcommodity);
                var SettedCommodityAnalyzedInfos = await _commodityAnalyzedInfoRepository.GetToListByMCommodityIdAsync(mcommodity.Id);
                var name = SetNameBySettedCommodityAnalyzedInfos(SettedCommodityAnalyzedInfos, sellerSMCommodity, mcommodity.ImportantKwds.FirstOrDefault());
                sellerSMCommodity.Name = name;
                await sellerSMCommodity.PostAsync(_sellerMarketDataContext);
            }
            catch (Exception ex)
            {
                if (ex.Message.Equals("동일 카테고리 연관 키워드가 없습니다."))
                {
                    return;
                }
                if (ex.Message.Equals("직접분석"))
                {
                    return;
                }
            }
        }

        public async Task SetNameofCommodity(SellerSMCommodity sellerSMCommodity)
        {
            // 파라미터 조건 확인
            if (sellerSMCommodity.SellerMCommodity == null) { sellerSMCommodity.SellerMCommodity = await _sellerMarketDataContext.GetByIdAsync<SellerMCommodity>(sellerSMCommodity.SellerMCommodityId); }
            var mcommodity = sellerSMCommodity.SellerMCommodity;
            if (mcommodity.ImportantKwds == null) { return; }
            try
            {
                var commoidtyAnlayzeInfos = await _commodityAnalyzedInfoRepository.GetToListByMCommodityIdAsync(mcommodity.Id);
                if (commoidtyAnlayzeInfos.Count == 0) { return; }
                //카테고리 확인
                var Category = await _openMarketCommonCategoryRepostiory.GetByCodeAsync(sellerSMCommodity.Code);
                var sameCategoryCommodityAnalyzedInfos = await _importantKwdSettingService.FurtherAnalyzingImportantKwdAndReturn(sellerSMCommodity);
                sameCategoryCommodityAnalyzedInfos = FilterByAvg(sameCategoryCommodityAnalyzedInfos);
                await StoringAnalyzedInfo(sameCategoryCommodityAnalyzedInfos, mcommodity);
                var SettedCommodityAnalyzedInfos = await _commodityAnalyzedInfoRepository.GetToListByMCommodityIdAsync(mcommodity.Id);
                var name = SetNameBySettedCommodityAnalyzedInfos(SettedCommodityAnalyzedInfos, sellerSMCommodity, mcommodity.ImportantKwds.FirstOrDefault());
                sellerSMCommodity.Name = name;
                await sellerSMCommodity.PostAsync(_sellerMarketDataContext);
            }
            catch (Exception ex)
            {
                if (ex.Message.Equals("동일 카테고리 연관 키워드가 없습니다."))
                {
                    return;
                }
            }
        }
        private string SetNameBySettedCommodityAnalyzedInfos(List<CommodityAnalyzedInfo> commodityAnalyzedInfos, SellerSMCommodity sellerSMCommodity, string ImportantKwd)
        {
            List<string> NamingInfo = new();
            var name = "";
            if (sellerSMCommodity.ImportantKwdType == NamingType.CombinationForm)
            {
                FilterByCombinationType(commodityAnalyzedInfos, ImportantKwd, NamingInfo);
            }
            if (sellerSMCommodity.ImportantKwdType == NamingType.MainForm || sellerSMCommodity.ImportantKwdType == NamingType.MixForm)
            {
                FilterByMixAndMainType(commodityAnalyzedInfos, ImportantKwd, NamingInfo);
            }
            if (NamingInfo.Count > 0)
            {
                name = SetName(NamingInfo);
            }
            return name;
        }
        private string SetName(List<string> namingInfos)
        {
            StringBuilder stringBuilder = new();
            int cnt = 0;
            foreach (var value in namingInfos)
            {
                if (cnt >= 7) { break; }
                stringBuilder.Append(value);
                stringBuilder.Append(" ");
                cnt++;
            }
            return stringBuilder.ToString();
        }
        private void FilterByCombinationType(List<CommodityAnalyzedInfo> commodityAnalyzedInfos, string ImportantKwd, List<string> NamingInfo)
        {
            foreach (var info in commodityAnalyzedInfos)
            {
                if (info.SeletedKeywords == ImportantKwd) { continue; }
                var SubStringValue = info.SeletedKeywords.Substring(info.SeletedKeywords.Length - ImportantKwd.Length, ImportantKwd.Length);
                if (SubStringValue == ImportantKwd)
                {
                    var NameValue = info.SeletedKeywords.Substring(0, info.SeletedKeywords.Length - ImportantKwd.Length);
                    NamingInfo.Add(NameValue);
                }
            }
        }
        private void FilterByMixAndMainType(List<CommodityAnalyzedInfo> commodityAnalyzedInfos, string ImportantKwd, List<string> NamingInfo)
        {
            foreach (var info in commodityAnalyzedInfos)
            {
                if (info.SeletedKeywords == ImportantKwd) { continue; }
                bool IsContatin = info.SeletedKeywords.Contains(ImportantKwd);
                if (IsContatin)
                {
                    NamingInfo.Add(info.SeletedKeywords);
                }
            }
        }
        private async Task StoringAnalyzedInfo(List<CommodityAnalyzedInfo> commodityAnalyzedInfos, SellerMCommodity sellerMCommodity)
        {
            foreach (var info in commodityAnalyzedInfos)
            {
                if (info.SellerMCommodityId == null) { info.SellerMCommodityId = sellerMCommodity.Id; }
                await info.PostAsync(_sellerMarketDataContext);
            }
        }
        private List<CommodityAnalyzedInfo> FilterByCategory(List<CommodityAnalyzedInfo> sameCategoryCommodityAnalyzedInfos, OpenMarketCommonCategory Category)
        {
            return sameCategoryCommodityAnalyzedInfos = sameCategoryCommodityAnalyzedInfos.Where(
                e => e.Category1.Equals(Category.Category1) &&
                 e.Category2.Equals(Category.Category2) &&
                  e.Category3.Equals(Category.Category3) &&
                   e.Category4.Equals(Category.Category4)
                ).ToList();
        }
        private List<CommodityAnalyzedInfo> FilterByAvg(List<CommodityAnalyzedInfo> sameCategoryCommodityAnalyzedInfos)
        {
            var AvgValue = sameCategoryCommodityAnalyzedInfos.Average(e => e.MonthlyMobileQcCnt);
            var AvgInfo = sameCategoryCommodityAnalyzedInfos.Where(e => e.MonthlyMobileQcCnt <= AvgValue).ToList();
            return AvgInfo;
        }

    }
}
