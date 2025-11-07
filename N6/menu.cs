using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Documents;
using System.Windows.Forms;

namespace N6
{
    // CHUẨN HÓA: Tên lớp phải là PascalCase
    public partial class Dashboard : Form
    {
        #region Fields (Biến thành viên)

        private bool isMenuCollapsed = false;
        private bool isDarkMode = false;
        private const int menuWidth = 200;
        private const int collapsedMenuWidth = 60;
        private Button currentActiveBtn;
        private bool isDisposed = false;

        // Khai báo các mục menu làm biến thành viên để Dispose an toàn
        private ContextMenuStrip userMenu;
        private ToolStripMenuItem profileItem;
        private ToolStripMenuItem logoutItem;

        // Định nghĩa các bảng màu
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

        #endregion

        #region P/Invoke (Di chuyển Form không viền)

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        /// <summary>
        /// Xử lý sự kiện MouseDown trên TopBar để cho phép di chuyển cửa sổ.
        /// </summary>
        private void TopBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (isDisposed) return;
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        #endregion

        #region Constructor & Form Load

        /// <summary>
        /// Hàm khởi tạo của Form Dashboard.
        /// </summary>
        public Dashboard()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void dashboard_Load(object sender, EventArgs e)
        {
            LoadUserInfo();
            CreateMainMenuItems();
            InitUserMenu();
            ApplyTheme();

            // Kích hoạt nút Trang chủ
            var homeBtn = FindMenuButtonByTag("CN1");
            if (homeBtn != null) SetActiveMenuButton(homeBtn);

            // Tải module trang chủ ban đầu
            LoadModule("CN1");

            // Gán sự kiện
            this.lblAssistiveToggle.Click += lblAssistiveToggle_Click;

            // Gán sự kiện kéo thả cho thanh top bar
            this.panelTopBar.MouseDown += TopBar_MouseDown;
            this.labelAppTitle.MouseDown += TopBar_MouseDown;
            this.pictureBoxAppIcon.MouseDown += TopBar_MouseDown;
            this.labelUserName.MouseDown += TopBar_MouseDown;
            this.labelSubject.MouseDown += TopBar_MouseDown;
        }

        #endregion

        #region Core Logic (Tải Module)

        /// <summary>
        /// Dọn dẹp panel chính và tải UserControl mới dựa trên mã chức năng.
        /// </summary>
        /// <param name="moduleCode">Mã chức năng (ví dụ: "CN1", "CN2").</param>
        private void LoadModule(string moduleCode)
        {
            // 1. Dọn dẹp control cũ và gỡ bỏ sự kiện
            if (panelMain.Controls.Count > 0)
            {
                var oldControl = panelMain.Controls[0];
                if (oldControl is UC_Home oldHome)
                {
                    oldHome.FunctionSelected -= LoadModule; // Gỡ bỏ sự kiện
                }
                panelMain.Controls.Clear();
                oldControl.Dispose(); // Quan trọng: Giải phóng tài nguyên
            }

            Button targetBtn = FindMenuButtonByTag(moduleCode);
            if (targetBtn != null)
            {
                SetActiveMenuButton(targetBtn);
            }

            // 2. Lấy thông tin người dùng
            string user = Properties.Settings.Default["CurrentUser"]?.ToString();
            string maGV = DatabaseHelper.GetTeacherIdByUsername(user);
            UserControl uc = null;

            // 3. Tạo control mới
            switch (moduleCode)
            {
                case "CN1": // Trang chủ
                    UC_Home homeUc = new UC_Home(user ?? "Giáo viên");
                    homeUc.FunctionSelected += LoadModule; // Gán lại sự kiện
                    uc = homeUc;
                    break;
                case "CN2": // Quản lý lớp
                    uc = new UC_QuanLyLop(user);
                    break;
                case "CN3": // Thời khóa biểu
                    uc = new UC_ThoiKhoaBieu(maGV);
                    break;
                case "CN4": // Quản lý tài liệu
                    uc = new UC_QuanLyTaiLieu(maGV);
                    break;
                case "CN5": // Mini-games
                    uc = new UC_MiniGames();
                    break;
                case "CN6": // Báo cáo
                    uc = new UC_BaoCao(maGV);
                    break;
                case "CN7": // Phân tích AI
                    uc = new UC_PhanTichAI(maGV);
                    break;
                case "CN8": // Đăng xuất
                    DialogResult r = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (r == DialogResult.Yes)
                    {
                        Application.Restart();
                    }
                    return; // Không tải UC nào cả
            }

            // 4. Thêm control mới vào panel
            if (uc != null)
            {
                uc.Dock = DockStyle.Fill;
                panelMain.Controls.Add(uc);
            }
        }

        #endregion

        #region User Info & Menu (Hồ sơ người dùng)

        /// <summary>
        /// Tải thông tin (Tên, Môn, GVCN, Avatar) của giáo viên lên TopBar.
        /// </summary>
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

            try
            {
                TeacherProfile profile = DatabaseHelper.GetTeacherProfile(username);
                if (profile == null)
                {
                    labelUserName.Text = "Không tìm thấy";
                    labelSubject.Text = "";
                    pictureBoxUser.Image = Properties.Resources.user_avatar;
                    return;
                }

                labelUserName.Text = profile.Ten;
                string subjectText = profile.TenMon;
                string maGV = DatabaseHelper.GetTeacherIdByUsername(username);
                string tenLopChuNhiem = DatabaseHelper.GetHomeroomClassNameByTeacherId(maGV);

                string gvcnText = !string.IsNullOrEmpty(tenLopChuNhiem)
                    ? $"GVCN: {tenLopChuNhiem}"
                    : "GVCN: Không";

                labelSubject.Text = $"{subjectText} | {gvcnText}";

                // Tải Avatar
                string avatarPath = Path.Combine(Application.StartupPath, profile.AnhDaiDien ?? "");
                if (!string.IsNullOrWhiteSpace(profile.AnhDaiDien) && File.Exists(avatarPath))
                {
                    using (var stream = new MemoryStream(File.ReadAllBytes(avatarPath)))
                    {
                        pictureBoxUser.Image = Image.FromStream(stream);
                    }
                }
                else
                {
                    pictureBoxUser.Image = Properties.Resources.user_avatar;
                }
            }
            catch (Exception) // Bắt lỗi nếu file ảnh bị hỏng
            {
                pictureBoxUser.Image = Properties.Resources.user_avatar;
            }
            pictureBoxUser.SizeMode = PictureBoxSizeMode.Zoom;
        }

        /// <summary>
        /// Khởi tạo ContextMenuStrip cho menu người dùng.
        /// </summary>
        private void InitUserMenu()
        {
            userMenu = new ContextMenuStrip();
            userMenu.Font = new Font("Segoe UI", 11, FontStyle.Regular);

            profileItem = new ToolStripMenuItem("👤 Hồ sơ cá nhân");
            profileItem.Click += ProfileItem_Click;

            logoutItem = new ToolStripMenuItem("🚪 Đăng xuất");
            logoutItem.Click += LogoutItem_Click;

            userMenu.Items.Add(profileItem);
            userMenu.Items.Add(new ToolStripSeparator());
            userMenu.Items.Add(logoutItem);

            pictureBoxUser.Click += PictureBoxUser_Click;
            labelUserName.Click += LabelUserName_Click;
        }

        private void ProfileItem_Click(object sender, EventArgs e)
        {
            string currentUser = Properties.Settings.Default["CurrentUser"]?.ToString();
            using (UserProfileForm pf = new UserProfileForm(currentUser))
            {
                // Gán sự kiện: Nếu avatar thay đổi, tải lại thông tin user
                pf.AvatarChanged += (s, args) => {
                    LoadUserInfo();
                };
                pf.ShowDialog(this);
            }
        }

        private void LogoutItem_Click(object sender, EventArgs e)
        {
            LoadModule("CN8"); // Gọi module đăng xuất
        }

        private void PictureBoxUser_Click(object sender, EventArgs e)
        {
            userMenu.Show(pictureBoxUser, new Point(0, pictureBoxUser.Height));
        }

        private void LabelUserName_Click(object sender, EventArgs e)
        {
            userMenu.Show(labelUserName, new Point(0, labelUserName.Height));
        }

        #endregion

        #region Main Menu (Menu chính bên trái)

        /// <summary>
        /// Tạo động các nút chức năng trong menu chính.
        /// CHUẨN HÓA: Sử dụng Tag để lưu mã chức năng.
        /// </summary>
        private void CreateMainMenuItems()
        {
            var menuItems = new[]
            {
                ("🏠 Trang chủ", "CN1"),
                ("👨‍🎓 Quản lý lớp học", "CN2"),
                ("☁️ Thời khóa biểu", "CN3"),
                ("📑 Quản lý tài liệu", "CN4"),
                ("🎮 Mini-games", "CN5"),
                ("📑 Báo cáo và Xuất dữ liệu", "CN6"),
                ("📊 Phân tích AI", "CN7"),
                ("🚪 Đăng xuất", "CN8")
            };

            // Thêm các nút từ dưới lên (để "Trang chủ" ở trên cùng)
            foreach (var item in menuItems.Reverse())
            {
                Button btn = new Button
                {
                    Text = item.Item1,
                    Tag = item.Item2, // CHUẨN HÓA: Gán mã chức năng vào Tag
                    Dock = DockStyle.Top,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 11, FontStyle.Regular),
                    Height = 55,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Padding = new Padding(15, 0, 0, 0)
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.Click += MenuItem_Click;
                panelMenu.Controls.Add(btn);
            }
        }

        /// <summary>
        /// Đánh dấu một nút là "đang hoạt động" và bỏ chọn các nút khác.
        /// </summary>
        private void SetActiveMenuButton(Button btn)
        {
            if (btn == null) return;

            var colors = isDarkMode ? darkModeColors : lightModeColors;
            foreach (Control c in panelMenu.Controls)
            {
                if (c is Button b)
                {
                    bool isActive = (b == btn);
                    b.BackColor = isActive ? colors["menuBtnActiveBg"] : colors["menuBg"];
                    b.ForeColor = isActive ? colors["textPrimary"] : colors["menuBtnText"];
                    b.Font = new Font("Segoe UI", 11, isActive ? FontStyle.Bold : FontStyle.Regular);
                    b.FlatAppearance.MouseOverBackColor = isActive ? colors["menuBtnActiveBg"] : colors["btnHover"];
                }
            }
            currentActiveBtn = btn;
        }

        /// <summary>
        /// Tìm một nút trong menu dựa trên Tag (mã chức năng).
        /// </summary>
        private Button FindMenuButtonByTag(string tag)
        {
            return panelMenu.Controls.OfType<Button>().FirstOrDefault(b => b.Tag?.ToString() == tag);
        }

        /// <summary>
        /// Xử lý sự kiện click cho tất cả các nút menu chính.
        /// CHUẨN HÓA: Đọc mã chức năng từ Tag thay vì Text.
        /// </summary>
        private void MenuItem_Click(object sender, EventArgs e)
        {
            if (!(sender is Button btn)) return;

            string moduleCode = btn.Tag?.ToString();
            if (string.IsNullOrEmpty(moduleCode))
            {
                MessageBox.Show("Chức năng này đang được phát triển!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SetActiveMenuButton(btn);
            LoadModule(moduleCode);
        }

        #endregion

        #region Theme & UI Helpers (Giao diện)

        private void btnThemeToggle_Click(object sender, EventArgs e)
        {
            isDarkMode = !isDarkMode;
            btnThemeToggle.Text = isDarkMode ? "☀️" : "🌙";
            ApplyTheme();
        }

        /// <summary>
        /// Áp dụng bảng màu (Sáng/Tối) cho tất cả các control.
        /// </summary>
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

            // Áp dụng lại theme cho các nút menu
            SetActiveMenuButton(currentActiveBtn);
        }

        private void lblAssistiveToggle_Click(object sender, EventArgs e)
        {
            string maGV = DatabaseHelper.GetTeacherIdByUsername(Properties.Settings.Default.CurrentUser);
            if (string.IsNullOrEmpty(maGV)) return;

            frmAssistiveMenu menu = new frmAssistiveMenu(maGV);
            Point screenPoint = lblAssistiveToggle.PointToScreen(Point.Empty);

            // Tính toán vị trí để menu bật lên (pop-up)
            int menuX = screenPoint.X - menu.Width + lblAssistiveToggle.Width;
            int menuY = screenPoint.Y - menu.Height - 5;

            menu.Location = new Point(menuX, menuY);
            menu.Show(); // Hiển thị dạng không modal
        }

        // --- Các nút điều khiển cửa sổ (Đóng, Thu nhỏ, Phóng to) ---
        private void labelClose_Click(object sender, EventArgs e) => Application.Exit();
        private void labelMinimize_Click(object sender, EventArgs e) => this.WindowState = FormWindowState.Minimized;
        private void labelMaximize_Click(object sender, EventArgs e) =>
            this.WindowState = this.WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;

        #endregion

        #region Dispose (Dọn dẹp tài nguyên)

        /// <summary>
        /// Dọn dẹp tài nguyên và gỡ bỏ các trình xử lý sự kiện để tránh rò rỉ bộ nhớ.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                isDisposed = true; // Đặt cờ

                // Gỡ bỏ các sự kiện của Form
                this.Resize -= dashboard_Load;

                // Gỡ bỏ sự kiện kéo thả cửa sổ
                if (this.panelTopBar != null) this.panelTopBar.MouseDown -= TopBar_MouseDown;
                if (this.labelAppTitle != null) this.labelAppTitle.MouseDown -= TopBar_MouseDown;
                if (this.pictureBoxAppIcon != null) this.pictureBoxAppIcon.MouseDown -= TopBar_MouseDown;
                if (this.labelUserName != null) this.labelUserName.MouseDown -= TopBar_MouseDown;
                if (this.labelSubject != null) this.labelSubject.MouseDown -= TopBar_MouseDown;

                // Gỡ bỏ các sự kiện Click
                if (this.lblAssistiveToggle != null) this.lblAssistiveToggle.Click -= lblAssistiveToggle_Click;
                if (this.pictureBoxUser != null) this.pictureBoxUser.Click -= PictureBoxUser_Click;
                if (this.labelUserName != null) this.labelUserName.Click -= LabelUserName_Click;
                if (this.btnThemeToggle != null) this.btnThemeToggle.Click -= btnThemeToggle_Click;

                // Dọn dẹp ContextMenu (tạo động)
                if (profileItem != null) profileItem.Click -= ProfileItem_Click;
                if (logoutItem != null) logoutItem.Click -= LogoutItem_Click;
                profileItem?.Dispose();
                logoutItem?.Dispose();
                userMenu?.Dispose();

                // Dọn dẹp các nút menu chính (tạo động)
                if (panelMenu != null)
                {
                    foreach (Button btn in panelMenu.Controls.OfType<Button>().ToList())
                    {
                        btn.Click -= MenuItem_Click;
                        btn.Dispose();
                    }
                }

                // Dọn dẹp UserControl hiện tại
                if (panelMain.Controls.Count > 0)
                {
                    var c = panelMain.Controls[0];
                    if (c is UC_Home home) { home.FunctionSelected -= LoadModule; }
                    panelMain.Controls.Clear();
                    c.Dispose();
                }

                // Dọn dẹp các component mặc định của form
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