using BusinessData.ofDataAccessLayer.ofModelExtenstions;
using Microsoft.Extensions.Logging;
using Microsoft.Office.Interop.Excel;
using SellerCommon.SellerData.Model;
using SellerCommon.SellerData.ofRepository.ofMarket;
using SellerData.ofDataAccessLayer.ofDataContext;

namespace SellerData.Services.ofOrdering
{
    public static class OnchOrderingExcelMetaInfo
    {
        public const int CommodityCode = 5;
    }
    public interface IOnchOrderingInfoService
    {
        Task RegisterOrderingInfo(string filePath);
    }
    public class OnchOrderingInfoService : IOnchOrderingInfoService
    {
        private Application excelApp;
        private readonly ILogger<OnchOrderingInfoService> _logger;
        private readonly SellerMarketDataContext _sellerMarketDataContext;
        private readonly ISellerEMCommodityRepository _sellerEMCommodityRepository;
        private readonly ISellerMCommodityRepository _sellerMCommodityRepository;
        private readonly ISellerMMCommodityRepository _sellerMMCommodityRepository;
        public OnchOrderingInfoService(ILogger<OnchOrderingInfoService> logger, SellerMarketDataContext sellerMarketDataContext,
            ISellerEMCommodityRepository sellerEMCommodityRepository,
            ISellerMCommodityRepository sellerMCommodityRepository, ISellerMMCommodityRepository sellerMMCommodityRepository)
        {
            _logger = logger;
            _sellerMarketDataContext = sellerMarketDataContext;
            _sellerEMCommodityRepository = sellerEMCommodityRepository;
            _sellerMCommodityRepository = sellerMCommodityRepository;
            _sellerMMCommodityRepository = sellerMMCommodityRepository;
        }

        public async Task RegisterOrderingInfo(string filePath)
        {
            excelApp = new();
            excelApp.Visible = true;
            Workbook workbook = excelApp.Workbooks.Open(filePath);
            Worksheet worksheet = excelApp.ActiveSheet;
            for(int i = 1; i < worksheet.Rows.Count; i++)
            {
                if (worksheet.Cells[i, OnchOrderingExcelMetaInfo.CommodityCode] != null)
                {
                    var CommodityCode = worksheet.Cells[i, OnchOrderingExcelMetaInfo.CommodityCode];
                    _logger.LogInformation("기록합니다.");

                    List<SellerMMCommodity> SellerMMCommodites = await _sellerMMCommodityRepository.GetToListWithMCommodityFilterByCommodityCode(CommodityCode);
                    var SellerMMCommodity = SellerMMCommodites.FirstOrDefault();
                    if(SellerMMCommodity != null)
                    {
                        SellerEMCommodity sellerEMCommodity = new();
                        sellerEMCommodity.SellerMCommodityId = SellerMMCommodity.SellerMCommodityId;
                        sellerEMCommodity.SellerMMCommodityId = SellerMMCommodity.Id;
                        sellerEMCommodity.Code = CommodityCode;
                        await sellerEMCommodity.PostAsync(_sellerMarketDataContext);
                    }
                }
            }
        }
    }
}
