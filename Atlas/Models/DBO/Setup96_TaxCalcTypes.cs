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
    
    public partial class Setup96_TaxCalcTypes
    {
        public Setup96_TaxCalcTypes()
        {
            this.BID01_Headers = new HashSet<BID01_Headers>();
        }
    
        public int TaxCalcTypeID { get; set; }
        public string TaxCalcType { get; set; }
        public decimal TaxRate { get; set; }
    
        public virtual ICollection<BID01_Headers> BID01_Headers { get; set; }
    }
}
