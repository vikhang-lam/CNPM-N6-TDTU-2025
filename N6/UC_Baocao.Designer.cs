using System.Windows.Forms;

namespace N6
{
    partial class UC_BaoCao
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblTitle;
        private Button btnXuatExcel;
        private Button btnXuatPDF;
        private ComboBox cboLoaiBaoCao;
        private ComboBox cboLop;
        private ComboBox cboHocKy;
        private ComboBox cboKhoi;
        private Button btnXuatBaoCao;
        private System.Windows.Forms.DataGridView dgvDuLieu;

        // New layout containers
        private TableLayoutPanel layoutRoot;
        private Panel pnlHeader;
        private FlowLayoutPanel pnlHeaderRight;
        private FlowLayoutPanel pnlFilters;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnXuatExcel = new System.Windows.Forms.Button();
            this.btnXuatPDF = new System.Windows.Forms.Button();
            this.cboLoaiBaoCao = new System.Windows.Forms.ComboBox();
            this.cboLop = new System.Windows.Forms.ComboBox();
            this.cboHocKy = new System.Windows.Forms.ComboBox();
            this.cboKhoi = new System.Windows.Forms.ComboBox();
            this.btnXuatBaoCao = new System.Windows.Forms.Button();
            this.dgvDuLieu = new System.Windows.Forms.DataGridView();
            this.layoutRoot = new System.Windows.Forms.TableLayoutPanel();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.pnlHeaderRight = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlFilters = new System.Windows.Forms.FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDuLieu)).BeginInit();
            this.layoutRoot.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.pnlHeaderRight.SuspendLayout();
            this.pnlFilters.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(20, 10);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(422, 41);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "📑 BÁO CÁO & XUẤT DỮ LIỆU";
            // 
            // btnXuatExcel
            // 
            this.btnXuatExcel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.btnXuatExcel.FlatAppearance.BorderSize = 0;
            this.btnXuatExcel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnXuatExcel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnXuatExcel.ForeColor = System.Drawing.Color.White;
            this.btnXuatExcel.Location = new System.Drawing.Point(190, 5);
            this.btnXuatExcel.Margin = new System.Windows.Forms.Padding(10, 5, 0, 5);
            this.btnXuatExcel.Name = "btnXuatExcel";
            this.btnXuatExcel.Size = new System.Drawing.Size(150, 36);
            this.btnXuatExcel.TabIndex = 0;
            this.btnXuatExcel.Text = "📊 Xuất Excel";
            this.btnXuatExcel.UseVisualStyleBackColor = false;
            this.btnXuatExcel.Click += new System.EventHandler(this.btnXuatExcel_Click);
            // 
            // btnXuatPDF
            // 
            this.btnXuatPDF.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.btnXuatPDF.FlatAppearance.BorderSize = 0;
            this.btnXuatPDF.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnXuatPDF.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnXuatPDF.ForeColor = System.Drawing.Color.White;
            this.btnXuatPDF.Location = new System.Drawing.Point(20, 5);
            this.btnXuatPDF.Margin = new System.Windows.Forms.Padding(10, 5, 10, 5);
            this.btnXuatPDF.Name = "btnXuatPDF";
            this.btnXuatPDF.Size = new System.Drawing.Size(150, 36);
            this.btnXuatPDF.TabIndex = 1;
            this.btnXuatPDF.Text = "📄 Xuất PDF";
            this.btnXuatPDF.UseVisualStyleBackColor = false;
            this.btnXuatPDF.Click += new System.EventHandler(this.btnXuatPDF_Click);
            // 
            // cboLoaiBaoCao
            // 
            this.cboLoaiBaoCao.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLoaiBaoCao.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboLoaiBaoCao.Items.AddRange(new object[] {
            "Báo cáo chuyên cần",
            "Bảng điểm học kỳ",
            "Hồ sơ học sinh",
            "Thống kê tổng hợp khối"});
            this.cboLoaiBaoCao.Location = new System.Drawing.Point(20, 15);
            this.cboLoaiBaoCao.Margin = new System.Windows.Forms.Padding(0, 5, 15, 5);
            this.cboLoaiBaoCao.Name = "cboLoaiBaoCao";
            this.cboLoaiBaoCao.Size = new System.Drawing.Size(230, 31);
            this.cboLoaiBaoCao.TabIndex = 0;
            this.cboLoaiBaoCao.SelectedIndexChanged += new System.EventHandler(this.cboLoaiBaoCao_SelectedIndexChanged);
            // 
            // cboLop
            // 
            this.cboLop.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLop.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboLop.Location = new System.Drawing.Point(430, 15);
            this.cboLop.Margin = new System.Windows.Forms.Padding(0, 5, 15, 5);
            this.cboLop.Name = "cboLop";
            this.cboLop.Size = new System.Drawing.Size(180, 31);
            this.cboLop.TabIndex = 2;
            this.cboLop.Visible = false;
            // 
            // cboHocKy
            // 
            this.cboHocKy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboHocKy.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboHocKy.Items.AddRange(new object[] {
            "Học kỳ 1",
            "Học kỳ 2"});
            this.cboHocKy.Location = new System.Drawing.Point(625, 15);
            this.cboHocKy.Margin = new System.Windows.Forms.Padding(0, 5, 15, 5);
            this.cboHocKy.Name = "cboHocKy";
            this.cboHocKy.Size = new System.Drawing.Size(130, 31);
            this.cboHocKy.TabIndex = 3;
            this.cboHocKy.Visible = false;
            // 
            // cboKhoi
            // 
            this.cboKhoi.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboKhoi.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboKhoi.Location = new System.Drawing.Point(265, 15);
            this.cboKhoi.Margin = new System.Windows.Forms.Padding(0, 5, 15, 5);
            this.cboKhoi.Name = "cboKhoi";
            this.cboKhoi.Size = new System.Drawing.Size(150, 31);
            this.cboKhoi.TabIndex = 1;
            this.cboKhoi.Visible = false;
            // 
            // btnXuatBaoCao
            // 
            this.btnXuatBaoCao.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(139)))), ((int)(((byte)(34)))));
            this.btnXuatBaoCao.FlatAppearance.BorderSize = 0;
            this.btnXuatBaoCao.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnXuatBaoCao.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnXuatBaoCao.ForeColor = System.Drawing.Color.White;
            this.btnXuatBaoCao.Location = new System.Drawing.Point(770, 15);
            this.btnXuatBaoCao.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.btnXuatBaoCao.Name = "btnXuatBaoCao";
            this.btnXuatBaoCao.Size = new System.Drawing.Size(160, 36);
            this.btnXuatBaoCao.TabIndex = 4;
            this.btnXuatBaoCao.Text = "📋 Xuất Báo Cáo";
            this.btnXuatBaoCao.UseVisualStyleBackColor = false;
            this.btnXuatBaoCao.Click += new System.EventHandler(this.btnXuatBaoCao_Click);
            // 
            // dgvDuLieu
            // 
            this.dgvDuLieu.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvDuLieu.BackgroundColor = System.Drawing.Color.White;
            this.dgvDuLieu.ColumnHeadersHeight = 29;
            this.dgvDuLieu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDuLieu.Location = new System.Drawing.Point(3, 123);
            this.dgvDuLieu.Name = "dgvDuLieu";
            this.dgvDuLieu.ReadOnly = true;
            this.dgvDuLieu.RowHeadersWidth = 51;
            this.dgvDuLieu.Size = new System.Drawing.Size(934, 474);
            this.dgvDuLieu.TabIndex = 2;
            // 
            // layoutRoot
            // 
            this.layoutRoot.ColumnCount = 1;
            this.layoutRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.layoutRoot.Controls.Add(this.pnlHeader, 0, 0);
            this.layoutRoot.Controls.Add(this.pnlFilters, 0, 1);
            this.layoutRoot.Controls.Add(this.dgvDuLieu, 0, 2);
            this.layoutRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutRoot.Location = new System.Drawing.Point(0, 0);
            this.layoutRoot.Name = "layoutRoot";
            this.layoutRoot.RowCount = 3;
            this.layoutRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.layoutRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.layoutRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutRoot.Size = new System.Drawing.Size(940, 600);
            this.layoutRoot.TabIndex = 0;
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(30)))), ((int)(((byte)(45)))));
            this.pnlHeader.Controls.Add(this.pnlHeaderRight);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlHeader.Location = new System.Drawing.Point(3, 3);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Padding = new System.Windows.Forms.Padding(20, 10, 20, 10);
            this.pnlHeader.Size = new System.Drawing.Size(934, 54);
            this.pnlHeader.TabIndex = 0;
            // 
            // pnlHeaderRight
            // 
            this.pnlHeaderRight.Controls.Add(this.btnXuatExcel);
            this.pnlHeaderRight.Controls.Add(this.btnXuatPDF);
            this.pnlHeaderRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlHeaderRight.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.pnlHeaderRight.Location = new System.Drawing.Point(574, 10);
            this.pnlHeaderRight.Margin = new System.Windows.Forms.Padding(0);
            this.pnlHeaderRight.Name = "pnlHeaderRight";
            this.pnlHeaderRight.Size = new System.Drawing.Size(340, 34);
            this.pnlHeaderRight.TabIndex = 0;
            this.pnlHeaderRight.WrapContents = false;
            // 
            // pnlFilters
            // 
            this.pnlFilters.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(40)))), ((int)(((byte)(55)))));
            this.pnlFilters.Controls.Add(this.cboLoaiBaoCao);
            this.pnlFilters.Controls.Add(this.cboKhoi);
            this.pnlFilters.Controls.Add(this.cboLop);
            this.pnlFilters.Controls.Add(this.cboHocKy);
            this.pnlFilters.Controls.Add(this.btnXuatBaoCao);
            this.pnlFilters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlFilters.Location = new System.Drawing.Point(3, 63);
            this.pnlFilters.Name = "pnlFilters";
            this.pnlFilters.Padding = new System.Windows.Forms.Padding(20, 10, 20, 10);
            this.pnlFilters.Size = new System.Drawing.Size(934, 54);
            this.pnlFilters.TabIndex = 1;
            this.pnlFilters.WrapContents = false;
            // 
            // UC_BaoCao
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(30)))), ((int)(((byte)(45)))));
            this.Controls.Add(this.layoutRoot);
            this.Name = "UC_BaoCao";
            this.Size = new System.Drawing.Size(940, 600);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDuLieu)).EndInit();
            this.layoutRoot.ResumeLayout(false);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlHeaderRight.ResumeLayout(false);
            this.pnlFilters.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}