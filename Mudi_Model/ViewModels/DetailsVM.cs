using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mudi_Models.ViewModels
{
    public class DetailsVM
    {
       // public WishListVM WishListVM { get; set; }
        public DetailsVM()
        {
            Product = new Product();
        }

        public Product Product { get; set; }
        public int Stock { get; set; }
        public bool ExistsInCart { get; set; }
        public bool ExistsWish { get; set; }
    }
}