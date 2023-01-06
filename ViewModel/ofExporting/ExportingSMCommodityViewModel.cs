using SellerCommon.SellerData.Services.ofExporting;
using SellerData.ViewModel.Common;
using SellerCommon.SellerData.Model;
using SellerData.ofDataAccessLayer.ofDataContext;
using SellerCommon.SellerData.ofRepository.ofMarket;
using Microsoft.Extensions.Configuration;
using BusinessData.ofDataAccessLayer.ofModelExtenstions;
using SellerData.Services.ofExporting;
using Microsoft.Extensions.Logging;

namespace SellerData.ViewModel.ofExporting
{
    public class ExportingSMCommodityViewModel : SellerObservableObject
    {
        private readonly ICommodityExportingService _commodityExportingService;
        private readonly ISellerSMCommodityRepository _sellerSMCommodityRepository;
        private readonly ISellerMMCommodityRepository _sellerMMCommodityRepostiory;
        private readonly ICommodityConfirmingService _confirmingCommodityService;
        private readonly ISellerMCommodityRepository _sellerMCommoditryRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ExportingSMCommodityViewModel> _logger;
        public ExportingSMCommodityViewModel(SellerMarketDataContext sellerMarketDataContext,
                                            ISellerSMCommodityRepository sellerSMCommodityRepository,
                                            ICommodityExportingService commodityExportingService,
                                            ISellerMMCommodityRepository sellerMMCommodityRepository,
                                            ICommodityConfirmingService confirmingCommodityService,
                                            ISellerMCommodityRepository sellerMCommodityRepository,
                                            IConfiguration configuration, 
                                            ILogger<ExportingSMCommodityViewModel> logger)
                            : base(sellerMarketDataContext)
        {
            _commodityExportingService = commodityExportingService;
            _sellerMMCommodityRepostiory = sellerMMCommodityRepository;
            _sellerSMCommodityRepository = sellerSMCommodityRepository;
            _confirmingCommodityService = confirmingCommodityService;
            _sellerMCommoditryRepository = sellerMCommodityRepository;
            _configuration = configuration;
            _logger = logger;
            _savePath = _configuration.GetSection("Scrapping")["CommodityListPath"];
            _commodityListExcelPath = _configuration.GetSection("Market")["CommodityListExelFile"];
            
        }
        public List<SellerSMCommodity> ExportingSellerSMCommodities = new();
        public List<SellerSMCommodity> ViewSellerSMCommodities = new();
        public List<SellerSMCommodity> SimpleSellerSMCommodities = new();
        public List<SellerOpenMarket> sellerOpenMarkets = new();
        public SellerMCommodity SelectedMCommodity = new();
        private SellerOpenMarket ExportingOpenMarket = new();
        public List<SellerSMCommodity> NotRegisterToSmartStoreSMCommodities = new();
        public List<SellerMCommodity> sellerMCommodities = new();
        public List<SellerMMCommodity> sellerMMCommodities = new();
        private bool _IsSelected;
        public bool IsSelected
        {
            get => _IsSelected;
            set => SetProperty(ref _IsSelected, value);
        }
        private string _savePath = "";
        private string _commodityListExcelPath = "";
        public List<string> Images = new();
        public bool IsImageView = false;

        public override async Task InitLoadAsync()
        {
            ExportingSellerSMCommodities = await _sellerSMCommodityRepository.GetToListByExportingConditionsWithSellerMCommodity();
            ViewSellerSMCommodities = await _sellerSMCommodityRepository.GetToListWithSellerMCommodityAndMarketAsync();
            ViewSellerSMCommodities = ViewSellerSMCommodities.Where(e => e.DeliveryCode != null && e.OriginCode != null && e.Code != null ).ToList();
            _logger.LogInformation(ExportingSellerSMCommodities.Count.ToString());
        }
        public async Task SelectedImageView(string Id)
        {
            Images = new();
            var FindValue = ExportingSellerSMCommodities.Find(e => e.Id.Equals(Id));
            FindValue.SellerMCommodity = await _SellerMarketDataContext.GetByIdAsync<SellerMCommodity>(FindValue.SellerMCommodityId);
            Images = FindValue.SellerMCommodity.DetailImages;
            IsImageView = true;
        }
        public async Task ConfirmingRegisterCommodityToSmartStore()
        {
            await _confirmingCommodityService.ConfirmingRegisterToSmartStore(_commodityListExcelPath);
        }
        
        public async Task FindById(string id)
        {
            var value = ExportingSellerSMCommodities.FirstOrDefault(e => e.Id.Equals(id));
            if(value != null)
            {
                if(value.SellerMCommodity == null)
                {
                    value.SellerMCommodity = await _SellerMarketDataContext.GetByIdAsync<SellerMCommodity>(value.SellerMCommodityId);
                    SelectedMCommodity = value.SellerMCommodity;
                    IsSelected = true;
                }
                else
                {
                    SelectedMCommodity = value.SellerMCommodity;
                    IsSelected = true;
                }
            }
        }
        public async Task ReImaging(string id)
        {
            var value = NotRegisterToSmartStoreSMCommodities.FirstOrDefault(e => e.Id.Equals(id));
            if(value != null)
            {
                value.IsReimaging = true;
                await value.PutAsync(_SellerMarketDataContext);
            }
        }
        public async Task IsSmartStoreExporting(string id)
        {
            var value = NotRegisterToSmartStoreSMCommodities.FirstOrDefault(e => e.Id.Equals(id));
            if (value != null)
            {
                value.IsSmartStoreExporting = true;
                await value.PutAsync(_SellerMarketDataContext);
            }
        }
        public async Task IsNotSmartStoreExporting(string id)
        {
            var value = NotRegisterToSmartStoreSMCommodities.FirstOrDefault(e => e.Id.Equals(id));
            if (value != null)
            {
                value.IsSmartStoreExporting = false;
                await value.PutAsync(_SellerMarketDataContext);
            }
        }
        public void SelectOpenMarket(string id)
        {
            ExportingOpenMarket = sellerOpenMarkets.FirstOrDefault(e => e.Id.Equals(id));
        }
        public async Task ExportingToExel(string filePath)
        {         
            foreach (var value in NotRegisterToSmartStoreSMCommodities)
            {
                await _commodityExportingService.ExportingToExcel(value, 0.13, ExportingOpenMarket.Name, filePath, _savePath);
            }
        }
        public async Task SimpleExporting(string filePath)
        {
            await _commodityExportingService.ToExcelCommodityList(ViewSellerSMCommodities, 0.13, "SmartStore", filePath, _savePath);
        }
        public async Task ToExcelCommodityList(string filePath)
        {
            await _commodityExportingService.ToExcelCommodityList(ViewSellerSMCommodities, 0.13, "SmartStore", filePath, _savePath);
        }
        public async Task ExportingToExelForSellerMCommodity(string filePath)
        {
            var ExportingSmartStoreCommodities = ExportingSellerSMCommodities.Where(e => e.SellerMCommodity.SellerMMCommodities.Count == 0).ToList();
            await _commodityExportingService.ToExcelCommodityList(ExportingSmartStoreCommodities, 0.13, "SmartStore", filePath, _savePath);
        }
        // 가격반영
        // filePath : 쿠팡 가격/재고/상품에 대한 엑셀파일
        public async Task ExportingToCoupangWithPriceUpdating(string filePath)
        {
            
        }
    }
}
