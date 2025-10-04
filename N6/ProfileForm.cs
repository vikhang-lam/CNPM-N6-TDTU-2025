using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace N6
{
    public partial class ProfileForm : Form
    {
        private string username;
        // ===> BƯỚC 1: KHAI BÁO EVENT
        public event EventHandler AvatarChanged;

        public ProfileForm(string currentUsername)
        {
            InitializeComponent();
            username = currentUsername;
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.None;
        }

        private void ProfileForm_Load(object sender, EventArgs e)
        {
            LoadTeacherInfo();
        }

        private void LoadTeacherInfo()
        {
            TeacherProfile profile = DatabaseHelper.GetTeacherProfile(username);

            if (profile != null)
            {
                lblName.Text = profile.Ten;
                lblSubject.Text = "Môn: " + (string.IsNullOrEmpty(profile.TenMon) ? "Chưa có" : profile.TenMon);
                lblEmail.Text = "Email: " + (string.IsNullOrEmpty(profile.Email) ? "Chưa có" : profile.Email);
                lblPhone.Text = "SĐT: " + (string.IsNullOrEmpty(profile.SDT) ? "Chưa có" : profile.SDT);

                // Ghép đường dẫn tương đối từ DB với thư mục chạy của ứng dụng
                string avatarPath = Path.Combine(Application.StartupPath, profile.AnhDaiDien ?? "");
                if (!string.IsNullOrWhiteSpace(profile.AnhDaiDien) && File.Exists(avatarPath))
                {
                    // Dùng MemoryStream để tránh khóa file ảnh
                    using (var stream = new MemoryStream(File.ReadAllBytes(avatarPath)))
                    {
                        picAvatar.Image = Image.FromStream(stream);
                    }
                }
                else
                {
                    // Nếu không có ảnh, dùng ảnh mặc định từ Resources
                    picAvatar.Image = Properties.Resources.user_avatar;
                }
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

        // ===> HÀM MỚI: XỬ LÝ SỰ KIỆN ĐỔI ẢNH ĐẠI DIỆN
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
                        // 1. Tạo thư mục 'Avatars' nếu chưa tồn tại
                        string avatarDir = Path.Combine(Application.StartupPath, "Avatars");
                        if (!Directory.Exists(avatarDir))
                        {
                            Directory.CreateDirectory(avatarDir);
                        }

                        // 2. Tạo tên file mới duy nhất và đường dẫn đích
                        string extension = Path.GetExtension(ofd.FileName);
                        string newFileName = $"{username}_{Guid.NewGuid().ToString().Substring(0, 8)}{extension}";
                        string destPath = Path.Combine(avatarDir, newFileName);

                        // 3. Sao chép file ảnh vào thư mục 'Avatars'
                        File.Copy(ofd.FileName, destPath, true);

                        // 4. Cập nhật PictureBox
                        using (var stream = new MemoryStream(File.ReadAllBytes(destPath)))
                        {
                            picAvatar.Image = Image.FromStream(stream);
                        }

                        // 5. Cập nhật đường dẫn tương đối vào database
                        string relativePath = Path.Combine("Avatars", newFileName);
                        DatabaseHelper.UpdateTeacherAvatar(username, relativePath);

                        // ===> BƯỚC 2: KÍCH HOẠT EVENT ĐỂ BÁO CHO FORM MENU
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

        private void btnClose_Click(object sender, EventArgs e) => this.Close();

    }

    // Lớp tùy chỉnh để tạo PictureBox hình tròn
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