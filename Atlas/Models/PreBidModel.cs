using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Atlas.Models
{
    public class PreBidModel
    {
        public string FormType { get; set; }
        public string JobNumber { get; set; }
        public string JobName { get; set; }
        public int BINumber { get; set; }
        public string BIName { get; set; }
        public string Company { get; set; }
        public string SalesPerson { get; set; }
        public string FenceType { get; set; }
        public string RateType { get; set; }
        public decimal Material { get; set; }
        public decimal MaterialHandling { get; set; }
        public decimal Concrete { get; set; }
        public decimal MaterialTotal { get; set; }
        public decimal MaterialCOSPercent { get; set; }
        public decimal OnsiteLabor { get; set; }
        public decimal LoadLabor { get; set; }
        public decimal DriveLabor { get; set; }
        public decimal Supervisor { get; set; }
        public decimal LaborReserve { get; set; }
        public decimal LaborTotal { get; set; }
        public decimal LaborCOSPercent { get; set; }
        public decimal OtherCharges { get; set; }
        public decimal OtherChargesMarkup { get; set; }
        public decimal OtherChargesTotal { get; set; }
        public decimal OtherCOSPercent { get; set; }
        public decimal EquipmentCost { get; set; }
        public decimal EquipmentCOSPercent { get; set; }
        public decimal Benefits { get; set; }
        public decimal Retirement { get; set; }
        public decimal PayrollTax { get; set; }
        public decimal WorkersComp { get; set; }
        public decimal IndirectTotal { get; set; }
        public decimal IndirectCOSPercent { get; set; }
        public decimal JobCost { get; set; }
        public decimal JobMarkupPercent { get; set; }
        public decimal JobMarkUpTotal { get; set; }
        public decimal SuggestdSoldFor { get; set; }
        public decimal PreTaxSoldFor { get; set; }
        public string SalesTaxType { get; set; }
        public decimal SalesTaxPercent { get; set; }
        public decimal SalesTaxTotal { get; set; }
        public string CrewHeadCount { get; set; }
        public decimal DaysOnsite { get; set; }
        public decimal CrewLaborBudget { get; set; }
        public decimal RevPerMh { get; set; }
        public string PRJID { get; set; }
    }
}