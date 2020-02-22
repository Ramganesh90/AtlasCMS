using Atlas.DAL;
using Atlas.DataAccess.Entity;
using Atlas.Models;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;

namespace Atlas.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public HomeController()
        {
        }

        public ActionResult Index()
        {
            if (Session["CommId"] != null && Session["LoggedInUser"] != null)
            {
                Session["PRJID"] = null;
                return View();
            }
            else
                return RedirectToAction("Login", "Account");
        }

      

        [HttpPost]
        public ActionResult Details(string name, string city)
        {
            var projectsView = HomeDal.getProjects(name, city, (string)Session["CommId"]);
            return Json(projectsView);
        }

        [HttpPost]
        public ActionResult GetPreBidByBid(string BidId)
        {
            var preBidDetails = HomeDal.getPreBidFormByBid(BidId);
            preBidDetails.PRJID = Convert.ToString(Session["CommID"]) + preBidDetails.PRJID;
            return Json(preBidDetails);
        }

        [HttpPost]
        public ActionResult UpdatePrebidForTax(string BidId, string preTaxSold)
        {
            var preBidUpdated = HomeDal.UpdateSalesTax(BidId, preTaxSold);
            return Json(preBidUpdated);
        }

        [Route("PreBid/BidItem/print/id/{BidId}")]
        public FileStreamResult printBidForm(string BidId)
        {
            var preBidDetails = HomeDal.getPreBidFormByBid(BidId);
            // Set up the document and the MS to write it to and create the PDF writer instance
            MemoryStream ms = new MemoryStream();
            Document document = new Document(PageSize.A4);
            PdfWriter writer = PdfWriter.GetInstance(document, ms);

            // Open the PDF document
            document.Open();

            // Set up fonts used in the document
            var LightBlue = new BaseColor(14, 45, 76);
            var grey = new BaseColor(119, 119, 119);
            iTextSharp.text.Font font_heading_1 = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 17, LightBlue);
            iTextSharp.text.Font font_body = FontFactory.GetFont(FontFactory.HELVETICA, 12, LightBlue);
            iTextSharp.text.Font font_body_bold = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, LightBlue);
            iTextSharp.text.Font spanBold = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, grey);
            iTextSharp.text.Font spanNormal = FontFactory.GetFont(FontFactory.HELVETICA, 11, grey);

            preBidDetails.JobNumber = (string.IsNullOrWhiteSpace(preBidDetails.JobNumber) ? BusinessConstants.NA : preBidDetails.JobNumber);
            // Create the heading paragraph with the headig font
            Paragraph header;
            header = new Paragraph("Pre-Bid [" +Convert.ToString(Session["CommID"])+ (preBidDetails.PRJID) + "] Bid Item #" + BidId, font_heading_1);
            iTextSharp.text.pdf.draw.VerticalPositionMark seperator = new iTextSharp.text.pdf.draw.LineSeparator();
            seperator.Offset = -4f;
            header.Add(seperator);
            document.Add(header);

            var HeaderTable = new PdfPTable(4);
            HeaderTable.HorizontalAlignment = 0;
            HeaderTable.SpacingBefore = 5;
            HeaderTable.SpacingAfter = 5;
            HeaderTable.DefaultCell.Border = 0;
            HeaderTable.SetWidths(new int[] { 4, 7, 3, 4 });

            HeaderTable.AddCell(new Phrase("Form Type:", spanBold));
            HeaderTable.AddCell(new Phrase(preBidDetails.FormType, spanNormal));
            HeaderTable.AddCell(new Phrase());
            HeaderTable.AddCell(new Phrase());

            HeaderTable.AddCell(new Phrase("JobNumber:", spanBold));
            HeaderTable.AddCell(new Phrase(preBidDetails.JobNumber, spanNormal));
            HeaderTable.AddCell(new Phrase("Job Name:", spanBold));
            HeaderTable.AddCell(new Phrase(preBidDetails.JobName, spanNormal));

            HeaderTable.AddCell(new Phrase("BI Number:", spanBold));
            HeaderTable.AddCell(new Phrase(Convert.ToString(preBidDetails.BINumber), spanNormal));
            HeaderTable.AddCell(new Phrase("BI Name:", spanBold));
            HeaderTable.AddCell(new Phrase(preBidDetails.BIName, spanNormal));

            HeaderTable.AddCell(new Phrase("Sales Person:", spanBold));
            HeaderTable.AddCell(new Phrase(Convert.ToString(preBidDetails.SalesPerson), spanNormal));
            HeaderTable.AddCell(new Phrase("Company:", spanBold));
            HeaderTable.AddCell(new Phrase(preBidDetails.Company, spanNormal));

            HeaderTable.AddCell(new Phrase("Fence Type:", spanBold));
            HeaderTable.AddCell(new Phrase(Convert.ToString(preBidDetails.FenceType), spanNormal));
            HeaderTable.AddCell(new Phrase("Rate Type:", spanBold));
            HeaderTable.AddCell(new Phrase(preBidDetails.RateType, spanNormal));
            HeaderTable.TotalWidth = 100;

            document.Add(HeaderTable);
            if (preBidDetails.PreTaxSoldFor < preBidDetails.SuggestdSoldFor)
            {
                Paragraph alertData;
                iTextSharp.text.Font redFont = FontFactory.GetFont(FontFactory.HELVETICA, 12, new BaseColor(255, 0, 0));
                string data = "Pre-Tax Sold For " + String.Format("{0:C}", preBidDetails.PreTaxSoldFor) + " is quoted less than Suggested Price " + String.Format("{0:C}", preBidDetails.SuggestdSoldFor);
                alertData = new Paragraph(data, redFont);
                document.Add(alertData);
            }

            var SectionTable = new PdfPTable(4);
            SectionTable.HorizontalAlignment = 0;
            SectionTable.SpacingBefore = 5;
            SectionTable.SpacingAfter = 5;
            SectionTable.DefaultCell.Border = 0;
            SectionTable.SetWidths(new int[] { 7, 4, 4, 4 });
            SectionTable.AddCell(getLabelValue("Material Cost:", font_body));
            SectionTable.AddCell(getDecimalValue(preBidDetails.Material, spanNormal));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Material Handling Charge:", font_body));
            SectionTable.AddCell(getDecimalValue(preBidDetails.MaterialHandling, spanNormal));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Concrete:", font_body));
            SectionTable.AddCell(getDecimalValue(preBidDetails.Concrete, spanNormal));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Material Total:", font_body));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));
            SectionTable.AddCell(getDecimalValue(preBidDetails.MaterialTotal, font_body_bold));
            SectionTable.AddCell(getDecimalValue(preBidDetails.MaterialCOSPercent, spanNormal,true));

            SectionTable.AddCell(getLabelValue("     ", spanNormal));
            SectionTable.AddCell(getLabelValue("     ", spanNormal));
            SectionTable.AddCell(getLabelValue("     ", spanNormal));
            SectionTable.AddCell(getLabelValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Onsite Labor:", font_body));
            SectionTable.AddCell(getDecimalValue(preBidDetails.OnsiteLabor, spanNormal));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Load Labor:", font_body));
            SectionTable.AddCell(getDecimalValue(preBidDetails.LoadLabor, spanNormal));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Drive Labor:", font_body));
            SectionTable.AddCell(getDecimalValue(preBidDetails.DriveLabor, spanNormal));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Supervisor:", font_body));
            SectionTable.AddCell(getDecimalValue(preBidDetails.Supervisor, spanNormal));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Labor Reserve:", font_body));
            SectionTable.AddCell(getDecimalValue(preBidDetails.LaborReserve, spanNormal));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Labor Total:", font_body));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));
            SectionTable.AddCell(getDecimalValue(preBidDetails.LaborTotal, font_body_bold));
            SectionTable.AddCell(getDecimalValue(preBidDetails.LaborCOSPercent, spanNormal, true));

            SectionTable.AddCell(getLabelValue("     ", spanNormal));
            SectionTable.AddCell(getLabelValue("     ", spanNormal));
            SectionTable.AddCell(getLabelValue("     ", spanNormal));
            SectionTable.AddCell(getLabelValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Other Charges:", font_body));
            SectionTable.AddCell(getDecimalValue(preBidDetails.OtherCharges, spanNormal));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Other Charges Markup:", font_body));
            SectionTable.AddCell(getDecimalValue(preBidDetails.OtherChargesMarkup, spanNormal));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Other Charges Total:", font_body));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));
            SectionTable.AddCell(getDecimalValue(preBidDetails.OtherChargesTotal, font_body_bold));
            SectionTable.AddCell(getDecimalValue(preBidDetails.OtherCOSPercent, spanNormal, true));

            SectionTable.AddCell(getLabelValue("     ", spanNormal));
            SectionTable.AddCell(getLabelValue("     ", spanNormal));
            SectionTable.AddCell(getLabelValue("     ", spanNormal));
            SectionTable.AddCell(getLabelValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Equipment Cost:", font_body));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));
            SectionTable.AddCell(getDecimalValue(preBidDetails.EquipmentCost, font_body_bold));
            SectionTable.AddCell(getDecimalValue(preBidDetails.EquipmentCOSPercent, spanNormal, true));

            SectionTable.AddCell(getLabelValue("     ", spanNormal));
            SectionTable.AddCell(getLabelValue("     ", spanNormal));
            SectionTable.AddCell(getLabelValue("     ", spanNormal));
            SectionTable.AddCell(getLabelValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Benefits:", font_body));
            SectionTable.AddCell(getDecimalValue(preBidDetails.Benefits, spanNormal));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Retirement:", font_body));
            SectionTable.AddCell(getDecimalValue(preBidDetails.Retirement, spanNormal));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Payroll Tax:", font_body));
            SectionTable.AddCell(getDecimalValue(preBidDetails.PayrollTax, spanNormal));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Workers Comp:", font_body));
            SectionTable.AddCell(getDecimalValue(preBidDetails.WorkersComp, spanNormal));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Indirect Total:", font_body));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));
            SectionTable.AddCell(getDecimalValue(preBidDetails.IndirectTotal, font_body_bold));
            SectionTable.AddCell(getDecimalValue(preBidDetails.IndirectCOSPercent, spanNormal, true));

            SectionTable.AddCell(getLabelValue("     ", spanNormal));
            SectionTable.AddCell(getLabelValue("     ", spanNormal));
            SectionTable.AddCell(getLabelValue("     ", spanNormal));
            SectionTable.AddCell(getLabelValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Job Cost:", font_body));
            SectionTable.AddCell(getDecimalValue(preBidDetails.JobCost, spanNormal));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Job Markup:", font_body));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));
            SectionTable.AddCell(getDecimalValue(preBidDetails.JobMarkUpTotal, font_body_bold));
            SectionTable.AddCell(getDecimalValue(preBidDetails.JobMarkupPercent, spanNormal, true));

            SectionTable.AddCell(getLabelValue("Suggested Sold For:", font_body));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));
            SectionTable.AddCell(getDecimalValue(preBidDetails.SuggestdSoldFor, font_body_bold));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Pre-Tax Sold For:", font_body));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));
            SectionTable.AddCell(getDecimalValue(preBidDetails.PreTaxSoldFor, font_body_bold));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("     ", spanNormal));
            SectionTable.AddCell(getLabelValue("     ", spanNormal));
            SectionTable.AddCell(getLabelValue("     ", spanNormal));
            SectionTable.AddCell(getLabelValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Sales Tax Type:", font_body));
            SectionTable.AddCell(getDecimalValue(preBidDetails.SalesTaxType, spanNormal));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Sales Tax Percent:", font_body));
            SectionTable.AddCell(getDecimalValue(preBidDetails.SalesTaxPercent, spanNormal,true));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Sales Tax Total:", font_body));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));
            SectionTable.AddCell(getDecimalValue(preBidDetails.SalesTaxTotal, font_body_bold));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));
          
            SectionTable.AddCell(getLabelValue("     ", spanNormal));
            SectionTable.AddCell(getLabelValue("     ", spanNormal));
            SectionTable.AddCell(getLabelValue("     ", spanNormal));
            SectionTable.AddCell(getLabelValue("     ", spanNormal));
            SectionTable.AddCell(getLabelValue("Crew Head Count:", font_body));
            SectionTable.AddCell(getDecimalValue(preBidDetails.CrewHeadCount, spanNormal,hasBorder:false));
            SectionTable.AddCell(getLabelValue("     ", spanNormal));
            SectionTable.AddCell(getLabelValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Days On Site:", font_body));
            SectionTable.AddCell(getDecimalValue(preBidDetails.DaysOnsite, spanNormal, hasBorder: false,plainText:true));
            SectionTable.AddCell(getLabelValue("     ", spanNormal));
            SectionTable.AddCell(getLabelValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Crew Labor Budget:", font_body));
            SectionTable.AddCell(getDecimalValue(preBidDetails.CrewLaborBudget, spanNormal, hasBorder: false));
            SectionTable.AddCell(getLabelValue("     ", spanNormal));
            SectionTable.AddCell(getLabelValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Rev Per Mh:", font_body));
            SectionTable.AddCell(getDecimalValue(preBidDetails.RevPerMh, spanNormal, hasBorder: false));
            SectionTable.AddCell(getLabelValue("     ", spanNormal));
            SectionTable.AddCell(getLabelValue("     ", spanNormal));

            document.Add(SectionTable);
            //document.Add(FooterTable);
            document.Close();
            byte[] file = ms.ToArray();
            MemoryStream output = new MemoryStream();
            output.Write(file, 0, file.Length);
            output.Position = 0;

            HttpContext.Response.AddHeader("content-disposition", "inline; filename=PreBid_BidItem_"+preBidDetails.BINumber+".pdf");
            return File(output, "application/pdf");
        }

        [Route("PreBid/Project/print/id/{PRJID}")]
        public FileStreamResult printProjectForm(string PRJID)
        {
            var preBidDetails = HomeDal.getPreBidFormByProject(PRJID);
            // Set up the document and the MS to write it to and create the PDF writer instance
            MemoryStream ms = new MemoryStream();
            Document document = new Document(PageSize.A4);
            PdfWriter writer = PdfWriter.GetInstance(document, ms);

            // Open the PDF document
            document.Open();

            // Set up fonts used in the document
            var LightBlue = new BaseColor(14, 45, 76);
            var grey = new BaseColor(119, 119, 119);
            iTextSharp.text.Font font_heading_1 = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 17, LightBlue);
            iTextSharp.text.Font font_body = FontFactory.GetFont(FontFactory.HELVETICA, 12, LightBlue);
            iTextSharp.text.Font font_body_bold = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, LightBlue);
            iTextSharp.text.Font spanBold = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, grey);
            iTextSharp.text.Font spanNormal = FontFactory.GetFont(FontFactory.HELVETICA, 11, grey);

            preBidDetails.JobNumber = (string.IsNullOrWhiteSpace(preBidDetails.JobNumber) ? BusinessConstants.NA : preBidDetails.JobNumber);
            // Create the heading paragraph with the headig font
            Paragraph header;
            header = new Paragraph("Pre-Bid [" + Convert.ToString(Session["CommID"]) + (preBidDetails.PRJID) + "] Roll-Up", font_heading_1);
            iTextSharp.text.pdf.draw.VerticalPositionMark seperator = new iTextSharp.text.pdf.draw.LineSeparator();
            seperator.Offset = -4f;
            header.Add(seperator);
            document.Add(header);

            var HeaderTable = new PdfPTable(4);
            HeaderTable.HorizontalAlignment = 0;
            HeaderTable.SpacingBefore = 5;
            HeaderTable.SpacingAfter = 5;
            HeaderTable.DefaultCell.Border = 0;
            HeaderTable.SetWidths(new int[] { 4, 7, 3, 4 });

            HeaderTable.AddCell(new Phrase("Form Type:", spanBold));
            HeaderTable.AddCell(new Phrase(preBidDetails.FormType, spanNormal));
            HeaderTable.AddCell(new Phrase());
            HeaderTable.AddCell(new Phrase());

            HeaderTable.AddCell(new Phrase("JobNumber:", spanBold));
            HeaderTable.AddCell(new Phrase(preBidDetails.JobNumber, spanNormal));
            HeaderTable.AddCell(new Phrase("Job Name:", spanBold));
            HeaderTable.AddCell(new Phrase(preBidDetails.JobName, spanNormal));
            HeaderTable.AddCell(new Phrase("Sales Person:", spanBold));
            HeaderTable.AddCell(new Phrase(Convert.ToString(preBidDetails.SalesPerson), spanNormal));
            HeaderTable.AddCell(new Phrase("Company:", spanBold));
            HeaderTable.AddCell(new Phrase(preBidDetails.Company, spanNormal));

            HeaderTable.AddCell(new Phrase("Rate Type:", spanBold));
            HeaderTable.AddCell(new Phrase(preBidDetails.RateType, spanNormal));
            HeaderTable.AddCell(new Phrase());
            HeaderTable.AddCell(new Phrase());
            HeaderTable.TotalWidth = 100;
            document.Add(HeaderTable);

            var SectionTable = new PdfPTable(3);
            SectionTable.HorizontalAlignment = 0;
            SectionTable.SpacingBefore = 5;
            SectionTable.SpacingAfter = 5;
            SectionTable.DefaultCell.Border = 0;
            SectionTable.SetWidths(new int[] { 7, 4, 4 });
            SectionTable.AddCell(getLabelValue("Material Cost:", font_body));
            SectionTable.AddCell(getDecimalValue(preBidDetails.Material, spanNormal));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Material Handling Charge:", font_body));
            SectionTable.AddCell(getDecimalValue(preBidDetails.MaterialHandling, spanNormal));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Concrete:", font_body));
            SectionTable.AddCell(getDecimalValue(preBidDetails.Concrete, spanNormal));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Material Total:", font_body));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));
            SectionTable.AddCell(getDecimalValue(preBidDetails.MaterialTotal, font_body_bold));

            SectionTable.AddCell(getLabelValue("     ", spanNormal));
            SectionTable.AddCell(getLabelValue("     ", spanNormal));
            SectionTable.AddCell(getLabelValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Onsite Labor:", font_body));
            SectionTable.AddCell(getDecimalValue(preBidDetails.OnsiteLabor, spanNormal));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Load Labor:", font_body));
            SectionTable.AddCell(getDecimalValue(preBidDetails.LoadLabor, spanNormal));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Drive Labor:", font_body));
            SectionTable.AddCell(getDecimalValue(preBidDetails.DriveLabor, spanNormal));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Supervisor:", font_body));
            SectionTable.AddCell(getDecimalValue(preBidDetails.Supervisor, spanNormal));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Labor Reserve:", font_body));
            SectionTable.AddCell(getDecimalValue(preBidDetails.LaborReserve, spanNormal));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Labor Total:", font_body));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));
            SectionTable.AddCell(getDecimalValue(preBidDetails.LaborTotal, font_body_bold));

            SectionTable.AddCell(getLabelValue("     ", spanNormal));
            SectionTable.AddCell(getLabelValue("     ", spanNormal));
            SectionTable.AddCell(getLabelValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Other Charges:", font_body));
            SectionTable.AddCell(getDecimalValue(preBidDetails.OtherCharges, spanNormal));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Other Charges Markup:", font_body));
            SectionTable.AddCell(getDecimalValue(preBidDetails.OtherChargesMarkup, spanNormal));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Other Charges Total:", font_body));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));
            SectionTable.AddCell(getDecimalValue(preBidDetails.OtherChargesTotal, font_body_bold));

            SectionTable.AddCell(getLabelValue("     ", spanNormal));
            SectionTable.AddCell(getLabelValue("     ", spanNormal));
            SectionTable.AddCell(getLabelValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Equipment Cost:", font_body));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));
            SectionTable.AddCell(getDecimalValue(preBidDetails.EquipmentCost, font_body_bold));

            SectionTable.AddCell(getLabelValue("     ", spanNormal));
            SectionTable.AddCell(getLabelValue("     ", spanNormal));
            SectionTable.AddCell(getLabelValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Benefits:", font_body));
            SectionTable.AddCell(getDecimalValue(preBidDetails.Benefits, spanNormal));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Retirement:", font_body));
            SectionTable.AddCell(getDecimalValue(preBidDetails.Retirement, spanNormal));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Payroll Tax:", font_body));
            SectionTable.AddCell(getDecimalValue(preBidDetails.PayrollTax, spanNormal));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Workers Comp:", font_body));
            SectionTable.AddCell(getDecimalValue(preBidDetails.WorkersComp, spanNormal));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Indirect Total:", font_body));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));
            SectionTable.AddCell(getDecimalValue(preBidDetails.IndirectTotal, font_body_bold));

            SectionTable.AddCell(getLabelValue("     ", spanNormal));
            SectionTable.AddCell(getLabelValue("     ", spanNormal));
            SectionTable.AddCell(getLabelValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Job Cost:", font_body));
            SectionTable.AddCell(getDecimalValue(preBidDetails.JobCost, spanNormal));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Job Markup:", font_body));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));
            SectionTable.AddCell(getDecimalValue(preBidDetails.JobMarkUpTotal, font_body_bold));

            SectionTable.AddCell(getLabelValue("Suggested Sold For:", font_body));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));
            SectionTable.AddCell(getDecimalValue(preBidDetails.SuggestdSoldFor, font_body_bold));

            SectionTable.AddCell(getLabelValue("Pre-Tax Sold For:", font_body));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));
            SectionTable.AddCell(getDecimalValue(preBidDetails.PreTaxSoldFor, font_body_bold));

            SectionTable.AddCell(getLabelValue("     ", spanNormal));
            SectionTable.AddCell(getLabelValue("     ", spanNormal));
            SectionTable.AddCell(getLabelValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Sales Tax Total:", font_body));
            SectionTable.AddCell(getDecimalValue("     ", spanNormal));
            SectionTable.AddCell(getDecimalValue(preBidDetails.SalesTaxTotal, font_body_bold));

            SectionTable.AddCell(getLabelValue("     ", spanNormal));
            SectionTable.AddCell(getLabelValue("     ", spanNormal));
            SectionTable.AddCell(getLabelValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Days On Site:", font_body));
            SectionTable.AddCell(getDecimalValue(preBidDetails.DaysOnsite, spanNormal, hasBorder: false,plainText:true));
            SectionTable.AddCell(getLabelValue("     ", spanNormal));

            SectionTable.AddCell(getLabelValue("Crew Labor Budget:", font_body));
            SectionTable.AddCell(getDecimalValue(preBidDetails.CrewLaborBudget, spanNormal, hasBorder: false));
            SectionTable.AddCell(getLabelValue("     ", spanNormal));

            document.Add(SectionTable);

            document.Close();
            byte[] file = ms.ToArray();
            MemoryStream output = new MemoryStream();
            output.Write(file, 0, file.Length);
            output.Position = 0;

            HttpContext.Response.AddHeader("content-disposition", "inline; filename=PreBid_RollUp_" + preBidDetails.PRJID + ".pdf");
            return File(output, "application/pdf");
        }

        public PdfPCell getLabelValue(string text, iTextSharp.text.Font fontStyle)
        {
            var cell = new PdfPCell(new Phrase(text, fontStyle));
            cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
            return cell;
        }

        public PdfPCell getDecimalValue(object text, iTextSharp.text.Font fontStyle,bool IsCosPercent = false,bool hasBorder = true,
            bool plainText = false)
        {
            if (!string.IsNullOrWhiteSpace(Convert.ToString(text)))
            {
                decimal val = 0M;
                if (!plainText)
                {
                    if (decimal.TryParse(text.ToString(), out val))
                    {
                        Convert.ToDecimal(text);
                        val = Math.Round(val, 2);
                        text = !IsCosPercent ? String.Format("{0:C}", val) : String.Format("{0}%", val);
                    }
                }
                else
                {
                    Convert.ToDecimal(text);
                    val = Math.Round(val, 2);
                    text = !IsCosPercent ? String.Format("{0:C}", val) : String.Format("{0}%", val);
                }
            }
            var cell = new PdfPCell(new Phrase(Convert.ToString(text), fontStyle));
            
            cell.HorizontalAlignment = 2;
            if (hasBorder)
            {
                cell.Border = iTextSharp.text.Rectangle.BOX;
                cell.BorderColor = new BaseColor(119, 119, 119);
            }
            else { cell.Border = iTextSharp.text.Rectangle.NO_BORDER; }
            return cell;
        }

      
    }
}