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
    
    public partial class vPFQLaborCost
    {
        public int BIDID { get; set; }
        public Nullable<decimal> OnsiteMhsBid { get; set; }
        public Nullable<decimal> MhsLoadBid { get; set; }
        public Nullable<decimal> MhsDriveBid { get; set; }
        public Nullable<decimal> MhsTotalBid { get; set; }
        public Nullable<decimal> TripsToSite { get; set; }
        public Nullable<decimal> OnSiteRate { get; set; }
        public Nullable<decimal> OffsiteRate { get; set; }
        public Nullable<decimal> OnsiteDollarsBid { get; set; }
        public Nullable<decimal> LoadDollarsBid { get; set; }
        public Nullable<decimal> DriveDollarsBid { get; set; }
        public Nullable<decimal> TotalDollarsBid { get; set; }
        public Nullable<decimal> BenefitRate { get; set; }
        public Nullable<decimal> BenefitDollarsBid { get; set; }
        public Nullable<decimal> RetirementBenefitRate { get; set; }
        public Nullable<decimal> RetirementDollarsBid { get; set; }
        public Nullable<decimal> PayrollTaxRate { get; set; }
        public Nullable<decimal> PayrollTaxBid { get; set; }
        public Nullable<decimal> WorkCompRate { get; set; }
        public Nullable<decimal> WorkCompBid { get; set; }
    }
}
