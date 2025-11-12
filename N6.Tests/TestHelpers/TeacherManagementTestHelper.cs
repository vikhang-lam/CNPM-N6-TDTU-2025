using System.Data;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace N6.Tests.TestHelpers
{
    public static class TeacherManagementTestHelper
    {
        public static DataTable CreateAllSubjectsData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaMon", typeof(string));
            dt.Columns.Add("TenMon", typeof(string));

            dt.Rows.Add("M001", "Toán");
            dt.Rows.Add("M002", "Văn");
            dt.Rows.Add("M003", "Anh");
            dt.Rows.Add("M004", "Lý");
            dt.Rows.Add("M005", "Hóa");

            return dt;
        }

        public static DataTable CreateAllTeachersData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaGV", typeof(string));
            dt.Columns.Add("Ten", typeof(string));
            dt.Columns.Add("Email", typeof(string));
            dt.Columns.Add("SDT", typeof(string));
            dt.Columns.Add("TrangThai", typeof(string));

            dt.Rows.Add("GV001", "Nguyễn Văn A", "a@email.com", "0123456789", "Đã xác nhận");
            dt.Rows.Add("GV002", "Trần Thị B", "b@email.com", "0987654321", "Chưa xác nhận");
            dt.Rows.Add("GV003", "Lê Văn C", "c@email.com", "0912345678", "Đã xác nhận");

            return dt;
        }

        public static DataTable CreatePendingTeachersData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaGV", typeof(string));
            dt.Columns.Add("Ten", typeof(string));
            dt.Columns.Add("Email", typeof(string));
            dt.Columns.Add("SDT", typeof(string));
            dt.Columns.Add("TrangThai", typeof(string));

            dt.Rows.Add("GV002", "Trần Thị B", "b@email.com", "0987654321", "Chưa xác nhận");
            dt.Rows.Add("GV004", "Phạm Thị D", "d@email.com", "0934567890", "Chưa xác nhận");

            return dt;
        }

        public static DataTable CreateApprovedTeachersData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaGV", typeof(string));
            dt.Columns.Add("Ten", typeof(string));
            dt.Columns.Add("Email", typeof(string));
            dt.Columns.Add("SDT", typeof(string));
            dt.Columns.Add("TrangThai", typeof(string));

            dt.Rows.Add("GV001", "Nguyễn Văn A", "a@email.com", "0123456789", "Đã xác nhận");
            dt.Rows.Add("GV003", "Lê Văn C", "c@email.com", "0912345678", "Đã xác nhận");

            return dt;
        }

        public static DataTable CreateValidTeacherData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaGV", typeof(string));
            dt.Columns.Add("Ten", typeof(string));
            dt.Columns.Add("Email", typeof(string));
            dt.Columns.Add("SDT", typeof(string));

            dt.Rows.Add("GV001", "Nguyễn Văn A", "a@email.com", "0123456789");

            return dt;
        }

        public static (string, string) CreateTeacherStatusUpdateResult()
        {
            return ("teacher@example.com", "Nguyễn Văn A");
        }

        public static TextBox CreateTeacherNameTextBox(string text)
        {
            return new TextBox { Text = text };
        }

        public static TextBox CreateTeacherEmailTextBox(string text)
        {
            return new TextBox { Text = text };
        }

        public static TextBox CreateTeacherPhoneTextBox(string text)
        {
            return new TextBox { Text = text };
        }

        public static CheckedListBox CreateSubjectsCheckListBox()
        {
            var clb = new CheckedListBox();
            clb.Items.Add("Toán");
            clb.Items.Add("Văn");
            clb.Items.Add("Anh");
            clb.Items.Add("Lý");
            clb.Items.Add("Hóa");
            return clb;
        }

        public static bool ValidateTeacherInfo(string name, string email, string phone)
        {
            // Validate name (only letters and spaces)
            if (string.IsNullOrWhiteSpace(name) || !Regex.IsMatch(name, @"^[\p{L}\s]+$"))
                return false;

            // Validate email format
            if (string.IsNullOrWhiteSpace(email) || !Regex.IsMatch(email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
                return false;

            // Validate phone (exactly 10 digits)
            if (string.IsNullOrWhiteSpace(phone) || !Regex.IsMatch(phone, @"^\d{10}$"))
                return false;

            return true;
        }

        public static DataTable CreateTeacherAssignmentsData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaMon", typeof(string));
            dt.Columns.Add("TenMon", typeof(string));
            dt.Columns.Add("MaLop", typeof(string));

            dt.Rows.Add("M001", "Toán", "LOP10A");
            dt.Rows.Add("M002", "Văn", "LOP10B");

            return dt;
        }
    }
}