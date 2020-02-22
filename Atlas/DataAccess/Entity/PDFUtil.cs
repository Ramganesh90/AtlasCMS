using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Atlas.DataAccess.Entity
{
    public static class PDFUtil
    {
        public static BaseColor LightBlue = new BaseColor(14, 45, 76);
        public static  BaseColor grey = new BaseColor(119, 119, 119);
        public static iTextSharp.text.Font font_heading_1 = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 17, new BaseColor(14, 45, 76));
        public static iTextSharp.text.Font font_heading_2 = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, new BaseColor(14, 45, 76));
        public static iTextSharp.text.Font font_body = FontFactory.GetFont(FontFactory.HELVETICA, 10, new BaseColor(14, 45, 76));
        public static iTextSharp.text.Font font_body_bold = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11, new BaseColor(14, 45, 76));
        public static iTextSharp.text.Font spanBold = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, new BaseColor(119, 119, 119));
        public static iTextSharp.text.Font spanNormal = FontFactory.GetFont(FontFactory.HELVETICA, 11, new BaseColor(119, 119, 119));
        public static iTextSharp.text.Font spanNormalBlack = FontFactory.GetFont(FontFactory.HELVETICA, 11, new BaseColor(0, 0, 0));

        public static PdfPCell HeaderCell(string text, Font font, int align = 0)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, font));
            cell.BorderColor = new BaseColor(119, 119, 119);
            cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER;
            cell.BorderWidthBottom = 3f;
            cell.BorderWidthTop = 3f;
            cell.PaddingBottom = 8f;
            cell.PaddingTop = 8f;
            cell.HorizontalAlignment = align;
            return cell;
        }

        public static Paragraph GetLineSeparator()
        {
            Paragraph LineSeparator = new Paragraph("   ");
            iTextSharp.text.pdf.draw.VerticalPositionMark seperator = new iTextSharp.text.pdf.draw.LineSeparator();
            seperator.Offset = -4f;
            LineSeparator.Add(seperator);
            return LineSeparator;
        }

        public static PdfPCell CreateCell(object text,Font fontStyle,int align = 0, bool hasBorder = false, BaseColor baseColor = null,bool padding =false)
        {
            var cell = new PdfPCell(new Phrase(Convert.ToString(text), fontStyle));
            if (hasBorder)
            {
                cell.Border = iTextSharp.text.Rectangle.BOX;
                if (baseColor == null)
                {
                    cell.BorderColor = new BaseColor(119, 119, 119);
                }
            }
            else { cell.Border = iTextSharp.text.Rectangle.NO_BORDER; }
            cell.HorizontalAlignment = align;
            if (!padding) {
                cell.Padding = 5 ;
            }
                return cell;
        }

        /// <summary>
        /// For Job Activation Headers
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static PdfPTable HeaderSection(string text)
        { var HeaderTable = new PdfPTable(1);
            HeaderTable.HorizontalAlignment = 0;
            HeaderTable.WidthPercentage = 98;
            HeaderTable.SpacingBefore = 10;
            HeaderTable.SpacingAfter = 5;
            HeaderTable.DefaultCell.Border = 0;
            HeaderTable.SetWidths(new float[] { 1 });

            PdfPCell cell = new PdfPCell(new Phrase(text, font_heading_2));
            cell.BorderColor = new BaseColor(119, 119, 119);
            cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER;
            cell.BorderWidthBottom = 1f;
            cell.BorderWidthTop = 1f;
            cell.PaddingBottom = 8f;
            cell.PaddingTop = 8f;
            cell.HorizontalAlignment = Rectangle.ALIGN_LEFT;

            HeaderTable.AddCell(cell);
            return HeaderTable;
           
        }
    }


}