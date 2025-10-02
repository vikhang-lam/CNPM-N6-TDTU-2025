using System;
using System.Collections.Generic;
using System.Drawing;
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
            {"mainBg", Color.FromArgb(185, 235, 250)},
            {"menuBg", Color.White},
            {"topBarBg", Color.FromArgb(0, 150, 200)},
            {"textPrimary", Color.Black},
            {"menuBtnText", Color.FromArgb(55, 71, 79)},
            {"menuBtnActiveBg", Color.FromArgb(220, 240, 250)},
            {"btnHover", Color.FromArgb(240, 240, 240)},
            {"userPanelText", Color.Black}
        };
        private Dictionary<Button, EventHandler> _handlers = new Dictionary<Button, EventHandler>();
        private Dictionary<string, Color> darkModeColors = new Dictionary<string, Color>()
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

        public MenuAdmin()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void MenuAdmin_Load(object sender, EventArgs e)
        {
            
            CreateMenuItems();
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            var colors = isDarkMode ? darkModeColors : lightModeColors;

            this.BackColor = colors["mainBg"];
            panelTopBar.BackColor = colors["topBarBg"];
            panelMenu.BackColor = colors["menuBg"];
            panelContent.BackColor = colors["mainBg"];

            foreach (Control c in panelMenu.Controls)
            {
                
                if (c is Panel p)
                {
                    foreach (Control child in p.Controls)
                    {
                        if (child is Label lbl) lbl.ForeColor = colors["userPanelText"];
                    }
                }
            }

            
        }

        private void CreateUserProfileSection()
        {
            Panel userPanel = new Panel();
            userPanel.Dock = DockStyle.Top;
            userPanel.Height = 100;

            PictureBox avatarBox = new PictureBox();
            avatarBox.Size = new Size(60, 60);
            avatarBox.Location = new Point(20, 20);
            avatarBox.SizeMode = PictureBoxSizeMode.Zoom;
            avatarBox.Image = Properties.Resources.user_avatar; // avatar mặc định admin

            Label nameLabel = new Label();
            nameLabel.Name = "nameLabel";
            nameLabel.Text = "Admin";
            nameLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            nameLabel.AutoSize = true;
            nameLabel.Location = new Point(90, 35);

            userPanel.Controls.Add(avatarBox);
            userPanel.Controls.Add(nameLabel);

            panelMenu.Controls.Add(userPanel);
        }

        private void CreateMenuItems()
        {
            var menuItems = new (string, EventHandler)[]
            {
                ("👨‍🏫 Quản lý Giáo viên", btnQuanLyGV_Click),
                ("🏫 Quản lý Lớp học", btnQuanLyLop_Click),
                ("🚪 Đăng xuất", btnDangXuat_Click)
            };

            foreach (var (text, handler) in menuItems)
            {
                Button btn = new Button();
                btn.Text = text;
                btn.Dock = DockStyle.Top;
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
                btn.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                btn.Height = 50;
                btn.TextAlign = ContentAlignment.MiddleLeft;
                btn.Padding = new Padding(15, 0, 0, 0);
                btn.Click += Btn_Click;

                panelMenu.Controls.Add(btn);
                panelMenu.Controls.SetChildIndex(btn, 1);
            }
        }
        private void Btn_Click(object sender, EventArgs e)
        {
            if (sender is Button btnSender)
            {
                ActivateButton(btnSender);

                // Gọi lại handler gốc nếu có
                if (_handlers.TryGetValue(btnSender, out var h))
                {
                    h(sender, e);
                }
            }
        }

        private void ActivateButton(Button btn)
        {
            if (currentActiveBtn != null)
                currentActiveBtn.BackColor = isDarkMode ? darkModeColors["menuBg"] : lightModeColors["menuBg"];

            currentActiveBtn = btn;
            btn.BackColor = isDarkMode ? darkModeColors["menuBtnActiveBg"] : lightModeColors["menuBtnActiveBg"];
        }

        // === Các sự kiện nút menu ===
        private void btnQuanLyGV_Click(object sender, EventArgs e)
        {
            panelContent.Controls.Clear();
            Label lbl = new Label
            {
                Text = "Màn hình Quản lý Giáo viên",
                AutoSize = true,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(50, 50)
            };
            panelContent.Controls.Add(lbl);
        }

        private void btnQuanLyLop_Click(object sender, EventArgs e)
        {
            panelContent.Controls.Clear();
            Label lbl = new Label
            {
                Text = "Màn hình Quản lý Lớp học",
                AutoSize = true,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(50, 50)
            };
            panelContent.Controls.Add(lbl);
        }

        private void btnDangXuat_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất không?",
                                                  "Đăng xuất",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                this.Hide();
                login login = new login(); // mở lại form Login.cs
                login.Show();
                this.Close();
            }
        }

        // === Các nút top bar ===
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
    }
}
