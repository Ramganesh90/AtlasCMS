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
    
    public partial class Setup25_FenceFamily
    {
        public Setup25_FenceFamily()
        {
            this.Setup23_FieldLaborTypes = new HashSet<Setup23_FieldLaborTypes>();
            this.Setup25_FenceTypes = new HashSet<Setup25_FenceTypes>();
        }
    
        public int FenceFamilyId { get; set; }
        public string Description { get; set; }
    
        public virtual ICollection<Setup23_FieldLaborTypes> Setup23_FieldLaborTypes { get; set; }
        public virtual ICollection<Setup25_FenceTypes> Setup25_FenceTypes { get; set; }
    }
}
