using System;
using System.Drawing;
using System.Windows.Forms;

namespace N6
{
    public partial class InputBoxForm : Form
    {
        public string InputValue { get; private set; }

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
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.InputValue = txtInput.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}