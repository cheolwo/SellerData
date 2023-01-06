using BusinessData.ofDataAccessLayer.ofModelExtenstions;
using Microsoft.Extensions.Configuration;
using SellerCommon.SellerData.Model;
using SellerCommon.SellerData.ofRepository.ofMarket;
using SellerCommon.SellerData.Services;
using SellerData.ofDataAccessLayer.ofDataContext;
using SellerData.ofDataAccessLayer.ofModel;
using SellerData.ofDataAccessLayer.ofRepository.ofMarket;
using SellerData.Services.ofAnalyzing;
using SellerData.Services.ofCollecitng;
using SellerData.ViewModel.Common;

namespace SellerData.ViewModel.ofAnalyzing
{
    public class AnalyzingSMCommodityViewModel : SellerObservableObject
    {
        private readonly IFurtherAnalyzingImportantKwdService _furtherAnalyzingImportantKwdService;
        private readonly ISellerSMCommodityRepository _sellerSMCommodityRepository;
        private readonly ISellerMCommodityRepository _sellerMCommodityRepository;
        private readonly ICommodityNamingService _commodityNamingService;
        private readonly ICommodityAnalyzedInfoRepository _commodityAnalyzedInfoRepository;
        private readonly ISettingSmartStoreCategoryService _settingSmartStoreCategoryService;
        private readonly ICommodityExcelNamingServcie _commodityExcelNamingServcie;
        private readonly IOpenMarketCommonCategoryRepostiory _openMarketCommonCategoryRepostiory;
        private readonly ICommodityDeliveryInfoService _commodityDeliveryInfoService;
        private readonly IConfiguration _configuration;
        private readonly INaverSearchShoppingAPIService _naverSearchShoppingAPIService;
        private readonly IImportantKwdSettingService _importantKwdSettingService;
        private readonly string _blobConnectionString;
        public AnalyzingSMCommodityViewModel(SellerMarketDataContext sellerMarketDataContext,
            ISellerSMCommodityRepository sellerSMCommodityRepository,
            ISellerMCommodityRepository sellerMCommodityRepository,
            IFurtherAnalyzingImportantKwdService furtherAnalyzingImportantKwdService,
            ICommodityAnalyzedInfoRepository commodityAnalyzedInfoRepository,
            ISettingSmartStoreCategoryService settingSmartStoreCategoryService,
            ICommodityNamingService commodityNamingService,
            IImportantKwdSettingService importantKwdSettingService,
            ICommodityExcelNamingServcie commodityExcelNamingServcie,
            IOpenMarketCommonCategoryRepostiory openMarketCommonCategoryRepostiory,
            INaverSearchShoppingAPIService naverSearchShoppingAPIService,
            ICommodityDeliveryInfoService commoditydeliveryInfoService,
            IConfiguration configuration) : base(sellerMarketDataContext)
        {
            _sellerSMCommodityRepository = sellerSMCommodityRepository;
            _furtherAnalyzingImportantKwdService = furtherAnalyzingImportantKwdService;
            _commodityAnalyzedInfoRepository = commodityAnalyzedInfoRepository;
            _settingSmartStoreCategoryService = settingSmartStoreCategoryService;
            _commodityNamingService = commodityNamingService;
            _commodityExcelNamingServcie = commodityExcelNamingServcie;
            _openMarketCommonCategoryRepostiory = openMarketCommonCategoryRepostiory;
            _commodityDeliveryInfoService = commoditydeliveryInfoService;
            _sellerMCommodityRepository = sellerMCommodityRepository;
            _importantKwdSettingService = importantKwdSettingService;
            _configuration = configuration;
            _naverSearchShoppingAPIService = naverSearchShoppingAPIService;
            _blobConnectionString = _configuration.GetSection("MarketCommodityStorage")["ConnectionString"];
        }
        public List<SellerSMCommodity> SellerSMCommodities = new();
        public List<SellerSMCommodity> ViewSellerSMCommodities = new();
        public List<SellerSMCommodity> SellerSMCommoditiesWithMCommodity = new();
        public SellerSMCommodity SellerSMCommodity = new();
        public Dictionary<string, string> DicImportantKwd = new();
        public List<string> Images = new();
        public bool IsImageView = false;
        public OpenMarketCommonCategory category = new();
        private string _CategoryCodeForUpdate = "";
        private string _CurrentSellerMCommodityId = "";
        public string CurrentSellerMCommodityId
        {
            get => _CurrentSellerMCommodityId;
            set => SetProperty(ref _CurrentSellerMCommodityId, value);
        }
        public string CategoryCodeForUpdate
        {
            get => _CategoryCodeForUpdate;
            set => SetProperty(ref _CategoryCodeForUpdate, value);
        }
        public override async Task InitLoadAsync()
        {
            if (!IsLoad)
            {
                SellerSMCommodities = await _SellerMarketDataContext.GetsAsync<SellerSMCommodity>();
                SellerSMCommodities = SellerSMCommodities.Where(e => e.Name == null).ToList();
                ViewSellerSMCommodities = await _sellerSMCommodityRepository.GetToListWithSellerMCommodityFilterByImportantKwds();
                AggregateImportantKwds(ViewSellerSMCommodities);
            }
        }
        private void AggregateImportantKwds(List<SellerSMCommodity> sellerSMCommodities)
        {
            foreach(var value in sellerSMCommodities)
            {
                if(value.SellerMCommodity.ImportantKwds != null && value.SellerMCommodity.ImportantKwds.Count > 0)
                {
                    var stringvalue = value.SellerMCommodity.ImportantKwds.Aggregate((current, data) => current + " " + data);
                    DicImportantKwd.Add(value.Id, stringvalue);
                }
            }
        }
        public async Task SetNameofSMCommodityByNaverShopping()
        {
            var NamingViewSellerSMCommodities = ViewSellerSMCommodities.Where(e => e.Name == null).ToList();
            await _naverSearchShoppingAPIService.SetttingNameByImportantKwds(NamingViewSellerSMCommodities);
        }
        public async Task SetImportantKwdType()
        {
            var values = ViewSellerSMCommodities.Where(e => e.ImportantKwdType == null).ToList();
            foreach(var value in values)
            {
                await _importantKwdSettingService.SetImportantKwdType(value);
            }
        }
        public async Task RemoveDistintSMCommodity()
        {
            await _sellerMCommodityRepository.RemoveDistintSMCommodity();
        }
        public async Task SetNameByTable(string Id)
        {
            var UpdateValue = ViewSellerSMCommodities.Find(e => e.Id.Equals(Id));
            UpdateValue.IsNaming = true;
            await UpdateValue.PutAsync(_SellerMarketDataContext);
            ResetImageView();
            ResetCategory();
        }
        public async Task SetImageByTable(string Id)
        {
            var UpdateValue = ViewSellerSMCommodities.Find(e => e.Id.Equals(Id));
            UpdateValue.IsConfiguringofImage = true;
            await UpdateValue.PutAsync(_SellerMarketDataContext);
            ResetImageView();
            ResetCategory();
        }
        public async Task SelectedImageView(string Id)
        {
            Images = new();
            var FindValue = ViewSellerSMCommodities.Find(e => e.Id.Equals(Id));
            Images = await FindValue.SellerMCommodity.GetBlobItemsAsync(_SellerMarketDataContext, _blobConnectionString);
            IsImageView = true;
            foreach(var value in Images)
            {
                Console.WriteLine(value);
            }
        }
        public async Task SelectedCategory(string Id)
        {
            var FindValue = ViewSellerSMCommodities.Find(e => e.Id.Equals(Id));
            category = await _openMarketCommonCategoryRepostiory.GetByCodeAsync(FindValue.Code);
            CurrentSellerMCommodityId = Id;
        }
        public async Task UpdateCategory()
        {
            var FindValue = ViewSellerSMCommodities.Find(e => e.Id.Equals(CurrentSellerMCommodityId));
            var Category = await _openMarketCommonCategoryRepostiory.GetByCodeAsync(CategoryCodeForUpdate);
            if(Category != null)
            {
                FindValue.Code = Category.Code;
            }
            await FindValue.PutAsync(_SellerMarketDataContext);
            CategoryCodeForUpdate = "";
        }
        private void ResetCategory()
        {
            category = new();
        }
        public void ResetImageView()
        {
            Images = new();
            IsImageView = false;
        }
        private async Task GetSellerMCommodity(List<SellerSMCommodity> sellerSMCommodities)
        {
            foreach(var value in sellerSMCommodities)
            {
                if(value.SellerMCommodity == null)
                {
                    value.SellerMCommodity = await _SellerMarketDataContext.GetByIdAsync<SellerMCommodity>(value.SellerMCommodityId);
                }
            }
        }
        public async Task SettingCommodityNameUsingExcelFile(string Path)
        {
            SellerSMCommodities = SellerSMCommodities.Where(e => e.ImportantKwdType != null && e.SellerMCommodityId != null && e.Code != null).ToList();
            await _commodityExcelNamingServcie.SetNamingUsingExcelFile(SellerSMCommodities, Path);
        }
        /// <summary>
        /// 검색광고 및 쇼핑 API를 이용한 중요 키워드 추가구성 프로세스
        /// </summary>
        /// <returns></returns>
        public async Task FurtherAnalyzingImportantKwd()
        {
            foreach (var sellerSMCommodity in ViewSellerSMCommodities)
            {            
                var CommodityAnalyzeInfos = await _commodityAnalyzedInfoRepository.GetToListByMCommodityIdAsync(sellerSMCommodity.SellerMCommodity.Id);
                if(sellerSMCommodity.SellerMCommodity.ImportantKwds == null || sellerSMCommodity.SellerMCommodity.ImportantKwds.Count == 0)
                {
                    throw new ArgumentNullException("데이터를 준비하세요");
                }
                await _furtherAnalyzingImportantKwdService.FurtherAnalyzingImportantKwd(sellerSMCommodity);
            }
        }
        public async Task SettingCategory()
        {
            var NoCodeCommodities = SellerSMCommodities.Where(e => e.Code == null).ToList();
            foreach (var sellerSMCommodity in NoCodeCommodities)
            {
                await _settingSmartStoreCategoryService.SetCategory(sellerSMCommodity);
            }
        }
        public async Task SettingName()
        {
            var NoNameCommodities = SellerSMCommodities.Where(e => e.Name == null).ToList();
            foreach (var commodity in NoNameCommodities)
            {
                await _commodityNamingService.SetNameofCommodityByRelKwdService(commodity, 1000);
            }
        }
        public async Task SettingDevlieryCode()
        {
            var SellerSMCommodities = await _SellerMarketDataContext.GetsAsync<SellerSMCommodity>();
            var NoDeliveryCodeCommodities = SellerSMCommodities.Where(e => e.DeliveryCode == null).ToList();
            foreach (var commodity in NoDeliveryCodeCommodities)
            {
                await _commodityDeliveryInfoService.SetDeliveryCodeByDeliveryInfo(commodity);
            }
        }
        public async Task DeletingDeliveryCode()
        {
            var SellerSMCommodities = await _SellerMarketDataContext.GetsAsync<SellerSMCommodity>();
            var DeliveryCodeCommodities = SellerSMCommodities.Where(e => e.DeliveryCode != null).ToList();
            foreach(var value in DeliveryCodeCommodities)
            {
                value.DeliveryCode = null;
                await value.PutAsync(_SellerMarketDataContext);
            }
        }
    }
}
