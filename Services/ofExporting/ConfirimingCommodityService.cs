using SellerData.ofDataAccessLayer.ofDataContext;
using SellerCommon.SellerData.Model;
using Microsoft.Office.Interop.Excel;
using BusinessData.ofDataAccessLayer.ofModelExtenstions;
using SellerCommon.SellerData.ofRepository.ofMarket;

namespace SellerData.Services.ofExporting
{
    public interface ICommodityConfirmingService
    {
        Task ConfirmingRegisterToSmartStore(string CommodityPath);
    }
    public class CommodityConfirmingService : ICommodityConfirmingService
    {
        private readonly SellerMarketDataContext _sellerMarketDataContext;
        private readonly ISellerMMCommodityRepository _sellerMMCommodityRepository;
        public CommodityConfirmingService(SellerMarketDataContext sellerMarketDataContext, ISellerMMCommodityRepository sellerMMCommodityRepository)
        {
            _sellerMarketDataContext = sellerMarketDataContext;
            _sellerMMCommodityRepository = sellerMMCommodityRepository;
        }
        private Application excelApp;
        /// <summary>
        ///  1. 엑실 파일을 오픈하는 단계
        ///  2. 엑셀 파일에서 이름 열 데이터를 얻는 단계
        ///  3. 상기 데이터가 SMCommodity 또는 MCommodity 와 일치하는지 확인하는 단계
        ///  4. 일치 데이터의 FK를 가지는 SellerMCommodity가 없는지 확인하는 단계
        ///  4. 없을 경우 SellerMMCommodity를 생성하는 단계를 포함한다.
        /// </summary>
        /// <param name="CommodityPath"></param>
        /// <returns></returns>
        public async Task ConfirmingRegisterToSmartStore(string CommodityPath)
        {
            excelApp = new();
            excelApp.Visible = true;
            Workbook workbook = excelApp.Workbooks.Open(CommodityPath);
            Worksheet worksheet = excelApp.ActiveSheet;
            SellerMarket sellerMarket = new();
            sellerMarket = await _sellerMarketDataContext.GetByNameAsync<SellerMarket>("Onch3");
            SellerOpenMarket sellerOpenMarket = new();
            sellerOpenMarket = await _sellerMarketDataContext.GetByNameAsync<SellerOpenMarket>("SmartStore");
            for(int i = 2; i <= worksheet.UsedRange.Rows.Count; i++)
            {
                var NameofCommodity = worksheet.Cells[i, 5].Value;
                if(NameofCommodity == null || NameofCommodity == "") { break; }
                var SellerMCommodity = await _sellerMarketDataContext.GetByNameAsync<SellerMCommodity>(NameofCommodity);

                if (SellerMCommodity != null)
                {
                    var SellerMMCommodity = await _sellerMMCommodityRepository.GetByMCommodityIdAndMarketId(SellerMCommodity, sellerOpenMarket);
                    if(SellerMMCommodity == null)
                    {
                        SellerMMCommodity newSellerMMCommodity = new();
                        newSellerMMCommodity.SellerMarketId = sellerMarket.Id;
                        newSellerMMCommodity.SellerOpenMarketId = sellerOpenMarket.Id;
                        newSellerMMCommodity.SellerMCommodityId = SellerMCommodity.Id;
                        newSellerMMCommodity.CommodityId = SellerMCommodity.Id;
                        newSellerMMCommodity.CreateTime = DateTime.Now;
                        await newSellerMMCommodity.PostAsync(_sellerMarketDataContext);
                        continue;
                    }
                }
            }
        }
    }
}
