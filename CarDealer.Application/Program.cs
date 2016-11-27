namespace CarDealer.Application
{
    using Data;
    using Models;
    using Models.DTO;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    class Program
    {
        private const string ImportSuppliersPath = @"../../../Resources/suppliers.json";
        private const string ImportPartsPath = @"../../../Resources/parts.json";
        private const string ImportCarsPath = @"../../../Resources/cars.json";
        private const string ImportCustomersPath = @"../../../Resources/customers.json";

        static void Main(string[] args)
        {
            CarDealerContext context = new CarDealerContext();

            //ImportSuppliers(context);
            //ImportParts(context);
            //ImportCars(context);
            //ImportCustomers(context);
            ImportRandomSales(context);
        }

        private static void ImportRandomSales(CarDealerContext context)
        {
            Random rng = new Random();
            int salesCount = rng.Next(20, 50);
            int quantityOfAllCars = context.Cars.Count();
            int quantityOfAllCustomers = context.Customers.Count();
            Double[] discounts = new Double[] { 0, 5, 10, 15, 20, 30, 40, 50 };
            
            for (int i = 0; i < salesCount; i++)
            {
                Car randomCar = context.Cars.Find(rng.Next(1, quantityOfAllCars));
                Customer randomCustomer = context.Customers.Find(rng.Next(1, quantityOfAllCustomers));

                var newSale = new Sale()
                {
                    Car = randomCar,
                    Customer = randomCustomer,
                    Discount = discounts[rng.Next(0, discounts.Length)]
                };

                context.Sales.Add(newSale);
            }

            context.SaveChanges();
        }

        private static void ImportCustomers(CarDealerContext context)
        {
            var json = File.ReadAllText(ImportCustomersPath);
            var customers = JsonConvert.DeserializeObject<IEnumerable<CustomerDTO>>(json);

            foreach (var customer in customers)
            {
                if (customer.Name == null)
                {
                    Console.WriteLine("Invalid Supplier Name");
                    continue;
                }

                var importToDatabase = new Customer()
                {
                    Name = customer.Name,
                    BirthDate = DateTime.Parse(customer.BirthDate),
                    IsYoungDriver = customer.IsYoungDriver
                };

                if (importToDatabase.Name == null)
                {
                    Console.WriteLine("Import To Database Failer because of invalid Name");
                    continue;
                }

                context.Customers.Add(importToDatabase);
            }

            context.SaveChanges();
        }

        private static void ImportCars(CarDealerContext context)
        {
            var json = File.ReadAllText(ImportCarsPath);
            var cars = JsonConvert.DeserializeObject<IEnumerable<CarDTO>>(json);
            Random random = new Random();

            foreach (var car in cars)
            {
                if ((String.IsNullOrWhiteSpace(car.Make)) || car.Model == null)
                {
                    Console.WriteLine("Invalid Data");
                    continue;
                }

                var partsToAdd = new HashSet<Part>();
                int quantityOfAddedParts = random.Next(10, 21);
                int quantityOfAllParts = context.Parts.Count();

                for (int i = 0; i < quantityOfAddedParts; i++)
                {
                    Part newPart = context.Parts.Find(random.Next(1, quantityOfAllParts));

                    while (partsToAdd.Contains(newPart))
                    {
                        newPart = context.Parts.Find(random.Next(1, quantityOfAllParts));
                    }

                    partsToAdd.Add(newPart);
                }

                var newCar = new Car()
                {
                    Make = car.Make,
                    Model = car.Model,
                    Parts = partsToAdd,
                    TravelledDistance = car.TravelledDistance
                };

                context.Cars.Add(newCar);
            }

            context.SaveChanges();
        }

        private static void ImportParts(CarDealerContext context)
        {
            var json = File.ReadAllText(ImportPartsPath);
            var parts = JsonConvert.DeserializeObject<IEnumerable<PartDTO>>(json);
            Random rng = new Random();

            foreach (var part in parts)
            {
                if ((String.IsNullOrWhiteSpace(part.Name)) ||
                    (part.Price < 0) ||
                    (part.Quantity < 0))
                {
                    Console.WriteLine("Invalid Data");
                    continue;
                }

                var newPart = new Part()
                {
                    Name = part.Name,
                    Price = part.Price,
                    Quantity = part.Quantity,
                    Supplier = context.Suppliers.Find(rng.Next(1, context.Suppliers.Count()))
                };

                context.Parts.Add(newPart);
            }

            context.SaveChanges();
        }

        private static void ImportSuppliers(CarDealerContext context)
        {
            var json = File.ReadAllText(ImportSuppliersPath);
            var suppliers = JsonConvert.DeserializeObject<IEnumerable<SupplierDTO>>(json);
            foreach (var supplier in suppliers)
            {
                if (supplier.Name == null)
                {
                    Console.WriteLine("Invalid Supplier Name");
                    continue;
                }

                var importToDatabase = new Supplier() { Name = supplier.Name, IsImporter = supplier.IsImporter };

                if (importToDatabase.Name == null)
                {
                    Console.WriteLine("Import To Database Failer because of invalid Name");
                    continue;
                }

                context.Suppliers.Add(importToDatabase);
            }

            context.SaveChanges();
        }
    }
}