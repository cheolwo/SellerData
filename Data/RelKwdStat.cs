namespace SellerCommon.SellerData.Data
{
    public class RelKwdStat
    {

    }
    public class ShoppingInfo
    {
        
    }
    public class KwdInfo
    {

    }
    public class MCommodityCollectInfo
    {
        public string Name;
        public string KeyWords;
        public string OrderEndDate;
        public string DeliveryFee;
        public string DeliveryInfo;
        public string Code;
        public string CategoryNumber;
        public string Status;
        public string RepresentativeImageUrl;
        public List<string> DetailImages = new();
    }
    public class CommodityPageXPathInfo
    {
        public string Name;
        public string KeyWords;
        public string OrderEndDate;
        public string DeliveryInfo;
        public string Code;
        public string CategoryNumber;
        public string Status;
        public string RepresentativeImageUrl;
        public string DetailImages;
    }
}