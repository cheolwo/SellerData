using BusinessData.ofDataAccessLayer.ofCommon;
using BusinessData.ofDataAccessLayer.ofModelExtenstions;
using Microsoft.Extensions.Configuration;
using SellerCommon.SellerData.Model;
using SellerData.ofDataAccessLayer.ofDataContext;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace SellerData.Services.ofCollecitng
{
    public interface ICommodityImageStoringService
    {
        Task DistintStoringToBlobStorage(SellerMCommodity sellerMCommodity);
        Task ResizingAdditionalImageInBlobContainer(SellerMCommodity sellerMCommodity, int width, int height);
    }
    public class CommodityImageStoringService : ICommodityImageStoringService
    {
        private readonly IConfiguration _configuration;
        private readonly string ImageFolderPath;
        private readonly SellerMarketDataContext _sellerMarketContext;
        private readonly string _blobConnectionString;
        public CommodityImageStoringService(IConfiguration configuration, SellerMarketDataContext sellerMarketDataContext)
        {
            _configuration = configuration;
            _sellerMarketContext = sellerMarketDataContext;
            ImageFolderPath = _configuration.GetSection("Scrapping")["ImageDownloadPath"];
            _blobConnectionString = _configuration.GetSection("MarketCommodityStorage")["ConnectionString"];
        }
        public async Task DistintStoringToBlobStorage(SellerMCommodity sellerMCommodity)
        {
            if (sellerMCommodity.Container == null) { throw new ArgumentNullException(nameof(sellerMCommodity.Container)); }
            string FolderPath = Path.Combine(ImageFolderPath, sellerMCommodity.Container);
            var DirectoryInfo = Directory.CreateDirectory(FolderPath);
            var newImgaeFiles = CheckNewFiles(sellerMCommodity.ImageofInfos, DirectoryInfo);
            if (newImgaeFiles.Count == 0) { Console.WriteLine("Not Blob Update"); return; }
            await sellerMCommodity.UploadDistintImageAsnyc(sellerMCommodity.ImageofInfos, _sellerMarketContext, _blobConnectionString);
        }
        /// <summary>
        /// 로컬에도 같은 이미지를 찾아서 변경하고 업로드한다.
        /// </summary>
        /// <param name="sellerMCommodity"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task ResizingAdditionalImageInBlobContainer(SellerMCommodity sellerMCommodity, int width, int height)
        {
            if (sellerMCommodity.Container == null) { throw new ArgumentNullException(nameof(sellerMCommodity.Container)); }
            string FolderPath = Path.Combine(ImageFolderPath, sellerMCommodity.Container);
            var ResizingImageofInfos = CheckAndResizingImgae(FolderPath, sellerMCommodity.ImageofInfos, width, height);
            if (ResizingImageofInfos.Count == 0) { Console.WriteLine("No Resizing"); return; }
            await sellerMCommodity.UploadDistintImageAsnyc(ResizingImageofInfos, _sellerMarketContext, _blobConnectionString);
        }
        private List<ImageofInfo> CheckAndResizingImgae(string FolderPath, List<ImageofInfo> imageofInfos, int width, int height)
        {
            List<string> ResizingFileNames = new();
            List<string> ImageofInfos = new();
            List<ImageofInfo> FindImageofInfos = new();
            var DirectoryInfo = Directory.CreateDirectory(FolderPath);
            var Files = DirectoryInfo.GetFiles();
            foreach (var file in Files)
            {
                if (file.Name.Contains("대표이미지") || file.Name.Contains("추가이미지"))
                {
                    using (Image ResizingImgae = Image.Load(file.FullName))
                    {
                        if (ResizingImgae.Height != height || ResizingImgae.Width != width)
                        {
                            ResizingImgae.Mutate(e => e.Resize(width, height));
                            ResizingImgae.Save(file.FullName);
                        }
                    }
                    var FindValue = imageofInfos.Where(e => e.fileName.Equals(file.Name)).FirstOrDefault();

                    FindImageofInfos.Add(FindValue);
                }
            }
            return FindImageofInfos;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageofInfos"></param>
        /// <param name="directoryInfo"></param>
        /// <returns>new ImgaeofInfos</returns>
        private List<ImageofInfo> CheckNewFiles(List<ImageofInfo> imageofInfos, DirectoryInfo directoryInfo)
        {
            int cnt = 0;
            var Files = directoryInfo.EnumerateFiles();
            List<ImageofInfo> newImageofInfos = new();
            foreach (var file in Files)
            {
                var value = imageofInfos.Where(e => e.Info.Equals(file.FullName)).FirstOrDefault();
                ImageofInfo imageofInfo = new();
                imageofInfo.fileName = file.Name;
                imageofInfo.Info = file.FullName;
                if (file.Name.Contains("추가이미지")) { imageofInfo.Purpose = "추가이미지"; }
                if (file.Name.Contains("대표이미지")) { imageofInfo.Purpose = "대표이미지"; }
                imageofInfos.Add(imageofInfo);
                newImageofInfos.Add(imageofInfo);
            }
            return newImageofInfos;
        }
    }
}
