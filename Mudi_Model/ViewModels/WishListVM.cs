using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mudi_Models.ViewModels
{
    public class WishListVM
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public int Stock { get; set; }
        public double UnitPrice { get; set; }
    }
}
