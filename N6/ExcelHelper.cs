using ExcelDataReader;
using System;
using System.Data;
using System.IO;
using System.Text;


public static class ExcelHelper
{
    /// <summary>
    /// Đọc file Excel (.xlsx, .xls) hoặc CSV (.csv) và trả về một DataTable.
    /// Ném (throws) ngoại lệ nếu file không hợp lệ hoặc không đọc được.
    /// </summary>
    /// <param name="filePath">Đường dẫn đến file cần đọc.</param>
    /// <returns>Một DataTable chứa dữ liệu từ sheet đầu tiên.</returns>
    /// <exception cref="InvalidDataException">Ném ra khi file không phải định dạng Excel/CSV hợp lệ.</exception>
    /// <exception cref="Exception">Ném ra cho các lỗi đọc file khác.</exception>
    public static DataTable ReadExcelFile(string filePath)
    {
        try
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                IExcelDataReader reader;
                string extension = Path.GetExtension(filePath).ToLower();

                if (extension == ".csv")
                {
                    reader = ExcelReaderFactory.CreateCsvReader(stream);
                }
                else
                {
                    reader = ExcelReaderFactory.CreateReader(stream);
                }

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
            // CHUẨN HÓA: Ném ngoại lệ thay vì hiển thị MessageBox
            if (ex.Message.ToLower().Contains("invalid file signature"))
            {
                // Ném lại lỗi với thông điệp rõ ràng cho UI
                throw new InvalidDataException(
                    "Lỗi: File được chọn không phải là định dạng Excel hợp lệ (.xlsx, .xls).\n\n" +
                    "Vui lòng thử mở và lưu lại file bằng Microsoft Excel dưới dạng 'Excel Workbook (*.xlsx)' và thử lại.",
                    ex);
            }
            else
            {
                // Ném lại các lỗi khác
                throw new Exception("Lỗi không thể đọc file Excel: " + ex.Message, ex);
            }
        }
        return null;
    }
}