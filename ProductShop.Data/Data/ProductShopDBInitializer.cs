namespace ProductShop.Data.Data
{
    using Models;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ProductShopDBInitializer : CreateDatabaseIfNotExists<ProductShopContext>
    {
        private const string UsersPath = "../../../Resources/users.json";
        private const string ProductsPath = "../../../Resources/products.json";
        private const string CategoriesPath = "../../../Resources/categories.json";

        protected override void Seed(ProductShopContext context)
        {
            ImportUsers();
            ImportCategories();
            ImportProductsWithRandomUsersAndCategories();

            base.Seed(context);
        }

        private void ImportUsers()
        {
            ProductShopContext context = new ProductShopContext();

            var json = File.ReadAllText(UsersPath);
            var users = JsonConvert.DeserializeObject<IEnumerable<UserDTO>>(json);

            foreach (var user in users)
            {
                if (user.lastName == null)
                {
                    continue;
                }

                var userContext = new User()
                {
                    FirstName = user.firstName,
                    LastName = user.lastName,
                    Age = user.age
                };

                if (userContext.Age == 0)
                {
                    userContext.Age = null;
                }

                context.Users.Add(userContext);
            }

            context.SaveChanges();
        }

        private void ImportCategories()
        {
            ProductShopContext context = new ProductShopContext();

            var json = File.ReadAllText(CategoriesPath);
            var categories = JsonConvert.DeserializeObject<IEnumerable<CategoryDTO>>(json);

            foreach (var category in categories)
            {
                if (category.name == null)
                {
                    continue;
                }

                var categoryContext = new Category()
                {
                    Name = category.name
                };

                context.Categories.Add(categoryContext);
            }

            context.SaveChanges();
        }

        private void ImportProductsWithRandomUsersAndCategories()
        {
            ProductShopContext context = new ProductShopContext();
            Random rng = new Random();
            int totalNumberOfUsers = context.Users.Count();

            var json = File.ReadAllText(ProductsPath);
            var products = JsonConvert.DeserializeObject<IEnumerable<ProductDTO>>(json);

            foreach (var product in products)
            {
                if ((product.name == null) ||
                    (product.price.ToString() == null))
                {
                    continue;
                }
                
                int randomSellerUserId = rng.Next(0, totalNumberOfUsers);
                int randomBuyerUserId = rng.Next(0, totalNumberOfUsers);

                // Ensure Seller UserId and Buyer UserId are different.
                while (randomSellerUserId == randomBuyerUserId)
                {
                    randomBuyerUserId = rng.Next(0, totalNumberOfUsers);
                }

                var productContext = new Product()
                {
                    Name = product.name,
                    Price = product.price,
                    Seller = context.Users.Find(randomSellerUserId),
                    Buyer = context.Users.Find(randomBuyerUserId)
                };

                if (50 >= rng.Next(0, 100))
                {
                    productContext.Buyer = null;
                }

                Category randomCategory = context.Categories.Find(rng.Next(0, context.Categories.Count()));
                productContext.Categories.Add(randomCategory);

                context.Products.Add(productContext);
            }

            context.SaveChanges();
        }
    }
}