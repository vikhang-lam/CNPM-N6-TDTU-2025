using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq; // Thêm
using System.Diagnostics; // Thêm

// Đảm bảo bạn có các UserControl này trong dự án:
// using N6.UCs; // Ví dụ

namespace N6
{
    /// <summary>
    /// Form chính (Dashboard) dành cho Quản trị viên (Admin).
    /// </summary>
    public partial class MenuAdmin : Form
    {
        #region Fields (Biến thành viên)

        private bool isDarkMode = false;
        private Button currentActiveBtn;
        private ContextMenuStrip userMenu;
        private ToolStripMenuItem settingsItem; // Biến class để gỡ sự kiện
        private ToolStripMenuItem logoutItem; // Biến class để gỡ sự kiện
        private readonly Dictionary<Button, EventHandler> _buttonHandlers = new Dictionary<Button, EventHandler>();

        // Bảng màu (Sáng)
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

        // Bảng màu (Tối)
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
            CreateMenuItems(); // Tạo các nút menu động
            InitUserMenu();
            ApplyTheme();
            // Tải trang chủ Admin làm giao diện mặc định
            LoadAdminHomePage();
        }

        #endregion

        #region Theme Management (Quản lý Giao diện)

        /// <summary>
        /// Áp dụng bảng màu (Sáng/Tối) cho toàn bộ Form.
        /// </summary>
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

            // Áp dụng màu cho các nút menu động
            foreach (var btn in _buttonHandlers.Keys)
            {
                bool isActive = (btn == currentActiveBtn);
                btn.ForeColor = isActive ? colors["textPrimary"] : colors["menuBtnText"];
                btn.BackColor = isActive ? colors["menuBtnActiveBg"] : colors["menuBg"];
                btn.FlatAppearance.MouseOverBackColor = isActive ? colors["menuBtnActiveBg"] : colors["btnHover"];
            }
        }

        #endregion

        #region Menu & UI Creation (Tạo Menu & UI)

        /// <summary>
        /// Tải thông tin cơ bản của Admin lên TopBar.
        /// </summary>
        private void LoadAdminInfo()
        {
            lblAdminName.Text = "Admin";
            // Giả sử có ảnh user_avatar trong Resources
            avatarAdmin.Image = Properties.Resources.user_avatar;
            avatarAdmin.SizeMode = PictureBoxSizeMode.Zoom;
        }

        /// <summary>
        /// Khởi tạo ContextMenuStrip (menu chuột phải) cho avatar Admin.
        /// </summary>
        private void InitUserMenu()
        {
            userMenu = new ContextMenuStrip();
            userMenu.Font = new Font("Segoe UI", 11, FontStyle.Regular);

            settingsItem = new ToolStripMenuItem("👤 Hồ sơ cá nhân");
            settingsItem.Click += SettingsItem_Click;

            logoutItem = new ToolStripMenuItem("🚪 Đăng xuất");
            logoutItem.Click += (s, e) => btnDangXuat_Click(s, e);

            userMenu.Items.Add(settingsItem);
            userMenu.Items.Add(new ToolStripSeparator());
            userMenu.Items.Add(logoutItem);
        }

        /// <summary>
        /// Tạo động các nút chức năng trong menu chính bên trái.
        /// </summary>
        private void CreateMenuItems()
        {
            panelMenu.Controls.Clear();
            _buttonHandlers.Clear();

            var menuItems = new (string, EventHandler)[]
            {
                ("🏠 Trang chủ", btnTrangChu_Click),
                ("👨‍🏫 Quản lý Giáo viên", btnQuanLyGV_Click),
                ("🏫 Quản lý Lớp học", btnQuanLyLop_Click),
                ("🎓 Quản Lý Trường học", btnQuanLyTruongHoc_Click),
                ("📊 Báo cáo Admin", btnBaoCaoAdmin_Click),
                ("🚪 Đăng xuất", btnDangXuat_Click)
            };

            // Thêm các nút từ dưới lên (để "Trang chủ" ở trên cùng)
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

                _buttonHandlers[btn] = handler; // Lưu handler cụ thể
                panelMenu.Controls.Add(btn);
                btn.BringToFront(); // Đảm bảo thứ tự đúng
            }
        }

        #endregion

        #region Event Handlers (Xử lý sự kiện)

        /// <summary>
        /// Tải UserControl Trang chủ (UC_Home_Admin).
        /// </summary>
        private void LoadAdminHomePage()
        {
            ClearPanelContent(); // Chuẩn hóa: Đổi tên hàm
            var homeAdmin = new UC_Home_Admin();
            homeAdmin.Dock = DockStyle.Fill;
            homeAdmin.FunctionSelected += MoChucNangAdmin; // Gán sự kiện (đã chuẩn hóa)
            panelContent.Controls.Add(homeAdmin);

            // Đặt nút trang chủ là nút active
            var homeButton = FindButtonByText("Trang chủ");
            if (homeButton != null)
            {
                ActivateButton(homeButton);
            }
        }

        /// <summary>
        /// Tải UserControl Quản lý Trường học.
        /// </summary>
        private void btnQuanLyTruongHoc_Click(object sender, EventArgs e)
        {
            ClearPanelContent(); // Chuẩn hóa: Đổi tên hàm
            UC_QuanLyTruongHoc uc = new UC_QuanLyTruongHoc();
            uc.Dock = DockStyle.Fill;
            panelContent.Controls.Add(uc);
            // ActivateButton được gọi bởi GenericMenuButton_Click
        }

        /// <summary>
        /// Tải UserControl Báo cáo Admin.
        /// </summary>
        private void btnBaoCaoAdmin_Click(object sender, EventArgs e)
        {
            ClearPanelContent(); // Chuẩn hóa: Đổi tên hàm
            UC_BaoCao_Admin uc = new UC_BaoCao_Admin();
            uc.Dock = DockStyle.Fill;
            panelContent.Controls.Add(uc);
            // ActivateButton được gọi bởi GenericMenuButton_Click
        }

        /// <summary>
        /// Xử lý sự kiện click từ các thẻ trên UC_Home_Admin.
        /// </summary>
        private void MoChucNangAdmin(string maCN) // Tên 'ChonChucNang' đã được chuẩn hóa thành 'FunctionSelected'
        {
            Button btnToClick = null;
            switch (maCN)
            {
                case "Admin_QuanLyGV":
                    btnToClick = FindButtonByText("Quản lý Giáo viên");
                    break;
                case "Admin_QuanLyLop":
                    btnToClick = FindButtonByText("Quản lý Lớp học");
                    break;
                case "Admin_QuanLyTruongHoc":
                    btnToClick = FindButtonByText("Quản Lý Trường học");
                    break;
                case "Admin_BaoCao":
                    btnToClick = FindButtonByText("Báo cáo Admin");
                    break;
                case "Admin_DangXuat":
                    btnToClick = FindButtonByText("Đăng xuất");
                    break;
            }
            // Kích hoạt sự kiện Click của nút tương ứng
            btnToClick?.PerformClick();
        }

        /// <summary>
        /// Trình xử lý Click chung cho tất cả các nút menu.
        /// </summary>
        private void GenericMenuButton_Click(object sender, EventArgs e)
        {
            if (sender is Button btnSender)
            {
                ActivateButton(btnSender);
                if (_buttonHandlers.TryGetValue(btnSender, out var specificHandler))
                {
                    specificHandler(sender, e); // Gọi hàm xử lý riêng
                }
            }
        }

        private void btnTrangChu_Click(object sender, EventArgs e)
        {
            LoadAdminHomePage();
        }

        private void btnQuanLyGV_Click(object sender, EventArgs e)
        {
            ClearPanelContent(); // Chuẩn hóa: Đổi tên hàm
            UC_QuanLyGiaoVien uc = new UC_QuanLyGiaoVien();
            uc.Dock = DockStyle.Fill;
            panelContent.Controls.Add(uc);
            // ActivateButton được gọi bởi GenericMenuButton_Click
        }

        private void btnQuanLyLop_Click(object sender, EventArgs e)
        {
            ClearPanelContent(); // Chuẩn hóa: Đổi tên hàm
            UC_QuanLyLopHocSinh uc = new UC_QuanLyLopHocSinh();
            uc.Dock = DockStyle.Fill;
            panelContent.Controls.Add(uc);
            // ActivateButton được gọi bởi GenericMenuButton_Click
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
                this.DialogResult = DialogResult.OK; // Báo cho Program.cs
                this.Close();
            }
        }

        /// <summary>
        /// Hiển thị ContextMenu của người dùng.
        /// </summary>
        private void UserControl_Click(object sender, EventArgs e)
        {
            if (sender is Control control && userMenu != null)
            {
                userMenu.Show(control, new Point(0, control.Height));
            }
        }

        /// <summary>
        /// Mở Form Hồ sơ cá nhân.
        /// </summary>
        private void SettingsItem_Click(object sender, EventArgs e)
        {
            string currentUser = Properties.Settings.Default["CurrentUser"]?.ToString();
            using (UserProfileForm pf = new UserProfileForm(currentUser))
            {
                pf.ShowDialog(this);
            }
        }

        /// <summary>
        /// Chuyển đổi giao diện Sáng/Tối.
        /// </summary>
        private void btnThemeToggle_Click(object sender, EventArgs e)
        {
            isDarkMode = !isDarkMode;
            btnThemeToggle.Text = isDarkMode ? "☀️" : "🌙";
            ApplyTheme();
        }

        // --- Các nút điều khiển cửa sổ ---
        private void labelClose_Click(object sender, EventArgs e) => Application.Exit();

        private void labelMinimize_Click(object sender, EventArgs e) => this.WindowState = FormWindowState.Minimized;

        private void labelMaximize_Click(object sender, EventArgs e) =>
            this.WindowState = this.WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;

        #endregion

        #region Helper Methods (Hàm hỗ trợ)

        /// <summary>
        /// CHUẨN HÓA: Đổi tên hàm DọnDẹpPanelContent -> ClearPanelContent
        /// Dọn dẹp UserControl cũ trong panelContent trước khi tải UC mới.
        /// </summary>
        private void ClearPanelContent()
        {
            if (panelContent.Controls.Count > 0)
            {
                var oldControl = panelContent.Controls[0];
                if (oldControl is UC_Home_Admin oldHome)
                {
                    oldHome.FunctionSelected -= MoChucNangAdmin; // Gỡ sự kiện
                }
                panelContent.Controls.Clear();
                oldControl.Dispose();
            }
        }

        /// <summary>
        /// Kích hoạt (đổi màu) một nút menu và hủy kích hoạt nút cũ.
        /// </summary>
        private void ActivateButton(Button btn)
        {
            if (btn == null || btn == currentActiveBtn) return;

            var colors = isDarkMode ? darkModeColors : lightModeColors;

            // Hủy kích hoạt nút cũ
            if (currentActiveBtn != null)
            {
                currentActiveBtn.BackColor = colors["menuBg"];
                currentActiveBtn.ForeColor = colors["menuBtnText"];
            }

            // Kích hoạt nút mới
            currentActiveBtn = btn;
            currentActiveBtn.BackColor = colors["menuBtnActiveBg"];
            currentActiveBtn.ForeColor = colors["textPrimary"];
        }

        /// <summary>
        /// Tìm một nút menu động bằng văn bản (Text) của nó.
        /// </summary>
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

        #region Dispose

        /// <summary>
        /// Dọn dẹp tài nguyên và gỡ bỏ các trình xử lý sự kiện.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Gỡ bỏ sự kiện của các control trong Designer
                this.Load -= MenuAdmin_Load;
                if (this.avatarAdmin != null) this.avatarAdmin.Click -= UserControl_Click;
                if (this.lblAdminName != null) this.lblAdminName.Click -= UserControl_Click;
                if (this.btnThemeToggle != null) this.btnThemeToggle.Click -= btnThemeToggle_Click;
                if (this.labelClose != null) this.labelClose.Click -= labelClose_Click;
                if (this.labelMinimize != null) this.labelMinimize.Click -= labelMinimize_Click;
                if (this.labelMaximize != null) this.labelMaximize.Click -= labelMaximize_Click;

                // Gỡ bỏ sự kiện của các control động (Menu và các nút)
                if (settingsItem != null) settingsItem.Click -= SettingsItem_Click;
                if (logoutItem != null) logoutItem.Click -= (s, e) => btnDangXuat_Click(s, e);

                userMenu?.Dispose();
                settingsItem?.Dispose();
                logoutItem?.Dispose();

                // Gỡ sự kiện và dọn dẹp các nút menu động
                // Dùng ToList() để tạo bản sao, an toàn khi xóa khỏi Dictionary
                foreach (var btn in _buttonHandlers.Keys.ToList())
                {
                    btn.Click -= GenericMenuButton_Click;
                    _buttonHandlers.Remove(btn);
                    btn.Dispose(); // Vì chúng được tạo động, chúng ta phải hủy (dispose) chúng
                }
                _buttonHandlers.Clear();

                // Dọn dẹp UserControl hiện tại trong panelContent
                ClearPanelContent(); // Chuẩn hóa: Đổi tên hàm

                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}