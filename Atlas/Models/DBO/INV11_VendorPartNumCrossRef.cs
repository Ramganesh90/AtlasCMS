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
    
    public partial class INV11_VendorPartNumCrossRef
    {
        public string PK { get; set; }
        public string PartNum { get; set; }
        public string VendorID { get; set; }
        public string VendorPartNumber { get; set; }
        public string VendorUom { get; set; }
        public Nullable<decimal> VendorCost { get; set; }
        public decimal ConversionFactor { get; set; }
        public Nullable<decimal> InventoryCost { get; set; }
        public string InventoryUom { get; set; }
        public Nullable<System.DateTime> LastUpdated { get; set; }
    
        public virtual INV01_Master INV01_Master { get; set; }
    }
}
