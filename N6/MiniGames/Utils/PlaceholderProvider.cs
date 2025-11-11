using System;
using System.Drawing;
using System.Windows.Forms;

namespace N6
{
    public static class PlaceholderProvider
    {
        #region Public Methods

        /// <summary>
        /// Thiết lập placeholder cho TextBox. Khi control nhận focus, placeholder sẽ được xóa.
        /// Khi mất focus và rỗng, placeholder được khôi phục.
        /// </summary>
        /// <param name="textBox">TextBox đích cần áp dụng placeholder.</param>
        /// <param name="placeholder">Nội dung placeholder hiển thị.</param>
        public static void SetPlaceholder(TextBox textBox, string placeholder)
        {
            if (textBox == null)
            {
                throw new ArgumentNullException(nameof(textBox));
            }

            // Thiết lập trạng thái placeholder ban đầu (text + màu)
            textBox.Text = placeholder;
            textBox.ForeColor = Color.Gray;

            textBox.Enter += (sender, e) =>
            {
                // Khi focus, nếu đang là placeholder thì xóa và đổi màu chữ
                if (textBox.Text == placeholder)
                {
                    textBox.Text = string.Empty;
                    textBox.ForeColor = Color.Black;
                }
            };

            textBox.Leave += (sender, e) =>
            {
                // Khi mất focus và rỗng, khôi phục placeholder và màu xám
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = placeholder;
                    textBox.ForeColor = Color.Gray;
                }
            };
        }

        #endregion
    }
}