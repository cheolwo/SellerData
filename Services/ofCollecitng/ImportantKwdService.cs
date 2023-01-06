using BusinessData.ofDataAccessLayer.ofModelExtenstions;
using Microsoft.Extensions.Logging;
using SellerCommon.SellerData.Model;
using SellerCommon.SellerData.ofDbContext;
using SellerCommon.SellerData.ofPresentationLayer.ofExtensions;
using SellerCommon.SellerData.Services;
using SellerData.ofDataAccessLayer.ofDataContext;
using SellerData.ofDataAccessLayer.ofModel;
using SellerData.ofDataAccessLayer.ofRepository.ofMarket;

namespace SellerData.Services.ofCollecitng
{
    public interface IImportantKwdSettingService
    {
        Task SetImportantKwd(SellerMCommodity sellerMCommodity);
        Task SetImportantKwdNodes(SellerMCommodity sellerMCommodity);
        Task SetImportantKwdByNode(SellerMCommodity sellerMCommodity);
        Task SetImportantKwdType(SellerMCommodity sellerMCommodity);
        Task SetImportantKwdType(SellerSMCommodity sellerSMCommodity);
        Task<List<CommodityAnalyzedInfo>> FurtherAnalyzingImportantKwdAndReturn(SellerSMCommodity sellerSMCommodity);
        List<string> ConfigureHintKwds(List<string> ImportantKwds);
    }
    public interface IFurtherAnalyzingImportantKwdService
    {
        Task FurtherAnalyzingImportantKwd(SellerSMCommodity sellerSMCommodity);
        List<string> ConfigureHintKwds(List<string> ImportantKwds);
    }
    public static class NamingType
    {
        public const string MainForm = "주력형";
        public const string CombinationForm = "결합형";
        public const string MixForm = "결합/주력형";
    }
    public class ImportantKwdService : IImportantKwdSettingService, IFurtherAnalyzingImportantKwdService
    {
        private readonly SellerMarketDataContext _sellerMarketDataContext;
        private readonly OpenMarketDataContext _openMarketDataContext;
        private readonly INaverSerachRelKwdAPIService _naverSerachRelKwdAPIService;
        private readonly INaverSearchShoppingAPIService _naverSearchShoppingAPIService;
        private readonly ICollectingCommodityCategoryService _collectingCommodityCategoryService;
        private readonly IOpenMarketCommonCategoryRepostiory _openMarketCommonCategoryRepostiory;
        private readonly ILogger<ImportantKwdService> _logger;
        private readonly SellerMarketDbContext _sellerMarketDbContext;
        private int _FilterMonthlyCount;
        private int _FilterByCompetiveIndex;
        private List<CommodityAnalyzedInfo> CommodityAnalyzedInfos = new();
        private Dictionary<string, List<CommodityAnalyzedInfo>> DicSameCategoryCommodityAnalyzeInfos = new();
        public ImportantKwdService(SellerMarketDataContext sellerMarketDataContext, SellerMarketDbContext sellerMarketDbContext, OpenMarketDataContext openMarketDataContext,
            ICollectingCommodityCategoryService collectingCommodityCategoryService,
            INaverSearchShoppingAPIService naverSearchShoppingAPIService,
            INaverSerachRelKwdAPIService naverSerachRelKwdAPIService,
            IOpenMarketCommonCategoryRepostiory openMarketCommonCategoryRepostiory,
                                ILogger<ImportantKwdService> logger)
        {
            _openMarketDataContext = openMarketDataContext;
            _sellerMarketDbContext = sellerMarketDbContext;
            _sellerMarketDataContext = sellerMarketDataContext;
            _naverSearchShoppingAPIService = naverSearchShoppingAPIService;
            _naverSerachRelKwdAPIService = naverSerachRelKwdAPIService;
            _collectingCommodityCategoryService = collectingCommodityCategoryService;
            _openMarketCommonCategoryRepostiory = openMarketCommonCategoryRepostiory;
            _logger = logger;
            _FilterByCompetiveIndex = 50;
            _FilterMonthlyCount = 1000;
        }
        public async Task SetImportantKwdNodes(SellerMCommodity sellerMCommodity)
        {
            if (sellerMCommodity != null && sellerMCommodity.Keywords != null)
            {
                var Kwds = sellerMCommodity.Keywords.SplitKeywords();
                var DividedBefore = DividedKwdBeforeAboveTwoCharacter(Kwds);
                var DividedAfter = DividedKwdAfterAboveTwoCharacter(Kwds);
                var MergeList = DistintMerge(DividedBefore, DividedAfter);
                var ImportantKeywordNodes = CorrepontDiviedKwdToKwd(MergeList, Kwds);
                sellerMCommodity.ImportantKeywordNodes = ImportantKeywordNodes;
                await sellerMCommodity.PutAsync(_sellerMarketDataContext);
            }
        }
        public async Task SetImportantKwdType(SellerMCommodity sellerMCommodity)
        {
            var Type = CheckType(sellerMCommodity);   
            SellerSMCommodity sellerSMCommodity = new();    
            sellerSMCommodity.SellerMCommodityId = sellerMCommodity.Id;
            sellerSMCommodity.SellerMarketId = sellerMCommodity.CenterId;
            sellerSMCommodity.ImportantKwdType = Type;
            _logger.LogInformation(sellerSMCommodity.ImportantKwdType);
            await sellerSMCommodity.PutAsync(_sellerMarketDataContext);
        }
        public async Task SetImportantKwdType(SellerSMCommodity sellerSMCommodity)
        {
            if(sellerSMCommodity.SellerMCommodity == null) { sellerSMCommodity.SellerMCommodity = await _sellerMarketDataContext.GetByIdAsync<SellerMCommodity>(sellerSMCommodity.SellerMCommodityId); }
            var Type = CheckType(sellerSMCommodity.SellerMCommodity);
            sellerSMCommodity.ImportantKwdType = Type;
            _logger.LogInformation(sellerSMCommodity.ImportantKwdType);
            await sellerSMCommodity.PutAsync(_sellerMarketDataContext);
        }
        public async Task SetImportantKwdType(List<SellerSMCommodity> sellerSMCommodites)
        {
            foreach(var sellerSMCommodity in sellerSMCommodites)
            {
                if (sellerSMCommodity.SellerMCommodity == null) { sellerSMCommodity.SellerMCommodity = await _sellerMarketDataContext.GetByIdAsync<SellerMCommodity>(sellerSMCommodity.SellerMCommodityId); }
                var Type = CheckType(sellerSMCommodity.SellerMCommodity);
                sellerSMCommodity.ImportantKwdType = Type;
                _sellerMarketDbContext.Update(sellerSMCommodity);
                _logger.LogInformation(sellerSMCommodity.ImportantKwdType);
            }
            await _sellerMarketDbContext.SaveChangesAsync();
        }
        private string CheckType(SellerMCommodity sellerMCommodity)
        {
            if (sellerMCommodity != null && sellerMCommodity.ImportantKwds != null)
            {
                var ImportantKwd = sellerMCommodity.ImportantKwds.FirstOrDefault();
                if (ImportantKwd != null)
                {
                    List<string> CheckType = new();
                    foreach (var value in sellerMCommodity.ImportantKwds)
                    {
                        if (value.Equals(ImportantKwd)) { continue; }
                        if (value.Length < ImportantKwd.Length) { CheckType.Add("미포함"); break; }
                        int Length = ImportantKwd.Length;
                        var AfterSubValue = value.Substring(value.Length - Length, Length);
                        if (AfterSubValue.Equals(ImportantKwd)) { CheckType.Add("후위"); continue; }
                        var BeforeSubValue = value.Substring(0, Length);
                        if (BeforeSubValue.Equals(ImportantKwd)) { CheckType.Add("전위"); continue; }
                        if (BeforeSubValue.Contains(ImportantKwd)) { CheckType.Add("포함"); continue; }
                        else
                        {
                            CheckType.Add("미포함");
                            break;
                        }
                    }
                    string ImportantType = "";
                    var NotIncludedValue = CheckType.Find(e => e.Equals("미포함"));
                    if (NotIncludedValue != null) { ImportantType = NamingType.MainForm; return ImportantType; }
                    var BeforeValue = CheckType.Find(e => e.Equals("전위"));
                    var AfterValue = CheckType.Find(e => e.Equals("후위"));
                    if (BeforeValue != null && AfterValue != null) { ImportantType = NamingType.MixForm; return ImportantType; }
                    if (AfterValue != null && BeforeValue == null) { ImportantType = NamingType.CombinationForm; return ImportantType; }
                    ImportantType = NamingType.MainForm;
                    return ImportantType;
                }
                throw new ArgumentNullException(nameof(ImportantKwd));
            }
            throw new ArgumentNullException(nameof(sellerMCommodity) + "||" + nameof(sellerMCommodity.ImportantKwds));
        }

        private List<string> DistintMerge(List<string> DividedBefore, List<string> DividedAfter)
        {
            List<string> MergeList;
            MergeList = DividedBefore;
            foreach (var value in DividedAfter)
            {
                if (!MergeList.Contains(value))
                {
                    MergeList.Add(value);
                }
            }
            return MergeList;
        }
        private List<ImportantKeywordNode> CorrepontDiviedKwdToKwd(List<string> DividedKwds, List<string> Kwds)
        {
            List<ImportantKeywordNode> importantKeywordNodes = new();
            foreach (var DividedKwd in DividedKwds)
            {
                ImportantKeywordNode importantKeywordNode = new();
                importantKeywordNode.RelatedKeywords = new();
                List<string> CheckRelatedKwds = new();
                int cnt = 0;
                foreach (var kwd in Kwds)
                {
                    if (kwd.Contains(DividedKwd))
                    {
                        cnt++;
                        if (!CheckRelatedKwds.Contains(kwd))
                        {
                            CheckRelatedKwds.Add(kwd);
                        }
                    }
                }
                if (cnt >= 2)
                {
                    var EqualValue = importantKeywordNodes.Where(e => e.ImportantKeyword.Equals(DividedKwd)).FirstOrDefault();
                    if (EqualValue == null && CheckRelatedKwds.Count >= 3)
                    {
                        importantKeywordNode.ImportantKeyword = DividedKwd;
                        importantKeywordNode.RelatedKeywords = CheckRelatedKwds;
                        importantKeywordNodes.Add(importantKeywordNode);
                    }
                    OptimizingImportantKeywordNode(importantKeywordNodes);
                }
            }
            return importantKeywordNodes;
        }
        private void OptimizingImportantKeywordNode(List<ImportantKeywordNode> importantKeywordNodes)
        {
            List<ImportantKeywordNode> DeleteImportantKeywordNodes = new();
            foreach (var node in importantKeywordNodes)
            {
                var values = importantKeywordNodes.Where(e => e.ImportantKeyword.Contains(node.ImportantKeyword)).ToList();
                if (values.Count >= 2)
                {
                    var MaxLengthValue = values.Max(e => e.ImportantKeyword.Length);
                    var MaxLengthFindValue = values.FirstOrDefault(e => e.ImportantKeyword.Length.Equals(MaxLengthValue));
                    values.Remove(MaxLengthFindValue);
                    foreach (var value in values)
                    {
                        var FindValue = DeleteImportantKeywordNodes.Find(e => e.Equals(value));
                        if (FindValue == null)
                        {
                            DeleteImportantKeywordNodes.Add(value);
                        }
                    }
                }
            }
            foreach (var value in DeleteImportantKeywordNodes)
            {
                importantKeywordNodes.Remove(value);
            }
        }
        private List<string> DividedKwdBeforeAboveTwoCharacter(List<string> Kwds)
        {
            List<string> SubStrings = new();
            foreach (var kwd in Kwds)
            {
                int Length = kwd.Length;
                for (int i = 2; i < Length; i++)
                {
                    var subString = kwd.Substring(0, i);
                    if (!SubStrings.Contains(subString))
                    {
                        SubStrings.Add(subString);
                    }
                }
            }
            return SubStrings;
        }
        private List<string> DividedKwdAfterAboveTwoCharacter(List<string> Kwds)
        {
            List<string> SubStrings = new();
            foreach (var kwd in Kwds)
            {
                int Length = kwd.Length;
                for (int i = 2; i <= Length; i++)
                {
                    var subString = kwd.Substring(Length - i, i);
                    if (!SubStrings.Contains(subString))
                    {
                        SubStrings.Add(subString);
                    }
                }
            }
            return SubStrings;
        }
        public async Task SetImportantKwdByNode(SellerMCommodity sellerMCommodity)
        {
            if (sellerMCommodity != null)
            {
                sellerMCommodity.ImportantKwds = new();
                if (sellerMCommodity.ImportantKeywordNodes != null && sellerMCommodity.ImportantKeywordNodes.Count == 1)
                {
                    var value = sellerMCommodity.ImportantKeywordNodes.FirstOrDefault();
                    if (value.RelatedKeywords != null)
                    {
                        if (sellerMCommodity.ImportantKwds.Count == 0)
                        {
                            sellerMCommodity.ImportantKwds.Add(value.ImportantKeyword);
                        }
                        foreach (var kwd in value.RelatedKeywords)
                        {
                            var Findvalue = sellerMCommodity.ImportantKwds.Find(e => e.Equals(kwd));
                            if (Findvalue == null)
                            {
                                sellerMCommodity.ImportantKwds.Add(kwd);
                            }
                        }
                    }
                }
                if (sellerMCommodity.ImportantKeywordNodes != null && sellerMCommodity.ImportantKeywordNodes.Count >= 2)
                {
                    List<PointNode<ImportantKeywordNode>> PointNodes = new();
                    foreach (var node in sellerMCommodity.ImportantKeywordNodes)
                    {
                        var PointNode = CheckSettedImportantKwdAfterRelatedKeyword(node);
                        PointNodes.Add(PointNode);
                    }
                    var MaxPoint = PointNodes.Max(e => e.Point);
                    var MaxPointNode = PointNodes.FirstOrDefault(e => e.Point.Equals(MaxPoint));
                    if (sellerMCommodity.ImportantKwds.Count == 0)
                    {
                        sellerMCommodity.ImportantKwds.Add(MaxPointNode.t.ImportantKeyword);
                    }
                    foreach (var kwd in MaxPointNode.t.RelatedKeywords)
                    {
                        var value = sellerMCommodity.ImportantKwds.Find(e => e.Equals(kwd));
                        if (value == null)
                        {
                            sellerMCommodity.ImportantKwds.Add(kwd);
                        }
                    }
                }
                await sellerMCommodity.PutAsync(_sellerMarketDataContext);
            }
        }
        private PointNode<ImportantKeywordNode> CheckSettedImportantKwdAfterRelatedKeyword(ImportantKeywordNode importantKeywordNode)
        {
            PointNode<ImportantKeywordNode> pointNode = new(importantKeywordNode);
            if (importantKeywordNode != null && importantKeywordNode.RelatedKeywords != null)
            {
                foreach (var value in importantKeywordNode.RelatedKeywords)
                {
                    int ImportantKwdLength = importantKeywordNode.ImportantKeyword.Length;
                    var CheckValue = value.Substring(value.Length - ImportantKwdLength, ImportantKwdLength);
                    if (CheckValue.Equals(importantKeywordNode.ImportantKeyword))
                    {
                        pointNode.Point++;
                    }
                }
            }
            return pointNode;
        }
        public async Task SetImportantKwd(SellerMCommodity sellerMCommodity)
        {
            if (sellerMCommodity != null && sellerMCommodity.Keywords != null)
            {
                var Nodes = await _openMarketDataContext.GetsAsync<OpenMarketCommonCategory>();
                var kwds = sellerMCommodity.Keywords.SplitKeywords();
                var PointNodes = _collectingCommodityCategoryService.ConvertToPointNode(Nodes);
                PointNodes = _collectingCommodityCategoryService.CheckPoint(kwds, PointNodes);
                var MaxPointNodes = GetMaxPointCategoryNodes(PointNodes);
                var Kwds = GetKwdByCategoryList(MaxPointNodes);
                sellerMCommodity.ImportantKwds = GetImportantKwdByMaxPointsKwds(kwds, Kwds);
                await sellerMCommodity.PutAsync(_sellerMarketDataContext);
            }
        }
        private List<CategoryPoint> GetMaxPointCategoryNodes(List<CategoryPoint> PointNodes)
        {
            var MaxCategoryPoint = PointNodes.MaxBy(e => e.Point);
            int Point = MaxCategoryPoint.Point;
            var MaxPointNodes = PointNodes.Where(e => e.Point.Equals(Point)).ToList();
            return MaxPointNodes;
        }
        private List<string> GetKwdByCategoryList(List<CategoryPoint> PointNodes)
        {
            List<string> Kwds = new List<string>();
            foreach (var node in PointNodes)
            {
                GetKwdByCategory(node.OpenMarketCommonCategory.Category1, Kwds);
                GetKwdByCategory(node.OpenMarketCommonCategory.Category2, Kwds);
                GetKwdByCategory(node.OpenMarketCommonCategory.Category3, Kwds);
                GetKwdByCategory(node.OpenMarketCommonCategory.Category4, Kwds);
            }
            return Kwds;
        }
        private void GetKwdByCategory(string? Category, List<string> Kwds)
        {
            if (Category == null) { return; }
            var SplitKwds = Category.Split("/");
            foreach (var value in SplitKwds)
            {
                if (!Kwds.Contains(value))
                {
                    Kwds.Add(value);
                }
            }
        }
        private List<string> GetImportantKwdByMaxPointsKwds(List<string> CommodityKwds, List<string> KwdsofMaxPointNodes)
        {
            List<PointNode<string>> pointNodes = new();
            foreach (var commoditykwd in CommodityKwds)
            {
                pointNodes.Add(new(commoditykwd));
            }
            foreach (var kwd in KwdsofMaxPointNodes)
            {
                foreach (var pointNode in pointNodes)
                {
                    if (pointNode.t.Contains(kwd))
                    {
                        pointNode.Point++;
                    }
                }
            }
            var MaxPoint = pointNodes.Max(e => e.Point);
            var MaxPointNode = pointNodes.Where(e => e.Point.Equals(MaxPoint)).ToList();
            List<string> ImportantKwds = new();
            foreach (var value in MaxPointNode)
            {
                ImportantKwds.Add(value.t);
            }
            return ImportantKwds;
        }

        /// <summary>
        /// 1. 결합형인 경우 뒤에 포함되는 경우만 연관 키워드로 포함한다.
        /// 2. 주력형, 주력형/결합형인 경우 포함되기만 하면 연관 키워드로 포함한다.
        /// </summary>
        /// <param name="sellerSMCommodity"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotImplementedException"></exception>
        public async Task FurtherAnalyzingImportantKwd(SellerSMCommodity sellerSMCommodity)
        {
            List<string> HintKwds;
            if (sellerSMCommodity.SellerMCommodity == null) { throw new ArgumentNullException("Seller SMCommodity Is Null"); }
            var SellerMCommodity = sellerSMCommodity.SellerMCommodity;
            if (SellerMCommodity.ImportantKwds.Count != 0 || SellerMCommodity.ImportantKwds != null)
            {
                try
                {
                    HintKwds = ConfigureHintKwds(SellerMCommodity.ImportantKwds);
                    var Results = await _naverSerachRelKwdAPIService.GetRelKwdStat(HintKwds);
                    ConfigureRelKwdInfosWithFilterFilterByQc(Results, SellerMCommodity, CommodityAnalyzedInfos);
                    FilterByImportantKwdAndType(CommodityAnalyzedInfos, SellerMCommodity.ImportantKwds.FirstOrDefault(), sellerSMCommodity.ImportantKwdType);
                    await ConfigureShoppingInfoAsync(CommodityAnalyzedInfos);
                    DivideCommodityAnalyzeInfoBySameCategory(CommodityAnalyzedInfos);
                    if (DicSameCategoryCommodityAnalyzeInfos.Keys.Count == 0) { return; }
                    var SameCategoryCommodityAnalyzedInfos = await GetSameCategoryCommodityAnalyzeInfo(sellerSMCommodity, DicSameCategoryCommodityAnalyzeInfos);
                    await SaveCommodityAnalyziedInfos(SameCategoryCommodityAnalyzedInfos, _FilterByCompetiveIndex, SellerMCommodity.Id);
                    CommodityAnalyzedInfos = new();
                    DicSameCategoryCommodityAnalyzeInfos = new();
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                }
            }
        }
        public async Task<List<CommodityAnalyzedInfo>> FurtherAnalyzingImportantKwdAndReturn(SellerSMCommodity sellerSMCommodity)
        {
            List<string> HintKwds;
            if (CommodityAnalyzedInfos.Count > 0) { CommodityAnalyzedInfos = new(); }
            if (DicSameCategoryCommodityAnalyzeInfos.Count > 0) { DicSameCategoryCommodityAnalyzeInfos = new(); }
            if (sellerSMCommodity.SellerMCommodity == null) { throw new ArgumentNullException("SellerSMCommodity Is Null"); }
            var SellerMCommodity = sellerSMCommodity.SellerMCommodity;
            if (SellerMCommodity.ImportantKwds.Count != 0 || SellerMCommodity.ImportantKwds != null)
            {
                HintKwds = ConfigureHintKwds(SellerMCommodity.ImportantKwds);
                var Results = await _naverSerachRelKwdAPIService.GetRelKwdStat(HintKwds);
                ConfigureRelKwdInfosWithFilterFilterByQc(Results, SellerMCommodity, CommodityAnalyzedInfos);
                FilterByImportantKwdAndType(CommodityAnalyzedInfos, SellerMCommodity.ImportantKwds.FirstOrDefault(), sellerSMCommodity.ImportantKwdType);
                await ConfigureShoppingInfoAsync(CommodityAnalyzedInfos);
                DivideCommodityAnalyzeInfoBySameCategory(CommodityAnalyzedInfos);
                if (DicSameCategoryCommodityAnalyzeInfos.Keys.Count == 0) { throw new ArgumentException("동일 카테고리 연관 키워드가 없습니다."); }
                var SameCategoryCommodityAnalyzedInfos = await GetSameCategoryCommodityAnalyzeInfo(sellerSMCommodity, DicSameCategoryCommodityAnalyzeInfos);
                return SameCategoryCommodityAnalyzedInfos;
            }
            throw new ArgumentNullException("조건에 안맞는 데이터입니다.");
        }

        public List<string> ConfigureHintKwds(List<string> ImportantKwds)
        {
            if (ImportantKwds == null) { throw new ArgumentNullException("ImportantKwds Is Null"); }
            List<string> HintKwds = new();
            foreach (var value in ImportantKwds)
            {
                if (HintKwds.Count >= 5)
                {
                    break;
                }
                HintKwds.Add(value);
            }
            return HintKwds;
        }
        public void ConfigureRelKwdInfosWithFilterFilterByQc(NaverRelKwdStatResult naverRelKwdStatResult, SellerMCommodity sellerMCommodity, List<CommodityAnalyzedInfo> commodityAnalyzedInfos)
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
                FilterByQc(value, monthlyMobileQcCnt, monthlyPcQcCnt, sellerMCommodity);
            }
        }
        private void FilterByQc(KeywordInfo value, int monthlyMobileQcCnt, int monthlyPcQcCnt, SellerMCommodity sellerMCommodity)
        {
            if (monthlyMobileQcCnt > _FilterMonthlyCount)
            {
                CommodityAnalyzedInfo commodityAnalyzedInfo = new();
                commodityAnalyzedInfo.MonthlyPcQcCnt = monthlyPcQcCnt;
                commodityAnalyzedInfo.MonthlyMobileQcCnt = monthlyMobileQcCnt;
                commodityAnalyzedInfo.SeletedKeywords = value.relKeyword;
                commodityAnalyzedInfo.SellerMCommodityId = sellerMCommodity.Id;
                CommodityAnalyzedInfos.Add(commodityAnalyzedInfo);
            }
        }
        public void FilterByImportantKwdAndType(List<CommodityAnalyzedInfo> commodityAnalyzedInfos, string ImportantKwd, string namingType)
        {
            List<CommodityAnalyzedInfo> DeleteInfo = new();
            foreach (var value in commodityAnalyzedInfos)
            {
                bool IsContain;
                if (namingType == NamingType.MainForm || namingType == NamingType.MixForm)
                {
                    IsContain = value.SeletedKeywords.Contains(ImportantKwd);
                    if (IsContain) { continue; }
                    else
                    {
                        DeleteInfo.Add(value);
                    }
                }
                else
                {
                    if (value.SeletedKeywords.Length <= ImportantKwd.Length) { DeleteInfo.Add(value); continue; }
                    var SubValue = value.SeletedKeywords.Substring(value.SeletedKeywords.Length - ImportantKwd.Length, ImportantKwd.Length);
                    if (SubValue.Equals(ImportantKwd)) { continue; }
                    else
                    {
                        DeleteInfo.Add(value);
                    }
                }
            }
            foreach (var value in DeleteInfo)
            {
                commodityAnalyzedInfos.Remove(value);
            }
        }
        public async Task<bool> ConfigureShoppingInfoAsync(List<CommodityAnalyzedInfo> commodityAnalyzedInfos)
        {
            bool IsQueryable = true;
            foreach (var value in commodityAnalyzedInfos)
            {
                if (value.TotalCommodityCount > 0) { continue; }
                var ShppingInfo = await _naverSearchShoppingAPIService.SearchingByKeywords(value.SeletedKeywords);
                if (ShppingInfo != null)
                {
                    if (ShppingInfo.Category1 == "" || ShppingInfo.Category1 == " " || ShppingInfo.Category1 == null) { IsQueryable = false; continue; }
                    Console.WriteLine(value.TotalCommodityCount);
                    value.Category1 = ShppingInfo.Category1;
                    value.Category2 = ShppingInfo.Category2;
                    value.Category3 = ShppingInfo.Category3;
                    value.Category4 = ShppingInfo.Category4;
                    value.TotalCommodityCount = ShppingInfo.TotalCommodityCount;
                    Console.WriteLine(value.TotalCommodityCount);
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
        private List<CommodityAnalyzedInfo> GetMaxPointInfoCommodityAnalyzedInfo(List<string> ImportantKwds, Dictionary<string, List<CommodityAnalyzedInfo>> DicSameCategoryCommodityAnalyzeInfos)
        {
            var keys = DicSameCategoryCommodityAnalyzeInfos.Keys;
            string? MaxKey = null;
            int MaxkeyPoint = 0;
            foreach (var key in keys)
            {
                var CommodityAnalyzedInfos = DicSameCategoryCommodityAnalyzeInfos[key];
                if (MaxKey == null)
                {
                    MaxKey = key;
                }
                else
                {
                    int CurrentPoint = 0;
                    foreach (var kwd in ImportantKwds)
                    {
                        foreach (var info in CommodityAnalyzedInfos)
                        {
                            if (info.SeletedKeywords.Contains(kwd))
                            {
                                CurrentPoint++;
                            }
                        }
                    }
                    if (CurrentPoint > MaxkeyPoint)
                    {
                        MaxKey = key;
                        MaxkeyPoint = CurrentPoint;
                    }
                }
            }
            return DicSameCategoryCommodityAnalyzeInfos[MaxKey];
        }
        private async Task<List<CommodityAnalyzedInfo>> GetSameCategoryCommodityAnalyzeInfo(SellerSMCommodity sellerSMCommodity, Dictionary<string, List<CommodityAnalyzedInfo>> DicSameCategoryCommodityAnalyzeInfos)
        {
            if (sellerSMCommodity.Code == null) { throw new ArgumentNullException(nameof(sellerSMCommodity.Code)); }
            var Category = await _openMarketCommonCategoryRepostiory.GetByCodeAsync(sellerSMCommodity.Code);
            string SameKey = "";
            foreach (var key in DicSameCategoryCommodityAnalyzeInfos.Keys)
            {
                var Infos = DicSameCategoryCommodityAnalyzeInfos[key];
                var Info = Infos.FirstOrDefault();
                if(Info.Category1 == null) { continue; }
                if (Info.Category3 == null) { Info.Category3 = ""; }
                if (Info.Category4 == null) { Info.Category4 = ""; }
                if (Info.Category1.Equals(Category.Category1) &&
                    Info.Category2.Equals(Category.Category2) &&
                    Info.Category3.Equals(Category.Category3) &&
                    Info.Category4.Equals(Category.Category4)
                    )
                {
                    SameKey = key;
                    break;
                }
            }
            if (SameKey == "") 
            {
                // 유사성을 점수제로 계산해서 만들 수 있도록 한다.
                // 1. sellerSMCommodity 카테고리에의한 점수계산
                // 2. sellerMCommodity ImportantKwds 에 의한 점수 계산
                List<PointNode<CommodityAnalyzedInfo>> pointNodes = new();
                foreach(var key in DicSameCategoryCommodityAnalyzeInfos.Keys)
                {
                    var Infos = DicSameCategoryCommodityAnalyzeInfos[key];
                    var Info = Infos.FirstOrDefault();
                    PointNode<CommodityAnalyzedInfo> node = new PointNode<CommodityAnalyzedInfo>(Info);
                    List<string> PointCategoryString = new();
                    if(Info.Category3 == null) { Info.Category3 = ""; }
                    if(Info.Category4 == null) { Info.Category4 = ""; }
                    PointCategoryString.Add(node.t.Category1);
                    PointCategoryString.Add(node.t.Category2);
                    PointCategoryString.Add(node.t.Category3);
                    PointCategoryString.Add(node.t.Category4);

                    List<string> SMCommodityCategoryString = new();
                    SMCommodityCategoryString.Add(Category.Category1);
                    SMCommodityCategoryString.Add(Category.Category2);
                    SMCommodityCategoryString.Add(Category.Category3);
                    SMCommodityCategoryString.Add(Category.Category4);

                    foreach (var categoryvalue in PointCategoryString)
                    {
                        // 중요키워드에 의한 점수계산
                        foreach (var kwd in sellerSMCommodity.SellerMCommodity.ImportantKwds)
                        {
                            var IsContain = categoryvalue.Contains(kwd);
                            if (IsContain)
                            {
                                node.Point++;
                            }
                        }
                        foreach(var category in SMCommodityCategoryString)
                        {
                            var IsContain = categoryvalue.Contains(category);
                            if(IsContain)
                            {
                                node.Point++;
                            }
                        }
                        pointNodes.Add(node);
                    }
                }
                var MaxPoint = pointNodes.Max(e => e.Point);
                var MaxPointNode = pointNodes.Find(e => e.Point.Equals(MaxPoint));
                SameKey = MaxPointNode.t.Category1 + " " + MaxPointNode.t.Category2 + " " + MaxPointNode.t.Category3 + " " + MaxPointNode.t.Category4;
            }
            return DicSameCategoryCommodityAnalyzeInfos[SameKey];
        }
        private async Task SaveCommodityAnalyziedInfos(List<CommodityAnalyzedInfo> commodityAnalyzedInfos, int FilterByCompetiveIndex, string SellerMCommodityId)
        {
            foreach (var value in commodityAnalyzedInfos)
            {
                var index = value.TotalCommodityCount / (value.MonthlyMobileQcCnt + value.MonthlyPcQcCnt);
                if (index <= FilterByCompetiveIndex)
                {
                    value.SellerMCommodityId = SellerMCommodityId;
                    await value.PostAsync(_sellerMarketDataContext);
                }
            }
        }
    }
    public class PointNode<T> : IComparable where T : class
    {
        public int Point = 0;
        public T t;
        public PointNode(T t)
        {
            this.t = t;
        }
        public int CompareTo(object? other)
        {
            if(other == null) { throw new ArgumentNullException(nameof(other)); }
            else
            {
                var compareother = (PointNode<T>)other;
                return Point.CompareTo(compareother.Point);
            }
        }
    }
}
