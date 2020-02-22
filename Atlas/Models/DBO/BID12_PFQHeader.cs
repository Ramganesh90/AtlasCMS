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
    
    public partial class BID12_PFQHeader
    {
        public BID12_PFQHeader()
        {
            this.BID14_PFQLabor = new HashSet<BID14_PFQLabor>();
            this.BID13_PFQMaterial = new HashSet<BID13_PFQMaterial>();
            this.BID15_PFQEquipmentBurden = new HashSet<BID15_PFQEquipmentBurden>();
            this.BID17_PFQConcrete = new HashSet<BID17_PFQConcrete>();
            this.BID18_PFQOtherCosts = new HashSet<BID18_PFQOtherCosts>();
        }
    
        public int PFQHeaderID { get; set; }
        public int BIDID { get; set; }
        public int PFPId { get; set; }
        public decimal PFQQuantity { get; set; }
        public decimal Divider { get; set; }
        public Nullable<decimal> PFQPreTxSoldFor { get; set; }
    
        public virtual BID01_Headers BID01_Headers { get; set; }
        public virtual PFP01_Header PFP01_Header { get; set; }
        public virtual ICollection<BID14_PFQLabor> BID14_PFQLabor { get; set; }
        public virtual ICollection<BID13_PFQMaterial> BID13_PFQMaterial { get; set; }
        public virtual ICollection<BID15_PFQEquipmentBurden> BID15_PFQEquipmentBurden { get; set; }
        public virtual ICollection<BID17_PFQConcrete> BID17_PFQConcrete { get; set; }
        public virtual ICollection<BID18_PFQOtherCosts> BID18_PFQOtherCosts { get; set; }
    }
}
