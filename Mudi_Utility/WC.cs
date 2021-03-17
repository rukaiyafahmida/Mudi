using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Mudi_Utility
{
    public static class WC
    {
        public const string ImagePath = @"\images\product\";
        public const string SessionCart = "ShoppingCartSession";
        public const string WishList = "WishListSession";

        public const string AdminRole = "Admin";
        public const string CustomerRole = "Customer";

        public const string EmailAdmin = "170204001@aust.edu";

        public const string CategoryName = "Category";

        public const string StatusPending = "Pending";
        public const string StatusInProcess = "Processing";
        public const string StatusCompleted = "Completed";
        public const string StatusShipped = "Shipped";
        public const string StatusCancelled = "Cancelled";
        public static readonly IEnumerable<string> listStatus = new ReadOnlyCollection<string>(
           new List<string>
           {
                StatusCompleted,StatusCancelled,StatusInProcess,StatusPending,StatusShipped
           });

        public const string Success = "Success";

        public const string Error = "Error";

        public const string Info = "Info";
    }
}
