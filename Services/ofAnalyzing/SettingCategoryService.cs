using BusinessData.ofDataAccessLayer.ofModelExtenstions;
using SellerCommon.SellerData.Model;
using SellerCommon.SellerData.ofRepository.ofMarket;
using SellerData.ofDataAccessLayer.ofDataContext;
using SellerData.ofDataAccessLayer.ofRepository.ofMarket;

namespace SellerData.ViewModel.ofAnalyzing
{
    public interface ISettingSmartStoreCategoryService
    {
        Task SetCategory(SellerSMCommodity sellerSMCommodity);
    }
    public class SettingCategoryService : ISettingSmartStoreCategoryService
    {
        private readonly SellerMarketDataContext _sellerMarketDataContext;
        private readonly IOpenMarketCommonCategoryRepostiory _openMarketCommonCategoryRepostiory;
        private readonly ICommodityAnalyzedInfoRepository _commodityAnalyzedInfoRepository;
        public SettingCategoryService(SellerMarketDataContext sellerMarketDataContext, 
            IOpenMarketCommonCategoryRepostiory openMarketCommonCategoryRepostiory, 
            ICommodityAnalyzedInfoRepository commodityAnalyzedInfoRepository)
        {
            _sellerMarketDataContext = sellerMarketDataContext;
            _openMarketCommonCategoryRepostiory = openMarketCommonCategoryRepostiory;
            _commodityAnalyzedInfoRepository = commodityAnalyzedInfoRepository;
        }
        public async Task SetCategory(SellerSMCommodity sellerSMCommodity)
        {
            var SellerMCommodity = await _sellerMarketDataContext.GetByIdAsync<SellerMCommodity>(sellerSMCommodity.SellerMCommodityId);
            var CommodityAnalyzeInfos = await _commodityAnalyzedInfoRepository.GetToListByMCommodityIdAsync(SellerMCommodity.Id);
            if(CommodityAnalyzeInfos.Count == 0) { return; }
            var MinValue = CommodityAnalyzeInfos.Min(e => e.MonthlyMobileQcCnt);
            var InfoValue = CommodityAnalyzeInfos.FirstOrDefault(e => e.MonthlyMobileQcCnt.Equals(MinValue));
            var SmartStoreCategorys = await _openMarketCommonCategoryRepostiory.GetToListAsync();
            SmartStoreCategorys = SmartStoreCategorys.Where(e => e.Name.Equals("SmartStore")).ToList();
            var FindValue = SmartStoreCategorys.Find(
                e => e.Category1.Equals(InfoValue.Category1) &&
                e.Category2.Equals(InfoValue.Category2) &&
                e.Category3.Equals(InfoValue.Category3) &&
                e.Category4.Equals(InfoValue.Category4)
                );
            if(FindValue != null)
            {
                sellerSMCommodity.Code = FindValue.Code;
                await sellerSMCommodity.PutAsync(_sellerMarketDataContext);
            }
        }
    }
}
