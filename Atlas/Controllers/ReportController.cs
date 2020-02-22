using Atlas.DataAccess.Entity;
using Atlas.Models;
using Atlas.Models.DBO;
using iText.Kernel.Colors;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Atlas.Controllers
{
    public class ReportController : Controller
    {
        // Set up fonts used in the document
        //BaseColor LightBlue = new BaseColor(14, 45, 76);
        //BaseColor grey = new BaseColor(119, 119, 119);
        //iTextSharp.text.Font font_heading_1 = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 17, new BaseColor(14, 45, 76));
        //iTextSharp.text.Font font_body = FontFactory.GetFont(FontFactory.HELVETICA, 10, new BaseColor(14, 45, 76));
        //iTextSharp.text.Font font_body_bold = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11, new BaseColor(14, 45, 76));
        //iTextSharp.text.Font spanBold = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, new BaseColor(119, 119, 119));
        //iTextSharp.text.Font spanNormal = FontFactory.GetFont(FontFactory.HELVETICA, 11, new BaseColor(119, 119, 119));

        [Route("Report/timesheet/id/{code}/bid/{bidid}")]
        public FileStreamResult timesheet(string code, string bidid)
        {
            string title = string.Empty;
            if (code == "1")
            {
                title = "Crew Time Sheet";
            }
            else if (code == "2")
            {
                title = "Crew Sub Time Sheet";
            }
            var timesheetDtls = HomeDal.getCrewTimesheet(code, bidid);
            string Company = string.Empty;
            string MhRate = string.Empty;
            if (timesheetDtls.Count > 0)
            {
                Company = timesheetDtls.FirstOrDefault().Company;
                MhRate = timesheetDtls.FirstOrDefault().MhRate;
            }
            // Set up the document and the MS to write it to and create the PDF writer instance
            MemoryStream ms = new MemoryStream();
            Document document = new Document(PageSize.A4.Rotate(), 15, 15, 46, 42);
            PdfWriter writer = PdfWriter.GetInstance(document, ms);
            writer.PageEvent = new PDFReportEventHelper(title);

            // Open the PDF document
            document.Open();

            var HeaderTable = new PdfPTable(6);
            HeaderTable.HorizontalAlignment = 0;
            HeaderTable.WidthPercentage = 100;
            HeaderTable.SpacingBefore = 5;
            HeaderTable.SpacingAfter = 5;
            HeaderTable.DefaultCell.Border = 0;
            HeaderTable.SetWidths(new float[] { 6, 1.4f, 2, 1.1f, 5, 3 });

            HeaderTable.AddCell(new Phrase(""));
            HeaderTable.AddCell(new Phrase("Company", PDFUtil.font_body_bold));
            HeaderTable.AddCell(new Phrase(Company, PDFUtil.font_body));
            HeaderTable.AddCell(new Phrase("MhRate", PDFUtil.font_body_bold));
            HeaderTable.AddCell(new Phrase(MhRate, PDFUtil.font_body));
            var dateCell = new PdfPCell(new Phrase("Date: _____________", PDFUtil.font_body));
            dateCell.Border = Rectangle.NO_BORDER;
            dateCell.PaddingTop = 5f;
            HeaderTable.AddCell(dateCell);
            document.Add(HeaderTable);

            var sectionTable = new PdfPTable(10);
            sectionTable.WidthPercentage = 100;
            sectionTable.HorizontalAlignment = 0;
            sectionTable.SpacingBefore = 5;
            sectionTable.SpacingAfter = 5;
            sectionTable.DefaultCell.Border = 0;
            sectionTable.SetWidths(new float[] { 2.8f, 4.5f, 3, 3, 3, 3, 3, 1, 4, 3 });

            sectionTable.AddCell(new Phrase("", PDFUtil.font_body));
            sectionTable.AddCell(new Phrase("", PDFUtil.font_body));
            sectionTable.AddCell(PDFUtil.CreateCell("Start Time", PDFUtil.font_body, 1));
            sectionTable.AddCell(PDFUtil.CreateCell("On Site", PDFUtil.font_body, 1));
            sectionTable.AddCell(PDFUtil.CreateCell("Off Site", PDFUtil.font_body, 1));
            sectionTable.AddCell(PDFUtil.CreateCell("Stop Time", PDFUtil.font_body, 1));
            sectionTable.AddCell(PDFUtil.CreateCell("Lunch", PDFUtil.font_body, 1));
            sectionTable.AddCell(PDFUtil.CreateCell("  ", PDFUtil.font_body));
            sectionTable.AddCell(PDFUtil.CreateCell("", PDFUtil.font_body));
            sectionTable.AddCell(PDFUtil.CreateCell("", PDFUtil.font_body));

            sectionTable.AddCell(new Phrase("Crew Foreman", PDFUtil.font_body));
            sectionTable.AddCell(new Phrase("__________________", PDFUtil.font_body));
            sectionTable.AddCell(BlankCell());
            sectionTable.AddCell(BlankCell());
            sectionTable.AddCell(BlankCell());
            sectionTable.AddCell(BlankCell());
            sectionTable.AddCell(BlankCell());
            sectionTable.AddCell(new Phrase("  ", PDFUtil.font_body));
            sectionTable.AddCell(new Phrase("Monday", PDFUtil.font_body));
            sectionTable.AddCell(new Phrase("Friday", PDFUtil.font_body));

            sectionTable.AddCell(new Phrase("Crew Helper", PDFUtil.font_body));
            sectionTable.AddCell(new Phrase("__________________", PDFUtil.font_body));
            sectionTable.AddCell(BlankCell());
            sectionTable.AddCell(BlankCell());
            sectionTable.AddCell(BlankCell());
            sectionTable.AddCell(BlankCell());
            sectionTable.AddCell(BlankCell());
            sectionTable.AddCell(new Phrase("  ", PDFUtil.font_body));
            sectionTable.AddCell(new Phrase("Tuesday", PDFUtil.font_body));
            sectionTable.AddCell(new Phrase("Saturday", PDFUtil.font_body));

            sectionTable.AddCell(new Phrase("Crew Helper", PDFUtil.font_body));
            sectionTable.AddCell(new Phrase("__________________", PDFUtil.font_body));
            sectionTable.AddCell(BlankCell());
            sectionTable.AddCell(BlankCell());
            sectionTable.AddCell(BlankCell());
            sectionTable.AddCell(BlankCell());
            sectionTable.AddCell(BlankCell());
            sectionTable.AddCell(new Phrase("  ", PDFUtil.font_body));
            sectionTable.AddCell(new Phrase("Wednesday", PDFUtil.font_body));
            sectionTable.AddCell(new Phrase("Sunday", PDFUtil.font_body));

            sectionTable.AddCell(new Phrase("Crew Helper", PDFUtil.font_body));
            sectionTable.AddCell(new Phrase("__________________", PDFUtil.font_body));
            sectionTable.AddCell(BlankCell());
            sectionTable.AddCell(BlankCell());
            sectionTable.AddCell(BlankCell());
            sectionTable.AddCell(BlankCell());
            sectionTable.AddCell(BlankCell());
            sectionTable.AddCell(new Phrase("  ", PDFUtil.font_body));
            sectionTable.AddCell(new Phrase("Thursday", PDFUtil.font_body));
            sectionTable.AddCell(new Phrase("", PDFUtil.font_body));
            document.Add(sectionTable);
            if (timesheetDtls.Count > 0)
            {
                var Section2Table = new PdfPTable(4);
                Section2Table.WidthPercentage = 70;
                Section2Table.HorizontalAlignment = 0;
                Section2Table.SpacingBefore = 8;
                // Section2Table.SpacingAfter = 5;
                Section2Table.DefaultCell.Border = 0;
                Section2Table.SetWidths(new float[] { 1.6f, 8, 1.4f, 4 });
                Section2Table.PaddingTop = 10f;
                Section2Table.AddCell(PDFUtil.CreateCell("Job Name", PDFUtil.font_body_bold, 6));
                Section2Table.AddCell(PDFUtil.CreateCell(timesheetDtls.FirstOrDefault()?.JobName, PDFUtil.font_body, 6));
                Section2Table.AddCell(PDFUtil.CreateCell("Location", PDFUtil.font_body_bold, 6));
                Section2Table.AddCell(PDFUtil.CreateCell(timesheetDtls.FirstOrDefault()?.Location, PDFUtil.font_body, 6));
                document.Add(Section2Table);
            }

            var LaborSection = new PdfPTable(11);
            LaborSection.HorizontalAlignment = 0;
            LaborSection.WidthPercentage = 100;
            LaborSection.SpacingBefore = 2;
            LaborSection.SpacingAfter = 5;
            LaborSection.DefaultCell.Border = 0;
            LaborSection.SetWidths(new float[] { 2, 2, 2.4f, 8, 3, 3, 3, 3, 3, 3, 3 });

            LaborSection.AddCell(PDFUtil.HeaderCell("BI#", PDFUtil.font_body_bold));
            LaborSection.AddCell(PDFUtil.HeaderCell("Job#", PDFUtil.font_body_bold));
            LaborSection.AddCell(PDFUtil.HeaderCell("Labor Id", PDFUtil.font_body_bold));
            LaborSection.AddCell(PDFUtil.HeaderCell("Labor Description", PDFUtil.font_body_bold));
            LaborSection.AddCell(PDFUtil.HeaderCell("Bid Qty", PDFUtil.font_body_bold, 2));
            LaborSection.AddCell(PDFUtil.HeaderCell("Uom", PDFUtil.font_body_bold, 1));
            LaborSection.AddCell(PDFUtil.HeaderCell("Mhs Ea", PDFUtil.font_body_bold, 2));
            LaborSection.AddCell(PDFUtil.HeaderCell("Mhs Ext", PDFUtil.font_body_bold, 2));
            LaborSection.AddCell(PDFUtil.HeaderCell("Rate Ea", PDFUtil.font_body_bold, 2));
            LaborSection.AddCell(PDFUtil.HeaderCell("Crew Pay", PDFUtil.font_body_bold, 2));
            LaborSection.AddCell(PDFUtil.HeaderCell("Completed", PDFUtil.font_body_bold));
            LaborSection.HeaderRows = 1;
            LaborSection.SplitLate = false;
            LaborSection.SplitRows = true;
            int len = timesheetDtls.Count;
            for (int i = 0; i < len; i++)
            {
                if (i == 0)
                {
                    timesheetDtls[i].JobNumber = string.IsNullOrEmpty(timesheetDtls[i].JobNumber) ? "" : timesheetDtls[i].JobNumber;
                    LaborSection.AddCell(new Phrase(timesheetDtls[i].BIDID.ToString(), PDFUtil.font_body));
                    LaborSection.AddCell(new Phrase(timesheetDtls[i].JobNumber, PDFUtil.font_body));
                }
                else
                {
                    LaborSection.AddCell(new Phrase("", PDFUtil.font_body));
                    LaborSection.AddCell(new Phrase("", PDFUtil.font_body));
                }
                LaborSection.AddCell(ConvertData(timesheetDtls[i].LaborId, PDFUtil.font_body));
                LaborSection.AddCell(ConvertData(timesheetDtls[i].LaborDescription, PDFUtil.font_body));
                LaborSection.AddCell(PDFUtil.CreateCell(timesheetDtls[i].BidQty, PDFUtil.font_body, 2));
                LaborSection.AddCell(PDFUtil.CreateCell(timesheetDtls[i].Uom, PDFUtil.font_body, 1));
                LaborSection.AddCell(PDFUtil.CreateCell(timesheetDtls[i].MhsEa, PDFUtil.font_body, 2));
                LaborSection.AddCell(PDFUtil.CreateCell(timesheetDtls[i].MhsExt, PDFUtil.font_body, 2));
                LaborSection.AddCell(PDFUtil.CreateCell(timesheetDtls[i].RateEa, PDFUtil.font_body, 2));
                LaborSection.AddCell(PDFUtil.CreateCell(timesheetDtls[i].PayExt, PDFUtil.font_body, 2));
                LaborSection.AddCell(new Phrase("_________", PDFUtil.font_body));
            }

            if (len == 0)
            {
                var noData = new PdfPCell(new Phrase("No Data available", PDFUtil.font_body));
                noData.Border = Rectangle.NO_BORDER;
                noData.HorizontalAlignment = Element.ALIGN_CENTER;
                noData.Colspan = 11;
                LaborSection.AddCell(noData);
            }


            document.Add(LaborSection);

            var FooterTable = new PdfPTable(5);
            FooterTable.HorizontalAlignment = 0;
            FooterTable.WidthPercentage = 100;
            FooterTable.SpacingBefore = 5;
            FooterTable.SpacingAfter = 5;
            FooterTable.DefaultCell.Border = 0;
            FooterTable.SetWidths(new float[] { 1.6f, 2, 2, 2.9f, 3 });
            var notesCell = new PdfPCell(new Phrase("Notes:", PDFUtil.font_body_bold));
            notesCell.PaddingBottom = 10f;
            notesCell.Border = Rectangle.NO_BORDER;
            FooterTable.AddCell(notesCell);
            var noContent = new PdfPCell(new Phrase(""));
            noContent.Border = Rectangle.NO_BORDER;
            noContent.PaddingBottom = 10f;
            var noContentSpan = new PdfPCell(new Phrase(""));
            noContentSpan.Border = Rectangle.NO_BORDER;
            noContentSpan.Colspan = 4;
            FooterTable.AddCell(noContentSpan);

            var CommentsCell = new PdfPCell(new Phrase("Comments", PDFUtil.font_body_bold));
            CommentsCell.Border = Rectangle.NO_BORDER;
            FooterTable.AddCell(CommentsCell);
            var blankFullLines = new PdfPCell(new Phrase("_____________________________________________________________________________________________________", PDFUtil.font_body_bold));
            blankFullLines.Colspan = 4;
            blankFullLines.PaddingBottom = 10f;
            blankFullLines.Border = Rectangle.NO_BORDER;
            FooterTable.AddCell(blankFullLines);
            //FooterTable.AddCell(noContent);
            FooterTable.AddCell(noContent);
            FooterTable.AddCell(blankFullLines);
            var sign1 = new PdfPCell(new Phrase("Foreman Signature", PDFUtil.font_body_bold));
            sign1.Border = Rectangle.NO_BORDER;
            sign1.PaddingBottom = 10f;
            FooterTable.AddCell(sign1);
            var emptyLine = new PdfPCell(new Phrase("_____________________", PDFUtil.font_body_bold));
            emptyLine.HorizontalAlignment = 0;
            emptyLine.Border = iTextSharp.text.Rectangle.NO_BORDER;
            FooterTable.AddCell(emptyLine);
            FooterTable.AddCell(noContent);
            FooterTable.AddCell(noContent);
            FooterTable.AddCell(noContent);
            var sign2 = new PdfPCell(new Phrase("Helper Signature", PDFUtil.font_body_bold));
            sign2.Border = Rectangle.NO_BORDER;
            sign2.PaddingBottom = 5f;
            FooterTable.AddCell(sign2);
            FooterTable.AddCell(emptyLine);
            FooterTable.AddCell(noContent);
            FooterTable.AddCell(emptyLine);
            FooterTable.AddCell(emptyLine);


            var FooterTable2 = new PdfPTable(4);
            FooterTable2.HorizontalAlignment = 0;
            FooterTable2.WidthPercentage = 75;
            FooterTable2.SpacingBefore = 5;
            FooterTable2.SpacingAfter = 5;
            FooterTable2.DefaultCell.Border = 0;
            FooterTable2.SetWidths(new float[] { 1.6f, 2, 2, 3 });

            var sign3 = new PdfPCell(new Phrase("Supervisor", PDFUtil.font_body_bold));
            sign3.Border = Rectangle.NO_BORDER;
            FooterTable2.AddCell(sign3);
            FooterTable2.AddCell(emptyLine);
            var sign4 = new PdfPCell(new Phrase("Is Job 100% Completed?", PDFUtil.font_body_bold));
            sign4.Border = Rectangle.NO_BORDER;
            FooterTable2.AddCell(sign4);
            FooterTable2.AddCell(emptyLine);

            document.Add(FooterTable);
            document.Add(FooterTable2);
            document.Close();
            byte[] file = ms.ToArray();
            MemoryStream output = new MemoryStream();
            output.Write(file, 0, file.Length);
            output.Position = 0;

            HttpContext.Response.AddHeader("content-disposition", "inline; filename=Timesheet_" + bidid + ".pdf");
            return File(output, "application/pdf");
        }

        [Route("Report/MRO/bid/{bidid}")]
        public FileStreamResult MROReport(string bidid)
        {
            var packagingDtls = HomeDal.getPackaging(bidid);
            string MRO_Number = string.Empty;
            string FileName = string.Empty;
            string JobNumber = string.Empty;
            string ProjectName = string.Empty;
            string BidId = string.Empty;
            string BidName = string.Empty;
            DateTime RunDate = DateTime.MinValue;

            if (packagingDtls.Count > 0)
            {
                MRO_Number = packagingDtls.FirstOrDefault().MRO_Number.ToString();
                FileName = packagingDtls.FirstOrDefault().CfsFileName;
                JobNumber = packagingDtls.FirstOrDefault().JobNumber.ToString();
                ProjectName = packagingDtls.FirstOrDefault().ProjectName;
                BidId = packagingDtls.FirstOrDefault().BidItemNumber.ToString();
                BidName = packagingDtls.FirstOrDefault().BIDName;
                RunDate = packagingDtls.FirstOrDefault().RunDate;
            }
            // Set up the document and the MS to write it to and create the PDF writer instance
            MemoryStream ms = new MemoryStream();
            Document document = new Document(PageSize.A4.Rotate(), 15, 15, 46, 42);
            PdfWriter writer = PdfWriter.GetInstance(document, ms);
            //writer.PageEvent = new ITextEvents();
            writer.PageEvent = new PackagingEventHelper();
            // Open the PDF document

            document.Open();
            #region Packaging Top Section
            var HeaderTable = new PdfPTable(6);
            HeaderTable.HorizontalAlignment = 0;
            HeaderTable.WidthPercentage = 100;
            HeaderTable.SpacingBefore = 10;
            HeaderTable.SpacingAfter = 5;
            HeaderTable.DefaultCell.Border = 0;
            HeaderTable.SetWidths(new float[] { 0.8f, 2, 1.3f, 5, 1.3f, 3 });

            HeaderTable.AddCell(new Phrase("MRO #", PDFUtil.font_body_bold));
            HeaderTable.AddCell(new Phrase(MRO_Number, PDFUtil.font_body));
            HeaderTable.AddCell(new Phrase("File Name", PDFUtil.font_body_bold));
            HeaderTable.AddCell(new Phrase(FileName, PDFUtil.font_body));
            HeaderTable.AddCell(new Phrase("Printed Date", PDFUtil.font_body_bold));
            HeaderTable.AddCell(new Phrase(DateTime.Now.ToString("G"), PDFUtil.font_body));
            HeaderTable.AddCell(new Phrase("Job #", PDFUtil.font_body_bold));
            HeaderTable.AddCell(new Phrase(JobNumber, PDFUtil.font_body));
            HeaderTable.AddCell(new Phrase("Project Name", PDFUtil.font_body_bold));
            HeaderTable.AddCell(new Phrase(ProjectName, PDFUtil.font_body));
            HeaderTable.AddCell(new Phrase("Run Date", PDFUtil.font_body_bold));
            if (RunDate != DateTime.MinValue)
            {
                HeaderTable.AddCell(new Phrase(RunDate.ToString("d"), PDFUtil.font_body));
            }
            else
            {
                HeaderTable.AddCell(new Phrase("", PDFUtil.font_body));
            }
            HeaderTable.AddCell(new Phrase("Bid #", PDFUtil.font_body_bold));
            HeaderTable.AddCell(new Phrase(BidId, PDFUtil.font_body));
            HeaderTable.AddCell(new Phrase("Bid Name", PDFUtil.font_body_bold));
            HeaderTable.AddCell(new Phrase(BidName, PDFUtil.font_body));
            HeaderTable.AddCell(new Phrase(""));
            HeaderTable.AddCell(new Phrase(""));

            document.Add(HeaderTable);
            #endregion

            var LaborSection = new PdfPTable(7);
            LaborSection.HorizontalAlignment = 0;
            LaborSection.WidthPercentage = 100;
            LaborSection.SpacingBefore = 5;
            LaborSection.SpacingAfter = 5;
            LaborSection.DefaultCell.Border = 0;
            LaborSection.SetWidths(new float[] { 2, 2.5f, 2, 2, 2, 6.5f, 2 });

            LaborSection.AddCell(PDFUtil.HeaderCell("Qty Requested", PDFUtil.font_body_bold));
            LaborSection.AddCell(PDFUtil.HeaderCell("PartNum", PDFUtil.font_body_bold));
            LaborSection.AddCell(PDFUtil.HeaderCell("Date:__/__/__", PDFUtil.font_body_bold));
            LaborSection.AddCell(PDFUtil.HeaderCell("Date:__/__/__", PDFUtil.font_body_bold));
            LaborSection.AddCell(PDFUtil.HeaderCell("Date:__/__/__", PDFUtil.font_body_bold));
            LaborSection.AddCell(PDFUtil.HeaderCell("Description", PDFUtil.font_body_bold));
            LaborSection.AddCell(PDFUtil.HeaderCell("Return Qty", PDFUtil.font_body_bold));
            LaborSection.HeaderRows = 1;
            LaborSection.SplitLate = false;
            LaborSection.SplitRows = true;
            int len = packagingDtls.Count;
            int lineCount = 0;
            for (int i = 0; i < len; i++)
            {
                LaborSection.AddCell(ConvertData(Convert.ToInt32(packagingDtls[i].Qty), PDFUtil.font_body));
                LaborSection.AddCell(ConvertData(packagingDtls[i].PartNum, PDFUtil.font_body));
                LaborSection.AddCell(ConvertData("____________", PDFUtil.font_body));
                LaborSection.AddCell(ConvertData("____________", PDFUtil.font_body));
                LaborSection.AddCell(ConvertData("____________", PDFUtil.font_body));
                LaborSection.AddCell(ConvertData(packagingDtls[i].Description, PDFUtil.font_body));
                LaborSection.AddCell(ConvertData("____________", PDFUtil.font_body));
            }

            if (len == 0)
            {
                var noData = new PdfPCell(new Phrase("No Data available", PDFUtil.font_body));
                noData.Border = Rectangle.NO_BORDER;
                noData.HorizontalAlignment = Element.ALIGN_CENTER;
                noData.Colspan = 7;
                LaborSection.AddCell(noData);
            }


            document.Add(LaborSection);
            // document.Add(GetLineSeparator());
            document.Close();
            byte[] file = ms.ToArray();
            MemoryStream output = new MemoryStream();
            output.Write(file, 0, file.Length);
            output.Position = 0;

            HttpContext.Response.AddHeader("content-disposition", "inline; filename=Timesheet_" + bidid + ".pdf");
            return File(output, "application/pdf");
        }

        [Route("Report/JobActivation/id/{prjid}")]
        public FileStreamResult printJobActivation(string prjid)
        {
            string title = string.Empty;

            title = "Atlas Residential & Commerical Services LLC - Job Activation CheckList";

            var model = new JobActivationChecklistModel();
            model.PRJID = Convert.ToInt32(prjid);
            model = ProjectDAL.getProjectActivationDetails(model);
            model = ProjectDAL.JobActivationLookup(model);

            // Set up the document and the MS to write it to and create the PDF writer instance
            MemoryStream ms = new MemoryStream();
            Document document = new Document(PageSize.A4.Rotate(), 15, 15, 46, 42);
            PdfWriter writer = PdfWriter.GetInstance(document, ms);
            writer.PageEvent = new PDFReportEventHelper(title);

            // Open the PDF document
            document.Open();

            document.Add(PDFUtil.HeaderSection("Project Information"));

            var projectTable = new PdfPTable(2);
            projectTable.HorizontalAlignment = 0;
            projectTable.WidthPercentage = 100;
            projectTable.SpacingBefore = 5;
            projectTable.SpacingAfter = 5;
            projectTable.DefaultCell.Border = 0;
            projectTable.DefaultCell.Padding = 30f;
            projectTable.SetWidths(new float[] { 3, 6 });

            projectTable.AddCell(PDFUtil.CreateCell("Atlas Job Number", PDFUtil.font_body_bold, 2, false));
            projectTable.AddCell(PDFUtil.CreateCell(model.projectInformation.JobNumber, PDFUtil.spanNormalBlack, 0, false));
            projectTable.AddCell(PDFUtil.CreateCell("Atlas Company Name", PDFUtil.font_body_bold, 2, false));
            projectTable.AddCell(PDFUtil.CreateCell(model.projectInformation.CompanyName, PDFUtil.spanNormalBlack, 0, false));
            projectTable.AddCell(PDFUtil.CreateCell("Customer/Contractor Name", PDFUtil.font_body_bold, 2, false));
            projectTable.AddCell(PDFUtil.CreateCell(model.projectInformation.CustomerProfile.Name, PDFUtil.spanNormalBlack, 0, false));
            projectTable.AddCell(PDFUtil.CreateCell("Address, City, State, Zip", PDFUtil.font_body_bold, 2, false));
            string CustomerAddr = string.Format("{0} | {1} | {2} | {3}",
                                                        model.projectInformation.CustomerProfile.Address,
                                                        model.projectInformation.CustomerProfile.City,
                                                        model.projectInformation.CustomerProfile.State,
                                                        model.projectInformation.CustomerProfile.Zip);
            projectTable.AddCell(PDFUtil.CreateCell(CustomerAddr, PDFUtil.spanNormalBlack, 0, false));
            projectTable.AddCell(PDFUtil.CreateCell("Phone Number, Extension", PDFUtil.font_body_bold, 2, false));
            string CustomerPhone = string.Format("{0} | {1}",
                                                        model.projectInformation.CustomerProfile.PhoneNumber,
                                                        model.projectInformation.CustomerProfile.Extension);
            projectTable.AddCell(PDFUtil.CreateCell(CustomerPhone, PDFUtil.spanNormalBlack, 0, false));
            projectTable.AddCell(PDFUtil.CreateCell("Project Name", PDFUtil.font_body_bold, 2, false));
            projectTable.AddCell(PDFUtil.CreateCell(model.projectInformation.ProjectProfile.Name, PDFUtil.spanNormalBlack, 0, false));
            projectTable.AddCell(PDFUtil.CreateCell("Address, City, State, Zip", PDFUtil.font_body_bold, 2, false));
            string ProjectAddr = string.Format("{0} | {1} | {2} | {3}",
                                                       model.projectInformation.ProjectProfile.Address,
                                                       model.projectInformation.ProjectProfile.City,
                                                       model.projectInformation.ProjectProfile.State,
                                                       model.projectInformation.ProjectProfile.Zip);
            projectTable.AddCell(PDFUtil.CreateCell(ProjectAddr, PDFUtil.spanNormalBlack, 0, false));
            projectTable.AddCell(PDFUtil.CreateCell("Contact Name/Phone Number, Ext", PDFUtil.font_body_bold, 2, false));
            string ProjectPhone = string.Format("{0} | {1} | {2}",
                                                        model.projectInformation.CustomerProfile.ContactName,
                                                        model.projectInformation.CustomerProfile.PhoneNumber,
                                                        model.projectInformation.CustomerProfile.Extension);
            projectTable.AddCell(PDFUtil.CreateCell(ProjectPhone, PDFUtil.spanNormalBlack, 0, false));
            projectTable.AddCell(PDFUtil.CreateCell("Customer Type", PDFUtil.font_body_bold, 2, false));
            projectTable.AddCell(PDFUtil.CreateCell(model.ListCustomersType.
                First(i => i.CustomerTypeId.ToString() == model.projectInformation.CustomerType).Description, PDFUtil.spanNormalBlack, 0, false));
            projectTable.AddCell(PDFUtil.CreateCell("Job Type", PDFUtil.font_body_bold, 2, false));
            projectTable.AddCell(PDFUtil.CreateCell(model.ListJobTypes.
                First(i => i.JobTypeId.ToString() == model.projectInformation.Jobtype).JobTypeDesc, PDFUtil.spanNormalBlack, 0, false));
            projectTable.AddCell(PDFUtil.CreateCell("Customer Bid/Job Reference #", PDFUtil.font_body_bold, 2, false));
            projectTable.AddCell(PDFUtil.CreateCell(model.projectInformation.CustomerBidReference, PDFUtil.spanNormalBlack, 0, false));
            projectTable.AddCell(PDFUtil.CreateCell("Scope of Work to be performed", PDFUtil.font_body_bold, 2, false));
            projectTable.AddCell(PDFUtil.CreateCell(model.projectInformation.ScopeWorkToBePerformed, PDFUtil.spanNormalBlack, 0, false));

            projectTable.AddCell(PDFUtil.CreateCell("Type of Labor", PDFUtil.font_body_bold, 2, false));
            projectTable.AddCell(PDFUtil.CreateCell(model.projectInformation.TypeOfLabour, PDFUtil.spanNormalBlack, 0, false));

            document.Add(projectTable);

            document.Add(PDFUtil.HeaderSection("Contract/Job Paperwork"));

            var ContractTable = new PdfPTable(4);
            ContractTable.HorizontalAlignment = 0;
            ContractTable.WidthPercentage = 98;
            ContractTable.SpacingBefore = 5;
            ContractTable.SpacingAfter = 5;
            ContractTable.DefaultCell.Border = 0;
            ContractTable.DefaultCell.Padding = 30f;
            ContractTable.SetWidths(new float[] { 3, 1, 1, 5 });

            ContractTable.AddCell(PDFUtil.CreateCell("Copy of Contract or PO", PDFUtil.font_body_bold, 2, false));
            ContractTable.AddCell(PDFUtil.CreateCell(model.ListResponses.
                First(i => i.ResponseId.ToString() == model.contractPaperWork.CopyOfContractorPO).Response, PDFUtil.spanNormalBlack, 0, false));
            ContractTable.AddCell(PDFUtil.CreateCell("Comments", PDFUtil.font_body_bold, 2, false));
            ContractTable.AddCell(PDFUtil.CreateCell(model.contractPaperWork.CopyOfContractorPOComments, PDFUtil.spanNormalBlack, 0, false));

            ContractTable.AddCell(PDFUtil.CreateCell("Broken scope into appropriate phases", PDFUtil.font_body_bold, 2, false));
            ContractTable.AddCell(PDFUtil.CreateCell(model.ListResponses.
                First(i => i.ResponseId.ToString() == model.contractPaperWork.BrokenScopephases).Response, PDFUtil.spanNormalBlack, 0, false));
            ContractTable.AddCell(PDFUtil.CreateCell("Comments", PDFUtil.font_body_bold, 2, false));
            ContractTable.AddCell(PDFUtil.CreateCell(model.contractPaperWork.BrokenScopephasesComments, PDFUtil.spanNormalBlack, 0, false));

            ContractTable.AddCell(PDFUtil.CreateCell("Bid Roll-up (For each BI being activated)", PDFUtil.font_body_bold, 2, false));
            ContractTable.AddCell(PDFUtil.CreateCell(model.ListResponses.
                First(i => i.ResponseId.ToString() == model.contractPaperWork.BidRollUp).Response, PDFUtil.spanNormalBlack, 0, false));
            ContractTable.AddCell(PDFUtil.CreateCell("Comments", PDFUtil.font_body_bold, 2, false));
            ContractTable.AddCell(PDFUtil.CreateCell(model.contractPaperWork.BidRollUpComments, PDFUtil.spanNormalBlack, 0, false));

            ContractTable.AddCell(PDFUtil.CreateCell("Pack CFS for each BI rollup", PDFUtil.font_body_bold, 2, false));
            ContractTable.AddCell(PDFUtil.CreateCell(model.ListResponses.
                First(i => i.ResponseId.ToString() == model.contractPaperWork.PackCFSPIRollUp).Response, PDFUtil.spanNormalBlack, 0, false));
            ContractTable.AddCell(PDFUtil.CreateCell("Comments", PDFUtil.font_body_bold, 2, false));
            ContractTable.AddCell(PDFUtil.CreateCell(model.contractPaperWork.PackCFSPIRollUpcomments, PDFUtil.spanNormalBlack, 0, false));

            ContractTable.AddCell(PDFUtil.CreateCell("Applicable quote(s) for special material", PDFUtil.font_body_bold, 2, false));
            ContractTable.AddCell(PDFUtil.CreateCell(model.ListResponses.
                First(i => i.ResponseId.ToString() == model.contractPaperWork.ApplicableQuote).Response, PDFUtil.spanNormalBlack, 0, false));
            ContractTable.AddCell(PDFUtil.CreateCell("Comments", PDFUtil.font_body_bold, 2, false));
            ContractTable.AddCell(PDFUtil.CreateCell(model.contractPaperWork.ApplicableQuoteComments, PDFUtil.spanNormalBlack, 0, false));

            ContractTable.AddCell(PDFUtil.CreateCell("Drawings/field conditions or grading reports", PDFUtil.font_body_bold, 2, false));
            ContractTable.AddCell(PDFUtil.CreateCell(model.ListResponses.
                First(i => i.ResponseId.ToString() == model.contractPaperWork.DrawingConditions).Response, PDFUtil.spanNormalBlack, 0, false));
            ContractTable.AddCell(PDFUtil.CreateCell("Comments", PDFUtil.font_body_bold, 2, false));
            ContractTable.AddCell(PDFUtil.CreateCell(model.contractPaperWork.DrawingConditionsComments, PDFUtil.spanNormalBlack, 0, false));

            ContractTable.AddCell(PDFUtil.CreateCell("Site Photos", PDFUtil.font_body_bold, 2, false));
            ContractTable.AddCell(PDFUtil.CreateCell(model.ListResponses.
                First(i => i.ResponseId.ToString() == model.contractPaperWork.SitePhotos).Response, PDFUtil.spanNormalBlack, 0, false));
            ContractTable.AddCell(PDFUtil.CreateCell("Comments", PDFUtil.font_body_bold, 2, false));
            ContractTable.AddCell(PDFUtil.CreateCell(model.contractPaperWork.SitePhotosComments, PDFUtil.spanNormalBlack, 0, false));

            ContractTable.AddCell(PDFUtil.CreateCell("Hard Card completely filled out", PDFUtil.font_body_bold, 2, false));
            ContractTable.AddCell(PDFUtil.CreateCell(model.ListResponses.
                First(i => i.ResponseId.ToString() == model.contractPaperWork.HardCard).Response, PDFUtil.spanNormalBlack, 0, false));
            ContractTable.AddCell(PDFUtil.CreateCell("Comments", PDFUtil.font_body_bold, 2, false));
            ContractTable.AddCell(PDFUtil.CreateCell(model.contractPaperWork.HardCardComments, PDFUtil.spanNormalBlack, 0, false));

            ContractTable.AddCell(PDFUtil.CreateCell("Pay Envelope (residential only)", PDFUtil.font_body_bold, 2, false));
            ContractTable.AddCell(PDFUtil.CreateCell(model.ListResponses.
                First(i => i.ResponseId.ToString() == model.contractPaperWork.PayEnvelope).Response, PDFUtil.spanNormalBlack, 0, false));
            ContractTable.AddCell(PDFUtil.CreateCell("Comments", PDFUtil.font_body_bold, 2, false));
            ContractTable.AddCell(PDFUtil.CreateCell(model.contractPaperWork.PayEnvelopeComments, PDFUtil.spanNormalBlack, 0, false));

            document.Add(ContractTable);

            document.Add(PDFUtil.HeaderSection("Bonding/Insurance/Labor "));
            var BondingTable = new PdfPTable(4);
            BondingTable.HorizontalAlignment = 0;
            BondingTable.WidthPercentage = 98;
            BondingTable.SpacingBefore = 5;
            BondingTable.SpacingAfter = 5;
            BondingTable.DefaultCell.Border = 0;
            BondingTable.DefaultCell.Padding = 30f;
            BondingTable.SetWidths(new float[] { 3, 1, 1, 5 });

            BondingTable.AddCell(PDFUtil.CreateCell("Received bond (if required) and necessary insurance certification", PDFUtil.font_body_bold, 2, false));
            BondingTable.AddCell(PDFUtil.CreateCell(model.ListResponses.
                First(i => i.ResponseId.ToString() == model.bondingInsurance.InsuranceCertification).Response, PDFUtil.spanNormalBlack, 0, false));
            BondingTable.AddCell(PDFUtil.CreateCell("Comments", PDFUtil.font_body_bold, 2, false));
            BondingTable.AddCell(PDFUtil.CreateCell(model.bondingInsurance.InsuranceCertificationComments, PDFUtil.spanNormalBlack, 0, false));

            document.Add(BondingTable);
            document.Add(PDFUtil.HeaderSection("Safety Requirements - For all 'Yes' answers, please provide additional details"));

            var SafetyTable = new PdfPTable(4);
            SafetyTable.HorizontalAlignment = 0;
            SafetyTable.WidthPercentage = 98;
            SafetyTable.SpacingBefore = 5;
            SafetyTable.SpacingAfter = 5;
            SafetyTable.DefaultCell.Border = 0;
            SafetyTable.DefaultCell.Padding = 30f;
            SafetyTable.SetWidths(new float[] { 3, 1, 1, 5 });

            SafetyTable.AddCell(PDFUtil.CreateCell("Is there a safety officer on site? If so, provide Contact Informatios there a safety officer on site? If so, provide Contact Information", PDFUtil.font_body_bold, 2, false));
            SafetyTable.AddCell(PDFUtil.CreateCell(model.ListResponses.
                First(i => i.ResponseId.ToString() == model.safetyRequirements.SafetyOfficer).Response, PDFUtil.spanNormalBlack, 0, false));
            SafetyTable.AddCell(PDFUtil.CreateCell("Comments", PDFUtil.font_body_bold, 2, false));
            SafetyTable.AddCell(PDFUtil.CreateCell(model.safetyRequirements.SafetyOfficerComments, PDFUtil.spanNormalBlack, 0, false));

            SafetyTable.AddCell(PDFUtil.CreateCell("Any safety meetings/orientation or badging needed before the job starts?", PDFUtil.font_body_bold, 2, false));
            SafetyTable.AddCell(PDFUtil.CreateCell(model.ListResponses.
                First(i => i.ResponseId.ToString() == model.safetyRequirements.SafetyMeeting).Response, PDFUtil.spanNormalBlack, 0, false));
            SafetyTable.AddCell(PDFUtil.CreateCell("Comments", PDFUtil.font_body_bold, 2, false));
            SafetyTable.AddCell(PDFUtil.CreateCell(model.safetyRequirements.SafetyMeetingComments, PDFUtil.spanNormalBlack, 0, false));

            SafetyTable.AddCell(PDFUtil.CreateCell("Any daily safety meetings, truck/tool inspections required?", PDFUtil.font_body_bold, 2, false));
            SafetyTable.AddCell(PDFUtil.CreateCell(model.ListResponses.
                First(i => i.ResponseId.ToString() == model.safetyRequirements.DailySafetyMeeting).Response, PDFUtil.spanNormalBlack, 0, false));
            SafetyTable.AddCell(PDFUtil.CreateCell("Comments", PDFUtil.font_body_bold, 2, false));
            SafetyTable.AddCell(PDFUtil.CreateCell(model.safetyRequirements.DailySafetyMeetingComments, PDFUtil.spanNormalBlack, 0, false));

            SafetyTable.AddCell(PDFUtil.CreateCell("Any specific PPE needed (e.g. fall protection,flotation, fire clothing, etc.)", PDFUtil.font_body_bold, 2, false));
            SafetyTable.AddCell(PDFUtil.CreateCell(model.ListResponses.
                First(i => i.ResponseId.ToString() == model.safetyRequirements.PPENeeded).Response, PDFUtil.spanNormalBlack, 0, false));
            SafetyTable.AddCell(PDFUtil.CreateCell("Comments", PDFUtil.font_body_bold, 2, false));
            SafetyTable.AddCell(PDFUtil.CreateCell(model.safetyRequirements.PPENeededComments, PDFUtil.spanNormalBlack, 0, false));

            SafetyTable.AddCell(PDFUtil.CreateCell("Is fall protection is required? Please provide details of the situation ?", PDFUtil.font_body_bold, 2, false));
            SafetyTable.AddCell(PDFUtil.CreateCell(model.ListResponses.
                First(i => i.ResponseId.ToString() == model.safetyRequirements.FallProtection).Response, PDFUtil.spanNormalBlack, 0, false));
            SafetyTable.AddCell(PDFUtil.CreateCell("Comments", PDFUtil.font_body_bold, 2, false));
            SafetyTable.AddCell(PDFUtil.CreateCell(model.safetyRequirements.FallProtectionComments, PDFUtil.spanNormalBlack, 0, false));

            SafetyTable.AddCell(PDFUtil.CreateCell("Any equipment certifications required (e.g. Bobcats, Forklifts, High lift, etc)?", PDFUtil.font_body_bold, 2, false));
            SafetyTable.AddCell(PDFUtil.CreateCell(model.ListResponses.
                First(i => i.ResponseId.ToString() == model.safetyRequirements.EquipmentCertification).Response, PDFUtil.spanNormalBlack, 0, false));
            SafetyTable.AddCell(PDFUtil.CreateCell("Comments", PDFUtil.font_body_bold, 2, false));
            SafetyTable.AddCell(PDFUtil.CreateCell(model.safetyRequirements.EquipmentCertificationComments, PDFUtil.spanNormalBlack, 0, false));

            SafetyTable.AddCell(PDFUtil.CreateCell("Other hazards (water, lane closure, dust/respirator, HEPA, vacuum, heavy lifting)", PDFUtil.font_body_bold, 2, false));
            SafetyTable.AddCell(PDFUtil.CreateCell(model.ListResponses.
                First(i => i.ResponseId.ToString() == model.safetyRequirements.OtherHazards).Response, PDFUtil.spanNormalBlack, 0, false));
            SafetyTable.AddCell(PDFUtil.CreateCell("Comments", PDFUtil.font_body_bold, 2, false));
            SafetyTable.AddCell(PDFUtil.CreateCell(model.safetyRequirements.OtherHazardsComments, PDFUtil.spanNormalBlack, 0, false));

            document.Add(SafetyTable);
            document.Add(PDFUtil.HeaderSection("Other Important Factors"));

            var OtherImportantTable = new PdfPTable(2);
            OtherImportantTable.HorizontalAlignment = 0;
            OtherImportantTable.WidthPercentage = 98;
            OtherImportantTable.SpacingBefore = 5;
            OtherImportantTable.SpacingAfter = 5;
            OtherImportantTable.DefaultCell.Border = 0;
            OtherImportantTable.DefaultCell.Padding = 30f;
            OtherImportantTable.SetWidths(new float[] { 3, 7 });

            OtherImportantTable.AddCell(PDFUtil.CreateCell("Please fill in any other pertinant information", PDFUtil.font_body_bold, 2, false));
            OtherImportantTable.AddCell(PDFUtil.CreateCell(model.otherImportantFactors.OtherPertinentInformation, PDFUtil.spanNormalBlack, 0, false));
            document.Add(OtherImportantTable);

            document.Add(PDFUtil.GetLineSeparator());
            var footerTable = new PdfPTable(4);
            footerTable.HorizontalAlignment = 0;
            footerTable.WidthPercentage = 98;
            footerTable.SpacingBefore = 5;
            footerTable.SpacingAfter = 5;
            footerTable.DefaultCell.Border = 0;
            footerTable.DefaultCell.Padding = 30f;
            footerTable.SetWidths(new float[] { 1, 2, 1, 2 });

            footerTable.AddCell(PDFUtil.CreateCell("Estimator Name", PDFUtil.font_body_bold, 2, false));
            footerTable.AddCell(PDFUtil.CreateCell(model.projectInformation.Estimator, PDFUtil.spanNormalBlack, 0, false));
            footerTable.AddCell(PDFUtil.CreateCell("Date Completed", PDFUtil.font_body_bold, 2, false));
            footerTable.AddCell(PDFUtil.CreateCell(model.DateCompleted, PDFUtil.spanNormalBlack, 0, false));

            footerTable.AddCell(PDFUtil.CreateCell("Approved By", PDFUtil.font_body_bold, 2, false));
            footerTable.AddCell(PDFUtil.CreateCell(model.ApprovedBy, PDFUtil.spanNormalBlack, 0, false));
            footerTable.AddCell(PDFUtil.CreateCell("Date Reviewed", PDFUtil.font_body_bold, 2, false));
            footerTable.AddCell(PDFUtil.CreateCell(model.DateReviewed, PDFUtil.spanNormalBlack, 0, false));
            document.Add(footerTable);

            document.Close();
            byte[] file = ms.ToArray();
            MemoryStream output = new MemoryStream();
            output.Write(file, 0, file.Length);
            output.Position = 0;

            HttpContext.Response.AddHeader("content-disposition", "inline; filename=JobActivation_" + prjid + ".pdf");
            return File(output, "application/pdf");
        }

        [Route("Report/Project/id/{prjid}/Quote/{qgroup}/projectQuote")]
        public FileStreamResult printProjectQuote(string prjid, string qgroup)
        {
            

            List<Quotes> lstQuotes = new List<Quotes>();
            var model = QuoteDal.GetQuoteByGroup(prjid,qgroup);
            string title = string.Empty;
            var project = model.QuoteList.First();
            title = string.Format("Project #{0} - {1} **** Quote #{2} ", project.PRJID, project.ProjectName, qgroup);
            // Set up the document and the MS to write it to and create the PDF writer instance
            MemoryStream ms = new MemoryStream();
            Document document = new Document(PageSize.A4, 15, 15, 46, 42);
            PdfWriter writer = PdfWriter.GetInstance(document, ms);
            writer.PageEvent = new PDFReportEventHelper(title);

            document.Open();
            // Open the PDF document
            foreach (var quote in model.QuoteList)
            {
                document.Add(PDFUtil.HeaderSection(string.Format("BID#{0} - {1}",quote.BIDID, quote.BidName)));
                var BidRefItem = model.BidHeadersList.Where(i => i.BIDID == quote.BIDID).First();
                var bidTable = new PdfPTable(4);
                bidTable.HorizontalAlignment = 0;
                bidTable.WidthPercentage = 98;
                bidTable.SpacingBefore = 5;
                bidTable.SpacingAfter = 5;
                bidTable.DefaultCell.Border = 0;
                bidTable.DefaultCell.Padding = 10f;
                bidTable.SetWidths(new float[] { 3, 5, 3, 4 });

                DataSet refTable = ProjectDAL.LoadComboForHeight(prjid);
                var fenceType = refTable.Tables[0].AsEnumerable().
                                Where(i => i.Field<int>("FenceTypeID") == BidRefItem.FenceTypeID).FirstOrDefault()?.Field<string>("FenceType");

                var fenceHeight = refTable.Tables[1].AsEnumerable().
                               Where(i => i.Field<int>("FenceHtID") == BidRefItem.FenceHtID).FirstOrDefault()?.Field<decimal>("FenceHtFt");

                bidTable.AddCell(PDFUtil.CreateCell("Fence Type", PDFUtil.font_body_bold, 2, false));
                bidTable.AddCell(PDFUtil.CreateCell(fenceType, PDFUtil.spanNormalBlack, 0, false));
                bidTable.AddCell(PDFUtil.CreateCell("Fence Height", PDFUtil.font_body_bold, 2, false));
                bidTable.AddCell(PDFUtil.CreateCell(fenceHeight, PDFUtil.spanNormalBlack, 0, false));

                var FtRange = refTable.Tables[2].AsEnumerable().
                               Where(i => i.Field<int>("FtRangeID") == BidRefItem.FtRangeID).FirstOrDefault()?.Field<string>("FtRange");

                var DigType = refTable.Tables[3].AsEnumerable().
                               Where(i => i.Field<int>("DigTypeID") == BidRefItem.DigTypeID).FirstOrDefault()?.Field<string>("DigType");

                bidTable.AddCell(PDFUtil.CreateCell("Job Length", PDFUtil.font_body_bold, 2, false));
                bidTable.AddCell(PDFUtil.CreateCell(FtRange, PDFUtil.spanNormalBlack, 0, false));
                bidTable.AddCell(PDFUtil.CreateCell("Dig Conditions", PDFUtil.font_body_bold, 2, false));
                bidTable.AddCell(PDFUtil.CreateCell(DigType, PDFUtil.spanNormalBlack, 0, false));

                var UnitOfMeasure = refTable.Tables[5].AsEnumerable().
                              Where(i => i.Field<string>("UnitOfMeasure") == BidRefItem.UnitOfMeasure).FirstOrDefault()?.Field<string>("UomDescription");

                bidTable.AddCell(PDFUtil.CreateCell("Qty Of Bid Item", PDFUtil.font_body_bold, 2, false));
                bidTable.AddCell(PDFUtil.CreateCell(BidRefItem.QtyOfBI, PDFUtil.spanNormalBlack, 0, false));
                bidTable.AddCell(PDFUtil.CreateCell("Unit Of Measure", PDFUtil.font_body_bold, 2, false));
                bidTable.AddCell(PDFUtil.CreateCell(UnitOfMeasure, PDFUtil.spanNormalBlack, 0, false));

                var TaxCalcType = refTable.Tables[4].AsEnumerable().
                              Where(i => i.Field<int>("TaxCalcTypeID") == BidRefItem.TaxCalcTypeID).FirstOrDefault()?.Field<string>("TaxCalcType");

                bidTable.AddCell(PDFUtil.CreateCell("Sales Tax Type", PDFUtil.font_body_bold, 2, false));
                bidTable.AddCell(PDFUtil.CreateCell(TaxCalcType, PDFUtil.spanNormalBlack, 0, false));
                bidTable.AddCell(PDFUtil.CreateCell("Date Entered", PDFUtil.font_body_bold, 2, false));
                bidTable.AddCell(PDFUtil.CreateCell(BidRefItem.DateEntered.ToShortDateString(), PDFUtil.spanNormalBlack, 0, false));

                bidTable.AddCell(PDFUtil.CreateCell("Date Activated", PDFUtil.font_body_bold, 2, false));
                bidTable.AddCell(PDFUtil.CreateCell(BidRefItem.DateActivated.Value.ToShortDateString(), PDFUtil.spanNormalBlack, 0, false));
                bidTable.AddCell(PDFUtil.CreateCell("Status", PDFUtil.font_body_bold, 2, false));
                bidTable.AddCell(PDFUtil.CreateCell(BidRefItem.BIDStatusID, PDFUtil.spanNormalBlack, 0, false));

                document.Add(bidTable);
            }


            document.Close();
            byte[] file = ms.ToArray();
            MemoryStream output = new MemoryStream();
            output.Write(file, 0, file.Length);
            output.Position = 0;

            HttpContext.Response.AddHeader("content-disposition", "inline; filename=ProjectQuote_" + prjid + ".pdf");
            return File(output, "application/pdf");
        }

        public PdfPCell getSign(string text, Font font)
        {
            var cell = new PdfPCell(new Phrase(text, font));
            cell.Border = Rectangle.NO_BORDER;
            return cell;
        }

        private PdfPCell AddSignature(string text, Font font_body_bold, bool isLabel = true, bool hasSpan = false)
        {
            var cell = new PdfPCell();
            if (isLabel)
            {
                if (!string.IsNullOrEmpty(text))
                {
                    cell.Phrase = new Phrase(text, font_body_bold);
                    cell.Border = Rectangle.NO_BORDER;
                }
            }
            else
            {
                cell.Phrase = new Phrase("_____________________", font_body_bold);
                cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                if (hasSpan)
                {
                    cell.Colspan = 4;
                }
            }
            return cell;
        }

        public PdfPCell BlankCell()
        {
            var cell = new PdfPCell(new Phrase());
            cell.HorizontalAlignment = 2;
            cell.Border = iTextSharp.text.Rectangle.BOX;
            cell.BorderColor = new BaseColor(119, 119, 119);
            return cell;
        }

        public PdfPCell SemiLine(Font font)
        {
            var cell = new PdfPCell(new Phrase("_____________________", font));
            cell.HorizontalAlignment = 0;
            cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
            return cell;
        }

        public Phrase ConvertData(object value, Font font)
        {
            if (value != null)
            {
                return new Phrase(value.ToString(), font);
            }
            else
            {
                return new Phrase("", font);
            }
        }
    }
}