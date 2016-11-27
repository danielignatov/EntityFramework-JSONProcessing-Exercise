namespace CarDealer.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Supplier
    {
        #region Fields
        private ICollection<Part> parts;
        #endregion

        #region Constructor
        public Supplier()
        {
            this.parts = new HashSet<Part>();
        }
        #endregion

        #region Properties
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public bool IsImporter { get; set; }

        public virtual ICollection<Part> Parts
        {
            get
            {
                return this.parts;
            }
            set
            {
                this.parts = value;
            }
        }
        #endregion
    }
}
