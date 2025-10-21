using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Drawing.Layout; // SỬA LỖI FONT: Thêm thư viện này

namespace N6
{
    public static class ExportHelper
    {
        /// <summary>
        /// Xuất GridView ra file Excel (CSV format) sử dụng dấu chấm phẩy (;) làm phân cách.
        /// </summary>
        public static void ExportToExcel(DataGridView gridView, string fileName)
        {
            try
            {
                StringBuilder csvContent = new StringBuilder();

                // Thêm header từ GridView, sử dụng dấu ;
                string[] headers = new string[gridView.Columns.Count];
                for (int i = 0; i < gridView.Columns.Count; i++)
                {
                    // Bao bọc header trong dấu "" để xử lý các trường hợp đặc biệt
                    headers[i] = "\"" + gridView.Columns[i].HeaderText.Replace("\"", "\"\"") + "\"";
                }
                csvContent.AppendLine(string.Join(";", headers));

                // Thêm dữ liệu từ GridView, sử dụng dấu ;
                foreach (DataGridViewRow row in gridView.Rows)
                {
                    if (row.IsNewRow) continue;

                    string[] fields = new string[gridView.Columns.Count];
                    for (int i = 0; i < gridView.Columns.Count; i++)
                    {
                        string field = row.Cells[i].Value?.ToString() ?? "";
                        // Xử lý ký tự " bên trong dữ liệu và bao bọc toàn bộ bằng "
                        field = "\"" + field.Replace("\"", "\"\"") + "\"";
                        fields[i] = field;
                    }
                    csvContent.AppendLine(string.Join(";", fields));
                }

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
        /// Xuất GridView và các biểu đồ ra file PDF.
        /// </summary>
        public static void ExportToPDF(DataGridView gridView, string fileName, string title = "Báo cáo", params Image[] chartImages)
        {
            try
            {
                PdfDocument document = new PdfDocument();
                // Giữ lại việc xóa dấu cho metadata của file PDF cho an toàn
                document.Info.Title = RemoveVietnameseDiacritics(title);

                // --- TRANG 1: DỮ LIỆU BẢNG ---
                PdfPage page = document.AddPage();
                page.Size = PdfSharp.PageSize.A4;
                page.Orientation = PdfSharp.PageOrientation.Landscape;
                XGraphics gfx = XGraphics.FromPdfPage(page);

                // SỬA LỖI FONT: Thêm XPdfFontOptions để hỗ trợ Unicode (tiếng Việt)
                XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode);

                // SỬA LỖI FONT: Sử dụng font "Arial" (hoặc "Tahoma", "Times New Roman") với options Unicode
                XFont titleFont = new XFont("Arial", 16, XFontStyle.Bold, options);
                XFont headerFont = new XFont("Arial", 10, XFontStyle.Bold, options);
                XFont cellFont = new XFont("Arial", 9, XFontStyle.Regular, options);
                XFont infoFont = new XFont("Arial", 8, XFontStyle.Italic, options);

                double yPos = 30;
                double leftMargin = 30;
                double pageWidth = page.Width - 60;

                // SỬA LỖI FONT: Dùng XTextFormatter để vẽ title nhiều dòng và giữ tiếng Việt
                XTextFormatter tf = new XTextFormatter(gfx);
                XRect titleRect = new XRect(leftMargin, yPos, pageWidth, 100); // Khu vực vẽ title
                // Sử dụng title GỐC, không gọi RemoveVietnameseDiacritics
                tf.DrawString(title, titleFont, XBrushes.Black, titleRect, XStringFormats.TopLeft);

                // Cập nhật yPos dựa trên số dòng của title
                int lineCount = title.Split('\n').Length;
                yPos += (titleFont.GetHeight() * lineCount) + 20; // Thêm khoảng đệm
                // --- Kết thúc sửa lỗi title ---

                string dateInfo = $"Ngay xuat: {DateTime.Now:dd/MM/yyyy HH:mm}";
                XSize dateSize = gfx.MeasureString(dateInfo, infoFont);
                gfx.DrawString(dateInfo, infoFont, XBrushes.Gray, page.Width - leftMargin - dateSize.Width, yPos);
                yPos += 30;

                int colCount = gridView.Columns.Count;
                double colWidth = pageWidth / colCount;

                // VẼ HEADER
                double currentX = leftMargin;
                double headerHeight = 30;
                gfx.DrawRectangle(XBrushes.LightGray, leftMargin, yPos, pageWidth, headerHeight);
                gfx.DrawRectangle(XPens.Black, leftMargin, yPos, pageWidth, headerHeight);

                foreach (DataGridViewColumn col in gridView.Columns)
                {
                    // SỬA LỖI FONT: Bỏ RemoveVietnameseDiacritics, dùng HeaderText gốc
                    string headerText = col.HeaderText;
                    XRect headerRect = new XRect(currentX + 2, yPos + 2, colWidth - 4, headerHeight - 4);
                    gfx.DrawString(headerText, headerFont, XBrushes.Black, headerRect, XStringFormats.TopLeft);
                    if (currentX > leftMargin)
                        gfx.DrawLine(XPens.Black, currentX, yPos, currentX, yPos + headerHeight);
                    currentX += colWidth;
                }
                yPos += headerHeight;

                // VẼ DỮ LIỆU
                double rowHeight = 25;
                int rowCount = 0;
                foreach (DataGridViewRow row in gridView.Rows)
                {
                    if (row.IsNewRow) continue;

                    if (yPos + rowHeight > page.Height - 50) // Xử lý ngắt trang
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharp.PageOrientation.Landscape;
                        gfx = XGraphics.FromPdfPage(page);
                        yPos = 30;

                        currentX = leftMargin;
                        gfx.DrawRectangle(XBrushes.LightGray, leftMargin, yPos, pageWidth, headerHeight);
                        gfx.DrawRectangle(XPens.Black, leftMargin, yPos, pageWidth, headerHeight);
                        foreach (DataGridViewColumn col in gridView.Columns)
                        {
                            // SỬA LỖI FONT: Bỏ RemoveVietnameseDiacritics (cả ở phần ngắt trang)
                            string headerText = col.HeaderText;
                            XRect headerRect = new XRect(currentX + 2, yPos + 2, colWidth - 4, headerHeight - 4);
                            gfx.DrawString(headerText, headerFont, XBrushes.Black, headerRect, XStringFormats.TopLeft);
                            if (currentX > leftMargin)
                                gfx.DrawLine(XPens.Black, currentX, yPos, currentX, yPos + headerHeight);
                            currentX += colWidth;
                        }
                        yPos += headerHeight;
                    }

                    currentX = leftMargin;
                    if (rowCount % 2 == 0)
                        gfx.DrawRectangle(XBrushes.AliceBlue, leftMargin, yPos, pageWidth, rowHeight);
                    gfx.DrawRectangle(XPens.Gray, leftMargin, yPos, pageWidth, rowHeight);

                    for (int i = 0; i < gridView.Columns.Count; i++)
                    {
                        // SỬA LỖI FONT: Bỏ RemoveVietnameseDiacritics, dùng giá trị gốc
                        string cellText = row.Cells[i].Value?.ToString() ?? "";
                        XRect cellRect = new XRect(currentX + 2, yPos + 2, colWidth - 4, rowHeight - 4);
                        gfx.DrawString(cellText, cellFont, XBrushes.Black, cellRect, XStringFormats.TopLeft);
                        if (currentX > leftMargin)
                            gfx.DrawLine(XPens.Gray, currentX, yPos, currentX, yPos + rowHeight);
                        currentX += colWidth;
                    }
                    yPos += rowHeight;
                    rowCount++;
                }

                // --- THÊM TRANG MỚI CHO BIỂU ĐỒ ---
                if (chartImages != null && chartImages.Length > 0)
                {
                    PdfPage chartPage = document.AddPage();
                    chartPage.Orientation = PdfSharp.PageOrientation.Landscape;
                    XGraphics chartGfx = XGraphics.FromPdfPage(chartPage);
                    double chartYPos = 40;

                    foreach (Image chartImage in chartImages)
                    {
                        using (MemoryStream stream = new MemoryStream())
                        {
                            chartImage.Save(stream, ImageFormat.Png);
                            stream.Position = 0;
                            XImage xImage = XImage.FromStream(stream);

                            double ratio = xImage.PixelHeight / (double)xImage.PixelWidth;
                            double newWidth = chartPage.Width - 80;
                            double newHeight = newWidth * ratio;

                            if (chartYPos + newHeight > chartPage.Height - 40 && chartYPos > 40)
                            {
                                chartPage = document.AddPage();
                                chartPage.Orientation = PdfSharp.PageOrientation.Landscape;
                                chartGfx = XGraphics.FromPdfPage(chartPage);
                                chartYPos = 40;
                            }

                            chartGfx.DrawImage(xImage, 40, chartYPos, newWidth, newHeight);
                            chartYPos += newHeight + 20;
                        }
                    }
                }

                if (!fileName.ToLower().EndsWith(".pdf"))
                    fileName = Path.ChangeExtension(fileName, ".pdf");

                document.Save(fileName);
                document.Close();

                MessageBox.Show($"Đã xuất thành công file PDF: {fileName}", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                System.Diagnostics.Process.Start(fileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất PDF: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string RemoveVietnameseDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            string[] vietnameseChars = new string[]
            {
                "aAeEoOuUiIdDyY",
                "áàạảãâấầậẩẫăắằặẳẵ", "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
                "éèẹẻẽêếềệểễ", "ÉÈẸẺẼÊẾỀỆỂỄ",
                "óòọỏõôốồộổỗơớờợởỡ", "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
                "úùụủũưứừựửữ", "ÚÙỤỦŨƯỨỪỰỬỮ",
                "íìịỉĩ", "ÍÌỊỈĨ",
                "đ", "Đ",
                "ýỳỵỷỹ", "ÝỲỴỶỸ"
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