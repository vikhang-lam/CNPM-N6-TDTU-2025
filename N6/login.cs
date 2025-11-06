using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics; 

namespace N6
{
    public partial class login : Form
    {
        private float baseWidth = 1407f;
        private float baseHeight = 782f;
        private Dictionary<Control, float> baseFonts = new Dictionary<Control, float>();

        // --- P/Invoke để di chuyển Form không viền ---
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        // --- Kết thúc P/Invoke ---

        public login()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.White;
            this.Resize += login_Resize;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (this.ClientRectangle.Width > 0 && this.ClientRectangle.Height > 0)
            {
                using (System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    this.ClientRectangle,
                    Color.FromArgb(185, 235, 250),
                    Color.FromArgb(120, 200, 235),
                    90f))
                {
                    e.Graphics.FillRectangle(brush, this.ClientRectangle);
                }
            }
            else
            {
                base.OnPaintBackground(e);
            }
        }

        private void login_Load(object sender, EventArgs e)
        {
            Panel[] panels = { paneluser1, paneluser2, paneluser3, paneluser4 };
            foreach (var panel in panels)
            {
                panel.BorderStyle = BorderStyle.None;
                MakePanelRound(panel);
                // CHUẨN HÓA: Gán một sự kiện Click duy nhất
                panel.Click += panelUser_Click;
                // Gán sự kiện Resize
                panel.Resize += paneluser_Resize;
            }

            labelGreeting.BackColor = Color.Transparent;
            labelInstruction.BackColor = Color.Transparent;

            LoadAndDisplaySavedUsers();
            StoreBaseFonts(this);

            this.pictureBoxAppIcon.MouseClick += pictureBoxAppIcon_MouseClick;

            // Gán sự kiện MouseDown để di chuyển Form
            this.MouseDown += Form_MouseDown;
            this.labelGreeting.MouseDown += Form_MouseDown;
            this.labelInstruction.MouseDown += Form_MouseDown;
            this.pictureBoxAppIcon.MouseDown += Form_MouseDown;
            this.panelTopBar.MouseDown += Form_MouseDown;
        }

        /// <summary>
        /// Xử lý khi click chuột phải vào icon: Xóa cache đăng nhập.
        /// </summary>
        private void pictureBoxAppIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (MessageBox.Show("Bạn có muốn xóa tất cả các tài khoản đã lưu không?",
                                    "Xóa Cache Đăng Nhập",
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    for (int i = 1; i <= 3; i++)
                    {
                        Properties.Settings.Default[$"User{i}"] = "";
                    }
                    Properties.Settings.Default.Save();
                    LoadAndDisplaySavedUsers();
                    MessageBox.Show("Đã xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        /// <summary>
        /// Tải 3 người dùng gần nhất từ Settings và hiển thị lên các panel.
        /// </summary>
        private void LoadAndDisplaySavedUsers()
        {
            string u1 = Properties.Settings.Default["User1"]?.ToString();
            string u2 = Properties.Settings.Default["User2"]?.ToString();
            string u3 = Properties.Settings.Default["User3"]?.ToString();

            AddContentToPanel(paneluser1, string.IsNullOrEmpty(u1) ? "GV1" : u1);
            AddContentToPanel(paneluser2, string.IsNullOrEmpty(u2) ? "GV2" : u2);
            AddContentToPanel(paneluser3, string.IsNullOrEmpty(u3) ? "GV3" : u3);
            AddPlusSignToPanel(paneluser4);
        }

        /// <summary>
        /// Xử lý logic responsive khi panel thay đổi kích thước.
        /// </summary>
        private void paneluser_Resize(object sender, EventArgs e)
        {
            Panel pnl = sender as Panel;
            if (pnl == null || pnl.IsDisposed || pnl.Width <= 0 || pnl.Height <= 0) return;

            PictureBox avatarBox = pnl.Controls.OfType<PictureBox>().FirstOrDefault(c => (string)c.Tag == "avatar");
            Label nameLabel = pnl.Controls.OfType<Label>().FirstOrDefault(c => (string)c.Tag == "username");

            if (avatarBox != null)
            {
                int size = Math.Min(pnl.Width, pnl.Height) / 2;
                avatarBox.Size = new Size(size, size);
                avatarBox.Left = (pnl.Width - avatarBox.Width) / 2;
                avatarBox.Top = pnl.Height / 6;
            }

            if (nameLabel != null)
            {
                float fontSize = Math.Max(12, pnl.Width / 12);
                nameLabel.Font = new Font("Segoe UI", fontSize, FontStyle.Bold);
                nameLabel.AutoSize = true;
                nameLabel.Left = (pnl.Width - nameLabel.Width) / 2;
                nameLabel.Top = avatarBox != null ? avatarBox.Bottom + 20 : pnl.Height - nameLabel.Height - 20;
            }
        }

        /// <summary>
        /// Bo tròn các góc của panel.
        /// </summary>
        private void MakePanelRound(Panel panel)
        {
            if (panel.Width <= 0 || panel.Height <= 0) return;
            using (GraphicsPath path = new GraphicsPath())
            {
                int cornerRadius = 30;
                path.AddArc(0, 0, cornerRadius * 2, cornerRadius * 2, 180, 90);
                path.AddArc(panel.Width - cornerRadius * 2, 0, cornerRadius * 2, cornerRadius * 2, 270, 90);
                path.AddArc(panel.Width - cornerRadius * 2, panel.Height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);
                path.AddArc(0, panel.Height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);
                path.CloseAllFigures();
                panel.Region?.Dispose();
                panel.Region = new Region(path);
                panel.BackColor = Color.White;
            }
        }

        /// <summary>
        /// Thêm avatar và tên vào một panel người dùng.
        /// </summary>
        private void AddContentToPanel(Panel panel, string name, Image avatar = null)
        {
            panel.Controls.Clear();
            PictureBox avatarBox = new PictureBox
            {
                Tag = "avatar",
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
                Image = avatar ?? MakeAvatar()
            };
            panel.Controls.Add(avatarBox);

            Label nameLabel = new Label
            {
                Tag = "username",
                Text = name,
                ForeColor = Color.FromArgb(55, 71, 79),
                BackColor = Color.Transparent
            };
            panel.Controls.Add(nameLabel);
            paneluser_Resize(panel, EventArgs.Empty);
        }

        /// <summary>
        /// Tạo một ảnh avatar mặc định.
        /// </summary>
        private Bitmap MakeAvatar()
        {
            int size = 120;
            Bitmap bmp = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (LinearGradientBrush br = new LinearGradientBrush(
                    new Rectangle(0, 0, size, size),
                    Color.FromArgb(120, 200, 220),
                    Color.FromArgb(120, 140, 160),
                    45f))
                {
                    g.FillEllipse(br, 0, 0, size, size);
                }
                using (Pen p = new Pen(Color.White, 6))
                {
                    g.DrawEllipse(p, 34, 26, 52, 52);
                    g.DrawArc(p, 26, 66, 68, 44, 20, 140);
                }
            }
            return bmp;
        }

        /// <summary>
        /// Thêm dấu cộng vào panel "Người dùng khác".
        /// </summary>
        private void AddPlusSignToPanel(Panel panel)
        {
            panel.Controls.Clear();
            Label plusLabel = new Label
            {
                Text = "+",
                Font = new Font("Segoe UI", 60, FontStyle.Regular),
                ForeColor = Color.FromArgb(189, 189, 189),
                AutoSize = true
            };
            plusLabel.Location = new Point((panel.Width - plusLabel.Width) / 2, (panel.Height - plusLabel.Height) / 2 - 20);

            Label otherTeacherLabel = new Label
            {
                Text = "Giáo viên khác", // Bổ sung Text
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(97, 97, 97),
                AutoSize = true
            };
            otherTeacherLabel.Location = new Point((panel.Width - otherTeacherLabel.Width) / 2, plusLabel.Bottom + 10);
            panel.Controls.Add(plusLabel);
            panel.Controls.Add(otherTeacherLabel);
        }

        // --- Xử lý các nút trên Title Bar ---
        private void labelClose_Click(object sender, EventArgs e) => this.Close();
        private void labelMinimize_Click(object sender, EventArgs e) => this.WindowState = FormWindowState.Minimized;
        private void labelMaximize_Click(object sender, EventArgs e) =>
            this.WindowState = this.WindowState == FormWindowState.Normal ? FormWindowState.Maximized : FormWindowState.Normal;

        // --- Logic Responsive cho Form ---
        private void login_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized) return;
            if (this.IsDisposed || this.Width <= 0 || this.Height <= 0 || baseFonts.Count == 0) return;
            this.SuspendLayout();
            float scaleX = this.Width / baseWidth;
            float scaleY = this.Height / baseHeight;
            float scaleFactor = Math.Min(scaleX, scaleY);
            ScaleControls(this, scaleFactor);
            MakePanelRound(paneluser1);
            MakePanelRound(paneluser2);
            MakePanelRound(paneluser3);
            MakePanelRound(paneluser4);
            this.ResumeLayout();
        }

        private void ScaleControls(Control parent, float factor)
        {
            foreach (Control c in parent.Controls)
            {
                if (baseFonts.ContainsKey(c))
                {
                    float newSize = baseFonts[c] * factor;
                    if (newSize < baseFonts[c]) newSize = baseFonts[c];
                    c.Font = new Font(c.Font.FontFamily, newSize, c.Font.Style);
                }
                if (c.Controls.Count > 0) ScaleControls(c, factor);
            }
        }

        private void StoreBaseFonts(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                if (!baseFonts.ContainsKey(c)) baseFonts[c] = c.Font.Size;
                if (c.Controls.Count > 0) StoreBaseFonts(c);
            }
        }

        // --- Logic xử lý Đăng nhập ---

        /// <summary>
        /// CHUẨN HÓA: Một trình xử lý sự kiện duy nhất cho cả 4 panel người dùng.
        /// </summary>
        private void panelUser_Click(object sender, EventArgs e)
        {
            if (sender == paneluser1) HandleUserPanelClick(1);
            else if (sender == paneluser2) HandleUserPanelClick(2);
            else if (sender == paneluser3) HandleUserPanelClick(3);
            else if (sender == paneluser4) HandleUserPanelClick(4);
        }

        /// <summary>
        /// Cập nhật danh sách người dùng đã lưu (MRU - Most Recently Used).
        /// </summary>
        private void UpdateSavedUsers(string username)
        {
            var users = new List<string>
            {
                Properties.Settings.Default["User1"]?.ToString(),
                Properties.Settings.Default["User2"]?.ToString(),
                Properties.Settings.Default["User3"]?.ToString()
            }.Where(u => !string.IsNullOrEmpty(u)).ToList();

            users.RemoveAll(u => u.Equals(username, StringComparison.OrdinalIgnoreCase));
            users.Insert(0, username);

            var finalUsers = users.Take(3).ToList();
            Properties.Settings.Default["User1"] = finalUsers.ElementAtOrDefault(0) ?? "";
            Properties.Settings.Default["User2"] = finalUsers.ElementAtOrDefault(1) ?? "";
            Properties.Settings.Default["User3"] = finalUsers.ElementAtOrDefault(2) ?? "";
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Hiển thị Form Quên Mật Khẩu dưới dạng Dialog.
        /// </summary>
        private void ShowForgotPasswordForm()
        {
            // Form login (this) sẽ tự động bị vô hiệu hóa
            // cho đến khi form quên mật khẩu đóng lại.
            using (var forgotForm = new ForgotPasswordForm())
            {
                forgotForm.ShowDialog(this);
            }
        }

        /// <summary>
        /// Xử lý logic chính khi một panel người dùng được click.
        /// </summary>
        private void HandleUserPanelClick(int panelIndex)
        {
            try
            {
                string presetUsername = "";
                bool requireUsername = true;

                if (panelIndex >= 1 && panelIndex <= 3)
                {
                    presetUsername = Properties.Settings.Default[$"User{panelIndex}"]?.ToString();
                    if (string.IsNullOrEmpty(presetUsername) || presetUsername.StartsWith("GV"))
                    {
                        presetUsername = "";
                    }
                }

                using (var dlg = new LoginDialog(requireUsername, presetUsername))
                {
                    var dialogResult = dlg.ShowDialog();

                    if (dialogResult == DialogResult.OK)
                    {
                        ProcessLogin(dlg.Username, dlg.Password);
                    }
                    else if (dialogResult == DialogResult.Retry)
                    {
                        ShowRegistrationForm();
                    }
                    else if (dialogResult == DialogResult.Ignore)
                    {
                        ShowForgotPasswordForm();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi không mong muốn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine($"Lỗi HandleUserPanelClick: {ex}");
            }
        }

        /// <summary>
        /// Hiển thị Form Đăng Ký Giáo Viên dưới dạng Dialog.
        /// </summary>
        private void ShowRegistrationForm()
        {
            using (var registrationForm = new TeacherRegistrationForm())
            {
                // Hiển thị form đăng ký. Form login sẽ tự động bị vô hiệu hóa cho đến khi form này đóng.
                registrationForm.ShowDialog(this);
            }

            // Tải lại danh sách người dùng phòng trường hợp admin duyệt luôn
            LoadAndDisplaySavedUsers();
        }

        /// <summary>
        /// Xử lý logic kiểm tra đăng nhập với DatabaseHelper.
        /// </summary>
        private void ProcessLogin(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Tên đăng nhập và mật khẩu không được để trống.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string cleanUsername = username.Trim();
            string cleanPassword = password.Trim();

            try
            {
                // Xử lý đăng nhập Admin
                if (cleanUsername.Equals("admin", StringComparison.OrdinalIgnoreCase))
                {
                    if (DatabaseHelper.CheckAdminLogin(cleanUsername, cleanPassword))
                    {
                        this.DialogResult = DialogResult.OK;
                        Properties.Settings.Default["CurrentUser"] = "admin";
                        Properties.Settings.Default["isAdmin"] = true;
                        Properties.Settings.Default.Save();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Sai tài khoản hoặc mật khẩu ", "Đăng nhập thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }

                // Xử lý đăng nhập Giáo viên
                LoginStatus status = DatabaseHelper.CheckTeacherLogin(cleanUsername, cleanPassword);
                switch (status)
                {
                    case LoginStatus.Success:
                        UpdateSavedUsers(cleanUsername);
                        this.DialogResult = DialogResult.OK;
                        var profile = DatabaseHelper.GetTeacherProfile(cleanUsername);
                        Properties.Settings.Default["CurrentUser"] = (profile != null) ? profile.Ten : cleanUsername;
                        Properties.Settings.Default["isAdmin"] = false;
                        Properties.Settings.Default.Save();
                        this.Close();
                        break;
                    case LoginStatus.AccountNotActivated:
                        MessageBox.Show("Tài khoản của bạn đang chờ quản trị viên xác nhận.", "Tài khoản chưa kích hoạt", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    case LoginStatus.InvalidCredentials:
                        MessageBox.Show("Sai tài khoản hoặc mật khẩu của giáo viên.", "Đăng nhập thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi trong quá trình đăng nhập: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Dọn dẹp tài nguyên và gỡ bỏ các trình xử lý sự kiện.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                // Gỡ các sự kiện của Form
                this.Resize -= login_Resize;
                this.MouseDown -= Form_MouseDown;

                // Gỡ các sự kiện của Control
                if (paneluser1 != null)
                {
                    paneluser1.Resize -= paneluser_Resize;
                    paneluser1.Click -= panelUser_Click; 
                }
                if (paneluser2 != null)
                {
                    paneluser2.Resize -= paneluser_Resize;
                    paneluser2.Click -= panelUser_Click; 
                }
                if (paneluser3 != null)
                {
                    paneluser3.Resize -= paneluser_Resize;
                    paneluser3.Click -= panelUser_Click; 
                }
                if (paneluser4 != null)
                {
                    paneluser4.Resize -= paneluser_Resize; 
                    paneluser4.Click -= panelUser_Click;
                }
                if (pictureBoxAppIcon != null)
                {
                    pictureBoxAppIcon.MouseClick -= pictureBoxAppIcon_MouseClick;
                    pictureBoxAppIcon.MouseDown -= Form_MouseDown;
                }
                if (labelGreeting != null)
                {
                    labelGreeting.MouseDown -= Form_MouseDown;
                }
                if (labelInstruction != null)
                {
                    labelInstruction.MouseDown -= Form_MouseDown;
                }
                if (panelTopBar != null)
                {
                    panelTopBar.MouseDown -= Form_MouseDown;
                }
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}