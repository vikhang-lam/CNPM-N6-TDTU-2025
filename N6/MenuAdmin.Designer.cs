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
            this.components = new System.ComponentModel.Container();
            this.panelTopBar = new System.Windows.Forms.Panel();
            this.btnToggleMenu = new System.Windows.Forms.Button();
            this.btnThemeToggle = new System.Windows.Forms.Button();
            this.labelMaximize = new System.Windows.Forms.Label();
            this.labelClose = new System.Windows.Forms.Label();
            this.labelMinimize = new System.Windows.Forms.Label();
            this.panelMenu = new System.Windows.Forms.Panel();
            this.btnCollapseMenu = new System.Windows.Forms.Button();
            this.panelContent = new System.Windows.Forms.Panel();
            this.menuToggleTimer = new System.Windows.Forms.Timer(this.components);
            this.panelTopBar.SuspendLayout();
            this.panelMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTopBar
            // 
            this.panelTopBar.BackColor = System.Drawing.Color.FromArgb(179, 102, 255);
            this.panelTopBar.Controls.Add(this.btnToggleMenu);
            this.panelTopBar.Controls.Add(this.btnThemeToggle);
            this.panelTopBar.Controls.Add(this.labelMaximize);
            this.panelTopBar.Controls.Add(this.labelClose);
            this.panelTopBar.Controls.Add(this.labelMinimize);
            this.panelTopBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTopBar.Location = new System.Drawing.Point(0, 0);
            this.panelTopBar.Name = "panelTopBar";
            this.panelTopBar.Size = new System.Drawing.Size(1200, 50);
            this.panelTopBar.TabIndex = 0;
            this.panelTopBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelTopBar_MouseDown);
            // 
            // btnToggleMenu
            // 
            this.btnToggleMenu.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnToggleMenu.FlatAppearance.BorderSize = 0;
            this.btnToggleMenu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToggleMenu.Font = new System.Drawing.Font("Segoe UI", 16.2F);
            this.btnToggleMenu.ForeColor = System.Drawing.Color.White;
            this.btnToggleMenu.Location = new System.Drawing.Point(10, 0);
            this.btnToggleMenu.Name = "btnToggleMenu";
            this.btnToggleMenu.Size = new System.Drawing.Size(40, 50);
            this.btnToggleMenu.TabIndex = 6;
            this.btnToggleMenu.Text = "☰";
            this.btnToggleMenu.UseVisualStyleBackColor = true;
            this.btnToggleMenu.Click += new System.EventHandler(this.btnToggleMenu_Click);
            // 
            // btnThemeToggle
            // 
            this.btnThemeToggle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnThemeToggle.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnThemeToggle.FlatAppearance.BorderSize = 0;
            this.btnThemeToggle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnThemeToggle.Font = new System.Drawing.Font("Segoe UI", 16.2F);
            this.btnThemeToggle.ForeColor = System.Drawing.Color.White;
            this.btnThemeToggle.Location = new System.Drawing.Point(981, 0);
            this.btnThemeToggle.Name = "btnThemeToggle";
            this.btnThemeToggle.Size = new System.Drawing.Size(53, 50);
            this.btnThemeToggle.TabIndex = 5;
            this.btnThemeToggle.Text = "🌙";
            this.btnThemeToggle.UseVisualStyleBackColor = true;
            this.btnThemeToggle.Click += new System.EventHandler(this.btnThemeToggle_Click);
            // 
            // labelMaximize
            // 
            this.labelMaximize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelMaximize.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelMaximize.Font = new System.Drawing.Font("Segoe UI", 16.2F, System.Drawing.FontStyle.Bold);
            this.labelMaximize.ForeColor = System.Drawing.Color.White;
            this.labelMaximize.Location = new System.Drawing.Point(1090, 0);
            this.labelMaximize.Name = "labelMaximize";
            this.labelMaximize.Size = new System.Drawing.Size(50, 50);
            this.labelMaximize.TabIndex = 4;
            this.labelMaximize.Text = "◻";
            this.labelMaximize.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelMaximize.Click += new System.EventHandler(this.labelMaximize_Click);
            // 
            // labelClose
            // 
            this.labelClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelClose.Font = new System.Drawing.Font("Segoe UI", 16.2F, System.Drawing.FontStyle.Bold);
            this.labelClose.ForeColor = System.Drawing.Color.White;
            this.labelClose.Location = new System.Drawing.Point(1150, 0);
            this.labelClose.Name = "labelClose";
            this.labelClose.Size = new System.Drawing.Size(50, 50);
            this.labelClose.TabIndex = 3;
            this.labelClose.Text = "×";
            this.labelClose.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelClose.Click += new System.EventHandler(this.labelClose_Click);
            // 
            // labelMinimize
            // 
            this.labelMinimize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelMinimize.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelMinimize.Font = new System.Drawing.Font("Segoe UI", 16.2F, System.Drawing.FontStyle.Bold);
            this.labelMinimize.ForeColor = System.Drawing.Color.White;
            this.labelMinimize.Location = new System.Drawing.Point(1040, 0);
            this.labelMinimize.Name = "labelMinimize";
            this.labelMinimize.Size = new System.Drawing.Size(50, 50);
            this.labelMinimize.TabIndex = 2;
            this.labelMinimize.Text = "-";
            this.labelMinimize.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelMinimize.Click += new System.EventHandler(this.labelMinimize_Click);
            // 
            // panelMenu
            // 
            this.panelMenu.AutoScroll = true;
            this.panelMenu.BackColor = System.Drawing.Color.White;
            this.panelMenu.Controls.Add(this.btnCollapseMenu);
            this.panelMenu.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelMenu.Location = new System.Drawing.Point(0, 50);
            this.panelMenu.Name = "panelMenu";
            this.panelMenu.Size = new System.Drawing.Size(200, 700);
            this.panelMenu.TabIndex = 1;
            // 
            // btnCollapseMenu
            // 
            this.btnCollapseMenu.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCollapseMenu.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
            this.btnCollapseMenu.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCollapseMenu.FlatAppearance.BorderSize = 0;
            this.btnCollapseMenu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCollapseMenu.Font = new System.Drawing.Font("Segoe UI", 16.2F, System.Drawing.FontStyle.Bold);
            this.btnCollapseMenu.ForeColor = System.Drawing.Color.Gray;
            this.btnCollapseMenu.Location = new System.Drawing.Point(160, 0);
            this.btnCollapseMenu.Name = "btnCollapseMenu";
            this.btnCollapseMenu.Size = new System.Drawing.Size(40, 40);
            this.btnCollapseMenu.TabIndex = 0;
            this.btnCollapseMenu.Text = "‹";
            this.btnCollapseMenu.UseVisualStyleBackColor = false;
            this.btnCollapseMenu.Click += new System.EventHandler(this.btnCollapseMenu_Click);
            // 
            // panelContent
            // 
            this.panelContent.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContent.Location = new System.Drawing.Point(200, 50);
            this.panelContent.Name = "panelContent";
            this.panelContent.Size = new System.Drawing.Size(1000, 700);
            this.panelContent.TabIndex = 2;
            // 
            // menuToggleTimer
            // 
            this.menuToggleTimer.Tick += new System.EventHandler(this.menuToggleTimer_Tick);
            // 
            // MenuAdmin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 750);
            this.Controls.Add(this.panelContent);
            this.Controls.Add(this.panelMenu);
            this.Controls.Add(this.panelTopBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MenuAdmin";
            this.Text = "MenuAdmin";
            this.Load += new System.EventHandler(this.MenuAdmin_Load);
            this.panelTopBar.ResumeLayout(false);
            this.panelMenu.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion

        private System.Windows.Forms.Panel panelTopBar;
        private System.Windows.Forms.Button btnToggleMenu;
        private System.Windows.Forms.Button btnThemeToggle;
        private System.Windows.Forms.Label labelMaximize;
        private System.Windows.Forms.Label labelClose;
        private System.Windows.Forms.Label labelMinimize;
        private System.Windows.Forms.Panel panelMenu;
        private System.Windows.Forms.Button btnCollapseMenu;
        private System.Windows.Forms.Panel panelContent;
        private System.Windows.Forms.Timer menuToggleTimer;
        private System.Windows.Forms.Panel panelUserInfo;
    }
}
