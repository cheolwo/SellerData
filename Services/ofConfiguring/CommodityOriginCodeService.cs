using BusinessData.ofDataAccessLayer.ofModelExtenstions;
using Microsoft.Office.Interop.Excel;
using SellerCommon.SellerData.Model;
using SellerData.ofDataAccessLayer.ofDataContext;
using SellerData.ofDataAccessLayer.ofModel;

namespace SellerData.Services.ofConfiguring
{
    public interface ICommodityOriginCodeService
    {
        Task SetCodeByExcelData(string path);
        Task SetCode(SellerSMCommodity sellerSMCommodity);
    }
    public class CommodityOriginCodeService : ICommodityOriginCodeService
    {
        private readonly OpenMarketDataContext _openMarketDataContext;
        private readonly SellerMarketDataContext _sellerMarketDataContext;
        private Application excelApp;
        public CommodityOriginCodeService(OpenMarketDataContext openMarketDataContext, SellerMarketDataContext sellerMarketDataContext)
        {
            _openMarketDataContext = openMarketDataContext;
            _sellerMarketDataContext = sellerMarketDataContext;
        }
        private CommodityOriginCode commodityOriginCode = new();
        public async Task SetCodeByExcelData(string path)
        {
            excelApp = new();
            excelApp.Visible = true;
            Workbook workbook = excelApp.Workbooks.Open(path);
            Worksheet workSheet = excelApp.ActiveSheet;
            for (int i = 2; i <= 516; i++)
            {
                commodityOriginCode.Code = workSheet.Cells[i, 1].Value?.ToString() ?? "";
                commodityOriginCode.Name = workSheet.Cells[i, 2].Value?.ToString() ?? "";
                Console.WriteLine(commodityOriginCode.Name);
                await commodityOriginCode.PostAsync(_openMarketDataContext);
                commodityOriginCode.Id = null;
            }
        }
       public async Task SetCode(SellerSMCommodity sellerSMCommodity)
        {
            if (sellerSMCommodity.SellerMCommodity == null) { sellerSMCommodity.SellerMCommodity = await _sellerMarketDataContext.GetByIdAsync<SellerMCommodity>(sellerSMCommodity.SellerMCommodityId); }
            var DicDetailInfo = sellerSMCommodity.SellerMCommodity.DetailCommodityInfo;
            CommodityOriginCode OriginCode = new();
            var OriginCodes = await _openMarketDataContext.GetsAsync<CommodityOriginCode>();
            if(DicDetailInfo == null) 
            {
                OriginCode = OriginCodes.FirstOrDefault(e => e.Name.Equals("중국"));
                sellerSMCommodity.OriginCode = OriginCode.Code;
                await sellerSMCommodity.PutAsync(_sellerMarketDataContext);
                return;
            }
            var key = DicDetailInfo.Keys.FirstOrDefault(e => e.Contains("제조국"));
            Console.WriteLine(key);
            string originValue = "";
            if(key == null ) { originValue = "국산"; }
            else { originValue = DicDetailInfo[key]; }
            Console.WriteLine(originValue);
            var IsDomestic = false;
            if (originValue.Contains("대한민국") || originValue.Contains("국산") || originValue.Contains("국내산") ||
                originValue.Contains("한국") || originValue.Contains("국내") || originValue.Contains("Korea") || originValue.Contains("kor"))
            {
                OriginCode = OriginCodes.FirstOrDefault(e => e.Name.Equals("국산"));
            }
            if (originValue.Contains("중국") || originValue.Contains("china") || originValue.Contains("차이나"))
            {
                OriginCode = OriginCodes.FirstOrDefault(e => e.Name.Equals("중국"));
            }
            if (originValue.Contains("의무"))
            {
                OriginCode = OriginCodes.FirstOrDefault(e => e.Name.Contains("의무"));
            }
            if (originValue.Contains("일본"))
            {
                OriginCode = OriginCodes.FirstOrDefault(e => e.Name.Contains("일본"));
            }
            if (originValue.Contains("미국"))
            {
                OriginCode = OriginCodes.FirstOrDefault(e => e.Name.Contains("미국"));
            }
            if (originValue.Contains("베트남"))
            {
                OriginCode = OriginCodes.FirstOrDefault(e => e.Name.Contains("베트남"));
            }
            if (originValue.Contains("상세"))
            {
                OriginCode = OriginCodes.FirstOrDefault(e => e.Name.Contains("상세"));
            }
            sellerSMCommodity.OriginCode = OriginCode.Code;
            await sellerSMCommodity.PutAsync(_sellerMarketDataContext);
        }
    }
}
