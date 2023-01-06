using BusinessData.ofDataAccessLayer.ofMarket.ofModel;
using BusinessData.ofDataAccessLayer.ofCommon;
using SellerCommon.SellerData.ofDbContext;
using SellerData.Data;
using SellerData.ofDataAccessLayer.ofDataContext;
using BusinessData.ofDataAccessLayer.ofCommon.ofAttribute;
using BusinessData.ofDataAccessLayer.ofModelExtenstions;
using System.Text;

namespace SellerCommon.SellerData.Model
{
    /*
    제품명
    추천키워드
    배송비/택배사
    주문마감
    제품코드
    대표이미지경로
    상세이미지경로
    추가이미지경로
    */
    //[DbContext(typeof(OnChannalDbContext), SellerDbConnectionString.OnChannelDbConnection)]
    //[Relation(typeof(OnChannal), nameof(OnChannal))]
    //public class OnChannal : Entity
    //{
    //    public List<OnChannalPage> OnChannalPageRoutes;
    //    public List<OnChannalCommodity> OnChannalCommodities;
    //    public List<OnChannalPageCommodity> OnChannalPageCommodities; 
    //}
    //[DbContext(typeof(OnChannalDbContext), SellerDbConnectionString.OnChannelDbConnection)]
    //[Relation(typeof(OnChannalPageCommodity), nameof(OnChannalPageCommodity))]
    //public class OnChannalPageCommodity : Entity
    //{
    //    public OnChannalCommodity OnChannalCommodity;
    //    public OnChannalPage OnChannalPage;
    //}
    //[DbContext(typeof(OnChannalDbContext), SellerDbConnectionString.OnChannelDbConnection)]
    //[Relation(typeof(OnChannalPage), nameof(OnChannalPage))]
    //public class OnChannalPage : Entity
    //{

    //}
    //[DbContext(typeof(OnChannalDbContext), SellerDbConnectionString.OnChannelDbConnection)]
    //[Relation(typeof(OnChannalCommodity), nameof(OnChannalCommodity))]
    //public class OnChannalCommodity : Entity
    //{
    //    public string href;
    //    public string KeyWords;
    //    public string Category;
    //    public string DeliveryFee;
    //    public string NameofDeliveryAgency;
    //    public string RepresentativeImage;
    //    public List<string> AdditionalImages;
    //    public List<string> DetailImages;
    //    public string OnChannalId;

    //    public OnChannal OnChannal;
    //}
    public static class ImageSellerMarket
    {
        public const string DeliveryInfoImage = "배송안내 이미지";
    }
    [DataContext(typeof(SellerMarketDataContext))]
    [DbContext(typeof(SellerMarketDbContext), SellerDbConnectionString.MarketDbConnection)]
    [Relation(typeof(SellerMarket), nameof(SellerMarket))]
    public class SellerMarket : Market
    {
        public string? AsPhoneNumber { get; set; }
        public string? AsInfo { get; set; }
        public List<string>? FilterCategoryCodes { get; set; }
        public List<SellerMCommodity> SellerMCommodities { get; set; }
        public List<SellerSMCommodity> SellerSMCommodities { get; set; }
        public List<SellerMMCommodity> SellerMMCommodities { get; set; }
        public List<SellerEMCommodity> SellerEMCommodities { get; set; }
        public async Task<string?> GetSrcofDeliveryInfoImage(SellerMarketDataContext _sellerMarketDataContext, string _connectionString)
        {
            string Src = null;
            var ImageSrcs = await this.GetBlobItemsAsync(_sellerMarketDataContext, _connectionString);
            Src = ImageSrcs.FirstOrDefault(e => e.Contains(ImageSellerMarket.DeliveryInfoImage));
            return Src;
        }
    }
    [DataContext(typeof(SellerMarketDataContext))]
    [DbContext(typeof(SellerMarketDbContext), SellerDbConnectionString.MarketDbConnection)]
    [Relation(typeof(SellerPlatMarket), nameof(PlatMarket))]
    public class SellerPlatMarket : PlatMarket
    {
    }
    public class CommodityOption
    {
        public string OptionName { get; set; }
        public string SalesType { get; set; }
        public string SellerPrice { get; set; }
        public string ConsumerPrice { get; set; }
    }
    [DataContext(typeof(SellerMarketDataContext))]
    [DbContext(typeof(SellerMarketDbContext), SellerDbConnectionString.MarketDbConnection)]
    [Relation(typeof(SellerMCommodity), nameof(SellerMCommodity))]
    public class SellerMCommodity : MCommodity
    {
        public string? Keywords { get; set; }
        public string? Category { get; set; }
        public string? DeliveryContent { get; set; }
        public string? CommodityPageUrl { get; set; }
        public string? RepresentativeImageUrl { get; set; }
        public List<string> DetailImages = new();
        public List<string>? ImportantKwds = new();
        public List<CommodityAnalyzedInfo> CommodityAnalyzedInfos { get; set; }
        public List<SellerSMCommodity> SellerSMCommodities { get; set; }
        public List<SellerMMCommodity> SellerMMCommodities { get; set; }
        public List<SellerEMCommodity> SellerEMCommodities { get; set; }
        public List<ImportantKeywordNode>? ImportantKeywordNodes { get; set; }
        public Dictionary<string, string>? DetailCommodityInfo { get; set; }
        public List<CommodityOption>? CommodityOptions { get; set; }
        public string? SellerMarketId { get; set; }
        public SellerMarket? SellerMarket { get; set; }

        public override bool Equals(object obj)
        {
            /*
                    return base.Equals(obj) && obj is Commodity commodity &&
                   Name == commodity.Name &&
                   Barcode == commodity.Barcode;
             */
            return base.Equals(obj) && obj is SellerMCommodity sellerMCommodity &&
                CommodityPageUrl == sellerMCommodity.CommodityPageUrl;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public List<string> ConfigureHintKwdByImportantKwds()
        {
            List<string> strings = new();
            if(ImportantKwds != null)
            {
                if (ImportantKwds.Count > 1)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    for (int i = 0; i < ImportantKwds.Count; i++)
                    {
                        strings.Add(ImportantKwds[i]);
                    }
                    return strings;
                }
                if (ImportantKwds.Count == 1)
                {
                    strings.Add(ImportantKwds[0]);
                    return strings;
                }
                if(ImportantKwds.Count == 0)
                {
                    return strings;
                }
            }
            return strings;
        }
       public string ToStringImportantKwds()
        {
            StringBuilder stringBuilder = new();
            if(ImportantKwds.Count > 0 && ImportantKwds != null)
            {
                foreach(var value in ImportantKwds)
                {
                    stringBuilder.Append(value.ToString());
                    stringBuilder.Append(" ");
                }
                return stringBuilder.ToString();
            }
            return "";
        }
        public string ToStringCombinationForm()
        {
            StringBuilder stringBuilder = new();
            if (ImportantKwds != null)
            {
                string ImportantKwd = ImportantKwds[0];
                for (int i = 1; i < ImportantKwds.Count; i++)
                {
                    var value = ImportantKwds[i].Split(ImportantKwd);
                    stringBuilder.Append(value[0]);
                    stringBuilder.Append(" ");
                }
                stringBuilder.Append(ImportantKwd);
            }
            return stringBuilder.ToString();
        }
        public string ToStringMixAndMainForm()
        {
            StringBuilder stringBuilder = new();
            if (ImportantKwds != null)
            {
                string ImportantKwd = ImportantKwds[0];
                for (int i = 1; i < ImportantKwds.Count; i++)
                {
                    stringBuilder.Append(ImportantKwds[i]);
                    stringBuilder.Append(" ");
                }
                stringBuilder.Append(ImportantKwd);
            }
            return stringBuilder.ToString();
        }
        public string ToStringExceptForIndex0()
        {
            StringBuilder stringBuilder = new();
            if (ImportantKwds != null)
            {
                for (int i = 1; i < ImportantKwds.Count; i++)
                {
                    stringBuilder.Append(ImportantKwds[i]);
                    stringBuilder.Append(" ");
                }
            }
            return stringBuilder.ToString();
        }
    }
    public class ImportantKeywordNode
    {
        public string? ImportantKeyword;
        public List<string>? RelatedKeywords;
    }
    [DataContext(typeof(SellerMarketDataContext))]
    [DbContext(typeof(SellerMarketDbContext), SellerDbConnectionString.MarketDbConnection)]
    [Relation(typeof(SellerSMCommodity), nameof(SellerSMCommodity))]
    public class SellerSMCommodity : SMCommodity
    {
        public string? DeliveryCode { get; set; }
        public string? OriginCode { get; set; }
        public bool IsNaming { get; set; }
        public bool IsConfiguringofImage { get; set; }
        public bool IsReimaging { get; set; }
        public bool IsSmartStoreExporting { get; set; }
        public string? ImportantKwdType { get; set; }
        public int? CommonDeliveryFee { get; set; }
        public int? JejudoDeliveryFee { get; set; }
        public int? MountainDeliveryFee { get; set; }
        public string? SellerMarketId { get; set; }
        public SellerMarket? SellerMarket { get; set; }
        public string? SellerMCommodityId { get; set; }
        public SellerMCommodity? SellerMCommodity { get; set; }
        public List<SellerMMCommodity>? SellerMMCommodities { get; set; }
    }
    [DataContext(typeof(SellerMarketDataContext))]
    [DbContext(typeof(SellerMarketDbContext), SellerDbConnectionString.MarketDbConnection)]
    [Relation(typeof(CommodityAnalyzedInfo), nameof(CommodityAnalyzedInfo))]
    public class CommodityAnalyzedInfo : Entity, IComparable<CommodityAnalyzedInfo>
    {
        public string? SeletedKeywords { get; set; }
        public int TotalCommodityCount { get; set; }
        public string? Category1 { get; set; }
        public string? Category2 { get; set; }
        public string? Category3 { get; set; }
        public string? Category4 { get; set; }
        public int MonthlyPcQcCnt { get; set; }
        public int MonthlyMobileQcCnt { get; set; }
        public SellerMCommodity? SellerMCommodity { get; set; }
        public string? SellerMCommodityId { get; set; }

        public int CompareTo(CommodityAnalyzedInfo? other)
        {
            if (other == null) { return 1; }
            else
            {
                return MonthlyMobileQcCnt.CompareTo(other.MonthlyMobileQcCnt);
            }
        }
    }
    [DataContext(typeof(SellerMarketDataContext))]
    [DbContext(typeof(SellerMarketDbContext), SellerDbConnectionString.MarketDbConnection)]
    [Relation(typeof(SellerMMCommodity), nameof(SellerMMCommodity))]
    public class SellerMMCommodity : MMCommodity
    {
        public string? SellerMCommodityId { get; set; }
        public SellerMCommodity? SellerMCommodity { get; set; }
        public string? SellerSMCommodityId { get; set; }
        public SellerSMCommodity? SellerSMCommodity { get; set; }
        public string? SellerMarketId { get; set; }
        public SellerMarket? SellerMarket { get; set; }
        public string? SellerOpenMarketId { get; set; }
        public SellerOpenMarket? SellerOpenMarket { get; set; }
        public List<SellerEMCommodity> SellerEMCommodities { get; set; }
    }
    [DataContext(typeof(SellerMarketDataContext))]
    [DbContext(typeof(SellerMarketDbContext), SellerDbConnectionString.MarketDbConnection)]
    [Relation(typeof(SellerOpenMarket), nameof(SellerOpenMarket))]
    public class SellerOpenMarket : Center
    {
        public double MarginRate {get; set;}
        public List<SellerMMCommodity> sellerMMCommodities { get; set; }
    }
    [DataContext(typeof(SellerMarketDataContext))]
    [DbContext(typeof(SellerMarketDbContext), SellerDbConnectionString.MarketDbConnection)]
    [Relation(typeof(SellerEMCommodity), nameof(SellerEMCommodity))]
    public class SellerEMCommodity : EMCommodity
    {
        public string? SellerMCommodityId { get; set; }
        public string? SellerMarketId { get; set; }
        public string? SellerMMCommodityId { get; set; }
        public SellerMMCommodity? sellerMMCommodity { get; set; }
        public SellerMarket? SellerMarket { get; set; }
        public SellerMCommodity? SellerMCommodity { get; set; }
    }
}