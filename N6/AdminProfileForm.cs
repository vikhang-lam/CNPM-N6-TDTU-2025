using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace N6
{
    public partial class AdminProfileForm : Form
    {
        private string username = "admin";  // tên mặc định

        public AdminProfileForm()
        {
            InitializeComponent();
        }

        private void AdminProfileForm_Load(object sender, EventArgs e)
        {
            lblUsername.Text = "Admin";
            picAvatar.Image = Properties.Resources.user_avatar;

            DataRow adminData = DatabaseHelper.GetAdminProfile(username);
            if (adminData != null)
            {
                lblEmail.Text = "Email: " + (adminData["Email"]?.ToString() ?? "admin@school.edu.vn");
            }
            else
            {
                MessageBox.Show("Không thể tải thông tin quản trị viên.", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        // ==== ĐỔI EMAIL ====
        private void btnChangeEmail_Click(object sender, EventArgs e)
        {
            using (Form f = new Form())
            {
                f.Text = "Đổi Email";
                f.Size = new Size(350, 150);
                f.StartPosition = FormStartPosition.CenterParent;
                f.FormBorderStyle = FormBorderStyle.FixedDialog;
                f.MaximizeBox = false;
                f.MinimizeBox = false;

                Label lbl = new Label() { Text = "Nhập email mới:", Location = new Point(10, 20), AutoSize = true };
                TextBox txt = new TextBox() { Location = new Point(10, 50), Width = 300 };
                Button btnOk = new Button() { Text = "Lưu", Location = new Point(220, 80), DialogResult = DialogResult.OK };
                f.Controls.Add(lbl);
                f.Controls.Add(txt);
                f.Controls.Add(btnOk);

                if (f.ShowDialog() == DialogResult.OK)
                {
                    string newEmail = txt.Text.Trim();
                    if (string.IsNullOrEmpty(newEmail))
                    {
                        MessageBox.Show("Email không được để trống!", "Cảnh báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    DatabaseHelper.UpdateAdminEmail(username, newEmail);
                    lblEmail.Text = "Email: " + newEmail;
                    MessageBox.Show("Cập nhật email thành công!", "Thông báo");
                }
            }
        }

        // ==== ĐỔI MẬT KHẨU ====
        private void btnSavePassword_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNewPass.Text))
            {
                MessageBox.Show("Mật khẩu mới không được để trống!", "Cảnh báo");
                return;
            }
            if (txtNewPass.Text != txtConfirmPass.Text)
            {
                MessageBox.Show("Mật khẩu nhập lại không khớp!", "Cảnh báo");
                return;
            }

            bool success = DatabaseHelper.ChangeAdminPassword(username, txtOldPass.Text, txtNewPass.Text);
            if (success)
            {
                MessageBox.Show("Đổi mật khẩu thành công!", "Thông báo");
                txtOldPass.Clear();
                txtNewPass.Clear();
                txtConfirmPass.Clear();
                panelPassword.Visible = false;
            }
            else
            {
                MessageBox.Show("Sai mật khẩu cũ!", "Lỗi");
            }
        }

        private void btnTogglePasswordPanel_Click(object sender, EventArgs e)
        {
            panelPassword.Visible = !panelPassword.Visible;
        }

        private void btnClose_Click(object sender, EventArgs e) => this.Close();
    }
}
