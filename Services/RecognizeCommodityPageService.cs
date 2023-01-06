using SellerCommon.SellerData.Data;

namespace SellerCommon.SellerData.Services
{
    public class RecognizeCommodityPageService : IRecognizeCommodityPageService
    {
        private readonly OnChannalXPathInfoService _OnChannalXPathInfoService;
        public RecognizeCommodityPageService(OnChannalXPathInfoService onChannalXPathInfoService)
        {
            _OnChannalXPathInfoService = onChannalXPathInfoService;
        }
        // 개인적으로 RecognizePage 면은 PageCode를 리턴해줘야되.
        public string RecognizePageByUrl(string url)
        {
            var platformCode = RecognizePlatformByUrl(url);
            if(platformCode.Equals(PlatformCode.OnChannal))
            {
                return RecognizeOnChannlPageByUrl(url);    
            }
            throw new ArgumentException(nameof(RecognizePageByUrl) + "Not Support Url");
        }
        private string RecognizeOnChannlPageByUrl(string url)
        {
            if (url.Contains("onch_view"))
            {
                return OnChannalPageCode.ExcellentCommodityPage.ToString();
            }
            if (url.Contains("dbcenter_view"))
            {
                return OnChannalPageCode.DataCenterCommodityPage.ToString();
            }
            //https://www.onch3.co.kr/onch_hall_of_fame.html?sec=best
            if (url.Contains("onch_hall_of_fame"))
            {
                return OnChannalPageCode.AccessPageToExcellentCommodity.ToString();
            }
            //https://www.onch3.co.kr/dbcenter_renewal/excel_download_center.html
            if (url.Contains("dbcenter_renewal"))
            {
                return OnChannalPageCode.AccessPageToDataCenterCommodity.ToString();
            }
            throw new ArgumentNullException(nameof(RecognizeOnChannlPageByUrl) + "Not Support Page");
        }
        public PlatformCode RecognizePlatformByUrl(string url)
        {
            if(url.Contains("onch3"))
            {
                return PlatformCode.OnChannal;
            }
            throw new ArgumentNullException("Not Suppot Platform Url");
        }
        public CommodityPageXPathInfo GetXPathInfoByUrl(string url)
        {
            PlatformCode platformCode = RecognizePlatformByUrl(url);
            if(platformCode.Equals(PlatformCode.OnChannal))
            {
                return _OnChannalXPathInfoService.GetCommodityPageXPathInfo(url);
            }
            throw new ArgumentException("Not Support Url");
        }

        public string RecognizePlatformPage(PlatformCode code, string url)
        {
            switch(code)
            {
                case PlatformCode.OnChannal:
                    return RecognizeOnChannlPageByUrl(url);
                default:
                    return "";
            }
        }
    }
}