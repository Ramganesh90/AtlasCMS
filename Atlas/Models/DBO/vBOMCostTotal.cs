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
    
    public partial class vBOMCostTotal
    {
        public string PartNum { get; set; }
        public string PartDescription { get; set; }
        public string ProcurementTypeID { get; set; }
        public decimal BomMatCost { get; set; }
        public decimal BomMatCostFuture { get; set; }
        public decimal BomMatCostPrevious { get; set; }
        public decimal BomLbrCost { get; set; }
        public decimal BomLbrCostFuture { get; set; }
        public decimal BomLbrCostPrevious { get; set; }
        public Nullable<decimal> BomTotCost { get; set; }
        public Nullable<decimal> BomTotCostFuture { get; set; }
        public Nullable<decimal> BomTotCostPrevious { get; set; }
        public Nullable<decimal> BomMhs { get; set; }
        public Nullable<decimal> BomWeight { get; set; }
        public string ProdLine { get; set; }
        public string SubClassID { get; set; }
        public decimal Dimention1 { get; set; }
        public decimal Dimention2 { get; set; }
        public decimal Dimention3 { get; set; }
        public string SubClass { get; set; }
        public string Class { get; set; }
        public string CatLoc { get; set; }
    }
}
