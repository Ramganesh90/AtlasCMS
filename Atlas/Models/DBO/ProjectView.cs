using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Atlas.Models.DBO
{
    public class ProjectView
    {
        public int PRJID { get; set; }
        public string ProjectName { get; set; }
        public string JobStatus { get; set; }
        public string JobSiteCity { get; set; }

    }

    public class Projects
    {
        public List<ProjectView> QuotedProjects{ get; set; }
        public List<ProjectView> ActivePendingProjects { get; set; }
        public List<ProjectView> CompletedProjects { get; set; }
    }

   
}