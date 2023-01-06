using BusinessData.ofDataAccessLayer.ofGeneric.ofTypeConfiguration;
using BusinessData.ofDataAccessLayer.ofMarket.ofDbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using SellerCommon.SellerData.Model;

namespace SellerCommon.SellerData.ofDbContext
{
    public class SellerMarketDbContext : DbContext
    {
        private string _connectionstring;
        public SellerMarketDbContext(DbContextOptions<SellerMarketDbContext> options)
            :base(options)
        {
            
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //if(_connectionstring is null) { _connectionstring = SellerDbConnectionString.MarketDbConnection; }
            //optionsBuilder.UseSqlServer(_connectionstring);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new SellerMarketConfiguration());
            modelBuilder.ApplyConfiguration(new SellerMCommodityConfiguration());
            modelBuilder.ApplyConfiguration(new SellerSMCommodityConfiguration());
            modelBuilder.ApplyConfiguration(new SellerMMCommodityConfiguration());
            modelBuilder.ApplyConfiguration(new SellerEMCommodityConfiguration());
            modelBuilder.ApplyConfiguration(new CommodityAnalyzedInfoConfiguration());
            modelBuilder.ApplyConfiguration(new SellerOpenMarketConfiguration());
        }
    }
    public class SellerMarketConfiguration : MarketConfiguration<SellerMarket>
    {
        public override void Configure(EntityTypeBuilder<SellerMarket> builder)
        {
            base.Configure(builder);
            builder.Ignore(c => c.CenterCards);
            builder.Ignore(c => c.CenterIPAddresses);
            builder.Ignore(c => c.CenterMacAddresses);
            builder.Ignore(c => c.ChangedUsers);
            builder.Property(c => c.FilterCategoryCodes).HasConversion(
                 v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                 v => JsonConvert.DeserializeObject<List<string>>(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                 new ValueComparer<List<string>?>((c1, c2) => c1.SequenceEqual(c2),
                 c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList()));
            builder.ToTable("SellerMarket");
        }
    }
    public class SellerMCommodityConfiguration : MCommodityConfiguration<SellerMCommodity>
    {
        public override void Configure(EntityTypeBuilder<SellerMCommodity> builder)
        {
            base.Configure(builder);
            builder.Property(c => c.DetailImages).HasConversion(
                 v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                 v => JsonConvert.DeserializeObject<List<string>>(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                 new ValueComparer<List<string>>((c1, c2) => c1.SequenceEqual(c2),
                 c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList()));
            builder.Property(c => c.ImportantKwds).HasConversion(
                 v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                 v => JsonConvert.DeserializeObject<List<string>>(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                 new ValueComparer<List<string>>((c1, c2) => c1.SequenceEqual(c2),
                 c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList()));
            builder.Property(c => c.CommodityOptions).HasConversion(
                 v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                 v => JsonConvert.DeserializeObject<List<CommodityOption>>(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                 new ValueComparer<List<CommodityOption>>((c1, c2) => c1.SequenceEqual(c2),
                 c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList()));
            builder.Property(c => c.ImportantKeywordNodes).HasConversion(
                 v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                 v => JsonConvert.DeserializeObject<List<ImportantKeywordNode>>(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                 new ValueComparer<List<ImportantKeywordNode>>((c1, c2) => c1.SequenceEqual(c2),
                 c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList()));
            builder.Property(c => c.DetailCommodityInfo).HasConversion(
                 v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                 v => JsonConvert.DeserializeObject<Dictionary<string, string>>(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            builder.Ignore(c => c.ChangedUsers);
            builder.ToTable("SellerMCommodity");
        }
    }
    public class SellerSMCommodityConfiguration : SMCommodityConfiguration<SellerSMCommodity>
    {
        public override void Configure(EntityTypeBuilder<SellerSMCommodity> builder)
        {
            base.Configure(builder);
            builder.Ignore(c => c.ChangedUsers);
            builder.Ignore(c => c.CreateTime);
            builder.Ignore(c => c.CenterId);
            builder.Ignore(c => c.CommodityId);
            builder.ToTable("SellerSMCommodity");
        }
    }
    public class CommodityAnalyzedInfoConfiguration : EntityConfiguration<CommodityAnalyzedInfo>
    {
        public override void Configure(EntityTypeBuilder<CommodityAnalyzedInfo> builder)
        {
            base.Configure(builder);
            builder.Ignore(c => c.ChangedUsers);
            builder.Ignore(c => c.ImageofInfos);
            builder.Ignore(c => c.CreateTime);
            builder.Ignore(c => c.Code);
            builder.Ignore(c => c.Docs);
            builder.Ignore(c => c.Container);
            builder.ToTable("CommodityAnalyzedInfo");
        }
    }
    public class SellerMMCommodityConfiguration : MMCommodityConfiguration<SellerMMCommodity>
    {
        public override void Configure(EntityTypeBuilder<SellerMMCommodity> builder)
        {
            base.Configure(builder);
            builder.Ignore(c => c.ChangedUsers);
            builder.Ignore(c => c.CreateTime);
            builder.Ignore(c => c.ImageofInfos);
            builder.Ignore(c => c.CenterId);
            builder.Ignore(c => c.CommodityId);
            builder.ToTable("SellerMMCommodity");
        }
    }
    public class SellerOpenMarketConfiguration : CenterConfiguration<SellerOpenMarket>
    {
        public override void Configure(EntityTypeBuilder<SellerOpenMarket> builder)
        {
            base.Configure(builder);
            builder.Ignore(c => c.CenterCards);
            builder.Ignore(c => c.CenterIPAddresses);
            builder.Ignore(c => c.CenterMacAddresses);
            builder.Ignore(c => c.ChangedUsers);
            builder.Ignore(c => c.CreateTime);
            builder.Ignore(c => c.ImageofInfos);
            builder.Ignore(c => c.FailLogin);
            builder.Ignore(c => c.Address);
            builder.Ignore(c => c.FaxNumber);
        }
    }
    public class SellerEMCommodityConfiguration : EMCommodityConfiguration<SellerEMCommodity>
    {
        public override void Configure(EntityTypeBuilder<SellerEMCommodity> builder)
        {
            base.Configure(builder);
            builder.Ignore(c => c.ChangedUsers);
            builder.Ignore(c => c.CreateTime);
            builder.Ignore(c => c.ImageofInfos);
            builder.Ignore(c => c.CenterId);
            builder.Ignore(c => c.CommodityId);
            builder.ToTable("SellerEMCommodity");
        }
    }
}