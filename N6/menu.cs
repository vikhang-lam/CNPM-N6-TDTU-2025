using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using N6.Properties;

namespace N6
{
    public partial class dashboard : Form
    {
        private bool isMenuCollapsed = false;
        private bool isDarkMode = false;
        private const int menuWidth = 200;
        private const int collapsedMenuWidth = 60;
        private Button currentActiveBtn;

        // Define color schemes
        private Dictionary<string, Color> lightModeColors = new Dictionary<string, Color>()
        {
            {"mainBg", Color.FromArgb(240, 240, 240)},
            {"menuBg", Color.White},
            {"topBarBg", Color.FromArgb(179, 102, 255)},
            {"textPrimary", Color.Black},
            {"textSecondary", Color.Gray},
            {"iconColor", Color.White},
            {"cellBg", Color.White},
            {"btnHover", Color.FromArgb(240, 240, 240)},
            {"menuBtnText", Color.FromArgb(102, 102, 255)},
            {"menuBtnActive", Color.FromArgb(102, 102, 255)},
            {"menuBtnActiveBg", Color.FromArgb(240, 240, 240)},
            {"btnCollapseMenu", Color.FromArgb(240, 240, 240)},
            {"userPanelText", Color.Black}
        };

        private Dictionary<string, Color> darkModeColors = new Dictionary<string, Color>()
        {
            {"mainBg", Color.FromArgb(46, 51, 73)},
            {"menuBg", Color.FromArgb(24, 30, 54)},
            {"topBarBg", Color.FromArgb(46, 51, 73)},
            {"textPrimary", Color.White},
            {"textSecondary", Color.FromArgb(158, 161, 176)},
            {"iconColor", Color.White},
            {"cellBg", Color.FromArgb(37, 42, 64)},
            {"btnHover", Color.FromArgb(46, 51, 73)},
            {"menuBtnText", Color.FromArgb(158, 161, 176)},
            {"menuBtnActive", Color.FromArgb(0, 126, 249)},
            {"menuBtnActiveBg", Color.FromArgb(46, 51, 73)},
            {"btnCollapseMenu", Color.FromArgb(24, 30, 54)},
            {"userPanelText", Color.White}
        };

        public dashboard()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void dashboard_Load(object sender, EventArgs e)
        {
            CreateUserProfileSection();
            CreateMainMenuItems();
            CreateDashboardContent();
            ApplyTheme(isDarkMode);

            // Ẩn thanh scrollbar nhưng vẫn cho phép cuộn
            panelMenu.VerticalScroll.Visible = false;
            panelMenu.VerticalScroll.Enabled = false;
            panelMenu.HorizontalScroll.Visible = false;
            panelMenu.HorizontalScroll.Enabled = false;
        }

        private void ApplyTheme(bool isDark)
        {
            Dictionary<string, Color> theme = isDark ? darkModeColors : lightModeColors;

            this.BackColor = theme["mainBg"];
            panelTopBar.BackColor = theme["topBarBg"];
            panelMenu.BackColor = theme["menuBg"];
            panelContent.BackColor = theme["mainBg"];
            labelAppTitle.ForeColor = theme["iconColor"];
            labelMaximize.ForeColor = theme["iconColor"];
            labelMinimize.ForeColor = theme["iconColor"];
            labelClose.ForeColor = theme["iconColor"];
            btnThemeToggle.ForeColor = theme["iconColor"];
            btnThemeToggle.Text = isDark ? "☀️" : "🌙";
            btnCollapseMenu.BackColor = theme["btnCollapseMenu"];
            btnCollapseMenu.ForeColor = theme["textSecondary"];
            btnToggleMenu.ForeColor = theme["iconColor"];

            // Áp dụng theme cho User Panel
            var userPanel = panelMenu.Controls.OfType<Panel>().FirstOrDefault();
            if (userPanel != null)
            {
                userPanel.BackColor = theme["menuBg"];
                userPanel.Controls.OfType<Label>().FirstOrDefault(l => l.Name == "nameLabel").ForeColor = theme["userPanelText"];
                userPanel.Controls.OfType<Label>().FirstOrDefault(l => l.Name == "subjectLabel").ForeColor = theme["textSecondary"];
            }

            // Áp dụng theme cho các nút menu
            foreach (Control control in panelMenu.Controls)
            {
                if (control is Button btn)
                {
                    btn.BackColor = theme["menuBg"];
                    btn.ForeColor = theme["menuBtnText"];
                    if (btn == currentActiveBtn)
                    {
                        btn.BackColor = theme["menuBtnActiveBg"];
                        btn.ForeColor = theme["menuBtnActive"];
                    }
                }
            }

            // Cập nhật lại màu sắc của các RoundedPanel và các label bên trong
            foreach (var panel in panelContent.Controls.OfType<RoundedPanel>())
            {
                panel.BackColor = theme["cellBg"];
                foreach (Control c in panel.Controls)
                {
                    if (c is TableLayoutPanel contentTable)
                    {
                        foreach (Control innerControl in contentTable.Controls)
                        {
                            if (innerControl is Label lbl)
                            {
                                if (lbl.Name.StartsWith("value") || lbl.Name.StartsWith("title"))
                                    lbl.ForeColor = theme["textPrimary"];
                                else if (lbl.Name.StartsWith("description"))
                                    lbl.ForeColor = theme["textSecondary"];
                            }
                        }
                    }
                }
            }

            // Cập nhật màu sắc cho bảng chào mừng
            var welcomePanel = panelContent.Controls.OfType<TableLayoutPanel>().FirstOrDefault()?.Controls.OfType<Panel>().FirstOrDefault(p => p.Name == "welcomePanel");
            if (welcomePanel != null)
            {
                welcomePanel.Controls.OfType<Label>().FirstOrDefault(l => l.Name == "pageTitle").ForeColor = theme["textPrimary"];
                welcomePanel.Controls.OfType<Label>().FirstOrDefault(l => l.Name == "subTitle").ForeColor = theme["textSecondary"];
            }
        }

        private void CreateUserProfileSection()
        {
            Panel userPanel = new Panel();
            userPanel.Dock = DockStyle.Top;
            userPanel.Height = 150;
            userPanel.BackColor = isDarkMode ? darkModeColors["menuBg"] : lightModeColors["menuBg"];

            PictureBox avatarBox = new PictureBox();
            avatarBox.Size = new Size(80, 80);
            avatarBox.Location = new Point(10, 20);
            avatarBox.SizeMode = PictureBoxSizeMode.Zoom;
            avatarBox.BackColor = Color.Gray;

            Label nameLabel = new Label();
            nameLabel.Name = "nameLabel";
            nameLabel.Text = "Cô Thanh Hoa";
            nameLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            nameLabel.ForeColor = isDarkMode ? darkModeColors["userPanelText"] : lightModeColors["userPanelText"];
            nameLabel.Location = new Point(10, 110);
            nameLabel.AutoSize = true;

            Label subjectLabel = new Label();
            subjectLabel.Name = "subjectLabel";
            subjectLabel.Text = "Tiếng Việt";
            subjectLabel.Font = new Font("Segoe UI", 10);
            subjectLabel.ForeColor = isDarkMode ? darkModeColors["textSecondary"] : lightModeColors["textSecondary"];
            subjectLabel.Location = new Point(10, 135);
            subjectLabel.AutoSize = true;

            userPanel.Controls.Add(avatarBox);
            userPanel.Controls.Add(nameLabel);
            userPanel.Controls.Add(subjectLabel);
            panelMenu.Controls.Add(userPanel);
        }

        private void CreateMainMenuItems()
        {
            var menuItems = new string[]
            {
                "Trang chủ",
                "Quản lý học sinh",
                "Điểm danh",
                "Kết quả học tập",
                "Báo cáo & Xuất",
                "Mini-games",
                "Công cụ hỗ trợ",
                "Sao lưu & Đồng bộ",
                "Đăng xuất"
            };

            foreach (var item in menuItems)
            {
                Button btn = new Button();
                btn.Text = item;
                btn.Dock = DockStyle.Top;
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
                btn.Font = new Font("Segoe UI", 12);
                btn.ForeColor = isDarkMode ? darkModeColors["menuBtnText"] : lightModeColors["menuBtnText"];
                btn.ImageAlign = ContentAlignment.MiddleLeft;
                btn.TextAlign = ContentAlignment.MiddleLeft;
                btn.Padding = new Padding(15, 0, 0, 0);
                btn.Height = 50;
                btn.Click += MenuItem_Click;

                panelMenu.Controls.Add(btn);
            }

            List<Control> controls = panelMenu.Controls.Cast<Control>().ToList();
            controls.Reverse();
            panelMenu.Controls.Clear();
            controls.ForEach(c => panelMenu.Controls.Add(c));

            // Set the first button as active
            if (panelMenu.Controls.OfType<Button>().Any())
            {
                currentActiveBtn = panelMenu.Controls.OfType<Button>().LastOrDefault();
            }
        }

        private void CreateDashboardContent()
        {
            panelContent.Controls.Clear();

            // Sử dụng một TableLayoutPanel duy nhất để chứa tất cả nội dung
            TableLayoutPanel mainContentTable = new TableLayoutPanel();
            mainContentTable.Dock = DockStyle.Fill;
            mainContentTable.ColumnCount = 1;
            mainContentTable.RowCount = 3;
            mainContentTable.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Hàng cho phần chào mừng
            mainContentTable.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Hàng cho các ô thống kê
            mainContentTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Hàng cho các ô chức năng chính
            mainContentTable.Padding = new Padding(40);
            panelContent.Controls.Add(mainContentTable);

            // Panel chào mừng
            Panel topPanel = new Panel();
            topPanel.Name = "welcomePanel";
            topPanel.Dock = DockStyle.Fill;
            topPanel.Height = 150;
            topPanel.BackColor = Color.Transparent;

            Label pageTitle = new Label();
            pageTitle.Name = "pageTitle";
            pageTitle.Text = "Chào mừng trở lại!";
            pageTitle.Font = new Font("Segoe UI Semibold", 20);
            pageTitle.Location = new Point(0, 0);
            pageTitle.AutoSize = true;

            Label subTitle = new Label();
            subTitle.Name = "subTitle";
            subTitle.Text = "Chọn chức năng bạn muốn sử dụng";
            subTitle.Font = new Font("Segoe UI", 12);
            subTitle.Location = new Point(0, 40);
            subTitle.AutoSize = true;

            topPanel.Controls.Add(pageTitle);
            topPanel.Controls.Add(subTitle);
            mainContentTable.Controls.Add(topPanel, 0, 0);

            // TableLayoutPanel cho các ô thống kê
            TableLayoutPanel statsTable = new TableLayoutPanel();
            statsTable.Dock = DockStyle.Top;
            statsTable.Height = 150;
            statsTable.ColumnCount = 4;
            statsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            statsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            statsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            statsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            statsTable.Margin = new Padding(0, 0, 0, 20); // Thêm margin dưới
            mainContentTable.Controls.Add(statsTable, 0, 1);

            CreateStatsCell(statsTable, "156", "Tổng học sinh", 0);
            CreateStatsCell(statsTable, "8", "Lớp học", 1);
            CreateStatsCell(statsTable, "92%", "Tỷ lệ có mặt", 2);
            CreateStatsCell(statsTable, "45", "Báo cáo tháng", 3);

            // TableLayoutPanel cho các ô chức năng
            TableLayoutPanel mainTable = new TableLayoutPanel();
            mainTable.Dock = DockStyle.Fill;
            mainTable.ColumnCount = 4;
            mainTable.RowCount = 2;
            mainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            mainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            mainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            mainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            mainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            mainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            mainContentTable.Controls.Add(mainTable, 0, 2);

            CreateFeatureCell(mainTable, 0, 0, "Quản lý lớp & học sinh", "Quản lý thông tin cơ bản của học sinh. Thêm/sửa/xóa, Import từ Excel và kiểm tra trùng mã.");
            CreateFeatureCell(mainTable, 1, 0, "Điểm danh", "Tạo mã QR, điểm danh thủ công, quản lý sĩ số lớp học.");
            CreateFeatureCell(mainTable, 2, 0, "Kết quả học tập & ghi chú", "Nhập điểm học kỳ (HK1, HK2). Ghi chú hàng tháng tự động lưu ngày giờ.");
            CreateFeatureCell(mainTable, 3, 0, "Báo cáo & Xuất dữ liệu", "Xuất báo cáo chuyên cần (Excel, PDF), bảng điểm học kỳ, ghi chú, và thống kê tổng hợp.");
            CreateFeatureCell(mainTable, 0, 1, "Công cụ tương tác / Mini-games", "Danh sách trò chơi: Gọi tên ngẫu nhiên, Quiz nhanh, Ghép chữ, Flashcard, Nghe-chọn-hình, Sắp xếp câu, Điền từ, Lật thẻ, Random số, Pass a ball.");
            CreateFeatureCell(mainTable, 1, 1, "Công cụ hỗ trợ", "Bảng trắng mini, ghi chú nhanh lưu vào hồ sơ HS, đồng hồ hẹn giờ/đếm giờ.");
            CreateFeatureCell(mainTable, 2, 1, "Sao lưu / Đồng bộ / Khôi phục", "Export/Import backup, đồng bộ thủ công qua Google Drive.");
            CreateFeatureCell(mainTable, 3, 1, "Hoạt động gần đây", "Xem lại các hoạt động và thay đổi gần đây trong hệ thống.");
        }

        private void CreateStatsCell(TableLayoutPanel table, string value, string label, int column)
        {
            RoundedPanel panel = new RoundedPanel();
            panel.Dock = DockStyle.Fill;
            panel.Margin = new Padding(10);
            panel.BackColor = isDarkMode ? darkModeColors["cellBg"] : lightModeColors["cellBg"];

            // Sử dụng TableLayoutPanel lồng để bố cục các label
            TableLayoutPanel contentTable = new TableLayoutPanel();
            contentTable.Dock = DockStyle.Fill;
            contentTable.ColumnCount = 1;
            contentTable.RowCount = 2;
            contentTable.RowStyles.Add(new RowStyle(SizeType.Percent, 60F));
            contentTable.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));
            contentTable.Padding = new Padding(15);
            contentTable.BackColor = Color.Transparent;

            Label valueLabel = new Label();
            valueLabel.Name = "valueLabel";
            valueLabel.Text = value;
            valueLabel.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            valueLabel.ForeColor = isDarkMode ? darkModeColors["textPrimary"] : lightModeColors["textPrimary"];
            valueLabel.Dock = DockStyle.Fill;
            valueLabel.TextAlign = ContentAlignment.BottomLeft;

            Label descriptionLabel = new Label();
            descriptionLabel.Name = "descriptionLabel";
            descriptionLabel.Text = label;
            descriptionLabel.Font = new Font("Segoe UI", 10);
            descriptionLabel.ForeColor = isDarkMode ? darkModeColors["textSecondary"] : lightModeColors["textSecondary"];
            descriptionLabel.Dock = DockStyle.Fill;
            descriptionLabel.TextAlign = ContentAlignment.TopLeft;

            contentTable.Controls.Add(valueLabel, 0, 0);
            contentTable.Controls.Add(descriptionLabel, 0, 1);
            panel.Controls.Add(contentTable);
            table.Controls.Add(panel, column, 0);
        }

        private void CreateFeatureCell(TableLayoutPanel table, int column, int row, string title, string description)
        {
            RoundedPanel panel = new RoundedPanel();
            panel.Dock = DockStyle.Fill;
            panel.Margin = new Padding(10);
            panel.BackColor = isDarkMode ? darkModeColors["cellBg"] : lightModeColors["cellBg"];

            // Sử dụng TableLayoutPanel lồng để bố cục các label
            TableLayoutPanel contentTable = new TableLayoutPanel();
            contentTable.Dock = DockStyle.Fill;
            contentTable.ColumnCount = 1;
            contentTable.RowCount = 2;
            contentTable.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Tự động co giãn theo title
            contentTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Tự động co giãn theo description
            contentTable.Padding = new Padding(15);
            contentTable.BackColor = Color.Transparent;
            panel.Controls.Add(contentTable);

            Label titleLabel = new Label();
            titleLabel.Name = "titleLabel";
            titleLabel.Text = title;
            titleLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            titleLabel.ForeColor = isDarkMode ? darkModeColors["textPrimary"] : lightModeColors["textPrimary"];
            titleLabel.Dock = DockStyle.Top;
            titleLabel.AutoSize = true;
            contentTable.Controls.Add(titleLabel, 0, 0);

            Label descLabel = new Label();
            descLabel.Name = "descriptionLabel";
            descLabel.Text = description;
            descLabel.Font = new Font("Segoe UI", 9);
            descLabel.ForeColor = isDarkMode ? darkModeColors["textSecondary"] : lightModeColors["textSecondary"];
            descLabel.Dock = DockStyle.Fill;
            descLabel.AutoSize = true;

            // Thêm dòng này để giới hạn chiều rộng và tự động xuống dòng
            descLabel.MaximumSize = new Size(panel.Width - 30, 0);
            panel.Resize += (sender, e) =>
            {
                descLabel.MaximumSize = new Size(panel.Width - 30, 0);
            };

            contentTable.Controls.Add(descLabel, 0, 1);

            table.Controls.Add(panel, column, row);
        }

        private void MenuItem_Click(object sender, EventArgs e)
        {
            ActivateButton(sender as Button);
        }

        private void ActivateButton(Button btn)
        {
            if (currentActiveBtn != null)
            {
                currentActiveBtn.BackColor = isDarkMode ? darkModeColors["menuBg"] : lightModeColors["menuBg"];
                currentActiveBtn.ForeColor = isDarkMode ? darkModeColors["menuBtnText"] : lightModeColors["menuBtnText"];
            }

            currentActiveBtn = btn;
            currentActiveBtn.BackColor = isDarkMode ? darkModeColors["menuBtnActiveBg"] : lightModeColors["menuBtnActiveBg"];
            currentActiveBtn.ForeColor = isDarkMode ? darkModeColors["menuBtnActive"] : lightModeColors["menuBtnActive"];
        }

        private void btnThemeToggle_Click(object sender, EventArgs e)
        {
            isDarkMode = !isDarkMode;
            ApplyTheme(isDarkMode);
        }

        private void btnCollapseMenu_Click(object sender, EventArgs e)
        {
            isMenuCollapsed = !isMenuCollapsed;
            if (isMenuCollapsed)
            {
                btnCollapseMenu.Text = "›";
            }
            else
            {
                btnCollapseMenu.Text = "‹";
            }
            menuToggleTimer.Start();
        }

        private void btnToggleMenu_Click(object sender, EventArgs e)
        {
            panelMenu.Visible = !panelMenu.Visible;
        }

        private void MenuToggleTimer_Tick(object sender, EventArgs e)
        {
            if (isMenuCollapsed)
            {
                if (panelMenu.Width <= collapsedMenuWidth)
                {
                    panelMenu.Width = collapsedMenuWidth;
                    menuToggleTimer.Stop();
                }
                else
                {
                    panelMenu.Width -= 10;
                }
            }
            else
            {
                if (panelMenu.Width >= menuWidth)
                {
                    panelMenu.Width = menuWidth;
                    menuToggleTimer.Stop();
                }
                else
                {
                    panelMenu.Width += 10;
                }
            }
        }

        // --- Các phương thức xử lý nút form ---

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HTCAPTION = 0x2;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void panelTopBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
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
    }
}