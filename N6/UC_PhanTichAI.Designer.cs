namespace N6
{
    partial class UC_PhanTichAI
    {
        private System.ComponentModel.IContainer components = null;

        // SỬA ĐỔI: panelFilters giờ là FlowLayoutPanel
        private System.Windows.Forms.FlowLayoutPanel panelFilters;
        private System.Windows.Forms.Button btnPhanTich;
        private System.Windows.Forms.ComboBox cbScope;
        private System.Windows.Forms.ComboBox cbDetail;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelCards;
        private ScottPlot.WinForms.FormsPlot formsPlot1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;

        private System.Windows.Forms.Label lblMonHoc;
        private System.Windows.Forms.ComboBox cbMonHoc;
        private System.Windows.Forms.Label lblHocKy;
        private System.Windows.Forms.ComboBox cbHocKy;


        private void InitializeComponent()
        {
            // SỬA ĐỔI: Khai báo panelFilters là FlowLayoutPanel
            this.panelFilters = new System.Windows.Forms.FlowLayoutPanel();
            this.lblHocKy = new System.Windows.Forms.Label();
            this.cbHocKy = new System.Windows.Forms.ComboBox();
            this.lblMonHoc = new System.Windows.Forms.Label();
            this.cbMonHoc = new System.Windows.Forms.ComboBox();
            this.cbDetail = new System.Windows.Forms.ComboBox();
            this.btnPhanTich = new System.Windows.Forms.Button();
            this.cbScope = new System.Windows.Forms.ComboBox();
            this.flowLayoutPanelCards = new System.Windows.Forms.FlowLayoutPanel();
            this.formsPlot1 = new ScottPlot.WinForms.FormsPlot();
            this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.panelFilters.SuspendLayout();
            this.tableLayoutPanelMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelFilters
            // 
            this.panelFilters.BackColor = System.Drawing.Color.White;
            this.panelFilters.Controls.Add(this.cbScope);
            this.panelFilters.Controls.Add(this.cbDetail);
            this.panelFilters.Controls.Add(this.lblMonHoc);
            this.panelFilters.Controls.Add(this.cbMonHoc);
            this.panelFilters.Controls.Add(this.lblHocKy);
            this.panelFilters.Controls.Add(this.cbHocKy);
            this.panelFilters.Controls.Add(this.btnPhanTich);
            this.panelFilters.Dock = System.Windows.Forms.DockStyle.Fill;
            // SỬA ĐỔI: Các thuộc tính quan trọng của FlowLayoutPanel
            this.panelFilters.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.panelFilters.Location = new System.Drawing.Point(3, 3);
            this.panelFilters.Name = "panelFilters";
            this.panelFilters.Padding = new System.Windows.Forms.Padding(10, 20, 10, 10);
            this.panelFilters.Size = new System.Drawing.Size(994, 74);
            this.panelFilters.TabIndex = 0;
            this.panelFilters.WrapContents = false;
            // 
            // cbScope
            // 
            this.cbScope.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbScope.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.cbScope.FormattingEnabled = true;
            // XÓA: Location
            this.cbScope.Margin = new System.Windows.Forms.Padding(3, 3, 15, 3);
            this.cbScope.Name = "cbScope";
            this.cbScope.Size = new System.Drawing.Size(145, 28);
            this.cbScope.TabIndex = 1;
            // 
            // cbDetail
            // 
            this.cbDetail.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDetail.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.cbDetail.FormattingEnabled = true;
            // XÓA: Location
            this.cbDetail.Margin = new System.Windows.Forms.Padding(3, 3, 25, 3);
            this.cbDetail.Name = "cbDetail";
            this.cbDetail.Size = new System.Drawing.Size(145, 28);
            this.cbDetail.TabIndex = 3;
            // 
            // lblMonHoc
            // 
            this.lblMonHoc.AutoSize = true;
            this.lblMonHoc.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            // XÓA: Location
            this.lblMonHoc.Margin = new System.Windows.Forms.Padding(3, 8, 3, 3); // Chỉnh margin để căn giữa theo chiều dọc
            this.lblMonHoc.Name = "lblMonHoc";
            this.lblMonHoc.Size = new System.Drawing.Size(63, 17);
            this.lblMonHoc.TabIndex = 5;
            this.lblMonHoc.Text = "Môn học:";
            // 
            // cbMonHoc
            // 
            this.cbMonHoc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMonHoc.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.cbMonHoc.FormattingEnabled = true;
            // XÓA: Location
            this.cbMonHoc.Margin = new System.Windows.Forms.Padding(3, 3, 25, 3);
            this.cbMonHoc.Name = "cbMonHoc";
            this.cbMonHoc.Size = new System.Drawing.Size(150, 28);
            this.cbMonHoc.TabIndex = 4;
            // 
            // lblHocKy
            // 
            this.lblHocKy.AutoSize = true;
            this.lblHocKy.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            // XÓA: Location
            this.lblHocKy.Margin = new System.Windows.Forms.Padding(3, 8, 3, 3); // Chỉnh margin
            this.lblHocKy.Name = "lblHocKy";
            this.lblHocKy.Size = new System.Drawing.Size(56, 17);
            this.lblHocKy.TabIndex = 7;
            this.lblHocKy.Text = "Học kỳ:";
            // 
            // cbHocKy
            // 
            this.cbHocKy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbHocKy.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.cbHocKy.FormattingEnabled = true;
            // XÓA: Location
            this.cbHocKy.Margin = new System.Windows.Forms.Padding(3, 3, 25, 3);
            this.cbHocKy.Name = "cbHocKy";
            this.cbHocKy.Size = new System.Drawing.Size(130, 28);
            this.cbHocKy.TabIndex = 6;
            // 
            // btnPhanTich
            // 
            this.btnPhanTich.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnPhanTich.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPhanTich.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnPhanTich.ForeColor = System.Drawing.Color.White;
            // XÓA: Location
            this.btnPhanTich.Margin = new System.Windows.Forms.Padding(10, 0, 3, 3);
            this.btnPhanTich.Name = "btnPhanTich";
            this.btnPhanTich.Size = new System.Drawing.Size(140, 38);
            this.btnPhanTich.TabIndex = 2;
            this.btnPhanTich.Text = "🚀 Phân tích";
            this.btnPhanTich.UseVisualStyleBackColor = false;
            // 
            // flowLayoutPanelCards, formsPlot1, tableLayoutPanelMain (Giữ nguyên)
            // ...
            this.flowLayoutPanelCards.AutoScroll = true;
            this.flowLayoutPanelCards.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanelCards.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanelCards.Location = new System.Drawing.Point(653, 83);
            this.flowLayoutPanelCards.Name = "flowLayoutPanelCards";
            this.flowLayoutPanelCards.Size = new System.Drawing.Size(344, 514);
            this.flowLayoutPanelCards.TabIndex = 2;
            this.flowLayoutPanelCards.WrapContents = false;
            // 
            // formsPlot1
            // 
            this.formsPlot1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.formsPlot1.Location = new System.Drawing.Point(3, 83);
            this.formsPlot1.Name = "formsPlot1";
            this.formsPlot1.Size = new System.Drawing.Size(644, 514);
            this.formsPlot1.TabIndex = 1;
            // 
            // tableLayoutPanelMain
            // 
            this.tableLayoutPanelMain.ColumnCount = 2;
            // Cột 1 (Biểu đồ) chiếm 65%
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 65F));
            // Cột 2 (Thẻ) chiếm 35%
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.tableLayoutPanelMain.Controls.Add(this.panelFilters, 0, 0);
            this.tableLayoutPanelMain.Controls.Add(this.formsPlot1, 0, 1);
            this.tableLayoutPanelMain.Controls.Add(this.flowLayoutPanelCards, 1, 1);
            this.tableLayoutPanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelMain.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            this.tableLayoutPanelMain.RowCount = 2;
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelMain.SetColumnSpan(this.panelFilters, 2);
            this.tableLayoutPanelMain.Size = new System.Drawing.Size(1000, 600);
            this.tableLayoutPanelMain.TabIndex = 3;
            // 
            // UC_PhanTichAI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(245)))), ((int)(((byte)(250)))));
            this.Controls.Add(this.tableLayoutPanelMain);
            this.Name = "UC_PhanTichAI";
            this.Size = new System.Drawing.Size(1000, 600);
            this.panelFilters.ResumeLayout(false);
            this.panelFilters.PerformLayout();
            this.tableLayoutPanelMain.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}