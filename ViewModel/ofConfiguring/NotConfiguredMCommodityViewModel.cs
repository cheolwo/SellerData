using BusinessData.ofDataAccessLayer.ofModelExtenstions;
using Microsoft.Extensions.Configuration;
using SellerCommon.SellerData.Model;
using SellerCommon.SellerData.ofRepository.ofMarket;
using SellerData.ofDataAccessLayer.ofDataContext;
using SellerData.ViewModel.Common;

namespace SellerCommon.SellerData.ViewModel.ofConfiguring
{
    // SMCommodity와 관련한 파일을 만드는 단계
    public class NotConfiguredMCommodityViewModel : SellerObservableObject
    {
        private readonly ISellerSMCommodityRepository _SellerSMCommodityRepository;
        private readonly IConfiguration _Configuration;
        private readonly string FilePath;
        public NotConfiguredMCommodityViewModel(SellerMarketDataContext sellerMarketDataContext,
        ISellerSMCommodityRepository sellerSMCommodityRepository,
        IConfiguration configuration) : base(sellerMarketDataContext)
        {
            _Configuration = configuration;
            _SellerSMCommodityRepository = sellerSMCommodityRepository;
            FilePath = _Configuration.GetSection("SMCommodityFilePath")["PlayAutoAgency"];
        }
        private string _FileName;
        public string FileName
        {
            get => _FileName;
            set => SetProperty(ref _FileName, value);
        }
        public List<SellerSMCommodity> SellerSMCommodities = new();
        public override async Task InitLoadAsync()
        {
            if(!IsLoad)
            {
                SellerSMCommodities = await _SellerSMCommodityRepository.GetToListAsyncNullDocs();
                IsLoad = true;
            }
        }
        public async Task ConfiguringFilesForRegisteringToOpenMarket()
        {
            foreach(var value in SellerSMCommodities)
            { 
                var FileStream = await value.ConvertToFileStream(_SellerMarketDataContext, FileName);
                
            }
        }
        private void ConfiguringFileForFileForRegisteringToOpenMarket()
        {
            
        }
        
    }
}