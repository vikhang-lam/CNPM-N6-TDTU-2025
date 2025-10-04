namespace N6
{
    // Dùng CircularPictureBox từ ProfileForm
    using static N6.ProfileForm;

    partial class AdminProfileForm
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
            this.panelMain = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.panelPassword = new System.Windows.Forms.Panel();
            this.btnSavePassword = new System.Windows.Forms.Button();
            this.txtConfirmPass = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtNewPass = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtOldPass = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnTogglePasswordPanel = new System.Windows.Forms.Button();
            this.panelInfo = new System.Windows.Forms.Panel();
            this.btnChangeEmail = new System.Windows.Forms.Button();
            this.lblEmail = new System.Windows.Forms.Label();
            this.lblUsername = new System.Windows.Forms.Label();
            this.picAvatar = new N6.CircularPictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panelMain.SuspendLayout();
            this.panelPassword.SuspendLayout();
            this.panelInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picAvatar)).BeginInit();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.BackColor = System.Drawing.Color.FromArgb(245, 247, 250);
            this.panelMain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelMain.Controls.Add(this.btnClose);
            this.panelMain.Controls.Add(this.panelPassword);
            this.panelMain.Controls.Add(this.btnTogglePasswordPanel);
            this.panelMain.Controls.Add(this.panelInfo);
            this.panelMain.Controls.Add(this.picAvatar);
            this.panelMain.Controls.Add(this.label1);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(450, 520);
            this.panelMain.TabIndex = 0;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(231, 76, 60);
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(410, 10);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(29, 27);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "X";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // panelPassword
            // 
            this.panelPassword.Controls.Add(this.btnSavePassword);
            this.panelPassword.Controls.Add(this.txtConfirmPass);
            this.panelPassword.Controls.Add(this.label4);
            this.panelPassword.Controls.Add(this.txtNewPass);
            this.panelPassword.Controls.Add(this.label3);
            this.panelPassword.Controls.Add(this.txtOldPass);
            this.panelPassword.Controls.Add(this.label2);
            this.panelPassword.Location = new System.Drawing.Point(30, 310);
            this.panelPassword.Name = "panelPassword";
            this.panelPassword.Size = new System.Drawing.Size(390, 150);
            this.panelPassword.TabIndex = 4;
            this.panelPassword.Visible = false;
            // 
            // btnSavePassword
            // 
            this.btnSavePassword.BackColor = System.Drawing.Color.FromArgb(46, 204, 113);
            this.btnSavePassword.FlatAppearance.BorderSize = 0;
            this.btnSavePassword.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSavePassword.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnSavePassword.ForeColor = System.Drawing.Color.White;
            this.btnSavePassword.Location = new System.Drawing.Point(260, 110);
            this.btnSavePassword.Name = "btnSavePassword";
            this.btnSavePassword.Size = new System.Drawing.Size(120, 30);
            this.btnSavePassword.TabIndex = 6;
            this.btnSavePassword.Text = "Lưu thay đổi";
            this.btnSavePassword.UseVisualStyleBackColor = false;
            this.btnSavePassword.Click += new System.EventHandler(this.btnSavePassword_Click);
            // 
            // txtConfirmPass
            // 
            this.txtConfirmPass.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.txtConfirmPass.Location = new System.Drawing.Point(140, 75);
            this.txtConfirmPass.Name = "txtConfirmPass";
            this.txtConfirmPass.PasswordChar = '*';
            this.txtConfirmPass.Size = new System.Drawing.Size(240, 25);
            this.txtConfirmPass.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.label4.Location = new System.Drawing.Point(10, 78);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(124, 17);
            this.label4.Text = "Nhập lại mật khẩu:";
            // 
            // txtNewPass
            // 
            this.txtNewPass.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.txtNewPass.Location = new System.Drawing.Point(140, 40);
            this.txtNewPass.Name = "txtNewPass";
            this.txtNewPass.PasswordChar = '*';
            this.txtNewPass.Size = new System.Drawing.Size(240, 25);
            this.txtNewPass.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(10, 43);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(98, 17);
            this.label3.Text = "Mật khẩu mới:";
            // 
            // txtOldPass
            // 
            this.txtOldPass.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.txtOldPass.Location = new System.Drawing.Point(140, 5);
            this.txtOldPass.Name = "txtOldPass";
            this.txtOldPass.PasswordChar = '*';
            this.txtOldPass.Size = new System.Drawing.Size(240, 25);
            this.txtOldPass.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(10, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 17);
            this.label2.Text = "Mật khẩu cũ:";
            // 
            // btnTogglePasswordPanel
            // 
            this.btnTogglePasswordPanel.BackColor = System.Drawing.Color.FromArgb(52, 152, 219);
            this.btnTogglePasswordPanel.FlatAppearance.BorderSize = 0;
            this.btnTogglePasswordPanel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTogglePasswordPanel.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold);
            this.btnTogglePasswordPanel.ForeColor = System.Drawing.Color.White;
            this.btnTogglePasswordPanel.Location = new System.Drawing.Point(150, 260);
            this.btnTogglePasswordPanel.Name = "btnTogglePasswordPanel";
            this.btnTogglePasswordPanel.Size = new System.Drawing.Size(150, 40);
            this.btnTogglePasswordPanel.TabIndex = 3;
            this.btnTogglePasswordPanel.Text = "Đổi mật khẩu";
            this.btnTogglePasswordPanel.UseVisualStyleBackColor = false;
            this.btnTogglePasswordPanel.Click += new System.EventHandler(this.btnTogglePasswordPanel_Click);
            // 
            // panelInfo
            // 
            this.panelInfo.Controls.Add(this.btnChangeEmail);
            this.panelInfo.Controls.Add(this.lblEmail);
            this.panelInfo.Controls.Add(this.lblUsername);
            this.panelInfo.Location = new System.Drawing.Point(3, 160);
            this.panelInfo.Name = "panelInfo";
            this.panelInfo.Size = new System.Drawing.Size(444, 90);
            this.panelInfo.TabIndex = 2;
            // 
            // btnChangeEmail
            // 
            this.btnChangeEmail.BackColor = System.Drawing.Color.FromArgb(52, 152, 219);
            this.btnChangeEmail.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnChangeEmail.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnChangeEmail.ForeColor = System.Drawing.Color.White;
            this.btnChangeEmail.Location = new System.Drawing.Point(330, 50);
            this.btnChangeEmail.Size = new System.Drawing.Size(90, 27);
            this.btnChangeEmail.Text = "Đổi Email";
            this.btnChangeEmail.UseVisualStyleBackColor = false;
            this.btnChangeEmail.Click += new System.EventHandler(this.btnChangeEmail_Click);
            // 
            // lblEmail
            // 
            this.lblEmail.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblEmail.Location = new System.Drawing.Point(10, 50);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(300, 23);
            this.lblEmail.Text = "Email: admin@example.com";
            this.lblEmail.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblUsername
            // 
            this.lblUsername.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold);
            this.lblUsername.Location = new System.Drawing.Point(10, 10);
            this.lblUsername.Size = new System.Drawing.Size(424, 30);
            this.lblUsername.Text = "Admin";
            this.lblUsername.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // picAvatar
            // 
            this.picAvatar.Location = new System.Drawing.Point(175, 50);
            this.picAvatar.Name = "picAvatar";
            this.picAvatar.Size = new System.Drawing.Size(100, 100);
            this.picAvatar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picAvatar.TabIndex = 1;
            this.picAvatar.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(120, 10);
            this.label1.Size = new System.Drawing.Size(210, 32);
            this.label1.Text = "Hồ sơ Quản trị viên";
            // 
            // AdminProfileForm
            // 
            this.ClientSize = new System.Drawing.Size(450, 520);
            this.Controls.Add(this.panelMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "AdminProfileForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AdminProfileForm";
            this.Load += new System.EventHandler(this.AdminProfileForm_Load);
            this.panelMain.ResumeLayout(false);
            this.panelMain.PerformLayout();
            this.panelPassword.ResumeLayout(false);
            this.panelPassword.PerformLayout();
            this.panelInfo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picAvatar)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Label label1;
        private CircularPictureBox picAvatar;
        private System.Windows.Forms.Panel panelInfo;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.Button btnTogglePasswordPanel;
        private System.Windows.Forms.Panel panelPassword;
        private System.Windows.Forms.Button btnSavePassword;
        private System.Windows.Forms.TextBox txtConfirmPass;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtNewPass;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtOldPass;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnChangeEmail;
    }
}
