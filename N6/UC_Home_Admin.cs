using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics; // Thêm

namespace N6
{
    /// <summary>
    /// UserControl hiển thị trang chủ (Home) cho Admin, chứa các thẻ điều hướng chính.
    /// </summary>
    public partial class UC_Home_Admin : UserControl
    {
        /// <summary>
        /// Sự kiện được kích hoạt khi người dùng chọn một thẻ chức năng Admin.
        /// Trả về một chuỗi (string) là mã của chức năng đó (ví dụ: "Admin_QuanLyGV").
        /// </summary>
        public event Action<string> FunctionSelected; // CHUẨN HÓA: Đổi tên từ ChonChucNang
        private readonly PaintEventHandler _cardPaintHandler;

        public UC_Home_Admin()
        {
            InitializeComponent();
            _cardPaintHandler = new PaintEventHandler(Card_PaintShadow);
            CreateFunctionCards();

            // Gán sự kiện resize cho UserControl để các card tự động điều chỉnh kích thước
            this.Resize += new System.EventHandler(this.UC_Home_Admin_Resize);
            this.Load += new System.EventHandler(this.UC_Home_Admin_Load);
        }

        private void UC_Home_Admin_Load(object sender, EventArgs e)
        {
            // Kích hoạt resize khi tải để sắp xếp các thẻ
            UC_Home_Admin_Resize(this, EventArgs.Empty);
        }

        /// <summary>
        /// Vẽ hiệu ứng bóng mờ (shadow) đơn giản cho các thẻ.
        /// </summary>
        private void Card_PaintShadow(object sender, PaintEventArgs e)
        {
            var card = sender as Control;
            if (card == null) return;
            var g = e.Graphics;
            using (var shadow = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
            {
                g.FillRectangle(shadow, 3, 3, card.Width - 3, card.Height - 3);
            }
        }

        /// <summary>
        /// Tạo động các thẻ (card) chức năng cho Admin.
        /// </summary>
        private void CreateFunctionCards()
        {
            // Định nghĩa các chức năng của Admin
            var adminFunctions = new[]
            {
                new { Name = "👨‍🏫 Quản lý Giáo viên", Tag = "Admin_QuanLyGV", Icon = "cn_teacher_management" },
                new { Name = "🏫 Quản lý Lớp học", Tag = "Admin_QuanLyLop", Icon = "cn_class_management" },
                new { Name = "🎓 Quản lý Trường học", Tag = "Admin_QuanLyTruongHoc", Icon = "cn_school_management" },
                new { Name = "📊 Báo cáo Admin", Tag = "Admin_BaoCao", Icon = "cn6" },
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

                // Đăng ký các sự kiện (sẽ được gỡ trong Dispose)
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

        /// <summary>
        /// Xử lý hiệu ứng khi di chuột vào thẻ.
        /// </summary>
        private void Card_MouseEnter(object sender, EventArgs e)
        {
            Control control = sender as Control;
            Panel card = (control is Panel) ? control as Panel : control.Parent as Panel;
            if (card == null) return;
            card.BorderStyle = BorderStyle.FixedSingle;
            card.Refresh();
        }

        /// <summary>
        /// Xử lý hiệu ứng khi di chuột ra khỏi thẻ.
        /// </summary>
        private void Card_MouseLeave(object sender, EventArgs e)
        {
            Control control = sender as Control;
            Panel card = (control is Panel) ? control as Panel : control.Parent as Panel;
            if (card == null) return;
            card.BorderStyle = BorderStyle.None;
        }

        /// <summary>
        /// Xử lý khi click vào thẻ, kích hoạt sự kiện FunctionSelected.
        /// </summary>
        private void Card_Click(object sender, EventArgs e)
        {
            Control control = sender as Control;
            Panel p = control as Panel ?? control.Parent as Panel;
            if (p?.Tag != null)
            {
                FunctionSelected?.Invoke(p.Tag.ToString()); // CHUẨN HÓA: Gọi sự kiện đã đổi tên
            }
        }

        /// <summary>
        /// Xử lý logic responsive, sắp xếp lại kích thước các thẻ dựa trên kích thước của FlowPanel.
        /// </summary>
        private void UC_Home_Admin_Resize(object sender, EventArgs e)
        {
            const int idealCardWidth = 250;
            const int cardMargin = 45; // Tổng margin ngang (20 + 20) + 5 an toàn

            int panelWidth = flowPanel.ClientSize.Width;
            if (panelWidth <= 0) return;

            // Tính toán số cột
            int columns = Math.Max(1, panelWidth / (idealCardWidth + cardMargin));

            // Tính toán chiều rộng mới cho mỗi thẻ
            int newCardWidth = (panelWidth / columns) - cardMargin;
            int newCardHeight = (int)(newCardWidth * 0.65) + 50; // Tính chiều cao

            // Áp dụng kích thước mới
            foreach (Panel card in flowPanel.Controls.OfType<Panel>())
            {
                card.Width = newCardWidth;
                card.Height = newCardHeight;
            }
        }

        /// <summary>
        /// Dọn dẹp tài nguyên và gỡ bỏ các trình xử lý sự kiện.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Gỡ sự kiện của UserControl
                this.Resize -= this.UC_Home_Admin_Resize;
                this.Load -= this.UC_Home_Admin_Load; // Gỡ sự kiện Load

                // Gỡ sự kiện của các control động (các thẻ)
                if (flowPanel != null)
                {
                    // Dùng ToArray() để tạo bản sao, tránh lỗi thay đổi collection khi đang duyệt
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
                        // Không cần gọi card.Dispose() vì flowPanel.Dispose() sẽ làm việc đó
                    }
                }

                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}