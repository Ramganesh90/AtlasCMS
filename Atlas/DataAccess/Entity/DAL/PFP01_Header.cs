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
    
    public partial class PFP01_Header
    {
        public PFP01_Header()
        {
            this.BID12_PFQHeader = new HashSet<BID12_PFQHeader>();
            this.PFP02_Material = new HashSet<PFP02_Material>();
            this.PFP03_Labor = new HashSet<PFP03_Labor>();
            this.PFP04_Concrete = new HashSet<PFP04_Concrete>();
        }
    
        public int PFPId { get; set; }
        public string PFPName { get; set; }
        public string PPFShortName { get; set; }
        public int FenceTypeID { get; set; }
        public string MhRateID { get; set; }
        public decimal Divider { get; set; }
        public string PFPUom { get; set; }
        public int PFPCatId { get; set; }
        public int ColorId { get; set; }
        public int FenceHtID { get; set; }
        public int FtRangeID { get; set; }
        public int DigTypeID { get; set; }
        public int PFPComponentTypeId { get; set; }
        public string ContractSource { get; set; }
        public string ContractItemNum { get; set; }
        public Nullable<decimal> PFPSort { get; set; }
    
        public virtual ICollection<BID12_PFQHeader> BID12_PFQHeader { get; set; }
        public virtual INV18_Color INV18_Color { get; set; }
        public virtual PFP10_Catagory PFP10_Catagory { get; set; }
        public virtual PFP11_ComponentType PFP11_ComponentType { get; set; }
        public virtual Setup02_MhRates Setup02_MhRates { get; set; }
        public virtual Setup25_FenceTypes Setup25_FenceTypes { get; set; }
        public virtual Setup26_FenceHts Setup26_FenceHts { get; set; }
        public virtual Setup27_FootageRanges Setup27_FootageRanges { get; set; }
        public virtual Setup30_DigTypes Setup30_DigTypes { get; set; }
        public virtual ICollection<PFP02_Material> PFP02_Material { get; set; }
        public virtual ICollection<PFP03_Labor> PFP03_Labor { get; set; }
        public virtual ICollection<PFP04_Concrete> PFP04_Concrete { get; set; }
    }
}
