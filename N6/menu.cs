using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace N6
{
    public partial class dashboard : Form
    {
        private bool isMenuCollapsed = false;
        private bool isDarkMode = false;
        private const int menuWidth = 200;
        private const int collapsedMenuWidth = 60;

        // Giữ delegate (tránh GC thu gom rác gây lỗi CallbackOnCollectedDelegate)
        private EventHandler logoutHandler;

        private Dictionary<string, Color> lightModeColors = new Dictionary<string, Color>()
        {
            {"mainBg", Color.FromArgb(185, 235, 250)},
            {"menuBg", Color.White},
            {"topBarBg", Color.FromArgb(0, 150, 200)},
            {"textPrimary", Color.Black},
            {"textSecondary", Color.Gray},
            {"menuBtnText", Color.FromArgb(55, 71, 79)},
            {"menuBtnActiveBg", Color.FromArgb(220, 240, 250)},
            {"btnHover", Color.FromArgb(240, 240, 240)},
            {"userPanelText", Color.Black}
        };

        private Dictionary<string, Color> darkModeColors = new Dictionary<string, Color>()
        {
            {"mainBg", Color.FromArgb(46, 51, 73)},
            {"menuBg", Color.FromArgb(24, 30, 54)},
            {"topBarBg", Color.FromArgb(46, 51, 73)},
            {"textPrimary", Color.White},
            {"textSecondary", Color.FromArgb(158, 161, 176)},
            {"menuBtnText", Color.FromArgb(158, 161, 176)},
            {"menuBtnActiveBg", Color.FromArgb(46, 51, 73)},
            {"btnHover", Color.FromArgb(64, 70, 90)},
            {"userPanelText", Color.White}
        };

        public dashboard()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;

            // giữ delegate logoutHandler tránh bị GC
            logoutHandler = LogoutItem_Click;
        }

        private void dashboard_Load(object sender, EventArgs e)
        {
            LoadUserInfo();
            CreateMainMenuItems();
            ApplyTheme();
            InitUserMenu();
        }

        private void LoadUserInfo()
        {
            string userName = Properties.Settings.Default["CurrentUser"]?.ToString();
            string subject = Properties.Settings.Default["CurrentSubject"]?.ToString();
            if (string.IsNullOrWhiteSpace(userName)) userName = "Người dùng";
            if (string.IsNullOrWhiteSpace(subject)) subject = "";
            labelUserName.Text = userName;
            labelSubject.Text = subject;

            string avatarPath = Properties.Settings.Default["CurrentUserAvatar"]?.ToString();

            if (!string.IsNullOrWhiteSpace(avatarPath) && System.IO.File.Exists(avatarPath))
            {
                try
                {
                    pictureBoxUser.Image = Image.FromFile(avatarPath);
                }
                catch
                {
                    pictureBoxUser.Image = Properties.Resources.user_avatar;
                }
            }
            else
            {
                pictureBoxUser.Image = Properties.Resources.user_avatar;
            }

            pictureBoxUser.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxUser.Region = new Region(new Rectangle(0, 0, pictureBoxUser.Width, pictureBoxUser.Height));
        }

        private ContextMenuStrip userMenu;

        private void InitUserMenu()
        {
            userMenu = new ContextMenuStrip();
            userMenu.Font = new Font("Segoe UI", 11, FontStyle.Regular);

            ToolStripMenuItem profileItem = new ToolStripMenuItem("👤 Hồ sơ cá nhân");
            profileItem.Click += ProfileItem_Click;

            ToolStripMenuItem settingsItem = new ToolStripMenuItem("⚙️ Cài đặt");
            settingsItem.Click += SettingsItem_Click;

            ToolStripMenuItem logoutItem = new ToolStripMenuItem("🚪 Đăng xuất");
            logoutItem.Click += logoutHandler; // dùng field delegate đã lưu

            userMenu.Items.Add(profileItem);
            userMenu.Items.Add(settingsItem);
            userMenu.Items.Add(new ToolStripSeparator());
            userMenu.Items.Add(logoutItem);

            // Gắn context menu cho avatar + tên
            pictureBoxUser.Click += PictureBoxUser_Click;
            labelUserName.Click += LabelUserName_Click;
        }

        // ================== HANDLER ==================
        private void ProfileItem_Click(object sender, EventArgs e)
        {
            string currentUser = Properties.Settings.Default["CurrentUser"]?.ToString();
            using (ProfileForm pf = new ProfileForm(currentUser))
            {
                pf.ShowDialog(this);
            }
        }

        private void SettingsItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Mở trang cài đặt");
        }

        private void LogoutItem_Click(object sender, EventArgs e)
        {
            // Gọi logout từ menu chính
            MenuItem_Click(new Button { Text = "🚪 Đăng xuất" }, EventArgs.Empty);
        }

        private void PictureBoxUser_Click(object sender, EventArgs e)
        {
            userMenu.Show(pictureBoxUser, new Point(0, pictureBoxUser.Height));
        }

        private void LabelUserName_Click(object sender, EventArgs e)
        {
            userMenu.Show(labelUserName, new Point(0, labelUserName.Height));
        }

        private void CreateMainMenuItems()
        {
            var menuItems = new string[]
            {
                "🚪 Đăng xuất",
                "📊 Phân tích AI",
                "📑 Báo cáo & Xuất dữ liệu",
                "🎮 Mini-games",
                "📑 quản lí tài liệu",
                "☁️ Thời khóa biểu",
                "👨‍🎓 Quản lý lớp học",
                "🏠 Trang chủ"
            };

            foreach (var item in menuItems)
            {
                Button btn = new Button();
                btn.Text = item;
                btn.Dock = DockStyle.Top;
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
                btn.Font = new Font("Segoe UI", 11, FontStyle.Bold);
                btn.Height = 55;
                btn.TextAlign = ContentAlignment.MiddleLeft;
                btn.Padding = new Padding(15, 0, 0, 0);
                btn.Click += MenuItem_Click;

                panelMenu.Controls.Add(btn);
                panelMenu.Controls.SetChildIndex(btn, panelMenu.Controls.Count - 1);
            }
        }

        private void MenuItem_Click(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                if (btn.Text.Contains("Quản lý lớp học"))
                {
                    panelContent.Controls.Clear();
                    string currentUser = Properties.Settings.Default["CurrentUser"]?.ToString();
                    UC_QuanLyLop uc = new UC_QuanLyLop(currentUser);
                    uc.Dock = DockStyle.Fill;
                    panelContent.Controls.Add(uc);
                    return;
                }

                // Xử lý Đăng xuất
                if (btn.Text.Contains("Đăng xuất"))
                {
                    DialogResult result = MessageBox.Show(
                        "Bạn có chắc chắn muốn đăng xuất không?",
                        "Xác nhận",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        Properties.Settings.Default["CurrentUser"] = "";
                        Properties.Settings.Default["CurrentSubject"] = "";
                        Properties.Settings.Default["CurrentUserAvatar"] = "";
                        Properties.Settings.Default.Save();

                        this.Close(); // chỉ Close, Program.cs sẽ quay về login
                    }
                    return;
                }
                if (btn.Text.Contains("quản lí tài liệu"))
                {
                    panelContent.Controls.Clear();
                    string user = Properties.Settings.Default["CurrentUser"]?.ToString();
                    string maGV = DatabaseHelper.GetMaGVByUsername(user);
                    UC_QuanLyTaiLieu uc = new UC_QuanLyTaiLieu(maGV);
                    uc.Dock = DockStyle.Fill;
                    panelContent.Controls.Add(uc);
                    return;
                }


            }
        }

        private void btnToggleMenu_Click(object sender, EventArgs e)
        {
            panelMenu.Width = isMenuCollapsed ? menuWidth : collapsedMenuWidth;
            isMenuCollapsed = !isMenuCollapsed;
        }

        private void btnThemeToggle_Click(object sender, EventArgs e)
        {
            isDarkMode = !isDarkMode;
            btnThemeToggle.Text = isDarkMode ? "☀️" : "🌙";
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            var colors = isDarkMode ? darkModeColors : lightModeColors;

            this.BackColor = colors["mainBg"];
            panelTopBar.BackColor = colors["topBarBg"];
            panelMenu.BackColor = colors["menuBg"];
            panelContent.BackColor = colors["mainBg"];

            labelAppTitle.ForeColor = colors["textPrimary"];
            labelUserName.ForeColor = colors["userPanelText"];
            labelSubject.ForeColor = colors["userPanelText"];
        }

        private void btnCollapseMenu_Click(object sender, EventArgs e) => btnToggleMenu_Click(sender, e);

        private void labelClose_Click(object sender, EventArgs e) => Application.Exit();
        private void labelMinimize_Click(object sender, EventArgs e) => this.WindowState = FormWindowState.Minimized;
        private void labelMaximize_Click(object sender, EventArgs e) =>
            this.WindowState = this.WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
    }
}
