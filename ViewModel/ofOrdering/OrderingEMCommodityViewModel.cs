using Microsoft.Extensions.Configuration;
using SellerData.ofDataAccessLayer.ofDataContext;
using SellerData.ViewModel.Common;

namespace SellerData.ViewModel.ofOrdering
{
    public class OrderingEMCommodityViewModel : SellerObservableObject
    {
        private readonly IConfiguration _configuration;
        private string Och3OrderingExcelFilePath = "";
      
        public OrderingEMCommodityViewModel(SellerMarketDataContext sellerMarketDataContext, IConfiguration configuration)
                :base(sellerMarketDataContext)
        {
            _configuration = configuration;
            Och3OrderingExcelFilePath = _configuration.GetSection("Ordering")["Onch"];
        }
        public override async Task InitLoadAsync()
        {
            if (!IsLoad)
            {
            }
        }
        public async Task RegisterOrderInfo()
        {
            
        }
    }
}
