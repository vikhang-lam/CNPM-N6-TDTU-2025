using System.Windows.Forms;

namespace N6.Tests.TestHelpers
{
    public static class MockDataHelper
    {
        public static TextBox CreateValidNameTextBox()
        {
            return new TextBox { Text = "Nguyễn Văn A", ForeColor = System.Drawing.Color.Black };
        }

        public static TextBox CreateValidUsernameTextBox()
        {
            return new TextBox { Text = "nguyenvana", ForeColor = System.Drawing.Color.Black };
        }

        public static TextBox CreateValidEmailTextBox()
        {
            return new TextBox { Text = "nguyenvana@example.com", ForeColor = System.Drawing.Color.Black };
        }

        public static TextBox CreateValidPhoneTextBox()
        {
            return new TextBox { Text = "0123456789", ForeColor = System.Drawing.Color.Black };
        }

        public static TextBox CreateValidPasswordTextBox()
        {
            return new TextBox { Text = "password123", ForeColor = System.Drawing.Color.Black, UseSystemPasswordChar = true };
        }

        public static TextBox CreatePlaceholderTextBox(string placeholder)
        {
            return new TextBox
            {
                Text = placeholder,
                ForeColor = System.Drawing.Color.Gray,
                Tag = placeholder
            };
        }
    }
}