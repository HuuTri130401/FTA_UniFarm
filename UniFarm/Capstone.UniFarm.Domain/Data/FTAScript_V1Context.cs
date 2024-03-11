using System;
using System.Collections.Generic;
using Capstone.UniFarm.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace Capstone.UniFarm.Domain.Data
{
    public partial class FTAScript_V1Context : IdentityDbContext<Account, IdentityRole<Guid>, Guid>
    {
        public FTAScript_V1Context()
        {
        }

        public FTAScript_V1Context(DbContextOptions<FTAScript_V1Context> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; } = null!;
        public virtual DbSet<AccountRole> AccountRoles { get; set; } = null!;
        public virtual DbSet<Apartment> Apartments { get; set; } = null!;
        public virtual DbSet<ApartmentStation> ApartmentStations { get; set; } = null!;
        public virtual DbSet<Area> Areas { get; set; } = null!;
        public virtual DbSet<Batch> Batches { get; set; } = null!;
        public virtual DbSet<BusinessDay> BusinessDays { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<CollectedHub> CollectedHubs { get; set; } = null!;
        public virtual DbSet<FarmHub> FarmHubs { get; set; } = null!;
        public virtual DbSet<Menu> Menus { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public virtual DbSet<Payment> Payments { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<ProductImage> ProductImages { get; set; } = null!;
        public virtual DbSet<ProductItem> ProductItems { get; set; } = null!;
        public virtual DbSet<ProductItemInMenu> ProductItemInMenus { get; set; } = null!;
        public virtual DbSet<Station> Stations { get; set; } = null!;
        public virtual DbSet<Transaction> Transactions { get; set; } = null!;
        public virtual DbSet<Transfer> Transfers { get; set; } = null!;
        public virtual DbSet<Wallet> Wallets { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(GetConnectionString());
            }
        }

        private string GetConnectionString()
        {
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();
            var strConn = config["ConnectionStrings:DefaultConnection"];

            return strConn;
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Account>().ToTable("Account");
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasIndex(e => e.UserName).IsUnique(false);
            });
            modelBuilder.Entity<IdentityRole<Guid>>().ToTable("Roles");
                /*.HasData(
                    new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = "Admin", NormalizedName = "ADMIN" },
                    new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = "FarmHub", NormalizedName = "FARMHUB" },
                    new IdentityRole<Guid>
                        { Id = Guid.NewGuid(), Name = "CollectedStaff", NormalizedName = "COLLECTEDSTAFF" },
                    new IdentityRole<Guid>
                        { Id = Guid.NewGuid(), Name = "StationStaff", NormalizedName = "STATIONSTAFF" },
                    new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = "Customer", NormalizedName = "CUSTOMER" });*/
            modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
            modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");

            modelBuilder.Entity<Account>(entity => { entity.Property(e => e.Id).HasDefaultValueSql("(newid())"); });

            modelBuilder.Entity<AccountRole>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.Account)
                    .WithOne(p => p.AccountRoles)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__AccountRo__Accou__5165187F");
            });

            modelBuilder.Entity<Apartment>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.Area)
                    .WithMany(p => p.Apartments)
                    .HasForeignKey(d => d.AreaId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__Apartment__AreaI__44FF419A");
            });

            modelBuilder.Entity<ApartmentStation>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.ApartmentStations)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK__Apartment__Accou__4AB81AF0");

                entity.HasOne(d => d.Apartment)
                    .WithMany(p => p.ApartmentStations)
                    .HasForeignKey(d => d.ApartmentId)
                    .HasConstraintName("FK__Apartment__Apart__49C3F6B7");

                entity.HasOne(d => d.Station)
                    .WithMany(p => p.ApartmentStations)
                    .HasForeignKey(d => d.StationId)
                    .HasConstraintName("FK__Apartment__Stati__48CFD27E");
            });

            modelBuilder.Entity<Area>(entity => { entity.Property(e => e.Id).HasDefaultValueSql("(newid())"); });

            modelBuilder.Entity<Batch>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.BusinessDay)
                    .WithMany(p => p.Batches)
                    .HasForeignKey(d => d.BusinessDayId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Batch__BusinessD__10566F31");

                entity.HasOne(d => d.Collected)
                    .WithMany(p => p.Batches)
                    .HasForeignKey(d => d.CollectedId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Batch__Collected__0D7A0286");

                entity.HasOne(d => d.FarmHub)
                    .WithMany(p => p.Batches)
                    .HasForeignKey(d => d.FarmHubId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Batch__FarmHubId__0E6E26BF");

            });

            modelBuilder.Entity<BusinessDay>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<Category>(entity => { entity.Property(e => e.Id).HasDefaultValueSql("(newid())"); });

            modelBuilder.Entity<CollectedHub>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            });

            modelBuilder.Entity<FarmHub>(entity => { entity.Property(e => e.Id).HasDefaultValueSql("(newid())"); });

            modelBuilder.Entity<Menu>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.BusinessDay)
                    .WithMany(p => p.Menus)
                    .HasForeignKey(d => d.BusinessDayId)
                    .HasConstraintName("FK__Menu__BusinessDa__6A30C649");

                entity.HasOne(d => d.FarmHub)
                    .WithMany(p => p.Menus)
                    .HasForeignKey(d => d.FarmHubId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Menu__FarmHubId__693CA210");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.BusinessDay)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.BusinessDayId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Order__BusinessD__04E4BC85");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Order__CustomerI__02084FDA");

                entity.HasOne(d => d.FarmHub)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.FarmHubId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Order__FarmHubId__01142BA1");

                entity.HasOne(d => d.Station)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.StationId)
                    .HasConstraintName("FK__Order__StationId__03F0984C");

                
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__OrderDeta__Order__08B54D69");

                entity.HasOne(d => d.ProductItem)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.ProductItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__OrderDeta__Produ__09A971A2");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.Wallet)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.WalletId)
                    .HasConstraintName("FK__Payment__WalletI__778AC167");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Product__Categor__59063A47");
            });

            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.ProductItem)
                    .WithMany(p => p.ProductImages)
                    .HasForeignKey(d => d.ProductItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductIm__Produ__5CD6CB2B");
            });

            modelBuilder.Entity<ProductItem>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductItems)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductIt__Produ__60A75C0F");
            });

            modelBuilder.Entity<ProductItemInMenu>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.Menu)
                    .WithMany(p => p.ProductItemInMenus)
                    .HasForeignKey(d => d.MenuId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductIt__MenuI__6E01572D");

                entity.HasOne(d => d.ProductItem)
                    .WithMany(p => p.ProductItemInMenus)
                    .HasForeignKey(d => d.ProductItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductIt__Produ__6EF57B66");
            });

            modelBuilder.Entity<Station>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.Area)
                    .WithMany(p => p.Stations)
                    .HasForeignKey(d => d.AreaId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__Station__AreaId__412EB0B6");
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.PaymentDate).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Wallet)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.WalletId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Transacti__Walle__7C4F7684");
                
                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK__Transacti__Order__02FC7413");
                
            });

            modelBuilder.Entity<Transfer>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.Collected)
                    .WithMany(p => p.Transfers)
                    .HasForeignKey(d => d.CollectedId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Transfer__Collec__14270015");
                

                entity.HasOne(d => d.Station)
                    .WithMany(p => p.Transfers)
                    .HasForeignKey(d => d.StationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Transfer__Statio__151B244E");
            });

            modelBuilder.Entity<Wallet>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Wallets)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Wallet__AccountI__73BA3083");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}