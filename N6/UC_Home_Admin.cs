using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace N6
{
    public partial class UC_Home_Admin : UserControl
    {
        public event Action<string> ChonChucNang;
        private readonly PaintEventHandler _cardPaintHandler;

        public UC_Home_Admin()
        {
            InitializeComponent();
            _cardPaintHandler = new PaintEventHandler(Card_PaintShadow);
            CreateFunctionCards();

            // Gán sự kiện resize cho UserControl để các card tự động điều chỉnh kích thước
            this.Resize += new System.EventHandler(this.UC_Home_Admin_Resize);
        }

        private void UC_Home_Admin_Load(object sender, EventArgs e)
        {
            // Gọi phương thức resize khi form được tải lần đầu
            UC_Home_Admin_Resize(this, EventArgs.Empty);
        }

        private void Card_PaintShadow(object sender, PaintEventArgs e)
        {
            var card = sender as Control;
            if (card == null) return;
            var g = e.Graphics;
            using (var shadow = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
                g.FillRectangle(shadow, 3, 3, card.Width - 3, card.Height - 3);
        }

        private void CreateFunctionCards()
        {
            // **FIX:** Chỉ định nghĩa 3 chức năng theo yêu cầu
            var adminFunctions = new[]
            {
                new { Name = "👨‍🏫 Quản lý Giáo viên", Tag = "Admin_QuanLyGV", Icon = "cn_teacher_management" },
                new { Name = "🏫 Quản lý Lớp học", Tag = "Admin_QuanLyLop", Icon = "cn_class_management" },
                new { Name = "Học vụ & Trường học", Tag = "Admin_QuanLyTruongHoc", Icon = "cn_school_management" }, // <-- THÊM DÒNG NÀY
                new { Name = "📊 Báo cáo Admin", Tag = "Admin_BaoCao", Icon = "cn_admin_report" }, // <<< THÊM DÒNG NÀY (chọn Icon phù hợp)
                new { Name = "🚪 Đăng xuất", Tag = "Admin_DangXuat", Icon = "cn_logout" }
            };

            const int idealCardWidth = 250;

            foreach (var func in adminFunctions)
            {
                Panel card = new Panel
                {
                    Width = idealCardWidth,
                    Height = 160,
                    Margin = new Padding(20),
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.None,
                    Tag = func.Tag,
                    Cursor = Cursors.Hand,
                };

                PictureBox pic = new PictureBox
                {
                    Dock = DockStyle.Fill,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    // LƯU Ý: Bạn cần có các icon tương ứng trong Properties.Resources
                    // Ví dụ: Properties.Resources.cn_teacher_management
                    Image = (Image)Properties.Resources.ResourceManager.GetObject(func.Icon) ?? Properties.Resources.logo, // fallback icon
                    BackColor = Color.White
                };

                Label lbl = new Label
                {
                    Text = func.Name,
                    Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(30, 40, 60),
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Bottom,
                    Height = 50
                };

                // Đăng ký các sự kiện
                card.Paint += _cardPaintHandler;
                card.MouseEnter += Card_MouseEnter;
                card.MouseLeave += Card_MouseLeave;
                card.Click += Card_Click;
                pic.Click += Card_Click;
                lbl.Click += Card_Click;

                card.Controls.Add(lbl);
                card.Controls.Add(pic);
                flowPanel.Controls.Add(card);
            }
        }

        private void Card_MouseEnter(object sender, EventArgs e)
        {
            Control control = sender as Control;
            Panel card = (control is Panel) ? control as Panel : control.Parent as Panel;
            if (card == null) return;
            card.BorderStyle = BorderStyle.FixedSingle;
            card.Refresh();
        }

        private void Card_MouseLeave(object sender, EventArgs e)
        {
            Control control = sender as Control;
            Panel card = (control is Panel) ? control as Panel : control.Parent as Panel;
            if (card == null) return;
            card.BorderStyle = BorderStyle.None;
        }

        private void Card_Click(object sender, EventArgs e)
        {
            Control control = sender as Control;
            Panel p = control as Panel ?? control.Parent as Panel;
            if (p?.Tag != null)
            {
                ChonChucNang?.Invoke(p.Tag.ToString());
            }
        }

        private void UC_Home_Admin_Resize(object sender, EventArgs e)
        {
            const int idealCardWidth = 250;
            const int cardMargin = 45;

            int panelWidth = flowPanel.ClientSize.Width;
            if (panelWidth <= 0) return;

            int columns = Math.Max(1, panelWidth / (idealCardWidth + cardMargin));

            int newCardWidth = (panelWidth / columns) - cardMargin;
            int newCardHeight = (int)(newCardWidth * 0.65) + 50;

            foreach (Panel card in flowPanel.Controls.OfType<Panel>())
            {
                card.Width = newCardWidth;
                card.Height = newCardHeight;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Resize -= this.UC_Home_Admin_Resize;

                if (flowPanel != null)
                {
                    foreach (var card in flowPanel.Controls.OfType<Panel>().ToArray())
                    {
                        card.Paint -= _cardPaintHandler;
                        card.MouseEnter -= Card_MouseEnter;
                        card.MouseLeave -= Card_MouseLeave;
                        card.Click -= Card_Click;
                        foreach (Control child in card.Controls)
                        {
                            child.Click -= Card_Click;
                        }
                    }
                }
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}