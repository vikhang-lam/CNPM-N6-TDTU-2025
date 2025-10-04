using System;
using System.Drawing;
using System.Windows.Forms;

namespace N6
{
    partial class UC_PhanTichAI
    {
        private Label lblTitle;
        private Button btnPhanTich;
        private RichTextBox txtKetQua;
        private Panel pnlAI;

        private void InitializeComponent()
        {
            this.lblTitle = new Label();
            this.btnPhanTich = new Button();
            this.txtKetQua = new RichTextBox();
            this.pnlAI = new Panel();

            // lblTitle
            this.lblTitle.Text = "📊 PHÂN TÍCH DỮ LIỆU BẰNG AI";
            this.lblTitle.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            this.lblTitle.ForeColor = Color.White;
            this.lblTitle.Location = new Point(30, 20);
            this.lblTitle.AutoSize = true;

            // btnPhanTich
            this.btnPhanTich.Text = "🚀 Chạy phân tích";
            this.btnPhanTich.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.btnPhanTich.BackColor = Color.FromArgb(0, 200, 140);
            this.btnPhanTich.ForeColor = Color.White;
            this.btnPhanTich.FlatStyle = FlatStyle.Flat;
            this.btnPhanTich.FlatAppearance.BorderSize = 0;
            this.btnPhanTich.Size = new Size(180, 45);
            this.btnPhanTich.Location = new Point(40, 80);
            this.btnPhanTich.Click += new EventHandler(this.btnPhanTich_Click);

            // txtKetQua
            this.txtKetQua.Location = new Point(40, 140);
            this.txtKetQua.Size = new Size(860, 420);
            this.txtKetQua.Font = new Font("Consolas", 10F);
            this.txtKetQua.BackColor = Color.FromArgb(35, 40, 55);
            this.txtKetQua.ForeColor = Color.LightGreen;
            this.txtKetQua.ReadOnly = true;
            this.txtKetQua.Text = "Kết quả phân tích sẽ hiển thị ở đây...";

            // UC_PhanTichAI
            this.BackColor = Color.FromArgb(25, 30, 45);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.btnPhanTich);
            this.Controls.Add(this.txtKetQua);
            this.Size = new Size(940, 600);
        }
    }
}
