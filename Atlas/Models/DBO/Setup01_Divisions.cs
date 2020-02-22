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
    
    public partial class Setup01_Divisions
    {
        public Setup01_Divisions()
        {
            this.PRJ01_Headers = new HashSet<PRJ01_Headers>();
            this.PRL01_Employees = new HashSet<PRL01_Employees>();
            this.Setup00_EarningCodes = new HashSet<Setup00_EarningCodes>();
            this.Setup02_MhRates = new HashSet<Setup02_MhRates>();
            this.Setup25_FenceTypes = new HashSet<Setup25_FenceTypes>();
        }
    
        public string DivID { get; set; }
        public string Division { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string OfficePhone { get; set; }
        public string OfficeFax { get; set; }
        public string ContractorLicense { get; set; }
    
        public virtual ICollection<PRJ01_Headers> PRJ01_Headers { get; set; }
        public virtual ICollection<PRL01_Employees> PRL01_Employees { get; set; }
        public virtual ICollection<Setup00_EarningCodes> Setup00_EarningCodes { get; set; }
        public virtual ICollection<Setup02_MhRates> Setup02_MhRates { get; set; }
        public virtual ICollection<Setup25_FenceTypes> Setup25_FenceTypes { get; set; }
    }
}
