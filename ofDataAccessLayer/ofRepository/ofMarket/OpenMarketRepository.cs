using BusinessData.ofDataAccessLayer.ofGeneric.ofRepository;
using BusinessData.ofDataAccessLayer.ofMarket.ofRepository;
using SellerData.ofDataAccessLayer.ofDbContext;
using SellerData.ofDataAccessLayer.ofModel;

namespace SellerData.ofDataAccessLayer.ofRepository.ofMarket
{
    public interface IOpenMarketRepository : IMarketRepository<OpenMarket>
    {

    }
    public class OpenMarketRepository :  MarketRepository<OpenMarket>, IOpenMarketRepository
    {
        public OpenMarketRepository(OpenMarketDbContext dbContext)
            : base(dbContext)
        {

        }
    }
    public interface IOpenMarketCommonCategoryRepostiory : IEntityDataRepository<OpenMarketCommonCategory>
    {

    }
    public class OpenMarketCommonCategoryRepository : EntityDataRepository<OpenMarketCommonCategory>, IOpenMarketCommonCategoryRepostiory
    {
        public OpenMarketCommonCategoryRepository(OpenMarketDbContext dbContext)
            :base(dbContext)
        {

        }
    }
}
