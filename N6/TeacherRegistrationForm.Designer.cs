namespace N6
{
    partial class TeacherRegistrationForm
    {
        private System.ComponentModel.IContainer components = null;

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing && (components != null))
        //    {
        //        components.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.btnSubmit = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlNameBorder = new System.Windows.Forms.Panel();
            this.txtName = new System.Windows.Forms.TextBox();
            this.pnlUsernameBorder = new System.Windows.Forms.Panel();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.pnlEmailBorder = new System.Windows.Forms.Panel();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.pnlPhoneBorder = new System.Windows.Forms.Panel();
            this.txtPhone = new System.Windows.Forms.TextBox();
            this.pnlPasswordBorder = new System.Windows.Forms.Panel();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.pnlConfirmPasswordBorder = new System.Windows.Forms.Panel();
            this.txtConfirmPassword = new System.Windows.Forms.TextBox();
            this.pnlSubjectBorder = new System.Windows.Forms.Panel();
            this.cmbSubject = new System.Windows.Forms.ComboBox();
            this.lblSubTitle = new System.Windows.Forms.Label();
            this.lblClose = new System.Windows.Forms.Label();
            this.pictureBoxIcon = new System.Windows.Forms.PictureBox();
            this.pnlNameBorder.SuspendLayout();
            this.pnlUsernameBorder.SuspendLayout();
            this.pnlEmailBorder.SuspendLayout();
            this.pnlPhoneBorder.SuspendLayout();
            this.pnlPasswordBorder.SuspendLayout();
            this.pnlConfirmPasswordBorder.SuspendLayout();
            this.pnlSubjectBorder.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSubmit
            // 
            this.btnSubmit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(202)))), ((int)(((byte)(245)))));
            this.btnSubmit.FlatAppearance.BorderSize = 0;
            this.btnSubmit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSubmit.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnSubmit.ForeColor = System.Drawing.Color.White;
            this.btnSubmit.Location = new System.Drawing.Point(490, 460);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(260, 42);
            this.btnSubmit.TabIndex = 7;
            this.btnSubmit.Text = "Gửi Yêu Cầu";
            this.btnSubmit.UseVisualStyleBackColor = false;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.White;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnCancel.ForeColor = System.Drawing.Color.Gray;
            this.btnCancel.Location = new System.Drawing.Point(340, 460);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(140, 42);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Hủy";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.Black;
            this.lblTitle.Location = new System.Drawing.Point(12, 120);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(760, 37);
            this.lblTitle.TabIndex = 9;
            this.lblTitle.Text = "Đăng Ký Tài Khoản Giáo Viên";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlNameBorder
            // 
            this.pnlNameBorder.Controls.Add(this.txtName);
            this.pnlNameBorder.Location = new System.Drawing.Point(50, 200);
            this.pnlNameBorder.Name = "pnlNameBorder";
            this.pnlNameBorder.Size = new System.Drawing.Size(340, 45);
            this.pnlNameBorder.TabIndex = 10;
            // 
            // txtName
            // 
            this.txtName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtName.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtName.Location = new System.Drawing.Point(15, 12);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(310, 23);
            this.txtName.TabIndex = 0;
            // 
            // pnlUsernameBorder
            // 
            this.pnlUsernameBorder.Controls.Add(this.txtUsername);
            this.pnlUsernameBorder.Location = new System.Drawing.Point(400, 200);
            this.pnlUsernameBorder.Name = "pnlUsernameBorder";
            this.pnlUsernameBorder.Size = new System.Drawing.Size(340, 45);
            this.pnlUsernameBorder.TabIndex = 11;
            // 
            // txtUsername
            // 
            this.txtUsername.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtUsername.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtUsername.Location = new System.Drawing.Point(15, 12);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(310, 23);
            this.txtUsername.TabIndex = 1;
            // 
            // pnlEmailBorder
            // 
            this.pnlEmailBorder.Controls.Add(this.txtEmail);
            this.pnlEmailBorder.Location = new System.Drawing.Point(50, 260);
            this.pnlEmailBorder.Name = "pnlEmailBorder";
            this.pnlEmailBorder.Size = new System.Drawing.Size(340, 45);
            this.pnlEmailBorder.TabIndex = 11;
            // 
            // txtEmail
            // 
            this.txtEmail.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtEmail.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtEmail.Location = new System.Drawing.Point(15, 12);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(310, 23);
            this.txtEmail.TabIndex = 2;
            // 
            // pnlPhoneBorder
            // 
            this.pnlPhoneBorder.Controls.Add(this.txtPhone);
            this.pnlPhoneBorder.Location = new System.Drawing.Point(400, 260);
            this.pnlPhoneBorder.Name = "pnlPhoneBorder";
            this.pnlPhoneBorder.Size = new System.Drawing.Size(340, 45);
            this.pnlPhoneBorder.TabIndex = 12;
            // 
            // txtPhone
            // 
            this.txtPhone.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtPhone.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtPhone.Location = new System.Drawing.Point(15, 12);
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.Size = new System.Drawing.Size(310, 23);
            this.txtPhone.TabIndex = 3;
            // 
            // pnlPasswordBorder
            // 
            this.pnlPasswordBorder.Controls.Add(this.txtPassword);
            this.pnlPasswordBorder.Location = new System.Drawing.Point(50, 320);
            this.pnlPasswordBorder.Name = "pnlPasswordBorder";
            this.pnlPasswordBorder.Size = new System.Drawing.Size(340, 45);
            this.pnlPasswordBorder.TabIndex = 13;
            // 
            // txtPassword
            // 
            this.txtPassword.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtPassword.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtPassword.Location = new System.Drawing.Point(15, 12);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(310, 23);
            this.txtPassword.TabIndex = 4;
            // 
            // pnlConfirmPasswordBorder
            // 
            this.pnlConfirmPasswordBorder.Controls.Add(this.txtConfirmPassword);
            this.pnlConfirmPasswordBorder.Location = new System.Drawing.Point(400, 320);
            this.pnlConfirmPasswordBorder.Name = "pnlConfirmPasswordBorder";
            this.pnlConfirmPasswordBorder.Size = new System.Drawing.Size(340, 45);
            this.pnlConfirmPasswordBorder.TabIndex = 14;
            // 
            // txtConfirmPassword
            // 
            this.txtConfirmPassword.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtConfirmPassword.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtConfirmPassword.Location = new System.Drawing.Point(15, 12);
            this.txtConfirmPassword.Name = "txtConfirmPassword";
            this.txtConfirmPassword.Size = new System.Drawing.Size(310, 23);
            this.txtConfirmPassword.TabIndex = 5;
            // 
            // pnlSubjectBorder
            // 
            this.pnlSubjectBorder.Controls.Add(this.cmbSubject);
            this.pnlSubjectBorder.Location = new System.Drawing.Point(50, 380);
            this.pnlSubjectBorder.Name = "pnlSubjectBorder";
            this.pnlSubjectBorder.Size = new System.Drawing.Size(690, 45);
            this.pnlSubjectBorder.TabIndex = 15;
            // 
            // cmbSubject
            // 
            this.cmbSubject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSubject.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbSubject.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbSubject.FormattingEnabled = true;
            this.cmbSubject.Location = new System.Drawing.Point(15, 7);
            this.cmbSubject.Name = "cmbSubject";
            this.cmbSubject.Size = new System.Drawing.Size(660, 31);
            this.cmbSubject.TabIndex = 6;
            // 
            // lblSubTitle
            // 
            this.lblSubTitle.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblSubTitle.ForeColor = System.Drawing.Color.Gray;
            this.lblSubTitle.Location = new System.Drawing.Point(12, 157);
            this.lblSubTitle.Name = "lblSubTitle";
            this.lblSubTitle.Size = new System.Drawing.Size(760, 23);
            this.lblSubTitle.TabIndex = 17;
            this.lblSubTitle.Text = "Vui lòng nhập thông tin để tạo yêu cầu";
            this.lblSubTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblClose
            // 
            this.lblClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblClose.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblClose.ForeColor = System.Drawing.Color.Gray;
            this.lblClose.Location = new System.Drawing.Point(742, 9);
            this.lblClose.Name = "lblClose";
            this.lblClose.Size = new System.Drawing.Size(30, 30);
            this.lblClose.TabIndex = 18;
            this.lblClose.Text = "×";
            this.lblClose.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBoxIcon
            // 
            this.pictureBoxIcon.Location = new System.Drawing.Point(340, 20);
            this.pictureBoxIcon.Name = "pictureBoxIcon";
            this.pictureBoxIcon.Size = new System.Drawing.Size(120, 97);
            this.pictureBoxIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxIcon.TabIndex = 16;
            this.pictureBoxIcon.TabStop = false;
            // 
            // TeacherRegistrationForm
            // 
            this.AcceptButton = this.btnSubmit;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(784, 531);
            this.Controls.Add(this.lblClose);
            this.Controls.Add(this.lblSubTitle);
            this.Controls.Add(this.pictureBoxIcon);
            this.Controls.Add(this.pnlSubjectBorder);
            this.Controls.Add(this.pnlConfirmPasswordBorder);
            this.Controls.Add(this.pnlPasswordBorder);
            this.Controls.Add(this.pnlPhoneBorder);
            this.Controls.Add(this.pnlEmailBorder);
            this.Controls.Add(this.pnlUsernameBorder);
            this.Controls.Add(this.pnlNameBorder);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSubmit);
            this.Name = "TeacherRegistrationForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Đăng Ký Tài Khoản";
            this.Load += new System.EventHandler(this.TeacherRegistrationForm_Load);
            this.pnlNameBorder.ResumeLayout(false);
            this.pnlNameBorder.PerformLayout();
            this.pnlUsernameBorder.ResumeLayout(false);
            this.pnlUsernameBorder.PerformLayout();
            this.pnlEmailBorder.ResumeLayout(false);
            this.pnlEmailBorder.PerformLayout();
            this.pnlPhoneBorder.ResumeLayout(false);
            this.pnlPhoneBorder.PerformLayout();
            this.pnlPasswordBorder.ResumeLayout(false);
            this.pnlPasswordBorder.PerformLayout();
            this.pnlConfirmPasswordBorder.ResumeLayout(false);
            this.pnlConfirmPasswordBorder.PerformLayout();
            this.pnlSubjectBorder.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel pnlNameBorder;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Panel pnlUsernameBorder;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Panel pnlEmailBorder;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Panel pnlPhoneBorder;
        private System.Windows.Forms.TextBox txtPhone;
        private System.Windows.Forms.Panel pnlPasswordBorder;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Panel pnlConfirmPasswordBorder;
        private System.Windows.Forms.TextBox txtConfirmPassword;
        private System.Windows.Forms.Panel pnlSubjectBorder;
        private System.Windows.Forms.ComboBox cmbSubject;
        private System.Windows.Forms.PictureBox pictureBoxIcon;
        private System.Windows.Forms.Label lblSubTitle;
        private System.Windows.Forms.Label lblClose;
    }
}