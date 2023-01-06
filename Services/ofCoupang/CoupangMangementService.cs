using Microsoft.Extensions.Logging;
using Microsoft.Office.Interop.Excel;
using SellerCommon.SellerData.Model;
using SellerCommon.SellerData.ofRepository.ofMarket;
using SellerCommon.SellerData.Services.ofConfiguring;
using SellerCommon.SellerData.Services.ofExporting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellerData.Services.ofCoupang
{
    public interface ICoupangExcelExportingService
    {
        Task ExportingToCoupang(string filePath);
    }
    public static class CoupangCommodityExcelFileInfo
    {
        public const int Code = 6;
        public const int NameofCommodity = 8;
        public const int CommodityOption = 9;
        public const int SellingPrice = 16;
        public const int DiscountSellingPrice = 17;
        public const int StateofSelling = 18;
        public const int Quantity = 19;
    }
    public class CoupangMangementService : ICoupangExcelExportingService
    {
        private Application ExcelApp;
        private readonly ISellerSMCommodityRepository _sellerSMCommodityRepository;
        private readonly ISellerMMCommodityRepository _sellerMMCommodityRepository;
        private readonly ICommodityOptionService _commodityOptionService;
        private readonly ILogger<CoupangMangementService> _logger;
        
        public CoupangMangementService(ISellerSMCommodityRepository sellerSMCommodityRepository,
                                                                ISellerMMCommodityRepository sellerMMCommodityRepository,
                                                                 ICommodityOptionService commodityOptionService,
                                                                 ILogger<CoupangMangementService> logger)
        {
            _sellerSMCommodityRepository = sellerSMCommodityRepository;
            _sellerMMCommodityRepository = sellerMMCommodityRepository;
            _commodityOptionService = commodityOptionService;
            _logger = logger;
        }
        public async Task ExportingToCoupang(string filePath)
        {
            List<SellerMMCommodity> sellerMMCommodities = await _sellerMMCommodityRepository.GetToListWithRelatedDatas();
            ExcelApp = new();
            Workbook workbook = ExcelApp.Workbooks.Open(filePath);
            Worksheet worksheet = ExcelApp.ActiveSheet;
            Dictionary<string, SellerMMCommodity> DicSellerMMCommodity = ConvertToDic(sellerMMCommodities);
            //
            for(int StartRow = 4; StartRow <= worksheet.UsedRange.Rows.Count; StartRow++)
            {
                try
                {
                    var SellerSMCommodityId = (string)worksheet.Cells[StartRow, CoupangCommodityExcelFileInfo.Code].Value;
                    var SellerOptionofSMCommodity = worksheet.Cells[StartRow, CoupangCommodityExcelFileInfo.CommodityOption].Value;
                    if (DicSellerMMCommodity[SellerSMCommodityId] != null)
                    {
                        var sellerMCommodity = DicSellerMMCommodity[SellerSMCommodityId].SellerMCommodity ?? throw new ArgumentException("SellerMCommodity Is Null");
                        var ExportingOptionInfo = _commodityOptionService.ConfigureOptionInfo(sellerMCommodity, 0.2);
                        
                    }
                }
                catch(Exception e)
                {
                    _logger.LogInformation(e.Message);
                    continue;
                }
            }
        }
        private IEnumerable<string> GetOptions(ExportingOptionInfo exportingOptionInfo)
        {
            var strings = exportingOptionInfo.Name.Split(',');
            List<string> newStrings = new();
            foreach(var value in strings)
            {
                if (value[0] == ' ')
                {
                    var newValue = value.Substring(1, value.Length);
                    newStrings.Add(newValue);
                    continue;
                }
                newStrings.Add(value);
            }
            return newStrings;
        }
        private Dictionary<string, SellerMMCommodity> ConvertToDic(List<SellerMMCommodity> sellerMMCommodities)
        {
            Dictionary<string, SellerMMCommodity> keyValuePairs = new();
            foreach(var sellerMMCommodity in sellerMMCommodities)
            {
                if(sellerMMCommodity.SellerSMCommodity != null && sellerMMCommodity.SellerMCommodity != null)
                {
                    var Key = sellerMMCommodity.SellerSMCommodity.GetBasicCode();
                    keyValuePairs.Add(Key, sellerMMCommodity);
                }
            }
            return keyValuePairs;
        }
        
    }
}
