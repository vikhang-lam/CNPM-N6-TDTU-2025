namespace N6
{
    partial class dashboard
    {
        private System.ComponentModel.IContainer components = null;

       

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(dashboard));
            this.panelTopBar = new System.Windows.Forms.Panel();
            this.labelAppTitle = new System.Windows.Forms.Label();
            this.btnThemeToggle = new System.Windows.Forms.Button();
            this.labelMaximize = new System.Windows.Forms.Label();
            this.labelClose = new System.Windows.Forms.Label();
            this.labelMinimize = new System.Windows.Forms.Label();
            this.pictureBoxAppIcon = new System.Windows.Forms.PictureBox();
            this.pictureBoxUser = new System.Windows.Forms.PictureBox();
            this.labelUserName = new System.Windows.Forms.Label();
            this.labelSubject = new System.Windows.Forms.Label();
            this.panelMenu = new System.Windows.Forms.Panel();
            this.panelContent = new System.Windows.Forms.Panel();
            this.panelMain = new System.Windows.Forms.Panel();
            this.lblAssistiveToggle = new System.Windows.Forms.Label();
            this.panelTopBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAppIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxUser)).BeginInit();
            this.panelContent.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTopBar
            // 
            this.panelTopBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(200)))));
            this.panelTopBar.Controls.Add(this.labelAppTitle);
            this.panelTopBar.Controls.Add(this.btnThemeToggle);
            this.panelTopBar.Controls.Add(this.labelMaximize);
            this.panelTopBar.Controls.Add(this.labelClose);
            this.panelTopBar.Controls.Add(this.labelMinimize);
            this.panelTopBar.Controls.Add(this.pictureBoxAppIcon);
            this.panelTopBar.Controls.Add(this.pictureBoxUser);
            this.panelTopBar.Controls.Add(this.labelUserName);
            this.panelTopBar.Controls.Add(this.labelSubject);
            this.panelTopBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTopBar.Location = new System.Drawing.Point(0, 0);
            this.panelTopBar.Name = "panelTopBar";
            this.panelTopBar.Size = new System.Drawing.Size(1200, 59);
            this.panelTopBar.TabIndex = 0;
            // 
            // labelAppTitle
            // 
            this.labelAppTitle.AutoSize = true;
            this.labelAppTitle.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.labelAppTitle.ForeColor = System.Drawing.Color.White;
            this.labelAppTitle.Location = new System.Drawing.Point(60, 19);
            this.labelAppTitle.Name = "labelAppTitle";
            this.labelAppTitle.Size = new System.Drawing.Size(206, 25);
            this.labelAppTitle.TabIndex = 5;
            this.labelAppTitle.Text = "Quản Lý và Giảng Dạy";
            // 
            // btnThemeToggle
            // 
            this.btnThemeToggle.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnThemeToggle.FlatAppearance.BorderSize = 0;
            this.btnThemeToggle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnThemeToggle.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.btnThemeToggle.ForeColor = System.Drawing.Color.White;
            this.btnThemeToggle.Location = new System.Drawing.Point(950, 4);
            this.btnThemeToggle.Name = "btnThemeToggle";
            this.btnThemeToggle.Size = new System.Drawing.Size(50, 50);
            this.btnThemeToggle.TabIndex = 1;
            this.btnThemeToggle.Text = "🌙";
            this.btnThemeToggle.Click += new System.EventHandler(this.btnThemeToggle_Click);
            // 
            // labelMaximize
            // 
            this.labelMaximize.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelMaximize.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.labelMaximize.ForeColor = System.Drawing.Color.White;
            this.labelMaximize.Location = new System.Drawing.Point(1080, 4);
            this.labelMaximize.Name = "labelMaximize";
            this.labelMaximize.Size = new System.Drawing.Size(50, 50);
            this.labelMaximize.TabIndex = 2;
            this.labelMaximize.Text = "◻";
            this.labelMaximize.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelMaximize.Click += new System.EventHandler(this.labelMaximize_Click);
            // 
            // labelClose
            // 
            this.labelClose.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelClose.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.labelClose.ForeColor = System.Drawing.Color.White;
            this.labelClose.Location = new System.Drawing.Point(1130, 4);
            this.labelClose.Name = "labelClose";
            this.labelClose.Size = new System.Drawing.Size(50, 50);
            this.labelClose.TabIndex = 3;
            this.labelClose.Text = "×";
            this.labelClose.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelClose.Click += new System.EventHandler(this.labelClose_Click);
            // 
            // labelMinimize
            // 
            this.labelMinimize.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelMinimize.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.labelMinimize.ForeColor = System.Drawing.Color.White;
            this.labelMinimize.Location = new System.Drawing.Point(1030, 4);
            this.labelMinimize.Name = "labelMinimize";
            this.labelMinimize.Size = new System.Drawing.Size(50, 50);
            this.labelMinimize.TabIndex = 4;
            this.labelMinimize.Text = "–";
            this.labelMinimize.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelMinimize.Click += new System.EventHandler(this.labelMinimize_Click);
            // 
            // pictureBoxAppIcon
            // 
            this.pictureBoxAppIcon.Image = global::N6.Properties.Resources.logo;
            this.pictureBoxAppIcon.Location = new System.Drawing.Point(12, 10);
            this.pictureBoxAppIcon.Name = "pictureBoxAppIcon";
            this.pictureBoxAppIcon.Size = new System.Drawing.Size(42, 40);
            this.pictureBoxAppIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxAppIcon.TabIndex = 6;
            this.pictureBoxAppIcon.TabStop = false;
            // 
            // pictureBoxUser
            // 
            this.pictureBoxUser.BackColor = System.Drawing.Color.LightGray;
            this.pictureBoxUser.Location = new System.Drawing.Point(224, 10);
            this.pictureBoxUser.Name = "pictureBoxUser";
            this.pictureBoxUser.Size = new System.Drawing.Size(45, 43);
            this.pictureBoxUser.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxUser.TabIndex = 7;
            this.pictureBoxUser.TabStop = false;
            // 
            // labelUserName
            // 
            this.labelUserName.AutoSize = true;
            this.labelUserName.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.labelUserName.ForeColor = System.Drawing.Color.White;
            this.labelUserName.Location = new System.Drawing.Point(275, 10);
            this.labelUserName.Name = "labelUserName";
            this.labelUserName.Size = new System.Drawing.Size(108, 23);
            this.labelUserName.TabIndex = 8;
            this.labelUserName.Text = "Người dùng";
            // 
            // labelSubject
            // 
            this.labelSubject.AutoSize = true;
            this.labelSubject.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.labelSubject.ForeColor = System.Drawing.Color.White;
            this.labelSubject.Location = new System.Drawing.Point(275, 30);
            this.labelSubject.Name = "labelSubject";
            this.labelSubject.Size = new System.Drawing.Size(0, 20);
            this.labelSubject.TabIndex = 9;
            // 
            // panelMenu
            // 
            this.panelMenu.AutoScroll = true;
            this.panelMenu.BackColor = System.Drawing.Color.White;
            this.panelMenu.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelMenu.Location = new System.Drawing.Point(0, 59);
            this.panelMenu.Name = "panelMenu";
            this.panelMenu.Size = new System.Drawing.Size(200, 691);
            this.panelMenu.TabIndex = 1;
            // 
            // panelContent
            // 
            this.panelContent.Controls.Add(this.panelMain);
            this.panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContent.Location = new System.Drawing.Point(200, 59);
            this.panelContent.Name = "panelContent";
            this.panelContent.Size = new System.Drawing.Size(1000, 691);
            this.panelContent.TabIndex = 2;
            // 
            // panelMain
            // 
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(1000, 691);
            this.panelMain.TabIndex = 0;
            // 
            // lblAssistiveToggle
            // 
            this.lblAssistiveToggle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAssistiveToggle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(65)))));
            this.lblAssistiveToggle.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblAssistiveToggle.Font = new System.Drawing.Font("Segoe UI Emoji", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAssistiveToggle.ForeColor = System.Drawing.Color.White;
            this.lblAssistiveToggle.Location = new System.Drawing.Point(1140, 690);
            this.lblAssistiveToggle.Name = "lblAssistiveToggle";
            this.lblAssistiveToggle.Size = new System.Drawing.Size(50, 50);
            this.lblAssistiveToggle.TabIndex = 99;
            this.lblAssistiveToggle.Text = "✨";
            this.lblAssistiveToggle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dashboard
            // 
            this.ClientSize = new System.Drawing.Size(1200, 750);
            this.Controls.Add(this.lblAssistiveToggle);
            this.Controls.Add(this.panelContent);
            this.Controls.Add(this.panelMenu);
            this.Controls.Add(this.panelTopBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "dashboard";
            this.Text = "dashboard";
            this.Load += new System.EventHandler(this.dashboard_Load);
            this.panelTopBar.ResumeLayout(false);
            this.panelTopBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAppIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxUser)).EndInit();
            this.panelContent.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTopBar;
        private System.Windows.Forms.Button btnThemeToggle;
        private System.Windows.Forms.Label labelMaximize;
        private System.Windows.Forms.Label labelClose;
        private System.Windows.Forms.Label labelMinimize;
        private System.Windows.Forms.PictureBox pictureBoxAppIcon;
        private System.Windows.Forms.Panel panelMenu;
        private System.Windows.Forms.Panel panelContent;
        private System.Windows.Forms.Label labelSubject;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.PictureBox pictureBoxUser;
        private System.Windows.Forms.Label labelUserName;
        private System.Windows.Forms.Label lblAssistiveToggle;
        private System.Windows.Forms.Label labelAppTitle;
    }
}