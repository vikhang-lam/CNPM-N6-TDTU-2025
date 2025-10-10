namespace N6
{
    partial class UC_PhanTichAI
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel panelFilters;
        private System.Windows.Forms.Button btnPhanTich;
        private System.Windows.Forms.ComboBox cbScope;
        private System.Windows.Forms.Label lblScope;
        private System.Windows.Forms.ComboBox cbDetail;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelCards;
        // ====> THAY THẾ CartesianChart BẰNG FormsPlot <====
        private ScottPlot.WinForms.FormsPlot formsPlot1;

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
            this.panelFilters = new System.Windows.Forms.Panel();
            this.cbDetail = new System.Windows.Forms.ComboBox();
            this.btnPhanTich = new System.Windows.Forms.Button();
            this.cbScope = new System.Windows.Forms.ComboBox();
            this.lblScope = new System.Windows.Forms.Label();
            this.flowLayoutPanelCards = new System.Windows.Forms.FlowLayoutPanel();
            // ====> KHỞI TẠO FormsPlot <====
            this.formsPlot1 = new ScottPlot.WinForms.FormsPlot();
            this.panelFilters.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelFilters
            // 
            this.panelFilters.BackColor = System.Drawing.Color.White;
            this.panelFilters.Controls.Add(this.cbDetail);
            this.panelFilters.Controls.Add(this.btnPhanTich);
            this.panelFilters.Controls.Add(this.cbScope);
            this.panelFilters.Controls.Add(this.lblScope);
            this.panelFilters.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelFilters.Location = new System.Drawing.Point(10, 10);
            this.panelFilters.Name = "panelFilters";
            this.panelFilters.Size = new System.Drawing.Size(980, 60);
            this.panelFilters.TabIndex = 0;
            // 
            // cbDetail, btnPhanTich, cbScope, lblScope (giữ nguyên)
            // ...
            // 
            // flowLayoutPanelCards
            // 
            this.flowLayoutPanelCards.AutoScroll = true;
            this.flowLayoutPanelCards.Dock = System.Windows.Forms.DockStyle.Right;
            this.flowLayoutPanelCards.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanelCards.Location = new System.Drawing.Point(690, 70);
            this.flowLayoutPanelCards.Name = "flowLayoutPanelCards";
            this.flowLayoutPanelCards.Size = new System.Drawing.Size(300, 520);
            this.flowLayoutPanelCards.TabIndex = 2;
            this.flowLayoutPanelCards.WrapContents = false;
            // 
            // formsPlot1
            // 
            this.formsPlot1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.formsPlot1.Location = new System.Drawing.Point(10, 70);
            this.formsPlot1.Name = "formsPlot1";
            this.formsPlot1.Size = new System.Drawing.Size(680, 520);
            this.formsPlot1.TabIndex = 3;
            // 
            // UC_PhanTichAI
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(245)))), ((int)(((byte)(250)))));
            this.Controls.Add(this.formsPlot1);
            this.Controls.Add(this.flowLayoutPanelCards);
            this.Controls.Add(this.panelFilters);
            this.Name = "UC_PhanTichAI";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Size = new System.Drawing.Size(1000, 600);
            this.panelFilters.ResumeLayout(false);
            this.panelFilters.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}