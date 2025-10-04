namespace N6
{
    partial class UC_ThoiKhoaBieu
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dgvTKB;
        private System.Windows.Forms.Label lblWeek;
        private System.Windows.Forms.Button btnPrevWeek;
        private System.Windows.Forms.Button btnNextWeek;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.ContextMenuStrip contextMenuTKB;
        private System.Windows.Forms.ToolStripMenuItem doiMauMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem xoaGhiChuMenuItem;

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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvTKB = new System.Windows.Forms.DataGridView();
            this.contextMenuTKB = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.doiMauMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.xoaGhiChuMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lblWeek = new System.Windows.Forms.Label();
            this.btnPrevWeek = new System.Windows.Forms.Button();
            this.btnNextWeek = new System.Windows.Forms.Button();
            this.panelTop = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTKB)).BeginInit();
            this.contextMenuTKB.SuspendLayout();
            this.panelTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvTKB
            // 
            this.dgvTKB.AllowUserToAddRows = false;
            this.dgvTKB.AllowUserToDeleteRows = false;
            this.dgvTKB.AllowUserToResizeColumns = false;
            this.dgvTKB.AllowUserToResizeRows = false;
            this.dgvTKB.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvTKB.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvTKB.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTKB.ContextMenuStrip = this.contextMenuTKB;
            this.dgvTKB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTKB.Location = new System.Drawing.Point(0, 55);
            this.dgvTKB.MultiSelect = false;
            this.dgvTKB.Name = "dgvTKB";
            this.dgvTKB.ReadOnly = true;
            this.dgvTKB.RowHeadersWidth = 100;
            this.dgvTKB.RowTemplate.Height = 60;
            this.dgvTKB.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvTKB.Size = new System.Drawing.Size(900, 545);
            this.dgvTKB.TabIndex = 0;
            this.dgvTKB.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvTKB_CellDoubleClick);
            this.dgvTKB.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvTKB_CellMouseDown);
            // 
            // contextMenuTKB
            // 
            this.contextMenuTKB.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuTKB.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.doiMauMenuItem,
            this.toolStripSeparator1,
            this.xoaGhiChuMenuItem});
            this.contextMenuTKB.Name = "contextMenuTKB";
            this.contextMenuTKB.Size = new System.Drawing.Size(185, 68);
            // 
            // doiMauMenuItem
            // 
            this.doiMauMenuItem.Name = "doiMauMenuItem";
            this.doiMauMenuItem.Size = new System.Drawing.Size(184, 30);
            this.doiMauMenuItem.Text = "Đổi màu";
            this.doiMauMenuItem.Click += new System.EventHandler(this.doiMauMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(181, 6);
            // 
            // xoaGhiChuMenuItem
            // 
            this.xoaGhiChuMenuItem.Name = "xoaGhiChuMenuItem";
            this.xoaGhiChuMenuItem.Size = new System.Drawing.Size(184, 30);
            this.xoaGhiChuMenuItem.Text = "Xóa ghi chú";
            this.xoaGhiChuMenuItem.Click += new System.EventHandler(this.xoaGhiChuMenuItem_Click);
            // 
            // lblWeek
            // 
            this.lblWeek.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblWeek.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblWeek.Location = new System.Drawing.Point(250, 10);
            this.lblWeek.Name = "lblWeek";
            this.lblWeek.Size = new System.Drawing.Size(400, 35);
            this.lblWeek.TabIndex = 1;
            this.lblWeek.Text = "Thời khóa biểu: dd/MM - dd/MM/yyyy";
            this.lblWeek.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnPrevWeek
            // 
            this.btnPrevWeek.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnPrevWeek.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnPrevWeek.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnPrevWeek.Location = new System.Drawing.Point(10, 10);
            this.btnPrevWeek.Name = "btnPrevWeek";
            this.btnPrevWeek.Size = new System.Drawing.Size(90, 35);
            this.btnPrevWeek.TabIndex = 2;
            this.btnPrevWeek.Text = "← Trước";
            this.btnPrevWeek.UseVisualStyleBackColor = false;
            this.btnPrevWeek.Click += new System.EventHandler(this.btnPrevWeek_Click);
            // 
            // btnNextWeek
            // 
            this.btnNextWeek.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnNextWeek.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnNextWeek.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnNextWeek.Location = new System.Drawing.Point(800, 10);
            this.btnNextWeek.Name = "btnNextWeek";
            this.btnNextWeek.Size = new System.Drawing.Size(90, 35);
            this.btnNextWeek.TabIndex = 3;
            this.btnNextWeek.Text = "Sau →";
            this.btnNextWeek.UseVisualStyleBackColor = false;
            this.btnNextWeek.Click += new System.EventHandler(this.btnNextWeek_Click);
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.btnPrevWeek);
            this.panelTop.Controls.Add(this.btnNextWeek);
            this.panelTop.Controls.Add(this.lblWeek);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(900, 55);
            this.panelTop.TabIndex = 4;
            // 
            // UC_ThoiKhoaBieu
            // 
            this.Controls.Add(this.dgvTKB);
            this.Controls.Add(this.panelTop);
            this.Name = "UC_ThoiKhoaBieu";
            this.Size = new System.Drawing.Size(900, 600);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTKB)).EndInit();
            this.contextMenuTKB.ResumeLayout(false);
            this.panelTop.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}