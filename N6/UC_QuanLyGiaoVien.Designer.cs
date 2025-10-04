using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
namespace N6
{
    partial class UC_QuanLyGiaoVien
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dgvGV;
        private System.Windows.Forms.Button btnReload;
        private System.Windows.Forms.Button btnChoDuyet;
        private System.Windows.Forms.Button btnXacNhan;
        private System.Windows.Forms.Button btnHuy;
        private System.Windows.Forms.Button btnSua;
        private System.Windows.Forms.Button btnXoa;
        private System.Windows.Forms.TextBox txtTen;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.TextBox txtSDT;
        private System.Windows.Forms.Label lblTen;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.Label lblSDT;
        private System.Windows.Forms.Panel panelEdit;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.dgvGV = new System.Windows.Forms.DataGridView();
            this.btnReload = new System.Windows.Forms.Button();
            this.btnChoDuyet = new System.Windows.Forms.Button();
            this.btnXacNhan = new System.Windows.Forms.Button();
            this.btnHuy = new System.Windows.Forms.Button();
            this.btnSua = new System.Windows.Forms.Button();
            this.btnXoa = new System.Windows.Forms.Button();
            this.txtTen = new System.Windows.Forms.TextBox();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.txtSDT = new System.Windows.Forms.TextBox();
            this.lblTen = new System.Windows.Forms.Label();
            this.lblEmail = new System.Windows.Forms.Label();
            this.lblSDT = new System.Windows.Forms.Label();
            this.panelEdit = new System.Windows.Forms.Panel();

            ((System.ComponentModel.ISupportInitialize)(this.dgvGV)).BeginInit();
            this.panelEdit.SuspendLayout();
            this.SuspendLayout();

            // === DGV ===
            this.dgvGV.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvGV.Location = new System.Drawing.Point(20, 20);
            this.dgvGV.Name = "dgvGV";
            this.dgvGV.Size = new System.Drawing.Size(900, 400);
            this.dgvGV.TabIndex = 0;
            this.dgvGV.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvGV_CellClick);

            // === PANEL EDIT ===
            this.panelEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.panelEdit.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelEdit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelEdit.Location = new System.Drawing.Point(20, 440);
            this.panelEdit.Size = new System.Drawing.Size(900, 160);
            this.panelEdit.Controls.Add(this.lblTen);
            this.panelEdit.Controls.Add(this.txtTen);
            this.panelEdit.Controls.Add(this.lblEmail);
            this.panelEdit.Controls.Add(this.txtEmail);
            this.panelEdit.Controls.Add(this.lblSDT);
            this.panelEdit.Controls.Add(this.txtSDT);
            this.panelEdit.Controls.Add(this.btnSua);
            this.panelEdit.Controls.Add(this.btnXoa);
            this.panelEdit.Controls.Add(this.btnChoDuyet);
            this.panelEdit.Controls.Add(this.btnXacNhan);
            this.panelEdit.Controls.Add(this.btnHuy);
            this.panelEdit.Controls.Add(this.btnReload);

            // === LABEL + TEXTBOX ===
            this.lblTen.Text = "Tên GV:";
            this.lblTen.Location = new System.Drawing.Point(20, 20);
            this.txtTen.Location = new System.Drawing.Point(90, 18);
            this.txtTen.Width = 200;

            this.lblEmail.Text = "Email:";
            this.lblEmail.Location = new System.Drawing.Point(310, 20);
            this.txtEmail.Location = new System.Drawing.Point(370, 18);
            this.txtEmail.Width = 200;

            this.lblSDT.Text = "SĐT:";
            this.lblSDT.Location = new System.Drawing.Point(600, 20);
            this.txtSDT.Location = new System.Drawing.Point(650, 18);
            this.txtSDT.Width = 200;

            // === BUTTON STYLE ===
            

            this.btnReload.Text = "🔄 Tải lại";
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);

            this.btnChoDuyet.Text = "⏳ Chờ duyệt";
            this.btnChoDuyet.Click += new System.EventHandler(this.btnChoDuyet_Click);

            this.btnXacNhan.Text = "✅ Xác nhận";
            this.btnXacNhan.Click += new System.EventHandler(this.btnXacNhan_Click);

            this.btnHuy.Text = "❌ Hủy";
            this.btnHuy.Click += new System.EventHandler(this.btnHuy_Click);

            this.btnSua.Text = "✏️ Sửa";
            this.btnSua.Click += new System.EventHandler(this.btnSua_Click);

            this.btnXoa.Text = "🗑️ Xóa";
            this.btnXoa.Click += new System.EventHandler(this.btnXoa_Click);

            // === UC ===
            this.Controls.Add(this.dgvGV);
            this.Controls.Add(this.panelEdit);
            this.BackColor = Color.FromArgb(245, 250, 255);
            this.Size = new System.Drawing.Size(940, 620);

            ((System.ComponentModel.ISupportInitialize)(this.dgvGV)).EndInit();
            this.panelEdit.ResumeLayout(false);
            this.panelEdit.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}
