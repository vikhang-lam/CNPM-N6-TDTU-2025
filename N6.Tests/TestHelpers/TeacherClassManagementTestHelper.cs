using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace N6.Tests.TestHelpers
{
    public static class TeacherClassManagementTestHelper
    {
        #region Test Data Creation

        public static DataTable CreateTeachingClassesDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaLop", typeof(string));
            dt.Columns.Add("TenLop", typeof(string));
            dt.Columns.Add("Khoi", typeof(string));
            dt.Columns.Add("NamHoc", typeof(string));

            dt.Rows.Add("10A1", "Lớp 10A1", "10", "2024");
            dt.Rows.Add("10A2", "Lớp 10A2", "10", "2024");
            dt.Rows.Add("11A1", "Lớp 11A1", "11", "2024");

            return dt;
        }

        public static DataTable CreateHomeroomClassesDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaLop", typeof(string));
            dt.Columns.Add("TenLop", typeof(string));
            dt.Columns.Add("Khoi", typeof(string));
            dt.Columns.Add("NamHoc", typeof(string));

            dt.Rows.Add("10A1", "Lớp 10A1", "10", "2024");

            return dt;
        }

        public static DataTable CreateEmptyClassesDataTable()
        {
            return new DataTable();
        }

        public static DataTable CreateAttendanceDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaHS", typeof(string));
            dt.Columns.Add("HoTen", typeof(string));
            dt.Columns.Add("TrangThai", typeof(string));
            dt.Columns.Add("ThoiGianCapNhat", typeof(DateTime));

            dt.Rows.Add("HS001", "Nguyễn Văn An", "Có mặt", DateTime.Now);
            dt.Rows.Add("HS002", "Trần Thị Bình", "Vắng", DateTime.Now);
            dt.Rows.Add("HS003", "Lê Văn Cường", "Có mặt", DateTime.Now);

            return dt;
        }

        public static DataTable CreateAttendanceDataTableWithStatus()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaHS", typeof(string));
            dt.Columns.Add("HoTen", typeof(string));
            dt.Columns.Add("TrangThai", typeof(string));

            dt.Rows.Add("HS001", "Nguyễn Văn An", "Có mặt");
            dt.Rows.Add("HS002", "Trần Thị Bình", "Vắng");
            dt.Rows.Add("HS003", "Lê Văn Cường", "Có mặt");

            return dt;
        }

        public static DataTable CreateStudentDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaHS", typeof(string));
            dt.Columns.Add("HoTen", typeof(string));
            dt.Columns.Add("GioiTinh", typeof(string));
            dt.Columns.Add("NgaySinh", typeof(DateTime));
            dt.Columns.Add("DiaChi", typeof(string));

            dt.Rows.Add("HS001", "Nguyễn Văn An", "Nam", new DateTime(2007, 5, 15), "Hà Nội");
            dt.Rows.Add("HS002", "Trần Thị Bình", "Nữ", new DateTime(2007, 8, 20), "Hải Phòng");
            dt.Rows.Add("HS003", "Lê Văn Cường", "Nam", new DateTime(2007, 3, 10), "Đà Nẵng");

            return dt;
        }

        public static DataTable CreateScorePivotDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaHS", typeof(string));
            dt.Columns.Add("HoTen", typeof(string));
            dt.Columns.Add("Thang1", typeof(float));
            dt.Columns.Add("Thang2", typeof(float));
            dt.Columns.Add("Thang3", typeof(float));
            dt.Columns.Add("GiuaKi", typeof(float));
            dt.Columns.Add("CuoiKi", typeof(float));
            dt.Columns.Add("NhanXet", typeof(string));

            dt.Rows.Add("HS001", "Nguyễn Văn An", 8.5f, 7.5f, 9.0f, 8.0f, 8.5f, "Học tốt");
            dt.Rows.Add("HS002", "Trần Thị Bình", 7.0f, 8.0f, 7.5f, 7.5f, 8.0f, "Tiến bộ");
            dt.Rows.Add("HS003", "Lê Văn Cường", 9.0f, 8.5f, 9.5f, 9.0f, 9.5f, "Xuất sắc");

            return dt;
        }

        public static DataTable CreateHomeroomGradebookDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaHS", typeof(string));
            dt.Columns.Add("HoTen", typeof(string));
            dt.Columns.Add("Toan", typeof(float));
            dt.Columns.Add("Van", typeof(float));
            dt.Columns.Add("Anh", typeof(float));
            dt.Columns.Add("Ly", typeof(float));

            dt.Rows.Add("HS001", "Nguyễn Văn An", 8.5f, 7.5f, 8.0f, 9.0f);
            dt.Rows.Add("HS002", "Trần Thị Bình", 7.0f, 8.5f, 7.5f, 8.0f);
            dt.Rows.Add("HS003", "Lê Văn Cường", 9.0f, 8.0f, 9.5f, 9.0f);

            return dt;
        }

        public static DataTable CreateClassFundDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaQL", typeof(string));
            dt.Columns.Add("Loai", typeof(string));
            dt.Columns.Add("SoTien", typeof(decimal));
            dt.Columns.Add("Ngay", typeof(DateTime));
            dt.Columns.Add("GhiChu", typeof(string));

            dt.Rows.Add("QL001", "Thu", 50000, DateTime.Today, "Tiền quỹ lớp");
            dt.Rows.Add("QL002", "Thu", 100000, DateTime.Today.AddDays(-1), "Tiền phụ huynh ủng hộ");
            dt.Rows.Add("QL003", "Chi", 50000, DateTime.Today.AddDays(-2), "Mua đồ dùng học tập");

            return dt;
        }
        // Thêm vào TeacherClassManagementTestHelper class
        public static DataTable CreateTeacherSubjectsDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaMon", typeof(string));
            dt.Columns.Add("TenMon", typeof(string));

            dt.Rows.Add("TOAN", "Toán");
            dt.Rows.Add("VAN", "Ngữ Văn");
            dt.Rows.Add("ANH", "Tiếng Anh");

            return dt;
        }

        public static string GetTeacherIdByUsername(string username)
        {
            // Mock implementation
            return "GV001";
        }

        public static Dictionary<string, bool> CreateLockStatusCache()
        {
            return new Dictionary<string, bool>
    {
        { "Thang1_Ki1", true },
        { "Thang2_Ki1", false },
        { "Thang3_Ki1", false },
        { "GiuaKi1", true },
        { "CuoiKi1", false },
        { "Thang1_Ki2", false },
        { "Thang2_Ki2", false },
        { "Thang3_Ki2", false },
        { "GiuaKi2", false },
        { "CuoiKi2", false }
    };
        }

        public static string MapColumnToLoai(string columnName, int semester)
        {
            if (string.IsNullOrEmpty(columnName)) return null;

            columnName = columnName.Trim();
            if (columnName.Equals("Thang1", StringComparison.OrdinalIgnoreCase)) return $"Thang1_Ki{semester}";
            if (columnName.Equals("Thang2", StringComparison.OrdinalIgnoreCase)) return $"Thang2_Ki{semester}";
            if (columnName.Equals("Thang3", StringComparison.OrdinalIgnoreCase)) return $"Thang3_Ki{semester}";
            if (columnName.Equals("GiuaKi", StringComparison.OrdinalIgnoreCase)) return $"GiuaKi{semester}";
            if (columnName.Equals("CuoiKi", StringComparison.OrdinalIgnoreCase)) return $"CuoiKi{semester}";
            if (columnName.Equals("NhanXet", StringComparison.OrdinalIgnoreCase)) return $"CuoiKi{semester}";
            if (columnName.Equals("GhiChu", StringComparison.OrdinalIgnoreCase)) return $"CuoiKi{semester}";

            return null;
        }

        public static void ApplyQuyLopGridFormatting(DataGridView grid)
        {
            // Mock implementation for grid formatting
            grid.AutoGenerateColumns = false;
        }

        public static DataTable CreateEmptyDataTable()
        {
            return new DataTable();
        }

        public static DataTable CreateComplexFundDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaQL", typeof(string));
            dt.Columns.Add("Loai", typeof(string));
            dt.Columns.Add("SoTien", typeof(decimal));
            dt.Columns.Add("Ngay", typeof(DateTime));
            dt.Columns.Add("GhiChu", typeof(string));

            dt.Rows.Add("QL001", "Thu", 50000, DateTime.Today, "Tiền quỹ lớp");
            dt.Rows.Add("QL002", "Thu", 100000, DateTime.Today.AddDays(-1), "Tiền phụ huynh ủng hộ");
            dt.Rows.Add("QL003", "Thu", 100000, DateTime.Today.AddDays(-2), "Tiền hoạt động");
            dt.Rows.Add("QL004", "Chi", 50000, DateTime.Today.AddDays(-3), "Mua đồ dùng học tập");
            dt.Rows.Add("QL005", "Chi", 70000, DateTime.Today.AddDays(-4), "Tổ chức sinh nhật");

            return dt;
        }

        public static bool ValidateQRContent(string qrContent)
        {
            // Simple validation - QR content should start with "HS"
            return !string.IsNullOrEmpty(qrContent) && qrContent.StartsWith("HS");
        }

        public static DataTable CreateLargeClassDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaHS", typeof(string));
            dt.Columns.Add("HoTen", typeof(string));
            dt.Columns.Add("GioiTinh", typeof(string));

            for (int i = 1; i <= 50; i++)
            {
                dt.Rows.Add($"HS{i:000}", $"Học Sinh {i}", i % 2 == 0 ? "Nam" : "Nữ");
            }

            return dt;
        }

        public static DataTable CreateLargeScoreDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaHS", typeof(string));
            dt.Columns.Add("HoTen", typeof(string));
            dt.Columns.Add("Thang1", typeof(float));
            dt.Columns.Add("Thang2", typeof(float));
            dt.Columns.Add("Thang3", typeof(float));

            var random = new Random();
            for (int i = 1; i <= 50; i++)
            {
                dt.Rows.Add($"HS{i:000}", $"Học Sinh {i}",
                           (float)(random.NextDouble() * 10),
                           (float)(random.NextDouble() * 10),
                           (float)(random.NextDouble() * 10));
            }

            return dt;
        }
        #endregion

        #region Mock Results

        public static OperationResult CreateSuccessOperationResult()
        {
            return new OperationResult
            {
                Success = true,
                Message = "Operation completed successfully"
            };
        }

        public static OperationResult CreateFailureOperationResult()
        {
            return new OperationResult
            {
                Success = false,
                Message = "Operation failed"
            };
        }

        #endregion

        #region Validation Helpers

        public static bool ValidateScore(float score)
        {
            return score >= 0 && score <= 10;
        }

        public static bool CheckScoreLockStatus(string scoreColumn)
        {
            // Mock lock status - Thang1_Ki1 is locked, others are not
            return scoreColumn == "Thang1_Ki1";
        }

        #endregion

        #region UI Helpers

        public static void ApplyModernGridStyle(DataGridView dgv)
        {
            dgv.BorderStyle = BorderStyle.None;
            dgv.RowHeadersVisible = false;
            dgv.BackgroundColor = Color.White;
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(242, 245, 250);
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 250, 252);
        }

        public static MockTeacherClassManagementForm CreateTeacherClassManagementForm(string username)
        {
            return new MockTeacherClassManagementForm(username);
        }

        #endregion
    }

    // Supporting classes
    public class OperationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    // Mock form for testing
    public class MockTeacherClassManagementForm
    {
        public string Username { get; private set; }
        public bool IsHomeroomView { get; private set; }

        public MockTeacherClassManagementForm(string username)
        {
            Username = username;
            IsHomeroomView = false;
        }

        public void SimulateClassButtonClick(string classCode, string className)
        {
            IsHomeroomView = false;
        }

        public void SimulateHomeroomButtonClick(string classCode, string className)
        {
            IsHomeroomView = true;
        }

        public void SimulateShowAttendance()
        {
            // Mock implementation
        }

        public void SimulateShowScores()
        {
            // Mock implementation
        }

        public void SimulateShowHomeroomView()
        {
            IsHomeroomView = true;
        }

        public void SimulateTabSelectionChange()
        {
            // Mock implementation
        }

        public bool SimulateDispose()
        {
            return true; // Resources cleaned
        }
    }

}