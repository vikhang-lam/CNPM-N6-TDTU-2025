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
            CreateFunctionCards(); // Call the new method here
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
                card.Width = 250;
                card.Height = 160;
                card.Margin = new Padding(20);
                card.BackColor = Color.White;
                card.BorderStyle = BorderStyle.None;
                card.Tag = $"CN{i + 1}";
                card.Cursor = Cursors.Hand;

                // Ảnh minh họa
                PictureBox pic = new PictureBox();
                pic.Size = new Size(90, 90);
                pic.Location = new Point((card.Width - 90) / 2, 15);
                pic.SizeMode = PictureBoxSizeMode.Zoom;
                pic.Image = (Image)Properties.Resources.ResourceManager.GetObject($"cn{i + 1}");
                pic.BackColor = Color.Transparent;

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

                // Gắn sự kiện
                card.Controls.Add(pic);
                card.Controls.Add(lbl);
                card.MouseEnter += Card_MouseEnter;
                card.MouseLeave += Card_MouseLeave;
                card.Click += Card_Click;
                pic.Click += Card_Click; // Ensure child controls also trigger the event
                lbl.Click += Card_Click; // Ensure child controls also trigger the event

                flowPanel.Controls.Add(card);
            }
        }

        private void Card_MouseEnter(object sender, EventArgs e)
        {
            // Note: The 'sender' might be the Label or PictureBox, so we find the parent Panel.
            Control control = sender as Control;
            Panel card = control is Panel ? control as Panel : control.Parent as Panel;
            if (card == null) return;

            card.BackColor = Color.FromArgb(230, 243, 255); // xanh nhạt dịu
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
            // Find the parent Panel regardless of what was clicked (Panel, PictureBox, or Label)
            Control control = sender as Control;
            Panel p = control as Panel ?? control.Parent as Panel;

            if (p != null && p.Tag != null)
            {
                ChonChucNang?.Invoke(p.Tag.ToString());
            }
        }
    }
}