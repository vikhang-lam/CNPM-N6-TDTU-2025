using System;
using System.Drawing;
using System.Windows.Forms;

namespace N6
{
    partial class UC_BaoCao
    {
        private Label lblTitle;
        private Button btnXuatExcel;
        private Button btnXuatPDF;
        private Panel pnlContent;

        private void InitializeComponent()
        {
            this.lblTitle = new Label();
            this.btnXuatExcel = new Button();
            this.btnXuatPDF = new Button();
            this.pnlContent = new Panel();

            // lblTitle
            this.lblTitle.Text = "📑 BÁO CÁO & XUẤT DỮ LIỆU";
            this.lblTitle.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            this.lblTitle.ForeColor = Color.White;
            this.lblTitle.Location = new Point(30, 20);
            this.lblTitle.AutoSize = true;

            // btnXuatExcel
            this.btnXuatExcel.Text = "📊 Xuất Excel";
            this.btnXuatExcel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.btnXuatExcel.BackColor = Color.FromArgb(0, 120, 215);
            this.btnXuatExcel.ForeColor = Color.White;
            this.btnXuatExcel.FlatStyle = FlatStyle.Flat;
            this.btnXuatExcel.FlatAppearance.BorderSize = 0;
            this.btnXuatExcel.Size = new Size(150, 40);
            this.btnXuatExcel.Location = new Point(40, 80);
            this.btnXuatExcel.Click += new EventHandler(this.btnXuatExcel_Click);

            // btnXuatPDF
            this.btnXuatPDF.Text = "📄 Xuất PDF";
            this.btnXuatPDF.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.btnXuatPDF.BackColor = Color.FromArgb(220, 70, 70);
            this.btnXuatPDF.ForeColor = Color.White;
            this.btnXuatPDF.FlatStyle = FlatStyle.Flat;
            this.btnXuatPDF.FlatAppearance.BorderSize = 0;
            this.btnXuatPDF.Size = new Size(150, 40);
            this.btnXuatPDF.Location = new Point(210, 80);
            this.btnXuatPDF.Click += new EventHandler(this.btnXuatPDF_Click);

            // pnlContent
            this.pnlContent.Location = new Point(30, 140);
            this.pnlContent.Size = new Size(860, 420);
            this.pnlContent.BackColor = Color.FromArgb(35, 40, 55);
            this.pnlContent.BorderStyle = BorderStyle.FixedSingle;

            // UC_BaoCao
            this.BackColor = Color.FromArgb(25, 30, 45);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.btnXuatExcel);
            this.Controls.Add(this.btnXuatPDF);
            this.Controls.Add(this.pnlContent);
            this.Size = new Size(940, 600);
        }
    }
}
