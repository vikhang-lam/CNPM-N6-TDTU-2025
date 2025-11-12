using System;
using System.Data;

namespace N6.Tests.TestHelpers
{
    /// <summary>
    /// Cung cấp dữ liệu DataTable giả cho việc kiểm thử UC_ThoiKhoaBieu (UC37, 38, 39).
    /// </summary>
    public static class TimetableTestHelper
    {
        /// <summary>
        /// Trả về một ngày Thứ 2 (Monday) cố định để test.
        /// </summary>
        public static DateTime GetTestMonday()
        {
            return new DateTime(2025, 11, 10); // Giả sử ngày 10/11/2025 là Thứ 2
        }

        /// <summary>
        /// Mô phỏng kết quả từ DatabaseHelper.GetTimetableByTeacher.
        /// </summary>
        public static DataTable CreateMockTimetableData(DateTime monday)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Ngay", typeof(DateTime));
            dt.Columns.Add("Tiet", typeof(int));
            dt.Columns.Add("TenMon", typeof(string));
            dt.Columns.Add("TenLop", typeof(string));
            dt.Columns.Add("GhiChu", typeof(string));
            dt.Columns.Add("MauSac", typeof(string));

            // Tiết 1, Thứ 2 (10/11) - Có màu
            dt.Rows.Add(monday, 1, "Toán", "Lớp 5A", "", "#FF0000"); // Đỏ

            // Tiết 3, Thứ 4 (12/11) - Có ghi chú
            dt.Rows.Add(monday.AddDays(2), 3, "Tiếng Việt", "Lớp 5B", "Kiểm tra miệng", "");

            // Tiết 5, Thứ 6 (14/11) - Chỉ có TKB
            dt.Rows.Add(monday.AddDays(4), 5, "Khoa học", "Lớp 5A", "", "");

            return dt;
        }

        /// <summary>
        /// Mô phỏng kết quả từ DatabaseHelper.GetClassesByTeacher (dùng cho frmGhiChuTKB).
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
        /// Dữ liệu hợp lệ để lưu ghi chú từ frmGhiChuTKB.
        /// </summary>
        public static (string maLop, int tiet, string ghiChu) CreateValidNoteInput()
        {
            return ("L5A", 2, "Họp phụ huynh");
        }
    }
}