using System;
using System.Collections.Generic; // Added for List
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq; // Added for LINQ operations
using System.Text;
using System.Windows.Forms;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout; // Ensure this is included
using PdfSharp.Pdf;


namespace N6 // Make sure this namespace matches your project
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

                // Get visible columns only
                var visibleColumns = gridView.Columns.Cast<DataGridViewColumn>()
                                         .Where(col => col.Visible)
                                         .ToList();

                // Add header from visible GridView columns, using ;
                string[] headers = new string[visibleColumns.Count];
                for (int i = 0; i < visibleColumns.Count; i++)
                {
                    // Bao bọc header trong dấu "" để xử lý các trường hợp đặc biệt (dấu phẩy, ngoặc kép, xuống dòng)
                    headers[i] = "\"" + visibleColumns[i].HeaderText.Replace("\"", "\"\"") + "\"";
                }
                csvContent.AppendLine(string.Join(";", headers));

                // Add data from visible GridView columns, using ;
                foreach (DataGridViewRow row in gridView.Rows)
                {
                    if (row.IsNewRow) continue;

                    string[] fields = new string[visibleColumns.Count];
                    for (int i = 0; i < visibleColumns.Count; i++)
                    {
                        var cell = row.Cells[visibleColumns[i].Name]; // Access cell by column name
                        string field = cell.FormattedValue?.ToString() ?? ""; // Use FormattedValue for display consistency

                        // Xử lý ký tự " bên trong dữ liệu và bao bọc toàn bộ bằng "
                        field = "\"" + field.Replace("\"", "\"\"") + "\"";
                        fields[i] = field;
                    }
                    csvContent.AppendLine(string.Join(";", fields));
                }

                // Ensure the file extension is .csv as we are creating a CSV file
                if (!fileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    fileName = Path.ChangeExtension(fileName, ".csv");
                }

                // Ghi file với UTF-8 BOM để Excel đọc đúng tiếng Việt
                File.WriteAllText(fileName, csvContent.ToString(), Encoding.UTF8); // Simpler write with BOM for UTF8

                // MessageBox is handled by the calling code in UC_BaoCao_Admin
                // MessageBox.Show($"Đã xuất thành công file Excel: {fileName}", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // Throw exception to be caught by the calling code (UC_BaoCao_Admin)
                throw new Exception($"Lỗi khi xuất file CSV: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Xuất GridView và các biểu đồ/ảnh ra file PDF, có thêm tiêu đề lớn trong nội dung.
        /// </summary>
        /// <param name="gridView">DataGridView chính chứa dữ liệu.</param>
        /// <param name="fileName">Đường dẫn file PDF để lưu.</param>
        /// <param name="documentTitleAndFilters">Tiêu đề chung + bộ lọc (cho metadata và phần đầu trang).</param>
        /// <param name="mainContentHeader">Tiêu đề lớn sẽ hiển thị phía trên bảng dữ liệu.</param>
        /// <param name="imagesToExport">Mảng các ảnh (biểu đồ, lưới phụ) để thêm vào trang sau.</param>
        public static void ExportToPDF(DataGridView gridView, string fileName, string documentTitleAndFilters, string mainContentHeader, params Image[] imagesToExport)
        {
            try
            {
                PdfDocument document = new PdfDocument();
                document.Info.Title = RemoveVietnameseDiacritics(documentTitleAndFilters.Split('\n')[0]);

                PdfPage page = document.AddPage();
                page.Size = PdfSharp.PageSize.A4;
                page.Orientation = PdfSharp.PageOrientation.Landscape;
                XGraphics gfx = XGraphics.FromPdfPage(page);

                XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode);
                XFont docTitleFont = new XFont("Arial", 14, XFontStyle.Bold, options);
                XFont largeHeaderFont = new XFont("Arial", 24, XFontStyle.Bold, options);
                XFont gridHeaderFont = new XFont("Arial", 10, XFontStyle.Bold, options);
                XFont classListFont = new XFont("Arial", 10, XFontStyle.Regular, options);
                XFont cellFont = new XFont("Arial", 9, XFontStyle.Regular, options);
                XFont infoFont = new XFont("Arial", 8, XFontStyle.Italic, options);

                double yPos = 30;
                double leftMargin = 30;
                double rightMargin = 30;
                double contentWidth = page.Width - leftMargin - rightMargin;

                // 1️⃣ Tiêu đề và bộ lọc (nếu có)
                XTextFormatter tf = new XTextFormatter(gfx);
                if (!string.IsNullOrEmpty(documentTitleAndFilters))
                {
                    XRect docTitleRect = new XRect(leftMargin, yPos, contentWidth, 100);
                    tf.DrawString(documentTitleAndFilters, docTitleFont, XBrushes.Black, docTitleRect, XStringFormats.TopLeft);
                    int titleLineCount = documentTitleAndFilters.Split('\n').Length;
                    yPos += (docTitleFont.GetHeight() * titleLineCount) + 10;
                }

                // 2️⃣ Ngày xuất
                string dateInfo = $"Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm}";
                XSize dateSize = gfx.MeasureString(dateInfo, infoFont);
                gfx.DrawString(dateInfo, infoFont, XBrushes.Gray, page.Width - rightMargin - dateSize.Width, yPos);
                yPos += dateSize.Height + 15;

                // --- KIỂM TRA ĐỊNH DẠNG BÁO CÁO ---
                bool isCombinedKhoiReport = gridView.Columns.Contains("IsHeader");

                // *** SỬA LỖI: Kiểm tra xem đây là báo cáo Khối hay Lớp ***
                // Dựa vào tiêu đề chính được truyền từ UC_BaoCao_Admin
                bool isKhoiReportPDF = mainContentHeader.ToUpper().Contains("KHỐI");

                if (isCombinedKhoiReport)
                {
                    // === PATH A: VẼ BÁO CÁO GỘP (KHỐI hoặc LỚP) ===

                    // 1. Định nghĩa cột
                    string[] colDataNames = { "PhanLoai", "Col_TS", "Col_Nu", "Col_DanToc_Percent", "Col_NDT" };
                    double[] colWidths = {
                        contentWidth * 0.30, // PhanLoai
                        contentWidth * 0.175, // Col_TS
                        contentWidth * 0.175, // Col_Nu
                        contentWidth * 0.175, // Col_DanToc_Percent
                        contentWidth * 0.175  // Col_NDT
                    };
                    double rowHeight = 25;
                    double mainHeaderRowHeight = 35;
                    double subHeaderRowHeight = 25;

                    // 2. Vẽ Hàng 1 (Tên Khối hoặc Tên Lớp)
                    XRect headerRect = new XRect(leftMargin, yPos, contentWidth, mainHeaderRowHeight);
                    gfx.DrawRectangle(XBrushes.Gainsboro, headerRect);
                    gfx.DrawRectangle(XPens.Black, headerRect);
                    gfx.DrawString(mainContentHeader, largeHeaderFont, XBrushes.Black, headerRect, XStringFormats.Center);
                    yPos += mainHeaderRowHeight;

                    // 3. Vẽ Hàng 2 (Chỉ vẽ nếu là Báo cáo Khối và có Tag)
                    if (isKhoiReportPDF && gridView.Tag is string tagValue && !string.IsNullOrWhiteSpace(tagValue))
                    {
                        string classList = tagValue;
                        XRect subHeaderRect = new XRect(leftMargin, yPos, contentWidth, subHeaderRowHeight);
                        gfx.DrawRectangle(XBrushes.Gainsboro, subHeaderRect);
                        gfx.DrawRectangle(XPens.Black, subHeaderRect);
                        gfx.DrawString(classList, classListFont, XBrushes.Black, subHeaderRect, XStringFormats.Center);
                        yPos += subHeaderRowHeight;
                    }

                    // 4. Vẽ các hàng dữ liệu (bắt đầu từ hàng "ĐIỂM")
                    foreach (DataGridViewRow row in gridView.Rows)
                    {
                        if (row.IsNewRow) continue;
                        if (yPos + rowHeight > page.Height - 40) break;

                        bool isHeaderRow = Convert.ToInt32(row.Cells["IsHeader"].Value) == 1;
                        bool isXepLoaiHeader = isHeaderRow && (row.Cells["PhanLoai"].Value?.ToString() == "XẾP LOẠI");

                        XFont font = isHeaderRow ? gridHeaderFont : cellFont;
                        XBrush bgBrush = isHeaderRow ? XBrushes.Gainsboro : XBrushes.White;

                        gfx.DrawRectangle(bgBrush, leftMargin, yPos, contentWidth, rowHeight);
                        gfx.DrawRectangle(XPens.Gray, leftMargin, yPos, contentWidth, rowHeight);

                        double currentX = leftMargin;
                        for (int i = 0; i < colDataNames.Length; i++)
                        {
                            string cellText = row.Cells[colDataNames[i]].Value?.ToString() ?? "";
                            XStringFormat alignment = XStringFormats.Center;

                            if (i == 0) // Cột đầu tiên
                            {
                                alignment = isHeaderRow ? XStringFormats.CenterLeft : XStringFormats.Center;
                            }

                            // *** SỬA LỖI: CHỈ ÁP DỤNG LOGIC "%" NẾU LÀ BÁO CÁO KHỐI ***
                            if (isKhoiReportPDF && isXepLoaiHeader) // Text đặc biệt cho hàng "XẾP LOẠI" (Báo cáo Khối)
                            {
                                if (i == 2) cellText = "";
                                if (i == 3) cellText = "%";
                                if (i == 4) cellText = "";
                            }
                            // (Nếu là Báo cáo Lớp, cellText sẽ giữ nguyên giá trị từ DGV là "Nữ", "Dân tộc"...)

                            XRect cellRect = new XRect(currentX + 5, yPos, colWidths[i] - 10, rowHeight);
                            gfx.DrawString(cellText, font, XBrushes.Black, cellRect, alignment);

                            // Vẽ đường kẻ cột
                            if (i > 0)
                                gfx.DrawLine(XPens.Gray, currentX, yPos, currentX, yPos + rowHeight);

                            if (i == 0 && isHeaderRow)
                            {
                                gfx.DrawLine(XPens.Gray, currentX + colWidths[i], yPos, currentX + colWidths[i], yPos + rowHeight);
                            }

                            currentX += colWidths[i];
                        }
                        yPos += rowHeight;
                    }
                }
                else
                {
                    // === PATH B: VẼ BÁO CÁO LỚP (CŨ) HOẶC CÁC BÁO CÁO KHÁC ===
                    // (Logic này giữ nguyên, không thay đổi)

                    // 3️⃣ Header lớn (Lớp)
                    if (!string.IsNullOrEmpty(mainContentHeader))
                    {
                        XRect largeHeaderRect = new XRect(leftMargin, yPos, contentWidth, 50);
                        gfx.DrawString(mainContentHeader, largeHeaderFont, XBrushes.Black, largeHeaderRect, XStringFormats.TopCenter);
                        yPos += largeHeaderFont.GetHeight() + 15;
                    }

                    // 4️⃣ Vẽ DataGridView chính (Bảng điểm)
                    var visibleColumns = gridView.Columns.Cast<DataGridViewColumn>()
                                             .Where(col => col.Visible)
                                             .ToList();
                    if (visibleColumns.Count == 0)
                        throw new Exception("Không có cột nào hiển thị trong DataGridView.");

                    double colWidth = contentWidth / visibleColumns.Count;
                    double gridHeaderHeight = 30;
                    double currentX = leftMargin;

                    // Header cột
                    gfx.DrawRectangle(XBrushes.LightGray, leftMargin, yPos, contentWidth, gridHeaderHeight);
                    gfx.DrawRectangle(XPens.Black, leftMargin, yPos, contentWidth, gridHeaderHeight);
                    foreach (var col in visibleColumns)
                    {
                        XRect headerRect = new XRect(currentX + 3, yPos + 5, colWidth - 6, gridHeaderHeight - 7);
                        tf.DrawString(col.HeaderText, gridHeaderFont, XBrushes.Black, headerRect, XStringFormats.TopLeft);
                        if (currentX > leftMargin)
                            gfx.DrawLine(XPens.Black, currentX, yPos, currentX, yPos + gridHeaderHeight);
                        currentX += colWidth;
                    }
                    yPos += gridHeaderHeight;

                    // Hàng dữ liệu
                    double rowHeight = 25;
                    int rowCountOnPage = 0;
                    foreach (DataGridViewRow row in gridView.Rows)
                    {
                        if (row.IsNewRow) continue;
                        if (yPos + rowHeight > page.Height - 200) break;

                        currentX = leftMargin;
                        XBrush rowBrush = (rowCountOnPage % 2 == 0) ? XBrushes.White : XBrushes.AliceBlue;
                        gfx.DrawRectangle(rowBrush, leftMargin, yPos, contentWidth, rowHeight);
                        gfx.DrawRectangle(XPens.Gray, leftMargin, yPos, contentWidth, rowHeight);

                        for (int i = 0; i < visibleColumns.Count; i++)
                        {
                            var cell = row.Cells[visibleColumns[i].Name];
                            string cellText = cell.FormattedValue?.ToString() ?? "";
                            XRect cellRect = new XRect(currentX + 3, yPos + 5, colWidth - 6, rowHeight - 7);
                            tf.DrawString(cellText, cellFont, XBrushes.Black, cellRect, XStringFormats.TopLeft);
                            if (currentX > leftMargin)
                                gfx.DrawLine(XPens.Gray, currentX, yPos, currentX, yPos + rowHeight);
                            currentX += colWidth;
                        }
                        yPos += rowHeight;
                        rowCountOnPage++;
                    }

                    // 6️⃣ Vẽ tiếp bảng phụ (Bảng xếp loại) ngay dưới
                    if (imagesToExport != null && imagesToExport.Length > 0 && imagesToExport[0] != null)
                    {
                        using (MemoryStream stream = new MemoryStream())
                        {
                            imagesToExport[0].Save(stream, ImageFormat.Png);
                            stream.Position = 0;
                            XImage xImage = XImage.FromStream(stream);

                            double availableWidth = page.Width - (2 * leftMargin);
                            double imgWidth = availableWidth;
                            double ratio = xImage.PixelHeight / (double)xImage.PixelWidth;
                            double imgHeight = imgWidth * ratio;
                            double availableHeight = page.Height - yPos - 40;

                            if (imgHeight > availableHeight)
                            {
                                imgHeight = availableHeight;
                                imgWidth = imgHeight / ratio;
                            }

                            double imgX = (page.Width - imgWidth) / 2;
                            gfx.DrawImage(xImage, imgX, yPos + 20, imgWidth, imgHeight);
                        }
                    }
                }

                // 7️⃣ Lưu file
                if (!fileName.ToLower().EndsWith(".pdf"))
                    fileName = Path.ChangeExtension(fileName, ".pdf");
                document.Save(fileName);
                document.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating PDF file: {ex.Message}", ex);
            }
        }


        /// <summary>
        /// Removes Vietnamese diacritics from a string.
        /// </summary>
        private static string RemoveVietnameseDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            // Using Normalize to decompose characters and remove non-spacing marks
            string normalizedString = text.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            foreach (char c in normalizedString)
            {
                var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    // Special case for Đ/đ
                    if (c == 'Đ') stringBuilder.Append('D');
                    else if (c == 'đ') stringBuilder.Append('d');
                    else stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}