using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Atlas.Models.DBO
{
    public class AIABilling
    {
        public int PRJID { get; set; }
        [Display(Name ="Commission Expire")]
        [Required]
        public DateTime CommissionExpires { get; set; }
        [Display(Name = "Application Number")]
        [Required]
        public int ApplicationNum { get; set; }
        [Display(Name = "Billing Date")]
        [Required]
        public DateTime BillingDate { get; set; }
        [Display(Name = "Entry Date")]
        [Required]
        public DateTime EntryDate { get; set; }
        [Display(Name = "Notary Date")]
        [Required]
        public DateTime NotaryDate { get; set; }
        [Display(Name = "OriginalContract Sum")]
        [Required]
        public decimal OriginalContractSum { get; set; }
        [Display(Name = "Net Change By CO")]
        [Required]
        public decimal NetChangeByCO { get; set; }
        [Display(Name = "Item Number")]
        [Required]
        public string ItemNo { get; set; }
        [Display(Name = "Qty Of BI")]
        [Required]
        public decimal QtyOfBI { get; set; }
        [Display(Name = "Unit Of Measure")]
        [Required]
        public string UnitOfMeasure { get; set; }
        [Display(Name = "Unit Price")]
        [Required]
        public string UnitPrice { get; set; }
        [Display(Name = "BID Item Number")]
        [Required]
        public string BIDID { get; set; }
        [Display(Name = "Billed Qty")]
        [Required]
        public decimal BilledQty { get; set; }
        [Display(Name = "Accept")]
        [Required]
        public bool Accept { get; set; }
    }
}