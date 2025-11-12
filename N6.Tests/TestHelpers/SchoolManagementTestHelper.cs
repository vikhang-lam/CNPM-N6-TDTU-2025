using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace N6.Tests.TestHelpers
{
    public static class SchoolManagementTestHelper
    {
        #region Test Data Creation

        public static (string maMon, string tenMon) CreateValidSubject()
        {
            return ("MH001", "Toán Học");
        }

        public static (string maMon, string tenMon) CreateInvalidSubject_EmptyName()
        {
            return ("MH002", "");
        }

        public static (string maMon, string tenMon) CreateDuplicateSubject()
        {
            return ("MH001", "Toán Học Khác");
        }

        public static DataTable CreateMockSubjectsDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaMon", typeof(string));
            dt.Columns.Add("TenMon", typeof(string));

            dt.Rows.Add("MH001", "Toán Học");
            dt.Rows.Add("MH002", "Ngữ Văn");
            dt.Rows.Add("MH003", "Tiếng Anh");

            return dt;
        }

        public static DataTable CreateMockScoreDeadlinesDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaCotDiem", typeof(string));
            dt.Columns.Add("TenHienThi", typeof(string));
            dt.Columns.Add("Khoi", typeof(string));
            dt.Columns.Add("HocKy", typeof(int));
            dt.Columns.Add("NgayMoDiem", typeof(DateTime));
            dt.Columns.Add("NgayKhoaDiem", typeof(DateTime));
            dt.Columns.Add("KhoaThuCong", typeof(bool));
            dt.Columns.Add("DaKhoa", typeof(bool));

            dt.Rows.Add("CD001", "Điểm miệng HK1", "Khối 1", 1,
                       new DateTime(2024, 9, 1), new DateTime(2024, 12, 31), false, false);
            dt.Rows.Add("CD002", "Điểm 15p HK1", "Khối 2", 1,
                       new DateTime(2024, 9, 1), new DateTime(2024, 12, 31), false, true);

            return dt;
        }

        public static DataTable CreateMockClassesDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaLop", typeof(string));
            dt.Columns.Add("TenLop", typeof(string));
            dt.Columns.Add("Khoi", typeof(string));
            dt.Columns.Add("NamHoc", typeof(string));

            dt.Rows.Add("LOP1A", "Lớp 1A", "Khối 1", "2024-2025");
            dt.Rows.Add("LOP1B", "Lớp 1B", "Khối 1", "2024-2025");
            dt.Rows.Add("LOP2A", "Lớp 2A", "Khối 2", "2024-2025");

            return dt;
        }

        public static DataTable CreateMockStudentPromotionDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaHS", typeof(string));
            dt.Columns.Add("HoTen", typeof(string));
            dt.Columns.Add("DiemTB", typeof(double));

            dt.Rows.Add("HS001", "Nguyễn Văn A", 8.5);
            dt.Rows.Add("HS002", "Trần Thị B", 4.2);
            dt.Rows.Add("HS003", "Lê Văn C", 7.8);

            return dt;
        }

        #endregion

        #region UI Controls Creation

        public static TextBox CreateSubjectCodeTextBox(string text, bool readOnly = false)
        {
            return new TextBox
            {
                Text = text,
                ReadOnly = readOnly,
                BackColor = readOnly ? SystemColors.Control : SystemColors.Window
            };
        }

        public static TextBox CreateSubjectNameTextBox(string text)
        {
            return new TextBox
            {
                Text = text,
                BackColor = SystemColors.Window
            };
        }

        public static DataGridView CreateSubjectsDataGridView()
        {
            var dgv = new DataGridView
            {
                DataSource = CreateMockSubjectsDataTable(),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            return dgv;
        }

        public static DataGridView CreateScoreDeadlinesDataGridView()
        {
            var dgv = new DataGridView
            {
                DataSource = CreateMockScoreDeadlinesDataTable(),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = false
            };

            return dgv;
        }

        public static ComboBox CreateGradeFilterComboBox()
        {
            var comboBox = new ComboBox();
            comboBox.Items.AddRange(new string[] { "Tất cả", "Khối 1", "Khối 2", "Khối 3", "Khối 4", "Khối 5" });
            comboBox.SelectedIndex = 0;
            return comboBox;
        }

        public static ComboBox CreateClassComboBox()
        {
            var comboBox = new ComboBox
            {
                DataSource = CreateMockClassesDataTable(),
                DisplayMember = "TenLop",
                ValueMember = "MaLop"
            };
            return comboBox;
        }

        public static ListBox CreateStudentListBox()
        {
            var listBox = new ListBox();
            var students = new[]
            {
                "Nguyễn Văn A (HS001) - ĐTB: 8.50",
                "Trần Thị B (HS002) - ĐTB: 4.20",
                "Lê Văn C (HS003) - ĐTB: 7.80"
            };
            listBox.Items.AddRange(students);
            return listBox;
        }

        #endregion

        #region Mock Results

        public static DataTable CreateMockClassDetails()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaLop", typeof(string));
            dt.Columns.Add("TenLop", typeof(string));
            dt.Columns.Add("Khoi", typeof(string));
            dt.Columns.Add("NamHoc", typeof(string));

            dt.Rows.Add("LOP1A", "Lớp 1A", "Khối 1", "2024-2025");
            return dt;
        }

        public static DataTable CreateMockSemesterScoreboard()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaHS", typeof(string));
            dt.Columns.Add("HoTen", typeof(string));
            dt.Columns.Add("Trung bình chung", typeof(double));

            dt.Rows.Add("HS001", "Nguyễn Văn A", 8.5);
            dt.Rows.Add("HS002", "Trần Thị B", 4.2);
            dt.Rows.Add("HS003", "Lê Văn C", 7.8);

            return dt;
        }

        public static bool CreateSuccessfulDatabaseOperation()
        {
            return true;
        }

        public static bool CreateFailedDatabaseOperation()
        {
            return false;
        }

        public static Exception CreateDatabaseException(string message = "Database error occurred")
        {
            return new Exception(message);
        }

        #endregion

        #region Promotion Test Data

        public static (string maLopCu, string khoi, bool isLop5) CreateValidPromotionData()
        {
            return ("LOP1A", "Khối 1", false);
        }

        public static (string maLopCu, string khoi, bool isLop5) CreateGrade5PromotionData()
        {
            return ("LOP5A", "Khối 5", true);
        }

        public static (string maLopMoiLenLop, string maLopMoiOLaiLop) CreateValidDestinationClasses()
        {
            return ("LOP2A", "LOP1B");
        }

        public static (string maLopMoiLenLop, string maLopMoiOLaiLop) CreateInvalidDestinationClasses_Missing()
        {
            return (null, null);
        }

        #endregion
    }
}
