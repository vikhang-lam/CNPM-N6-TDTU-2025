using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace N6
{
    public partial class login : Form
    {
        public login()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (this.ClientRectangle.Width == 0 || this.ClientRectangle.Height == 0)
            {
                return;
            }

            using (LinearGradientBrush brush = new LinearGradientBrush(
                this.ClientRectangle,
                Color.FromArgb(179, 102, 255), // Tím nhạt
                Color.FromArgb(255, 102, 204), // Hồng
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

            // Lấy user đã lưu
            string u1 = Properties.Settings.Default["User1"]?.ToString();
            string u2 = Properties.Settings.Default["User2"]?.ToString();
            string u3 = Properties.Settings.Default["User3"]?.ToString();

            AddContentToPanel(paneluser1, string.IsNullOrEmpty(u1) ? "GV1" : u1);
            AddContentToPanel(paneluser2, string.IsNullOrEmpty(u2) ? "GV2" : u2);
            AddContentToPanel(paneluser3, string.IsNullOrEmpty(u3) ? "GV3" : u3);
            AddPlusSignToPanel(paneluser4);
        }

        private void MakePanelRound(Panel panel)
        {
            if (panel.Width <= 0 || panel.Height <= 0)
                return;

            GraphicsPath path = new GraphicsPath();
            int cornerRadius = 30;

            path.AddArc(0, 0, cornerRadius * 2, cornerRadius * 2, 180, 90);
            path.AddArc(panel.Width - cornerRadius * 2, 0, cornerRadius * 2, cornerRadius * 2, 270, 90);
            path.AddArc(panel.Width - cornerRadius * 2, panel.Height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);
            path.AddArc(0, panel.Height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);
            path.CloseAllFigures();

            panel.Region = new Region(path);
        }

        private void AddContentToPanel(Panel panel, string name, Image avatar = null)
        {
            panel.Controls.Clear();

            PictureBox avatarBox = new PictureBox();
            avatarBox.Size = new Size(120, 120);
            avatarBox.Location = new Point((panel.Width - avatarBox.Width) / 2, 30);
            avatarBox.SizeMode = PictureBoxSizeMode.Zoom;
            avatarBox.BackColor = Color.Transparent;

            // Nếu có avatar thì dùng, không thì tạo avatar mặc định gradient
            if (avatar != null)
                avatarBox.Image = avatar;
            else
                avatarBox.Image = MakeAvatar(); // giống hàm trong LoginDialog.cs

            // bo tròn
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddEllipse(0, 0, avatarBox.Width - 1, avatarBox.Height - 1);
                avatarBox.Region = new Region(path);
            }

            Label nameLabel = new Label();
            nameLabel.Text = name;
            nameLabel.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            nameLabel.ForeColor = Color.Black;
            nameLabel.AutoSize = true;
            nameLabel.BackColor = Color.Transparent;
            nameLabel.Location = new Point((panel.Width - nameLabel.Width) / 2, avatarBox.Bottom + 15);

            panel.Controls.Add(avatarBox);
            panel.Controls.Add(nameLabel);
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
                    Color.FromArgb(155, 81, 224),
                    Color.FromArgb(244, 143, 177),
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
            plusLabel.ForeColor = Color.LightGray;
            plusLabel.AutoSize = true;
            plusLabel.Location = new Point((panel.Width - plusLabel.Width) / 2, (panel.Height - plusLabel.Height) / 2 - 20);

            Label otherTeacherLabel = new Label();
            otherTeacherLabel.Text = "Giáo viên khác";
            otherTeacherLabel.Font = new Font("Segoe UI", 12);
            otherTeacherLabel.ForeColor = Color.DimGray;
            otherTeacherLabel.AutoSize = true;
            otherTeacherLabel.Location = new Point((panel.Width - otherTeacherLabel.Width) / 2, plusLabel.Bottom + 10);

            panel.Controls.Add(plusLabel);
            panel.Controls.Add(otherTeacherLabel);
        }

        // --- Hiệu ứng nháy sáng khi di chuột ---
        private void paneluser_MouseEnter(object sender, EventArgs e)
        {
            Panel panel = sender as Panel;
            if (panel != null)
            {
                panel.BackColor = Color.FromArgb(240, 240, 240); // Sáng hơn
            }
        }

        private void paneluser_MouseLeave(object sender, EventArgs e)
        {
            Panel panel = sender as Panel;
            if (panel != null)
            {
                panel.BackColor = Color.White; // Trả lại màu trắng ban đầu
            }
        }

        // --- Kéo form không cần DllImport ---
        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x84;
            const int HTCLIENT = 1;
            const int HTCAPTION = 2;

            if (m.Msg == WM_NCHITTEST)
            {
                base.WndProc(ref m);
                if ((int)m.Result == HTCLIENT)
                    m.Result = (IntPtr)HTCAPTION;
                return;
            }
            base.WndProc(ref m);
        }


        private void labelClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void labelMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void labelMaximize_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e) { }
        private void panel4_Paint(object sender, PaintEventArgs e) { }
        private void panel2_Paint(object sender, PaintEventArgs e) { }
        private void panel3_Paint(object sender, PaintEventArgs e) { }
        private void paneluser1_Paint(object sender, PaintEventArgs e) { }

        // ================== KẾT NỐI VỚI DATABASE ==================
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
                    // Nhập user + pass
                    using (var dlg = new LoginDialog(requireUsername: true))
                    {
                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            string username = dlg.Username?.Trim();
                            string password = dlg.Password?.Trim();

                            try
                            {
                                if (username.Equals("admin", StringComparison.OrdinalIgnoreCase))
                                {
                                    // --- Kiểm tra admin ---
                                    if (DatabaseHelper.CheckAdminLogin(username, password))
                                    {
                                        MenuAdmin adminForm = new MenuAdmin();
                                        adminForm.Show();
                                        this.Hide();
                                    }
                                    else
                                    {
                                        MessageBox.Show("Sai tài khoản hoặc mật khẩu admin.",
                                            "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                }
                                else
                                {
                                    // --- Kiểm tra giáo viên ---
                                    if (DatabaseHelper.CheckTeacherLogin(username, password))
                                    {
                                        if (panelIndex != 4)
                                        {
                                            Properties.Settings.Default[$"User{panelIndex}"] = username;
                                            Properties.Settings.Default.Save();
                                        }

                                        dashboard dash = new dashboard();
                                        dash.Show();
                                        this.Hide();
                                    }
                                    else
                                    {
                                        MessageBox.Show("Sai tài khoản hoặc mật khẩu giáo viên.",
                                            "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Lỗi khi kiểm tra tài khoản: " + ex.Message,
                                    "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                else
                {
                    // Đã lưu user → chỉ nhập pass
                    using (var dlg = new LoginDialog(requireUsername: false, presetUsername: savedUser))
                    {
                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            string username = savedUser.Trim();
                            string password = dlg.Password?.Trim();

                            try
                            {
                                if (username.Equals("admin", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (DatabaseHelper.CheckAdminLogin(username, password))
                                    {
                                        MenuAdmin adminForm = new MenuAdmin();
                                        adminForm.Show();
                                        this.Hide();
                                    }
                                    else
                                    {
                                        MessageBox.Show("Sai mật khẩu admin.",
                                            "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                }
                                else
                                {
                                    if (DatabaseHelper.CheckTeacherLogin(username, password))
                                    {
                                        dashboard dash = new dashboard();
                                        dash.Show();
                                        this.Hide();
                                    }
                                    else
                                    {
                                        MessageBox.Show("Sai mật khẩu giáo viên.",
                                            "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Lỗi khi kiểm tra mật khẩu: " + ex.Message,
                                    "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi không mong muốn: " + ex.Message,
                    "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}