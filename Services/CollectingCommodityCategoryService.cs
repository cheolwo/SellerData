using BusinessData.ofDataAccessLayer.ofModelExtenstions;
using Microsoft.Extensions.Logging;
using Microsoft.Office.Interop.Excel;
using SellerCommon.SellerData.Model;
using SellerCommon.SellerData.ofPresentationLayer.ofExtensions;
using SellerData.ofDataAccessLayer.ofDataContext;
using SellerData.ofDataAccessLayer.ofModel;

namespace SellerData.Services
{
    public interface ICollectingCommodityCategoryService
    {
        Task CollectingCategory(SellerMCommodity sellerMCommodity);
        Task DeleteAllCategory(List<SellerMCommodity> sellerMCommodities);
        List<CategoryPoint> CheckPoint(List<string> keywords, List<CategoryPoint> CategoryPoint);
        List<CategoryPoint> ConvertToPointNode(List<OpenMarketCommonCategory> openMarketCommonCategories);
    }
    public class CollectingCommodityCategoryService : ICollectingCommodityCategoryService
    {
        private readonly ILogger<CollectingCommodityCategoryService> _Logger;
        private readonly SellerMarketDataContext _sellerMarketDataContext;
        private readonly OpenMarketDataContext _openMarketDataContext;
        public CollectingCommodityCategoryService(ILogger<CollectingCommodityCategoryService> logger, SellerMarketDataContext sellerMarketDataContext, OpenMarketDataContext openMarketDataContext)
        {
            _Logger = logger;
            _sellerMarketDataContext = sellerMarketDataContext;
            _openMarketDataContext = openMarketDataContext; 
        }
        public async Task CollectingCategory(SellerMCommodity sellerMCommodity)
        {
            var CategoryList = await _openMarketDataContext.GetsAsync<OpenMarketCommonCategory>();
            var keywords = sellerMCommodity.Keywords.SplitKeywords();
            var splitNameKwds = sellerMCommodity.Name.Split(" ");
            foreach(var namekwd in splitNameKwds)
            {
                keywords.Add(namekwd);
            }
            if(keywords == null) { throw new ArgumentNullException(nameof(keywords)); }
            var CategoryPoint = ConvertToPointNode(CategoryList);
            CheckPoint(keywords, CategoryPoint);
            var MaxCategoryPoint = CategoryPoint.MaxBy(e => e.Point);
            if(sellerMCommodity != null && MaxCategoryPoint != null)
            {
                _Logger.LogInformation(MaxCategoryPoint.OpenMarketCommonCategory.Id);
                sellerMCommodity.Category = MaxCategoryPoint.OpenMarketCommonCategory.Id;
                await sellerMCommodity.PutAsync(_sellerMarketDataContext);
            }
        }
        public List<CategoryPoint> CheckPoint(List<string> keywords, List<CategoryPoint> CategoryPoint)
        {
            foreach (var kwd in keywords)
            {
                foreach (var categoryrow in CategoryPoint)
                {
                    if (categoryrow.OpenMarketCommonCategory.Category1.Contains(kwd))
                    {
                        _Logger.LogInformation($"Category1 Get Point : {categoryrow.OpenMarketCommonCategory.Category1}");
                        categoryrow.Point++;
                    }
                    if (categoryrow.OpenMarketCommonCategory.Category2.Contains(kwd))
                    {
                        _Logger.LogInformation($"Category2 Get Point : {categoryrow.OpenMarketCommonCategory.Category2}");
                        categoryrow.Point++;
                    }
                    if (categoryrow.OpenMarketCommonCategory.Category3.Contains(kwd))
                    {
                        if(categoryrow.OpenMarketCommonCategory.Category4 == null)
                        {
                            categoryrow.Point += 2;
                            _Logger.LogInformation($"Category3 Get Point : {categoryrow.OpenMarketCommonCategory.Category3}");
                            continue;
                        }
                        categoryrow.Point++;
                    }
                    if (categoryrow.OpenMarketCommonCategory.Category4.Contains(kwd))
                    {
                        _Logger.LogInformation($"Category4 Get Point : {categoryrow.OpenMarketCommonCategory.Category4}");
                        categoryrow.Point += 2;
                    }
                }
            }
            CategoryPoint.Sort();
            CategoryPoint.Reverse();
            return CategoryPoint;
        }
        public async Task DeleteAllCategory(List<SellerMCommodity> sellerMCommodities)
        {
            foreach(var value in sellerMCommodities)
            {
                value.Category = null;
                await value.PutAsync(_sellerMarketDataContext);
            }
        }
        public List<CategoryPoint> ConvertToPointNode(List<OpenMarketCommonCategory> openMarketCommonCategories)
        {
            List<CategoryPoint> categoryPoints = new();
            foreach(var value in openMarketCommonCategories)
            {
                CategoryPoint categoryPoint = new();
                categoryPoint.OpenMarketCommonCategory = value;
                categoryPoint.Point = 0;
                categoryPoints.Add(categoryPoint);
            }
            return categoryPoints;
        }
    }
    public class CategoryPoint : IComparable<CategoryPoint>
    {
        public OpenMarketCommonCategory OpenMarketCommonCategory { get; set; }
        public int Point { get; set; }
        public CategoryPoint()
        {
            OpenMarketCommonCategory = new();
        }

        public int CompareTo(CategoryPoint? other)
        {
            if (other == null)
            {
                return 1;
            }
            else
                return this.Point.CompareTo(other.Point);
        }
    }
}
