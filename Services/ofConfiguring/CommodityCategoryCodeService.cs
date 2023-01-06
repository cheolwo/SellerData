using BusinessData.ofDataAccessLayer.ofModelExtenstions;
using Microsoft.Office.Interop.Excel;
using SellerCommon.SellerData.Model;
using SellerCommon.SellerData.ofRepository.ofMarket;
using SellerCommon.SellerData.Services;
using SellerData.ofDataAccessLayer.ofDataContext;
using SellerData.ofDataAccessLayer.ofModel;
using SellerData.ofDataAccessLayer.ofRepository.ofMarket;

namespace SellerData.Services.ofConfiguring
{
    public interface ICommodityCategoryCodeService
    {
        Task StoringExcelCommonCategoryList(string Path);
        Task SetCode(SellerSMCommodity sellerSMCommodity);
    }
    public class CommodityCategoryCodeService : ICommodityCategoryCodeService
    {
        private readonly IOpenMarketCommonCategoryRepostiory _openMarketCommonCategoryRepostiory;
        private readonly ICommodityAnalyzedInfoRepository _commodityAnalyzedInfoRepository;
        private readonly INaverSearchShoppingAPIService _naverSearchShoppingAPIService;
        private readonly SellerMarketDataContext _sellerMarketDataContext;
        private readonly OpenMarketDataContext _openMarketDataContext;
        private Application excelApp;
        public CommodityCategoryCodeService(OpenMarketDataContext openMarketDataContext, SellerMarketDataContext sellerMarketDataContext,
                                                                        IOpenMarketCommonCategoryRepostiory openMarketCommonCategoryRepostiory, ICommodityAnalyzedInfoRepository commodityAnalyzedInfoRepository, INaverSearchShoppingAPIService naverSearchShoppingAPIService)
        {
            _openMarketDataContext = openMarketDataContext;
            _sellerMarketDataContext = sellerMarketDataContext;
            _openMarketCommonCategoryRepostiory = openMarketCommonCategoryRepostiory;
            _commodityAnalyzedInfoRepository = commodityAnalyzedInfoRepository;
            _naverSearchShoppingAPIService = naverSearchShoppingAPIService;
        }
        private OpenMarketCommonCategory OpenMarketCommonCategory = new();
        public List<OpenMarketCommonCategory> openMarketCommonCategories = new();
        public async Task StoringExcelCommonCategoryList(string Path)
        {
            excelApp = new();
            excelApp.Visible = true;
            Workbook workbook = excelApp.Workbooks.Open(Path);
            Worksheet workSheet = excelApp.ActiveSheet;
            Console.WriteLine(workSheet.Cells[1, 1].Value);
            for (int i = 2; i <= 13103; i++)
            {
                OpenMarketCommonCategory.Code = workSheet.Cells[i, 1].Value?.ToString() ?? "";
                OpenMarketCommonCategory.Category1 = workSheet.Cells[i, 2].Value?.ToString() ?? "";
                OpenMarketCommonCategory.Category2 = workSheet.Cells[i, 3].Value?.ToString() ?? "";
                OpenMarketCommonCategory.Category3 = workSheet.Cells[i, 4].Value?.ToString() ?? "";
                OpenMarketCommonCategory.Category4 = workSheet.Cells[i, 5].Value?.ToString() ?? "";
                await OpenMarketCommonCategory.PostAsync(_openMarketDataContext);
                OpenMarketCommonCategory.Id = null;
            }
        }
        public async Task SetCode(SellerSMCommodity sellerSMCommodity)
        {
            if (sellerSMCommodity.SellerMCommodity == null) { sellerSMCommodity.SellerMCommodity = await _sellerMarketDataContext.GetByIdAsync<SellerMCommodity>(sellerSMCommodity.SellerMCommodityId); }
            var SellerMCommodity = sellerSMCommodity.SellerMCommodity;
            var SmartStoreCategorys = await _openMarketCommonCategoryRepostiory.GetToListAsync();
            SmartStoreCategorys = SmartStoreCategorys.Where(e => e.Name.Equals("SmartStore")).ToList();
            var CommodityAnalyzeInfos = await _commodityAnalyzedInfoRepository.GetToListByMCommodityIdAsync(SellerMCommodity.Id);
            if (CommodityAnalyzeInfos.Count == 0)
            {
                if (sellerSMCommodity.Name != null)
                {
                    CommodityAnalyzedInfo InfoValue = await _naverSearchShoppingAPIService.SearchingByKeywords(sellerSMCommodity.Name);
                    InfoValue.SellerMCommodityId = sellerSMCommodity.SellerMCommodityId;
                    await InfoValue.PostAsync(_sellerMarketDataContext);
                    var FindValue = SmartStoreCategorys.Find(
                                e => e.Category1.Equals(InfoValue.Category1) &&
                                e.Category2.Equals(InfoValue.Category2) &&
                                e.Category3.Equals(InfoValue.Category3) &&
                                e.Category4.Equals(InfoValue.Category4)
                            );
                    if (FindValue != null)
                    {
                        sellerSMCommodity.Code = FindValue.Code;
                        await sellerSMCommodity.PutAsync(_sellerMarketDataContext);
                    }
                    return;
                }
                if (sellerSMCommodity.SellerMCommodity.Name != null)
                {
                    CommodityAnalyzedInfo InfoValue = await _naverSearchShoppingAPIService.SearchingByKeywords(sellerSMCommodity.SellerMCommodity.Name);
                    InfoValue.SellerMCommodityId = sellerSMCommodity.SellerMCommodityId;
                    await InfoValue.PostAsync(_sellerMarketDataContext);
                    var FindValue = SmartStoreCategorys.Find(
                                e => e.Category1.Equals(InfoValue.Category1) &&
                                e.Category2.Equals(InfoValue.Category2) &&
                                e.Category3.Equals(InfoValue.Category3) &&
                                e.Category4.Equals(InfoValue.Category4)
                            );
                    if (FindValue != null)
                    {
                        sellerSMCommodity.Code = FindValue.Code;
                        await sellerSMCommodity.PutAsync(_sellerMarketDataContext);
                    }
                    return;
                }
            }
            else
            {
                var MinValue = CommodityAnalyzeInfos.Min(e => e.MonthlyMobileQcCnt);
                var InfoValue = CommodityAnalyzeInfos.FirstOrDefault(e => e.MonthlyMobileQcCnt.Equals(MinValue));
                var FindValue = SmartStoreCategorys.Find(
                    e => e.Category1.Equals(InfoValue.Category1) &&
                    e.Category2.Equals(InfoValue.Category2) &&
                    e.Category3.Equals(InfoValue.Category3) &&
                    e.Category4.Equals(InfoValue.Category4)
                    );
                if (FindValue != null)
                {
                    sellerSMCommodity.Code = FindValue.Code;
                    await sellerSMCommodity.PutAsync(_sellerMarketDataContext);
                }
            }

        }
    }
}
