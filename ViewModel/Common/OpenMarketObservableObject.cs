using SellerData.ofDataAccessLayer.ofDataContext;

namespace SellerData.ViewModel.Common
{
    public class OpenMarketObservableObject : CommonObservableObject
    {
        protected readonly OpenMarketDataContext _openMarketDataContext;
        public OpenMarketObservableObject(OpenMarketDataContext openMarketDataContext)
        {
            _openMarketDataContext = openMarketDataContext;
        }
        public virtual async Task InitLoadAsync() { }
    }
}
