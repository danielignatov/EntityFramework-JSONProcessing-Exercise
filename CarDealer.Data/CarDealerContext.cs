namespace CarDealer.Data
{
    using Models;
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class CarDealerContext : DbContext
    {
        // Your context has been configured to use a 'CarDealerContext' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'CarDealer.Data.CarDealerContext' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'CarDealerContext' 
        // connection string in the application configuration file.
        public CarDealerContext()
            : base("name=CarDealerContext")
        {
        }

        public virtual IDbSet<Car> Cars { get; set; }

        public virtual IDbSet<Supplier> Suppliers { get; set; }

        public virtual IDbSet<Part> Parts { get; set; }

        public virtual IDbSet<Sale> Sales { get; set; }

        public virtual IDbSet<Customer> Customers { get; set; }
    }

    //public class MyEntity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}
}