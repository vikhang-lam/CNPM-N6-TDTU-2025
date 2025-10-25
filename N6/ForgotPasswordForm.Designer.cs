namespace N6
{
    partial class ForgotPasswordForm
    {
        private System.ComponentModel.IContainer components = null;

       

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlUsernameBorder = new System.Windows.Forms.Panel();
            this.txtUsernameOrEmail = new System.Windows.Forms.TextBox();
            this.pnlOtpBorder = new System.Windows.Forms.Panel();
            this.txtOtp = new System.Windows.Forms.TextBox();
            this.pnlPasswordBorder = new System.Windows.Forms.Panel();
            this.txtNewPassword = new System.Windows.Forms.TextBox();
            this.pnlConfirmPasswordBorder = new System.Windows.Forms.Panel();
            this.txtConfirmPassword = new System.Windows.Forms.TextBox();
            this.btnSendOtp = new System.Windows.Forms.Button();
            this.btnResetPassword = new System.Windows.Forms.Button();
            this.lblClose = new System.Windows.Forms.Label();
            this.lblSubTitle = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.pnlUsernameBorder.SuspendLayout();
            this.pnlOtpBorder.SuspendLayout();
            this.pnlPasswordBorder.SuspendLayout();
            this.pnlConfirmPasswordBorder.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.Black;
            this.lblTitle.Location = new System.Drawing.Point(12, 40);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(560, 37);
            this.lblTitle.TabIndex = 10;
            this.lblTitle.Text = "Khôi Phục Mật Khẩu";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlUsernameBorder
            // 
            this.pnlUsernameBorder.Controls.Add(this.txtUsernameOrEmail);
            this.pnlUsernameBorder.Location = new System.Drawing.Point(50, 110);
            this.pnlUsernameBorder.Name = "pnlUsernameBorder";
            this.pnlUsernameBorder.Size = new System.Drawing.Size(360, 45);
            this.pnlUsernameBorder.TabIndex = 0;
            // 
            // txtUsernameOrEmail
            // 
            this.txtUsernameOrEmail.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtUsernameOrEmail.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtUsernameOrEmail.Location = new System.Drawing.Point(15, 12);
            this.txtUsernameOrEmail.Name = "txtUsernameOrEmail";
            this.txtUsernameOrEmail.Size = new System.Drawing.Size(330, 23);
            this.txtUsernameOrEmail.TabIndex = 0;
            // 
            // pnlOtpBorder
            // 
            this.pnlOtpBorder.Controls.Add(this.txtOtp);
            this.pnlOtpBorder.Location = new System.Drawing.Point(50, 175);
            this.pnlOtpBorder.Name = "pnlOtpBorder";
            this.pnlOtpBorder.Size = new System.Drawing.Size(500, 45);
            this.pnlOtpBorder.TabIndex = 2;
            // 
            // txtOtp
            // 
            this.txtOtp.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtOtp.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtOtp.Location = new System.Drawing.Point(15, 12);
            this.txtOtp.Name = "txtOtp";
            this.txtOtp.Size = new System.Drawing.Size(470, 23);
            this.txtOtp.TabIndex = 0;
            // 
            // pnlPasswordBorder
            // 
            this.pnlPasswordBorder.Controls.Add(this.txtNewPassword);
            this.pnlPasswordBorder.Location = new System.Drawing.Point(50, 235);
            this.pnlPasswordBorder.Name = "pnlPasswordBorder";
            this.pnlPasswordBorder.Size = new System.Drawing.Size(500, 45);
            this.pnlPasswordBorder.TabIndex = 3;
            // 
            // txtNewPassword
            // 
            this.txtNewPassword.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtNewPassword.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtNewPassword.Location = new System.Drawing.Point(15, 12);
            this.txtNewPassword.Name = "txtNewPassword";
            this.txtNewPassword.Size = new System.Drawing.Size(470, 23);
            this.txtNewPassword.TabIndex = 0;
            // 
            // pnlConfirmPasswordBorder
            // 
            this.pnlConfirmPasswordBorder.Controls.Add(this.txtConfirmPassword);
            this.pnlConfirmPasswordBorder.Location = new System.Drawing.Point(50, 295);
            this.pnlConfirmPasswordBorder.Name = "pnlConfirmPasswordBorder";
            this.pnlConfirmPasswordBorder.Size = new System.Drawing.Size(500, 45);
            this.pnlConfirmPasswordBorder.TabIndex = 4;
            // 
            // txtConfirmPassword
            // 
            this.txtConfirmPassword.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtConfirmPassword.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtConfirmPassword.Location = new System.Drawing.Point(15, 12);
            this.txtConfirmPassword.Name = "txtConfirmPassword";
            this.txtConfirmPassword.Size = new System.Drawing.Size(470, 23);
            this.txtConfirmPassword.TabIndex = 0;
            // 
            // btnSendOtp
            // 
            this.btnSendOtp.BackColor = System.Drawing.Color.SeaGreen;
            this.btnSendOtp.FlatAppearance.BorderSize = 0;
            this.btnSendOtp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSendOtp.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnSendOtp.ForeColor = System.Drawing.Color.White;
            this.btnSendOtp.Location = new System.Drawing.Point(420, 110);
            this.btnSendOtp.Name = "btnSendOtp";
            this.btnSendOtp.Size = new System.Drawing.Size(130, 45);
            this.btnSendOtp.TabIndex = 1;
            this.btnSendOtp.Text = "Gửi OTP";
            this.btnSendOtp.UseVisualStyleBackColor = false;
            this.btnSendOtp.Click += new System.EventHandler(this.btnSendOtp_Click);
            // 
            // btnResetPassword
            // 
            this.btnResetPassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(202)))), ((int)(((byte)(245)))));
            this.btnResetPassword.FlatAppearance.BorderSize = 0;
            this.btnResetPassword.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnResetPassword.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnResetPassword.ForeColor = System.Drawing.Color.White;
            this.btnResetPassword.Location = new System.Drawing.Point(120, 370);
            this.btnResetPassword.Name = "btnResetPassword";
            this.btnResetPassword.Size = new System.Drawing.Size(180, 42);
            this.btnResetPassword.TabIndex = 5;
            this.btnResetPassword.Text = "Đặt Lại Mật Khẩu";
            this.btnResetPassword.UseVisualStyleBackColor = false;
            this.btnResetPassword.Click += new System.EventHandler(this.btnResetPassword_Click);
            // 
            // lblClose
            // 
            this.lblClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblClose.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblClose.ForeColor = System.Drawing.Color.Gray;
            this.lblClose.Location = new System.Drawing.Point(542, 9);
            this.lblClose.Name = "lblClose";
            this.lblClose.Size = new System.Drawing.Size(30, 30);
            this.lblClose.TabIndex = 19;
            this.lblClose.Text = "×";
            this.lblClose.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSubTitle
            // 
            this.lblSubTitle.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblSubTitle.ForeColor = System.Drawing.Color.Gray;
            this.lblSubTitle.Location = new System.Drawing.Point(12, 77);
            this.lblSubTitle.Name = "lblSubTitle";
            this.lblSubTitle.Size = new System.Drawing.Size(560, 23);
            this.lblSubTitle.TabIndex = 20;
            this.lblSubTitle.Text = "Nhập Tên đăng nhập hoặc Email để nhận mã OTP";
            this.lblSubTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.White;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnCancel.ForeColor = System.Drawing.Color.Gray;
            this.btnCancel.Location = new System.Drawing.Point(315, 370);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(140, 42);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Hủy";
            this.btnCancel.UseVisualStyleBackColor = false;
            // 
            // ForgotPasswordForm
            // 
            this.AcceptButton = this.btnResetPassword;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(584, 441);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lblSubTitle);
            this.Controls.Add(this.lblClose);
            this.Controls.Add(this.btnResetPassword);
            this.Controls.Add(this.btnSendOtp);
            this.Controls.Add(this.pnlConfirmPasswordBorder);
            this.Controls.Add(this.pnlPasswordBorder);
            this.Controls.Add(this.pnlOtpBorder);
            this.Controls.Add(this.pnlUsernameBorder);
            this.Controls.Add(this.lblTitle);
            this.Name = "ForgotPasswordForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ForgotPasswordForm";
            this.Load += new System.EventHandler(this.ForgotPasswordForm_Load);
            this.pnlUsernameBorder.ResumeLayout(false);
            this.pnlUsernameBorder.PerformLayout();
            this.pnlOtpBorder.ResumeLayout(false);
            this.pnlOtpBorder.PerformLayout();
            this.pnlPasswordBorder.ResumeLayout(false);
            this.pnlPasswordBorder.PerformLayout();
            this.pnlConfirmPasswordBorder.ResumeLayout(false);
            this.pnlConfirmPasswordBorder.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel pnlUsernameBorder;
        private System.Windows.Forms.TextBox txtUsernameOrEmail;
        private System.Windows.Forms.Panel pnlOtpBorder;
        private System.Windows.Forms.TextBox txtOtp;
        private System.Windows.Forms.Panel pnlPasswordBorder;
        private System.Windows.Forms.TextBox txtNewPassword;
        private System.Windows.Forms.Panel pnlConfirmPasswordBorder;
        private System.Windows.Forms.TextBox txtConfirmPassword;
        private System.Windows.Forms.Button btnSendOtp;
        private System.Windows.Forms.Button btnResetPassword;
        private System.Windows.Forms.Label lblClose;
        private System.Windows.Forms.Label lblSubTitle;
        private System.Windows.Forms.Button btnCancel;
    }
}