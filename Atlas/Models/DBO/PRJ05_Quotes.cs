using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Atlas.Models.DBO
{
    public class PRJ05_Quotes
    {
        public int QuoteId { get; set; }
        public int BIDID { get; set; }
        public DateTime QuoteDate { get; set; }
        public string BidName { get; set; }
        public int QuoteGroup { get; set; }
        public string ProjectName { get; set; }
        public int PRJID { get; set; }
    }
    public class Quotes
    {
        public int PRJID { get; set; }
        public List<PRJ05_Quotes> QuotesItem { get; set; }
        public string ProjectName { get; set; }
        public int QuoteGroup { get; set; }
    }

       
}