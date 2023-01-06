using SellerCommon.SellerData.Model;
using SellerCommon.SellerData.ofRepository.ofMarket;
using SellerData.ofDataAccessLayer.ofDataContext;
using SellerData.Services.ofCollecitng;
using SellerData.ViewModel.Common;

namespace SellerData.ViewModel.ofConfiguring
{
    public class ImagingSMCommodityViewModel : SellerObservableObject
    {
        private readonly ICommodityImageStoringService _commodityImageStoringService;
        private readonly ISellerSMCommodityRepository _sellerSMCommodityRepository;
        private readonly ISellerMCommodityRepository _sellerMCommodityRepository;
        private readonly ISellerMMCommodityRepository _sellerMMCommodityRepostiory;
        public ImagingSMCommodityViewModel(SellerMarketDataContext sellerMarketDataContext,
                                                                        ICommodityImageStoringService commodityImageStoringService,
                                                                        ISellerSMCommodityRepository sellerSMCommodityRepository,
                                                                        ISellerMCommodityRepository sellerMCommodityRepository,
                                                                        ISellerMMCommodityRepository sellerMMCommodityRepostiory)
                                                                                                            : base(sellerMarketDataContext)
        {
            _commodityImageStoringService = commodityImageStoringService;
            _sellerSMCommodityRepository = sellerSMCommodityRepository;
            _sellerMCommodityRepository = sellerMCommodityRepository;
            _sellerMMCommodityRepostiory = sellerMMCommodityRepostiory;
        }
        public List<SellerSMCommodity> ConfiguredImgaeSellerSMCommodities = new();
        public List<SellerSMCommodity> ExportingSellerSMCommodities = new();
        public List<SellerOpenMarket> sellerOpenMarkets = new();
        private SellerOpenMarket ExportingOpenMarket = new();
        public List<SellerSMCommodity> NotRegisterToSmartStoreSMCommodities = new();
        public override async Task InitLoadAsync()
        {
            if (!IsLoad)
            {
                ConfiguredImgaeSellerSMCommodities = await _sellerSMCommodityRepository.GetToListByIsConfiguredImgaeAsync();
                ConfiguredImgaeSellerSMCommodities = ConfiguredImgaeSellerSMCommodities.Where(e => e.IsConfiguringofImage).ToList();
                sellerOpenMarkets = await _SellerMarketDataContext.GetsAsync<SellerOpenMarket>();
                var SmartStore = sellerOpenMarkets.FirstOrDefault(e => e.Name.Equals("SmartStore"));
                ExportingOpenMarket = SmartStore;
                foreach (var value in ConfiguredImgaeSellerSMCommodities)
                {
                    var commodity = await _sellerMMCommodityRepostiory.GetBySMCommodityIdAndMarketId(value, SmartStore);
                    if (commodity == null)
                    {
                        NotRegisterToSmartStoreSMCommodities.Add(value);
                    }
                }
                IsLoad = true;
            }
        }
        public async Task StoringConfiguredImgae()
        {
            foreach(var value in NotRegisterToSmartStoreSMCommodities)
            {
                var SellerMCommodity = await _sellerMCommodityRepository.GetByIdAsync(value.SellerMCommodityId);
                if(SellerMCommodity == null) { throw new ArgumentException(nameof(SellerSMCommodity)); }
                await _commodityImageStoringService.DistintStoringToBlobStorage(SellerMCommodity);
            }          
        }
        public async Task ResizingAdditionalImgae()
        {
            foreach(var value in NotRegisterToSmartStoreSMCommodities)
            {
                var SellerMCommodity = await _sellerMCommodityRepository.GetByIdAsync(value.SellerMCommodityId);
                if (SellerMCommodity == null) { throw new ArgumentException(nameof(SellerSMCommodity)); }
                await _commodityImageStoringService.ResizingAdditionalImageInBlobContainer(SellerMCommodity, 1000, 1000);
            }
        }
    }
}
