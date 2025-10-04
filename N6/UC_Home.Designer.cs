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
