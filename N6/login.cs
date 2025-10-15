using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace N6
{
    public partial class login : Form
    {
        private float baseWidth = 1407f;
        private float baseHeight = 782f;
        private Dictionary<Control, float> baseFonts = new Dictionary<Control, float>();

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
            paneluser1.BorderStyle = BorderStyle.None;
            paneluser2.BorderStyle = BorderStyle.None;
            paneluser3.BorderStyle = BorderStyle.None;
            paneluser4.BorderStyle = BorderStyle.None;

            labelGreeting.BackColor = Color.Transparent;
            labelInstruction.BackColor = Color.Transparent;

            MakePanelRound(paneluser1);
            MakePanelRound(paneluser2);
            MakePanelRound(paneluser3);
            MakePanelRound(paneluser4);

            LoadAndDisplaySavedUsers();
            StoreBaseFonts(this);

            this.paneluser1.Resize += paneluser_Resize;
            this.paneluser2.Resize += paneluser_Resize;
            this.paneluser3.Resize += paneluser_Resize;
            this.paneluser4.Resize += paneluser_Resize;

            this.paneluser1.Click += new System.EventHandler(this.paneluser1_Click);
            this.paneluser2.Click += new System.EventHandler(this.paneluser2_Click);
            this.paneluser3.Click += new System.EventHandler(this.paneluser3_Click);
            this.paneluser4.Click += new System.EventHandler(this.paneluser4_Click);

            this.pictureBoxAppIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBoxAppIcon_MouseClick);

            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);
            this.labelGreeting.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);
            this.labelInstruction.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);
            this.pictureBoxAppIcon.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);
            this.panelTopBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);
        }

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
                Text = "Giáo viên khác",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(97, 97, 97),
                AutoSize = true
            };
            otherTeacherLabel.Location = new Point((panel.Width - otherTeacherLabel.Width) / 2, plusLabel.Bottom + 10);
            panel.Controls.Add(plusLabel);
            panel.Controls.Add(otherTeacherLabel);
        }

        private void labelClose_Click(object sender, EventArgs e) => this.Close();
        private void labelMinimize_Click(object sender, EventArgs e) => this.WindowState = FormWindowState.Minimized;
        private void labelMaximize_Click(object sender, EventArgs e) =>
            this.WindowState = this.WindowState == FormWindowState.Normal ? FormWindowState.Maximized : FormWindowState.Normal;

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

        private void paneluser1_Click(object sender, EventArgs e) => HandleUserPanelClick(1);
        private void paneluser2_Click(object sender, EventArgs e) => HandleUserPanelClick(2);
        private void paneluser3_Click(object sender, EventArgs e) => HandleUserPanelClick(3);
        private void paneluser4_Click(object sender, EventArgs e) => HandleUserPanelClick(4);

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
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hệ thống: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // === PHƯƠNG THỨC ĐÃ ĐƯỢC CẬP NHẬT ===
        private void ShowRegistrationForm()
        {
            // Không ẩn form login nữa
            // this.Visible = false; 

            using (var registrationForm = new TeacherRegistrationForm())
            {
                // Hiển thị form đăng ký. Form login sẽ tự động bị vô hiệu hóa cho đến khi form này đóng.
                registrationForm.ShowDialog(this);
            }

            // Không cần hiện lại form login
            // this.Visible = true;

            // Chỉ cần tải lại danh sách người dùng
            LoadAndDisplaySavedUsers();
        }

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
                if (cleanUsername.Equals("admin", StringComparison.OrdinalIgnoreCase))
                {
                    if (DatabaseHelper.CheckAdminLogin(cleanUsername, cleanPassword))
                    {
                        this.DialogResult = DialogResult.OK;
                        Properties.Settings.Default["CurrentUser"] = "Admin";
                        Properties.Settings.Default["isAdmin"] = true;
                        Properties.Settings.Default.Save();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Sai tài khoản hoặc mật khẩu của Admin.", "Đăng nhập thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }

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
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                this.Resize -= login_Resize;
                this.MouseDown -= Form_MouseDown;

                if (paneluser1 != null)
                {
                    paneluser1.Resize -= paneluser_Resize;
                    paneluser1.Click -= paneluser1_Click;
                }
                if (paneluser2 != null)
                {
                    paneluser2.Resize -= paneluser_Resize;
                    paneluser2.Click -= paneluser2_Click;
                }
                if (paneluser3 != null)
                {
                    paneluser3.Resize -= paneluser_Resize;
                    paneluser3.Click -= paneluser3_Click;
                }
                if (paneluser4 != null)
                {
                    paneluser4.Resize -= paneluser_Resize;
                    paneluser4.Click -= paneluser4_Click;
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