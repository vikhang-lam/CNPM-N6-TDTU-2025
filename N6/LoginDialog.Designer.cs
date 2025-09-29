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

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pictureBoxBackground = new System.Windows.Forms.PictureBox();
            this.lblClose = new System.Windows.Forms.Label();
            this.pictureBoxAvatar = new System.Windows.Forms.PictureBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSubTitle = new System.Windows.Forms.Label();
            this.pnlUsernameBorder = new System.Windows.Forms.Panel();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.pnlPasswordBorder = new System.Windows.Forms.Panel();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.picEye = new System.Windows.Forms.PictureBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();

            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBackground)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAvatar)).BeginInit();
            this.pnlUsernameBorder.SuspendLayout();
            this.pnlPasswordBorder.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picEye)).BeginInit();
            this.SuspendLayout();

            // pictureBoxBackground
            this.pictureBoxBackground.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxBackground.BackColor = System.Drawing.Color.White;
            this.pictureBoxBackground.Size = new System.Drawing.Size(420, 360);
            this.pictureBoxBackground.TabStop = false;

            // lblClose
            this.lblClose.Text = "×";
            this.lblClose.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblClose.ForeColor = System.Drawing.Color.Gray;
            this.lblClose.AutoSize = false;
            this.lblClose.Size = new System.Drawing.Size(30, 30);
            this.lblClose.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblClose.Location = new System.Drawing.Point(390, 0);

            // pictureBoxAvatar
            this.pictureBoxAvatar.Size = new System.Drawing.Size(100, 100);
            this.pictureBoxAvatar.Location = new System.Drawing.Point(160, 40);
            this.pictureBoxAvatar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;

            // lblTitle
            this.lblTitle.Text = "Đăng nhập";
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.Black;
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblTitle.Size = new System.Drawing.Size(420, 30);
            this.lblTitle.Location = new System.Drawing.Point(0, 150);

            // lblSubTitle
            this.lblSubTitle.Text = "Nhập thông tin đăng nhập của bạn";
            this.lblSubTitle.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblSubTitle.ForeColor = System.Drawing.Color.Gray;
            this.lblSubTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblSubTitle.Size = new System.Drawing.Size(420, 22);
            this.lblSubTitle.Location = new System.Drawing.Point(0, 180);

            // pnlUsernameBorder
            this.pnlUsernameBorder.Size = new System.Drawing.Size(340, 40);
            this.pnlUsernameBorder.Location = new System.Drawing.Point(40, 210);
            this.pnlUsernameBorder.BackColor = System.Drawing.Color.White;
            this.pnlUsernameBorder.Controls.Add(this.txtUsername);

            // txtUsername
            this.txtUsername.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtUsername.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtUsername.BackColor = System.Drawing.Color.White;
            this.txtUsername.Location = new System.Drawing.Point(12, 10);
            this.txtUsername.Width = 310;

            // pnlPasswordBorder
            this.pnlPasswordBorder.Size = new System.Drawing.Size(340, 40);
            this.pnlPasswordBorder.Location = new System.Drawing.Point(40, 260);
            this.pnlPasswordBorder.BackColor = System.Drawing.Color.White;
            this.pnlPasswordBorder.Controls.Add(this.txtPassword);
            this.pnlPasswordBorder.Controls.Add(this.picEye);

            // txtPassword
            this.txtPassword.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtPassword.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtPassword.BackColor = System.Drawing.Color.White;
            this.txtPassword.Location = new System.Drawing.Point(12, 10);
            this.txtPassword.Width = 280;

            // picEye
            this.picEye.Size = new System.Drawing.Size(18, 18);
            this.picEye.Location = new System.Drawing.Point(310, 11);
            this.picEye.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picEye.Cursor = System.Windows.Forms.Cursors.Hand;

            // btnCancel
            this.btnCancel.Text = "Hủy";
            this.btnCancel.Size = new System.Drawing.Size(150, 38);
            this.btnCancel.Location = new System.Drawing.Point(40, 310);
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
            this.btnCancel.BackColor = System.Drawing.Color.White;
            this.btnCancel.ForeColor = System.Drawing.Color.Gray;

            // btnOK
            this.btnOK.Text = "Đăng nhập";
            this.btnOK.Size = new System.Drawing.Size(180, 38);
            this.btnOK.Location = new System.Drawing.Point(200, 310);
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.FlatAppearance.BorderSize = 0;
            this.btnOK.BackColor = System.Drawing.ColorTranslator.FromHtml("#F48FB1");
            this.btnOK.ForeColor = System.Drawing.Color.White;

            // LoginDialog
            this.AcceptButton = this.btnOK;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(420, 360);
            this.Controls.Add(this.lblClose);
            this.Controls.Add(this.pictureBoxAvatar);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblSubTitle);
            this.Controls.Add(this.pnlUsernameBorder);
            this.Controls.Add(this.pnlPasswordBorder);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.pictureBoxBackground);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Đăng nhập";

            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBackground)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAvatar)).EndInit();
            this.pnlUsernameBorder.ResumeLayout(false);
            this.pnlUsernameBorder.PerformLayout();
            this.pnlPasswordBorder.ResumeLayout(false);
            this.pnlPasswordBorder.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picEye)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
