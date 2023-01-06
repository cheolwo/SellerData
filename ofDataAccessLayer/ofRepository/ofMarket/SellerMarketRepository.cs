using BusinessData.ofDataAccessLayer.ofGeneric.ofRepository;
using BusinessData.ofDataAccessLayer.ofMarket.ofModel;
using BusinessData.ofDataAccessLayer.ofMarket.ofRepository;
using Microsoft.EntityFrameworkCore;
using SellerCommon.SellerData.Model;
using SellerCommon.SellerData.ofDbContext;

namespace SellerCommon.SellerData.ofRepository.ofMarket
{
    public interface ISellerPlatMarketRepository : IPlatMarketRepository<SellerPlatMarket>
    {

    }
    public interface ISellerMarketRepository : IMarketRepository<SellerMarket>
    {
        Task<SellerMarket> GetByNameWithMCommodities(string name);
    }
    public interface ISellerMCommodityRepository : IMCommodityRepository<SellerMCommodity>
    {
        Task<SellerMCommodity> GetByCommodityPageUrl(string commodityPageUrl);
        Task<List<SellerMCommodity>> GetToListWithAnalyzedInfos();
        Task<List<SellerMCommodity>> GetToListByCenterIdAndWithAnalyzedInfos(string CenterId);
        Task<SellerMCommodity> GetByIdWithAnalyzedInfos(string Id);
        Task<List<SellerMCommodity>> GetToListByNoDataofSMCommodity();
        Task<List<SellerMCommodity>> GetToListByNoDataofSMCommodityAndWithCommodityAnalyzedInfos();
        Task<SellerMCommodity> GetByIdWithSellerMarketAsync(string id);
        Task<List<SellerMCommodity>> GetToListWithSMCommodity();
        Task<List<SellerMCommodity>> GetToListBySellerMarketIdWithSMCommodity(string SellerMarketId);
        Task<List<SellerMCommodity>> GetToListByNoNameAsync();
        Task RemoveDistintSMCommodity();
    }
    public interface ISellerSMCommodityRepository : ISMCommodityRepository<SellerSMCommodity>
    {
        Task<List<SellerSMCommodity>> GetToListAsyncNullDocs();
        Task<List<SellerSMCommodity>> GetToListByIsConfiguredImgaeAsync();
        Task<List<SellerSMCommodity>> GetToListWithSellerMCommoditiyAsync();
        Task<List<SellerSMCommodity>> GetToListWithSellerMCommodityAndMarketAsync();
        Task<List<SellerSMCommodity>> GetToListByExportingConditions();
        Task<List<SellerSMCommodity>> GetToListByExportingConditionsWithSellerMCommodity();
        Task<SellerSMCommodity> GetBySellerMCommodityIdAsync(string sellerMCommodityId);
        Task<List<SellerSMCommodity>> GetToListWithSellerMCommodityFilterByImportantKwds();
    }
    public interface ISellerMMCommodityRepository : IMMCommodityRepository<SellerMMCommodity>
    {
        Task<SellerMMCommodity> GetBySellerMMCommodityIdAndNameofMarketAsync(SellerSMCommodity sellerSMCommodity, string OpenMarketId);
        Task<SellerMMCommodity> GetBySMCommodityIdAndMarketId(SellerSMCommodity sellerSMCommodity, SellerOpenMarket sellerOpenMarket);
        Task<SellerMMCommodity> GetByMCommodityIdAndMarketId(SellerMCommodity sellerMCommodity, SellerOpenMarket sellerOpenMarket);
        Task<List<SellerMMCommodity>> GetToListWithRelatedDatas();
        Task<List<SellerMMCommodity>> GetToListWithMCommodityFilterByCommodityCode(string code);
    }
    public interface ISellerEMCommodityRepository : IEMCommodityRepository<SellerEMCommodity>
    {
    
    }
    public interface ICommodityAnalyzedInfoRepository : IEntityDataRepository<CommodityAnalyzedInfo>
    {
        Task<List<CommodityAnalyzedInfo>> GetToListByMCommodityIdAsync(string MCommodityId);
        Task<List<CommodityAnalyzedInfo>> GetToListByMCommodityIdWithNotNullTotalCommodity(string MCommodityId);
        Task<List<CommodityAnalyzedInfo>> GetToListofTotalCommodityIsZeroAsync();
    }
    public class CommodityAnalyzedInfoRepository : EntityDataRepository<CommodityAnalyzedInfo>, ICommodityAnalyzedInfoRepository
    {
        public CommodityAnalyzedInfoRepository(SellerMarketDbContext dbContext)
            :base(dbContext)
        {

        }

        public async Task<List<CommodityAnalyzedInfo>> GetToListByMCommodityIdAsync(string MCommodityId)
        {
            return await _DbContext.Set<CommodityAnalyzedInfo>().Where(e => e.SellerMCommodityId.Equals(MCommodityId)).ToListAsync();
        }

        public async Task<List<CommodityAnalyzedInfo>> GetToListByMCommodityIdWithNotNullTotalCommodity(string MCommodityId)
        {
            return await _DbContext.Set<CommodityAnalyzedInfo>().Where(e=>e.SellerMCommodityId.Equals(MCommodityId)).Where(e=>e.TotalCommodityCount > 0).ToListAsync();
        }

        public async Task<List<CommodityAnalyzedInfo>> GetToListofTotalCommodityIsZeroAsync()
        {
            return await _DbContext.Set<CommodityAnalyzedInfo>().Where(e => e.TotalCommodityCount <= 0).ToListAsync();
        }
    }
    public class SellerPlatMarketRepository : PlatMarketRepository<SellerPlatMarket>, ISellerPlatMarketRepository
    {
        public SellerPlatMarketRepository(SellerMarketDbContext dbContext)
            : base(dbContext)
        {

        }
    }
    public class SellerMarketRepository : MarketRepository<SellerMarket>, ISellerMarketRepository
    {
        public SellerMarketRepository(SellerMarketDbContext dbContext)
            :base(dbContext)
        {

        }

        public async Task<SellerMarket> GetByNameWithMCommodities(string name)
        {
            return await _DbContext.Set<SellerMarket>().Include(e => e.SellerMCommodities).FirstOrDefaultAsync(e => e.Name == name);
        }
    }
    public class SellerMCommodityRepository : MCommodityRepository<SellerMCommodity>, ISellerMCommodityRepository
    {
        public SellerMCommodityRepository(SellerMarketDbContext dbContext)
            :base(dbContext)
        {

        }
        public async Task RemoveDistintSMCommodity()
        {
            var SellerMCommodities = await GetToListWithSMCommodity();
            List<SellerSMCommodity> RemoveDatas = new();
            foreach(var value in SellerMCommodities)
            {
                if(value.SellerSMCommodities.Count > 1)
                {
                    var RemoveValue = value.SellerSMCommodities.FirstOrDefault(e => e.Name == null && e.DeliveryCode == null && e.OriginCode == null);
                    if(RemoveValue != null)
                    {
                        RemoveDatas.Add(RemoveValue);
                    }
                }
            }
            foreach(var removeValue in RemoveDatas)
            {
                _DbContext.Remove(removeValue);
            }
            await _DbContext.SaveChangesAsync();
        }
        public async Task<SellerMCommodity> GetByCommodityPageUrl(string commodityPageUrl)
        {
            return await _DbContext.Set<SellerMCommodity>().FirstOrDefaultAsync(e => e.CommodityPageUrl.Equals(commodityPageUrl));
        }

        public async Task<List<SellerMCommodity>> GetToListWithAnalyzedInfos()
        {
            return await _DbContext.Set<SellerMCommodity>().Include(e => e.CommodityAnalyzedInfos).ToListAsync();
        }
        public async Task<List<SellerMCommodity>> GetToListByCenterIdAndWithAnalyzedInfos(string CenterId)
        {
            return await _DbContext.Set<SellerMCommodity>().Where(e => e.CenterId.Equals(CenterId)).Include(e => e.CommodityAnalyzedInfos).ToListAsync();
        }

        public async Task<SellerMCommodity> GetByIdWithAnalyzedInfos(string Id)
        {
            return await _DbContext.Set<SellerMCommodity>().Where(e => e.Id.Equals(Id)).Include(e => e.CommodityAnalyzedInfos).FirstOrDefaultAsync();
        }

        public async Task<List<SellerMCommodity>> GetToListByNoDataofSMCommodity()
        {
            return await _DbContext.Set<SellerMCommodity>().Include(e=>e.SellerSMCommodities).Where(e=>e.SellerSMCommodities.Count() == 0 || e.SellerSMCommodities == null).ToListAsync();
        }
        public async Task<List<SellerMCommodity>> GetToListByNoNameAsync()
        {
            return await _DbContext.Set<SellerMCommodity>().Where(e => e.Name == null || e.Name == "").ToListAsync();
        }

        public async Task<List<SellerMCommodity>> GetToListByNoDataofSMCommodityAndWithCommodityAnalyzedInfos()
        {
            return await _DbContext.Set<SellerMCommodity>().Include(e => e.SellerSMCommodities).Include(e => e.CommodityAnalyzedInfos).Where(e => e.SellerSMCommodities.Count() == 0 || e.SellerSMCommodities == null).ToListAsync();
        }

        public async Task<SellerMCommodity> GetByIdWithSellerMarketAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<SellerMCommodity>> GetToListWithSMCommodity()    
        {
            return await _DbContext.Set<SellerMCommodity>().Include(e => e.SellerSMCommodities).ToListAsync();
        }

        public async Task<List<SellerMCommodity>> GetToListBySellerMarketIdWithSMCommodity(string SellerMarketId)
        {
            return await _DbContext.Set<SellerMCommodity>().Where(e => e.CenterId.Equals(SellerMarketId)).Include(e => e.SellerSMCommodities).ToListAsync();
        }
    }
    public class SellerSMCommodityRepository : SMCommodityRepository<SellerSMCommodity>, ISellerSMCommodityRepository
    {
        public SellerSMCommodityRepository(SellerMarketDbContext dbContext)
            :base(dbContext)
        {

        }
        public async Task<List<SellerSMCommodity>> GetToListAsyncNullDocs()
        {
            return await _DbContext.Set<SellerSMCommodity>().Where(e=>e.Docs == null).ToListAsync();
        }

        public async Task<List<SellerSMCommodity>> GetToListByIsConfiguredImgaeAsync()
        {
            return await _DbContext.Set<SellerSMCommodity>().Where(e => e.IsConfiguringofImage == true).ToListAsync();
        }


        /// <summary>
        /// public string? DeliveryCode { get; set; }
        ///  public string? OriginCode { get; set; }
        ///  public bool IsNaming { get; set; }
        ///  public bool IsConfiguringofImage { get; set; }
        /// </summary>
        /// <returns></returns>
        public async Task<List<SellerSMCommodity>> GetToListByExportingConditions()
        {
            return await _DbContext.Set<SellerSMCommodity>().Where(e=> e.DeliveryCode != null && e.Code != null && e.OriginCode != null).ToListAsync();
        }
        public async Task<List<SellerSMCommodity>> GetToListByExportingConditionsWithSellerMCommodity()
        {
            return await _DbContext.Set<SellerSMCommodity>().Where(e => e.DeliveryCode != null && e.Code != null && e.OriginCode != null).Include(e=>e.SellerMCommodity).ThenInclude(e=>e.SellerMMCommodities).ToListAsync();
        }

        public async Task<List<SellerSMCommodity>> GetToListWithSellerMCommoditiyAsync()
        {
            return await _DbContext.Set<SellerSMCommodity>().Include(e => e.SellerMCommodity).ToListAsync();
        }
        public async Task<List<SellerSMCommodity>> GetToListWithSellerMCommodityFilterByImportantKwds()
        {
            var SellerSMCommodities = await _DbContext.Set<SellerSMCommodity>().Include(e => e.SellerMCommodity).ToListAsync();
            List<SellerSMCommodity> RemoveSMCommodities = new();
            foreach(var value in SellerSMCommodities)
            {
                if(value.SellerMCommodity == null)
                {
                    RemoveSMCommodities.Add(value);
                    continue;
                }
                if(value.SellerMCommodity.ImportantKwds == null || value.SellerMCommodity.ImportantKwds.Count == 0)
                {
                    RemoveSMCommodities.Add(value);
                    continue;
                }
            }
            foreach(var removevalue in RemoveSMCommodities)
            {
                SellerSMCommodities.Remove(removevalue);
            }
            return SellerSMCommodities;
        }

        public async Task<SellerSMCommodity> GetBySellerMCommodityIdAsync(string sellerMCommodityId)
        {
            return await _DbContext.Set<SellerSMCommodity>().FirstOrDefaultAsync(e => e.SellerMCommodityId.Equals(sellerMCommodityId));
        }

        public async Task<List<SellerSMCommodity>> GetToListWithSellerMCommodityAndMarketAsync()
        {
            return await _DbContext.Set<SellerSMCommodity>().Include(e => e.SellerMarket).Include(e => e.SellerMCommodity).ToListAsync();
        }
    }
    public class SellerMMCommodityRepository : MMCommodityRepository<SellerMMCommodity>, ISellerMMCommodityRepository
    {
        public SellerMMCommodityRepository(SellerMarketDbContext dbContext)
            :base(dbContext)
        {

        }

        public async Task<SellerMMCommodity> GetByMCommodityIdAndMarketId(SellerMCommodity sellerMCommodity, SellerOpenMarket sellerOpenMarket)
        {
            return await _DbContext.Set<SellerMMCommodity>().FirstOrDefaultAsync(e => e.SellerMCommodityId.Equals(sellerMCommodity.Id) && e.SellerOpenMarketId.Equals(sellerOpenMarket.Id));
        }

        public async Task<SellerMMCommodity> GetBySellerMMCommodityIdAndNameofMarketAsync(SellerSMCommodity sellerSMCommodity, string OpenMarketId)
        {
            return await _DbContext.Set<SellerMMCommodity>().FirstOrDefaultAsync(e => e.SellerMCommodityId.Equals(sellerSMCommodity.Id) && e.SellerOpenMarketId.Equals(OpenMarketId));
        }
        public async Task<SellerMMCommodity> GetBySMCommodityIdAndMarketId(SellerSMCommodity sellerSMCommodity, SellerOpenMarket sellerOpenMarket)
        {
            return await _DbContext.Set<SellerMMCommodity>().FirstOrDefaultAsync(e => e.SellerSMCommodityId.Equals(sellerSMCommodity.Id) && e.SellerOpenMarketId.Equals(sellerOpenMarket.Id));
        }

        public async Task<List<SellerMMCommodity>> GetToListWithMCommodityFilterByCommodityCode(string code)
        {
            return await _DbContext.Set<SellerMMCommodity>().Include(e => e.SellerMCommodity).Where(e => e.SellerMCommodity.Code.Equals(code)).ToListAsync();
        }
        public async Task<List<SellerMMCommodity>> GetToListWithRelatedDatas()
        {
            return await _DbContext.Set<SellerMMCommodity>().Include(e => e.SellerMCommodity).Include(e => e.SellerSMCommodity).Include(e => e.SellerMarket).Include(e=>e.SellerOpenMarket).ToListAsync();
        }
     
    }
    public class SellerEMCommodityRepository : EMCommodityRepository<SellerEMCommodity>, ISellerEMCommodityRepository
    {
        public SellerEMCommodityRepository(SellerMarketDbContext dbContext)
            :base(dbContext)
        {

        }
    }
}