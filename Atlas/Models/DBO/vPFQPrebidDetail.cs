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
    
    public partial class vPFQPrebidDetail
    {
        public int BIDID { get; set; }
        public int PFQHeaderID { get; set; }
        public decimal MaterialCost { get; set; }
        public decimal MaterialMarkup { get; set; }
        public decimal ConcreteCost { get; set; }
        public Nullable<decimal> MaterialCostTotal { get; set; }
        public Nullable<decimal> OnsiteMhsBid { get; set; }
        public Nullable<decimal> LoadMhsBid { get; set; }
        public Nullable<decimal> DriveMhsBid { get; set; }
        public Nullable<decimal> TotalMhsBid { get; set; }
        public Nullable<decimal> OnsiteLbrCost { get; set; }
        public Nullable<decimal> LoadLbrCost { get; set; }
        public Nullable<decimal> DriveLbrCost { get; set; }
        public Nullable<decimal> CrewLaborCost { get; set; }
        public Nullable<decimal> SupervisorLabor { get; set; }
        public Nullable<decimal> LaborMarkup { get; set; }
        public Nullable<decimal> LaborCostTotal { get; set; }
        public Nullable<decimal> OtherCost { get; set; }
        public Nullable<decimal> OtherCostMarkup { get; set; }
        public Nullable<decimal> OtherCostTotal { get; set; }
        public Nullable<decimal> EquipmentCostTotal { get; set; }
        public Nullable<decimal> BenefitCost { get; set; }
        public Nullable<decimal> RetirementCost { get; set; }
        public Nullable<decimal> PayrollTaxCost { get; set; }
        public Nullable<decimal> WorkCompCost { get; set; }
        public Nullable<decimal> IndirectCostTotal { get; set; }
        public Nullable<decimal> JobCost { get; set; }
        public Nullable<decimal> SuggestedSoldFor { get; set; }
        public Nullable<decimal> PFQPreTaxSoldFor { get; set; }
        public decimal SalTxPer { get; set; }
        public Nullable<decimal> SalesTax { get; set; }
    }
}
