using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimpleAdsAuth.Data;
namespace SimpleAdsAuth.Web.Models
{
    public class IndexViewModel
    {
        public List<AdViewModel> ads { get; set; }
        public string message { get; set; }
    }
}
