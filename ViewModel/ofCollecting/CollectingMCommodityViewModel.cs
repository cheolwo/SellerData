using SellerCommon.SellerData.Model;
using SellerCommon.SellerData.Services;
using SellerData.ofDataAccessLayer.ofDataContext;
using SellerCommon.SellerData.ViewModel.ofWholeSale;
using SellerCommon.SellerData.ofRepository.ofMarket;
using SellerData.Services;
using SellerData.ViewModel.Common;
using SellerData.Services.ofCollecitng;

namespace SellerData.ViewModel.ofWholeSale
{
    public class CollectingMCommodityViewModel : SellerObservableObject
    {
        public SellerMarket SellerMarket = new();
        public List<SellerMCommodity> SellerMCommodities = new();
        public List<SellerMCommodity> NoDataCommodities = new();
        public List<SellerMCommodity> DuplicatedCommodities = new();
        public List<SellerMCommodity> NoDetailCommodityInfoCommodities = new();
        public List<SellerMCommodity> NoOptionCommodityInfoCommodities = new();
        public List<SellerMCommodity> NoCategorySellerMCommoditites = new();
        public int count = 0;
        public int DuplicatedCount = 0;
        private readonly ISellerMarketRepository _SellerMarketRepository;
        private readonly ISellerMCommodityRepository _SellerMCommodityRepository;
        private readonly IOch3CommodityPageScrappingService _Och3CommodityPageScrappingService;
        private readonly IOnChannalMappingService _Och3ScrappingInfoMappingService;
        private readonly ICollectingCommodityCategoryService _CollectingCommodityCategoryService;
        private readonly ICommodityScarppingService _commodityScarppingService;

        public CollectingMCommodityViewModel(SellerMarketDataContext sellerMarketDataContext,
            ISellerMarketRepository sellerMarketRepository,
            ISellerMCommodityRepository sellerMCommodityRepository,
            IOch3CommodityPageScrappingService och3CommodityPageScrappingService,
            IOnChannalMappingService och3ScrappingInfoMappingService,
            ICollectingCommodityCategoryService collectingCommodityCategoryService,
            ICommodityScarppingService commodityScarppingService) : base(sellerMarketDataContext)
        {
            _SellerMarketRepository = sellerMarketRepository;
            _SellerMCommodityRepository = sellerMCommodityRepository;
            _Och3CommodityPageScrappingService = och3CommodityPageScrappingService;
            _Och3ScrappingInfoMappingService = och3ScrappingInfoMappingService;
            IsLoad = false;
            _CollectingCommodityCategoryService = collectingCommodityCategoryService;
            _commodityScarppingService = commodityScarppingService;
        }
        private string _SearchKwd;
        public string SearchKwd
        {
            get => _SearchKwd;
            set => SetProperty(ref _SearchKwd, value);
        }
        private int _StartPage;
        public int StartPage
        {
            get => _StartPage;
            set => SetProperty(ref _StartPage, value);
        }
        private int _EndPage;
        public int EndPage
        {
            get => _EndPage;
            set => SetProperty(ref _EndPage, value);
        }
        public SellerMCommodity sellerMCommodity = new();
        public override async Task InitLoadAsync()
        {
            if (!IsLoad)
            {
                SellerMarket = await _SellerMarketRepository.GetByNameAsync(OnChannal.OnChannalName);
                SellerMCommodities = await _SellerMCommodityRepository.GetToListByCenterIdAsync(SellerMarket.Id);
                count = SellerMCommodities.Count;
                NoCategorySellerMCommoditites = SellerMCommodities.Where(e => e.Category == null).ToList();
                NoDataCommodities = SellerMCommodities.Where(e => e.Keywords == null).ToList();
                NoDetailCommodityInfoCommodities = SellerMCommodities.Where(e => e.DetailCommodityInfo == null).ToList();
                NoOptionCommodityInfoCommodities = SellerMCommodities.Where(e => e.CommodityOptions == null).ToList();
            }
        }
        //public void CheckDuplicated()
        //{
        //    foreach (var commodity in SellerMCommodities)
        //    {
        //        var CheckList = SellerMCommodities.Where(e => e.CommodityPageUrl.Equals(commodity.CommodityPageUrl)).ToList();
        //        foreach (var node in CheckList)
        //        {
        //            Console.WriteLine(node.CommodityPageUrl);
        //        }
        //    }
        //}
        public async Task Scrapping()
        {
            foreach (var commodity in NoDataCommodities)
            {
                var mCommodityCollectInfo = await _Och3CommodityPageScrappingService.Scrapping(commodity.CommodityPageUrl);
                var MappingValue = _Och3ScrappingInfoMappingService.MCommodityCollectInfoToSellerMCommodity(mCommodityCollectInfo);
                commodity.Code = MappingValue.Code;
                commodity.DeliveryContent = MappingValue.DeliveryContent;
                commodity.Keywords = MappingValue.Keywords;
                commodity.Name = MappingValue.Name;
                commodity.RepresentativeImageUrl = MappingValue.RepresentativeImageUrl;
                commodity.DetailImages = MappingValue.DetailImages;
            }
            await Update();
        }
        public async Task CollectingAccessPath()
        {
            await _commodityScarppingService.CollectingAccessPath(SearchKwd);
        }
        public async Task ScrappingInDataCenter()
        {
            await _commodityScarppingService.Scrapping();
        }
        public async Task ScrappingInDataCenterForNoDetailImageUrl()
        {
            var Commodities = SellerMCommodities.Where(e => e.DetailImages.Count == 0).ToList();

            await _commodityScarppingService.DetailImageUrlScrapping(Commodities);

        }
        public async Task CollectingCategory()
        {
            foreach (var commdity in NoCategorySellerMCommoditites)
            {
                await _CollectingCommodityCategoryService.CollectingCategory(commdity);
            }
        }
        public async Task DeletingAllCategory()
        {
            await _CollectingCommodityCategoryService.DeleteAllCategory(SellerMCommodities);
        }
        public async Task DetailCommodityInfoScrapping()
        {
            foreach (var commodity in NoDetailCommodityInfoCommodities)
            {
                var DetailCommodityInfo = await _Och3CommodityPageScrappingService.DetailCommodityInfoScrapping(commodity.CommodityPageUrl);
                commodity.DetailCommodityInfo = DetailCommodityInfo;
                if (DetailCommodityInfo.Count == 0)
                {
                    continue;
                }
                await _SellerMCommodityRepository.UpdateAsync(commodity);
            }
        }
        public async Task CommodityOptionInfoScrapping()
        {
            await _Och3CommodityPageScrappingService.CommodityOptionScrapping(NoOptionCommodityInfoCommodities);
        }
        public async Task CommodityDeliveryInfoScrapping()
        {
            var NoDeliveryInfoSellerMCommodities = SellerMCommodities.Where(e => e.DeliveryContent == null).ToList();
            await _Och3CommodityPageScrappingService.CommodityDeliveryInfoScrapping(NoDeliveryInfoSellerMCommodities);
        }
        public async Task SelectViewOption(string id)
        {
            var FindValue = SellerMCommodities.FirstOrDefault(e => e.Id.Equals(id));
            if(FindValue != null)
            {
                sellerMCommodity = FindValue;
            }
        }
        public List<string> Images = new();
        public async Task GetImage(string id)
        {
            Images = new();
            var FindValue = SellerMCommodities.FirstOrDefault(e => e.Id.Equals(id));
            Images = FindValue.DetailImages;
        }
        // 수정
        public async Task Update()
        {
            foreach (var commodity in NoDataCommodities)
            {
                await _SellerMCommodityRepository.UpdateAsync(commodity);
            }
            SellerMCommodities.Clear();
            NoDataCommodities.Clear();
            IsLoad = false;
        }
    }
}

