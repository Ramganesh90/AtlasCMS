using Atlas.DAL;
using Atlas.Models.DBO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Atlas.Models
{
    public class QuoteViewModel
    {
        public List<PRJ05_Quotes> QuoteList{ get; set; }
        public List<BID01_Headers> BidHeadersList { get; set; }
    }
}