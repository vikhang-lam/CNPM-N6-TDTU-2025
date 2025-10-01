namespace N6
{
    partial class UC_QuanLyLop
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel panelSidebar;
        private System.Windows.Forms.Button btnDiemDanh;
        private System.Windows.Forms.Button btnQR;
        private System.Windows.Forms.Button btnKetQua;
        private System.Windows.Forms.Button btnHocSinh;
        private System.Windows.Forms.Panel panelContent;

        private System.Windows.Forms.DataGridView dgvDiemDanh;
        private System.Windows.Forms.Button btnLuuDiemDanh;

        private System.Windows.Forms.PictureBox pictureQR;
        private System.Windows.Forms.Button btnStartQR;
        private System.Windows.Forms.Button btnStopQR;
        private System.Windows.Forms.Label lblQRStatus;

        private System.Windows.Forms.DataGridView dgvKetQua;
        private System.Windows.Forms.Button btnLuuKQ;

        private System.Windows.Forms.DataGridView dgvHocSinh;
        private System.Windows.Forms.Button btnXemHoSo;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelSidebar = new System.Windows.Forms.Panel();
            this.btnHocSinh = new System.Windows.Forms.Button();
            this.btnKetQua = new System.Windows.Forms.Button();
            this.btnQR = new System.Windows.Forms.Button();
            this.btnDiemDanh = new System.Windows.Forms.Button();
            this.panelContent = new System.Windows.Forms.Panel();

            this.dgvDiemDanh = new System.Windows.Forms.DataGridView();
            this.btnLuuDiemDanh = new System.Windows.Forms.Button();

            this.pictureQR = new System.Windows.Forms.PictureBox();
            this.btnStartQR = new System.Windows.Forms.Button();
            this.btnStopQR = new System.Windows.Forms.Button();
            this.lblQRStatus = new System.Windows.Forms.Label();

            this.dgvKetQua = new System.Windows.Forms.DataGridView();
            this.btnLuuKQ = new System.Windows.Forms.Button();

            this.dgvHocSinh = new System.Windows.Forms.DataGridView();
            this.btnXemHoSo = new System.Windows.Forms.Button();

            this.panelSidebar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDiemDanh)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureQR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvKetQua)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHocSinh)).BeginInit();
            this.SuspendLayout();
            // 
            // panelSidebar
            // 
            this.panelSidebar.BackColor = System.Drawing.Color.FromArgb(30, 30, 45);
            this.panelSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelSidebar.Width = 180;
            this.panelSidebar.Controls.Add(this.btnHocSinh);
            this.panelSidebar.Controls.Add(this.btnKetQua);
            this.panelSidebar.Controls.Add(this.btnQR);
            this.panelSidebar.Controls.Add(this.btnDiemDanh);
            // 
            // Sidebar Buttons
            // 
            this.btnDiemDanh.Text = "👥 Điểm danh";
            this.btnQR.Text = "📷 Điểm danh QR";
            this.btnKetQua.Text = "📊 Kết quả";
            this.btnHocSinh.Text = "📑 Hồ sơ";

            System.Windows.Forms.Button[] sidebarBtns = { btnDiemDanh, btnQR, btnKetQua, btnHocSinh };
            foreach (var btn in sidebarBtns)
            {
                btn.Dock = System.Windows.Forms.DockStyle.Top;
                btn.Height = 50;
                btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
                btn.ForeColor = System.Drawing.Color.White;
                btn.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
                btn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                btn.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
                btn.BackColor = System.Drawing.Color.FromArgb(45, 45, 65);
            }

            // 
            // panelContent
            // 
            this.panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContent.BackColor = System.Drawing.Color.WhiteSmoke;
            // 
            // UC_QuanLyLop
            // 
            this.Controls.Add(this.panelContent);
            this.Controls.Add(this.panelSidebar);
            this.Size = new System.Drawing.Size(1000, 600);

            this.panelSidebar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDiemDanh)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureQR)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvKetQua)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHocSinh)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
