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
        private System.Windows.Forms.Button btnImportTKB;
        // *** THÊM KHAI BÁO NÚT XÓA TKB TUẦN ***
        private System.Windows.Forms.Button btnXoaTKB;
        // *** THÊM KHAI BÁO MENU ITEM MỚI ***
        private System.Windows.Forms.ToolStripMenuItem xoaTKBMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2; // Thêm separator

        


        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvTKB = new System.Windows.Forms.DataGridView();
            this.contextMenuTKB = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.doiMauMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.xoaGhiChuMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            // *** KHỞI TẠO MENU ITEM MỚI ***
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.xoaTKBMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            // ***--------------------------***
            this.lblWeek = new System.Windows.Forms.Label();
            this.btnPrevWeek = new System.Windows.Forms.Button();
            this.btnNextWeek = new System.Windows.Forms.Button();
            this.panelTop = new System.Windows.Forms.Panel();
            this.btnImportTKB = new System.Windows.Forms.Button();
            // *** KHỞI TẠO NÚT XÓA TKB TUẦN ***
            this.btnXoaTKB = new System.Windows.Forms.Button();
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
            this.contextMenuTKB.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuTKB.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.doiMauMenuItem,
            this.toolStripSeparator1,
            this.xoaGhiChuMenuItem,
            // *** THÊM MENU ITEM MỚI VÀO ĐÂY ***
            this.toolStripSeparator2,
            this.xoaTKBMenuItem});
            this.contextMenuTKB.Name = "contextMenuTKB";
            // *** CẬP NHẬT KÍCH THƯỚC CONTEXT MENU ***
            this.contextMenuTKB.Size = new System.Drawing.Size(211, 110); // Tăng kích thước chiều cao
            //
            // doiMauMenuItem
            //
            this.doiMauMenuItem.Name = "doiMauMenuItem";
            // *** CẬP NHẬT KÍCH THƯỚC ITEM ***
            this.doiMauMenuItem.Size = new System.Drawing.Size(210, 24);
            this.doiMauMenuItem.Text = "Đổi màu nền";
            this.doiMauMenuItem.Click += new System.EventHandler(this.doiMauMenuItem_Click);
            //
            // toolStripSeparator1
            //
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            // *** CẬP NHẬT KÍCH THƯỚC ITEM ***
            this.toolStripSeparator1.Size = new System.Drawing.Size(207, 6);
            //
            // xoaGhiChuMenuItem
            //
            this.xoaGhiChuMenuItem.Name = "xoaGhiChuMenuItem";
            // *** CẬP NHẬT KÍCH THƯỚC ITEM ***
            this.xoaGhiChuMenuItem.Size = new System.Drawing.Size(210, 24);
            this.xoaGhiChuMenuItem.Text = "Xóa ghi chú";
            this.xoaGhiChuMenuItem.Click += new System.EventHandler(this.xoaGhiChuMenuItem_Click);
            //
            // toolStripSeparator2
            // *** THÊM SEPARATOR MỚI ***
            //
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(207, 6);
            //
            // xoaTKBMenuItem
            // *** THÊM MENU ITEM MỚI ***
            //
            this.xoaTKBMenuItem.ForeColor = System.Drawing.Color.Red; // Màu đỏ để cảnh báo
            this.xoaTKBMenuItem.Name = "xoaTKBMenuItem";
            this.xoaTKBMenuItem.Size = new System.Drawing.Size(210, 24);
            this.xoaTKBMenuItem.Text = "Xóa TKB tiết này";
            this.xoaTKBMenuItem.Click += new System.EventHandler(this.xoaTKBMenuItem_Click); // Sẽ thêm sự kiện này
            // ***----------------------***
            //
            // lblWeek
            //
            this.lblWeek.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblWeek.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblWeek.Location = new System.Drawing.Point(280, 10); // Điều chỉnh vị trí nếu cần
            this.lblWeek.Name = "lblWeek";
            this.lblWeek.Size = new System.Drawing.Size(340, 35); // Điều chỉnh kích thước nếu cần
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
            // *** THÊM btnXoaTKB VÀO PANEL ***
            this.panelTop.Controls.Add(this.btnXoaTKB);
            this.panelTop.Controls.Add(this.btnImportTKB);
            this.panelTop.Controls.Add(this.btnPrevWeek);
            this.panelTop.Controls.Add(this.btnNextWeek);
            this.panelTop.Controls.Add(this.lblWeek);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(900, 55);
            this.panelTop.TabIndex = 4;
            //
            // btnImportTKB
            //
            this.btnImportTKB.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnImportTKB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.btnImportTKB.FlatAppearance.BorderSize = 0;
            this.btnImportTKB.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImportTKB.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnImportTKB.ForeColor = System.Drawing.Color.White;
            this.btnImportTKB.Location = new System.Drawing.Point(110, 10); // Điều chỉnh vị trí nếu cần
            this.btnImportTKB.Name = "btnImportTKB";
            this.btnImportTKB.Size = new System.Drawing.Size(120, 35);
            this.btnImportTKB.TabIndex = 4;
            this.btnImportTKB.Text = "📥 Import";
            this.btnImportTKB.UseVisualStyleBackColor = false;
            this.btnImportTKB.Click += new System.EventHandler(this.btnImportTKB_Click);
            //
            // btnXoaTKB
            // *** THÊM THUỘC TÍNH CHO NÚT MỚI ***
            //
            this.btnXoaTKB.Anchor = System.Windows.Forms.AnchorStyles.Right; // Đặt bên phải
            this.btnXoaTKB.BackColor = System.Drawing.Color.IndianRed; // Màu đỏ cảnh báo
            this.btnXoaTKB.FlatAppearance.BorderSize = 0;
            this.btnXoaTKB.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnXoaTKB.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnXoaTKB.ForeColor = System.Drawing.Color.White;
            // Điều chỉnh vị trí Left để không chồng lên nút Next
            this.btnXoaTKB.Location = new System.Drawing.Point(640, 10);
            this.btnXoaTKB.Name = "btnXoaTKB";
            this.btnXoaTKB.Size = new System.Drawing.Size(150, 35); // Tăng chiều rộng
            this.btnXoaTKB.TabIndex = 5; // Tăng TabIndex
            this.btnXoaTKB.Text = "🗑️ Xóa TKB Tuần";
            this.btnXoaTKB.UseVisualStyleBackColor = false;
            // Sự kiện Click sẽ được thêm trong file .cs
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