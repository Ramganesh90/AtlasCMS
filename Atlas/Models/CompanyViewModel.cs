using Atlas.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Atlas.Models
{
    public class CompanyViewModel
    {
        public SAL01_Company Company { get; set; }
        public List<Setup97_ZipCodes> Zip_State { get; set; }

    }
}