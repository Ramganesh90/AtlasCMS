using Atlas.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Atlas.Models
{
    public class CustomerProfileViewModel
    {
        public SAL02_Contacts sal02_Contact { get; set; }
        public List<PRJ01_Headers> prj01_Headers { get; set; }
        public List<Notes> projectNotes { get; set; }

        public List<SAL03_ResAppointments> sal03_ResAppointments { get; set; }
    }

    public class Notes
    {
        public DateTime PRJDateEntered { get; set; }
        public string PRJNotes { get; set; }
    }
}