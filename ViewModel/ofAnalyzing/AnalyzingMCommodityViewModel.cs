using BusinessData.ofDataAccessLayer.ofModelExtenstions;
using Microsoft.Extensions.Logging;
using SellerCommon.SellerData.Model;
using SellerCommon.SellerData.ofPresentationLayer.ofExtensions;
using SellerCommon.SellerData.ofRepository.ofMarket;
using SellerCommon.SellerData.Services;
using SellerCommon.SellerData.ViewModel.ofWholeSale;
using SellerData.ofDataAccessLayer.ofDataContext;
using SellerData.Services.ofCollecitng;
using SellerData.ViewModel.Common;
using System.Text;

namespace SellerData.ViewModel.ofAnalyzing
{
    public class AnalyzingKeyword
    {
        public string keyword { get; set; }
        public bool IsSelected { get; set; }
    }
    public class AnalyzingMCommodityViewModel : SellerObservableObject
    {
        private readonly ISellerMCommodityRepository _SellerMCommodityRepository;
        private readonly ISellerMarketRepository _SellerMarketRepository;
        private readonly ICommodityAnalyzedInfoRepository _CommodityAnalyzedInfoRepository;
        private readonly INaverSearchShoppingAPIService _NaverSearchShoppingAPIService;
        private readonly INaverSerachRelKwdAPIService _NaverSerachRelKwdAPIService;
        private readonly IImportantKwdSettingService _ImportantKwdSettingService;
        private readonly ILogger<AnalyzingMCommodityViewModel> _logger;
        public AnalyzingMCommodityViewModel(SellerMarketDataContext sellerMarketDataContext,
            ISellerMCommodityRepository sellerMCommodityRepository,
            ISellerMarketRepository sellerMarketRepository,
            ICommodityAnalyzedInfoRepository commodityAnalyzedInfoRepository,
            INaverSearchShoppingAPIService naverSearchShoppingAPIService,
            INaverSerachRelKwdAPIService naverSerachRelKwdAPIService,
            IImportantKwdSettingService importantKwdSettingService,
            ILogger<AnalyzingMCommodityViewModel> logger)
            : base(sellerMarketDataContext)
        {
            _SellerMCommodityRepository = sellerMCommodityRepository;
            _SellerMarketRepository = sellerMarketRepository;
            _CommodityAnalyzedInfoRepository = commodityAnalyzedInfoRepository;
            _NaverSearchShoppingAPIService = naverSearchShoppingAPIService;
            _NaverSerachRelKwdAPIService = naverSerachRelKwdAPIService;
            _ImportantKwdSettingService = importantKwdSettingService;
            _logger = logger;
        }

        public SellerMarket SellerMarket = new();
        public List<CommodityAnalyzedInfo> CommodityAnalyzedInfos = new();
        public List<SellerMCommodity> SellerMCommodities = new();
        public List<SellerMCommodity> NoDataCommodities = new();
        public SellerMCommodity SelectedMCommodity = new();
        public List<string> SplitKeywords = new();
        public List<string> RelKwdsofImportantKwd = new();
        public List<string> CandidateKwds = new();
        public Dictionary<long, string> DicSelectedAnalyzingKwd = new();
        public Dictionary<string, List<CommodityAnalyzedInfo>> DicSameCategoryCommodityAnalyzeInfos = new();
        public NaverRelKwdStatResult naverRelKwdStatResult = new();
        private CommodityAnalyzedInfo CommodityAnalyzedInfo = new();
        public List<CommodityAnalyzedInfo> TotalCommodityCategoyNullCommodityAnalyzedInfos = new();

        private bool _IsSelect;
        public bool IsSelect
        {
            get => _IsSelect;
            set => SetProperty(ref _IsSelect, value);
        }
        private string _SeletedCode;
        public string SelectedCode
        {
            get => _SeletedCode;
            set => SetProperty(ref _SeletedCode, value);
        }
        private string _ImportantKeyword;
        public string ImportantKeyword
        {
            get => _ImportantKeyword;
            set => SetProperty(ref _ImportantKeyword, value);
        }
        private int _FilterMonthlyCount;
        public int FilterMonthlyCount
        {
            get => _FilterMonthlyCount;
            set => SetProperty(ref _FilterMonthlyCount, value);
        }
        private bool _IsSetFilterCount;
        public bool IsSetFilterCount
        {
            get => _IsSetFilterCount;
            set => SetProperty(ref _IsSetFilterCount, value);
        }
        private string _SelectedKwds;
        public string SelectedKwds
        {
            get => _SelectedKwds;
            set => SetProperty(ref _SelectedKwds, value);
        }
        private int _FilterByCompetiveIndex;
        public int FilterByCompetiveIndex
        {
            get => _FilterByCompetiveIndex;
            set => SetProperty(ref _FilterByCompetiveIndex, value);
        }
        private int _NotAnalyzingCommodityCount;
        public int NotAnalyzingCommodityCount
        {
            get => _NotAnalyzingCommodityCount;
            set => SetProperty(ref _NotAnalyzingCommodityCount, value);
        }
        public override async Task InitLoadAsync()
        {
            if (!IsLoad)
            {
                IsBusy = true;
                SellerMarket = await _SellerMarketRepository.GetByNameAsync(OnChannal.OnChannalName);
                SellerMCommodities = await _SellerMCommodityRepository.GetToListBySellerMarketIdWithSMCommodity(SellerMarket.Id);
                NoDataCommodities = SellerMCommodities.Where(e => e.Keywords == null).ToList();
                TotalCommodityCategoyNullCommodityAnalyzedInfos = await _CommodityAnalyzedInfoRepository.GetToListofTotalCommodityIsZeroAsync();
                FilterByCompetiveIndex = 50;
                FilterMonthlyCount = 0;
                NotAnalyzingCommodityCount = SellerMCommodities.Count;
                IsBusy = false;
            }
        }
        public async Task SettingCommodityAnalyzeInfo()
        {
            List<CommodityAnalyzedInfo> ImportantKwdCommodityAnalyzeInfo = new();
            foreach (var commodity in SellerMCommodities)
            {
                if (commodity.ImportantKwds.Count == 0) { continue; }
                var CurrentCommodityAnalyzeInfos = await _CommodityAnalyzedInfoRepository.GetToListByMCommodityIdAsync(commodity.Id);
                if (CurrentCommodityAnalyzeInfos.Count == 0)
                {
                    var HintKwds = _ImportantKwdSettingService.ConfigureHintKwds(commodity.ImportantKwds);
                    var Result = await _NaverSerachRelKwdAPIService.GetRelKwdStat(HintKwds);
                    ConfigureRelKwdInfos(Result);
                    foreach (var kwd in HintKwds)
                    {
                        var FindValue = CommodityAnalyzedInfos.Find(e => e.SeletedKeywords.Equals(kwd));
                        if (FindValue != null)
                        {
                            ImportantKwdCommodityAnalyzeInfo.Add(FindValue);
                        }
                    }
                    if (CommodityAnalyzedInfos.Count > 0)
                    {
                        foreach (var value in ImportantKwdCommodityAnalyzeInfo)
                        {
                            var ShoppingInfo = await _NaverSearchShoppingAPIService.SearchingByKeywords(value.SeletedKeywords);
                            value.Category1 = ShoppingInfo.Category1;
                            value.Category2 = ShoppingInfo.Category2;
                            value.Category3 = ShoppingInfo.Category3;
                            value.Category4 = ShoppingInfo.Category4;
                            value.SellerMCommodityId = commodity.Id;
                            value.TotalCommodityCount = ShoppingInfo.TotalCommodityCount;
                            await value.PostAsync(_SellerMarketDataContext);
                        }
                    }
                    ImportantKwdCommodityAnalyzeInfo = new();
                    CommodityAnalyzedInfos = new();
                }
            }
        }
        public async Task ConfigureRelKwdInfo()
        {
            foreach (var commodity in SellerMCommodities)
            {
                if (commodity.ImportantKwds.Count == 0) { continue; }
                var CurrentCommodityAnalyzeInfos = await _CommodityAnalyzedInfoRepository.GetToListByMCommodityIdAsync(commodity.Id);
                var HintKwds = commodity.ConfigureHintKwdByImportantKwds();
                if(HintKwds.Count == 0) { continue;}
                var Result = await _NaverSerachRelKwdAPIService.GetRelKwdStat(HintKwds);
                ConfigureRelKwdInfos(Result);
                if(CommodityAnalyzedInfos.Count == 0) { continue;}
                CommodityAnalyzedInfos = CommodityAnalyzedInfos.Where(e => e.MonthlyMobileQcCnt >= 2000).ToList();
                foreach (var info in CommodityAnalyzedInfos)
                {
                    if (info.SeletedKeywords != null)
                    {
                        if (info.SeletedKeywords.Contains(commodity.ImportantKwds[0]) && commodity.ImportantKwds.FirstOrDefault(e => e.Equals(info.SeletedKeywords)) == null)
                        {
                            commodity.ImportantKwds.Add(info.SeletedKeywords);
                        }
                    }
                }
                int cnt = 0;
                List<string> RemoveKwds = new();
                foreach (var kwd in commodity.ImportantKwds)
                {
                    if(cnt == 0) { cnt++;  continue; }
                    var FindValue = CommodityAnalyzedInfos.FirstOrDefault(e => e.SeletedKeywords.Equals(kwd));
                    if(FindValue != null) { continue; }
                    RemoveKwds.Add(kwd);
                }
                foreach(var removekwd in RemoveKwds)
                {
                    commodity.ImportantKwds.Remove(removekwd);
                }
                await commodity.PutAsync(_SellerMarketDataContext);
                CommodityAnalyzedInfos = new();
            }
        }
        public async Task DeleteAllCommodityAnalyzeInfo()
        {
            var Info = await _CommodityAnalyzedInfoRepository.GetToListAsync();
            foreach (var value in Info)
            {
                await value.DeleteAsync(_SellerMarketDataContext);
            }
        }
        public async Task SettingImportantKwdNodes()
        {
            var NotSettingImportantKwdNodes = SellerMCommodities.Where(e => e.ImportantKeywordNodes == null && e.Name != null).ToList();
            int Totalcnt = NotSettingImportantKwdNodes.Count;
            int cnt = 1;
            foreach (var value in NotSettingImportantKwdNodes)
            {
                _logger.LogInformation(Totalcnt.ToString());
                _logger.LogInformation(cnt.ToString());
                cnt++;
                await _ImportantKwdSettingService.SetImportantKwdNodes(value);
            }
        }
        public async Task SettingImportantKwds()
        {
            var NotSettingImportantKwds = SellerMCommodities.Where(e => e.ImportantKwds.Count == 0 && e.ImportantKeywordNodes != null).ToList();
            int Totalcnt = NotSettingImportantKwds.Count;
            int cnt = 1;
            foreach (var value in NotSettingImportantKwds)
            {
                _logger.LogInformation(Totalcnt.ToString());
                _logger.LogInformation(cnt.ToString());
                cnt++;
                await _ImportantKwdSettingService.SetImportantKwdByNode(value);
            }
        }
        public async Task DeleteAllImportantKwds()
        {
            var SettingImportantKwdsSellerMCommodities = SellerMCommodities.Where(e => e.ImportantKwds != null).ToList();
            foreach (var value in SettingImportantKwdsSellerMCommodities)
            {
                value.ImportantKwds = null;
                await value.PutAsync(_SellerMarketDataContext);
            }
        }
        public async Task DeleteAllImportantKwdNodes()
        {
            var SettingImportantKwdsSellerMCommodities = SellerMCommodities.Where(e => e.ImportantKeywordNodes != null).ToList();
            foreach (var value in SettingImportantKwdsSellerMCommodities)
            {
                value.ImportantKeywordNodes = null;
                await value.PutAsync(_SellerMarketDataContext);
            }
        }
        public async Task SettingImportantKwdType()
        {
            var values = SellerMCommodities.Where(e => e.ImportantKwds != null && e.ImportantKwds.Count > 0).ToList();
            _logger.LogInformation(values.Count.ToString());
            int cnt = 1;
            foreach (var value in values)
            {
                if (value.ImportantKwds == null || value.ImportantKwds.Count == 0) { continue; }
                _logger.LogInformation(value.ToStringImportantKwds());
                _logger.LogInformation(cnt.ToString());
                cnt++;
                await _ImportantKwdSettingService.SetImportantKwdType(value);
            }
        }
        public async Task DeleteAllImportantKwdType()
        {
            var values = SellerMCommodities.Where(e => e.SellerSMCommodities.Count > 0).ToList();
            foreach (var value in values)
            {
                foreach (var sm in value.SMCommodities)
                {
                    await sm.DeleteAsync(_SellerMarketDataContext);
                }
            }
        }
        public async Task DeleteAllCommodityAnalyzedInfos()
        {
            foreach (var value in SellerMCommodities)
            {
                foreach (var info in value.CommodityAnalyzedInfos)
                {
                    if (info != null)
                    {
                        await info.DeleteAsync(_SellerMarketDataContext);
                    }
                }
            }
        }
        public async Task ReAnalyzingCommodityAnalyzedInfo()
        {
            await ConfigureShoppingInfoAndUpdateAsync(TotalCommodityCategoyNullCommodityAnalyzedInfos);
            AnalyzedInit();
        }
        // 1. 분석 상품정보 선택
        public void SelectingCode(string code)
        {
            SelectedCode = code;
            var value = SellerMCommodities.FirstOrDefault(e => e.Code.Equals(SelectedCode));
            if (value != null)
            {
                SelectedMCommodity = value;
                SplitKeywords = SelectedMCommodity.Keywords?.SplitKeywords();
            }
            IsSelect = true;
        }
        // 2. 분석 키워드 설정
        public void SelectingAnalyzingKwd(string keyword)
        {
            // 타임 스탬프가 필요하네.
            if (DicSelectedAnalyzingKwd.Keys.Count >= 5)
            {
                var stamps = DicSelectedAnalyzingKwd.Keys;
                var MinStamp = stamps.Min();
                DicSelectedAnalyzingKwd.Remove(MinStamp);
                var newTimeStamp = GetTimeStamp();
                DicSelectedAnalyzingKwd.Add(newTimeStamp, keyword);
                SelectedKwds = SelectedAnalyzingKwds();
            }
            else
            {
                var TimeStamp = GetTimeStamp();
                DicSelectedAnalyzingKwd.Add(TimeStamp, keyword);
                SelectedKwds = SelectedAnalyzingKwds();
            }
        }
        public string SelectedAnalyzingKwds()
        {
            StringBuilder stringBuilder = new();
            foreach (var key in DicSelectedAnalyzingKwd.Keys)
            {
                stringBuilder.Append(DicSelectedAnalyzingKwd[key]);
                stringBuilder.Append(" ");
            }
            return stringBuilder.ToString();
        }
        public void SetFilter()
        {
            IsSetFilterCount = true;
        }
        // 3. 중요 키워드의 연관 키워드 조회
        public async Task GetRelatedKwdsInfo()
        {
            if (IsSetFilterCount)
            {
                IsBusy = true;
                List<string> SelectedKwdList = new();
                foreach (var key in DicSelectedAnalyzingKwd.Keys)
                {
                    SelectedKwdList.Add(DicSelectedAnalyzingKwd[key]);
                }
                IsBusy = false;
                Back();
            }
        }
        private List<CommodityAnalyzedInfo> GetMaxInfoCommodityAnalyzedInfo(Dictionary<string, List<CommodityAnalyzedInfo>> DicSameCategoryCommodityAnalyzeInfos)
        {
            var keys = DicSameCategoryCommodityAnalyzeInfos.Keys;
            string? MaxKey = null;
            foreach (var key in keys)
            {
                var CommodityAnalyzedInfos = DicSameCategoryCommodityAnalyzeInfos[key];
                if (MaxKey == null)
                {
                    MaxKey = key;
                }
                else
                {
                    int MaxKeyCount = DicSameCategoryCommodityAnalyzeInfos[MaxKey].Count;
                    int CurrentKeyCount = DicSameCategoryCommodityAnalyzeInfos[key].Count;
                    if (CurrentKeyCount > MaxKeyCount)
                    {
                        MaxKey = key;
                    }
                }
            }
            return DicSameCategoryCommodityAnalyzeInfos[MaxKey];
        }
        private async Task SaveCommodityAnalyziedInfos(List<CommodityAnalyzedInfo> commodityAnalyzedInfos, int FilterByCompetiveIndex, string SellerMCommodityId)
        {
            foreach (var value in commodityAnalyzedInfos)
            {
                var index = value.TotalCommodityCount / (value.MonthlyMobileQcCnt + value.MonthlyPcQcCnt);
                if (index <= FilterByCompetiveIndex)
                {
                    value.SellerMCommodityId = SellerMCommodityId;
                    await value.PostAsync(_SellerMarketDataContext);
                }
            }
        }
        private void ConfigureRelKwdInfos(NaverRelKwdStatResult naverRelKwdStatResult)
        {
            var valuelist = naverRelKwdStatResult.keywordList;
            if(valuelist == null) { return; }
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
                if (monthlyMobileQcCnt >= _FilterMonthlyCount)
                {
                    CommodityAnalyzedInfo commodityAnalyzedInfo = new();
                    commodityAnalyzedInfo.MonthlyPcQcCnt = monthlyPcQcCnt;
                    commodityAnalyzedInfo.MonthlyMobileQcCnt = monthlyMobileQcCnt;
                    commodityAnalyzedInfo.SeletedKeywords = value.relKeyword;
                    commodityAnalyzedInfo.SellerMCommodityId = SelectedMCommodity.Id;
                    CommodityAnalyzedInfos.Add(commodityAnalyzedInfo);
                }
            }
        }
        public string SetCompetition(int MonthlyMobile, int MonthlyPc, int TotalCommodity)
        {
            return (TotalCommodity / (MonthlyMobile + MonthlyPc)).ToString();
        }
        private async Task<bool> ConfigureShoppingInfo(List<CommodityAnalyzedInfo> commodityAnalyzedInfos)
        {
            bool IsQueryable = true;
            foreach (var value in commodityAnalyzedInfos)
            {
                if (value.TotalCommodityCount > 0) { continue; }
                var ShppingInfo = await _NaverSearchShoppingAPIService.SearchingByKeywords(value.SeletedKeywords);
                if (ShppingInfo != null)
                {
                    if (ShppingInfo.Category1 == "" || ShppingInfo.Category1 == " " || ShppingInfo.Category1 == null) { IsQueryable = false; break; }
                    value.Category1 = ShppingInfo.Category1;
                    value.Category2 = ShppingInfo.Category2;
                    value.Category3 = ShppingInfo.Category3;
                    value.Category4 = ShppingInfo.Category4;
                    value.TotalCommodityCount = ShppingInfo.TotalCommodityCount;
                }
            }
            return IsQueryable;
        }
        private async Task<bool> ConfigureShoppingInfoAndUpdateAsync(List<CommodityAnalyzedInfo> commodityAnalyzedInfos)
        {
            bool IsQueryable = true;
            foreach (var value in commodityAnalyzedInfos)
            {
                if (value.TotalCommodityCount > 0) { continue; }
                var ShppingInfo = await _NaverSearchShoppingAPIService.SearchingByKeywords(value.SeletedKeywords);
                if (ShppingInfo != null)
                {
                    if (ShppingInfo.Category1 == "" || ShppingInfo.Category1 == " " || ShppingInfo.Category1 == null) { IsQueryable = false; continue; }
                    Console.Write("Before : ");
                    Console.WriteLine(value.TotalCommodityCount);
                    value.Category1 = ShppingInfo.Category1;
                    value.Category2 = ShppingInfo.Category2;
                    value.Category3 = ShppingInfo.Category3;
                    value.Category4 = ShppingInfo.Category4;
                    value.TotalCommodityCount = ShppingInfo.TotalCommodityCount;
                    Console.Write("After : ");
                    Console.WriteLine(value.TotalCommodityCount);
                    await value.PutAsync(_SellerMarketDataContext);
                }
            }
            return IsQueryable;
        }

        private void DivideCommodityAnalyzeInfoBySameCategory(List<CommodityAnalyzedInfo> commodityAnalyzedInfos)
        {
            foreach (var analyzedvalue in commodityAnalyzedInfos)
            {
                var category = CreateCategoryKey(analyzedvalue);
                var keys = DicSameCategoryCommodityAnalyzeInfos.Keys;
                bool IsAdded = false;
                foreach (var key in keys)
                {
                    if (key == category)
                    {
                        DicSameCategoryCommodityAnalyzeInfos[key].Add(analyzedvalue);
                        IsAdded = true;
                        break;
                    }
                }
                if (!IsAdded)
                {
                    DicSameCategoryCommodityAnalyzeInfos.Add(category, new List<CommodityAnalyzedInfo>() { analyzedvalue });
                }
            }
        }
        private string CreateCategoryKey(CommodityAnalyzedInfo analyzedvalue)
        {
            return analyzedvalue.Category1 + " " + analyzedvalue.Category2 + " " + analyzedvalue.Category3 + " " + analyzedvalue.Category4;
        }
        public async Task SelectingAnalyzedKwd(string keyword, CommodityAnalyzedInfo commodityAnalyzedInfo)
        {
            var value = CommodityAnalyzedInfos.FirstOrDefault(e => e.SeletedKeywords.Equals(keyword));
            if (value != null)
            {
                IsBusy = true;
                await value.PostAsync(_SellerMarketDataContext);
                var key = CreateCategoryKey(value);
                DicSameCategoryCommodityAnalyzeInfos[key].Remove(value);
                var SellerMCommodity = SellerMCommodities.FirstOrDefault(e => e.Equals(commodityAnalyzedInfo));
                if (SellerMCommodity != null) { SellerMCommodities.Remove(SellerMCommodity); }
                IsBusy = false;
            }
        }
        private long GetTimeStamp()
        {
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return (long)timeSpan.TotalMilliseconds;
        }
        // 5. 선택된 분석 키워드 분석
        public async Task AnalyzingSelectedKwds()
        {
            List<string> SelectedKwds = new();
            foreach (var key in DicSelectedAnalyzingKwd.Keys)
            {
                SelectedKwds.Add(DicSelectedAnalyzingKwd[key]);
            }
            naverRelKwdStatResult = await _NaverSerachRelKwdAPIService.GetRelKwdStat(SelectedKwds);
        }
        public async Task TotalAnalyzing()
        {
            IsBusy = true;
            List<SellerMCommodity> DeletingSellerMCommodites = new();
            var AnalyzingSellerMCommodities = SellerMCommodities.Where(e => e.CommodityAnalyzedInfos.Count == 0).ToList();
            foreach (var value in AnalyzingSellerMCommodities)
            {
                SplitKeywords = value.Keywords?.SplitKeywords();
                List<string> AnalyzingKwd = new();
                foreach (var kwd in SplitKeywords)
                {
                    if (kwd == null) { continue; }
                    if (kwd == "") { continue; }
                    if (kwd == " ") { continue; }
                    if (AnalyzingKwd.Count >= 5) { break; } // 5이상 정지
                    var kwdvalue = AnalyzingKwd.Find(e => e.Equals(kwd));
                    if (kwdvalue == null) // 중복제거
                    {
                        AnalyzingKwd.Add(kwd);
                    }
                }
                naverRelKwdStatResult = await _NaverSerachRelKwdAPIService.GetRelKwdStat(AnalyzingKwd);
                ConfigureRelKwdInfos(naverRelKwdStatResult);
                bool IsQueryable = await ConfigureShoppingInfo(CommodityAnalyzedInfos);
                DivideCommodityAnalyzeInfoBySameCategory(CommodityAnalyzedInfos);
                if (DicSameCategoryCommodityAnalyzeInfos.Keys.Count == 0)
                {
                    DeletingSellerMCommodites.Add(value);
                    AnalyzedInit();
                    continue;
                }
                var MaxCommodityAnalyzedInfos = GetMaxInfoCommodityAnalyzedInfo(DicSameCategoryCommodityAnalyzeInfos);
                await SaveCommodityAnalyziedInfos(MaxCommodityAnalyzedInfos, FilterByCompetiveIndex, value.Id);
                AnalyzedInit();
                NotAnalyzingCommodityCount--;
            }
            foreach (var deletevalue in DeletingSellerMCommodites)
            {
                await deletevalue.DeleteAsync(_SellerMarketDataContext);
            }
            IsBusy = false;
            Back();
        }
        private void AnalyzedInit()
        {
            naverRelKwdStatResult = new();
            DicSelectedAnalyzingKwd = new();
            DicSameCategoryCommodityAnalyzeInfos = new();
            SplitKeywords = new();
            if (CommodityAnalyzedInfos.Count > 0)
            {
                CommodityAnalyzedInfos = new();
            }
        }
        public void Back()
        {
            naverRelKwdStatResult = new();
            DicSelectedAnalyzingKwd = new();
            DicSameCategoryCommodityAnalyzeInfos = new();
            SelectedKwds = null;
            SelectedMCommodity = new();
            SplitKeywords = new();
            RelKwdsofImportantKwd = new();
            if (CommodityAnalyzedInfos.Count > 0)
            {
                CommodityAnalyzedInfos = new();
            }
            CandidateKwds = new();
            IsSelect = false;
            IsSetFilterCount = false;
        }
        public async Task AnalyzingSplitKeywords()
        {
            foreach (var value in CandidateKwds)
            {
                var Info = await _NaverSearchShoppingAPIService.SearchingByKeywords(value);
                CommodityAnalyzedInfos.Add(Info);
            }
        }
        public async Task DeletingCategory()
        {
            foreach (var value in SellerMCommodities)
            {
                value.Category = null;
                await value.PutAsync(_SellerMarketDataContext);
            }
        }
    }
}
