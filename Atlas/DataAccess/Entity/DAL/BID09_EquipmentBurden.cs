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
    
    public partial class BID09_EquipmentBurden
    {
        public int EquipBurdID { get; set; }
        public int BIDID { get; set; }
        public int EquipCostID { get; set; }
        public int Qty { get; set; }
        public decimal EquipRatePerHr { get; set; }
        public Nullable<decimal> EquipCostPerHr { get; set; }
    
        public virtual BID01_Headers BID01_Headers { get; set; }
        public virtual Setup10_EquipmentCosts Setup10_EquipmentCosts { get; set; }
    }
}
