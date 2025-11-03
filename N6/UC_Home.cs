using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace N6
{
    /// <summary>
    /// UserControl hiển thị màn hình trang chủ với các thẻ chức năng.
    /// </summary>
    public partial class UC_Home : UserControl
    {
        /// <summary>
        /// Sự kiện được kích hoạt khi người dùng chọn một thẻ chức năng.
        /// Trả về một chuỗi (string) là mã của chức năng đó (ví dụ: "CN1", "CN2").
        /// </summary>
        // CHUẨN HÓA: Đổi tên sự kiện sang Tiếng Anh (PascalCase)
        public event Action<string> FunctionSelected;

        private readonly PaintEventHandler _cardPaintHandler;

        /// <summary>
        /// Hàm khởi tạo, thiết lập lời chào và tạo các thẻ chức năng.
        /// </summary>
        /// <param name="tenGV">Tên của giáo viên để hiển thị lời chào.</param>
        public UC_Home(string tenGV = "Giáo viên")
        {
            InitializeComponent();
            _cardPaintHandler = new PaintEventHandler(Card_PaintShadow);
            lblLoiChao.Text = $"Xin chào, {tenGV}! 👋";
            CreateFunctionCards();

            this.Resize += new System.EventHandler(this.UC_Home_Resize);
        }

        private void UC_Home_Load(object sender, EventArgs e)
        {
            // Kích hoạt Resize khi tải để sắp xếp các thẻ
            UC_Home_Resize(this, EventArgs.Empty);
        }

        /// <summary>
        /// Vẽ một lớp bóng mờ đơn giản cho các thẻ.
        /// </summary>
        private void Card_PaintShadow(object sender, PaintEventArgs e)
        {
            var card = sender as Control;
            if (card == null) return;
            var g = e.Graphics;
            using (var shadow = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
                g.FillRectangle(shadow, 3, 3, card.Width - 3, card.Height - 3);
        }

        /// <summary>
        /// Tạo động các thẻ chức năng và thêm chúng vào FlowLayoutPanel.
        /// </summary>
        private void CreateFunctionCards()
        {
            string[] cnTen = {
                "🏠 Trang chủ", "👨‍🎓 Quản lý lớp học", "☁️ Thời khóa biểu",
                "📑 Quản lý tài liệu", "🎮 Mini-games", "📑 Báo cáo và Xuất dữ liệu",
                "📊 Phân tích AI", "🚪 Đăng xuất"
            };

            const int idealCardWidth = 250; // Định nghĩa chiều rộng lý tưởng của card

            for (int i = 0; i < cnTen.Length; i++)
            {
                Panel card = new Panel
                {
                    Width = idealCardWidth,
                    Height = 160,
                    Margin = new Padding(20),
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.None,
                    Tag = $"CN{i + 1}", // Gán mã chức năng vào Tag
                    Cursor = Cursors.Hand,
                };

                PictureBox pic = new PictureBox
                {
                    Dock = DockStyle.Fill,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Image = (Image)Properties.Resources.ResourceManager.GetObject($"cn{i + 1}"),
                    BackColor = Color.White
                };

                Label lbl = new Label
                {
                    Text = cnTen[i],
                    Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(30, 40, 60),
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Bottom,
                    Height = 50
                };

                // Đăng ký các sự kiện
                // CHUẨN HÓA: Xóa bỏ sự kiện card.Resize vì hàm Card_Resize rỗng.
                card.Paint += _cardPaintHandler;
                card.MouseEnter += Card_MouseEnter;
                card.MouseLeave += Card_MouseLeave;
                card.Click += Card_Click;
                pic.Click += Card_Click; // Gán cùng sự kiện Click
                lbl.Click += Card_Click; // Gán cùng sự kiện Click

                card.Controls.Add(lbl);
                card.Controls.Add(pic);
                flowPanel.Controls.Add(card);
            }
        }

        #region Card Events (Hover, Click)

        /// <summary>
        /// Xử lý hiệu ứng khi di chuột vào thẻ.
        /// </summary>
        private void Card_MouseEnter(object sender, EventArgs e)
        {
            Control control = sender as Control;
            Panel card = (control is Panel) ? (control as Panel) : (control.Parent as Panel);
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
            Panel card = (control is Panel) ? (control as Panel) : (control.Parent as Panel);
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
            if (p != null && p.Tag != null)
            {
                // CHUẨN HÓA: Gọi sự kiện đã đổi tên
                FunctionSelected?.Invoke(p.Tag.ToString());
            }
        }

        #endregion

        /// <summary>
        /// Xử lý logic responsive, sắp xếp lại kích thước các thẻ dựa trên kích thước của FlowPanel.
        /// </summary>
        private void UC_Home_Resize(object sender, EventArgs e)
        {
            // Tạm dừng layout để tránh giật/lag khi thay đổi kích thước nhiều control
            flowPanel.SuspendLayout();

            try
            {
                const int idealCardWidth = 250;
                const int cardHorizontalMargins = 40; // (Padding 20 mỗi bên)

                int panelWidth = flowPanel.ClientSize.Width;
                if (panelWidth <= 0) return;

                // Tính toán số cột
                int columns = Math.Max(1, panelWidth / (idealCardWidth + cardHorizontalMargins));

                // Tính toán chiều rộng mới cho mỗi thẻ
                int widthPerColumn = panelWidth / columns;
                int newCardWidth = widthPerColumn - cardHorizontalMargins - 4; // Trừ lề và 1 chút lỗi
                int newCardHeight = (int)(newCardWidth * 0.7) + 50; // Tính chiều cao (tỷ lệ 0.7) + chiều cao label

                // Áp dụng kích thước mới cho tất cả các thẻ
                foreach (Control ctrl in flowPanel.Controls)
                {
                    if (ctrl is Panel card)
                    {
                        card.Width = newCardWidth;
                        card.Height = newCardHeight;
                    }
                }
            }
            finally
            {
                // Tiếp tục layout
                flowPanel.ResumeLayout(true);
            }
        }

        // CHUẨN HÓA: Xóa phương thức Card_Resize() rỗng.

        /// <summary>
        /// Dọn dẹp tài nguyên và gỡ bỏ các trình xử lý sự kiện.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Gỡ sự kiện của UserControl
                this.Resize -= this.UC_Home_Resize;

                // Gỡ sự kiện của các control động (các thẻ)
                if (flowPanel != null && flowPanel.Controls != null)
                {
                    var cards = flowPanel.Controls.OfType<Panel>().ToArray();
                    foreach (var card in cards)
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