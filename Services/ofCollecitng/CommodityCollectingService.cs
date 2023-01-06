using Microsoft.Extensions.Configuration;
using SellerCommon.SellerData.Model;
using SellerCommon.SellerData.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellerData.Services.ofCollecitng
{
    public interface ICommodityCollectingService
    {
        Task<List<SellerMCommodity>> CollecintgCommodityInfo(PlatformCode platformCode, string PageCode, string url);
    }
    public class CommodityCollectingService : ICommodityCollectingService
    {
        private readonly Och3CommodityPageScrappingService _Och3CommodityPageScrappingService;
        public CommodityCollectingService(Och3CommodityPageScrappingService och3CommodityPageScrappingService)
        {
            _Och3CommodityPageScrappingService = och3CommodityPageScrappingService;
        }
        public async Task<List<SellerMCommodity>> CollecintgCommodityInfo(PlatformCode platformCode, string PageCode, string url)
        {
            switch(platformCode)
            {
                case PlatformCode.OnChannal:
                    return await _Och3CommodityPageScrappingService.Scrapping(PageCode, url);
                default:
                    throw new ArgumentException("Not Support Platform Code");
            }
        }
    }
}
