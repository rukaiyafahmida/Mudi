using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mudi_Models.ViewModels
{
    public class WishListVM
    {
        public WishListHeader WishListHeader { get; set; }
        public IEnumerable<WishListDetail> WishListDetail { get; set; }
    }
}
