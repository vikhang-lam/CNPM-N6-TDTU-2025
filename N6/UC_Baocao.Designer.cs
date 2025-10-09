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
            this.lblTitle = new Label();
            this.btnXuatExcel = new Button();
            this.btnXuatPDF = new Button();
            this.cboLoaiBaoCao = new ComboBox();
            this.cboLop = new ComboBox();
            this.cboHocKy = new ComboBox();
            this.cboKhoi = new ComboBox();
            this.btnXuatBaoCao = new Button();
            this.dgvDuLieu = new System.Windows.Forms.DataGridView();

            this.layoutRoot = new TableLayoutPanel();
            this.pnlHeader = new Panel();
            this.pnlHeaderRight = new FlowLayoutPanel();
            this.pnlFilters = new FlowLayoutPanel();

            // layoutRoot
            this.layoutRoot.ColumnCount = 1;
            this.layoutRoot.RowCount = 3;
            this.layoutRoot.Dock = DockStyle.Fill;
            this.layoutRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F)); // header
            this.layoutRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F)); // filters
            this.layoutRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // grid

            // pnlHeader
            this.pnlHeader.Dock = DockStyle.Fill;
            this.pnlHeader.Padding = new Padding(20, 10, 20, 10);
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(25, 30, 45);

            // lblTitle
            this.lblTitle.Text = "📑 BÁO CÁO & XUẤT DỮ LIỆU";
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.AutoSize = true;
            this.lblTitle.Dock = DockStyle.Left;

            // pnlHeaderRight
            this.pnlHeaderRight.Dock = DockStyle.Right;
            this.pnlHeaderRight.FlowDirection = FlowDirection.RightToLeft;
            this.pnlHeaderRight.WrapContents = false;
            this.pnlHeaderRight.AutoSize = false;
            this.pnlHeaderRight.Width = 340;
            this.pnlHeaderRight.Padding = new Padding(0);
            this.pnlHeaderRight.Margin = new Padding(0);

            // btnXuatExcel
            this.btnXuatExcel.Text = "📊 Xuất Excel";
            this.btnXuatExcel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnXuatExcel.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this.btnXuatExcel.ForeColor = System.Drawing.Color.White;
            this.btnXuatExcel.FlatStyle = FlatStyle.Flat;
            this.btnXuatExcel.FlatAppearance.BorderSize = 0;
            this.btnXuatExcel.Size = new System.Drawing.Size(150, 36);
            this.btnXuatExcel.Margin = new Padding(10, 5, 0, 5);
            this.btnXuatExcel.Click += new System.EventHandler(this.btnXuatExcel_Click);

            // btnXuatPDF
            this.btnXuatPDF.Text = "📄 Xuất PDF";
            this.btnXuatPDF.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnXuatPDF.BackColor = System.Drawing.Color.FromArgb(220, 70, 70);
            this.btnXuatPDF.ForeColor = System.Drawing.Color.White;
            this.btnXuatPDF.FlatStyle = FlatStyle.Flat;
            this.btnXuatPDF.FlatAppearance.BorderSize = 0;
            this.btnXuatPDF.Size = new System.Drawing.Size(150, 36);
            this.btnXuatPDF.Margin = new Padding(10, 5, 10, 5);
            this.btnXuatPDF.Click += new System.EventHandler(this.btnXuatPDF_Click);

            this.pnlHeaderRight.Controls.Add(this.btnXuatExcel);
            this.pnlHeaderRight.Controls.Add(this.btnXuatPDF);

            this.pnlHeader.Controls.Add(this.pnlHeaderRight);
            this.pnlHeader.Controls.Add(this.lblTitle);

            // pnlFilters
            this.pnlFilters.Dock = DockStyle.Fill;
            this.pnlFilters.FlowDirection = FlowDirection.LeftToRight;
            this.pnlFilters.WrapContents = false;
            this.pnlFilters.Padding = new Padding(20, 10, 20, 10);
            this.pnlFilters.BackColor = System.Drawing.Color.FromArgb(35, 40, 55);

            // cboLoaiBaoCao
            this.cboLoaiBaoCao.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboLoaiBaoCao.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboLoaiBaoCao.Size = new System.Drawing.Size(230, 35);
            this.cboLoaiBaoCao.Margin = new Padding(0, 5, 15, 5);
            this.cboLoaiBaoCao.Items.AddRange(new object[] {
                "Báo cáo chuyên cần",
                "Bảng điểm học kỳ",
                "Hồ sơ học sinh",
                "Thống kê tổng hợp khối"
            });
            this.cboLoaiBaoCao.SelectedIndexChanged += new System.EventHandler(this.cboLoaiBaoCao_SelectedIndexChanged);

            // cboKhoi
            this.cboKhoi.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboKhoi.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboKhoi.Size = new System.Drawing.Size(150, 35);
            this.cboKhoi.Margin = new Padding(0, 5, 15, 5);
            this.cboKhoi.Visible = false;

            // cboLop
            this.cboLop.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboLop.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboLop.Size = new System.Drawing.Size(180, 35);
            this.cboLop.Margin = new Padding(0, 5, 15, 5);
            this.cboLop.Visible = false;

            // cboHocKy
            this.cboHocKy.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboHocKy.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboHocKy.Size = new System.Drawing.Size(130, 35);
            this.cboHocKy.Margin = new Padding(0, 5, 15, 5);
            this.cboHocKy.Items.AddRange(new object[] { "Học kỳ 1", "Học kỳ 2" });
            this.cboHocKy.Visible = false;

            // btnXuatBaoCao
            this.btnXuatBaoCao.Text = "📋 Xuất Báo Cáo";
            this.btnXuatBaoCao.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnXuatBaoCao.BackColor = System.Drawing.Color.FromArgb(34, 139, 34);
            this.btnXuatBaoCao.ForeColor = System.Drawing.Color.White;
            this.btnXuatBaoCao.FlatStyle = FlatStyle.Flat;
            this.btnXuatBaoCao.FlatAppearance.BorderSize = 0;
            this.btnXuatBaoCao.Size = new System.Drawing.Size(160, 36);
            this.btnXuatBaoCao.Margin = new Padding(0, 5, 0, 5);
            this.btnXuatBaoCao.Click += new System.EventHandler(this.btnXuatBaoCao_Click);

            this.pnlFilters.Controls.Add(this.cboLoaiBaoCao);
            this.pnlFilters.Controls.Add(this.cboKhoi);
            this.pnlFilters.Controls.Add(this.cboLop);
            this.pnlFilters.Controls.Add(this.cboHocKy);
            this.pnlFilters.Controls.Add(this.btnXuatBaoCao);

            // dgvDuLieu
            this.dgvDuLieu.Dock = DockStyle.Fill;
            this.dgvDuLieu.BackgroundColor = System.Drawing.Color.White;
            this.dgvDuLieu.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvDuLieu.ReadOnly = true;

            // Add to layout
            this.layoutRoot.Controls.Add(this.pnlHeader, 0, 0);
            this.layoutRoot.Controls.Add(this.pnlFilters, 0, 1);
            this.layoutRoot.Controls.Add(this.dgvDuLieu, 0, 2);

            // UC_BaoCao
            this.BackColor = System.Drawing.Color.FromArgb(25, 30, 45);
            this.Controls.Clear();
            this.Controls.Add(this.layoutRoot);
            this.Size = new System.Drawing.Size(940, 600);
        }
    }
}