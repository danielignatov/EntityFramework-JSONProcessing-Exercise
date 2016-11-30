namespace CarDealer.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Sale
    {
        #region Fields
        #endregion

        #region Constructor
        public Sale()
        {

        }
        #endregion

        #region Properties
        [Key]
        public int Id { get; set; }
        
        public Car Car { get; set; }
        
        public Customer Customer { get; set; }

        public Decimal Discount { get; set; }
        #endregion
    }
}
