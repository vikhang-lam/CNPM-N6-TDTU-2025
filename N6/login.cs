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
            using (LinearGradientBrush brush = new LinearGradientBrush(
                this.ClientRectangle,
                Color.FromArgb(185, 235, 250),
                Color.FromArgb(120, 200, 235),
                90f))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
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

            string u1 = Properties.Settings.Default["User1"]?.ToString();
            string u2 = Properties.Settings.Default["User2"]?.ToString();
            string u3 = Properties.Settings.Default["User3"]?.ToString();

            AddContentToPanel(paneluser1, string.IsNullOrEmpty(u1) ? "GV1" : u1);
            AddContentToPanel(paneluser2, string.IsNullOrEmpty(u2) ? "GV2" : u2);
            AddContentToPanel(paneluser3, string.IsNullOrEmpty(u3) ? "GV3" : u3);
            AddPlusSignToPanel(paneluser4);
            StoreBaseFonts(this);

            this.paneluser1.Resize += paneluser_Resize;
            this.paneluser2.Resize += paneluser_Resize;
            this.paneluser3.Resize += paneluser_Resize;
            this.paneluser4.Resize += paneluser_Resize;
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

            GraphicsPath path = new GraphicsPath();
            int cornerRadius = 30;

            path.AddArc(0, 0, cornerRadius * 2, cornerRadius * 2, 180, 90);
            path.AddArc(panel.Width - cornerRadius * 2, 0, cornerRadius * 2, cornerRadius * 2, 270, 90);
            path.AddArc(panel.Width - cornerRadius * 2, panel.Height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);
            path.AddArc(0, panel.Height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);
            path.CloseAllFigures();

            panel.Region = new Region(path);
            panel.BackColor = Color.White;
        }

        private void AddContentToPanel(Panel panel, string name, Image avatar = null)
        {
            panel.Controls.Clear();

            PictureBox avatarBox = new PictureBox();
            avatarBox.Tag = "avatar";
            avatarBox.SizeMode = PictureBoxSizeMode.Zoom;
            avatarBox.BackColor = Color.Transparent;
            avatarBox.Image = avatar ?? MakeAvatar();
            panel.Controls.Add(avatarBox);

            Label nameLabel = new Label();
            nameLabel.Tag = "username";
            nameLabel.Text = name;
            nameLabel.ForeColor = Color.FromArgb(55, 71, 79);
            nameLabel.BackColor = Color.Transparent;
            panel.Controls.Add(nameLabel);

            paneluser_Resize(panel, EventArgs.Empty);
        }

        private Bitmap MakeAvatar()
        {
            int size = 120;
            Bitmap bmp = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
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

            Label plusLabel = new Label();
            plusLabel.Text = "+";
            plusLabel.Font = new Font("Segoe UI", 60, FontStyle.Regular);
            plusLabel.ForeColor = Color.FromArgb(189, 189, 189);
            plusLabel.AutoSize = true;
            plusLabel.Location = new Point((panel.Width - plusLabel.Width) / 2, (panel.Height - plusLabel.Height) / 2 - 20);

            Label otherTeacherLabel = new Label();
            otherTeacherLabel.Font = new Font("Segoe UI", 12);
            otherTeacherLabel.ForeColor = Color.FromArgb(97, 97, 97);
            otherTeacherLabel.AutoSize = true;
            otherTeacherLabel.Location = new Point((panel.Width - otherTeacherLabel.Width) / 2, plusLabel.Bottom + 10);

            panel.Controls.Add(plusLabel);
            panel.Controls.Add(otherTeacherLabel);
        }

        private void labelClose_Click(object sender, EventArgs e) => this.Close();
        private void labelMinimize_Click(object sender, EventArgs e) => this.WindowState = FormWindowState.Minimized;
        private void labelMaximize_Click(object sender, EventArgs e) =>
            this.WindowState = this.WindowState == FormWindowState.Normal
                ? FormWindowState.Maximized
                : FormWindowState.Normal;

        private void login_Resize(object sender, EventArgs e)
        {
            if (this.IsDisposed || this.Width <= 0 || this.Height <= 0 || baseFonts.Count == 0) return;
            this.SuspendLayout();
            float scaleX = this.Width / baseWidth;
            float scaleY = this.Height / baseHeight;
            float scaleFactor = Math.Min(scaleX, scaleY);

            ScaleControls(this, scaleFactor);
            this.ResumeLayout();
        }

        private void ScaleControls(Control parent, float factor)
        {
            foreach (Control c in parent.Controls)
            {
                if (baseFonts.ContainsKey(c))
                {
                    float baseSize = baseFonts[c];
                    float newSize = baseSize * factor;

                    if (newSize < baseSize)
                        newSize = baseSize;

                    c.Font = new Font(c.Font.FontFamily, newSize, c.Font.Style);
                }

                if (c.Controls.Count > 0)
                    ScaleControls(c, factor);
            }
        }

        private void StoreBaseFonts(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                if (!baseFonts.ContainsKey(c))
                    baseFonts[c] = c.Font.Size;

                if (c.Controls.Count > 0)
                    StoreBaseFonts(c);
            }
        }

        // ================== LOGIN ==================
        private void paneluser1_Click(object sender, EventArgs e) => HandleUserPanelClick(1);
        private void paneluser2_Click(object sender, EventArgs e) => HandleUserPanelClick(2);
        private void paneluser3_Click(object sender, EventArgs e) => HandleUserPanelClick(3);
        private void paneluser4_Click(object sender, EventArgs e) => HandleUserPanelClick(4);

        private void HandleUserPanelClick(int panelIndex)
        {
            try
            {
                string savedUser = Properties.Settings.Default[$"User{panelIndex}"]?.ToString();

                if (panelIndex == 4 || string.IsNullOrEmpty(savedUser))
                {
                    using (var dlg = new LoginDialog(requireUsername: true))
                    {
                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            string username = dlg.Username?.Trim();
                            string password = dlg.Password?.Trim();

                            if (username.Equals("admin", StringComparison.OrdinalIgnoreCase))
                            {
                                if (DatabaseHelper.CheckAdminLogin(username, password))
                                {
                                    this.DialogResult = DialogResult.OK;
                                    Properties.Settings.Default.isAdmin = true;
                                    Properties.Settings.Default.Save();
                                    this.Close();
                                }
                                else
                                {
                                    MessageBox.Show("Sai tài khoản hoặc mật khẩu admin.");
                                }
                            }
                            else
                            {
                                if (DatabaseHelper.CheckTeacherLogin(username, password))
                                {
                                    if (panelIndex != 4)
                                    {
                                        Properties.Settings.Default[$"User{panelIndex}"] = username;
                                        Properties.Settings.Default.Save();
                                    }
                                    this.DialogResult = DialogResult.OK;
                                    Properties.Settings.Default.isAdmin = false;
                                    Properties.Settings.Default.Save();
                                    this.Close();
                                }
                                else
                                {
                                    MessageBox.Show("Sai tài khoản hoặc mật khẩu giáo viên.");
                                }
                            }
                        }
                    }
                }
                else
                {
                    using (var dlg = new LoginDialog(requireUsername: true, presetUsername: savedUser))
                    {
                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            string username = savedUser.Trim();
                            string password = dlg.Password?.Trim();

                            if (username.Equals("admin", StringComparison.OrdinalIgnoreCase))
                            {
                                if (DatabaseHelper.CheckAdminLogin(username, password))
                                {
                                    this.DialogResult = DialogResult.OK;
                                    Properties.Settings.Default.isAdmin = true;
                                    Properties.Settings.Default.Save();
                                    this.Close();
                                }
                                else
                                {
                                    MessageBox.Show("Sai mật khẩu admin.");
                                }
                            }
                            else
                            {
                                if (DatabaseHelper.CheckTeacherLogin(username, password))
                                {
                                    this.DialogResult = DialogResult.OK;
                                    Properties.Settings.Default.isAdmin = false;
                                    Properties.Settings.Default.Save();
                                    this.Close();
                                }
                                else
                                {
                                    MessageBox.Show("Sai mật khẩu giáo viên.");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hệ thống: " + ex.Message);
            }
        }
    }
}
