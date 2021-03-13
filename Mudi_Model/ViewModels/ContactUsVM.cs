using Mudi_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mudi_Models.ViewModels
{
    public class ContactUsVM
    {
        public ContactUsVM()
        {
            ApplicationUser = new ApplicationUser();
        }

        public ApplicationUser ApplicationUser { get; set; }

        public string Message { get; set; }
        public string Subject { get; set; }
    }
}
