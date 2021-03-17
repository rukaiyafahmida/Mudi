using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mudi_Models
{
    public class WebSiteDetail
    {
        [Key]
        public int Id { get; set; }
        public string AboutUs { get; set; }
        public string ContactUs { get; set; }
    }
}
