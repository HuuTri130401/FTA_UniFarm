using System;
using System.Collections.Generic;
using Capstone.UniFarm.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Capstone.UniFarm.Infrastructure.Data
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
        public virtual DbSet<Batch> Batches { get; set; } = null!;
        public virtual DbSet<BusinessDate> BusinessDates { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<CollectedHub> CollectedHubs { get; set; } = null!;
        public virtual DbSet<FarmHub> FarmHubs { get; set; } = null!;
        public virtual DbSet<Menu> Menus { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public virtual DbSet<PriceSystem> PriceSystems { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<ProductImage> ProductImages { get; set; } = null!;
        public virtual DbSet<ProductItem> ProductItems { get; set; } = null!;
        public virtual DbSet<ProductMenu> ProductMenus { get; set; } = null!;
        public virtual DbSet<SessionMenu> SessionMenus { get; set; } = null!;
        public virtual DbSet<Station> Stations { get; set; } = null!;
        public virtual DbSet<Transaction> Transactions { get; set; } = null!;
        public virtual DbSet<Transfer> Transfers { get; set; } = null!;
        public virtual DbSet<Wallet> Wallets { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("server =DANIALTIEN\\DANIALTIEN; database = FTAScript_V1;uid=sa;pwd=12345;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Account>().ToTable("Accounts");
            modelBuilder.Entity<IdentityRole<Guid>>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
            modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");

       

            modelBuilder.Entity<Batch>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.Collected)
                    .WithMany(p => p.Batches)
                    .HasForeignKey(d => d.CollectedId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Batch__Collected__02FC7413");

                entity.HasOne(d => d.FarmHub)
                    .WithMany(p => p.Batches)
                    .HasForeignKey(d => d.FarmHubId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Batch__FarmHubId__03F0984C");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Batches)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Batch__OrderId__04E4BC85");
            });

            modelBuilder.Entity<BusinessDate>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            });

            modelBuilder.Entity<CollectedHub>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.Account)
                    .WithOne(p => p.CollectedHub)
                    .HasForeignKey<CollectedHub>(d => d.AccountId)
                    .HasConstraintName("FK__Collected__Accou__3C69FB99");
            });

            modelBuilder.Entity<FarmHub>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.Account)
                    .WithOne(p => p.FarmHub)
                    .HasForeignKey<FarmHub>(d => d.AccountId)
                    .HasConstraintName("FK__FarmHub__Account__45F365D3");
            });

            modelBuilder.Entity<Menu>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.FarmHub)
                    .WithMany(p => p.Menus)
                    .HasForeignKey(d => d.FarmHubId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Menu__FarmHubId__71D1E811");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Order__CustomerI__6754599E");

                entity.HasOne(d => d.FarmHub)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.FarmHubId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Order__FarmHubId__66603565");

                entity.HasOne(d => d.StationHub)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.StationHubId)
                    .HasConstraintName("FK__Order__StationHu__693CA210");

                entity.HasOne(d => d.Transaction)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.TransactionId)
                    .HasConstraintName("FK__Order__Transacti__68487DD7");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__OrderDeta__Order__6D0D32F4");

                entity.HasOne(d => d.ProductItem)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.ProductItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__OrderDeta__Produ__6E01572D");
            });

            modelBuilder.Entity<PriceSystem>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.PriceSystems)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PriceSyst__Categ__4CA06362");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Product__Categor__5165187F");

                entity.HasOne(d => d.FarmHub)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.FarmHubId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Product__FarmHub__5070F446");
            });

            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductImages)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductIm__Produ__5535A963");
            });

            modelBuilder.Entity<ProductItem>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductItems)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductIt__Produ__59063A47");
            });

            modelBuilder.Entity<ProductMenu>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.Menu)
                    .WithMany(p => p.ProductMenus)
                    .HasForeignKey(d => d.MenuId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductMe__MenuI__75A278F5");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductMenus)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductMe__Produ__76969D2E");
            });

            modelBuilder.Entity<SessionMenu>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.BusinessDate)
                    .WithMany(p => p.SessionMenus)
                    .HasForeignKey(d => d.BusinessDateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__SessionMe__Busin__7F2BE32F");

                entity.HasOne(d => d.Menu)
                    .WithMany(p => p.SessionMenus)
                    .HasForeignKey(d => d.MenuId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__SessionMe__MenuI__7E37BEF6");
            });

            modelBuilder.Entity<Station>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.Account)
                    .WithOne(p => p.Station)
                    .HasForeignKey<Station>(d => d.AccountId)
                    .HasConstraintName("FK__Station__Account__412EB0B6");
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.PaymentDate).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Wallet)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.WalletId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Transacti__Walle__619B8048");
            });

            modelBuilder.Entity<Transfer>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.Collected)
                    .WithMany(p => p.Transfers)
                    .HasForeignKey(d => d.CollectedId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Transfer__Collec__08B54D69");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Transfers)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Transfer__OrderI__0A9D95DB");

                entity.HasOne(d => d.Station)
                    .WithMany(p => p.Transfers)
                    .HasForeignKey(d => d.StationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Transfer__Statio__09A971A2");
            });

            modelBuilder.Entity<Wallet>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Wallets)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Wallet__AccountI__5CD6CB2B");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
