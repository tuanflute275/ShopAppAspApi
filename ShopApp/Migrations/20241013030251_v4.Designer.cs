﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ShopApp.Data;

#nullable disable

namespace ShopApp.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20241013030251_v4")]
    partial class v4
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ShopApp.Models.Entities.Blog", b =>
                {
                    b.Property<int>("BlogId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("BlogId");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("BlogId"));

                    b.Property<string>("BlogDescription")
                        .IsRequired()
                        .HasColumnType("ntext")
                        .HasColumnName("BlogDescription");

                    b.Property<string>("BlogImage")
                        .HasColumnType("nvarchar(200)")
                        .HasColumnName("BlogImage");

                    b.Property<string>("BlogSlug")
                        .HasColumnType("nvarchar(200)")
                        .HasColumnName("BlogSlug");

                    b.Property<string>("BlogTitle")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)")
                        .HasColumnName("BlogTitle");

                    b.Property<DateTime?>("CreateDate")
                        .HasColumnType("datetime2")
                        .HasColumnName("CreateDate");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2")
                        .HasColumnName("UpdateDate");

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("UserId");

                    b.HasKey("BlogId");

                    b.HasIndex("UserId");

                    b.ToTable("Blog");
                });

            modelBuilder.Entity("ShopApp.Models.Entities.Cart", b =>
                {
                    b.Property<int>("CartId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("CartId");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CartId"));

                    b.Property<int>("ProductId")
                        .HasColumnType("int")
                        .HasColumnName("ProductId");

                    b.Property<int>("Quantity")
                        .HasColumnType("int")
                        .HasColumnName("Quantity");

                    b.Property<int?>("TotalAmount")
                        .HasColumnType("int")
                        .HasColumnName("TotalAmount");

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("UserId");

                    b.HasKey("CartId");

                    b.HasIndex("ProductId");

                    b.HasIndex("UserId");

                    b.ToTable("Cart");
                });

            modelBuilder.Entity("ShopApp.Models.Entities.Category", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CategoryId"));

                    b.Property<string>("CategoryName")
                        .IsRequired()
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("CategorySlug")
                        .HasColumnType("nvarchar(200)");

                    b.Property<bool>("CategoryStatus")
                        .HasColumnType("bit");

                    b.HasKey("CategoryId");

                    b.ToTable("Category");
                });

            modelBuilder.Entity("ShopApp.Models.Entities.Coupon", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Description")
                        .HasColumnType("ntext");

                    b.Property<int?>("Percent")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Coupon");
                });

            modelBuilder.Entity("ShopApp.Models.Entities.CouponCondition", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Attribute")
                        .IsRequired()
                        .HasColumnType("nvarchar(255)");

                    b.Property<int>("CouponId")
                        .HasColumnType("int");

                    b.Property<double>("DiscountAmount")
                        .HasColumnType("float")
                        .HasColumnName("discountAmount");

                    b.Property<string>("Operator")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("CouponId");

                    b.ToTable("couponCondition");
                });

            modelBuilder.Entity("ShopApp.Models.Entities.Log", b =>
                {
                    b.Property<int>("LogId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("logId");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("LogId"));

                    b.Property<string>("IpAdress")
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("ipAdress");

                    b.Property<string>("Request")
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("request");

                    b.Property<string>("Response")
                        .HasColumnType("ntext")
                        .HasColumnName("response");

                    b.Property<DateTime?>("TimeActionRequest")
                        .HasColumnType("datetime2")
                        .HasColumnName("timeActionRequest");

                    b.Property<DateTime?>("TimeLogin")
                        .HasColumnType("datetime2")
                        .HasColumnName("timeLogin");

                    b.Property<DateTime?>("TimeLogout")
                        .HasColumnType("datetime2")
                        .HasColumnName("timeLogout");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("userName");

                    b.Property<string>("WorkTation")
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("workTation");

                    b.HasKey("LogId");

                    b.ToTable("Log");
                });

            modelBuilder.Entity("ShopApp.Models.Entities.Order", b =>
                {
                    b.Property<int>("OrderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("OrderId");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("OrderId"));

                    b.Property<int?>("CouponId")
                        .HasColumnType("int");

                    b.Property<string>("OrderAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(200)")
                        .HasColumnName("OrderAddress");

                    b.Property<double>("OrderAmount")
                        .HasColumnType("float")
                        .HasColumnName("OrderAmount");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("datetime2")
                        .HasColumnName("OrderDate");

                    b.Property<string>("OrderEmail")
                        .IsRequired()
                        .HasColumnType("nvarchar(200)")
                        .HasColumnName("OrderEmail");

                    b.Property<string>("OrderFullName")
                        .IsRequired()
                        .HasColumnType("nvarchar(200)")
                        .HasColumnName("OrderFullName");

                    b.Property<string>("OrderNote")
                        .IsRequired()
                        .HasColumnType("ntext")
                        .HasColumnName("OrderNote");

                    b.Property<string>("OrderPaymentMethods")
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("OrderPaymentMethods");

                    b.Property<string>("OrderPhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(15)")
                        .HasColumnName("OrderPhoneNumber");

                    b.Property<bool>("OrderStatus")
                        .HasColumnType("bit")
                        .HasColumnName("OrderStatus");

                    b.Property<string>("OrderStatusPayment")
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("OrderStatusPayment");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("OrderId");

                    b.HasIndex("CouponId");

                    b.HasIndex("UserId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("ShopApp.Models.Entities.OrderDetail", b =>
                {
                    b.Property<int>("OrderDetailId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("OrderDetailId");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("OrderDetailId"));

                    b.Property<int>("OrderId")
                        .HasColumnType("int")
                        .HasColumnName("OrderId");

                    b.Property<double>("Price")
                        .HasColumnType("float")
                        .HasColumnName("Price");

                    b.Property<int>("ProductId")
                        .HasColumnType("int")
                        .HasColumnName("ProductId");

                    b.Property<int>("Quantity")
                        .HasColumnType("int")
                        .HasColumnName("Quantity");

                    b.Property<double>("TotalMoney")
                        .HasColumnType("float")
                        .HasColumnName("TotalMoney");

                    b.HasKey("OrderDetailId");

                    b.HasIndex("OrderId");

                    b.HasIndex("ProductId");

                    b.ToTable("OrderDetail");
                });

            modelBuilder.Entity("ShopApp.Models.Entities.Product", b =>
                {
                    b.Property<int>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ProductId");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductId"));

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("ProductDescription")
                        .IsRequired()
                        .HasColumnType("ntext");

                    b.Property<string>("ProductImage")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("ProductName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<double>("ProductPrice")
                        .HasColumnType("float");

                    b.Property<double>("ProductSalePrice")
                        .HasColumnType("float");

                    b.Property<string>("ProductSlug")
                        .IsRequired()
                        .HasColumnType("nvarchar(200)");

                    b.Property<bool>("ProductStatus")
                        .HasColumnType("bit");

                    b.HasKey("ProductId");

                    b.HasIndex("CategoryId");

                    b.ToTable("Product");
                });

            modelBuilder.Entity("ShopApp.Models.Entities.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("RoleDescription")
                        .HasColumnType("ntext");

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("Id");

                    b.ToTable("Role");
                });

            modelBuilder.Entity("ShopApp.Models.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("UserActive")
                        .HasColumnType("bit");

                    b.Property<string>("UserAddress")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("UserAvatar")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<int?>("UserCount")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UserCurrentTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserEmail")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("UserFullName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<bool>("UserGender")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("UserPassword")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("UserPhoneNumber")
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.Property<DateTime?>("UserUnlockTime")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("User");
                });

            modelBuilder.Entity("ShopApp.Models.Entities.UserRole", b =>
                {
                    b.Property<int>("UserId")
                        .HasMaxLength(255)
                        .HasColumnType("int");

                    b.Property<int>("RoleId")
                        .HasMaxLength(255)
                        .HasColumnType("int");

                    b.HasKey("UserId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRole");
                });

            modelBuilder.Entity("ShopApp.Models.Entities.Blog", b =>
                {
                    b.HasOne("ShopApp.Models.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("ShopApp.Models.Entities.Cart", b =>
                {
                    b.HasOne("ShopApp.Models.Entities.Product", "Product")
                        .WithMany("Carts")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("ShopApp.Models.Entities.User", "User")
                        .WithMany("Carts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Product");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ShopApp.Models.Entities.CouponCondition", b =>
                {
                    b.HasOne("ShopApp.Models.Entities.Coupon", "Coupon")
                        .WithMany("CouponConditions")
                        .HasForeignKey("CouponId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Coupon");
                });

            modelBuilder.Entity("ShopApp.Models.Entities.Order", b =>
                {
                    b.HasOne("ShopApp.Models.Entities.Coupon", "Coupon")
                        .WithMany()
                        .HasForeignKey("CouponId");

                    b.HasOne("ShopApp.Models.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Coupon");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ShopApp.Models.Entities.OrderDetail", b =>
                {
                    b.HasOne("ShopApp.Models.Entities.Order", "Order")
                        .WithMany("OrderDetails")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("ShopApp.Models.Entities.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Order");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("ShopApp.Models.Entities.Product", b =>
                {
                    b.HasOne("ShopApp.Models.Entities.Category", "Category")
                        .WithMany("Products")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("ShopApp.Models.Entities.UserRole", b =>
                {
                    b.HasOne("ShopApp.Models.Entities.Role", "Role")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("ShopApp.Models.Entities.User", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ShopApp.Models.Entities.Category", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("ShopApp.Models.Entities.Coupon", b =>
                {
                    b.Navigation("CouponConditions");
                });

            modelBuilder.Entity("ShopApp.Models.Entities.Order", b =>
                {
                    b.Navigation("OrderDetails");
                });

            modelBuilder.Entity("ShopApp.Models.Entities.Product", b =>
                {
                    b.Navigation("Carts");
                });

            modelBuilder.Entity("ShopApp.Models.Entities.Role", b =>
                {
                    b.Navigation("UserRoles");
                });

            modelBuilder.Entity("ShopApp.Models.Entities.User", b =>
                {
                    b.Navigation("Carts");

                    b.Navigation("UserRoles");
                });
#pragma warning restore 612, 618
        }
    }
}
