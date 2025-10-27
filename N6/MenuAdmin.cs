using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

// Đảm bảo bạn có các UserControl này trong dự án:
// using N6.UCs; // Ví dụ

namespace N6
{
    public partial class MenuAdmin : Form
    {
        #region Fields

        private bool isDarkMode = false;
        private Button currentActiveBtn;
        private ContextMenuStrip userMenu;
        private readonly Dictionary<Button, EventHandler> _buttonHandlers = new Dictionary<Button, EventHandler>();

        private readonly Dictionary<string, Color> lightModeColors = new Dictionary<string, Color>()
        {
            {"mainBg", Color.FromArgb(240, 245, 255)},
            {"menuBg", Color.White},
            {"topBarBg", Color.FromArgb(0, 150, 200)},
            {"textPrimary", Color.Black},
            {"menuBtnText", Color.FromArgb(55, 71, 79)},
            {"menuBtnActiveBg", Color.FromArgb(220, 240, 250)},
            {"btnHover", Color.FromArgb(240, 240, 240)},
            {"userPanelText", Color.White}
        };

        private readonly Dictionary<string, Color> darkModeColors = new Dictionary<string, Color>()
        {
            {"mainBg", Color.FromArgb(46, 51, 73)},
            {"menuBg", Color.FromArgb(24, 30, 54)},
            {"topBarBg", Color.FromArgb(46, 51, 73)},
            {"textPrimary", Color.White},
            {"menuBtnText", Color.FromArgb(158, 161, 176)},
            {"menuBtnActiveBg", Color.FromArgb(46, 51, 73)},
            {"btnHover", Color.FromArgb(64, 70, 90)},
            {"userPanelText", Color.White}
        };

        #endregion

        #region Constructor & Load

        public MenuAdmin()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void MenuAdmin_Load(object sender, EventArgs e)
        {
            LoadAdminInfo();
            CreateMenuItems();
            InitUserMenu();
            ApplyTheme();
            // Tải trang chủ Admin làm giao diện mặc định
            LoadAdminHomePage();
        }

        #endregion

        #region Theme Management

        private void ApplyTheme()
        {
            var colors = isDarkMode ? darkModeColors : lightModeColors;

            this.BackColor = colors["mainBg"];
            panelTopBar.BackColor = colors["topBarBg"];
            panelMenu.BackColor = colors["menuBg"];
            panelContent.BackColor = colors["mainBg"];

            lblAdminName.ForeColor = colors["userPanelText"];
            labelAppTitle.ForeColor = colors["userPanelText"];
            labelClose.ForeColor = colors["userPanelText"];
            labelMaximize.ForeColor = colors["userPanelText"];
            labelMinimize.ForeColor = colors["userPanelText"];
            btnThemeToggle.ForeColor = colors["userPanelText"];

            foreach (var btn in _buttonHandlers.Keys)
            {
                bool isActive = (btn == currentActiveBtn);
                btn.ForeColor = isActive ? colors["textPrimary"] : colors["menuBtnText"];
                btn.BackColor = isActive ? colors["menuBtnActiveBg"] : colors["menuBg"];
                btn.FlatAppearance.MouseOverBackColor = isActive ? colors["menuBtnActiveBg"] : colors["btnHover"];
            }
        }

        #endregion

        #region Menu & UI Creation

        private void LoadAdminInfo()
        {
            lblAdminName.Text = "Admin";
            // Giả sử có ảnh user_avatar trong Resources
            avatarAdmin.Image = Properties.Resources.user_avatar;
            avatarAdmin.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void InitUserMenu()
        {
            userMenu = new ContextMenuStrip();
            userMenu.Font = new Font("Segoe UI", 11, FontStyle.Regular);

            ToolStripMenuItem settingsItem = new ToolStripMenuItem("👤 Hồ sơ cá nhân");
            settingsItem.Click += SettingsItem_Click;

            ToolStripMenuItem logoutItem = new ToolStripMenuItem("🚪 Đăng xuất");
            logoutItem.Click += (s, e) => btnDangXuat_Click(s, e);

            userMenu.Items.Add(settingsItem);
            userMenu.Items.Add(new ToolStripSeparator());
            userMenu.Items.Add(logoutItem);

            avatarAdmin.Click += UserControl_Click;
            lblAdminName.Click += UserControl_Click;
        }

        private void CreateMenuItems()
        {
            // Thêm một nút trang chủ vào menu
            var menuItems = new (string, EventHandler)[]
            {
                ("🏠 Trang chủ", btnTrangChu_Click),
                ("👨‍🏫 Quản lý Giáo viên", btnQuanLyGV_Click),
                ("🏫 Quản lý Lớp học", btnQuanLyLop_Click),
                ("Quản Lý Trường học", btnQuanLyTruongHoc_Click),
                ("📊 Báo cáo Admin", btnBaoCaoAdmin_Click),
                ("🚪 Đăng xuất", btnDangXuat_Click)
            };

            foreach (var (text, handler) in menuItems)
            {
                Button btn = new Button
                {
                    Text = text,
                    Dock = DockStyle.Top,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    Height = 50,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Padding = new Padding(15, 0, 0, 0)
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.Click += GenericMenuButton_Click;

                _buttonHandlers[btn] = handler;
                panelMenu.Controls.Add(btn);
                btn.BringToFront();
            }
        }

        #endregion

        #region Event Handlers

        // --- Home Page and Navigation ---
        private void LoadAdminHomePage()
        {
            panelContent.Controls.Clear();
            var homeAdmin = new UC_Home_Admin();
            homeAdmin.Dock = DockStyle.Fill;
            homeAdmin.ChonChucNang += MoChucNangAdmin;
            panelContent.Controls.Add(homeAdmin);

            // Đặt nút trang chủ là nút active
            var homeButton = FindButtonByText("Trang chủ");
            if (homeButton != null) ActivateButton(homeButton);
        }
        private void btnQuanLyTruongHoc_Click(object sender, EventArgs e)
        {
            panelContent.Controls.Clear();
            UC_QuanLyTruongHoc uc = new UC_QuanLyTruongHoc();
            uc.Dock = DockStyle.Fill;
            panelContent.Controls.Add(uc);
            ActivateButton(FindButtonByText("Học vụ & Trường học"));
        }
        private void btnBaoCaoAdmin_Click(object sender, EventArgs e)
        {
            panelContent.Controls.Clear();
            // Tạo instance của UserControl mới (đảm bảo namespace đúng)
            UC_BaoCao_Admin uc = new UC_BaoCao_Admin();
            uc.Dock = DockStyle.Fill;
            panelContent.Controls.Add(uc);
            // Kích hoạt nút menu tương ứng
            ActivateButton(FindButtonByText("Báo cáo Admin"));
        }
        private void MoChucNangAdmin(string maCN)
        {
            switch (maCN)
            {
                case "Admin_QuanLyGV":
                    btnQuanLyGV_Click(this, EventArgs.Empty);
                    break;
                case "Admin_QuanLyLop":
                    btnQuanLyLop_Click(this, EventArgs.Empty);
                    break;
                case "Admin_QuanLyTruongHoc": // Giữ nguyên case này
                    btnQuanLyTruongHoc_Click(this, EventArgs.Empty);
                    break;
                case "Admin_BaoCao": // <<< THÊM CASE NÀY >>>
                    btnBaoCaoAdmin_Click(this, EventArgs.Empty);
                    break;
                case "Admin_DangXuat":
                    btnDangXuat_Click(this, EventArgs.Empty);
                    break;
            }
        }

        // --- Generic and Specific Button Clicks ---
        private void GenericMenuButton_Click(object sender, EventArgs e)
        {
            if (sender is Button btnSender)
            {
                ActivateButton(btnSender);
                if (_buttonHandlers.TryGetValue(btnSender, out var specificHandler))
                {
                    specificHandler(sender, e);
                }
            }
        }

        private void btnTrangChu_Click(object sender, EventArgs e)
        {
            LoadAdminHomePage();
        }

        private void btnQuanLyGV_Click(object sender, EventArgs e)
        {
            panelContent.Controls.Clear();
            // Thay UC_QuanLyGiaoVien bằng tên UserControl thực tế của bạn
            UC_QuanLyGiaoVien uc = new UC_QuanLyGiaoVien();
            uc.Dock = DockStyle.Fill;
            panelContent.Controls.Add(uc);
            ActivateButton(FindButtonByText("Quản lý Giáo viên"));
        }

        private void btnQuanLyLop_Click(object sender, EventArgs e)
        {
            panelContent.Controls.Clear();
            // Thay UC_QuanLyLopHocSinh bằng tên UserControl thực tế của bạn
            UC_QuanLyLopHocSinh uc = new UC_QuanLyLopHocSinh();
            uc.Dock = DockStyle.Fill;
            panelContent.Controls.Add(uc);
            ActivateButton(FindButtonByText("Quản lý Lớp học"));
        }

        private void btnDangXuat_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất không?",
                                                  "Xác nhận Đăng xuất",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Properties.Settings.Default.CurrentUser = "";
                Properties.Settings.Default.isAdmin = false;
                Properties.Settings.Default.Save();
                this.Close(); // Program.cs sẽ xử lý việc mở lại form đăng nhập
            }
        }

        // --- User Menu and Settings ---
        private void UserControl_Click(object sender, EventArgs e)
        {
            if (sender is Control control)
                userMenu.Show(control, new Point(0, control.Height));
        }

        private void SettingsItem_Click(object sender, EventArgs e)
        {
            using (AdminProfileForm pf = new AdminProfileForm())
            {
                pf.ShowDialog(this);
            }
        }

        // --- Theme and Window Controls ---
        private void btnThemeToggle_Click(object sender, EventArgs e)
        {
            isDarkMode = !isDarkMode;
            btnThemeToggle.Text = isDarkMode ? "☀️" : "🌙";
            ApplyTheme();
        }

        private void labelClose_Click(object sender, EventArgs e) => Application.Exit();

        private void labelMinimize_Click(object sender, EventArgs e) => this.WindowState = FormWindowState.Minimized;

        private void labelMaximize_Click(object sender, EventArgs e) =>
            this.WindowState = this.WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;

        #endregion

        #region Helper Methods

        private void ActivateButton(Button btn)
        {
            if (btn == null || btn == currentActiveBtn) return;

            var colors = isDarkMode ? darkModeColors : lightModeColors;

            // Deactivate the old button
            if (currentActiveBtn != null)
            {
                currentActiveBtn.BackColor = colors["menuBg"];
                currentActiveBtn.ForeColor = colors["menuBtnText"];
            }

            // Activate the new button
            currentActiveBtn = btn;
            currentActiveBtn.BackColor = colors["menuBtnActiveBg"];
            currentActiveBtn.ForeColor = colors["textPrimary"];
        }

        private Button FindButtonByText(string text)
        {
            foreach (Button btn in _buttonHandlers.Keys)
            {
                if (btn.Text.Contains(text))
                {
                    return btn;
                }
            }
            return null;
        }

        #endregion
    }
}