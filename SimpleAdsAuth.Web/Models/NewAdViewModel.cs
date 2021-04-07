using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimpleAdsAuth.Data;
namespace SimpleAdsAuth.Web.Models
{
    public class AdViewModel
    {
        public Ad ad { get; set; }
        public bool canDelete { get; set; }
    }
   
}
