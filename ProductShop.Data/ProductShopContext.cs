namespace ProductShop.Data
{
    using Models;
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class ProductShopContext : DbContext
    {
        public ProductShopContext()
            : base("name=ProductShopContext")
        {
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

            //modelBuilder.Entity<Product>()
            //    .HasKey<User>(s => s.Seller)
            //    .Map(cs => cs.)

            base.OnModelCreating(modelBuilder);
        }
    }
}