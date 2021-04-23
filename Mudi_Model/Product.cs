using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Mudi_Models
{
    public class Product
    {
        public Product()
        {
            TempQty = 1;
        }
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Range(1, int.MaxValue)]
        public double Price { get; set; }

        //[Range(1, int.MaxValue)]
        //public double SalePrice { get; set; }

        public string Image { get; set; }

        [Required]
        public string Unit { get; set; }

        [Range(1, int.MaxValue)]
        public int Stock { get; set; }


        [Display(Name = "Category Type")]
        public int CategoryId { get; set; }

        public string ShortDescription { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        [NotMapped]
        [Display(Name = "Quantity")]
        [Range(1, 100, ErrorMessage = "Quantity must be greater than 0")]
        public int TempQty { get; set; }

        public int ProductPopularity { get; set; }

    }
}
