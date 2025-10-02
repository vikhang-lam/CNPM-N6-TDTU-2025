namespace N6
{
    partial class MenuAdmin
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.panelTopBar = new System.Windows.Forms.Panel();
            this.avatarAdmin = new System.Windows.Forms.PictureBox();
            this.lblAdminName = new System.Windows.Forms.Label();
            this.btnToggleMenu = new System.Windows.Forms.Button();
            this.btnThemeToggle = new System.Windows.Forms.Button();
            this.labelMaximize = new System.Windows.Forms.Label();
            this.labelClose = new System.Windows.Forms.Label();
            this.labelMinimize = new System.Windows.Forms.Label();
            this.labelAppTitle = new System.Windows.Forms.Label();
            this.pictureBoxAppIcon = new System.Windows.Forms.PictureBox();
            this.panelMenu = new System.Windows.Forms.Panel();
            this.panelContent = new System.Windows.Forms.Panel();
            this.panelTopBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.avatarAdmin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAppIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // panelTopBar
            // 
            this.panelTopBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(200)))));
            this.panelTopBar.Controls.Add(this.avatarAdmin);
            this.panelTopBar.Controls.Add(this.lblAdminName);
            this.panelTopBar.Controls.Add(this.btnToggleMenu);
            this.panelTopBar.Controls.Add(this.btnThemeToggle);
            this.panelTopBar.Controls.Add(this.labelMaximize);
            this.panelTopBar.Controls.Add(this.labelClose);
            this.panelTopBar.Controls.Add(this.labelMinimize);
            this.panelTopBar.Controls.Add(this.labelAppTitle);
            this.panelTopBar.Controls.Add(this.pictureBoxAppIcon);
            this.panelTopBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTopBar.Location = new System.Drawing.Point(0, 0);
            this.panelTopBar.Name = "panelTopBar";
            this.panelTopBar.Size = new System.Drawing.Size(1200, 50);
            this.panelTopBar.TabIndex = 0;
            // 
            // avatarAdmin
            // 
            this.avatarAdmin.Image = global::N6.Properties.Resources.user_avatar;
            this.avatarAdmin.Location = new System.Drawing.Point(180, 5);
            this.avatarAdmin.Name = "avatarAdmin";
            this.avatarAdmin.Size = new System.Drawing.Size(40, 40);
            this.avatarAdmin.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.avatarAdmin.TabIndex = 0;
            this.avatarAdmin.TabStop = false;
            // 
            // lblAdminName
            // 
            this.lblAdminName.AutoSize = true;
            this.lblAdminName.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblAdminName.ForeColor = System.Drawing.Color.White;
            this.lblAdminName.Location = new System.Drawing.Point(230, 15);
            this.lblAdminName.Name = "lblAdminName";
            this.lblAdminName.Size = new System.Drawing.Size(71, 25);
            this.lblAdminName.TabIndex = 1;
            this.lblAdminName.Text = "Admin";
            // 
            // btnToggleMenu
            // 
            this.btnToggleMenu.FlatAppearance.BorderSize = 0;
            this.btnToggleMenu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToggleMenu.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.btnToggleMenu.ForeColor = System.Drawing.Color.White;
            this.btnToggleMenu.Location = new System.Drawing.Point(10, 0);
            this.btnToggleMenu.Name = "btnToggleMenu";
            this.btnToggleMenu.Size = new System.Drawing.Size(40, 50);
            this.btnToggleMenu.TabIndex = 2;
            this.btnToggleMenu.Text = "☰";
            this.btnToggleMenu.Click += new System.EventHandler(this.btnToggleMenu_Click);
            // 
            // btnThemeToggle
            // 
            this.btnThemeToggle.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnThemeToggle.FlatAppearance.BorderSize = 0;
            this.btnThemeToggle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnThemeToggle.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.btnThemeToggle.ForeColor = System.Drawing.Color.White;
            this.btnThemeToggle.Location = new System.Drawing.Point(950, 0);
            this.btnThemeToggle.Name = "btnThemeToggle";
            this.btnThemeToggle.Size = new System.Drawing.Size(50, 50);
            this.btnThemeToggle.TabIndex = 3;
            this.btnThemeToggle.Text = "🌙";
            this.btnThemeToggle.Click += new System.EventHandler(this.btnThemeToggle_Click);
            // 
            // labelMaximize
            // 
            this.labelMaximize.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelMaximize.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.labelMaximize.ForeColor = System.Drawing.Color.White;
            this.labelMaximize.Location = new System.Drawing.Point(1080, 0);
            this.labelMaximize.Name = "labelMaximize";
            this.labelMaximize.Size = new System.Drawing.Size(50, 50);
            this.labelMaximize.TabIndex = 4;
            this.labelMaximize.Text = "◻";
            this.labelMaximize.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelMaximize.Click += new System.EventHandler(this.labelMaximize_Click);
            // 
            // labelClose
            // 
            this.labelClose.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelClose.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.labelClose.ForeColor = System.Drawing.Color.White;
            this.labelClose.Location = new System.Drawing.Point(1130, 0);
            this.labelClose.Name = "labelClose";
            this.labelClose.Size = new System.Drawing.Size(50, 50);
            this.labelClose.TabIndex = 5;
            this.labelClose.Text = "×";
            this.labelClose.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelClose.Click += new System.EventHandler(this.labelClose_Click);
            // 
            // labelMinimize
            // 
            this.labelMinimize.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelMinimize.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.labelMinimize.ForeColor = System.Drawing.Color.White;
            this.labelMinimize.Location = new System.Drawing.Point(1030, 0);
            this.labelMinimize.Name = "labelMinimize";
            this.labelMinimize.Size = new System.Drawing.Size(50, 50);
            this.labelMinimize.TabIndex = 6;
            this.labelMinimize.Text = "–";
            this.labelMinimize.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelMinimize.Click += new System.EventHandler(this.labelMinimize_Click);
            // 
            // labelAppTitle
            // 
            this.labelAppTitle.AutoSize = true;
            this.labelAppTitle.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.labelAppTitle.ForeColor = System.Drawing.Color.White;
            this.labelAppTitle.Location = new System.Drawing.Point(60, 15);
            this.labelAppTitle.Name = "labelAppTitle";
            this.labelAppTitle.Size = new System.Drawing.Size(125, 25);
            this.labelAppTitle.TabIndex = 7;
            this.labelAppTitle.Text = "Admin Panel";
            // 
            // pictureBoxAppIcon
            // 
            this.pictureBoxAppIcon.Location = new System.Drawing.Point(30, 10);
            this.pictureBoxAppIcon.Name = "pictureBoxAppIcon";
            this.pictureBoxAppIcon.Size = new System.Drawing.Size(30, 30);
            this.pictureBoxAppIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxAppIcon.TabIndex = 8;
            this.pictureBoxAppIcon.TabStop = false;
            // 
            // panelMenu
            // 
            this.panelMenu.AutoScroll = true;
            this.panelMenu.BackColor = System.Drawing.Color.White;
            this.panelMenu.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelMenu.Location = new System.Drawing.Point(0, 50);
            this.panelMenu.Name = "panelMenu";
            this.panelMenu.Size = new System.Drawing.Size(200, 700);
            this.panelMenu.TabIndex = 1;
            // 
            // panelContent
            // 
            this.panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContent.Location = new System.Drawing.Point(200, 50);
            this.panelContent.Name = "panelContent";
            this.panelContent.Size = new System.Drawing.Size(1000, 700);
            this.panelContent.TabIndex = 2;
            // 
            // MenuAdmin
            // 
            this.ClientSize = new System.Drawing.Size(1200, 750);
            this.Controls.Add(this.panelContent);
            this.Controls.Add(this.panelMenu);
            this.Controls.Add(this.panelTopBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MenuAdmin";
            this.Text = "MenuAdmin";
            this.Load += new System.EventHandler(this.MenuAdmin_Load);
            this.panelTopBar.ResumeLayout(false);
            this.panelTopBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.avatarAdmin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAppIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTopBar;
        private System.Windows.Forms.Button btnToggleMenu;
        private System.Windows.Forms.Button btnThemeToggle;
        private System.Windows.Forms.Label labelMaximize;
        private System.Windows.Forms.Label labelClose;
        private System.Windows.Forms.Label labelMinimize;
        private System.Windows.Forms.Label labelAppTitle;
        private System.Windows.Forms.PictureBox pictureBoxAppIcon;
        private System.Windows.Forms.PictureBox avatarAdmin;
        private System.Windows.Forms.Label lblAdminName;
        private System.Windows.Forms.Panel panelMenu;
        private System.Windows.Forms.Panel panelContent;
    }
}
