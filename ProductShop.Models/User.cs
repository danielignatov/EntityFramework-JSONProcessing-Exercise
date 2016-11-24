namespace ProductShop.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class User
    {
        #region Fields
        private ICollection<Product> soldProducts;
        private ICollection<Product> boughtProducts;
        private ICollection<User> friends;
        #endregion

        #region Constructor
        public User()
        {
            this.soldProducts = new HashSet<Product>();
            this.boughtProducts = new HashSet<Product>();
            this.friends = new HashSet<User>();
        }

        public User(int id, string firstName, string lastName, int age)
        {
            this.Id = id;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Age = age;
            this.soldProducts = new HashSet<Product>();
            this.boughtProducts = new HashSet<Product>();
            this.friends = new HashSet<User>();
        }
        #endregion

        #region Properties
        [Key]
        public int Id { get; set; }

        public string FirstName { get; set; }

        [Required, MinLength(3)]
        public string LastName { get; set; }

        public int Age { get; set; }

        [NotMapped]
        public ICollection<Product> SoldProducts
        {
            get
            {
                return this.soldProducts;
            }
            set
            {
                this.soldProducts = value;
            }
        }

        [NotMapped]
        public ICollection<Product> BoughtProducts
        {
            get
            {
                return this.boughtProducts;
            }
            set
            {
                this.boughtProducts = value;
            }
        }

        public virtual ICollection<User> Friends
        {
            get
            {
                return this.friends;
            }
            set
            {
                this.friends = value;
            }
        }
        #endregion
    }
}