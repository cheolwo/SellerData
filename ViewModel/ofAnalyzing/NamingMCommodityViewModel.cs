using BusinessData.ofDataAccessLayer.ofModelExtenstions;
using SellerCommon.SellerData.Model;
using SellerCommon.SellerData.ofRepository.ofMarket;
using SellerData.ofDataAccessLayer.ofDataContext;
using SellerData.ViewModel.Common;

namespace SellerData.ViewModel.ofAnalyzing
{
    public class NamingMCommodityViewModel : SellerObservableObject
    {
        private readonly ISellerMCommodityRepository _SellerMCommodityRepository; 
        public NamingMCommodityViewModel(SellerMarketDataContext sellerMarketDataContext, ISellerMCommodityRepository sellerMCommodityRepository)
            :base(sellerMarketDataContext)
        {
            _SellerMCommodityRepository = sellerMCommodityRepository;
        }
        public string _NameofCommodity;
        public string NameofCommodity
        {
            get => _NameofCommodity;
            set => SetProperty(ref _NameofCommodity, value);
        }
        public SellerMCommodity SellerMCommodity = new();
        public SellerSMCommodity SellerSMCommodity = new();
        public async Task InitLoadAsync(string id)
        {
            SellerMCommodity.CommodityAnalyzedInfos = new();
            SellerMCommodity = await _SellerMCommodityRepository.GetByIdWithAnalyzedInfos(id);
        }
        public async Task Configuring()
        {
            if(NameofCommodity != null)
            {
                SellerSMCommodity.Name = _NameofCommodity;
                await SellerSMCommodity.PostAsync(_SellerMarketDataContext);
            }
        }
        public void SelectingKwd(string keyword)
        {
            _NameofCommodity = _NameofCommodity + " " + keyword;
        }
        
    }
}
