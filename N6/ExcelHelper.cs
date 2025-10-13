using ExcelDataReader;
using System;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;

public static class ExcelHelper
{
    public static DataTable ReadExcelFile(string filePath)
    {
        try
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                // SỬA LỖI: Khai báo trình đọc IExcelDataReader
                IExcelDataReader reader;

                // Lấy phần mở rộng của file để kiểm tra
                string extension = Path.GetExtension(filePath).ToLower();

                // Dựa vào đuôi file để chọn đúng trình đọc
                if (extension == ".csv")
                {
                    // Nếu là file .csv, dùng trình đọc CSV
                    reader = ExcelReaderFactory.CreateCsvReader(stream);
                }
                else
                {
                    // Nếu là file .xlsx hoặc .xls, dùng trình đọc Excel mặc định
                    reader = ExcelReaderFactory.CreateReader(stream);
                }

                // Dùng trình đọc đã được chọn
                using (reader)
                {
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true
                        }
                    });

                    if (result.Tables.Count > 0)
                    {
                        return result.Tables[0];
                    }
                }
            }
        }
        catch (Exception ex)
        {
            if (ex.Message.ToLower().Contains("invalid file signature"))
            {
                MessageBox.Show(
                    "Lỗi: File được chọn không phải là định dạng Excel hợp lệ (.xlsx, .xls).\n\n" +
                    "Vui lòng thử mở và lưu lại file bằng Microsoft Excel dưới dạng 'Excel Workbook (*.xlsx)' và thử lại.",
                    "Lỗi Định Dạng File",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("Lỗi không thể đọc file Excel: " + ex.Message, "Lỗi File", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        return null;
    }
}