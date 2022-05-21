using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using UMC_FORM.Business;
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
              
                worksheet.Cells["A1"].Value = ticket.TITLE.ToUpper();
                worksheet.Cells["A1:I1"].Style.Font.Size = 16;
                worksheet.Cells["A1:I1"].Style.Font.Bold = true;
                worksheet.Cells["A1:I1"].Merge = true;
                worksheet.Cells["A1:I1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["B2"].Value = "Bộ phận đăng ký:" + ticket.DEPT;
                worksheet.Cells["B2:I2"].Style.Font.Bold = true;
                worksheet.Cells["B2:I2"].Merge = true;
                worksheet.Cells["B3"].Value = "Ngày đăng ký:" + ticket.DATE_REGISTER.ToString("dd/MM/yyyy");
                worksheet.Cells["B3:I3"].Merge = true;
                worksheet.Cells["B3:I3"].Style.Font.Bold = true;
                worksheet.Cells["B4"].Value = "Số người đăng ký:" + ticket.NUMBER_REGISTER;
                worksheet.Cells["B4:I4"].Merge = true;
                worksheet.Cells["B4:I4"].Style.Font.Bold = true;
                worksheet.Cells["A5"].Value = "STT";
                worksheet.Cells["A5"].Style.Font.Bold = true;
                worksheet.Cells["B5"].Value = "Họ tên";
                worksheet.Cells["B5"].Style.Font.Bold = true;
                worksheet.Cells["C5"].Value = "Mã nhân viên";
                worksheet.Cells["C5"].Style.Font.Bold = true;
                worksheet.Cells["D5"].Value = "From";
                worksheet.Cells["D5"].Style.Font.Bold = true;
                worksheet.Cells["E5"].Value = "To";
                worksheet.Cells["E5"].Style.Font.Bold = true;
                worksheet.Cells["F5"].Value = "Tổng";
                worksheet.Cells["F5"].Style.Font.Bold = true;
                worksheet.Cells["G5"].Value = "Lý do nghỉ";
                worksheet.Cells["G5"].Style.Font.Bold = true;
                worksheet.Cells["H5"].Value = "Rmks";
                worksheet.Cells["H5"].Style.Font.Bold = true;

                if (ticket.FORM_NAME == Constant.GA_PAID_LEAVE_ID)
                {
                    worksheet.Cells["I5"].Value = "Nghỉ đặc biệt";
                    worksheet.Cells["I5"].Style.Font.Bold = true;
                }
              
                using (var range = worksheet.Cells["A5:I5"])
                {
                    // Set PatternType
                    //range.Style.Fill.PatternType = ExcelFillStyle.DarkGray;
                    // Set Màu cho Background
                    //range.Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                    // Canh giữa cho các text
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    //range.AutoFitColumns();

                }
                int i = 6;
                foreach (var item in ticket.GA_LEAVE_FORM_ITEMs)
                {
                    worksheet.Cells["A" + i].Value = (i-5).ToString();
                    worksheet.Cells["B" + i].Value = item.FULLNAME;
                    worksheet.Cells["C" + i].Value = item.CODE;
                    worksheet.Cells["C" + i].Style.Numberformat.Format = "####0";
                    worksheet.Cells["D" + i].Value = item.TIME_FROM;
                    worksheet.Cells["D" + i].Style.Numberformat.Format = "dd/MM/yyyy";
                    worksheet.Cells["E" + i].Value = item.TIME_TO.ToString("dd/MM/yyyy");
                    worksheet.Cells["E" + i].Style.Numberformat.Format = "dd/MM/yyyy";
                    worksheet.Cells["F" + i].Value = item.TOTAL;
                    worksheet.Cells["G" + i].Value = item.REASON;
                    worksheet.Cells["H" + i].Value = item.REMARK;
                    if(ticket.FORM_NAME == Constant.GA_PAID_LEAVE_ID && item.SPEACIAL_LEAVE)
                    {
                        worksheet.Cells["I" + i].Value = "√";
                    }
                    worksheet.Cells["A" + i+":I" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A" + i + ":I" + i].AutoFitColumns();
                    i++;
                }

                worksheet.Cells["A5:I" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A5:I" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A5:I" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A5:I" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A5:I" + i].Style.Border.Top.Color.SetColor(Color.Black);
                worksheet.Cells["A5:I" + i].Style.Border.Bottom.Color.SetColor(Color.Black);
                worksheet.Cells["A5:I" + i].Style.Border.Left.Color.SetColor(Color.Black);
                worksheet.Cells["A5:I" + i].Style.Border.Right.Color.SetColor(Color.Black);
                worksheet.Column(1).Width = 5;
                worksheet.Column(2).Width = 20;
                worksheet.Column(3).Width = 10;
                worksheet.Column(4).Width = 15;
                worksheet.Column(5).Width = 15;
                worksheet.Column(6).Width = 5;
                worksheet.Column(7).Width = 30;
                worksheet.Column(8).Width = 15;
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