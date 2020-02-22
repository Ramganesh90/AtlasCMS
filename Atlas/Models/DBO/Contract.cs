using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Atlas.Models.DBO
{
    public class Contract
    {
        public int PRJID { get; set; }
        [Display(Name = "Contract Date")]
        [Required]
        public DateTime ContractDate { get; set; }
        [Display(Name = "Contractors Project Number")]
        [Required]
        public string ContractorsProjectNumber { get; set; }
        [Display(Name = "Retainage Completed Work %")]
        [Required]
        public int RetainageCompletedWork { get; set; }
        [Display(Name = "Retainage Stored Material %")]
        [Required]
        public int RetainageStoredMaterial { get; set; }
    }
}