using BusinessData.ofDataAccessLayer.ofModelExtenstions;
using Microsoft.Extensions.Logging;
using Microsoft.Office.Interop.Excel;
using SellerData.ofDataAccessLayer.ofDataContext;
using SellerData.ofDataAccessLayer.ofModel;
using SellerData.ofDataAccessLayer.ofRepository.ofMarket;
using SellerData.ViewModel.Common;

namespace SellerData.ViewModel.ofOpenMarketing
{
    public class OpenMarketCommonCategoryListViewModel : OpenMarketObservableObject
    {
        private readonly IOpenMarketCommonCategoryRepostiory _OpenMarketCommonCategoryRepostiory;
        private readonly ILogger<OpenMarketCommonCategoryListViewModel> _logger;
        private Application excelApp;
        public OpenMarketCommonCategoryListViewModel(OpenMarketDataContext openMarketDataContext, IOpenMarketCommonCategoryRepostiory openMarketCommonCategoryRepostiory,
            ILogger<OpenMarketCommonCategoryListViewModel> logger)
            : base(openMarketDataContext)
        {
            _OpenMarketCommonCategoryRepostiory = openMarketCommonCategoryRepostiory;
            _logger = logger;
        }
        private OpenMarketCommonCategory OpenMarketCommonCategory = new();
        public List<OpenMarketCommonCategory> openMarketCommonCategories = new();
        public override async Task InitLoadAsync()
        {
            openMarketCommonCategories = await _OpenMarketCommonCategoryRepostiory.GetToListAsync();
        }
        public async Task StoringExcelCommonCategoryList(string Path)
        {
            excelApp = new();
            excelApp.Visible = true;
            Workbook workbook = excelApp.Workbooks.Open(Path);
            Worksheet workSheet = excelApp.ActiveSheet;
            Console.WriteLine(workSheet.Cells[1, 1].Value);
            for(int i = 2; i <= 13103; i++)
            {
                OpenMarketCommonCategory.Code = workSheet.Cells[i, 1].Value?.ToString() ?? "";
                OpenMarketCommonCategory.Category1 = workSheet.Cells[i, 2].Value?.ToString() ?? "";
                OpenMarketCommonCategory.Category2 = workSheet.Cells[i, 3].Value?.ToString() ?? "";
                OpenMarketCommonCategory.Category3 = workSheet.Cells[i, 4].Value?.ToString() ?? "";
                OpenMarketCommonCategory.Category4 = workSheet.Cells[i, 5].Value?.ToString() ?? "";
                _logger.Log(LogLevel.Information, $"index : {i} Code : {OpenMarketCommonCategory.Code}");
                await OpenMarketCommonCategory.PostAsync(_openMarketDataContext);
                OpenMarketCommonCategory.Id = null;
            }
        }
        public async Task StoringExcelSmartStoreCategoryList(string Path)
        {
            //2 ~4818
            excelApp = new();
            excelApp.Visible = true;
            Workbook workbook = excelApp.Workbooks.Open(Path);
            Worksheet workSheet = excelApp.ActiveSheet;
            for (int i = 2; i <= 4818; i++)
            {
                OpenMarketCommonCategory.Name = "SmartStore";
                OpenMarketCommonCategory.Code = workSheet.Cells[i, 1].Value?.ToString() ?? "";
                OpenMarketCommonCategory.Category1 = workSheet.Cells[i, 2].Value?.ToString() ?? "";
                OpenMarketCommonCategory.Category2 = workSheet.Cells[i, 3].Value?.ToString() ?? "";
                OpenMarketCommonCategory.Category3 = workSheet.Cells[i, 4].Value?.ToString() ?? "";
                OpenMarketCommonCategory.Category4 = workSheet.Cells[i, 5].Value?.ToString() ?? "";
                _logger.Log(LogLevel.Information, $"index : {i} Code : {OpenMarketCommonCategory.Code}");
                await OpenMarketCommonCategory.PostAsync(_openMarketDataContext);
                OpenMarketCommonCategory.Id = null;
            }
        }
        
    }
}
