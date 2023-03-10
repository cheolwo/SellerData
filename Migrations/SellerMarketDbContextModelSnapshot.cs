// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SellerCommon.SellerData.ofDbContext;

#nullable disable

namespace SellerData.Migrations
{
    [DbContext(typeof(SellerMarketDbContext))]
    partial class SellerMarketDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("SellerCommon.SellerData.Model.CommodityAnalyzedInfo", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Category1")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Category2")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Category3")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Category4")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("MonthlyMobileQcCnt")
                        .HasColumnType("int");

                    b.Property<int>("MonthlyPcQcCnt")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("SeletedKeywords")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SellerMCommodityId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("TotalCommodityCount")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("SellerMCommodityId");

                    b.ToTable("CommodityAnalyzedInfo", (string)null);
                });

            modelBuilder.Entity("SellerCommon.SellerData.Model.SellerEMCommodity", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Container")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Docs")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<string>("SellerMCommodityId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("SellerMMCommodityId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("SellerMarketId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("SellerMCommodityId");

                    b.HasIndex("SellerMMCommodityId");

                    b.HasIndex("SellerMarketId");

                    b.ToTable("SellerEMCommodity", (string)null);
                });

            modelBuilder.Entity("SellerCommon.SellerData.Model.SellerMarket", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AsInfo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AsPhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Container")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CountryCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Docs")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("FailLogin")
                        .HasColumnType("int");

                    b.Property<string>("FaxNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FilterCategoryCodes")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageofInfos")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LoginId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)");

                    b.Property<string>("NameofMarket")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("SellerMarket", (string)null);
                });

            modelBuilder.Entity("SellerCommon.SellerData.Model.SellerMCommodity", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Barcode")
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("Category")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CenterId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CommodityOptions")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CommodityPageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Container")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("DeliveryContent")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DetailCommodityInfo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DetailImages")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Docs")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HsCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageofInfos")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImportantKeywordNodes")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImportantKwds")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Keywords")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)");

                    b.Property<string>("RepresentativeImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SellerMarketId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("SellerMarketId");

                    b.ToTable("SellerMCommodity", (string)null);
                });

            modelBuilder.Entity("SellerCommon.SellerData.Model.SellerMMCommodity", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Container")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Docs")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<string>("SellerMCommodityId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("SellerMarketId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("SellerOpenMarketId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("SellerSMCommodityId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("SellerMCommodityId");

                    b.HasIndex("SellerMarketId");

                    b.HasIndex("SellerOpenMarketId");

                    b.HasIndex("SellerSMCommodityId");

                    b.ToTable("SellerMMCommodity", (string)null);
                });

            modelBuilder.Entity("SellerCommon.SellerData.Model.SellerOpenMarket", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Container")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CountryCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Docs")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LoginId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("MarginRate")
                        .HasColumnType("float");

                    b.Property<string>("Name")
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("SellerOpenMarket");
                });

            modelBuilder.Entity("SellerCommon.SellerData.Model.SellerSMCommodity", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("CommonDeliveryFee")
                        .HasColumnType("int");

                    b.Property<string>("Container")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DeliveryCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Docs")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageofInfos")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImportantKwdType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsConfiguringofImage")
                        .HasColumnType("bit");

                    b.Property<bool>("IsNaming")
                        .HasColumnType("bit");

                    b.Property<bool>("IsReimaging")
                        .HasColumnType("bit");

                    b.Property<bool>("IsSmartStoreExporting")
                        .HasColumnType("bit");

                    b.Property<int?>("JejudoDeliveryFee")
                        .HasColumnType("int");

                    b.Property<int?>("MountainDeliveryFee")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("OriginCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<string>("SWCommodityId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SellerMCommodityId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("SellerMarketId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("SellerMCommodityId");

                    b.HasIndex("SellerMarketId");

                    b.ToTable("SellerSMCommodity", (string)null);
                });

            modelBuilder.Entity("SellerCommon.SellerData.Model.CommodityAnalyzedInfo", b =>
                {
                    b.HasOne("SellerCommon.SellerData.Model.SellerMCommodity", "SellerMCommodity")
                        .WithMany("CommodityAnalyzedInfos")
                        .HasForeignKey("SellerMCommodityId");

                    b.Navigation("SellerMCommodity");
                });

            modelBuilder.Entity("SellerCommon.SellerData.Model.SellerEMCommodity", b =>
                {
                    b.HasOne("SellerCommon.SellerData.Model.SellerMCommodity", "SellerMCommodity")
                        .WithMany("SellerEMCommodities")
                        .HasForeignKey("SellerMCommodityId");

                    b.HasOne("SellerCommon.SellerData.Model.SellerMMCommodity", "sellerMMCommodity")
                        .WithMany("SellerEMCommodities")
                        .HasForeignKey("SellerMMCommodityId");

                    b.HasOne("SellerCommon.SellerData.Model.SellerMarket", "SellerMarket")
                        .WithMany("SellerEMCommodities")
                        .HasForeignKey("SellerMarketId");

                    b.Navigation("SellerMCommodity");

                    b.Navigation("SellerMarket");

                    b.Navigation("sellerMMCommodity");
                });

            modelBuilder.Entity("SellerCommon.SellerData.Model.SellerMCommodity", b =>
                {
                    b.HasOne("SellerCommon.SellerData.Model.SellerMarket", "SellerMarket")
                        .WithMany("SellerMCommodities")
                        .HasForeignKey("SellerMarketId");

                    b.Navigation("SellerMarket");
                });

            modelBuilder.Entity("SellerCommon.SellerData.Model.SellerMMCommodity", b =>
                {
                    b.HasOne("SellerCommon.SellerData.Model.SellerMCommodity", "SellerMCommodity")
                        .WithMany("SellerMMCommodities")
                        .HasForeignKey("SellerMCommodityId");

                    b.HasOne("SellerCommon.SellerData.Model.SellerMarket", "SellerMarket")
                        .WithMany("SellerMMCommodities")
                        .HasForeignKey("SellerMarketId");

                    b.HasOne("SellerCommon.SellerData.Model.SellerOpenMarket", "SellerOpenMarket")
                        .WithMany("sellerMMCommodities")
                        .HasForeignKey("SellerOpenMarketId");

                    b.HasOne("SellerCommon.SellerData.Model.SellerSMCommodity", "SellerSMCommodity")
                        .WithMany("SellerMMCommodities")
                        .HasForeignKey("SellerSMCommodityId");

                    b.Navigation("SellerMCommodity");

                    b.Navigation("SellerMarket");

                    b.Navigation("SellerOpenMarket");

                    b.Navigation("SellerSMCommodity");
                });

            modelBuilder.Entity("SellerCommon.SellerData.Model.SellerSMCommodity", b =>
                {
                    b.HasOne("SellerCommon.SellerData.Model.SellerMCommodity", "SellerMCommodity")
                        .WithMany("SellerSMCommodities")
                        .HasForeignKey("SellerMCommodityId");

                    b.HasOne("SellerCommon.SellerData.Model.SellerMarket", "SellerMarket")
                        .WithMany("SellerSMCommodities")
                        .HasForeignKey("SellerMarketId");

                    b.Navigation("SellerMCommodity");

                    b.Navigation("SellerMarket");
                });

            modelBuilder.Entity("SellerCommon.SellerData.Model.SellerMarket", b =>
                {
                    b.Navigation("SellerEMCommodities");

                    b.Navigation("SellerMCommodities");

                    b.Navigation("SellerMMCommodities");

                    b.Navigation("SellerSMCommodities");
                });

            modelBuilder.Entity("SellerCommon.SellerData.Model.SellerMCommodity", b =>
                {
                    b.Navigation("CommodityAnalyzedInfos");

                    b.Navigation("SellerEMCommodities");

                    b.Navigation("SellerMMCommodities");

                    b.Navigation("SellerSMCommodities");
                });

            modelBuilder.Entity("SellerCommon.SellerData.Model.SellerMMCommodity", b =>
                {
                    b.Navigation("SellerEMCommodities");
                });

            modelBuilder.Entity("SellerCommon.SellerData.Model.SellerOpenMarket", b =>
                {
                    b.Navigation("sellerMMCommodities");
                });

            modelBuilder.Entity("SellerCommon.SellerData.Model.SellerSMCommodity", b =>
                {
                    b.Navigation("SellerMMCommodities");
                });
#pragma warning restore 612, 618
        }
    }
}
