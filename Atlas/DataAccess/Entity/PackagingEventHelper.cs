using iText.Kernel.Colors;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Atlas.DataAccess.Entity
{
     public class PackagingEventHelper : PdfPageEventHelper  
    {  
        PdfContentByte cb;  
        PdfTemplate template;
        PdfTemplate templateSepartor;
        BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

        public override void OnOpenDocument(PdfWriter writer, Document document)  
        {  
            cb = writer.DirectContent;  
            template = cb.CreateTemplate(50, 50);
            templateSepartor = cb.CreateTemplate(50, 50); 
        }
        public override void OnStartPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);
            Rectangle pageSize = document.PageSize;
            PdfPTable table = new PdfPTable(1);
            table.SetTotalWidth(new float[] { 800f });
            table.LockedWidth = (true);
            PdfPCell cell;
            Paragraph header;
            header = new Paragraph("Packaging",PDFUtil.font_heading_1);
            iTextSharp.text.pdf.draw.VerticalPositionMark seperator = new iTextSharp.text.pdf.draw.LineSeparator();
            seperator.Offset = -4f;
            header.Add(seperator);
            cell = new PdfPCell(header);
            cell.Border = Rectangle.NO_BORDER;
            table.AddCell(cell);
            table.WriteSelectedRows(0, -1, pageSize.GetLeft(15), pageSize.GetTop(15), writer.DirectContent);

        }
        public override void OnEndPage(PdfWriter writer, Document document)  
        {
            base.OnEndPage(writer, document);
            int pageN = writer.PageNumber;
            String text = "Page " + pageN + " of ";
            float len = bf.GetWidthPoint(text, 10);
            Rectangle pageSize = document.PageSize;
            cb.SetRGBColorFill(100, 100, 100);

            cb.BeginText();
            cb.SetFontAndSize(bf, 8);
            cb.SetTextMatrix(pageSize.GetLeft(15), pageSize.GetBottom(45));
            cb.ShowText("________________________________________________________________________________________"
                        +"__________________________________________________________________________________________");
            cb.EndText();
            cb.AddTemplate(template, pageSize.GetRight(98) + len, pageSize.GetBottom(30));

            cb.BeginText();
            cb.SetFontAndSize(bf, 10);
            cb.SetTextMatrix(pageSize.GetRight(100), pageSize.GetBottom(30));
            cb.ShowText(text);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(bf, 10);
            cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER,
                "Foreman: _________________________",
              pageSize.GetLeft(280),
              pageSize.GetBottom(30), 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(bf, 10);
            cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER,
                "Picker: _________________________",
              pageSize.GetLeft(560),
              pageSize.GetBottom(30), 0);
            cb.EndText();


            cb.BeginText();
            cb.SetFontAndSize(bf, 10);
            cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT,
                 DateTime.Now.ToString("dddd MMMMM dd, yyyy"),
                pageSize.GetLeft(130),
                pageSize.GetBottom(30), 0);
            cb.EndText();
        }  
  
        public override void OnCloseDocument(PdfWriter writer, Document document)  
        {  
            base.OnCloseDocument(writer, document);  
  
            template.BeginText();  
            template.SetFontAndSize(bf, 10);  
            template.SetTextMatrix(0, 0);  
            template.ShowText("" + (writer.PageNumber));  
            template.EndText();  
        }  
    }  
}