namespace N6
{
    partial class login
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelTopBar = new System.Windows.Forms.Panel();
            this.labelMaximize = new System.Windows.Forms.Label();
            this.labelClose = new System.Windows.Forms.Label();
            this.labelMinimize = new System.Windows.Forms.Label();
            this.labelAppTitle = new System.Windows.Forms.Label();
            this.pictureBoxAppIcon = new System.Windows.Forms.PictureBox();
            this.labelGreeting = new System.Windows.Forms.Label();
            this.labelInstruction = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.paneluser1 = new System.Windows.Forms.Panel();
            this.paneluser2 = new System.Windows.Forms.Panel();
            this.paneluser3 = new System.Windows.Forms.Panel();
            this.paneluser4 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panelTopBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAppIcon)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTopBar
            // 
            this.panelTopBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(179)))), ((int)(((byte)(102)))), ((int)(((byte)(255)))));
            this.panelTopBar.Controls.Add(this.labelMaximize);
            this.panelTopBar.Controls.Add(this.labelClose);
            this.panelTopBar.Controls.Add(this.labelMinimize);
            this.panelTopBar.Controls.Add(this.labelAppTitle);
            this.panelTopBar.Controls.Add(this.pictureBoxAppIcon);
            this.panelTopBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTopBar.Location = new System.Drawing.Point(0, 0);
            this.panelTopBar.Name = "panelTopBar";
            this.panelTopBar.Size = new System.Drawing.Size(1407, 50);
            this.panelTopBar.TabIndex = 0;
            this.panelTopBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelTopBar_MouseDown);
            // 
            // labelMaximize
            // 
            this.labelMaximize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelMaximize.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelMaximize.Font = new System.Drawing.Font("Arial", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMaximize.ForeColor = System.Drawing.Color.White;
            this.labelMaximize.Location = new System.Drawing.Point(1297, 0);
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
            this.labelClose.Font = new System.Drawing.Font("Arial", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelClose.ForeColor = System.Drawing.Color.White;
            this.labelClose.Location = new System.Drawing.Point(1357, 0);
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
            this.labelMinimize.Font = new System.Drawing.Font("Arial", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMinimize.ForeColor = System.Drawing.Color.White;
            this.labelMinimize.Location = new System.Drawing.Point(1247, 0);
            this.labelMinimize.Name = "labelMinimize";
            this.labelMinimize.Size = new System.Drawing.Size(50, 50);
            this.labelMinimize.TabIndex = 2;
            this.labelMinimize.Text = "-";
            this.labelMinimize.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelMinimize.Click += new System.EventHandler(this.labelMinimize_Click);
            // 
            // labelAppTitle
            // 
            this.labelAppTitle.AutoSize = true;
            this.labelAppTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAppTitle.ForeColor = System.Drawing.Color.White;
            this.labelAppTitle.Location = new System.Drawing.Point(50, 15);
            this.labelAppTitle.Name = "labelAppTitle";
            this.labelAppTitle.Size = new System.Drawing.Size(108, 23);
            this.labelAppTitle.TabIndex = 1;
            this.labelAppTitle.Text = "EduManager";
            // 
            // pictureBoxAppIcon
            // 
            this.pictureBoxAppIcon.Location = new System.Drawing.Point(10, 10);
            this.pictureBoxAppIcon.Name = "pictureBoxAppIcon";
            this.pictureBoxAppIcon.Size = new System.Drawing.Size(30, 30);
            this.pictureBoxAppIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxAppIcon.TabIndex = 0;
            this.pictureBoxAppIcon.TabStop = false;
            // 
            // labelGreeting
            // 
            this.labelGreeting.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelGreeting.BackColor = System.Drawing.Color.Transparent;
            this.labelGreeting.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelGreeting.ForeColor = System.Drawing.Color.White;
            this.labelGreeting.Location = new System.Drawing.Point(0, 80);
            this.labelGreeting.Name = "labelGreeting";
            this.labelGreeting.Size = new System.Drawing.Size(1407, 50);
            this.labelGreeting.TabIndex = 2;
            this.labelGreeting.Text = "Chào mừng trở lại!";
            this.labelGreeting.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelInstruction
            // 
            this.labelInstruction.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelInstruction.BackColor = System.Drawing.Color.Transparent;
            this.labelInstruction.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelInstruction.ForeColor = System.Drawing.Color.White;
            this.labelInstruction.Location = new System.Drawing.Point(0, 140);
            this.labelInstruction.Name = "labelInstruction";
            this.labelInstruction.Size = new System.Drawing.Size(1407, 30);
            this.labelInstruction.TabIndex = 3;
            this.labelInstruction.Text = "Chọn tài khoản giáo viên để tiếp tục";
            this.labelInstruction.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.paneluser1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.paneluser2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.paneluser3, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.paneluser4, 3, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(142, 200);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(10);
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1111, 400);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // paneluser1
            // 
            this.paneluser1.BackColor = System.Drawing.Color.White;
            this.paneluser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.paneluser1.Location = new System.Drawing.Point(25, 25);
            this.paneluser1.Margin = new System.Windows.Forms.Padding(15);
            this.paneluser1.Name = "paneluser1";
            this.paneluser1.Size = new System.Drawing.Size(242, 350);
            this.paneluser1.TabIndex = 0;
            this.paneluser1.MouseEnter += new System.EventHandler(this.paneluser_MouseEnter);
            this.paneluser1.MouseLeave += new System.EventHandler(this.paneluser_MouseLeave);
            // 
            // paneluser2
            // 
            this.paneluser2.BackColor = System.Drawing.Color.White;
            this.paneluser2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.paneluser2.Location = new System.Drawing.Point(297, 25);
            this.paneluser2.Margin = new System.Windows.Forms.Padding(15);
            this.paneluser2.Name = "paneluser2";
            this.paneluser2.Size = new System.Drawing.Size(242, 350);
            this.paneluser2.TabIndex = 1;
            this.paneluser2.MouseEnter += new System.EventHandler(this.paneluser_MouseEnter);
            this.paneluser2.MouseLeave += new System.EventHandler(this.paneluser_MouseLeave);
            // 
            // paneluser3
            // 
            this.paneluser3.BackColor = System.Drawing.Color.White;
            this.paneluser3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.paneluser3.Location = new System.Drawing.Point(569, 25);
            this.paneluser3.Margin = new System.Windows.Forms.Padding(15);
            this.paneluser3.Name = "paneluser3";
            this.paneluser3.Size = new System.Drawing.Size(242, 350);
            this.paneluser3.TabIndex = 2;
            this.paneluser3.MouseEnter += new System.EventHandler(this.paneluser_MouseEnter);
            this.paneluser3.MouseLeave += new System.EventHandler(this.paneluser_MouseLeave);
            // 
            // paneluser4
            // 
            this.paneluser4.BackColor = System.Drawing.Color.White;
            this.paneluser4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.paneluser4.Location = new System.Drawing.Point(841, 25);
            this.paneluser4.Margin = new System.Windows.Forms.Padding(15);
            this.paneluser4.Name = "paneluser4";
            this.paneluser4.Size = new System.Drawing.Size(245, 350);
            this.paneluser4.TabIndex = 3;
            this.paneluser4.MouseEnter += new System.EventHandler(this.paneluser_MouseEnter);
            this.paneluser4.MouseLeave += new System.EventHandler(this.paneluser_MouseLeave);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(0, 654);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(1407, 30);
            this.label1.TabIndex = 5;
            this.label1.Text = "2025 EduManager - phần mềm quản lí học sinh";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1407, 782);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.labelInstruction);
            this.Controls.Add(this.labelGreeting);
            this.Controls.Add(this.panelTopBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "login";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.login_Load);
            this.panelTopBar.ResumeLayout(false);
            this.panelTopBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAppIcon)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panelTopBar;
        private System.Windows.Forms.Label labelClose;
        private System.Windows.Forms.Label labelMinimize;
        private System.Windows.Forms.Label labelAppTitle;
        private System.Windows.Forms.PictureBox pictureBoxAppIcon;
        private System.Windows.Forms.Label labelGreeting;
        private System.Windows.Forms.Label labelInstruction;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel paneluser1;
        private System.Windows.Forms.Panel paneluser2;
        private System.Windows.Forms.Panel paneluser3;
        private System.Windows.Forms.Panel paneluser4;
        private System.Windows.Forms.Label labelMaximize;
        private System.Windows.Forms.Label label1;
    }
}