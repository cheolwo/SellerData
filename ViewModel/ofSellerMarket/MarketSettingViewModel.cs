using BusinessData.ofDataAccessLayer.ofModelExtenstions;
using SellerCommon.SellerData.Model;
using SellerData.ofDataAccessLayer.ofDataContext;
using SellerData.ofDataAccessLayer.ofModel;
using SellerData.ViewModel.Common;

namespace SellerData.ViewModel.ofSellerMarket
{
    public class MarketSettingViewModel : SellerObservableObject
    {
        private readonly OpenMarketDataContext _openMarketDataContext;
        public MarketSettingViewModel(SellerMarketDataContext sellerMarketDataContext, OpenMarketDataContext openMarketDataContext)
            : base(sellerMarketDataContext)
        {
            _openMarketDataContext = openMarketDataContext; 
        }
        public List<OpenMarketCommonCategory> openMarketCategories = new();
        public List<SellerMarket> sellerMarkets = new();
        public SellerMarket sellerMarket = new();
        public override async Task InitLoadAsync()
        {
            openMarketCategories = await _openMarketDataContext.GetsAsync<OpenMarketCommonCategory>();
            sellerMarkets = await _SellerMarketDataContext.GetsAsync<SellerMarket>();
        }
        public async Task SetFilterCategory(string categoryCode)
        {
            if(sellerMarket.FilterCategoryCodes == null)
            {
                sellerMarket.FilterCategoryCodes = new();
            }
            sellerMarket.FilterCategoryCodes.Add(categoryCode);
            await sellerMarket.PutAsync(_SellerMarketDataContext);
        }
        public async Task CreateMarket()
        {

        }
        public async Task FilteredFashionCategory()
        {
            var FashtionCategories = openMarketCategories.Where(e => e.Category1.Contains("패션")).ToList();
            if (sellerMarket.FilterCategoryCodes == null)
            {
                sellerMarket.FilterCategoryCodes = new();
            }
            foreach (var category in FashtionCategories)
            {
                if(category.Code == null) { continue; }
                if(sellerMarket.FilterCategoryCodes.FirstOrDefault(e=>e.Equals(category.Code)) == null)
                {
                    sellerMarket.FilterCategoryCodes.Add(category.Code);
                }
            }
            await sellerMarket.PutAsync(_SellerMarketDataContext);
        }
        public void SetMarket(string id)
        {
            sellerMarket = sellerMarkets.FirstOrDefault(e => e.Id.Equals(id));
        }
    }
}
