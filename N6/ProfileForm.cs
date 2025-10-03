using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace N6
{
    public partial class ProfileForm : Form
    {
        private string username;

        public ProfileForm(string currentUsername)
        {
            InitializeComponent();
            username = currentUsername;
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.None;
        }

        private void ProfileForm_Load(object sender, EventArgs e)
        {
            var profile = DatabaseHelper.GetTeacherProfile(username);
            if (profile != null)
            {
                lblName.Text = profile.Ten;
                lblSubject.Text = "Môn: " + profile.TenMon;
                lblEmail.Text = "Email: " + profile.Email;
                lblPhone.Text = "SĐT: " + profile.SDT;

                if (!string.IsNullOrWhiteSpace(profile.AnhDaiDien) && File.Exists(profile.AnhDaiDien))
                    picAvatar.Image = Image.FromFile(profile.AnhDaiDien);
                else
                    picAvatar.Image = Properties.Resources.user_avatar;
            }
            else
            {
                MessageBox.Show("Không tìm thấy thông tin giáo viên!", "Thông báo");
            }
        }



        private void LoadTeacherInfo()
        {
            TeacherProfile profile = DatabaseHelper.GetTeacherProfile(username);

            if (profile != null)
            {
                lblName.Text = profile.Ten;
                lblSubject.Text = "Môn: " + profile.TenMon;
                lblEmail.Text = "Email: " + (string.IsNullOrEmpty(profile.Email) ? "Chưa có" : profile.Email);
                lblPhone.Text = "SĐT: " + (string.IsNullOrEmpty(profile.SDT) ? "Chưa có" : profile.SDT);

                if (!string.IsNullOrWhiteSpace(profile.AnhDaiDien) && File.Exists(profile.AnhDaiDien))
                    picAvatar.Image = Image.FromFile(profile.AnhDaiDien);
                else
                    picAvatar.Image = Properties.Resources.user_avatar; // ảnh mặc định
            }
            else
            {
                MessageBox.Show("Không tìm thấy thông tin giáo viên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void btnTogglePasswordPanel_Click(object sender, EventArgs e)
        {
            panelPassword.Visible = !panelPassword.Visible;
        }

        private void btnSavePassword_Click(object sender, EventArgs e)
        {
            if (txtNewPass.Text != txtConfirmPass.Text)
            {
                MessageBox.Show("Mật khẩu nhập lại không khớp!");
                return;
            }

            try
            {
                bool ok = DatabaseHelper.ChangeTeacherPassword(username, txtOldPass.Text, txtNewPass.Text);
                if (ok)
                {
                    MessageBox.Show("Đổi mật khẩu thành công!");
                    panelPassword.Visible = false;
                }
                else
                {
                    MessageBox.Show("Sai mật khẩu cũ!");
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hệ thống: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnClose_Click(object sender, EventArgs e) => this.Close();

    }

}
