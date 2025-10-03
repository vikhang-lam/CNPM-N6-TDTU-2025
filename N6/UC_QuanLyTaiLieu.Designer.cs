namespace N6
{
    partial class UC_QuanLyTaiLieu
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.FlowLayoutPanel flowDocs;
        private System.Windows.Forms.Panel panelToolbar;
        private System.Windows.Forms.Button btnUpload;
        private System.Windows.Forms.Button btnRefresh;

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
            this.panelToolbar = new System.Windows.Forms.Panel();
            this.btnUpload = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.flowDocs = new System.Windows.Forms.FlowLayoutPanel();
            this.panelToolbar.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelToolbar
            // 
            this.panelToolbar.BackColor = System.Drawing.Color.FromArgb(0, 150, 200);
            this.panelToolbar.Controls.Add(this.btnUpload);
            this.panelToolbar.Controls.Add(this.btnRefresh);
            this.panelToolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelToolbar.Location = new System.Drawing.Point(0, 0);
            this.panelToolbar.Name = "panelToolbar";
            this.panelToolbar.Size = new System.Drawing.Size(800, 50);
            this.panelToolbar.TabIndex = 0;
            // 
            // btnUpload
            // 
            this.btnUpload.FlatAppearance.BorderSize = 0;
            this.btnUpload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpload.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnUpload.ForeColor = System.Drawing.Color.White;
            this.btnUpload.Location = new System.Drawing.Point(20, 10);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(120, 30);
            this.btnUpload.TabIndex = 0;
            this.btnUpload.Text = "⬆ Tải tài liệu";
            this.btnUpload.UseVisualStyleBackColor = true;
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.FlatAppearance.BorderSize = 0;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnRefresh.ForeColor = System.Drawing.Color.White;
            this.btnRefresh.Location = new System.Drawing.Point(160, 10);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(120, 30);
            this.btnRefresh.TabIndex = 1;
            this.btnRefresh.Text = "🔄 Làm mới";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // flowDocs
            // 
            this.flowDocs.AutoScroll = true;
            this.flowDocs.BackColor = System.Drawing.Color.FromArgb(240, 248, 255);
            this.flowDocs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowDocs.Location = new System.Drawing.Point(0, 50);
            this.flowDocs.Name = "flowDocs";
            this.flowDocs.Padding = new System.Windows.Forms.Padding(15);
            this.flowDocs.Size = new System.Drawing.Size(800, 550);
            this.flowDocs.TabIndex = 1;
            // 
            // UC_QuanLyTaiLieu
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.flowDocs);
            this.Controls.Add(this.panelToolbar);
            this.Name = "UC_QuanLyTaiLieu";
            this.Size = new System.Drawing.Size(800, 600);
            this.panelToolbar.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}
