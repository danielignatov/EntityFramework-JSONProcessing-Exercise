namespace ProductShop.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Category
    {
        #region Fields
        private ICollection<Product> products;
        #endregion

        #region Constructor
        public Category()
        {
            this.products = new HashSet<Product>();
        }

        public Category(int id, string name)
        {
            this.Id = id;
            this.Name = name;
            this.products = new HashSet<Product>();
        }
        #endregion

        #region Properties
        [Key]
        public int Id { get; set; }

        [Required, MinLength(3), MaxLength(15)]
        public string Name { get; set; }

        public virtual ICollection<Product> Products
        {
            get
            {
                return this.products;
            }
            set
            {
                this.products = value;
            }
        }
        #endregion
    }
}