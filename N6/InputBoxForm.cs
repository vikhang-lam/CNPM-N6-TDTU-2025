using System;
using System.Drawing;
using System.Windows.Forms;

namespace N6
{
    /// <summary>
    /// Một Form dialog tùy chỉnh để lấy dữ liệu nhập (string) từ người dùng.
    /// </summary>
    public partial class InputBoxForm : Form
    {
        /// <summary>
        /// Lấy giá trị mà người dùng đã nhập sau khi nhấn "Lưu".
        /// </summary>
        public string InputValue { get; private set; }

        /// <summary>
        /// Khởi tạo InputBoxForm.
        /// </summary>
        /// <param name="title">Tiêu đề của cửa sổ.</param>
        /// <param name="prompt">Lời nhắc hiển thị phía trên ô nhập liệu.</param>
        /// <param name="defaultValue">Giá trị mặc định (nếu có).</param>
        public InputBoxForm(string title, string prompt, string defaultValue = "")
        {
            InitializeComponent();

            // Gán giá trị
            this.lblTitle.Text = title;
            this.lblPrompt.Text = prompt;
            this.txtInput.Text = defaultValue;
            this.InputValue = defaultValue;

            // Căn chỉnh 
            this.txtInput.Select();

            // Thêm style cho TextBox
            this.txtInput.Padding = new Padding(5); // Căn lề 5px bên trong

            // Gán sự kiện (để gỡ trong Dispose)
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
        }

        /// <summary>
        /// Xử lý sự kiện click nút "Lưu".
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            this.InputValue = txtInput.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// Dọn dẹp tài nguyên và gỡ bỏ các trình xử lý sự kiện.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Gỡ bỏ sự kiện đã gán thủ công
                if (this.btnSave != null)
                {
                    this.btnSave.Click -= new System.EventHandler(this.btnSave_Click);
                }

                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}