using BusinessData.ofDataAccessLayer.ofCommon;
using BusinessData.ofDataAccessLayer.ofCommon.ofAttribute;
using BusinessData.ofDataAccessLayer.ofMarket.ofModel;
using SellerData.Data;
using SellerData.ofDataAccessLayer.ofDataContext;
using SellerData.ofDataAccessLayer.ofDbContext;

namespace SellerData.ofDataAccessLayer.ofModel
{
    [DbContext(typeof(OpenMarketDbContext), SellerDbConnectionString.OpenMarketDbConnection)]
    [DataContext(typeof(OpenMarketDataContext))]
    [Relation(typeof(OpenMarket), nameof(OpenMarket))]
    public class OpenMarket : Market
    {
        public float CommissionFee { get; set; }
        public string AccessKey { get; set; }
        public string ClientId { get; set; }
        public string SecretKey { get; set; }
        public List<OpenMarketCategory> OpenMarketCategories {get; set;}
    }
    [DbContext(typeof(OpenMarketDbContext), SellerDbConnectionString.OpenMarketDbConnection)]
    [DataContext(typeof(OpenMarketDataContext))]
    [Relation(typeof(OpenMarketCategory), nameof(OpenMarketCategory))]
    public class OpenMarketCategory : Entity
    {
        public double CommoissionRate {get; set;}
        public string OpenMarketId {get; set;}
        public OpenMarket OpenMarket {get ;set;}
    }
    [DbContext(typeof(OpenMarketDbContext), SellerDbConnectionString.OpenMarketDbConnection)]
    [DataContext(typeof(OpenMarketDataContext))]
    [Relation(typeof(OpenMarketCommonCategory), nameof(OpenMarketCommonCategory))]
    public class OpenMarketCommonCategory : Entity
    {
        public string Category1 { get; set; }
        public string Category2 { get; set; }
        public string Category3 { get; set; }
        public string Category4 { get; set; }
    }
    [DbContext(typeof(OpenMarketDbContext), SellerDbConnectionString.OpenMarketDbConnection)]
    [DataContext(typeof(OpenMarketDataContext))]
    [Relation(typeof(CommodityOriginCode), nameof(CommodityOriginCode))]
    public class CommodityOriginCode : Entity
    {

    }
    [DbContext(typeof(OpenMarketDbContext), SellerDbConnectionString.OpenMarketDbConnection)]
    [DataContext(typeof(OpenMarketDataContext))]
    [Relation(typeof(CommodityDeliveryCode), nameof(CommodityDeliveryCode))]
    public class CommodityDeliveryCode : Entity
    {

    }

}
