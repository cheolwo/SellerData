using BusinessData.ofDataAccessLayer.ofModelExtenstions;
using Microsoft.Extensions.Caching.Distributed;
using SellerCommon.SellerData.Model;
using SellerData.ofDataAccessLayer.ofDataContext;
using SellerData.ofDataAccessLayer.ofRepository.ofMarket;
using SellerData.ViewModel.Common;

namespace SellerData.ViewModel.ofMarketing
{
    public class OpenMarketViewModel : SellerObservableObject
    {
        public OpenMarketViewModel(SellerMarketDataContext sellerMarketDataContext) : base(sellerMarketDataContext)
        {
           
        }
        private bool _IsLoad;
        public bool IsLoad
        {
            get => _IsLoad;
            set => SetProperty(ref _IsLoad, value);
        }
        private bool _IsBusy;
        public bool IsBusy
        {
            get => _IsBusy;
            set => SetProperty(ref _IsBusy, value);
        }
        private double _MarginRate;
        public double MarginRate
        {
            get => _MarginRate;
            set => SetProperty(ref _MarginRate, value);
        }
        public SellerOpenMarket OpenMarket = new();
        public List<SellerOpenMarket> OpenMarkets = new();
        public override async Task InitLoadAsync()
        {
            IsBusy = true;
            OpenMarkets = await _SellerMarketDataContext.GetsAsync<SellerOpenMarket>();
            IsBusy = false;
        }
        public async Task AddAsync()
        {
            IsBusy = true;
            await OpenMarket.PostAsync(_SellerMarketDataContext);
            IsBusy = false;
        }
        public async Task UpdateAsync()
        {
            IsBusy = true;
            await OpenMarket.PutAsync(_SellerMarketDataContext);
            IsBusy = false;
        }
    }
}
