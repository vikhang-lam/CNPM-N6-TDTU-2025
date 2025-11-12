using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace N6.Tests.TestHelpers
{
    public static class UserProfileTestHelper
    {
        public static TeacherProfile CreateValidTeacherProfile()
        {
            return new TeacherProfile
            {
                Ten = "Nguyễn Văn A",
                TenMon = "Toán",
                Email = "teacher@email.com",
                SDT = "0123456789",
                AnhDaiDien = "Avatars/teacher123.jpg"
            };
        }

        public static TeacherProfile CreateTeacherProfileWithoutAvatar()
        {
            return new TeacherProfile
            {
                Ten = "Nguyễn Văn A",
                TenMon = "Toán",
                Email = "teacher@email.com",
                SDT = "0123456789",
                AnhDaiDien = null
            };
        }

        public static DataRow CreateValidAdminData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Email", typeof(string));
            var row = dt.NewRow();
            row["Email"] = "admin@school.edu.vn";
            return row;
        }

        public static DataRow CreateAdminDataWithoutEmail()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Email", typeof(string));
            var row = dt.NewRow();
            row["Email"] = null;
            return row;
        }

        public static (string, string, string) CreateValidPasswordChangeData()
        {
            return ("oldpassword", "newpassword123", "newpassword123");
        }

        public static (string, string, string) CreatePasswordMismatchData()
        {
            return ("oldpassword", "newpassword123", "differentpassword");
        }

        public static string CreateTestImageFile()
        {
            return "test_avatar.jpg";
        }

        public static PictureBox CreateCircularPictureBox()
        {
            return new CircularPictureBox
            {
                Size = new Size(100, 100),
                BackColor = Color.Transparent
            };
        }

        public static Label CreateNameLabel(string text)
        {
            return new Label { Text = text };
        }

        public static Label CreateEmailLabel(string text)
        {
            return new Label { Text = text };
        }

        public static Label CreateSubjectLabel(string text)
        {
            return new Label { Text = text };
        }

        public static Panel CreatePasswordPanel()
        {
            return new Panel
            {
                Visible = false,
                Size = new Size(300, 150)
            };
        }

        public static bool ValidateEmail(string email)
        {
            return !string.IsNullOrEmpty(email) &&
                   email.Contains("@") &&
                   email.Contains(".");
        }

        public static bool ValidatePassword(string password, string confirmPassword)
        {
            return !string.IsNullOrEmpty(password) &&
                   password == confirmPassword;
        }
    }
}