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
    using System.Xml.Linq;
    using System.Xml.XPath;

    class Program
    {
        private const string ImportSuppliersPath = @"../../../Resources/suppliers.json";
        private const string ImportPartsPath = @"../../../Resources/parts.json";
        private const string ImportCarsPath = @"../../../Resources/cars.json";
        private const string ImportCustomersPath = @"../../../Resources/customers.json";

        private const string ImportSuppliersPathXML = @"../../../Resources/suppliers.xml";
        private const string ImportPartsPathXML = @"../../../Resources/parts.xml";
        private const string ImportCarsPathXML = @"../../../Resources/cars.xml";
        private const string ImportCustomersPathXML = @"../../../Resources/customers.xml";

        static void Main(string[] args)
        {
            CarDealerContext context = new CarDealerContext();
            string carMake = "Toyota";

            /// JSON Processing
            //ImportSuppliers(context);
            //ImportParts(context);
            //ImportCars(context);
            //ImportCustomers(context);
            //ImportRandomSales(context);
            //ExportOrderedCustomersToJSON(context);
            //ExportCarsFromMakeToJSON(carMake, context);
            //ExportLocalSuppliersToJSON(context);
            //ExportCarsAndPartsToJSON(context);
            //ExportCustomersTotalSalesToJSON(context);
            //ExportSalesWithAppliedDiscountToJSON(context);

            /// XML Processing
            //ImportSuppliersFromXML(context);
            //ImportPartsFromXML(context);
            //ImportCarsFromXML(context);
            //ImportCustomersFromXML(context);
            //ImportRandomSales(context);
            ExportOrderedCustomersToXML(context);
            //ExportCarsFromMakeToXML(carMake, context);
            //ExportLocalSuppliersToXML(context);
            //ExportCarsAndPartsToXML(context);
            //ExportCustomersTotalSalesToXML(context);
            //ExportSalesWithAppliedDiscountToXML(context);
        }

        private static void ExportOrderedCustomersToXML(CarDealerContext context)
        {
            throw new NotImplementedException();
        }

        private static void ImportCustomersFromXML(CarDealerContext context)
        {
            var xml = XDocument.Load(ImportCustomersPathXML);
            int totalNumberOfCustomersAddedInDatabase = 0;
            int totalNumberOfCustomersThatFailedToBeAddedInDatabase = 0;
            Random rng = new Random();

            foreach (var customer in xml.Root.Elements("customer"))
            {
                var customerName = customer.Attribute("name");
                var customerBirthBate = customer.Element("birth-date");
                var customerIsYoungDriver = customer.Element("is-young-driver");

                if (String.IsNullOrWhiteSpace(customerName.Value))
                {
                    totalNumberOfCustomersThatFailedToBeAddedInDatabase++;
                    continue;
                }

                Customer customerEntity = new Customer()
                {
                    Name = customerName.Value,
                    BirthDate = Convert.ToDateTime(customerBirthBate.Value),
                    IsYoungDriver = Convert.ToBoolean(customerIsYoungDriver.Value)
                };

                context.Customers.Add(customerEntity);
                totalNumberOfCustomersAddedInDatabase++;
            }

            context.SaveChanges();
            Console.WriteLine($"{totalNumberOfCustomersAddedInDatabase} new customers added to DB!");

            if (totalNumberOfCustomersThatFailedToBeAddedInDatabase != 0)
            {
                Console.WriteLine($"{totalNumberOfCustomersThatFailedToBeAddedInDatabase} failed (Invalid XML data).");
            }
        }

        private static void ImportCarsFromXML(CarDealerContext context)
        {
            var xml = XDocument.Load(ImportCarsPathXML);
            int totalNumberOfCarsAddedInDatabase = 0;
            int totalNumberOfCarsThatFailedToBeAddedInDatabase = 0;
            Random rng = new Random();

            foreach (var car in xml.Root.Elements("car"))
            {
                var carMake = car.Element("make");
                var carModel = car.Element("model");
                var carTravelledDistance = car.Element("travelled-distance");

                if ((String.IsNullOrWhiteSpace(carMake.Value)) ||
                    (String.IsNullOrWhiteSpace(carModel.Value)))
                {
                    totalNumberOfCarsThatFailedToBeAddedInDatabase++;
                    continue;
                }

                var partsToAdd = new HashSet<Part>();
                int quantityOfAddedParts = rng.Next(10, 21);
                int quantityOfAllParts = context.Parts.Count();

                for (int i = 0; i < quantityOfAddedParts; i++)
                {
                    Part newPart = context.Parts.Find(rng.Next(1, quantityOfAllParts));

                    while (partsToAdd.Contains(newPart))
                    {
                        newPart = context.Parts.Find(rng.Next(1, quantityOfAllParts));
                    }

                    partsToAdd.Add(newPart);
                }

                Car carEntity = new Car()
                {
                    Make = carMake.Value,
                    Model = carModel.Value,
                    TravelledDistance = Convert.ToInt64(carTravelledDistance.Value),
                    Parts = partsToAdd
                };

                context.Cars.Add(carEntity);
                totalNumberOfCarsAddedInDatabase++;
            }

            context.SaveChanges();
            Console.WriteLine($"{totalNumberOfCarsAddedInDatabase} new cars added to DB!");

            if (totalNumberOfCarsThatFailedToBeAddedInDatabase != 0)
            {
                Console.WriteLine($"{totalNumberOfCarsThatFailedToBeAddedInDatabase} failed (Invalid XML data).");
            }
        }

        private static void ImportPartsFromXML(CarDealerContext context)
        {
            var xml = XDocument.Load(ImportPartsPathXML);
            var parts = xml.XPathSelectElements("parts/part");
            int totalNumberOfPartsAddedInDatabase = 0;
            int totalNumberOfPartsThatFailedToBeAddedInDatabase = 0;
            Random rng = new Random();

            foreach (var part in parts)
            {
                var partName = part.Attribute("name");
                var partPrice = part.Attribute("price");
                var partQuantity = part.Attribute("quantity");

                if (String.IsNullOrWhiteSpace(partName.Value))
                {
                    totalNumberOfPartsThatFailedToBeAddedInDatabase++;
                    continue;
                }

                Part partEntity = new Part()
                {
                    Name = partName.Value,
                    Price = Convert.ToDecimal(partPrice.Value),
                    Quantity = Convert.ToInt32(partQuantity.Value),
                    Supplier = context.Suppliers.Find(rng.Next(1, context.Suppliers.Count()))
                };

                context.Parts.Add(partEntity);
                totalNumberOfPartsAddedInDatabase++;
            }

            context.SaveChanges();
            Console.WriteLine($"{totalNumberOfPartsAddedInDatabase} new parts added to DB!");

            if (totalNumberOfPartsThatFailedToBeAddedInDatabase != 0)
            {
                Console.WriteLine($"{totalNumberOfPartsThatFailedToBeAddedInDatabase} failed (Invalid XML data).");
            }
        }

        private static void ImportSuppliersFromXML(CarDealerContext context)
        {
            var xml = XDocument.Load(ImportSuppliersPathXML);
            var suppliers = xml.XPathSelectElements("suppliers/supplier");
            int totalNumberOfSuppliersAddedInDatabase = 0;
            int totalNumberOfSuppliersThatFailedToBeAddedInDatabase = 0;

            foreach (var supplier in suppliers)
            {
                var supplierName = supplier.Attribute("name");
                var isImporter = supplier.Attribute("is-importer");

                if ((String.IsNullOrWhiteSpace(supplierName.Value)) ||
                    (String.IsNullOrWhiteSpace(isImporter.Value)) ||
                    (isImporter.Value != "true" && isImporter.Value != "false"))
                {
                    totalNumberOfSuppliersThatFailedToBeAddedInDatabase++;
                    continue;
                }

                Supplier supplierEntity = new Supplier()
                {
                    Name = supplierName.Value,
                    IsImporter = Convert.ToBoolean(isImporter.Value)
                };

                context.Suppliers.Add(supplierEntity);
                totalNumberOfSuppliersAddedInDatabase++;
            }

            context.SaveChanges();
            Console.WriteLine($"{totalNumberOfSuppliersAddedInDatabase} new suppliers added to DB!");

            if (totalNumberOfSuppliersThatFailedToBeAddedInDatabase != 0)
            {
                Console.WriteLine($"{totalNumberOfSuppliersThatFailedToBeAddedInDatabase} failed (Invalid XML data).");
            }
        }

        private static void ExportSalesWithAppliedDiscountToJSON(CarDealerContext context)
        {
            var salesWithAppliedDiscount = context.Sales
                .Select(sale => new
                {
                    car = new
                    {
                        sale.Car.Make,
                        sale.Car.Model,
                        sale.Car.TravelledDistance
                    },
                    customerName = sale.Customer.Name,
                    Discount = sale.Discount,
                    price = sale.Car.Parts.Sum(p => p.Price),
                    priceWithDiscount = sale.Car.Parts.Sum(p => p.Price) - (sale.Car.Parts.Sum(p => p.Price) * sale.Discount)
                });

            var json = JsonConvert.SerializeObject(salesWithAppliedDiscount, Formatting.Indented);
            File.WriteAllText($"../../sales-discounts.json", json);
            Console.WriteLine(json);
        }

        private static void ExportCustomersTotalSalesToJSON(CarDealerContext context)
        {
            var customersTotalSales = context.Customers
                .Where(s => s.Sales.Any() == true)
                .Select(c => new
                {
                    fullName = c.Name,
                    boughtCars = c.Sales.Count,
                    spentMoney = c.Sales.Sum(p => p.Car.Parts.Sum(s => s.Price))
                })
                .OrderByDescending(c => c.spentMoney)
                .ThenByDescending(sm => sm.spentMoney)
                .ToList();

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
            Decimal[] discounts = new Decimal[] { 0, 0.05m, 0.10m, 0.15m, 0.20m, 0.30m, 0.40m, 0.50m };

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