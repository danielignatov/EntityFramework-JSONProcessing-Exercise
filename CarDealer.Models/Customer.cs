namespace CarDealer.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Customer
    {
        #region Fields
        private ICollection<Sale> sales;
        #endregion

        #region Constructor
        public Customer()
        {
            this.sales = new HashSet<Sale>();
        }
        #endregion

        #region Properties
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        public bool IsYoungDriver { get; set; }

        public virtual ICollection<Sale> Sales
        {
            get
            {
                return this.sales;
            }
            set
            {
                this.sales = value;
            }
        }
        #endregion
    }
}