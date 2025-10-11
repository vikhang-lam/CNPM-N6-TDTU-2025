namespace N6
{
    partial class UC_QuanLyTaiLieu
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel panelToolbar;
        private System.Windows.Forms.Button btnUpload;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.TabControl tabDocs;
        private System.Windows.Forms.TabPage tabMyDocs;
        private System.Windows.Forms.TabPage tabSharedDocs;
        private System.Windows.Forms.FlowLayoutPanel flowMyDocs;
        private System.Windows.Forms.FlowLayoutPanel flowSharedDocs;

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
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnUpload = new System.Windows.Forms.Button();
            this.tabDocs = new System.Windows.Forms.TabControl();
            this.tabMyDocs = new System.Windows.Forms.TabPage();
            this.flowMyDocs = new System.Windows.Forms.FlowLayoutPanel();
            this.tabSharedDocs = new System.Windows.Forms.TabPage();
            this.flowSharedDocs = new System.Windows.Forms.FlowLayoutPanel();
            this.panelToolbar.SuspendLayout();
            this.tabDocs.SuspendLayout();
            this.tabMyDocs.SuspendLayout();
            this.tabSharedDocs.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelToolbar
            // 
            this.panelToolbar.BackColor = System.Drawing.Color.White;
            this.panelToolbar.Controls.Add(this.btnRefresh);
            this.panelToolbar.Controls.Add(this.btnUpload);
            this.panelToolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelToolbar.Location = new System.Drawing.Point(0, 0);
            this.panelToolbar.Name = "panelToolbar";
            this.panelToolbar.Size = new System.Drawing.Size(800, 50);
            this.panelToolbar.TabIndex = 0;
            this.panelToolbar.Paint += new System.Windows.Forms.PaintEventHandler(this.panelToolbar_Paint);
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.Color.White;
            this.btnRefresh.FlatAppearance.BorderColor = System.Drawing.Color.Gainsboro;
            this.btnRefresh.FlatAppearance.BorderSize = 0;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRefresh.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnRefresh.Location = new System.Drawing.Point(150, 8);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(120, 35);
            this.btnRefresh.TabIndex = 1;
            this.btnRefresh.Text = "🔄 Làm mới";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnUpload
            // 
            this.btnUpload.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(119)))), ((int)(((byte)(242)))));
            this.btnUpload.FlatAppearance.BorderSize = 0;
            this.btnUpload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpload.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUpload.ForeColor = System.Drawing.Color.White;
            this.btnUpload.Location = new System.Drawing.Point(15, 8);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(120, 35);
            this.btnUpload.TabIndex = 0;
            this.btnUpload.Text = "⬆ Tải lên";
            this.btnUpload.UseVisualStyleBackColor = false;
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // tabDocs
            // 
            this.tabDocs.Controls.Add(this.tabMyDocs);
            this.tabDocs.Controls.Add(this.tabSharedDocs);
            this.tabDocs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabDocs.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabDocs.Location = new System.Drawing.Point(0, 50);
            this.tabDocs.Name = "tabDocs";
            this.tabDocs.Padding = new System.Drawing.Point(10, 5);
            this.tabDocs.SelectedIndex = 0;
            this.tabDocs.Size = new System.Drawing.Size(800, 550);
            this.tabDocs.TabIndex = 1;
            // 
            // tabMyDocs
            // 
            this.tabMyDocs.Controls.Add(this.flowMyDocs);
            this.tabMyDocs.Location = new System.Drawing.Point(4, 30);
            this.tabMyDocs.Name = "tabMyDocs";
            this.tabMyDocs.Padding = new System.Windows.Forms.Padding(3);
            this.tabMyDocs.Size = new System.Drawing.Size(792, 516);
            this.tabMyDocs.TabIndex = 0;
            this.tabMyDocs.Text = "📂 Tài liệu của tôi";
            this.tabMyDocs.UseVisualStyleBackColor = true;
            // 
            // flowMyDocs
            // 
            this.flowMyDocs.AutoScroll = true;
            this.flowMyDocs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.flowMyDocs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowMyDocs.Location = new System.Drawing.Point(3, 3);
            this.flowMyDocs.Name = "flowMyDocs";
            this.flowMyDocs.Padding = new System.Windows.Forms.Padding(15);
            this.flowMyDocs.Size = new System.Drawing.Size(786, 510);
            this.flowMyDocs.TabIndex = 0;
            // 
            // tabSharedDocs
            // 
            this.tabSharedDocs.Controls.Add(this.flowSharedDocs);
            this.tabSharedDocs.Location = new System.Drawing.Point(4, 30);
            this.tabSharedDocs.Name = "tabSharedDocs";
            this.tabSharedDocs.Padding = new System.Windows.Forms.Padding(3);
            this.tabSharedDocs.Size = new System.Drawing.Size(792, 516);
            this.tabSharedDocs.TabIndex = 1;
            this.tabSharedDocs.Text = "🤝 Được chia sẻ";
            this.tabSharedDocs.UseVisualStyleBackColor = true;
            // 
            // flowSharedDocs
            // 
            this.flowSharedDocs.AutoScroll = true;
            this.flowSharedDocs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.flowSharedDocs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowSharedDocs.Location = new System.Drawing.Point(3, 3);
            this.flowSharedDocs.Name = "flowSharedDocs";
            this.flowSharedDocs.Padding = new System.Windows.Forms.Padding(15);
            this.flowSharedDocs.Size = new System.Drawing.Size(786, 510);
            this.flowSharedDocs.TabIndex = 0;
            // 
            // UC_QuanLyTaiLieu
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.tabDocs);
            this.Controls.Add(this.panelToolbar);
            this.Name = "UC_QuanLyTaiLieu";
            this.Size = new System.Drawing.Size(800, 600);
            this.panelToolbar.ResumeLayout(false);
            this.tabDocs.ResumeLayout(false);
            this.tabMyDocs.ResumeLayout(false);
            this.tabSharedDocs.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}