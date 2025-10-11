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

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing && (components != null))
        //        components.Dispose();
        //    base.Dispose(disposing);
        //}

        private void InitializeComponent()
        {
            this.lblLoiChao = new System.Windows.Forms.Label();
            this.lblPhuDe = new System.Windows.Forms.Label();
            this.flowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // lblLoiChao
            // 
            this.lblLoiChao.AutoSize = true;
            this.lblLoiChao.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lblLoiChao.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(40)))), ((int)(((byte)(80)))));
            this.lblLoiChao.Location = new System.Drawing.Point(40, 25);
            this.lblLoiChao.Name = "lblLoiChao";
            this.lblLoiChao.Size = new System.Drawing.Size(0, 50);
            this.lblLoiChao.TabIndex = 0;
            // 
            // lblPhuDe
            // 
            this.lblPhuDe.AutoSize = true;
            this.lblPhuDe.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblPhuDe.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(90)))), ((int)(((byte)(100)))));
            this.lblPhuDe.Location = new System.Drawing.Point(45, 75);
            this.lblPhuDe.Name = "lblPhuDe";
            this.lblPhuDe.Size = new System.Drawing.Size(507, 25);
            this.lblPhuDe.TabIndex = 1;
            this.lblPhuDe.Text = "Chúc bạn một ngày làm việc hiệu quả và nhiều niềm vui 🌼";
            // 
            // flowPanel
            // 
            this.flowPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowPanel.AutoScroll = true;
            this.flowPanel.BackColor = System.Drawing.Color.Transparent;
            this.flowPanel.Location = new System.Drawing.Point(30, 130);
            this.flowPanel.Name = "flowPanel";
            this.flowPanel.Size = new System.Drawing.Size(880, 430);
            this.flowPanel.TabIndex = 2;
            // 
            // UC_Home
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(255)))));
            this.Controls.Add(this.lblLoiChao);
            this.Controls.Add(this.lblPhuDe);
            this.Controls.Add(this.flowPanel);
            this.Name = "UC_Home";
            this.Size = new System.Drawing.Size(940, 600);
            this.Load += new System.EventHandler(this.UC_Home_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
