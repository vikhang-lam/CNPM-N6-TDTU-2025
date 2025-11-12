
using System.Windows.Forms;

namespace N6.Tests.TestHelpers
{
    public static class ForgotPasswordTestHelper
    {
        public static (string username, string email) CreateValidTeacherCredentials()
        {
            return ("teacher123", "teacher@example.com");
        }

        public static (string username, string email) CreateInvalidTeacherCredentials()
        {
            return ("invaliduser", "invalid@example.com");
        }

        public static (string username, string email) CreateEmptyCredentials()
        {
            return ("", "");
        }

        public static TextBox CreateUsernameTextBox(string text, bool isPlaceholder = false)
        {
            return new TextBox
            {
                Text = text,
                ForeColor = isPlaceholder ? System.Drawing.Color.Gray : System.Drawing.Color.Black
            };
        }

        public static TextBox CreateOtpTextBox(string text, bool isPlaceholder = false)
        {
            return new TextBox
            {
                Text = text,
                ForeColor = isPlaceholder ? System.Drawing.Color.Gray : System.Drawing.Color.Black
            };
        }

        public static TextBox CreatePasswordTextBox(string text, bool isPlaceholder = false, bool usePasswordChar = false)
        {
            return new TextBox
            {
                Text = text,
                ForeColor = isPlaceholder ? System.Drawing.Color.Gray : System.Drawing.Color.Black,
                UseSystemPasswordChar = usePasswordChar
            };
        }

        public static string GenerateValidOtp()
        {
            return "123456";
        }

        public static string GenerateInvalidOtp()
        {
            return "000000";
        }

        public static string GenerateExpiredOtp()
        {
            return "999999";
        }

        public static string CreateValidPassword()
        {
            return "newpassword123";
        }

        public static string CreateInvalidPassword()
        {
            return "short";
        }

        public static OtpRequestResult CreateValidOtpRequestResult()
        {
            return new OtpRequestResult
            {
                Success = true,
                Email = "teacher@example.com",
                Otp = "123456"
            };
        }

        public static OtpRequestResult CreateInvalidOtpRequestResult()
        {
            return new OtpRequestResult
            {
                Success = false,
                Email = null,
                Otp = null
            };
        }
    }
}