using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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
            {"menuBtnText", Color.White},
            {"menuBtnActiveBg", Color.FromArgb(46, 51, 73)},
            {"btnHover", Color.FromArgb(64, 70, 90)},
            {"userPanelText", Color.White}
        };

        public dashboard()
        {
            InitializeComponent();
            LoadHome();
            this.DoubleBuffered = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;

            // giữ delegate logoutHandler tránh bị GC
            logoutHandler = LogoutItem_Click;
        }
        private void LoadHome()
        {
            panelMain.Controls.Clear();
            string tenGV = Properties.Settings.Default.CurrentUser?? "Giáo viên";
            var home = new UC_Home(tenGV);
            home.Dock = DockStyle.Fill;
            home.ChonChucNang += MoChucNang;
            panelMain.Controls.Add(home);
        }

        private void MoChucNang(string maCN)
        {
            string user = Properties.Settings.Default["CurrentUser"]?.ToString();
            string maGV = DatabaseHelper.GetMaGVByUsername(user);
            panelMain.Controls.Clear();
            UserControl uc = null;
            switch (maCN)
            {
                case "CN1":
                    
                    UC_Home homeUc = new UC_Home(user ?? "Giáo viên");
                    homeUc.ChonChucNang += MoChucNang; 
                    uc = homeUc;
                    break;
                case "CN2":
                    uc = new UC_QuanLyLop(user);
                    break;
                case "CN3":
                    uc = new UC_ThoiKhoaBieu(maGV);
                    break;
                case "CN4":
                    uc = new UC_QuanLyTaiLieu(maGV);
                    break;
                case "CN5":
                    uc = new UC_MiniGames();
                    break;
                case "CN6":
                    uc = new UC_BaoCao();
                    break;
                case "CN7":
                    uc = new UC_PhanTichAI();
                    break;
                case "CN8":
                    // Đăng xuất
                    DialogResult r = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận", MessageBoxButtons.YesNo);
                    if (r == DialogResult.Yes)
                    {
                        Application.Restart();
                    }
                    return;
            }
            if (uc != null)
            {
                uc.Dock = DockStyle.Fill;
                panelMain.Controls.Add(uc);
            }
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
            string username = Properties.Settings.Default["CurrentUser"]?.ToString();
            if (string.IsNullOrEmpty(username))
            {
                labelUserName.Text = "Người dùng";
                labelSubject.Text = "";
                pictureBoxUser.Image = Properties.Resources.user_avatar;
                return;
            }

            TeacherProfile profile = DatabaseHelper.GetTeacherProfile(username);

            if (profile != null)
            {
                labelUserName.Text = profile.Ten;
                labelSubject.Text = profile.TenMon;

                string avatarPath = Path.Combine(Application.StartupPath, profile.AnhDaiDien ?? "");

                if (!string.IsNullOrWhiteSpace(profile.AnhDaiDien) && File.Exists(avatarPath))
                {
                    try
                    {
                        using (var stream = new MemoryStream(File.ReadAllBytes(avatarPath)))
                        {
                            pictureBoxUser.Image = Image.FromStream(stream);
                        }
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
            }
            else
            {
                labelUserName.Text = "Không tìm thấy";
                labelSubject.Text = "";
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
                pf.AvatarChanged += (s, args) => {
                 LoadUserInfo();
                };
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
            if (!(sender is Button btn)) return;

            string text = btn.Text.Trim();
            string maCN = "";

            // Gán mã chức năng tương ứng với text của nút
            if (text.Contains("Trang chủ")) maCN = "CN1";
            else if (text.Contains("Quản lý lớp học")) maCN = "CN2";
            else if (text.Contains("Thời khóa biểu")) maCN = "CN3";
            else if (text.Contains("quản lí tài liệu")) maCN = "CN4";
            else if (text.Contains("Mini-games")) maCN = "CN5";
            else if (text.Contains("Báo cáo")) maCN = "CN6";
            else if (text.Contains("Phân tích AI")) maCN = "CN7";
            else if (text.Contains("Đăng xuất")) maCN = "CN8";
            else
            {
                MessageBox.Show("Chức năng này đang được phát triển!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Luôn gọi MoChucNang để xử lý việc chuyển đổi UserControl
            // Điều này đảm bảo rằng UC_Home sẽ được tạo và đăng ký sự kiện đúng cách
            MoChucNang(maCN);
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
            foreach (Control c in panelMenu.Controls)
            {
                if (c is Button btn)
                {
                    btn.ForeColor = colors["menuBtnText"];
                    btn.BackColor = colors["menuBg"]; // đảm bảo nền khớp
                }
            }
        }

        private void btnCollapseMenu_Click(object sender, EventArgs e) => btnToggleMenu_Click(sender, e);

        private void labelClose_Click(object sender, EventArgs e) => Application.Exit();
        private void labelMinimize_Click(object sender, EventArgs e) => this.WindowState = FormWindowState.Minimized;
        private void labelMaximize_Click(object sender, EventArgs e) =>
            this.WindowState = this.WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
    }
}
