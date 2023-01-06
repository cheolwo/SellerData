using BusinessData.ofDataAccessLayer.ofCommon;
using BusinessData.ofDataAccessLayer.ofModelExtenstions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SellerCommon.SellerData.Model;
using SellerCommon.SellerData.ofRepository.ofMarket;
using SellerCommon.SellerData.ViewModel.ofWholeSale;
using SellerData.ofDataAccessLayer.ofDataContext;
using SellerData.ViewModel.Common;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace SellerData.ViewModel.ofCollecting
{
    public class CollectingImageViewModel : SellerObservableObject
    {
        private readonly IConfiguration _Configuration;
        private readonly ISellerMCommodityRepository _SellerMCommodityRepository;
        private readonly ISellerMarketRepository _SellerMarketRepository;
        private readonly ILogger<CollectingImageViewModel> _logger;
        private readonly string DownloadPath;
        private readonly string BlobStorageConnectionString;
        private readonly string BlobStorageAccessKey;
        public CollectingImageViewModel(SellerMarketDataContext sellerMarketDataContext, ISellerMCommodityRepository SellerMCommodityRepository,
                                                            ISellerMarketRepository SellerMarketRepositiory,
                                                                IConfiguration Configuration,
                                                                ILogger<CollectingImageViewModel> logger) : base(sellerMarketDataContext)
        {
            _SellerMCommodityRepository = SellerMCommodityRepository;
            _SellerMarketRepository = SellerMarketRepositiory;
            _Configuration = Configuration;
            _logger = logger;
            DownloadPath = _Configuration.GetSection("Scrapping")["ImageDownloadPath"];
            BlobStorageConnectionString = _Configuration.GetSection("MarketCommodityStorage")["ConnectionString"];
            BlobStorageAccessKey = _Configuration.GetSection("MarketCommodityStorage")["AccessKey"];
            Console.Write(DownloadPath);
        }
        private HttpClient HttpClient = new();
        public SellerMarket SellerMarket = new();
        public List<SellerMCommodity> SellerMCommodities = new();
        public List<SellerMCommodity> NoRepresentativeImgaeSellerMCommodities = new();
        public List<SellerMCommodity> NoDetailImageSellerMCommodities = new();
        public List<SellerMCommodity> NoContainerSellerMCommodities = new();
        public override async Task InitLoadAsync()
        {
            if (!IsLoad)
            {
                SellerMarket = await _SellerMarketRepository.GetByNameAsync(OnChannal.OnChannalName);
                SellerMCommodities = await _SellerMCommodityRepository.GetToListByCenterIdAsync(SellerMarket.Id);
                NoRepresentativeImgaeSellerMCommodities = SellerMCommodities.Where(e => e.ImageofInfos.Count == 0).ToList();
                NoDetailImageSellerMCommodities = SellerMCommodities.Where(e => e.ImageofInfos.Count <= 1).ToList();
                NoContainerSellerMCommodities = SellerMCommodities.Where(e => e.Container == null && e.Name != null).ToList();
                _logger.Log(LogLevel.Information, $"{nameof(NoDetailImageSellerMCommodities)} : {NoDetailImageSellerMCommodities.Count}");
                IsLoad = true;
            }
        }
        public async Task UploadImageToBlobStorage()
        {
            int cnt = 1;
            foreach(var value in NoContainerSellerMCommodities)
            {
                _logger.LogInformation(cnt.ToString());
                _logger.LogInformation(value.Name);
                cnt++;

                await value.UploadImageAsync(_SellerMarketDataContext, BlobStorageConnectionString);
            }
        }
        public async Task ChangeContainerAccessLevelToPublic()
        {
            var values = SellerMCommodities.Where(e => e.Container != null).ToList();
            foreach(var value in values)
            {
                await _SellerMarketDataContext.ChangeContainerAccessLevelToPublic(value, BlobStorageConnectionString);
                _logger.LogInformation(value.Name);
            }
        }
        public async Task DeletAllBlob()
        {
            var values = SellerMCommodities.Where(e => e.Container != null).ToList();
            foreach(var value in values)
            {
                await _SellerMarketDataContext.DeleteBlobContainer(value, BlobStorageConnectionString);
                value.Container = null;
                await value.PutAsync(_SellerMarketDataContext);
            }
        }
        public async Task DownloadingSingleImage(string code)
        {
            var value = SellerMCommodities.FirstOrDefault(e => e.Code.Equals(code));
            if (value != null)
            {
                HttpClient httpClient = new();
                Directory.CreateDirectory(DownloadPath);
                var MCommodityImagePath = Path.Combine(DownloadPath, value.Name);
                Directory.CreateDirectory(MCommodityImagePath);
                int cnt = 1;
                var detailImages = value.DetailImages;
                foreach (var imagepath in detailImages)
                {
                    string ImageName = value.Name + cnt.ToString() + ".jpg";
                    var path = Path.Combine(MCommodityImagePath, ImageName);
                    try
                    {
                        var filestream = await httpClient.GetStreamAsync(imagepath);
                        if (filestream != null)
                        {
                            var NewFile = File.Create(path);
                            await filestream.CopyToAsync(NewFile);
                            cnt++;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.Write(e.Message);
                        continue;
                    }
                }
            }
        }
        /// <summary>
        /// Downloading SellerMCommodity DetailImage
        /// </summary>
        public async Task DownloadingImage()
        {
            HttpClient httpClient = new();
            int cnt = 1;
            Directory.CreateDirectory(DownloadPath);
            foreach (var MCommodity in SellerMCommodities)
            {
                Console.WriteLine(MCommodity.Code);
                var MCommodityImagePath = Path.Combine(DownloadPath, MCommodity.Code);
                Directory.CreateDirectory(MCommodityImagePath);
                var detailImages = MCommodity.DetailImages;
                foreach (var imagepath in detailImages)
                {
                    try
                    {
                        string ImageName = MCommodity.Name + cnt.ToString() + ".jpg";
                        cnt++;
                        var path = Path.Combine(MCommodityImagePath, ImageName);
                        var filestream = await httpClient.GetStreamAsync(imagepath);
                        if (filestream != null)
                        {
                            var NewFile = File.Create(path);
                            await filestream.CopyToAsync(NewFile);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        Console.WriteLine(imagepath);
                        continue;
                    }
                }
                cnt = 1;
            }
        }
        /// <summary>
        /// 1. SellerMCommodities를 반복하는 단계
        /// 2. 반복하면서 이미지를 다운로드하는 단계
        /// 3. 디렉토리 및 파일명을 네이밍하는 단계
        /// 4. 다운로드된 파일을 1000x1000 으로 리사이징하여 네이밍 파일경로에 저장하는 단계
        /// 5. 상기 경로를 ImageofInfo에 저장하는 단계를 포함한다.
        /// </summary>
        /// <returns></returns>
        public async Task StoringRepresentativeImage()
        {
            int cnt = 1;
            foreach (var value in NoRepresentativeImgaeSellerMCommodities)
            {
                var RepresentativeImgaeUrl = value.RepresentativeImageUrl;
                var Stream = await HttpClient.GetStreamAsync(RepresentativeImgaeUrl);
                var DirectoryPath = Path.Combine(DownloadPath, value.Code ?? "");
                Directory.CreateDirectory(DirectoryPath);
                var FilePath = Path.Combine(DirectoryPath, "대표이미지.jpg");
                using (var newFileStream = File.Create(FilePath))
                {
                    await Stream.CopyToAsync(newFileStream);
                }

                using (Image image = Image.Load(FilePath))
                {
                    if (image.Height != 1000 || image.Width != 1000)
                    {
                        image.Mutate(e => e.Resize(1000, 1000));
                        image.Save(FilePath);
                    }
                }
                ImageofInfo imageofInfo = new();
                imageofInfo.Purpose = "대표이미지";
                imageofInfo.fileName = imageofInfo.Purpose + "-" + cnt.ToString();
                imageofInfo.Info = FilePath;
                imageofInfo.Id = cnt;
                value.ImageofInfos.Add(imageofInfo);
                await value.PutAsync(_SellerMarketDataContext);

                _logger.Log(LogLevel.Information, value.Id);
                _logger.Log(LogLevel.Information, cnt.ToString());
                _logger.Log(LogLevel.Information, imageofInfo.Purpose);
                _logger.Log(LogLevel.Information, imageofInfo.Info);
                _logger.Log(LogLevel.Information, imageofInfo.fileName);
            }
            _logger.Log(LogLevel.Information, "Finish!!!!!");
        }
        /// <summary>
        /// 1. NoDetailImgaeSellerMCommodities를 반복하는 단계
        /// 2. Directory Image Path를 생성하는 단계
        /// 3. DetailImage를 반복하는 단계
        /// 4. 상기 Path에 상세이미지 파일을 네이밍하여 DetailImage 파일을 생성하는 단계
        /// 5. DetailImage 경로를 스트림으로 받는 단계
        /// 6. 상기 스트림을 상기 DetailImgae 파일에 저장하는 단계를 포함한다.
        /// </summary>
        /// <returns></returns>
        public async Task StoringDetailImages()
        {
            foreach (var value in NoDetailImageSellerMCommodities)
            {
                int cnt = 1;
                string DetailImageDirectoryPath = Path.Combine(DownloadPath, value.Code, "상세이미지");
                Directory.CreateDirectory(DetailImageDirectoryPath);
                if (value.DetailImages.Count == 0) { continue; }
                foreach (var DetailImageSrc in value.DetailImages)
                {
                    try
                    {
                        var Stream = await HttpClient.GetStreamAsync(DetailImageSrc);
                        var naming = "상세이미지" + "-" + cnt.ToString() + ".jpg";
                        var FilePath = Path.Combine(DetailImageDirectoryPath, naming);
                        using (var FileStream = File.Create(FilePath))
                        { 
                            await Stream.CopyToAsync(FileStream);
                        }
                        cnt++;
                        ImageofInfo imageofInfo = new();
                        imageofInfo.Info = FilePath;
                        imageofInfo.Purpose = "상세이미지";
                        imageofInfo.fileName = naming;
                        imageofInfo.Id = value.ImageofInfos.Count + 1;
                        _logger.Log(LogLevel.Information, value.Id);
                        _logger.Log(LogLevel.Information, cnt.ToString());
                        _logger.Log(LogLevel.Information, imageofInfo.Purpose);
                        _logger.Log(LogLevel.Information, imageofInfo.Info);
                        _logger.Log(LogLevel.Information, imageofInfo.fileName);
                        value.ImageofInfos.Add(imageofInfo);
                    }
                    catch
                    {
                        continue;
                    }
                }
                await value.PutAsync(_SellerMarketDataContext);
            }
            _logger.Log(LogLevel.Information, "Finish!!!!!");
        }
        public async Task AllImgaeDeleting()
        {
            int cnt = 1;
            foreach(var value in SellerMCommodities)
            {
                value.ImageofInfos = new();
                await value.PutAsync(_SellerMarketDataContext);
                _logger.Log(LogLevel.Information, value.Id);
                _logger.Log(LogLevel.Information, cnt.ToString());
                cnt++;
            }
        }
        public async Task DeleteDetailFileImage()
        {
            
        }
    }
}
