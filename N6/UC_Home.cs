// UC_Home.cs

using System;
using System.Drawing;
using System.Windows.Forms;

namespace N6
{
    public partial class UC_Home : UserControl
    {
        public event Action<string> ChonChucNang;

        public UC_Home(string tenGV = "Giáo viên")
        {
            InitializeComponent();
            lblLoiChao.Text = $"Xin chào, {tenGV}! 👋";
            CreateFunctionCards();

            // Gán sự kiện Resize cho flowPanel để layout co giãn
            this.flowPanel.Resize += new System.EventHandler(this.FlowPanel_Resize);
        }

        private void UC_Home_Load(object sender, EventArgs e)
        {
            // Gọi phương thức resize một lần khi tải để áp dụng layout ban đầu
            FlowPanel_Resize(this, EventArgs.Empty);
        }

        private void CreateFunctionCards()
        {
            // ======= Danh sách chức năng =======
            string[] cnTen = {
                "🏠 Trang chủ",
                "👨‍🎓 Quản lý lớp học",
                "☁️ Thời khóa biểu",
                "📑 Quản lý tài liệu",
                "🎮 Mini-games",
                "📑 Báo cáo & Xuất dữ liệu",
                "📊 Phân tích AI",
                "🚪 Đăng xuất"
            };

            for (int i = 0; i < cnTen.Length; i++)
            {
                Panel card = new Panel();
                // Kích thước ban đầu, sẽ được cập nhật bởi FlowPanel_Resize
                card.Width = 250;
                card.Height = 160;
                card.Margin = new Padding(20);
                card.BackColor = Color.White;
                card.BorderStyle = BorderStyle.None;
                card.Tag = $"CN{i + 1}";
                card.Cursor = Cursors.Hand;
                card.Resize += Card_Resize; // Để cập nhật lại ảnh bên trong

                // Ảnh minh họa
                PictureBox pic = new PictureBox();
                pic.Dock = DockStyle.Fill; // Dùng Dock để lấp đầy
                pic.SizeMode = PictureBoxSizeMode.Zoom;
                pic.Image = (Image)Properties.Resources.ResourceManager.GetObject($"cn{i + 1}");
                pic.BackColor = Color.White; // Nền của khung ảnh là màu trắng

                // Tên chức năng
                Label lbl = new Label();
                lbl.Text = cnTen[i];
                lbl.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
                lbl.ForeColor = Color.FromArgb(30, 40, 60);
                lbl.AutoSize = false;
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.Dock = DockStyle.Bottom;
                lbl.Height = 50;

                // Đổ bóng nhẹ (giả lập)
                card.Paint += (s, e) =>
                {
                    var g = e.Graphics;
                    using (var shadow = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
                        g.FillRectangle(shadow, 3, 3, card.Width - 3, card.Height - 3);
                };

                // Gắn sự kiện (Thêm Label trước, PictureBox sau)
                card.Controls.Add(lbl);
                card.Controls.Add(pic);
                card.MouseEnter += Card_MouseEnter;
                card.MouseLeave += Card_MouseLeave;
                card.Click += Card_Click;
                pic.Click += Card_Click; // Đảm bảo các control con cũng kích hoạt sự kiện
                lbl.Click += Card_Click;

                flowPanel.Controls.Add(card);
            }
        }

        private void Card_MouseEnter(object sender, EventArgs e)
        {
            Control control = sender as Control;
            Panel card = control is Panel ? control as Panel : control.Parent as Panel;
            if (card == null) return;

            card.BackColor = Color.White; // Giữ nền trắng khi di chuột qua
            card.BorderStyle = BorderStyle.FixedSingle;
            card.Refresh();
        }

        private void Card_MouseLeave(object sender, EventArgs e)
        {
            Control control = sender as Control;
            Panel card = control is Panel ? control as Panel : control.Parent as Panel;
            if (card == null) return;

            card.BackColor = Color.White;
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

        private void FlowPanel_Resize(object sender, EventArgs e)
        {
            int panelWidth = flowPanel.ClientSize.Width;
            int columns = Math.Max(1, panelWidth / 250);
            int newCardWidth = (panelWidth / columns) - 45;
            int newCardHeight = (int)(newCardWidth * 0.75) + 50;


            foreach (Control control in flowPanel.Controls)
            {
                if (control is Panel card)
                {
                    card.Width = newCardWidth;
                    card.Height = newCardHeight;
                }
            }
        }

        private void Card_Resize(object sender, EventArgs e)
        {
            Panel card = sender as Panel;
            if (card == null) return;

            foreach (Control control in card.Controls)
            {
                if (control is PictureBox pic)
                {
                    // Phần này giờ không cần thiết vì PictureBox đã được Dock = Fill
                    // Nó sẽ tự động thay đổi kích thước theo thẻ cha
                    break;
                }
            }
        }
    }
}