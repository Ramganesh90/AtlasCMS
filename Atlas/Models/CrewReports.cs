using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Atlas.Models
{
    public class Timesheet
    {
        public string FormName { get; set; }
        public string Company { get; set; }
        public string MhRate { get; set; }
        public string JobNumber { get; set; }
        public string JobName { get; set; }
        public Int32 BIDID { get; set; }
        public string Location { get; set; }
        public int LaborId { get; set; }
        public string LaborDescription { get; set; }
        public string BidQty { get; set; }
        public string Uom { get; set; }
        public decimal MhsEa { get; set; }
        public decimal MhsExt { get; set; }
        public decimal RateEa { get; set; }
        public decimal PayExt { get; set; }
        public string Sort { get; set; }
    }

    public class Packaging
    {
        public int MRO_Number { get; set; }
        public string CfsFileName { get; set; }
        public DateTime RunDate { get; set; }
        public string JobNumber { get; set; }
        public string ProjectName { get; set; }
        public int BidItemNumber { get; set; }
        public string BIDName { get; set; }
        public decimal Qty { get; set; }
        public string PartNum { get; set; }
        public string Description { get; set; }
    }
}