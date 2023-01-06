using BusinessData.ofDataAccessLayer.ofGeneric.ofIdFactory;
using BusinessData.ofDataAccessLayer.ofGeneric.ofRepository;
using BusinessData.ofDataContext;
using BusinessLogic.ofEntityManager.ofGeneric.ofBlobStorage;
using Microsoft.Extensions.DependencyInjection;
using SellerCommon.SellerData.Model;
using SellerData.ofDataAccessLayer.ofFileFactory;

namespace SellerData.ofDataAccessLayer.ofDataContext
{
    public class SellerMarketDataContext : DataContext
    {
        public SellerMarketDataContext(IServiceScopeFactory serviceScopeFactory)
                :base(serviceScopeFactory)
        {
            
        }
        protected override void OnEntityBlobStorageBuilder(EntityManagerBuilder entityManagerBuilder)
        {
            entityManagerBuilder.ApplyEntityBlobStorage(typeof(SellerMarket).Name, new EntityBlobStorage<SellerMarket>());
            entityManagerBuilder.ApplyEntityBlobStorage(typeof(SellerPlatMarket).Name, new EntityBlobStorage<SellerPlatMarket>());
            entityManagerBuilder.ApplyEntityBlobStorage(typeof(SellerMCommodity).Name, new EntityBlobStorage<SellerMCommodity>());
            entityManagerBuilder.ApplyEntityBlobStorage(typeof(SellerSMCommodity).Name, new EntityBlobStorage<SellerSMCommodity>());
            entityManagerBuilder.ApplyEntityBlobStorage(typeof(SellerMMCommodity).Name, new EntityBlobStorage<SellerMMCommodity>());
            entityManagerBuilder.ApplyEntityBlobStorage(typeof(SellerEMCommodity).Name, new EntityBlobStorage<SellerEMCommodity>());
            entityManagerBuilder.ApplyEntityBlobStorage(typeof(CommodityAnalyzedInfo).Name, new EntityBlobStorage<CommodityAnalyzedInfo>());
            entityManagerBuilder.ApplyEntityBlobStorage(typeof(SellerOpenMarket).Name, new EntityBlobStorage<SellerOpenMarket>());
        }

        protected override void OnEntityExcelBuilder(EntityManagerBuilder entityManagerBuilder)
        {
            entityManagerBuilder.ApplyEntityExcelFileFactory(typeof(SellerSMCommodity).Name, new SellerSMCommodityExcelFileFactory());
        }

        protected override void OnEntityIdBuilder(EntityManagerBuilder entityManagerBuilder)
        {
            entityManagerBuilder.ApplyEntityIdFactory(typeof(SellerMarket).Name, new EntityIdFactory<SellerMarket>());
            entityManagerBuilder.ApplyEntityIdFactory(typeof(SellerPlatMarket).Name, new EntityIdFactory<SellerPlatMarket>());
            entityManagerBuilder.ApplyEntityIdFactory(typeof(SellerMCommodity).Name, new EntityIdFactory<SellerMCommodity>());
            entityManagerBuilder.ApplyEntityIdFactory(typeof(SellerSMCommodity).Name, new EntityIdFactory<SellerSMCommodity>());
            entityManagerBuilder.ApplyEntityIdFactory(typeof(SellerMMCommodity).Name, new EntityIdFactory<SellerMMCommodity>());
            entityManagerBuilder.ApplyEntityIdFactory(typeof(SellerEMCommodity).Name, new EntityIdFactory<SellerEMCommodity>());
            entityManagerBuilder.ApplyEntityIdFactory(typeof(CommodityAnalyzedInfo).Name, new EntityIdFactory<CommodityAnalyzedInfo>());
            entityManagerBuilder.ApplyEntityIdFactory(typeof(SellerOpenMarket).Name, new EntityIdFactory<SellerOpenMarket>());
        }
        protected override void OnEntityRepositoryBuilder(EntityManagerBuilder entityManagerBuilder)
        {
            entityManagerBuilder.ApplyEntityDataRepository(typeof(SellerMarket).Name, new EntityDataRepository<SellerMarket>());
            entityManagerBuilder.ApplyEntityDataRepository(typeof(SellerPlatMarket).Name, new EntityDataRepository<SellerPlatMarket>());
            entityManagerBuilder.ApplyEntityDataRepository(typeof(SellerMCommodity).Name, new EntityDataRepository<SellerMCommodity>());
            entityManagerBuilder.ApplyEntityDataRepository(typeof(SellerSMCommodity).Name, new EntityDataRepository<SellerSMCommodity>());
            entityManagerBuilder.ApplyEntityDataRepository(typeof(SellerMMCommodity).Name, new EntityDataRepository<SellerMMCommodity>());
            entityManagerBuilder.ApplyEntityDataRepository(typeof(SellerEMCommodity).Name, new EntityDataRepository<SellerEMCommodity>());
            entityManagerBuilder.ApplyEntityDataRepository(typeof(CommodityAnalyzedInfo).Name, new EntityDataRepository<CommodityAnalyzedInfo>());
            entityManagerBuilder.ApplyEntityDataRepository(typeof(SellerOpenMarket).Name, new EntityDataRepository<SellerOpenMarket>());
        }

        protected override void OnEntityPDFBuilder(EntityManagerBuilder entityManagerBuilder)
        {
            base.OnEntityPDFBuilder(entityManagerBuilder);
        }

    }
}
