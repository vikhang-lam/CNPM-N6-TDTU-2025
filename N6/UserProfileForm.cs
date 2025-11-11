using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics; // Thêm

namespace N6
{
    /// <summary>
    /// Form hiển thị và cho phép chỉnh sửa thông tin cá nhân (Giáo viên hoặc Admin).
    /// </summary>
    public partial class UserProfileForm : Form
    {
        private string username;
        private bool isAdmin = false;

        /// <summary>
        /// Sự kiện được kích hoạt khi giáo viên thay đổi ảnh đại diện.
        /// </summary>
        public event EventHandler AvatarChanged;

        public UserProfileForm(string currentUsername)
        {
            InitializeComponent();
            username = currentUsername;
            isAdmin = username.Equals("admin", StringComparison.OrdinalIgnoreCase);

            this.Padding = new Padding(5);
            this.BackColor = Color.LightGray;
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.None;
        }

        private void UserProfileForm_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// Tải thông tin dựa trên loại tài khoản (Admin hoặc Giáo viên).
        /// </summary>
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

        #region Load Data (Tải dữ liệu)

        /// <summary>
        /// Tải thông tin cho tài khoản Giáo viên.
        /// </summary>
        private void LoadTeacherInfo()
        {
            lblTitle.Text = "Hồ Sơ Cá Nhân";
            try
            {
                TeacherProfile profile = DatabaseHelper.GetTeacherProfile(username);
                if (profile == null)
                {
                    MessageBox.Show("Không tìm thấy thông tin giáo viên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                lblName.Text = profile.Ten;
                lblSubject.Text = "Môn: " + (string.IsNullOrEmpty(profile.TenMon) ? "Chưa có" : profile.TenMon);
                lblEmail.Text = "Email: " + (string.IsNullOrEmpty(profile.Email) ? "Chưa có" : profile.Email);
                lblPhone.Text = "SĐT: " + (string.IsNullOrEmpty(profile.SDT) ? "Chưa có" : profile.SDT);

                int margin = 5; // Khoảng cách giữa các label
                lblEmail.Top = lblSubject.Bottom + margin;
                lblPhone.Top = lblEmail.Bottom + margin;

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
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải hồ sơ giáo viên: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Tải thông tin cho tài khoản Admin.
        /// </summary>
        private void LoadAdminInfo()
        {
            lblTitle.Text = "Hồ sơ Quản trị viên";
            picAvatar.Image = Properties.Resources.user_avatar; // Admin dùng avatar mặc định

            try
            {
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
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải hồ sơ admin: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine(ex.ToString());
            }
        }

        #endregion

        #region User Actions (Hành động người dùng)

        /// <summary>
        /// Ẩn/hiện panel đổi mật khẩu.
        /// </summary>
        private void btnTogglePasswordPanel_Click(object sender, EventArgs e)
        {
            panelPassword.Visible = !panelPassword.Visible;
        }

        /// <summary>
        /// Xử lý lưu mật khẩu mới.
        /// </summary>
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
            catch (ArgumentException ex) // Bắt lỗi nghiệp vụ (ví dụ: mật khẩu chứa ký tự đặc biệt)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex) // Bắt lỗi hệ thống
            {
                MessageBox.Show("Lỗi hệ thống: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// (Giáo viên) Xử lý đổi ảnh đại diện.
        /// </summary>
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
                        // Tạo thư mục nếu chưa có
                        string avatarDir = Path.Combine(Application.StartupPath, "Avatars");
                        if (!Directory.Exists(avatarDir))
                        {
                            Directory.CreateDirectory(avatarDir);
                        }

                        // Tạo tên file mới, duy nhất
                        string extension = Path.GetExtension(ofd.FileName);
                        string newFileName = $"{username}_{Guid.NewGuid().ToString().Substring(0, 8)}{extension}";
                        string destPath = Path.Combine(avatarDir, newFileName);

                        File.Copy(ofd.FileName, destPath, true);

                        // Hiển thị ảnh mới (load từ file vừa copy)
                        using (var stream = new MemoryStream(File.ReadAllBytes(destPath)))
                        {
                            picAvatar.Image = Image.FromStream(stream);
                        }

                        // Lưu đường dẫn tương đối vào CSDL
                        string relativePath = Path.Combine("Avatars", newFileName);
                        DatabaseHelper.UpdateTeacherAvatar(username, relativePath);

                        // Phát sự kiện để Dashboard (form cha) cập nhật avatar
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

        /// <summary>
        /// (Admin) Xử lý đổi Email.
        /// </summary>
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
                        MessageBox.Show("Email không được để trống!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (!newEmail.Contains("@") || !newEmail.Contains("."))
                    {
                        MessageBox.Show("Email không hợp lệ!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        /// <summary>
        /// Đóng Form.
        /// </summary>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        /// <summary>
        /// Dọn dẹp tài nguyên và gỡ bỏ các trình xử lý sự kiện.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Gỡ bỏ các sự kiện đã gán thủ công
                this.Load -= UserProfileForm_Load;
                if (this.btnTogglePasswordPanel != null) this.btnTogglePasswordPanel.Click -= btnTogglePasswordPanel_Click;
                if (this.btnSavePassword != null) this.btnSavePassword.Click -= btnSavePassword_Click;
                if (this.btnChangeAvatar != null) this.btnChangeAvatar.Click -= btnChangeAvatar_Click;
                if (this.btnChangeEmail != null) this.btnChangeEmail.Click -= btnChangeEmail_Click;
                if (this.btnClose != null) this.btnClose.Click -= btnClose_Click;

                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// Lớp PictureBox tùy chỉnh để hiển thị ảnh hình tròn.
    /// </summary>
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