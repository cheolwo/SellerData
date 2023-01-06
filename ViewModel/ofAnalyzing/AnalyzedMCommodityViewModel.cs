using SellerCommon.SellerData.Model;
using SellerCommon.SellerData.ofRepository.ofMarket;
using SellerCommon.SellerData.ViewModel.ofWholeSale;
using SellerData.ofDataAccessLayer.ofDataContext;
using SellerData.ViewModel.Common;

namespace SellerData.ViewModel.ofAnalyzing
{
    public class AnalyzedMCommodityViewModel : SellerObservableObject
    {
        private readonly ICommodityAnalyzedInfoRepository _CommodityAnalyzedInfoRepository;
        private readonly ISellerMarketRepository _SellerMarketRepository;
        private readonly ISellerMCommodityRepository _SellerMCommodityRepository;
        public AnalyzedMCommodityViewModel(ICommodityAnalyzedInfoRepository commodityAnalyzedInfoRepository,
            ISellerMarketRepository sellerMarketRepository,
            ISellerMCommodityRepository sellerMCommodityRepository,
                                                                        SellerMarketDataContext sellerMarketDataContext) : base(sellerMarketDataContext)
        {
            _CommodityAnalyzedInfoRepository = commodityAnalyzedInfoRepository;
            _SellerMCommodityRepository = sellerMCommodityRepository;
            _SellerMarketRepository = sellerMarketRepository;
        }
        public List<SellerMCommodity> SellerMCommodities = new();
        public SellerMarket SellerMarket = new();
        public List<CommodityAnalyzedInfo> commodityAnalyzedInfos = new();
        public override async Task InitLoadAsync()
        {
            if (!IsLoad)
            {
                SellerMarket = await _SellerMarketRepository.GetByNameAsync(OnChannal.OnChannalName);
                SellerMCommodities = await _SellerMCommodityRepository.GetToListByCenterIdAndWithAnalyzedInfos(SellerMarket.Id);
                SellerMCommodities = SellerMCommodities.Where(e => e.CommodityAnalyzedInfos.Count > 0).ToList();
                IsLoad = true;
            }
        }
        public async Task ReLoad()
        {
            SellerMCommodities = new();
            SellerMCommodities = await _SellerMCommodityRepository.GetToListByCenterIdAndWithAnalyzedInfos(SellerMarket.Id);
            SellerMCommodities = SellerMCommodities.Where(e => e.CommodityAnalyzedInfos.Count > 0).ToList();
        }
        public async Task Delete(string id)
        {
            IsBusy = true;
            var SellerMCommodity = SellerMCommodities.FirstOrDefault(e => e.Id.Equals(id));
            foreach(var value in SellerMCommodity.CommodityAnalyzedInfos.ToList())
            {
                await _CommodityAnalyzedInfoRepository.DeleteByIdAsync(value.Id);
            }
            SellerMCommodities.Remove(SellerMCommodity);
            IsBusy = false;
        }
    }
}
