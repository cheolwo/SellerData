using SellerCommon.SellerData.Model;
using SellerData.ofDataAccessLayer.ofDataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellerData.Services
{
    public class NaverShoppingScarppingService
    {
        private readonly SellerMarketDataContext _sellerMarketDataContext;
        public NaverShoppingScarppingService(SellerMarketDataContext sellerMarketDataContext)
        {
            _sellerMarketDataContext = sellerMarketDataContext;
        }
        /// <summary>
        /// 1. 네이버 쇼핑 URL로 접근하는 단계
        /// 2. SellerMCommodity의 중요 키워드로 검색하는 단계
        /// 3. 검색한 것 중 
        /// </summary>
        /// <param name="sellerMCommodities"></param>
        /// <returns></returns>
        public async Task ScrappingMostNameofCommodity(List<SellerMCommodity> sellerMCommodities)
        {
            
        }
    }
}
