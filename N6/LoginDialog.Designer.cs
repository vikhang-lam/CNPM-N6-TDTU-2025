namespace N6
{
    partial class LoginDialog
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.PictureBox pictureBoxBackground;
        private System.Windows.Forms.Label lblClose;
        private System.Windows.Forms.PictureBox pictureBoxAvatar;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubTitle;
        private System.Windows.Forms.Panel pnlUsernameBorder;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Panel pnlPasswordBorder;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.PictureBox picEye;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblNewTeacherLink;
        private System.Windows.Forms.Label lblForgotPassword;

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing && (components != null))
        //    {
        //        components.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        private void InitializeComponent()
        {
            this.lblClose = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSubTitle = new System.Windows.Forms.Label();
            this.pnlUsernameBorder = new System.Windows.Forms.Panel();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.pnlPasswordBorder = new System.Windows.Forms.Panel();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.picEye = new System.Windows.Forms.PictureBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblNewTeacherLink = new System.Windows.Forms.Label();
            this.lblForgotPassword = new System.Windows.Forms.Label();
            this.pictureBoxAvatar = new System.Windows.Forms.PictureBox();
            this.pictureBoxBackground = new System.Windows.Forms.PictureBox();
            this.pnlUsernameBorder.SuspendLayout();
            this.pnlPasswordBorder.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picEye)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAvatar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBackground)).BeginInit();
            this.SuspendLayout();
            // 
            // lblClose
            // 
            this.lblClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblClose.BackColor = System.Drawing.Color.White;
            this.lblClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblClose.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblClose.ForeColor = System.Drawing.Color.Gray;
            this.lblClose.Location = new System.Drawing.Point(499, 9);
            this.lblClose.Name = "lblClose";
            this.lblClose.Size = new System.Drawing.Size(39, 40);
            this.lblClose.TabIndex = 1;
            this.lblClose.Text = "×";
            this.lblClose.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTitle
            // 
            this.lblTitle.BackColor = System.Drawing.Color.White;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.Black;
            this.lblTitle.Location = new System.Drawing.Point(34, 154);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(500, 46);
            this.lblTitle.TabIndex = 3;
            this.lblTitle.Text = "Đăng nhập";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSubTitle
            // 
            this.lblSubTitle.BackColor = System.Drawing.Color.White;
            this.lblSubTitle.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblSubTitle.ForeColor = System.Drawing.Color.Gray;
            this.lblSubTitle.Location = new System.Drawing.Point(35, 200);
            this.lblSubTitle.Name = "lblSubTitle";
            this.lblSubTitle.Size = new System.Drawing.Size(500, 33);
            this.lblSubTitle.TabIndex = 4;
            this.lblSubTitle.Text = "Nhập thông tin đăng nhập của bạn";
            this.lblSubTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlUsernameBorder
            // 
            this.pnlUsernameBorder.BackColor = System.Drawing.Color.White;
            this.pnlUsernameBorder.Controls.Add(this.txtUsername);
            this.pnlUsernameBorder.Location = new System.Drawing.Point(50, 247);
            this.pnlUsernameBorder.Name = "pnlUsernameBorder";
            this.pnlUsernameBorder.Size = new System.Drawing.Size(460, 45);
            this.pnlUsernameBorder.TabIndex = 5;
            // 
            // txtUsername
            // 
            this.txtUsername.BackColor = System.Drawing.Color.White;
            this.txtUsername.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtUsername.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtUsername.Location = new System.Drawing.Point(12, 10);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(406, 27);
            this.txtUsername.TabIndex = 0;
            // 
            // pnlPasswordBorder
            // 
            this.pnlPasswordBorder.BackColor = System.Drawing.Color.White;
            this.pnlPasswordBorder.Controls.Add(this.txtPassword);
            this.pnlPasswordBorder.Controls.Add(this.picEye);
            this.pnlPasswordBorder.Location = new System.Drawing.Point(50, 313);
            this.pnlPasswordBorder.Name = "pnlPasswordBorder";
            this.pnlPasswordBorder.Size = new System.Drawing.Size(460, 48);
            this.pnlPasswordBorder.TabIndex = 6;
            // 
            // txtPassword
            // 
            this.txtPassword.BackColor = System.Drawing.Color.White;
            this.txtPassword.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtPassword.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtPassword.Location = new System.Drawing.Point(3, 10);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(415, 27);
            this.txtPassword.TabIndex = 0;
            // 
            // picEye
            // 
            this.picEye.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picEye.Location = new System.Drawing.Point(418, 10);
            this.picEye.Name = "picEye";
            this.picEye.Size = new System.Drawing.Size(29, 27);
            this.picEye.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picEye.TabIndex = 1;
            this.picEye.TabStop = false;
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(202)))), ((int)(((byte)(245)))));
            this.btnOK.FlatAppearance.BorderSize = 0;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnOK.ForeColor = System.Drawing.Color.White;
            this.btnOK.Location = new System.Drawing.Point(40, 405);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(205, 52);
            this.btnOK.TabIndex = 8;
            this.btnOK.Text = "Đăng nhập";
            this.btnOK.UseVisualStyleBackColor = false;
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.White;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnCancel.ForeColor = System.Drawing.Color.Gray;
            this.btnCancel.Location = new System.Drawing.Point(309, 405);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(206, 52);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Hủy";
            this.btnCancel.UseVisualStyleBackColor = false;
            // 
            // lblNewTeacherLink
            // 
            this.lblNewTeacherLink.BackColor = System.Drawing.Color.White;
            this.lblNewTeacherLink.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblNewTeacherLink.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblNewTeacherLink.ForeColor = System.Drawing.Color.Gray;
            this.lblNewTeacherLink.Location = new System.Drawing.Point(310, 367);
            this.lblNewTeacherLink.Name = "lblNewTeacherLink";
            this.lblNewTeacherLink.Size = new System.Drawing.Size(205, 35);
            this.lblNewTeacherLink.TabIndex = 10;
            this.lblNewTeacherLink.Text = "Đăng ký tài khoản";
            this.lblNewTeacherLink.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblForgotPassword
            // 
            this.lblForgotPassword.AutoSize = true;
            this.lblForgotPassword.BackColor = System.Drawing.Color.White;
            this.lblForgotPassword.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblForgotPassword.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblForgotPassword.ForeColor = System.Drawing.Color.Gray;
            this.lblForgotPassword.Location = new System.Drawing.Point(46, 373);
            this.lblForgotPassword.Name = "lblForgotPassword";
            this.lblForgotPassword.Size = new System.Drawing.Size(137, 23);
            this.lblForgotPassword.TabIndex = 11;
            this.lblForgotPassword.Text = "Quên mật khẩu?";
            // 
            // pictureBoxAvatar
            // 
            this.pictureBoxAvatar.BackColor = System.Drawing.Color.White;
            this.pictureBoxAvatar.Location = new System.Drawing.Point(229, 34);
            this.pictureBoxAvatar.Name = "pictureBoxAvatar";
            this.pictureBoxAvatar.Size = new System.Drawing.Size(108, 108);
            this.pictureBoxAvatar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxAvatar.TabIndex = 2;
            this.pictureBoxAvatar.TabStop = false;
            // 
            // pictureBoxBackground
            // 
            this.pictureBoxBackground.BackColor = System.Drawing.Color.White;
            this.pictureBoxBackground.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxBackground.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxBackground.Name = "pictureBoxBackground";
            this.pictureBoxBackground.Size = new System.Drawing.Size(550, 480);
            this.pictureBoxBackground.TabIndex = 0;
            this.pictureBoxBackground.TabStop = false;
            // 
            // LoginDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(550, 480);
            this.Controls.Add(this.lblForgotPassword);
            this.Controls.Add(this.lblNewTeacherLink);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.pnlPasswordBorder);
            this.Controls.Add(this.pnlUsernameBorder);
            this.Controls.Add(this.lblSubTitle);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.pictureBoxAvatar);
            this.Controls.Add(this.lblClose);
            this.Controls.Add(this.pictureBoxBackground);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "LoginDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Đăng nhập";
            this.pnlUsernameBorder.ResumeLayout(false);
            this.pnlUsernameBorder.PerformLayout();
            this.pnlPasswordBorder.ResumeLayout(false);
            this.pnlPasswordBorder.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picEye)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAvatar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBackground)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}