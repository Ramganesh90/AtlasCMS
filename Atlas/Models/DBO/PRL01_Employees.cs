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
    
    public partial class PRL01_Employees
    {
        public PRL01_Employees()
        {
            this.BID03_MaterialHeader = new HashSet<BID03_MaterialHeader>();
            this.PRL09_TimeEntryMhs = new HashSet<PRL09_TimeEntryMhs>();
            this.Setup98_SalesCommissions = new HashSet<Setup98_SalesCommissions>();
        }
    
        public string EmployeeID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public byte ActiveFlag { get; set; }
        public string DivId { get; set; }
        public decimal EE_OnsiteRate { get; set; }
        public decimal EE_ScaleOnsiteRate { get; set; }
        public decimal EE_OffsiteRate { get; set; }
        public decimal EE_MinWageRate { get; set; }
        public string Type { get; set; }
        public byte SalesFlag { get; set; }
        public byte AdminFlag { get; set; }
        public int RoleID { get; set; }
        public string WinLogon { get; set; }
        public int DepartCodeId { get; set; }
    
        public virtual ICollection<BID03_MaterialHeader> BID03_MaterialHeader { get; set; }
        public virtual Setup01_Divisions Setup01_Divisions { get; set; }
        public virtual ICollection<PRL09_TimeEntryMhs> PRL09_TimeEntryMhs { get; set; }
        public virtual ICollection<Setup98_SalesCommissions> Setup98_SalesCommissions { get; set; }
    }
}
