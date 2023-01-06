using BusinessData.ofPresentationLayer.ofDTO.ofMarket;
using BusinessData.ofPresentationLayer.ofDTOContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellerData.ofPresentationLayer.ofDTO.ofMarket
{
    [DTOContext(typeof(MarketDTOContext))]
    public class SellerMarketDTO : MarketDTO
    {
    }
    [DTOContext(typeof(MarketDTOContext))]
    public class SellerMCommodityDTO : MCommodityDTO
    {
    }
    [DTOContext(typeof(MarketDTOContext))]
    public class SellerSMCommodityDTO : SMCommodityDTO
    {
    }
    [DTOContext(typeof(MarketDTOContext))]
    public class SellerMMCommodityDTO : MMCommodityDTO
    {
    }
    [DTOContext(typeof(MarketDTOContext))]
    public class SellerEMCommodityDTO : EMCommodityDTO
    {
    }
    [DTOContext(typeof(MarketDTOContext))]
    public class SellerPlatMarketDTO : PlatMarketDTO
    {
    }
}
