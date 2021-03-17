using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mudi_Models.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<OrderHeader> OrderHList { get; set; }
        public IEnumerable<Cart> CartList { get; set; }
        public IEnumerable<WishListDetail> WishListItems { get; set; }
    }
}
