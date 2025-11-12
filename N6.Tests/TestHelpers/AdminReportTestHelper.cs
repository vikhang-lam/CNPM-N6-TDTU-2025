using System.Data;

namespace N6.Tests.TestHelpers
{
    /// <summary>
    /// Cung cấp dữ liệu DataTable giả cho việc kiểm thử UC_BaoCao_Admin (UC36).
    /// </summary>
    public static class AdminReportTestHelper
    {
        /// <summary>
        /// Mô phỏng DatabaseHelper.GetAllClasses()
        /// </summary>
        public static DataTable CreateMockAllClasses()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaLop", typeof(string));
            dt.Columns.Add("TenLop", typeof(string));
            dt.Columns.Add("Khoi", typeof(string));
            dt.Rows.Add("L5A", "Lớp 5A", "Khối 5");
            dt.Rows.Add("L5B", "Lớp 5B", "Khối 5");
            dt.Rows.Add("L4A", "Lớp 4A", "Khối 4");
            return dt;
        }

        /// <summary>
        /// Mô phỏng DatabaseHelper.GetAllSubjects()
        /// </summary>
        public static DataTable CreateMockAllSubjects()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaMon", typeof(string));
            dt.Columns.Add("TenMon", typeof(string));
            dt.Rows.Add("TOAN", "Toán");
            dt.Rows.Add("TV", "Tiếng Việt");
            return dt;
        }

        /// <summary>
        /// Mô phỏng DatabaseHelper.GetMonthlyScoreTypes()
        /// </summary>
        public static DataTable CreateMockMonthlyScoreTypes()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("TenHienThi", typeof(string));
            dt.Columns.Add("MaCotDiem", typeof(string));
            dt.Rows.Add("Tháng 9 (GK1)", "Thang9_GK1");
            dt.Rows.Add("Tháng 10 (CK1)", "Thang10_CK1");
            return dt;
        }

        /// <summary>
        /// Mô phỏng DatabaseHelper.GetAttendanceReport_Admin()
        /// </summary>
        public static DataTable CreateMockAttendanceReport()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaHS", typeof(string));
            dt.Columns.Add("HoTen", typeof(string));
            dt.Columns.Add("TenLop", typeof(string));
            dt.Columns.Add("SoBuoiCoMat", typeof(long));
            dt.Columns.Add("SoBuoiVang", typeof(long));
            dt.Columns.Add("SoBuoiVangCoPhep", typeof(long));
            dt.Rows.Add("HS001", "Nguyễn Văn A", "Lớp 5A", 50, 2, 1);
            dt.Rows.Add("HS002", "Trần Thị B", "Lớp 5B", 48, 0, 5);
            return dt;
        }

        /// <summary>
        /// Mô phỏng DatabaseHelper.GetStudentRecords_Admin()
        /// </summary>
        public static DataTable CreateMockStudentRecords()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaHS", typeof(string));
            dt.Columns.Add("HoTen", typeof(string));
            dt.Columns.Add("TenLop", typeof(string));
            dt.Columns.Add("NgaySinh", typeof(string));
            dt.Columns.Add("DiaChi", typeof(string));
            dt.Rows.Add("HS001", "Nguyễn Văn A", "Lớp 5A", "2015-01-01", "TP. HCM");
            return dt;
        }

        /// <summary>
        /// Mô phỏng DatabaseHelper.GetGradeStatistics_Admin()
        /// </summary>
        public static DataTable CreateMockGradeStatistics()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("TenLop", typeof(string));
            dt.Columns.Add("SoHocSinh", typeof(int));
            dt.Columns.Add("DiemTrungBinh", typeof(double));
            dt.Rows.Add("Lớp 5A", 30, 8.5);
            dt.Rows.Add("Lớp 5B", 32, 8.2);
            return dt;
        }

        /// <summary>
        /// Mô phỏng DatabaseHelper.GetMonthlyReport_Statistics_Admin()
        /// </summary>
        public static DataTable CreateMockMonthlyStatisticsAdmin()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("LoaiThongKe", typeof(string));
            dt.Columns.Add("PhanLoai", typeof(string));
            dt.Columns.Add("TS", typeof(int));
            dt.Columns.Add("TyLe", typeof(double)); // Admin có TyLe

            // Thống kê Điểm
            dt.Rows.Add("Diem", "10", 5, 10.0);
            dt.Rows.Add("Diem", "9", 15, 30.0);
            dt.Rows.Add("Diem", "8", 20, 40.0);
            dt.Rows.Add("Diem", "Dưới 5", 10, 20.0);

            // Thống kê Xếp loại
            dt.Rows.Add("XepLoai", "T", 30, 60.0);
            dt.Rows.Add("XepLoai", "H", 10, 20.0);
            dt.Rows.Add("XepLoai", "C", 10, 20.0);
            return dt;
        }

        /// <summary>
        /// Mô phỏng DatabaseHelper.GetMonthlyReport_Statistics() (của GV)
        /// </summary>
        public static DataTable CreateMockMonthlyStatisticsTeacher()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("LoaiThongKe", typeof(string));
            dt.Columns.Add("PhanLoai", typeof(string));
            dt.Columns.Add("TS", typeof(int));
            dt.Columns.Add("Nu", typeof(int)); // GV có Nữ
            dt.Columns.Add("DanToc", typeof(int)); // GV có Dân tộc
            dt.Columns.Add("NDT", typeof(int)); // GV có NDT

            dt.Rows.Add("Diem", "10", 5, 2, 1, 0);
            dt.Rows.Add("XepLoai", "T", 5, 2, 1, 0);
            return dt;
        }

        /// <summary>
        /// Mô phỏng DataTable cho CalculateAndAddAverageColumn
        /// </summary>
        public static DataTable CreateMockPivotDataForAverage()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaHS", typeof(string));
            dt.Columns.Add("HoTen", typeof(string));
            dt.Columns.Add("Toan", typeof(double));
            dt.Columns.Add("TiengViet", typeof(double));
            dt.Columns.Add("DiemTB", typeof(double)); // Cột này sẽ được điền

            dt.Rows.Add("HS001", "Nguyễn Văn A", 8.0, 10.0, DBNull.Value);
            dt.Rows.Add("HS002", "Trần Thị B", 5.0, 7.0, DBNull.Value);
            dt.Rows.Add("HS003", "Lê Văn C", DBNull.Value, 9.0, DBNull.Value); // Test DBNull
            return dt;
        }
    }
}