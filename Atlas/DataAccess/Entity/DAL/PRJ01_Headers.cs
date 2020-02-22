//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Atlas.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class PRJ01_Headers
    {
        public PRJ01_Headers()
        {
            this.BID01_Headers = new HashSet<BID01_Headers>();
        }
    
        public int PRJID { get; set; }
        public System.DateTime PRJDateEntered { get; set; }
        public Nullable<System.DateTime> PRJDateActivated { get; set; }
        public string CommID { get; set; }
        public string JobNumber { get; set; }
        public string DivID { get; set; }
        public string MhRateID { get; set; }
        public string JobStatusId { get; set; }
        public string JobName { get; set; }
        public int DriveMultiplier { get; set; }
        public decimal DriveTime1Way { get; set; }
        public Nullable<decimal> DriveTimeDaily { get; set; }
        public decimal InstallCommRate { get; set; }
        public short BillTypeID { get; set; }
        public Nullable<int> JobSiteId { get; set; }
        public Nullable<int> PRJBillingID { get; set; }
        public string PRJNotes { get; set; }
    
        public virtual ICollection<BID01_Headers> BID01_Headers { get; set; }
        public virtual PRJ04_JobSites PRJ04_JobSites { get; set; }
        public virtual PRJ05_JobStatus PRJ05_JobStatus { get; set; }
        public virtual PRJ07_BillingType PRJ07_BillingType { get; set; }
        public virtual PRJ08_BillingInfo PRJ08_BillingInfo { get; set; }
        public virtual Setup01_Divisions Setup01_Divisions { get; set; }
        public virtual Setup02_MhRates Setup02_MhRates { get; set; }
    }
}
