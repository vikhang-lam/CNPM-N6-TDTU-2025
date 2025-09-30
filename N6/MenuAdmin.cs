using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace N6
{
    public partial class MenuAdmin : Form
    {
        private bool isMenuCollapsed = false;
        private bool isDarkMode = false;
        private const int menuWidth = 200;
        private const int collapsedMenuWidth = 60;
        private Button currentActiveBtn;

        private Dictionary<string, Color> lightModeColors = new Dictionary<string, Color>()
        {
            {"mainBg", Color.FromArgb(240, 240, 240)},
            {"menuBg", Color.White},
            {"topBarBg", Color.FromArgb(179, 102, 255)},
            {"textPrimary", Color.Black},
            {"textSecondary", Color.Gray},
            {"iconColor", Color.White},
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
            {"btnHover", Color.FromArgb(46, 51, 73)},
            {"menuBtnText", Color.FromArgb(158, 161, 176)},
            {"menuBtnActive", Color.FromArgb(0, 126, 249)},
            {"menuBtnActiveBg", Color.FromArgb(46, 51, 73)},
            {"btnCollapseMenu", Color.FromArgb(24, 30, 54)},
            {"userPanelText", Color.White}
        };

        public MenuAdmin()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void MenuAdmin_Load(object sender, EventArgs e)
        {
            CreateUserProfileSection();
            CreateMenuItems();
            CreateWelcomePanel();
            ApplyTheme(isDarkMode);
        }

        private void ApplyTheme(bool isDark)
        {
            var theme = isDark ? darkModeColors : lightModeColors;

            this.BackColor = theme["mainBg"];
            panelTopBar.BackColor = theme["topBarBg"];
            panelMenu.BackColor = theme["menuBg"];
            panelContent.BackColor = theme["mainBg"];
            labelClose.ForeColor = theme["iconColor"];
            labelMaximize.ForeColor = theme["iconColor"];
            labelMinimize.ForeColor = theme["iconColor"];
            btnThemeToggle.ForeColor = theme["iconColor"];
            btnThemeToggle.Text = isDark ? "☀️" : "🌙";
            btnCollapseMenu.BackColor = theme["btnCollapseMenu"];
            btnCollapseMenu.ForeColor = theme["textSecondary"];
            btnToggleMenu.ForeColor = theme["iconColor"];

            // Update User Panel
            var userPanel = panelMenu.Controls.OfType<Panel>().FirstOrDefault();
            if (userPanel != null)
            {
                userPanel.BackColor = theme["menuBg"];
                foreach (var lbl in userPanel.Controls.OfType<Label>())
                {
                    if (lbl.Name == "nameLabel")
                        lbl.ForeColor = theme["userPanelText"];
                    else
                        lbl.ForeColor = theme["textSecondary"];
                }
            }

            // Update Menu buttons
            foreach (Control control in panelMenu.Controls)
            {
                if (control is Button btn && btn != btnCollapseMenu)
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

            // Update Welcome panel text
            var welcomePanel = panelContent.Controls.OfType<Panel>().FirstOrDefault(p => p.Name == "welcomePanel");
            if (welcomePanel != null)
            {
                foreach (var lbl in welcomePanel.Controls.OfType<Label>())
                {
                    if (lbl.Name == "pageTitle")
                        lbl.ForeColor = theme["textPrimary"];
                    if (lbl.Name == "subTitle")
                        lbl.ForeColor = theme["textSecondary"];
                }
            }
        }

        private void CreateUserProfileSection()
        {
            Panel userPanel = new Panel();
            userPanel.Dock = DockStyle.Top;
            userPanel.Height = 120;
            userPanel.BackColor = isDarkMode ? darkModeColors["menuBg"] : lightModeColors["menuBg"];

            PictureBox avatarBox = new PictureBox();
            avatarBox.Size = new Size(60, 60);
            avatarBox.Location = new Point(10, 30);
            avatarBox.SizeMode = PictureBoxSizeMode.Zoom;
            avatarBox.BackColor = Color.Gray;

            Label nameLabel = new Label();
            nameLabel.Name = "nameLabel";
            nameLabel.Text = "Admin";
            nameLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            nameLabel.ForeColor = isDarkMode ? darkModeColors["userPanelText"] : lightModeColors["userPanelText"];
            nameLabel.AutoSize = true;
            nameLabel.Location = new Point(80, 35);

            Label roleLabel = new Label();
            roleLabel.Name = "roleLabel";
            roleLabel.Text = "Quản trị hệ thống";
            roleLabel.Font = new Font("Segoe UI", 10);
            roleLabel.ForeColor = isDarkMode ? darkModeColors["textSecondary"] : lightModeColors["textSecondary"];
            roleLabel.AutoSize = true;
            roleLabel.Location = new Point(80, 60);

            userPanel.Controls.Add(avatarBox);
            userPanel.Controls.Add(nameLabel);
            userPanel.Controls.Add(roleLabel);

            // 👉 Add userPanel sau cùng để nó nằm trên cùng menu
            panelMenu.Controls.Add(userPanel);
        }


        private void CreateMenuItems()
        {
            var menuItems = new (string, EventHandler)[]
            {
                ("👨‍🏫 Quản lý Giáo viên", btnQuanLyGV_Click),
                ("🏫 Quản lý Lớp & Học sinh", btnQuanLyLop_Click),
                ("📅 Quản lý Thời khóa biểu", btnTKB_Click),
                ("⚙️ Cấu hình hệ thống", (s,e)=> MessageBox.Show("Mở cấu hình")),
                ("🚪 Đăng xuất", (s,e)=> this.Close())
            };

            foreach (var (text, handler) in menuItems)
            {
                Button btn = new Button();
                btn.Text = text;
                btn.Dock = DockStyle.Top;
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
                btn.Font = new Font("Segoe UI", 12);
                btn.Height = 50;
                btn.TextAlign = ContentAlignment.MiddleLeft;
                btn.Padding = new Padding(15, 0, 0, 0);
                btn.Click += (s, e) => { ActivateButton(btn); handler(s, e); };

                panelMenu.Controls.Add(btn);
                panelMenu.Controls.SetChildIndex(btn, 1); // dưới userPanel
            }
        }

        private void CreateWelcomePanel()
        {
            panelContent.Controls.Clear();

            Panel welcomePanel = new Panel();
            welcomePanel.Name = "welcomePanel";
            welcomePanel.Dock = DockStyle.Top;
            welcomePanel.Height = 150;
            welcomePanel.BackColor = Color.Transparent;

            Label pageTitle = new Label();
            pageTitle.Name = "pageTitle";
            pageTitle.Text = "Chào mừng, Admin!";
            pageTitle.Font = new Font("Segoe UI Semibold", 20);
            pageTitle.AutoSize = true;
            pageTitle.Location = new Point(20, 20);

            Label subTitle = new Label();
            subTitle.Name = "subTitle";
            subTitle.Text = "Chọn chức năng bạn muốn sử dụng";
            subTitle.Font = new Font("Segoe UI", 12);
            subTitle.AutoSize = true;
            subTitle.Location = new Point(20, 60);

            welcomePanel.Controls.Add(pageTitle);
            welcomePanel.Controls.Add(subTitle);
            panelContent.Controls.Add(welcomePanel);
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

        // Nút chức năng demo
        private void btnQuanLyGV_Click(object sender, EventArgs e) => MessageBox.Show("Mở chức năng Quản lý Giáo viên");
        private void btnQuanLyLop_Click(object sender, EventArgs e) => MessageBox.Show("Mở chức năng Quản lý Lớp & Học sinh");
        private void btnTKB_Click(object sender, EventArgs e) => MessageBox.Show("Mở chức năng Quản lý Thời khóa biểu");

        // Theme toggle
        private void btnThemeToggle_Click(object sender, EventArgs e)
        {
            isDarkMode = !isDarkMode;
            ApplyTheme(isDarkMode);
        }

        private void btnCollapseMenu_Click(object sender, EventArgs e)
        {
            isMenuCollapsed = !isMenuCollapsed;
            btnCollapseMenu.Text = isMenuCollapsed ? "›" : "‹";
            menuToggleTimer.Start();
        }

        private void btnToggleMenu_Click(object sender, EventArgs e)
        {
            panelMenu.Visible = !panelMenu.Visible;
        }

        private void menuToggleTimer_Tick(object sender, EventArgs e)
        {
            if (isMenuCollapsed)
            {
                if (panelMenu.Width <= collapsedMenuWidth)
                {
                    panelMenu.Width = collapsedMenuWidth;
                    menuToggleTimer.Stop();

                    // 👉 ép vẽ lại layout
                    panelMenu.PerformLayout();
                    panelMenu.Refresh();
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

                    // 👉 ép vẽ lại layout
                    panelMenu.PerformLayout();
                    panelMenu.Refresh();
                }
                else
                {
                    panelMenu.Width += 10;
                }
            }
        }

        // Control buttons
        private void labelClose_Click(object sender, EventArgs e) => this.Close();
        private void labelMinimize_Click(object sender, EventArgs e) => this.WindowState = FormWindowState.Minimized;
        private void labelMaximize_Click(object sender, EventArgs e) =>
            this.WindowState = (this.WindowState == FormWindowState.Normal) ? FormWindowState.Maximized : FormWindowState.Normal;

        private void panelTopBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Message msg = Message.Create(this.Handle, 0xA1, new IntPtr(2), IntPtr.Zero);
                this.DefWndProc(ref msg);
            }
        }

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
    }
}
