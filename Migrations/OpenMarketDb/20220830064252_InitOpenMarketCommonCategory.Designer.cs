// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SellerData.ofDataAccessLayer.ofDbContext;

#nullable disable

namespace SellerData.Migrations.OpenMarketDb
{
    [DbContext(typeof(OpenMarketDbContext))]
    [Migration("20220830064252_InitOpenMarketCommonCategory")]
    partial class InitOpenMarketCommonCategory
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("SellerData.ofDataAccessLayer.ofModel.OpenMarket", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("AccessKey")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("CommissionFee")
                        .HasColumnType("real");

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

                    b.Property<string>("SecretKey")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("OpenMarket", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
