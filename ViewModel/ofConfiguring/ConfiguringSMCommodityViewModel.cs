using BusinessData.ofDataAccessLayer.ofModelExtenstions;
using Microsoft.Extensions.Logging;
using SellerCommon.SellerData.Model;
using SellerCommon.SellerData.ofRepository.ofMarket;
using SellerData.ofDataAccessLayer.ofDataContext;
using SellerData.Services.ofConfiguring;
using SellerData.ViewModel.Common;

namespace SellerCommon.SellerData.ViewModel.ofCofugring
{
    public class ConfiguringSMCommodityViewModel : SellerObservableObject
    {
        private readonly ICommodityOriginCodeService _commodityOriginCodeService;
        private readonly ICommodityDeliveryCodeService _commodityDeliveryCodeService;
        private readonly ICommodityCategoryCodeService _commodityCategoryCodeService;
        private readonly ISellerSMCommodityRepository _sellerSMCommodityRepository;
        private readonly ISellerMCommodityRepository _sellerMCommodityRepository;
        private readonly ILogger<ConfiguringSMCommodityViewModel> _logger;
        public ConfiguringSMCommodityViewModel(SellerMarketDataContext sellerMarketDataContext, ICommodityOriginCodeService commodityOriginCodeService, 
                                                                                ICommodityDeliveryCodeService commodityDeliveryCodeService,
                                                                                ICommodityCategoryCodeService commodityCategoryCodeService,
                                                                                ISellerSMCommodityRepository sellerSMCommodityRepository,
                                                                                ILogger<ConfiguringSMCommodityViewModel> logger, ISellerMCommodityRepository sellerMCommodityRepository) : base(sellerMarketDataContext)
        {
            _commodityCategoryCodeService = commodityCategoryCodeService;
            _commodityDeliveryCodeService = commodityDeliveryCodeService;
            _commodityOriginCodeService = commodityOriginCodeService;
            _sellerSMCommodityRepository = sellerSMCommodityRepository;
            _logger = logger;
            _sellerMCommodityRepository = sellerMCommodityRepository;
        }
        public List<SellerSMCommodity> sellerSMCommodities = new();
        public List<SellerMCommodity> SellerMCommodities = new();
        public SellerMarket SellerMarket = new();
        public List<string> Images = new();
        public bool IsImageView = false;
        public override async Task InitLoadAsync()
        {
            SellerMCommodities = await _sellerMCommodityRepository.GetToListAsync();
            SellerMCommodities = SellerMCommodities.Where(e => e.Name != null).ToList();
            SellerMarket = await _SellerMarketDataContext.GetByNameAsync<SellerMarket>("Onch3");
            sellerSMCommodities = await _SellerMarketDataContext.GetsAsync<SellerSMCommodity>();
        }
        public void ResetImageView()
        {
            Images = new();
            IsImageView = false;
        }
        public void SelectedImageView(string Id)
        {
            Images = new();
            var FindValue = SellerMCommodities.Find(e => e.Id.Equals(Id));
            Images = FindValue.DetailImages;
            IsImageView = true;
        }
        public async Task InitSellerSMCommodity()
        {
            foreach(var value in SellerMCommodities)
            {
                var FindValue = await _sellerSMCommodityRepository.GetBySellerMCommodityIdAsync(value.Id);
                if(FindValue == null)
                {
                    SellerSMCommodity sellerSMCommodity = new();
                    sellerSMCommodity.CommodityId = value.Id;
                    sellerSMCommodity.SellerMCommodityId = value.Id;
                    sellerSMCommodity.SellerMarketId = SellerMarket.Id;
                    await sellerSMCommodity.PostAsync(_SellerMarketDataContext);
                }
            }
        }
        public async Task SetOriginCode()
        {
            var NoOriginCodesSMCommodity = sellerSMCommodities.Where(e => e.OriginCode == null).ToList();
            _logger.LogInformation(NoOriginCodesSMCommodity.Count.ToString());
            foreach(var SMCommodity in NoOriginCodesSMCommodity)
            {
                _logger.LogInformation(SMCommodity.Id.ToString());
                await _commodityOriginCodeService.SetCode(SMCommodity);
            }
        }
        public async Task SetDeliveryCode()
        {
            var NoDeliveryCodes = sellerSMCommodities.Where(e => e.DeliveryCode == null).ToList();
            _logger.LogInformation(NoDeliveryCodes.Count.ToString());
            foreach (var SMCommodity in NoDeliveryCodes)
            {
                _logger.LogInformation(SMCommodity.Id.ToString());
                await _commodityDeliveryCodeService.SetCode(SMCommodity);
            }
        }
        public async Task SetCategoryCode()
        {
            var NoCategoryCodes = sellerSMCommodities.Where(e => e.Code == null).ToList();
            int cnt = 1;
            foreach(var SMCommodity in NoCategoryCodes)
            {
                _logger.LogInformation(NoCategoryCodes.Count.ToString());
                _logger.LogInformation(cnt.ToString());
                cnt++;
                await _commodityCategoryCodeService.SetCode(SMCommodity);
            }
        }
    }
}