namespace N6
{
    partial class UC_HoTroGiangDay
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.Button btnOpenPopupWhiteboard;
        private System.Windows.Forms.TextBox txtNotes;
        private System.Windows.Forms.Label lblTitle;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
                DisposeResources();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.mainPanel = new System.Windows.Forms.Panel();
            this.btnOpenPopupWhiteboard = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.mainPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.mainPanel.Controls.Add(this.txtNotes);
            this.mainPanel.Controls.Add(this.btnOpenPopupWhiteboard);
            this.mainPanel.Controls.Add(this.lblTitle);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Padding = new System.Windows.Forms.Padding(20);
            this.mainPanel.Size = new System.Drawing.Size(900, 600);
            this.mainPanel.TabIndex = 0;
            // 
            // btnOpenPopupWhiteboard
            // 
            this.btnOpenPopupWhiteboard.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenPopupWhiteboard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(255)))));
            this.btnOpenPopupWhiteboard.FlatAppearance.BorderSize = 0;
            this.btnOpenPopupWhiteboard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenPopupWhiteboard.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.btnOpenPopupWhiteboard.ForeColor = System.Drawing.Color.White;
            this.btnOpenPopupWhiteboard.Location = new System.Drawing.Point(20, 80);
            this.btnOpenPopupWhiteboard.Name = "btnOpenPopupWhiteboard";
            this.btnOpenPopupWhiteboard.Size = new System.Drawing.Size(860, 80);
            this.btnOpenPopupWhiteboard.TabIndex = 1;
            this.btnOpenPopupWhiteboard.Text = "🖊️ Mở Bảng Trắng Popup";
            this.btnOpenPopupWhiteboard.UseVisualStyleBackColor = false;
            this.btnOpenPopupWhiteboard.Click += new System.EventHandler(this.btnOpenPopupWhiteboard_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblTitle.Location = new System.Drawing.Point(20, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(205, 41);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Hỗ trợ giảng dạy";
            // 
            // txtNotes
            // 
            this.txtNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNotes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNotes.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtNotes.Location = new System.Drawing.Point(20, 200);
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtNotes.Size = new System.Drawing.Size(860, 380);
            this.txtNotes.TabIndex = 2;
            this.txtNotes.Text = "📝 Ghi chú nhanh...\r\n\r\nSử dụng khu vực này để ghi chú trong quá trình giảng dạy." +
    " Nội dung sẽ được tự động lưu.";
            // 
            // UC_HoTroGiangDay
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainPanel);
            this.Name = "UC_HoTroGiangDay";
            this.Size = new System.Drawing.Size(900, 600);
            this.Load += new System.EventHandler(this.UC_HoTroGiangDay_Load);
            this.mainPanel.ResumeLayout(false);
            this.mainPanel.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}