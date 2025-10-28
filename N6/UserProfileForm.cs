using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace N6
{
    public partial class UserProfileForm : Form
    {
        private string username;
        private bool isAdmin = false;
        public event EventHandler AvatarChanged;

        public UserProfileForm(string currentUsername)
        {
            InitializeComponent();
            username = currentUsername;
            isAdmin = username.Equals("admin", StringComparison.OrdinalIgnoreCase);

            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.None;
        }

        private void UserProfileForm_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            if (isAdmin)
            {
                LoadAdminInfo();
            }
            else
            {
                LoadTeacherInfo();
            }
        }

        // --- TẢI THÔNG TIN GIÁO VIÊN ---
        private void LoadTeacherInfo()
        {
            lblTitle.Text = "Hồ Sơ Cá Nhân";
            TeacherProfile profile = DatabaseHelper.GetTeacherProfile(username);

            if (profile != null)
            {
                lblName.Text = profile.Ten;
                lblSubject.Text = "Môn: " + (string.IsNullOrEmpty(profile.TenMon) ? "Chưa có" : profile.TenMon);
                lblEmail.Text = "Email: " + (string.IsNullOrEmpty(profile.Email) ? "Chưa có" : profile.Email);
                lblPhone.Text = "SĐT: " + (string.IsNullOrEmpty(profile.SDT) ? "Chưa có" : profile.SDT);

                // Load avatar
                string avatarPath = Path.Combine(Application.StartupPath, profile.AnhDaiDien ?? "");
                if (!string.IsNullOrWhiteSpace(profile.AnhDaiDien) && File.Exists(avatarPath))
                {
                    using (var stream = new MemoryStream(File.ReadAllBytes(avatarPath)))
                    {
                        picAvatar.Image = Image.FromStream(stream);
                    }
                }
                else
                {
                    picAvatar.Image = Properties.Resources.user_avatar;
                }

                // Ẩn/hiện control
                lblSubject.Visible = true;
                lblPhone.Visible = true;
                btnChangeAvatar.Visible = true;
                btnChangeEmail.Visible = false;
            }
            else
            {
                MessageBox.Show("Không tìm thấy thông tin giáo viên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // --- TẢI THÔNG TIN ADMIN ---
        private void LoadAdminInfo()
        {
            lblTitle.Text = "Hồ sơ Quản trị viên";
            picAvatar.Image = Properties.Resources.user_avatar; // Admin dùng avatar mặc định

            DataRow adminData = DatabaseHelper.GetAdminProfile(username);
            if (adminData != null)
            {
                lblName.Text = "Admin"; // Tên admin cố định
                lblEmail.Text = "Email: " + (adminData["Email"]?.ToString() ?? "admin@school.edu.vn");

                // Ẩn/hiện control
                lblSubject.Visible = false;
                lblPhone.Visible = false;
                btnChangeAvatar.Visible = false; // Admin không đổi avatar
                btnChangeEmail.Visible = true;  // Admin được đổi email
            }
            else
            {
                MessageBox.Show("Không thể tải thông tin quản trị viên.", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        // --- CHỨC NĂNG CHUNG: ĐỔI MẬT KHẨU ---
        private void btnTogglePasswordPanel_Click(object sender, EventArgs e)
        {
            panelPassword.Visible = !panelPassword.Visible;
        }

        private void btnSavePassword_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNewPass.Text))
            {
                MessageBox.Show("Mật khẩu mới không được để trống!", "Cảnh báo");
                return;
            }
            if (txtNewPass.Text != txtConfirmPass.Text)
            {
                MessageBox.Show("Mật khẩu nhập lại không khớp!");
                return;
            }

            bool success = false;
            try
            {
                if (isAdmin)
                {
                    success = DatabaseHelper.ChangeAdminPassword(username, txtOldPass.Text, txtNewPass.Text);
                }
                else
                {
                    success = DatabaseHelper.ChangeTeacherPassword(username, txtOldPass.Text, txtNewPass.Text);
                }

                if (success)
                {
                    MessageBox.Show("Đổi mật khẩu thành công!");
                    panelPassword.Visible = false;
                    txtOldPass.Clear();
                    txtNewPass.Clear();
                    txtConfirmPass.Clear();
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

        // --- CHỨC NĂNG RIÊNG: GIÁO VIÊN ĐỔI AVATAR ---
        private void btnChangeAvatar_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
                ofd.Title = "Chọn ảnh đại diện mới";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string avatarDir = Path.Combine(Application.StartupPath, "Avatars");
                        if (!Directory.Exists(avatarDir))
                        {
                            Directory.CreateDirectory(avatarDir);
                        }

                        string extension = Path.GetExtension(ofd.FileName);
                        string newFileName = $"{username}_{Guid.NewGuid().ToString().Substring(0, 8)}{extension}";
                        string destPath = Path.Combine(avatarDir, newFileName);

                        File.Copy(ofd.FileName, destPath, true);

                        using (var stream = new MemoryStream(File.ReadAllBytes(destPath)))
                        {
                            picAvatar.Image = Image.FromStream(stream);
                        }

                        string relativePath = Path.Combine("Avatars", newFileName);
                        DatabaseHelper.UpdateTeacherAvatar(username, relativePath);

                        AvatarChanged?.Invoke(this, EventArgs.Empty);

                        MessageBox.Show("Cập nhật ảnh đại diện thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Có lỗi xảy ra khi đổi ảnh: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // --- CHỨC NĂNG RIÊNG: ADMIN ĐỔI EMAIL ---
        private void btnChangeEmail_Click(object sender, EventArgs e)
        {
            string currentEmail = lblEmail.Text.Replace("Email: ", "").Trim();

            using (var inputBox = new InputBoxForm("Đổi Email", "Vui lòng nhập email mới:", currentEmail))
            {
                if (inputBox.ShowDialog(this) == DialogResult.OK)
                {
                    string newEmail = inputBox.InputValue.Trim();
                    if (string.IsNullOrEmpty(newEmail))
                    {
                        MessageBox.Show("Email không được để trống!", "Cảnh báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (!newEmail.Contains("@") || !newEmail.Contains("."))
                    {
                        MessageBox.Show("Email không hợp lệ!", "Cảnh báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    try
                    {
                        DatabaseHelper.UpdateAdminEmail(username, newEmail);
                        lblEmail.Text = "Email: " + newEmail;
                        MessageBox.Show("Cập nhật email thành công!", "Thông báo");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi cập nhật: " + ex.Message, "Lỗi");
                    }
                }
            }
        }


        // --- NÚT ĐÓNG FORM ---
        private void btnClose_Click(object sender, EventArgs e) => this.Close();
    }

    // Lớp tùy chỉnh để tạo PictureBox hình tròn (giữ nguyên)
    public class CircularPictureBox : PictureBox
    {
        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddEllipse(0, 0, this.Width - 1, this.Height - 1);
                this.Region = new Region(gp);
                pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            }
        }
    }
}