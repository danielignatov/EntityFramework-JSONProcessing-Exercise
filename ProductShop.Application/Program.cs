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

    class Program
    {
        static void Main(string[] args)
        {
            ProductShopContext context = new ProductShopContext();

            context.Products.Count();

            //ExportProductsInPriceRangeWithNoBuyerToJSON(500m, 1000m, context);
            //ExportUsersSoldProductToJSON(context);
            //ExportCategoriesByProductsCountToJSON(context);
            //ExportUsersAndProductsToJSON(context);
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