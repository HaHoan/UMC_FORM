using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using UMC_FORM.Models.GA;

namespace UMC_FORM.Ultils
{
    public static class ExcelHelper
    {
        public static Stream CreateExcelFile(Stream stream = null, GA_LEAVE_FORM item = null)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var excelPackage = new ExcelPackage(stream ?? new MemoryStream()))
            {
                // Tạo author cho file Excel
                excelPackage.Workbook.Properties.Author = "Hanker";
                // Tạo title cho file Excel
                excelPackage.Workbook.Properties.Title = "EPP test background";
                // thêm tí comments vào làm màu 
                excelPackage.Workbook.Properties.Comments = "This is my fucking generated Comments";
                // Add Sheet vào file Excel
                excelPackage.Workbook.Worksheets.Add("First Sheet");
                // Lấy Sheet bạn vừa mới tạo ra để thao tác 
                var workSheet = excelPackage.Workbook.Worksheets[0];
                // Đổ data vào Excel file
                // workSheet.Cells[1, 1].LoadFromCollection(list, true, TableStyles.Dark9);
                BindingFormatForExcel(workSheet, item);
                excelPackage.Save();
                return excelPackage.Stream;
            }
        }

        private static int BindingFormatForExcel(ExcelWorksheet worksheet, GA_LEAVE_FORM ticket)
        {
            try
            {
                worksheet.Cells.Style.WrapText = true;
                worksheet.Cells["A1"].Value = "ĐĂNG KÝ NGHỈ CHO NHIỀU NHÂN VIÊN";
                worksheet.Cells["A1:H1"].Merge = true;
                worksheet.Cells["A1:H1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["B3"].Value = "Tháng";
                worksheet.Cells["B4"].Value = "Bộ phận";
                worksheet.Cells["C4"].Value = ticket.DEPT;
                worksheet.Cells["E3"].Value = "Loại ngày nghỉ : AL, SL, NP, AL3, KT, BH";
                worksheet.Cells["E3:H3"].Merge = true;
                worksheet.Cells["E4"].Value = "Nửa ngày : TRUE, FALSE";
                worksheet.Cells["E4:H4"].Merge = true;

                worksheet.Cells["A6"].Value = "STT";
                worksheet.Cells["B6"].Value = "Mã nhân viên";
                worksheet.Cells["C6"].Value = "Họ tên";
                worksheet.Cells["D6"].Value = "Bộ phận";
                worksheet.Cells["E6"].Value = "Ngày";
                worksheet.Cells["F6"].Value = "Loại ngày nghỉ";
                worksheet.Cells["G6"].Value = "Nửa ngày";
                worksheet.Cells["H6"].Value = "Lý do";

                using (var range = worksheet.Cells["A6:H6"])
                {
                    // Set PatternType
                    range.Style.Fill.PatternType = ExcelFillStyle.DarkGray;
                    // Set Màu cho Background
                    range.Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                    // Canh giữa cho các text
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.AutoFitColumns();

                }
                int i = 7;
                foreach (var item in ticket.GA_LEAVE_FORM_ITEMs)
                {
                    worksheet.Cells["A" + i].Value = (i-6).ToString();
                    worksheet.Cells["B" + i].Value = item.CODE;
                    worksheet.Cells["C" + i].Value = item.FULLNAME;
                    worksheet.Cells["D" + i].Value = ticket.DEPT;
                    worksheet.Cells["E" + i].Value = "";
                    worksheet.Cells["F" + i].Value = "";
                    worksheet.Cells["G" + i].Value = item.TOTAL < 1 ? "TRUE" : "FALSE";
                    worksheet.Cells["H" + i].Value = item.REASON;
                    i++;
                }

                worksheet.Cells["A6:H" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A6:H" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A6:H" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A6:H" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A6:H" + i].Style.Border.Top.Color.SetColor(Color.Black);
                worksheet.Cells["A6:H" + i].Style.Border.Bottom.Color.SetColor(Color.Black);
                worksheet.Cells["A6:H" + i].Style.Border.Left.Color.SetColor(Color.Black);
                worksheet.Cells["A6:H" + i].Style.Border.Right.Color.SetColor(Color.Black);
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message.ToString());
                return 0;
            }

        }
    }
}