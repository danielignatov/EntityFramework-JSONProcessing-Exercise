namespace ProductShop.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Product
    {
        #region Fields
        private ICollection<Category> categories;
        #endregion

        #region Constructor
        public Product()
        {
            this.categories = new HashSet<Category>();
        }
        #endregion

        #region Properties
        [Key]
        public int Id { get; set; }

        [Required, MinLength(3)]
        public string Name { get; set; }
        
        [Required]
        public decimal Price { get; set; }
        
        public User Seller { get; set; }
        
        public User Buyer { get; set; }

        public virtual ICollection<Category> Categories
        {
            get
            {
                return this.categories;
            }
            set
            {
                this.categories = value;
            }
        }
        #endregion
    }
}