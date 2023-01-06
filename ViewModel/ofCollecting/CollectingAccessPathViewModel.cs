using BusinessData.ofDataAccessLayer.ofModelExtenstions;
using CommunityToolkit.Mvvm.ComponentModel;
using SellerCommon.SellerData.Model;
using SellerCommon.SellerData.ofRepository.ofMarket;
using SellerCommon.SellerData.Services;
using SellerData.ofDataAccessLayer.ofDataContext;
using SellerData.Services;
using SellerData.ViewModel.Common;

namespace SellerCommon.SellerData.ViewModel.ofWholeSale
{
    public static class OnChannal
    {
        public const string OnChannalName = "Onch3";
    }
    public class CollectingAccessPathViewModel : SellerObservableObject
    {
        private readonly ICollectingOnChannalURLInfo _collectingOnChannalURLInfo;
        private readonly IOch3CommodityPageScrappingService _och3CommodityPageScrappingService; 
        private readonly ISellerMCommodityRepository _SellerMCommodityRepository;
        public CollectingAccessPathViewModel(ICollectingOnChannalURLInfo collectingOnChannalURLInfo,
            ISellerMCommodityRepository sellerMCommodityRepository,
            IOch3CommodityPageScrappingService och3CommodityPageScrappingService,
            SellerMarketDataContext sellerMarketDataContext) :base(sellerMarketDataContext)
        {
            _collectingOnChannalURLInfo = collectingOnChannalURLInfo;
            _SellerMCommodityRepository = sellerMCommodityRepository;
            _och3CommodityPageScrappingService = och3CommodityPageScrappingService;
            Onch3Market.Name = OnChannal.OnChannalName;
            IsStore = false;
            IsBusy = false;
        }
        private string _Url;
        public string Url
        {
            get => _Url;
            set => SetProperty(ref _Url, value);
        }
        private bool _IsStore;
        public bool IsStore
        {
            get => _IsStore;
            set => SetProperty(ref _IsStore, value);
        }
        private string _Year;
        public string Year
        {
            get => _Year;
            set => SetProperty(ref _Year, value);
        }
        private int _StartYear;
        public int StartYear
        {
            get => _StartYear;
            set => SetProperty(ref _StartYear, value);
        }
        private int _EndYear;
        public int EndYear
        {
            get => _EndYear;
            set => SetProperty(ref _EndYear, value);
        }
        public List<string> CollectedPath = new();
        private SellerMCommodity SellerMCommodity = new();
        private SellerMarket Onch3Market = new();

        public async Task CollectAccessPathInfo()
        {
            if (_Url != null)
            {
                await _collectingOnChannalURLInfo.CollectCommmodityURLInfo(CollectedPath, _Url);
                IsStore = true;
            }
            else
            {
                throw new ArgumentNullException(nameof(CollectingAccessPathViewModel) + "Url Is Null");
            }
        }
        public async Task CollectingExcellntCommodityByYear()
        {
            var SellerMCommodities = await _och3CommodityPageScrappingService.ExcellentCommodityPageScrappingByYear(Year);
        }
        public async Task CollectingAllExcellentCommodity()
        {
            for(int i = StartYear; i <= EndYear; i++)
            {
                await _och3CommodityPageScrappingService.ExcellentCommodityPageScrappingByYear(i.ToString());
            }
        }
        public async Task CollectAllExcellentCommodityPathInfo()
        {
            await _collectingOnChannalURLInfo.CollectOnch3ExcellCommodity(CollectedPath);
            IsStore = true;
            IsBusy = true;
        }
        public async Task CollectExcellentCommodityPathInfoByYear()
        {
            IsBusy = true;
            await _collectingOnChannalURLInfo.CollectOnch3ExcellentCommodityPathByYear(CollectedPath, Year);
            IsStore = true;
            IsBusy = false;
        }
        public async Task StoreAccessPathInfo()
        {
            IsBusy = true;
            var marketvalue = await Onch3Market.GetByNameAsync(_SellerMarketDataContext);
            if (marketvalue == null)
            {
                marketvalue = await Onch3Market.PostAsync(_SellerMarketDataContext);
            }
            foreach (var path in CollectedPath)
            {
                var CheckDuplicatedValue = await _SellerMCommodityRepository.GetByCommodityPageUrl(path);
                if(CheckDuplicatedValue == null)
                {
                    SellerMCommodity.CommodityPageUrl = path;
                    SellerMCommodity.CenterId = marketvalue.Id;
                    await SellerMCommodity.PostAsync(_SellerMarketDataContext);
                }
            }
            CollectedPath.Clear();
            IsStore = false;
            IsBusy = false;
        }
    }
}