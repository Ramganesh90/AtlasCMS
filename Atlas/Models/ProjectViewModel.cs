using Atlas.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Atlas.Models
{
    public class ProjectViewModel
    {
        public PRJ04_JobSites JobSites { get; set; }
        public PRJ08_BillingInfo BillingInfoDetails { get; set; }
        public PRJ01_Headers JobDetails { get; set; }
    }
}