using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Atlas.Models.DBO
{
    public class Complaints
    {
        public int PRJID { get; set; }
        public int JOBID { get; set; }
        [Display(Name = "Job Number")]
        [Required]
        public string JobNumber { get; set; }
        [Display(Name = "Job Name")]
        [Required]
        public string JobName { get; set; }
        [Display(Name = "Job Address")]
        [Required]
        public string JobAddress { get; set; }
        [Display(Name = "Job City")]
        [Required]

        public string JobCity { get; set; }
        [Display(Name = "Job Phone")]
        [Required]
        public string JobPhone { get; set; }
        [Display(Name = "Contact")]
        [Required]
        public string JobContact { get; set; }
        [Display(Name = "Salesman")]
        [Required]
        public string Salesman { get; set; }

        [Display(Name = "Notes")]
        [Required]
        public string JobNotes { get; set; }
    }

    public class PunchList
    {
        [Display(Name ="PunchList Item ID")]
        public string PunchListId { get; set; }
        [Display(Name = "Job Number")]
        public string JobNumber { get; set; }
        [Display(Name = "Job Name")]
        public string JobName { get; set; }
        [Display(Name = "Problem")]
        public string Problem { get; set; }
        [Display(Name = "Date Received")]
        public string DateReceived { get; set; }
        [Display(Name = "Status")]
        public string Status { get; set; }
        public List<PunchStatus> PunchStatuses { get; internal set; }
    }

    public class PunchStatus
    {
        public int CmsStatusId { get; set; }
        public string StatusName { get; set; }
    }
}