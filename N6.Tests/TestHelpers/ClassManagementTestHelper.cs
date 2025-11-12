
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace N6.Tests.TestHelpers
{
    public static class ClassManagementTestHelper
    {
        #region Test Data Creation

        public static DataTable CreateSampleClassesDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaLop", typeof(string));
            dt.Columns.Add("TenLop", typeof(string));
            dt.Columns.Add("Khoi", typeof(string));
            dt.Columns.Add("NamHoc", typeof(string));
            dt.Columns.Add("MaGVCN", typeof(string));
            dt.Columns.Add("TenGVCN", typeof(string));
            dt.Columns.Add("SiSo", typeof(int));

            dt.Rows.Add("10A1", "Lớp 10A1", "10", "2024", "GV001", "Nguyễn Văn A", 35);
            dt.Rows.Add("10A2", "Lớp 10A2", "10", "2024", "GV002", "Trần Thị B", 32);
            dt.Rows.Add("11A1", "Lớp 11A1", "11", "2024", "GV003", "Lê Văn C", 38);

            return dt;
        }

        public static DataTable CreateSampleStudentsDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("STT", typeof(int));
            dt.Columns.Add("MaHS", typeof(string));
            dt.Columns.Add("HoTen", typeof(string));
            dt.Columns.Add("GioiTinh", typeof(string));
            dt.Columns.Add("NgaySinh", typeof(DateTime));
            dt.Columns.Add("DiaChi", typeof(string));
            dt.Columns.Add("DanToc", typeof(string));
            dt.Columns.Add("SDTPhuHuynh", typeof(string));

            dt.Rows.Add(1, "HS001", "Nguyễn Văn An", "Nam", new DateTime(2007, 5, 15), "Hà Nội", "Kinh", "0912345678");
            dt.Rows.Add(2, "HS002", "Trần Thị Bình", "Nữ", new DateTime(2007, 8, 20), "Hải Phòng", "Kinh", "0912345679");
            dt.Rows.Add(3, "HS003", "Lê Văn Cường", "Nam", new DateTime(2007, 3, 10), "Đà Nẵng", "Kinh", "0912345680");

            return dt;
        }

        public static DataTable CreateSampleTeachersDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaGV", typeof(string));
            dt.Columns.Add("Ten", typeof(string));
            dt.Columns.Add("CacMonDay", typeof(string));

            dt.Rows.Add("GV001", "Nguyễn Văn A", "Toán,Lý");
            dt.Rows.Add("GV002", "Trần Thị B", "Văn,Sử");
            dt.Rows.Add("GV003", "Lê Văn C", "Hóa,Sinh");
            dt.Rows.Add("GV004", "Phạm Thị D", "Toán"); // Unassigned teacher

            return dt;
        }

        public static DataTable CreateSampleTeachingAssignmentsDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaMon", typeof(string));
            dt.Columns.Add("TenMon", typeof(string));
            dt.Columns.Add("MaGV", typeof(string));
            dt.Columns.Add("TenGV", typeof(string));

            dt.Rows.Add("TOAN", "Toán", "GV001", "Nguyễn Văn A");
            dt.Rows.Add("VAN", "Văn", "GV002", "Trần Thị B");
            dt.Rows.Add("LY", "Lý", null, "(Trống)");

            return dt;
        }

        public static DataRow CreateSampleClassDetails()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("TenLop", typeof(string));
            dt.Columns.Add("NamHoc", typeof(string));
            dt.Columns.Add("MaGVCN", typeof(string));
            dt.Columns.Add("TenGVCN", typeof(string));
            dt.Columns.Add("SiSo", typeof(int));

            dt.Rows.Add("Lớp 10A1", "2024", "GV001", "Nguyễn Văn A", 35);

            return dt.Rows[0];
        }

        #endregion

        #region UI Controls Creation

        public static ComboBox CreateKhoiComboBox(string selectedKhoi = "Tất cả các khối")
        {
            var comboBox = new ComboBox();
            comboBox.Items.AddRange(new object[] { "Tất cả các khối", "10", "11", "12" });
            comboBox.SelectedItem = selectedKhoi;
            return comboBox;
        }

        public static DataGridView CreateClassesDataGridView(bool withSelection = true)
        {
            var dgv = new DataGridView();
            dgv.DataSource = CreateSampleClassesDataTable();

            if (withSelection && dgv.Rows.Count > 0)
            {
                dgv.Rows[0].Selected = true;
            }

            return dgv;
        }

        public static DataGridView CreateStudentsDataGridView()
        {
            var dgv = new DataGridView();
            dgv.DataSource = CreateSampleStudentsDataTable();
            return dgv;
        }

        public static DataGridView CreateTeachingAssignmentsDataGridView()
        {
            var dgv = new DataGridView();
            dgv.DataSource = CreateSampleTeachingAssignmentsDataTable();
            return dgv;
        }

        public static Panel CreateChuyenLopPanel()
        {
            var panel = new Panel();
            panel.Visible = false;

            var clb = new CheckedListBox();
            clb.Items.Add(new StudentItem { HoTen = "Nguyễn Văn An", MaHS = "HS001" });
            clb.Items.Add(new StudentItem { HoTen = "Trần Thị Bình", MaHS = "HS002" });

            var cbo = new ComboBox();
            cbo.DataSource = CreateSampleClassesDataTable();
            cbo.DisplayMember = "TenLop";
            cbo.ValueMember = "MaLop";

            panel.Controls.Add(clb);
            panel.Controls.Add(cbo);

            return panel;
        }

        public static ComboBox CreateGvcnComboBox()
        {
            var comboBox = new ComboBox();
            comboBox.DataSource = CreateSampleTeachersDataTable();
            comboBox.DisplayMember = "Ten";
            comboBox.ValueMember = "MaGV";
            comboBox.SelectedIndex = 0;
            return comboBox;
        }

        #endregion

        #region Mock Results

        public static ClassOperationResult CreateSuccessClassOperationResult()
        {
            return new ClassOperationResult
            {
                Success = true,
                Message = "Operation completed successfully"
            };
        }

        public static ClassOperationResult CreateFailureClassOperationResult()
        {
            return new ClassOperationResult
            {
                Success = false,
                Message = "Operation failed"
            };
        }

        public static StudentTransferResult CreateSuccessTransferResult(int transferredCount)
        {
            return new StudentTransferResult
            {
                Success = true,
                TransferredCount = transferredCount,
                Message = $"Successfully transferred {transferredCount} students"
            };
        }

        public static StudentTransferResult CreateFailureTransferResult()
        {
            return new StudentTransferResult
            {
                Success = false,
                TransferredCount = 0,
                Message = "Transfer failed"
            };
        }

        public static DatabaseHelper.ImportResult CreateSuccessImportResult()
        {
            return new DatabaseHelper.ImportResult
            {
                Success = 3,
                Skipped = 0,
                Failed = 0
            };
        }

        public static DatabaseHelper.ImportResult CreatePartialImportResult()
        {
            return new DatabaseHelper.ImportResult
            {
                Success = 2,
                Skipped = 1,
                Failed = 1
            };
        }

        #endregion

        #region Validation Helpers

        public static bool ValidateStudentName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            var regex = new System.Text.RegularExpressions.Regex(@"^[a-zA-ZÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơƯĂẠẢẤẦẨẪẬẮẰẲẴẶẸẺẼỀỀỂẾưăạảấầẩẫậắằẳẵặẹẻẽềềểếỄỆỈỊỌỎỐỒỔỖỘỚỜỞỠỢỤỦỨỪễệỉịọỏốồổỗộớờởỡợụủứừỬỮỰỲỴÝỶỸửữựỳỵỷỹ\s]+$");
            return regex.IsMatch(name);
        }

        public static bool ValidatePhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return true;

            return System.Text.RegularExpressions.Regex.IsMatch(phone, @"^\d+$");
        }

        public static bool ValidateClassCode(string code)
        {
            return !string.IsNullOrWhiteSpace(code) && code.Length >= 2;
        }

        public static bool ValidateGrade(string grade)
        {
            return !string.IsNullOrWhiteSpace(grade) && int.TryParse(grade, out int result) && result >= 1 && result <= 12;
        }

        #endregion

        #region Test Scenarios

        public static List<string> CreateValidStudentTransferList()
        {
            return new List<string> { "HS001", "HS002", "HS003" };
        }

        public static List<string> CreateEmptyStudentTransferList()
        {
            return new List<string>();
        }

        public static string CreateValidClassCode()
        {
            return "10A1";
        }

        public static string CreateInvalidClassCode()
        {
            return "";
        }

        public static string CreateValidStudentName()
        {
            return "Nguyễn Văn An";
        }

        public static string CreateInvalidStudentName()
        {
            return "Nguyen123";
        }

        public static string CreateValidPhoneNumber()
        {
            return "0912345678";
        }

        public static string CreateInvalidPhoneNumber()
        {
            return "abc123";
        }

        #endregion
    }

    // Supporting classes for test results
    public class ClassOperationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class StudentTransferResult
    {
        public bool Success { get; set; }
        public int TransferredCount { get; set; }
        public string Message { get; set; }
    }

    // Mock StudentItem class for testing
    public class StudentItem
    {
        public string HoTen { get; set; }
        public string MaHS { get; set; }

        public override string ToString()
        {
            return HoTen;
        }
    }
}
