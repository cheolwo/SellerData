using SellerData.ofDataAccessLayer.ofDataContext;
using SellerData.ofDataAccessLayer.ofModel;
using SellerData.Services.ofConfiguring;

namespace SellerData.ViewModel.Common
{
    public class CommodityCodeViewModel : OpenMarketObservableObject
    {
        private readonly ICommodityCategoryCodeService _commodityCategoryCodeService;
        private readonly ICommodityDeliveryCodeService _commodityDeliveryCodeService;
        private readonly ICommodityOriginCodeService _commodityOriginCodeService;
        public CommodityCodeViewModel(OpenMarketDataContext openMarketDataContext,
                                                            ICommodityCategoryCodeService commodityCategoryCodeService,
                                                             ICommodityDeliveryCodeService commodityDeliveryCodeService,
                                                             ICommodityOriginCodeService commodityOriginCodeService) : base(openMarketDataContext)
        {
            _commodityCategoryCodeService = commodityCategoryCodeService;
            _commodityDeliveryCodeService = commodityDeliveryCodeService;
            _commodityOriginCodeService = commodityOriginCodeService;
        }
        public List<OpenMarketCommonCategory> openMarketCommonCategories = new();
        public List<CommodityDeliveryCode> commodityDeliveryCodes = new();
        public List<CommodityOriginCode> commodityOriginCodes = new();
        public override async Task InitLoadAsync()
        {
            openMarketCommonCategories = await _openMarketDataContext.GetsAsync<OpenMarketCommonCategory>();
            commodityDeliveryCodes = await _openMarketDataContext.GetsAsync<CommodityDeliveryCode>();
            commodityOriginCodes = await _openMarketDataContext.GetsAsync<CommodityOriginCode>();
        }
        public async Task SetOriginCodeAsync(string path)
        {
            await _commodityOriginCodeService.SetCodeByExcelData(path);
        }
        public async Task SetCategoryCodeAsync(string path)
        {
            await _commodityCategoryCodeService.StoringExcelCommonCategoryList(path);
        }
        public async Task SetDeliveryCodeAsync(string path)
        {
            await _commodityDeliveryCodeService.SetCodeByExcelData(path);
        }
    }
}
