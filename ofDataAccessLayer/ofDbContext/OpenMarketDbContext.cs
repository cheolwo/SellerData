using BusinessData.ofDataAccessLayer.ofGeneric.ofTypeConfiguration;
using BusinessData.ofDataAccessLayer.ofMarket.ofDbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SellerData.ofDataAccessLayer.ofModel;

namespace SellerData.ofDataAccessLayer.ofDbContext
{
    public class OpenMarketDbContext : DbContext
    {
        public OpenMarketDbContext(DbContextOptions<OpenMarketDbContext> options)
            :base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new OpenMarketConfiguration());
            modelBuilder.ApplyConfiguration(new OpenMarketCommonCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new CommodityOriginCodeConfiguration());
            modelBuilder.ApplyConfiguration(new CommodityDevlieryCodeConfiguration());
        }
    }
    public class OpenMarketConfiguration : MarketConfiguration<OpenMarket>
    {
        public override void Configure(EntityTypeBuilder<OpenMarket> builder)
        {
            base.Configure(builder);
            builder.Ignore(c => c.CenterCards);
            builder.Ignore(c => c.CenterIPAddresses);
            builder.Ignore(c => c.CenterMacAddresses);
            builder.Ignore(c => c.ChangedUsers);
            builder.ToTable("OpenMarket");
        }
    }
    public class OpenMarketCategoryConfiguration : EntityConfiguration<OpenMarketCategory>
    {
        public override void Configure(EntityTypeBuilder<OpenMarketCategory> builder)
        {
            base.Configure(builder);
            builder.Property(e => e.Id).HasMaxLength(100);
            builder.Ignore(c => c.ChangedUsers);
            builder.Ignore(c => c.ImageofInfos);
            builder.Ignore(c => c.CreateTime);
            builder.Ignore(c => c.Docs);
            builder.Ignore(c => c.Container);
            builder.Ignore(c => c.UserId);
            builder.ToTable("OpenMarket");
        }
    }
    public class OpenMarketCommonCategoryConfiguration : EntityConfiguration<OpenMarketCommonCategory>
    {
        public override void Configure(EntityTypeBuilder<OpenMarketCommonCategory> builder)
        {
            base.Configure(builder);
            builder.Property(e => e.Id).HasMaxLength(100);
            builder.Ignore(c => c.ChangedUsers);
            builder.Ignore(c => c.ImageofInfos);
            builder.Ignore(c => c.CreateTime);
            builder.Ignore(c => c.Docs);
            builder.Ignore(c => c.Container);
            builder.Ignore(c => c.UserId);
        }
    }
    public class CommodityOriginCodeConfiguration : EntityConfiguration<CommodityOriginCode>
    {
        public override void Configure(EntityTypeBuilder<CommodityOriginCode> builder)
        {
            base.Configure(builder);
            builder.Property(e => e.Id).HasMaxLength(100);
            builder.Ignore(c => c.ChangedUsers);
            builder.Ignore(c => c.ImageofInfos);
            builder.Ignore(c => c.CreateTime);
            builder.Ignore(c => c.Docs);
            builder.Ignore(c => c.Container);
            builder.Ignore(c => c.UserId);
        }
    }
    public class CommodityDevlieryCodeConfiguration : EntityConfiguration<CommodityDeliveryCode>
    {
        public override void Configure(EntityTypeBuilder<CommodityDeliveryCode> builder)
        {
            base.Configure(builder);
            builder.Property(e => e.Id).HasMaxLength(100);
            builder.Ignore(c => c.ChangedUsers);
            builder.Ignore(c => c.ImageofInfos);
            builder.Ignore(c => c.CreateTime);
            builder.Ignore(c => c.Docs);
            builder.Ignore(c => c.Container);
            builder.Ignore(c => c.UserId);
        }
    }
}
