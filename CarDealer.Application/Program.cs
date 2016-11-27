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
            string carMake = "Toyota";

            //ImportSuppliers(context);
            //ImportParts(context);
            //ImportCars(context);
            //ImportCustomers(context);
            //ImportRandomSales(context);
            //ExportOrderedCustomersToJSON(context);
            //ExportCarsFromMakeToJSON(carMake, context);
            //ExportLocalSuppliersToJSON(context);
            //ExportCarsAndPartsToJSON(context);
            //ExportCustomersTotalSalesToJSON(context); // TODO
            //ExportSalesWithAppliedDiscountToJSON(context); // TODO
        }

        private static void ExportSalesWithAppliedDiscountToJSON(CarDealerContext context)
        {
            var sales = context.Sales;

            var salesWithAppliedDiscount = sales
                .Select(sale => new
                {
                    
                });

            var json = JsonConvert.SerializeObject(salesWithAppliedDiscount, Formatting.Indented);
            File.WriteAllText($"../../sales-discounts.json", json);
            Console.WriteLine(json);
        }

        private static void ExportCustomersTotalSalesToJSON(CarDealerContext context)
        {
            var customers = context.Customers;

            var customersTotalSales = customers
                .OrderBy(oc => oc.Sales.Count())
                //.ThenByDescending(sm => sm.Sales.Select(c => c.Car.Parts.Select(p => p.Price).Sum()))
                .Select(customer => new
                {
                    fullName = customer.Name,
                    boughtCars = customer.Sales.Count(),
                    // This need to be one number
                    spentMoney = customer.Sales.Select(c => c.Car.Parts.Select(p => p.Price).Sum())
                });

            var json = JsonConvert.SerializeObject(customersTotalSales, Formatting.Indented);
            File.WriteAllText($"../../customers-total-sales.json", json);
            Console.WriteLine(json);
        }

        private static void ExportCarsAndPartsToJSON(CarDealerContext context)
        {
            var carsAndParts = new
            {
                car = context.Cars
                .Select(car => new
                {
                    Make = car.Make,
                    Model = car.Model,
                    TravelledDistance = car.TravelledDistance,
                    parts = car.Parts.Select(part => new
                    {
                        Name = part.Name,
                        Price = part.Price
                    })
                })
            };

        var json = JsonConvert.SerializeObject(carsAndParts, Formatting.Indented);
            File.WriteAllText($"../../cars-and-parts.json", json);
            Console.WriteLine(json);
        }

        private static void ExportLocalSuppliersToJSON(CarDealerContext context)
        {
            var localSuppliers = context.Suppliers
                .Where(i => i.IsImporter == false)
                .Select(supplier => new
                {
                    Id = supplier.Id,
                    Name = supplier.Name,
                    partsCount = supplier.Parts.Count()
                });

            var json = JsonConvert.SerializeObject(localSuppliers, Formatting.Indented);
            File.WriteAllText($"../../local-suppliers.json", json);
            Console.WriteLine(json);
        }

        private static void ExportCarsFromMakeToJSON(string carMake, CarDealerContext context)
        {
            var carsFromMake = context.Cars
                .OrderBy(n => n.Model)
                .ThenByDescending(td => td.TravelledDistance)
                .Where(m => m.Make == carMake)
                .Select(car => new
                {
                    Id = car.Id,
                    Make = car.Make,
                    Model = car.Model,
                    TravelledDistance = car.TravelledDistance
                });

            var json = JsonConvert.SerializeObject(carsFromMake, Formatting.Indented);
            File.WriteAllText($"../../{carMake}-cars.json", json);
            Console.WriteLine(json);
        }

        private static void ExportOrderedCustomersToJSON(CarDealerContext context)
        {
            var orderedCustomers = context.Customers
                .OrderBy(bd => bd.BirthDate)
                .ThenBy(yd => yd.IsYoungDriver == false)
                .Select(user => new
                {
                    Id = user.Id,
                    Name = user.Name,
                    BirthDate = user.BirthDate,
                    IsYoungDriver = user.IsYoungDriver,
                    Sales = user.Sales.Select(sale => new
                    {
                        sale.Id,
                        sale.Car.Make,
                        sale.Customer.Name,
                        sale.Discount
                    })
                });

            var json = JsonConvert.SerializeObject(orderedCustomers, Formatting.Indented);
            File.WriteAllText("../../ordered-customers.json", json);
            Console.WriteLine(json);
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