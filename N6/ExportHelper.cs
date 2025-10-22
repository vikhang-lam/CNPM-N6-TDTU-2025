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
                document.Info.Title = RemoveVietnameseDiacritics(documentTitleAndFilters.Split('\n')[0]); // Use first line for metadata

                // --- PAGE 1: LARGE HEADER + DATA GRID ---
                PdfPage page = document.AddPage();
                page.Size = PdfSharp.PageSize.A4;
                page.Orientation = PdfSharp.PageOrientation.Landscape; // Landscape orientation
                XGraphics gfx = XGraphics.FromPdfPage(page);

                XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode); // For Vietnamese characters

                // Define fonts
                XFont docTitleFont = new XFont("Arial", 14, XFontStyle.Bold, options);
                XFont largeHeaderFont = new XFont("Arial", 24, XFontStyle.Bold, options); // Font for the main content header
                XFont gridHeaderFont = new XFont("Arial", 10, XFontStyle.Bold, options);
                XFont cellFont = new XFont("Arial", 9, XFontStyle.Regular, options);
                XFont infoFont = new XFont("Arial", 8, XFontStyle.Italic, options);

                double yPos = 30;
                double leftMargin = 30;
                double rightMargin = 30;
                double contentWidth = page.Width - leftMargin - rightMargin;

                // 1. Draw Document Title + Filters
                XTextFormatter tf = new XTextFormatter(gfx);
                XRect docTitleRect = new XRect(leftMargin, yPos, contentWidth, 100); // Area for title
                tf.DrawString(documentTitleAndFilters, docTitleFont, XBrushes.Black, docTitleRect, XStringFormats.TopLeft);
                int titleLineCount = documentTitleAndFilters.Split('\n').Length;
                yPos += (docTitleFont.GetHeight() * titleLineCount) + 10; // Adjust spacing

                // 2. Draw Export Date
                string dateInfo = $"Ngay xuat: {DateTime.Now:dd/MM/yyyy HH:mm}";
                XSize dateSize = gfx.MeasureString(dateInfo, infoFont);
                gfx.DrawString(dateInfo, infoFont, XBrushes.Gray, page.Width - rightMargin - dateSize.Width, yPos);
                yPos += dateSize.Height + 15;

                // 3. Draw Large Content Header (Centered)
                if (!string.IsNullOrEmpty(mainContentHeader))
                {
                    XRect largeHeaderRect = new XRect(leftMargin, yPos, contentWidth, 50); // Area for large header
                    gfx.DrawString(mainContentHeader, largeHeaderFont, XBrushes.Black, largeHeaderRect, XStringFormats.TopCenter); // Center align
                    yPos += largeHeaderFont.GetHeight() + 20; // Add significant space after
                }

                // 4. Draw DataGridView
                var visibleColumns = gridView.Columns.Cast<DataGridViewColumn>()
                                         .Where(col => col.Visible)
                                         .ToList();
                int colCount = visibleColumns.Count;
                if (colCount == 0) throw new Exception("Không có cột nào hiển thị trong DataGridView để xuất.");

                double colWidth = contentWidth / colCount;
                double gridHeaderHeight = 30; // Height for the header row

                // --- DRAW GRID HEADER ---
                Action drawGridHeader = () => {
                    double currentXHeader = leftMargin;
                    gfx.DrawRectangle(XBrushes.LightGray, leftMargin, yPos, contentWidth, gridHeaderHeight); // Background
                    gfx.DrawRectangle(XPens.Black, leftMargin, yPos, contentWidth, gridHeaderHeight);       // Border

                    foreach (var col in visibleColumns)
                    {
                        string headerText = col.HeaderText;
                        XRect headerRect = new XRect(currentXHeader + 3, yPos + 5, colWidth - 6, gridHeaderHeight - 7); // Padding
                        tf.DrawString(headerText, gridHeaderFont, XBrushes.Black, headerRect, XStringFormats.TopLeft); // Use TextFormatter for wrapping
                        // Draw vertical line
                        if (currentXHeader > leftMargin)
                            gfx.DrawLine(XPens.Black, currentXHeader, yPos, currentXHeader, yPos + gridHeaderHeight);
                        currentXHeader += colWidth;
                    }
                    yPos += gridHeaderHeight;
                };

                drawGridHeader(); // Draw header initially

                // --- DRAW GRID DATA ROWS ---
                double rowHeight = 25;
                int rowCountOnPage = 0;
                foreach (DataGridViewRow row in gridView.Rows)
                {
                    if (row.IsNewRow) continue;

                    // Page Break Check
                    if (yPos + rowHeight > page.Height - 50) // Check if row fits before drawing
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharp.PageOrientation.Landscape;
                        gfx = XGraphics.FromPdfPage(page);
                        tf = new XTextFormatter(gfx); // Update TextFormatter for new page
                        yPos = 30; // Reset Y position
                        rowCountOnPage = 0;
                        drawGridHeader(); // Redraw header on new page
                    }

                    double currentXRow = leftMargin;
                    // Alternating Row Color
                    XBrush rowBrush = (rowCountOnPage % 2 == 0) ? XBrushes.White : XBrushes.AliceBlue;
                    gfx.DrawRectangle(rowBrush, leftMargin, yPos, contentWidth, rowHeight);
                    // Row Border
                    gfx.DrawRectangle(XPens.Gray, leftMargin, yPos, contentWidth, rowHeight);

                    for (int i = 0; i < visibleColumns.Count; i++)
                    {
                        var cell = row.Cells[visibleColumns[i].Name];
                        string cellText = cell.FormattedValue?.ToString() ?? "";
                        XRect cellRect = new XRect(currentXRow + 3, yPos + 5, colWidth - 6, rowHeight - 7); // Padding
                        tf.DrawString(cellText, cellFont, XBrushes.Black, cellRect, XStringFormats.TopLeft); // Use TextFormatter

                        // Draw vertical line
                        if (currentXRow > leftMargin)
                            gfx.DrawLine(XPens.Gray, currentXRow, yPos, currentXRow, yPos + rowHeight);
                        currentXRow += colWidth;
                    }
                    yPos += rowHeight;
                    rowCountOnPage++;
                }

                // --- ADD NEW PAGES FOR IMAGES (Charts, Secondary Grids) ---
                if (imagesToExport != null && imagesToExport.Length > 0)
                {
                    PdfPage imagePage = null; // Initialize later
                    XGraphics imgGfx = null;
                    double imgYPos = 0;
                    bool firstImage = true;

                    foreach (Image img in imagesToExport)
                    {
                        if (img == null) continue;

                        using (MemoryStream stream = new MemoryStream())
                        {
                            try
                            {
                                img.Save(stream, ImageFormat.Png); // Save as PNG
                                stream.Position = 0;
                                XImage xImage = XImage.FromStream(stream);

                                // Calculate image dimensions preserving aspect ratio
                                double ratio = xImage.PixelHeight / (double)xImage.PixelWidth;
                                double availableWidth = page.Width - (2 * leftMargin); // Use page width with margins
                                double imgWidth = availableWidth;
                                double imgHeight = imgWidth * ratio;

                                // Scale down if too high for the page
                                double availableHeight = page.Height - 60; // Top/Bottom margin
                                if (imgHeight > availableHeight)
                                {
                                    imgHeight = availableHeight;
                                    imgWidth = imgHeight / ratio;
                                }

                                // Check if a new page is needed
                                if (firstImage || (imgYPos + imgHeight > page.Height - 40))
                                {
                                    imagePage = document.AddPage();
                                    imagePage.Orientation = PdfSharp.PageOrientation.Landscape;
                                    imgGfx = XGraphics.FromPdfPage(imagePage);
                                    imgYPos = 40; // Reset Y for new page
                                    firstImage = false;
                                }

                                // Draw image centered horizontally
                                double imgX = (imagePage.Width - imgWidth) / 2;
                                imgGfx.DrawImage(xImage, imgX, imgYPos, imgWidth, imgHeight);
                                imgYPos += imgHeight + 20; // Space between images

                            }
                            catch (Exception imgEx)
                            {
                                Console.WriteLine($"Error processing image for PDF export: {imgEx.Message}");
                                // Skip problematic image
                            }
                        }
                    }
                }
                // --- END ADDING IMAGES ---

                // Save the document
                if (!fileName.ToLower().EndsWith(".pdf"))
                    fileName = Path.ChangeExtension(fileName, ".pdf");
                document.Save(fileName);
                document.Close();

                // MessageBox and Process.Start are handled by the calling code (UC_BaoCao_Admin)

            }
            catch (Exception ex)
            {
                // Throw exception to be caught by the calling code (UC_BaoCao_Admin)
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