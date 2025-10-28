using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace N6
{
    public partial class UC_Home : UserControl
    {
        public event Action<string> ChonChucNang;
        private readonly PaintEventHandler _cardPaintHandler;

        public UC_Home(string tenGV = "Giáo viên")
        {
            InitializeComponent();
            _cardPaintHandler = new PaintEventHandler(Card_PaintShadow);
            lblLoiChao.Text = $"Xin chào, {tenGV}! 👋";
            CreateFunctionCards();

            // === THAY ĐỔI 1: Gán sự kiện cho UserControl thay vì FlowLayoutPanel ===
            this.Resize += new System.EventHandler(this.UC_Home_Resize);
        }

        private void UC_Home_Load(object sender, EventArgs e)
        {
            // === THAY ĐỔI 2: Gọi phương thức resize của UserControl khi tải ===
            UC_Home_Resize(this, EventArgs.Empty);
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
                    Tag = $"CN{i + 1}",
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
                card.Resize += Card_Resize;
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
            Panel card = control is Panel ? control as Panel : control.Parent as Panel;
            if (card == null) return;
            card.BorderStyle = BorderStyle.FixedSingle;
            card.Refresh();
        }

        private void Card_MouseLeave(object sender, EventArgs e)
        {
            Control control = sender as Control;
            Panel card = control is Panel ? control as Panel : control.Parent as Panel;
            if (card == null) return;
            card.BorderStyle = BorderStyle.None;
        }

        private void Card_Click(object sender, EventArgs e)
        {
            Control control = sender as Control;
            Panel p = control as Panel ?? control.Parent as Panel;
            if (p != null && p.Tag != null)
            {
                ChonChucNang?.Invoke(p.Tag.ToString());
            }
        }

        // === THAY ĐỔI 3: Đổi tên phương thức và giữ nguyên logic ===
        private void UC_Home_Resize(object sender, EventArgs e)
        {
            flowPanel.SuspendLayout();

            try
            {
                const int idealCardWidth = 250;
                const int cardHorizontalMargins = 40;

                int panelWidth = flowPanel.ClientSize.Width;

                int columns = Math.Max(1, panelWidth / (idealCardWidth + cardHorizontalMargins));

                int widthPerColumn = panelWidth / columns;

                int newCardWidth = widthPerColumn - cardHorizontalMargins - 4;

                int newCardHeight = (int)(newCardWidth * 0.7) + 50;

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
                flowPanel.ResumeLayout(true);
            }
        }

        private void Card_Resize(object sender, EventArgs e)
        {
            // Không cần làm gì vì PictureBox đã được Dock = Fill
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // === THAY ĐỔI 4: Hủy đăng ký sự kiện Resize của UserControl ===
                this.Resize -= this.UC_Home_Resize;

                if (flowPanel != null && flowPanel.Controls != null)
                {
                    var cards = flowPanel.Controls.OfType<Panel>().ToArray();
                    foreach (var card in cards)
                    {
                        card.Resize -= Card_Resize;
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

                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}