using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

// Đảm bảo bạn có ảnh trong Resources, ví dụ: Properties.Resources.admin_avatar
// Nếu không có, bạn có thể tạm thời comment dòng gán ảnh lại.
// using N6.Properties; 

namespace N6
{
    public partial class MenuAdmin : Form
    {
        #region Fields

        private bool isMenuCollapsed = false;
        private bool isDarkMode = false;
        private const int menuWidth = 200;
        private const int collapsedMenuWidth = 60;

        private Button currentActiveBtn;
        private ContextMenuStrip userMenu;
        private readonly Dictionary<Button, EventHandler> _buttonHandlers = new Dictionary<Button, EventHandler>();

        // Label cho vai trò sẽ được tạo động
        private Label lblRole;

        private readonly Dictionary<string, Color> lightModeColors = new Dictionary<string, Color>()
        {
            {"mainBg", Color.FromArgb(185, 235, 250)},
            {"menuBg", Color.White},
            {"topBarBg", Color.FromArgb(0, 150, 200)},
            {"textPrimary", Color.Black},
            {"menuBtnText", Color.FromArgb(55, 71, 79)},
            {"menuBtnActiveBg", Color.FromArgb(220, 240, 250)},
            {"btnHover", Color.FromArgb(240, 240, 240)},
            {"userPanelText", Color.Black}
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
            // Di chuyển FormBorderStyle và StartPosition vào đây để đảm bảo chúng được thiết lập trước khi form load
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void MenuAdmin_Load(object sender, EventArgs e)
        {
            CreateAndSetupRoleLabel();
            LoadAdminInfo();
            CreateMenuItems();
            InitUserMenu();
            ApplyTheme();
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

            // Sử dụng đúng tên control từ file Designer của bạn
            lblAdminName.ForeColor = colors["userPanelText"];
            if (lblRole != null) lblRole.ForeColor = colors["userPanelText"];

            foreach (Control c in panelMenu.Controls)
            {
                if (c is Button btn)
                {
                    btn.ForeColor = colors["menuBtnText"];
                    btn.BackColor = colors["menuBg"];
                }
            }

            if (currentActiveBtn != null)
            {
                currentActiveBtn.BackColor = colors["menuBtnActiveBg"];
            }
        }

        #endregion

        #region Menu & UI Creation

        private void CreateAndSetupRoleLabel()
        {
            lblRole = new Label
            {
                Name = "lblRole",
                Text = "Quản trị viên",
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                AutoSize = true,
                // Vị trí sẽ được đặt tương đối so với avatarAdmin và lblAdminName
                Location = new Point(avatarAdmin.Location.X + avatarAdmin.Width + 5, lblAdminName.Location.Y + lblAdminName.Height + 2)
            };
            panelMenu.Controls.Add(lblRole); // Thêm vào panel menu
        }

        private void LoadAdminInfo()
        {
            lblAdminName.Text = "Admin";
            avatarAdmin.Image = Properties.Resources.user_avatar; // dùng avatar mặc định
            avatarAdmin.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void InitUserMenu()
        {
            userMenu = new ContextMenuStrip();
            userMenu.Font = new Font("Segoe UI", 11, FontStyle.Regular);

            ToolStripMenuItem settingsItem = new ToolStripMenuItem("⚙️ Cài đặt");
            settingsItem.Click += SettingsItem_Click;

            ToolStripMenuItem logoutItem = new ToolStripMenuItem("🚪 Đăng xuất");
            logoutItem.Click += (s, e) => btnDangXuat_Click(s, e);

            userMenu.Items.Add(settingsItem);
            userMenu.Items.Add(new ToolStripSeparator());
            userMenu.Items.Add(logoutItem);

            // Gán sự kiện click cho các control hiện có
            avatarAdmin.Click += UserControl_Click;
            lblAdminName.Click += UserControl_Click;
            if (lblRole != null) lblRole.Click += UserControl_Click;
        }

        private void CreateMenuItems()
        {
            var menuItems = new (string, EventHandler)[]
            {
                ("👨‍🏫 Quản lý Giáo viên", btnQuanLyGV_Click),
                ("🏫 Quản lý Lớp học", btnQuanLyLop_Click),
                ("🚪 Đăng xuất", btnDangXuat_Click)
            };

            int topPosition = 120; // Vị trí bắt đầu cho nút đầu tiên, bên dưới khu vực admin

            foreach (var (text, handler) in menuItems)
            {
                Button btn = new Button
                {
                    Text = text,
                    Dock = DockStyle.Top, // Sử dụng DockStyle.Top sẽ dễ hơn
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    Height = 50,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Padding = new Padding(15, 0, 0, 0)
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.Click += GenericMenuButton_Click;

                _buttonHandlers[btn] = handler;

                panelMenu.Controls.Add(btn);
                btn.BringToFront(); // Đảm bảo các nút được xếp chồng đúng thứ tự
            }
        }

        #endregion

        #region Event Handlers

        private void UserControl_Click(object sender, EventArgs e)
        {
            Control control = sender as Control;
            userMenu.Show(control, new Point(0, control.Height));
        }

        private void SettingsItem_Click(object sender, EventArgs e)
        {
            AdminProfileForm profileForm = new AdminProfileForm();
            profileForm.ShowDialog(this);
        }

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
        private void btnQuanLyGV_Click(object sender, EventArgs e)
        {
            panelContent.Controls.Clear();
            UC_QuanLyGiaoVien uc = new UC_QuanLyGiaoVien();
            panelContent.Controls.Add(uc);
            uc.Dock = DockStyle.Fill;
        }
        private void ActivateButton(Button btn)
        {
            if (currentActiveBtn != null)
            {
                currentActiveBtn.BackColor = isDarkMode ? darkModeColors["menuBg"] : lightModeColors["menuBg"];
            }
            currentActiveBtn = btn;
            btn.BackColor = isDarkMode ? darkModeColors["menuBtnActiveBg"] : lightModeColors["menuBtnActiveBg"];
        }

        

        private void btnQuanLyLop_Click(object sender, EventArgs e)
        {
            panelContent.Controls.Clear();
            UC_QuanLyLopHocSinh uc = new UC_QuanLyLopHocSinh();
            panelContent.Controls.Add(uc);
            uc.Dock = DockStyle.Fill;
        }

        private void btnDangXuat_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất không?",
                                                  "Xác nhận Đăng xuất",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                // Bước 1: Xóa thông tin người dùng hiện tại trong Settings
                // Điều này rất quan trọng để Program.cs biết cần quay lại màn hình đăng nhập
                Properties.Settings.Default.CurrentUser = "";
                Properties.Settings.Default.isAdmin = false; // Reset luôn trạng thái admin
                Properties.Settings.Default.Save();

                // Bước 2: Đóng form hiện tại.
                // Program.cs sẽ tự động xử lý việc mở lại form đăng nhập.
                this.Close();
            }
        }

        private void btnToggleMenu_Click(object sender, EventArgs e)
        {
            panelMenu.Width = isMenuCollapsed ? menuWidth : collapsedMenuWidth;
            isMenuCollapsed = !isMenuCollapsed;
        }

        private void btnCollapseMenu_Click(object sender, EventArgs e) => btnToggleMenu_Click(sender, e);

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
    }
}