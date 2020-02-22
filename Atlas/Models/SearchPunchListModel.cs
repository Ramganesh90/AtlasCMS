using Atlas.Models.DBO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Atlas.Models
{
    public class SearchPunchListModel
    {
        public int PRJID { get; set; }
        public string JobNumber { get; set; }
        public string  JobName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string JobAddress { get; set; }
        public string JobState { get; set; }
        public string JobCity { get; set; }
        public string JobZip { get; set; }
        public string JobPhone { get; set; }
        public string Salesmen { get; set; }
        public List<BidItems> BidItem { get; set; }
    }
}