using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Atlas.Models.DBO
{
    public class BidItems
    {
        [Display(Name = "Include in Rollup")]
        public bool IncludeInRollup { get; set; }
        [Display(Name = "Select To Quote")]
        public bool SelectToQuote { get; set; }
        [Display(Name = "Bid #")]
        public string BidItemId { get; set; }
        [Display(Name = "PreBid")]
        public string PreBid { get; set; }
        [Display(Name = "Bid Name")]
        public string BidItemName { get; set; }
        public string FenceTypeId { get; set; }
        [Display(Name = "Fence Type")]
        public string FenceType { get; set; }
        public string PRJID { get; set; }
        [Display(Name ="Pre Tax Sold For")]
        public string PreTxSoldFor { get; set; }
        [Display(Name ="Date Activated")]
        public string DateActivated { get; set; }
    }
}