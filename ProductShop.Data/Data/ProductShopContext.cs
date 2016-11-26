namespace ProductShop.Data.Data
{
    using Data;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class ProductShopContext : DbContext
    {
        public ProductShopContext()
            : base("name=ProductShopContext")
        {
            Database.SetInitializer<ProductShopContext>(new ProductShopDBInitializer());
        }

        public virtual IDbSet<User> Users { get; set; }

        public virtual IDbSet<Product> Products { get; set; }

        public virtual IDbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
             .HasMany<User>(s => s.Friends)
             .WithMany()
             .Map(cs =>
             {
                 cs.MapLeftKey("UserId");
                 cs.MapRightKey("FriendId");
                 cs.ToTable("UserFriends");
             });

            modelBuilder.Entity<User>()
                .HasMany<Product>(sp => sp.SoldProducts)
                .WithOptional(b => b.Buyer)
                .Map(b =>
                {
                    b.MapKey("BuyerId");
                });

            modelBuilder.Entity<User>()
                .HasMany<Product>(bp => bp.BoughtProducts)
                .WithOptional(s => s.Seller)
                .Map(s =>
                {
                    s.MapKey("SellerId");
                });

            base.OnModelCreating(modelBuilder);
        }
    }
}