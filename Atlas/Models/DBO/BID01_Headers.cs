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
    using System.ComponentModel.DataAnnotations;
    using System.Web;
    public partial class BID01_Headers
    {
        public BID01_Headers()
        {
            this.BID02_LaborItems = new HashSet<BID02_LaborItems>();
            this.BID03_MaterialHeader = new HashSet<BID03_MaterialHeader>();
            this.BID06_CrewProfile = new HashSet<BID06_CrewProfile>();
            this.BID07_Concrete = new HashSet<BID07_Concrete>();
            this.BID08_OtherCosts = new HashSet<BID08_OtherCosts>();
            this.BID09_EquipmentBurden = new HashSet<BID09_EquipmentBurden>();
            this.BID12_PFQHeader = new HashSet<BID12_PFQHeader>();
            this.BID16_PFQCrewProfile = new HashSet<BID16_PFQCrewProfile>();
            this.PRL07_TimeEntryHeader = new HashSet<PRL07_TimeEntryHeader>();
            CFSFiles = new List<HttpPostedFileBase>();
        }

        public List<HttpPostedFileBase> CFSFiles { get; set; }
        
        public int BIDID { get; set; }
        public int PRJID { get; set; }

        [Display(Name ="Bid Item Name")]
        [Required]
        public string BIDName { get; set; }

        [Display(Name = "Quantity Of Bid Item")]
        [Required]
        public int QtyOfBI { get; set; }

        [Display(Name = "UOM")]
        [Required]
        public string UnitOfMeasure { get; set; }

        [Display(Name = "BI Type")]
        [Required]
        public string BIType { get; set; }

        [Display(Name = "Related BI")]
        public string RelatedBI { get; set; }
        
        public System.DateTime DateEntered { get; set; }
        public Nullable<System.DateTime> DateActivated { get; set; }
        public System.DateTime DateEstStart { get; set; }
        public string BIDStatusID { get; set; }
        public byte InRollup { get; set; }
        [Display(Name ="Calculate Drive Time")]
        [Required]
        public byte CalcDriveTimeFlag { get; set; }
        [Display(Name ="Labor Discount %")]
        [Required]
        public decimal LaborDiscount { get; set; }
        [Display(Name ="Labor Hold Back %")]
        [Required]
        public decimal LbrHoldBack { get; set; }
        [Display(Name ="Sales Tax Type")]
        [Required]
        public int TaxCalcTypeID { get; set; }
        public decimal SalTxPer { get; set; }
        public Nullable<decimal> PreTxSoldFor { get; set; }
        public Nullable<decimal> QuickPreTxSoldFor { get; set; }
        public byte EditBidItemFlag { get; set; }
        public Nullable<int> LaborFormID { get; set; }

        [Display(Name ="Select Fence Type")]
        [Required]
        public int FenceTypeID { get; set; }
        [Display(Name = "Select Height")]
        public int FenceHtID { get; set; }
        public decimal PercentOfHtStd { get; set; }
        [Display(Name = "Select Job Length")]
        public int FtRangeID { get; set; }
        public decimal PercentOfFtRangeStd { get; set; }
        [Display(Name = "Select Dig Conditions")]
        public int DigTypeID { get; set; }
        public decimal PercentOfDigStandard { get; set; }
        public decimal SupervisonMarkup { get; set; }
        public decimal MaterialMarkUp { get; set; }
        public decimal LaborMarkUp { get; set; }
        public decimal JobMarkUp { get; set; }
        public decimal PFPJobMarkup { get; set; }
        [Display(Name ="CFS File Name")]
        [Required]
        public string CFSFileName { get; set; }
        [Display(Name = "CFS Material Cost")]
        [Required]
        ////[RegularExpression(@"^[0-9]+(\.[0-9]{0,2})$",ErrorMessage ="Material Cost is not in correct format")]
        //[RegularExpression(@"^[-+]?\d+(\.\d+)?$", ErrorMessage = "Material Cost is not in correct format")]
       // [RegularExpression(@"([0-9]+\.[0-9]*)|([0-9]*\.[0-9]+)|([0-9]+)")]
        public string  MaterialCost { get; set; }

        [Display(Name = "Over -Ride")]
        [Required]
        public bool OverrideCost { get; set; }
        public int BIDMatHeaderID { get; set; }
        public virtual BID10_Status BID10_Status { get; set; }
        public virtual PRJ01_Headers PRJ01_Headers { get; set; }
        public virtual Setup25_FenceTypes Setup25_FenceTypes { get; set; }
        public virtual Setup26_FenceHts Setup26_FenceHts { get; set; }
        public virtual Setup27_FootageRanges Setup27_FootageRanges { get; set; }
        public virtual Setup30_DigTypes Setup30_DigTypes { get; set; }
        public virtual Setup96_TaxCalcTypes Setup96_TaxCalcTypes { get; set; }
        public virtual ICollection<BID02_LaborItems> BID02_LaborItems { get; set; }
        public virtual ICollection<BID03_MaterialHeader> BID03_MaterialHeader { get; set; }
        public virtual ICollection<BID06_CrewProfile> BID06_CrewProfile { get; set; }
        public virtual ICollection<BID07_Concrete> BID07_Concrete { get; set; }
        public virtual ICollection<BID08_OtherCosts> BID08_OtherCosts { get; set; }
        public virtual ICollection<BID09_EquipmentBurden> BID09_EquipmentBurden { get; set; }
        public virtual ICollection<BID12_PFQHeader> BID12_PFQHeader { get; set; }
        public virtual ICollection<BID16_PFQCrewProfile> BID16_PFQCrewProfile { get; set; }
        public virtual ICollection<PRL07_TimeEntryHeader> PRL07_TimeEntryHeader { get; set; }
    }


    public class BID10A_BiTypes
    {
        public int BITypeId { get; set; }
        public string BITypeName { get; set; }
    }

    public class RelatedBIDS
    {
        public int BIDID { get; set; }
    }
}
