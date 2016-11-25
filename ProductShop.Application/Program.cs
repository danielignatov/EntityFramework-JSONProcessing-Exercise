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

            ExportProductsInPriceRangeWithNoBuyerToJSON(500m, 1000m, context);
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