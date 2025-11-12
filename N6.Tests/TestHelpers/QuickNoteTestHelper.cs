using System.Data;

namespace N6.Tests.TestHelpers
{
    /// <summary>
    /// Cung cấp dữ liệu DataTable giả cho việc kiểm thử UC35 (frmGhiChuHocSinh).
    /// </summary>
    public static class QuickNoteTestHelper
    {
        /// <summary>
        /// Mô phỏng kết quả từ DatabaseHelper.GetClassesByTeacher(_maGV).
        /// </summary>
        public static DataTable CreateMockClassesData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaLop", typeof(string));
            dt.Columns.Add("TenLop", typeof(string));
            dt.Rows.Add("L5A", "Lớp 5A");
            dt.Rows.Add("L5B", "Lớp 5B");
            return dt;
        }

        /// <summary>
        /// Mô phỏng kết quả từ DatabaseHelper.GetSubjectsByTeacherAndClass(...).
        /// </summary>
        public static DataTable CreateMockSubjectsData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaMon", typeof(string));
            dt.Columns.Add("TenMon", typeof(string));
            dt.Rows.Add("TOAN", "Toán");
            dt.Rows.Add("TV", "Tiếng Việt");
            return dt;
        }

        /// <summary>
        /// Mô phỏng kết quả từ DatabaseHelper.GetStudentsByClass(...).
        /// </summary>
        public static DataTable CreateMockStudentsData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaHS", typeof(string));
            dt.Columns.Add("HoTen", typeof(string));
            dt.Rows.Add("HS001", "Nguyễn Văn A");
            dt.Rows.Add("HS002", "Trần Thị B");
            return dt;
        }

        /// <summary>
        /// Dữ liệu đầu vào hợp lệ để lưu.
        /// </summary>
        public static (string maHS, string maMon, string ghiChu) CreateValidNoteInput()
        {
            return ("HS001", "TOAN", "Em A có tiến bộ trong môn Toán.");
        }

        /// <summary>
        /// Dữ liệu đầu vào không hợp lệ (ghi chú rỗng).
        /// </summary>
        public static string CreateNullNoteInput()
        {
            return "";
        }
    }
}