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
    using System.Xml.Linq;
    using System.Xml.XPath;

    public class ProductShopDBInitializer : CreateDatabaseIfNotExists<ProductShopContext>
    {
        private const string UsersPath = "../../../Resources/users.json";
        private const string ProductsPath = "../../../Resources/products.json";
        private const string CategoriesPath = "../../../Resources/categories.json";

        private const string UsersPathXML = "../../../Resources/users.xml";
        private const string ProductsPathXML = "../../../Resources/products.xml";
        private const string CategoriesPathXML = "../../../Resources/categories.xml";

        protected override void Seed(ProductShopContext context)
        {
            /// JSON Imports
            //ImportUsers();
            //ImportCategories();
            //ImportProductsWithRandomUsersAndCategories();

            /// XML Imports
            ImportUsersFromXML();
            ImportCategoriesFromXML();
            ImportProductsFromXMLWithRandomUsersAndCategories();

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
                
                int randomSellerUserId = rng.Next(1, totalNumberOfUsers);
                int randomBuyerUserId = rng.Next(1, totalNumberOfUsers);

                // Ensure Seller UserId and Buyer UserId are different.
                while (randomSellerUserId == randomBuyerUserId)
                {
                    randomBuyerUserId = rng.Next(1, totalNumberOfUsers);
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
        
        private void ImportUsersFromXML()
        {
            ProductShopContext context = new ProductShopContext();

            var xml = XDocument.Load(UsersPathXML);
            var xmlUsers = xml.XPathSelectElements("users/user");

            foreach (var user in xmlUsers)
            {
                var xmlFirstName = user.Attribute("first-name");
                var xmlLastName = user.Attribute("last-name");
                var xmlAge = user.Attribute("age");

                if ((xmlFirstName == null) ||
                    (xmlLastName == null) ||
                    (xmlAge == null))
                {
                    continue;
                }

                User userEntity = new User()
                {
                    FirstName = xmlFirstName.Value,
                    LastName = xmlLastName.Value,
                    Age = Convert.ToInt32(xmlAge.Value)
                };

                context.Users.Add(userEntity);
            }

            context.SaveChanges();
        }

        private void ImportCategoriesFromXML()
        {
            ProductShopContext context = new ProductShopContext();

            var xml = XDocument.Load(CategoriesPathXML);

            foreach (var category in xml.Root.Elements("category"))
            {
                var xmlCategoryName = category.Element("name");

                if (xmlCategoryName == null)
                {
                    continue;
                }

                Category categoryEntity = new Category()
                {
                    Name = xmlCategoryName.Value
                };

                context.Categories.Add(categoryEntity);
            }

            context.SaveChanges();
        }

        private void ImportProductsFromXMLWithRandomUsersAndCategories()
        {
            ProductShopContext context = new ProductShopContext();
            Random rng = new Random();
            int totalNumberOfUsers = context.Users.Count();

            var xml = XDocument.Load(ProductsPathXML);

            foreach (var product in xml.Root.Elements("product"))
            {
                var xmlProductName = product.Element("name");
                var xmlProductPrice = product.Element("price");

                if ((xmlProductPrice == null) ||
                    (xmlProductName == null))
                {
                    continue;
                }

                int randomSellerUserId = rng.Next(1, totalNumberOfUsers);
                int randomBuyerUserId = rng.Next(1, totalNumberOfUsers);

                // Ensure Seller UserId and Buyer UserId are different.
                while (randomSellerUserId == randomBuyerUserId)
                {
                    randomBuyerUserId = rng.Next(1, totalNumberOfUsers);
                }

                var productContext = new Product()
                {
                    Name = xmlProductName.Value,
                    Price = Convert.ToDecimal(xmlProductPrice.Value),
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