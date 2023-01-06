using BusinessData.ofDataAccessLayer.ofModelExtenstions;
using Microsoft.Extensions.Configuration;
using Microsoft.Office.Interop.Excel;
using SellerCommon.SellerData.Model;
using SellerCommon.SellerData.ofRepository.ofMarket;
using SellerData.ofDataAccessLayer.ofDataContext;
using SellerData.ofDataAccessLayer.ofModel;
using SellerData.ofDataAccessLayer.ofRepository.ofMarket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellerData.Services.ofAnalyzing
{
    public interface ICommodityExcelNamingServcie
    {
        Task SetNamingUsingExcelFile(List<SellerSMCommodity> sellerSMCommodities, string ExcelFilePath);
        Task GetNamingUsingExcelFile(string Path);
    }
    public class CommodityExcelNamingService : ICommodityExcelNamingServcie
    {
        private readonly IOpenMarketCommonCategoryRepostiory _openMarketCommonCategoryRepostiory;
        private readonly ISellerMCommodityRepository _sellerMCommodityRepository;
        private readonly IConfiguration _configuration;
        private readonly SellerMarketDataContext _sellerMarketDataContext;
        public CommodityExcelNamingService(IOpenMarketCommonCategoryRepostiory openMarketCommonCategoryRepostiory,
                                                                    ISellerMCommodityRepository sellerMCommodityRepository,
                                                                    IConfiguration configuration,
                                                                    SellerMarketDataContext sellerMarketDataContext)
        {
            _openMarketCommonCategoryRepostiory = openMarketCommonCategoryRepostiory;
            _sellerMCommodityRepository = sellerMCommodityRepository;
            _sellerMarketDataContext = sellerMarketDataContext;
            _configuration = configuration;
            _BlobConnectionString = _configuration.GetSection("MarketCommodityStorage")["ConnectionString"];
        }
        private string _BlobConnectionString;
        private Application excelApp;

        public async Task SetNamingUsingExcelFile(List<SellerSMCommodity> sellerSMCommodities, string ExcelFilePath)
        {
            excelApp = new Application();

            Workbook wb = excelApp.Workbooks.Open(ExcelFilePath);
            Worksheet worksheet = wb.Worksheets["Sheet1"];
            excelApp.Visible = true;
            int i = 2;
            try
            {
                foreach (var value in sellerSMCommodities)
                {
                    worksheet.Cells[i, CommodityNamingExcelFileMetaData.SellerMCommodityId].Value = value.SellerMCommodityId;
                    worksheet.Cells[i, CommodityNamingExcelFileMetaData.SellerSMCommodityId].Value = value.Id;
                    if (value.SellerMCommodity == null)
                    {
                        value.SellerMCommodity = await _sellerMCommodityRepository.GetByIdAsync(value.SellerMCommodityId);
                    }
                    worksheet.Cells[i, CommodityNamingExcelFileMetaData.ImportantKwds].Value = value.SellerMCommodity.ImportantKwds.Aggregate("", (current, next) => current + " " + next);
                    worksheet.Cells[i, CommodityNamingExcelFileMetaData.NamingType].Value = value.ImportantKwdType;
                    var Category = await _openMarketCommonCategoryRepostiory.GetByCodeAsync(value.Code);
                    worksheet.Cells[i, CommodityNamingExcelFileMetaData.Category1].Value = Category.Category1;
                    worksheet.Cells[i, CommodityNamingExcelFileMetaData.Category2].Value = Category.Category2;
                    worksheet.Cells[i, CommodityNamingExcelFileMetaData.Category3].Value = Category.Category3;
                    worksheet.Cells[i, CommodityNamingExcelFileMetaData.Category4].Value = Category.Category4;
                    List<string> imageurl = new();

                    var BlobItems = await value.SellerMCommodity.GetBlobItemsAsync(_sellerMarketDataContext, _BlobConnectionString);
                    worksheet.Cells[i, CommodityNamingExcelFileMetaData.ImageUrl].Value = BlobItems.Aggregate("", (current, next) => current + "\r\n " + next);
                    i++;
                }
            }
            finally
            {
                worksheet.ClearArrows();
                wb.Close();
                excelApp.Quit();
            }
           
        }

        public Task GetNamingUsingExcelFile(string Path)
        {
            throw new NotImplementedException();
        }
        public static class CommodityNamingExcelFileMetaData
        {
            public const int SellerMCommodityId = 1;
            public const int SellerSMCommodityId = 2;
            public const int ImportantKwds = 3;
            public const int RelatedKwds = 4;
            public const int NamingType = 5;
            public const int Category1 = 6;
            public const int Category2 = 7;
            public const int Category3 = 8;
            public const int Category4 = 9;
            public const int ImageUrl = 10;
        }
    }
}
