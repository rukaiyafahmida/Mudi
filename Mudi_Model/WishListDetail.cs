using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mudi_Models
{
   public class WishListDetail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int WishListHeaderId { get; set; }
        [ForeignKey("WishListHeaderId")]
        public WishListHeader WishListHeader { get; set; }

        [Required]
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
    }
}
