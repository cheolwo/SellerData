using AutoMapper;
using SellerCommon.SellerData.Data;
using SellerCommon.SellerData.Model;
using SellerCommon.SellerData.Services;

namespace SellerData.Services
{
    /*
        var config = new MapperConfiguration(cfg => cfg.CreateMap<ExcellentCommoditPageXPathInfo, CommodityPageXPathInfo>());
        var mapper = config.CreateMapper();
        ExcellentCommoditPageXPathInfo excellentCommoditPageXPathInfo = new();
        CommodityPageXPathInfo dto = mapper.Map<CommodityPageXPathInfo>(excellentCommoditPageXPathInfo);
     */
    public interface IOnChannalMappingService
    {
        CommodityPageXPathInfo ExcellentPageXPathToCommodityPageXPathInfo(ExcellentCommoditPageXPathInfo excellentCommoditPageXPathInfo);
        CommodityPageXPathInfo DataCenterPageXPathInfoToCommodityPageXPathInfo(DataCenterCommodityPageXPathInfo dataCenterCommodityPageXPathInfo);
        SellerMCommodity MCommodityCollectInfoToSellerMCommodity(MCommodityCollectInfo mCommodityCollectInfo);
    }
    public class OnChannalMappingService : IOnChannalMappingService
    {
        private readonly Mapper ExcellentPageMapper;
        private readonly Mapper DataCenterPageMapper;
        private readonly Mapper MCommodityCollectInfoMapper;
        public OnChannalMappingService()
        {
            var ExcellentPageConfig = new MapperConfiguration(cfg => cfg.CreateMap<ExcellentCommoditPageXPathInfo, CommodityPageXPathInfo>());
            var DataCenterPageConfig = new MapperConfiguration(cfg => cfg.CreateMap<DataCenterCommodityPageXPathInfo, CommodityPageXPathInfo>());
            var MCommodityCollectInfoConfig = new MapperConfiguration(cfg => cfg.CreateMap<MCommodityCollectInfo, SellerMCommodity>());
            ExcellentPageMapper = new Mapper(ExcellentPageConfig);
            DataCenterPageMapper = new Mapper(DataCenterPageConfig);
            MCommodityCollectInfoMapper = new Mapper(MCommodityCollectInfoConfig);
        }
        public SellerMCommodity MCommodityCollectInfoToSellerMCommodity(MCommodityCollectInfo mCommodityCollectInfo)
        {
            return MCommodityCollectInfoMapper.Map<SellerMCommodity>(mCommodityCollectInfo);
        }
        public CommodityPageXPathInfo ExcellentPageXPathToCommodityPageXPathInfo(ExcellentCommoditPageXPathInfo excellentCommoditPageXPathInfo)
        {
            return ExcellentPageMapper.Map<CommodityPageXPathInfo>(excellentCommoditPageXPathInfo);
        }
        public CommodityPageXPathInfo DataCenterPageXPathInfoToCommodityPageXPathInfo(DataCenterCommodityPageXPathInfo dataCenterCommodityPageXPathInfo)
        {
            return DataCenterPageMapper.Map<CommodityPageXPathInfo>(dataCenterCommodityPageXPathInfo);
        }
    }
}
