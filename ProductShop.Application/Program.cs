namespace ProductShop.Application
{
    using Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    class Program
    {
        static void Main(string[] args)
        {
            ProductShopContext context = new ProductShopContext();

            context.Products.Count();
        }
    }
}