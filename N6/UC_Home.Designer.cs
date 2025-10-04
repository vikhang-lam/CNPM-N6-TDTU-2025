using System;
using System.Drawing;
using System.Windows.Forms;

namespace N6
{
    partial class UC_Home
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblLoiChao;
        private Label lblPhuDe;
        private FlowLayoutPanel flowPanel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblLoiChao = new Label();
            this.lblPhuDe = new Label();
            this.flowPanel = new FlowLayoutPanel();
            this.SuspendLayout();

            // ======= Tiêu đề =======
            this.lblLoiChao.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
            this.lblLoiChao.ForeColor = Color.FromArgb(20, 40, 80);
            this.lblLoiChao.Location = new Point(40, 25);
            this.lblLoiChao.AutoSize = true;

            this.lblPhuDe.Font = new Font("Segoe UI", 11F);
            this.lblPhuDe.ForeColor = Color.FromArgb(80, 90, 100);
            this.lblPhuDe.Location = new Point(45, 75);
            this.lblPhuDe.Text = "Chúc bạn một ngày làm việc hiệu quả và nhiều niềm vui 🌼";
            this.lblPhuDe.AutoSize = true;

            // ======= Flow Panel =======
            this.flowPanel.Location = new Point(30, 130);
            this.flowPanel.Size = new Size(880, 430);
            this.flowPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.flowPanel.BackColor = Color.Transparent;
            this.flowPanel.AutoScroll = true;
            this.flowPanel.WrapContents = true;

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
                pic.Click += Card_Click;
                lbl.Click += Card_Click;

                flowPanel.Controls.Add(card);
            }

            // ======= Toàn màn hình =======
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(240, 245, 255); // nền sáng dịu
            this.Controls.Add(this.lblLoiChao);
            this.Controls.Add(this.lblPhuDe);
            this.Controls.Add(this.flowPanel);
            this.Name = "UC_Home";
            this.Size = new Size(940, 600);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
