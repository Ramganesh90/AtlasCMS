using Atlas.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Atlas.Models.DBO
{
    public class BILabour
    {
        public List<LabourDetails> LabourDetails { get; set; }
        public List<LabourDetails> AddlnLabourDetails { get; set; }
        public List<ReviewLabour> ReviewLabourDetails { get; set; }
        public List<BID07_Concrete> ConcreteDetails { get; set; }
        public List<BID08_OtherCosts> BidOtherCosts { get; set; }
        public List<BID06_CrewProfile> BidCrewProfile { get; set; }
        public List<BID09_EquipmentBurden> BidEquipmentBurden { get; set; }
    }


    public class LabourLabels
    {
        public int FieldLbrTypeID { get; set; }
        public string ElemLabelText { get; set; }
        public int Sort { get; set; }
        public int FenceFamilyID { get; set; }
        public int FenceTypeID { get; set; }
    }

    public class LabourDdls
    {
        public string FieldLbrDtlsID { get; set; }
        public string FieldLbrDesc { get; set; }
        public string FieldLbrUom { get; set; }
        public string FieldLbrMhs { get; set; }
        public string FenceTypeID { get; set; }
        public string FenceType { get; set; }
        public string DivID { get; set; }
        public string SupervisonMarkup { get; set; }
        public string MaterialMarkUp { get; set; }
        public string LaborMarkUp { get; set; }
        public string JobMarkUp { get; set; }
        public string PFPJobMarkup { get; set; }
        public string FenceTypeSort { get; set; }
        public string FenceFamilyId { get; set; }
        public string Description { get; set; }
        public string FieldLbrTypeID { get; set; }
        public string FieldLbrTypeDesc { get; set; }
        public string FieldLaborTypesSort { get; set; }
        public string ModifyByFenceHt { get; set; }
        public string ModifyByFootageRange { get; set; }
        public string ModifyByDigType { get; set; }
        public string PrintToTimeSheet { get; set; }
        public string FieldLaborDetailsSort { get; set; }
        public string Discontinued { get; set; }

    }

    public class LabourDetails
    {
        public string RowNum { get; set; }
        public int BIDLaborItemsID { get; set; }
        public int BIDID { get; set; }
        public string labourLabel { get; set; }
        public int FieldLaborId { get; set; }
        public SelectList labourDdlForUI { get; set; }
        public string UOM { get; set; }
        public int Quantity { get; set; }
    }

    public class ReviewLabour
    {
        public string BIDID { get; set; }
        public string FieldLbrDtlsID { get; set; }
        public string FieldLbrDesc { get; set; }
        public int Qty { get; set; }
        public decimal? MhsEa { get; set; }
        public decimal MhsTotal { get; set; }
        public decimal CrewPayEa { get; set; }
        public decimal CrewPayTotal { get; set; }
        public decimal SubPayEa { get; set; }
        public decimal SubPayTotal { get; set; }
    }
}