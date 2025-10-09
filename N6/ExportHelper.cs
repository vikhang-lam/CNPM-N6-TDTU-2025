using System;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using PdfSharp.Pdf;
using PdfSharp.Drawing;

namespace N6
{
    public static class ExportHelper
    {
        /// <summary>
        /// Xuất GridView ra file Excel (CSV format)
        /// </summary>
        public static void ExportToExcel(DataGridView gridView, string fileName)
        {
            try
            {
                StringBuilder csvContent = new StringBuilder();

                // Thêm header từ GridView
                string[] headers = new string[gridView.Columns.Count];
                for (int i = 0; i < gridView.Columns.Count; i++)
                {
                    headers[i] = "\"" + gridView.Columns[i].HeaderText + "\"";
                }
                csvContent.AppendLine(string.Join(",", headers));

                // Thêm dữ liệu từ GridView
                foreach (DataGridViewRow row in gridView.Rows)
                {
                    if (row.IsNewRow) continue;
                    
                    string[] fields = new string[gridView.Columns.Count];
                    for (int i = 0; i < gridView.Columns.Count; i++)
                    {
                        string field = row.Cells[i].Value?.ToString() ?? "";
                        field = "\"" + field.Replace("\"", "\"\"") + "\"";
                        fields[i] = field;
                    }
                    csvContent.AppendLine(string.Join(",", fields));
                }

                // Đổi extension thành .csv
                if (Path.GetExtension(fileName).ToLower() == ".xlsx")
                {
                    fileName = Path.ChangeExtension(fileName, ".csv");
                }

                // Ghi file với UTF-8 BOM để Excel đọc đúng tiếng Việt
                byte[] csvBytes = Encoding.UTF8.GetBytes(csvContent.ToString());
                byte[] bom = Encoding.UTF8.GetPreamble();
                byte[] result = new byte[bom.Length + csvBytes.Length];
                Array.Copy(bom, 0, result, 0, bom.Length);
                Array.Copy(csvBytes, 0, result, bom.Length, csvBytes.Length);
                File.WriteAllBytes(fileName, result);

                MessageBox.Show($"Đã xuất thành công file Excel: {fileName}", 
                              "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất Excel: {ex.Message}", "Lỗi", 
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Xuất GridView ra file PDF (chuyển đổi sang không dấu để tránh lỗi font)
        /// </summary>
        public static void ExportToPDF(DataGridView gridView, string fileName, string title = "Báo cáo")
        {
            try
            {
                // Tạo document PDF
                PdfDocument document = new PdfDocument();
                document.Info.Title = RemoveVietnameseDiacritics(title);

                // Tạo trang
                PdfPage page = document.AddPage();
                page.Size = PdfSharp.PageSize.A4;
                page.Orientation = PdfSharp.PageOrientation.Landscape;

                XGraphics gfx = XGraphics.FromPdfPage(page);

                // Sử dụng font cơ bản
                XFont titleFont = new XFont("Arial", 16, XFontStyle.Bold);
                XFont headerFont = new XFont("Arial", 10, XFontStyle.Bold);
                XFont cellFont = new XFont("Arial", 9, XFontStyle.Regular);
                XFont infoFont = new XFont("Arial", 8, XFontStyle.Italic);

                double yPos = 30;
                double leftMargin = 30;
                double pageWidth = page.Width - 60;

                // Tiêu đề (chuyển sang không dấu)
                string cleanTitle = RemoveVietnameseDiacritics(title);
                XSize titleSize = gfx.MeasureString(cleanTitle, titleFont);
                gfx.DrawString(cleanTitle, titleFont, XBrushes.Black, 
                    leftMargin + (pageWidth - titleSize.Width) / 2, yPos);
                yPos += titleSize.Height + 20;

                // Ngày xuất
                string dateInfo = $"Ngay xuat: {DateTime.Now:dd/MM/yyyy HH:mm}";
                XSize dateSize = gfx.MeasureString(dateInfo, infoFont);
                gfx.DrawString(dateInfo, infoFont, XBrushes.Gray, 
                    page.Width - leftMargin - dateSize.Width, yPos);
                yPos += 30;

                // Tính độ rộng cột
                int colCount = gridView.Columns.Count;
                double colWidth = pageWidth / colCount;
                if (colWidth < 80) colWidth = 80;

                // Vẽ header
                double currentX = leftMargin;
                double headerHeight = 30;

                gfx.DrawRectangle(XBrushes.LightGray, leftMargin, yPos, pageWidth, headerHeight);
                gfx.DrawRectangle(XPens.Black, leftMargin, yPos, pageWidth, headerHeight);

                foreach (DataGridViewColumn col in gridView.Columns)
                {
                    string headerText = RemoveVietnameseDiacritics(col.HeaderText);
                    
                    // Cắt text nếu quá dài
                    if (gfx.MeasureString(headerText, headerFont).Width > colWidth - 4)
                    {
                        if (headerText.Length > 12)
                            headerText = headerText.Substring(0, 12) + "...";
                    }

                    XRect headerRect = new XRect(currentX + 2, yPos + 2, colWidth - 4, headerHeight - 4);
                    gfx.DrawString(headerText, headerFont, XBrushes.Black, 
                        headerRect, XStringFormats.Center);

                    if (currentX > leftMargin)
                        gfx.DrawLine(XPens.Black, currentX, yPos, currentX, yPos + headerHeight);

                    currentX += colWidth;
                }

                yPos += headerHeight;

                // Vẽ dữ liệu
                double rowHeight = 25;
                int rowCount = 0;

                foreach (DataGridViewRow row in gridView.Rows)
                {
                    if (row.IsNewRow) continue;

                    // Kiểm tra trang mới
                    if (yPos + rowHeight > page.Height - 50)
                    {
                        page = document.AddPage();
                        page.Size = PdfSharp.PageSize.A4;
                        page.Orientation = PdfSharp.PageOrientation.Landscape;
                        gfx = XGraphics.FromPdfPage(page);
                        yPos = 30;
                        
                        // Vẽ lại header ở trang mới
                        currentX = leftMargin;
                        gfx.DrawRectangle(XBrushes.LightGray, leftMargin, yPos, pageWidth, headerHeight);
                        gfx.DrawRectangle(XPens.Black, leftMargin, yPos, pageWidth, headerHeight);

                        foreach (DataGridViewColumn col in gridView.Columns)
                        {
                            string headerText = RemoveVietnameseDiacritics(col.HeaderText);
                            if (gfx.MeasureString(headerText, headerFont).Width > colWidth - 4)
                            {
                                if (headerText.Length > 12)
                                    headerText = headerText.Substring(0, 12) + "...";
                            }

                            XRect headerRect = new XRect(currentX + 2, yPos + 2, colWidth - 4, headerHeight - 4);
                            gfx.DrawString(headerText, headerFont, XBrushes.Black, 
                                headerRect, XStringFormats.Center);

                            if (currentX > leftMargin)
                                gfx.DrawLine(XPens.Black, currentX, yPos, currentX, yPos + headerHeight);

                            currentX += colWidth;
                        }
                        yPos += headerHeight;
                    }

                    currentX = leftMargin;

                    // Background cho dòng chẵn
                    if (rowCount % 2 == 0)
                        gfx.DrawRectangle(XBrushes.AliceBlue, leftMargin, yPos, pageWidth, rowHeight);

                    gfx.DrawRectangle(XPens.Gray, leftMargin, yPos, pageWidth, rowHeight);

                    // Vẽ cells
                    for (int i = 0; i < gridView.Columns.Count; i++)
                    {
                        string cellText = row.Cells[i].Value?.ToString() ?? "";
                        cellText = RemoveVietnameseDiacritics(cellText);
                        
                        // Cắt text nếu quá dài
                        if (gfx.MeasureString(cellText, cellFont).Width > colWidth - 4)
                        {
                            if (cellText.Length > 25)
                                cellText = cellText.Substring(0, 22) + "...";
                        }

                        XRect cellRect = new XRect(currentX + 2, yPos + 2, colWidth - 4, rowHeight - 4);
                        gfx.DrawString(cellText, cellFont, XBrushes.Black, 
                            cellRect, XStringFormats.CenterLeft);

                        if (currentX > leftMargin)
                            gfx.DrawLine(XPens.Gray, currentX, yPos, currentX, yPos + rowHeight);

                        currentX += colWidth;
                    }

                    yPos += rowHeight;
                    rowCount++;
                }

                // Tổng kết
                yPos += 20;
                gfx.DrawString($"Tong so ban ghi: {rowCount}", cellFont, XBrushes.Black, 
                    leftMargin, yPos);

                // Lưu file
                if (!fileName.ToLower().EndsWith(".pdf"))
                    fileName = Path.ChangeExtension(fileName, ".pdf");

                document.Save(fileName);
                document.Close();

                MessageBox.Show($"Đã xuất thành công file PDF: {fileName}", 
                              "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Mở file
                try
                {
                    System.Diagnostics.Process.Start(fileName);
                }
                catch { }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất PDF: {ex.Message}", "Lỗi", 
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Chuyển đổi ký tự tiếng Việt có dấu thành không dấu
        /// </summary>
        private static string RemoveVietnameseDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            string[] vietnameseChars = new string[]
            {
                "aAeEoOuUiIdDyY",
                "áàạảãâấầậẩẫăắằặẳẵ",
                "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
                "éèẹẻẽêếềệểễ",
                "ÉÈẸẺẼÊẾỀỆỂỄ",
                "óòọỏõôốồộổỗơớờợởỡ",
                "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
                "úùụủũưứừựửữ",
                "ÚÙỤỦŨƯỨỪỰỬỮ",
                "íìịỉĩ",
                "ÍÌỊỈĨ",
                "đ",
                "Đ",
                "ýỳỵỷỹ",
                "ÝỲỴỶỸ"
            };

            string result = text;
            for (int i = 1; i < vietnameseChars.Length; i++)
            {
                for (int j = 0; j < vietnameseChars[i].Length; j++)
                {
                    result = result.Replace(vietnameseChars[i][j], vietnameseChars[0][i - 1]);
                }
            }

            return result;
        }
    }
}