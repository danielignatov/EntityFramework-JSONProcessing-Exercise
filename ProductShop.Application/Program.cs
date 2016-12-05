namespace ProductShop.Application
{
    using Data;
    using Data.Data;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    class Program
    {
        static void Main(string[] args)
        {
            ProductShopContext context = new ProductShopContext();

            context.Products.Count();

            /// JSON Exports
            //ExportProductsInPriceRangeWithNoBuyerToJSON(500m, 1000m, context);
            //ExportUsersSoldProductToJSON(context);
            //ExportCategoriesByProductsCountToJSON(context);
            //ExportUsersAndProductsToJSON(context);

            /// XML Exports
            //ExportProductsInPriceRangeWithNoBuyerToXML(500m, 1000m, context);
            //ExportUsersSoldProductToXML(context);
            //ExportCategoriesByProductsCountToXML(context);
            ExportUsersAndProductsToXML(context);
        }

        private static void ExportProductsInPriceRangeWithNoBuyerToXML(decimal minValue, decimal maxValue, ProductShopContext context)
        {
            var productsInPriceRangeWithNoBuyer = context.Products
                .Where(b => b.Buyer == null && b.Price >= minValue && b.Price <= maxValue)
                .OrderBy(p => p.Price)
                .Select(product => new
                {
                    name = product.Name,
                    price = product.Price,
                    seller = (product.Seller.FirstName + " " + product.Seller.LastName).Trim()
                });

            string outputPath = "../../products-in-range.xml";

            var xmlDocument = new XDocument();
            var xmlProducts = new XElement("products");

            foreach (var product in productsInPriceRangeWithNoBuyer)
            {
                var xmlProduct = new XElement("product");
                xmlProduct.Add(new XAttribute("name", product.name));
                xmlProduct.Add(new XAttribute("price", product.price));
                xmlProduct.Add(new XAttribute("seller", product.seller));

                xmlProducts.Add(xmlProduct);
            }

            xmlDocument.Add(xmlProducts);

            xmlDocument.Save(outputPath);
            Console.WriteLine(xmlDocument);
        }

        private static void ExportUsersSoldProductToXML(ProductShopContext context)
        {
            var usersSoldProduct = context.Users
                .Where(sp => sp.SoldProducts.Count > 0)
                .OrderBy(ln => ln.LastName)
                .ThenBy(fn => fn.FirstName)
                .Select(user => new
                {
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    soldProducts = user.SoldProducts.Select(product => new
                    {
                        name = product.Name,
                        price = product.Price,
                        buyerFirstName = product.Buyer.FirstName,
                        buyerLastName = product.Buyer.LastName
                    })
                });

            string outputPath = "../../users-sold-products.xml";

            var xmlDocument = new XDocument();
            var xmlUsers = new XElement("users");

            foreach (var user in usersSoldProduct)
            {
                var xmlUser = new XElement("user");
                xmlUser.Add(new XAttribute("first-name", user.firstName));
                xmlUser.Add(new XAttribute("last-name", user.lastName));

                var xmlSoldProducts = new XElement("sold-products");

                foreach (var product in user.soldProducts)
                {
                    var xmlProduct = new XElement("product");

                    xmlProduct.Add(new XElement("name", product.name));
                    xmlProduct.Add(new XElement("price", product.price));
                    xmlProduct.Add(new XElement("buyer-first-name", product.buyerFirstName));
                    xmlProduct.Add(new XElement("buyer-last-name", product.buyerLastName));

                    xmlSoldProducts.Add(xmlProduct);
                }

                xmlUser.Add(xmlSoldProducts);
                xmlUsers.Add(xmlUser);
            }

            xmlDocument.Add(xmlUsers);

            xmlDocument.Save(outputPath);
            Console.WriteLine(xmlDocument);
        }

        private static void ExportCategoriesByProductsCountToXML(ProductShopContext context)
        {
            var categoriesByProductsCount = context.Categories
                .Where(pc => pc.Products.Count > 0)
                .OrderByDescending(pc => pc.Products.Count)
                .Select(cat => new
                {
                    category = cat.Name,
                    productsCount = cat.Products.Count,
                    averagePrice = cat.Products.Average(p => p.Price),
                    totalRevenue = cat.Products.Sum(s => s.Price)
                });

            string outputPath = "../../categories-by-products.xml";

            var xmlDocument = new XDocument();
            var xmlCategories = new XElement("categories");

            foreach (var category in categoriesByProductsCount)
            {
                var xmlCategory = new XElement("category");
                xmlCategory.Add(new XAttribute("name", category.category));

                xmlCategory.Add(new XElement("products-count", category.productsCount));
                xmlCategory.Add(new XElement("average-price", category.averagePrice));
                xmlCategory.Add(new XElement("total-revenue", category.totalRevenue));

                xmlCategories.Add(xmlCategory);
            }

            xmlDocument.Add(xmlCategories);

            xmlDocument.Save(outputPath);
            Console.WriteLine(xmlDocument);
        }

        private static void ExportUsersAndProductsToXML(ProductShopContext context)
        {
            var users = context.Users;

            var usersAndProducts = users
                .Where(sp => sp.SoldProducts.Count > 0)
                .OrderByDescending(sp => sp.SoldProducts.Count)
                .ThenBy(ln => ln.LastName)
                .Select(user => new
                {
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    age = user.Age,
                    soldProducts = new
                    {
                        count = user.SoldProducts.Count(),
                        products = user.SoldProducts.Select(product => new
                        {
                            name = product.Name,
                            price = product.Price
                        })
                    }
                });

            string outputPath = "../../users-and-products.xml";

            var xmlDocument = new XDocument();
            var xmlUsers = new XElement("users");
            xmlUsers.Add(new XAttribute("count", users.Count()));

            foreach (var user in usersAndProducts)
            {
                var xmlUser = new XElement("user");
                xmlUser.Add(new XAttribute("first-name", user.firstName));
                xmlUser.Add(new XAttribute("last-name", user.lastName));
                xmlUser.Add(new XAttribute("age", user.age));

                var xmlSoldProducts = new XElement("sold-products");
                xmlSoldProducts.Add(new XAttribute("count", user.soldProducts.count));

                foreach (var product in user.soldProducts.products)
                {
                    var xmlProduct = new XElement("product");
                    xmlProduct.Add(new XAttribute("name", product.name));
                    xmlProduct.Add(new XAttribute("price", product.price));

                    xmlSoldProducts.Add(xmlProduct);
                }

                xmlUser.Add(xmlSoldProducts);
                xmlUsers.Add(xmlUser);
            }

            xmlDocument.Add(xmlUsers);

            xmlDocument.Save(outputPath);
            Console.WriteLine(xmlDocument);
        }

        private static void ExportUsersAndProductsToJSON(ProductShopContext context)
        {
            var users = context.Users;

            var usersAndProducts = new
            {
                usersCount = users.Count(),
                users = users
                .Where(sp => sp.SoldProducts.Count > 0)
                .OrderByDescending(sp => sp.SoldProducts.Count)
                .ThenBy(ln => ln.LastName)
                .Select(user => new
                {
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    age = user.Age,
                    soldProducts = new
                    {
                        count = user.SoldProducts.Count(),
                        products = user.SoldProducts.Select(product => new
                        {
                            name = product.Name,
                            price = product.Price
                        })
                    }
                })
            };

            var outputAsJson = JsonConvert.SerializeObject(usersAndProducts, Formatting.Indented);

            File.WriteAllText("../../users-and-products.json", outputAsJson);
            Console.WriteLine(outputAsJson);
        }

        private static void ExportCategoriesByProductsCountToJSON(ProductShopContext context)
        {
            var categoriesByProductsCount = context.Categories
                .Where(pc => pc.Products.Count > 0)
                .OrderByDescending(pc => pc.Products.Count)
                .Select(cat => new
                {
                    category = cat.Name,
                    productsCount = cat.Products.Count,
                    averagePrice = cat.Products.Average(p => p.Price),
                    totalRevenue = cat.Products.Sum(s => s.Price)
                });

            var outputAsJson = JsonConvert.SerializeObject(categoriesByProductsCount, Formatting.Indented);

            File.WriteAllText("../../categories-by-products.json", outputAsJson);
            Console.WriteLine(outputAsJson);
        }

        private static void ExportUsersSoldProductToJSON(ProductShopContext context)
        {
            var usersSoldProduct = context.Users
                .Where(sp => sp.SoldProducts.Count > 0)
                .OrderBy(ln => ln.LastName)
                .ThenBy(fn => fn.FirstName)
                .Select(user => new
                {
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    soldProducts = user.SoldProducts.Select(product => new
                    {
                        name = product.Name,
                        price = product.Price,
                        buyerFirstName = product.Buyer.FirstName,
                        buyerLastName = product.Buyer.LastName
                    })
                });

            var outputAsJson = JsonConvert.SerializeObject(usersSoldProduct, Formatting.Indented);

            File.WriteAllText("../../users-sold-products.json", outputAsJson);
            Console.WriteLine(outputAsJson);
        }

        private static void ExportProductsInPriceRangeWithNoBuyerToJSON(decimal minValue, decimal maxValue, ProductShopContext context)
        {
            var productsInPriceRangeWithNoBuyer = context.Products
                .Where(b => b.Buyer == null && b.Price >= minValue && b.Price <= maxValue)
                .OrderBy(p => p.Price)
                .Select(product => new
                {
                    name = product.Name,
                    price = product.Price,
                    seller = (product.Seller.FirstName + " " + product.Seller.LastName).Trim()
                });

            var outputAsJson = JsonConvert.SerializeObject(productsInPriceRangeWithNoBuyer, Formatting.Indented);

            File.WriteAllText("../../products-in-range.json", outputAsJson);
            Console.WriteLine(outputAsJson);
        }
    }
}