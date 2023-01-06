using BusinessData.ofDataAccessLayer.ofGeneric.ofIdFactory;
using BusinessData.ofDataAccessLayer.ofGeneric.ofRepository;
using BusinessData.ofDataContext;
using Microsoft.Extensions.DependencyInjection;
using SellerData.ofDataAccessLayer.ofModel;

namespace SellerData.ofDataAccessLayer.ofDataContext
{
    public class OpenMarketDataContext : DataContext
    {
        public OpenMarketDataContext(IServiceScopeFactory serviceScopeFactory)
                : base(serviceScopeFactory)
        {

        }
        protected override void OnEntityBlobStorageBuilder(EntityManagerBuilder entityManagerBuilder)
        {
            base.OnEntityBlobStorageBuilder(entityManagerBuilder);
        }

        protected override void OnEntityExcelBuilder(EntityManagerBuilder entityManagerBuilder)
        {
            base.OnEntityExcelBuilder(entityManagerBuilder);
        }
        protected override void OnEntityIdBuilder(EntityManagerBuilder entityManagerBuilder)
        {
            entityManagerBuilder.ApplyEntityIdFactory(typeof(OpenMarket).Name, new EntityIdFactory<OpenMarket>());
            entityManagerBuilder.ApplyEntityIdFactory(typeof(OpenMarketCategory).Name, new EntityIdFactory<OpenMarketCategory>());
            entityManagerBuilder.ApplyEntityIdFactory(typeof(OpenMarketCommonCategory).Name, new EntityIdFactory<OpenMarketCommonCategory>());
            entityManagerBuilder.ApplyEntityIdFactory(typeof(CommodityOriginCode).Name, new EntityIdFactory<CommodityOriginCode>());
            entityManagerBuilder.ApplyEntityIdFactory(typeof(CommodityDeliveryCode).Name, new EntityIdFactory<CommodityDeliveryCode>());
        }
        protected override void OnEntityRepositoryBuilder(EntityManagerBuilder entityManagerBuilder)
        {
            entityManagerBuilder.ApplyEntityDataRepository(typeof(OpenMarket).Name, new EntityDataRepository<OpenMarket>());
            entityManagerBuilder.ApplyEntityDataRepository(typeof(OpenMarketCategory).Name, new EntityDataRepository<OpenMarketCategory>());
            entityManagerBuilder.ApplyEntityDataRepository(typeof(OpenMarketCommonCategory).Name, new EntityDataRepository<OpenMarketCommonCategory>());
            entityManagerBuilder.ApplyEntityDataRepository(typeof(CommodityOriginCode).Name, new EntityDataRepository<CommodityOriginCode>());
            entityManagerBuilder.ApplyEntityDataRepository(typeof(CommodityDeliveryCode).Name, new EntityDataRepository<CommodityDeliveryCode>());
        }

        protected override void OnEntityPDFBuilder(EntityManagerBuilder entityManagerBuilder)
        {
            base.OnEntityPDFBuilder(entityManagerBuilder);
        }

    }
}
