using CommunityToolkit.Mvvm.ComponentModel;
using SellerData.ofDataAccessLayer.ofDataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellerData.ViewModel.Common
{
    public class SellerObservableObject : CommonObservableObject
    {
        protected readonly SellerMarketDataContext _SellerMarketDataContext;
        public SellerObservableObject(SellerMarketDataContext sellerMarketDataContext)
        {
            _SellerMarketDataContext = sellerMarketDataContext;
        }
        public virtual async Task InitLoadAsync() { }
    }
}
