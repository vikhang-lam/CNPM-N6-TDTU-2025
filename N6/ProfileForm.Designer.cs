using System;
using System.Drawing;
using System.Windows.Forms;

namespace N6
{
    partial class ProfileForm
    {
        private RoundedPanel panelProfile;
        private RoundedPanel panelPassword;
        private PictureBox picAvatar;
        private Label lblName, lblSubject, lblEmail, lblPhone;
        private Button btnTogglePasswordPanel, btnClose, btnSavePassword;
        private TextBox txtOldPass, txtNewPass, txtConfirmPass;
        private Label lblOld, lblNew, lblConfirm;

        private void InitializeComponent()
        {
            this.panelProfile = new RoundedPanel();
            this.panelPassword = new RoundedPanel();
            this.picAvatar = new PictureBox();
            this.lblName = new Label();
            this.lblSubject = new Label();
            this.lblEmail = new Label();
            this.lblPhone = new Label();
            this.btnTogglePasswordPanel = new Button();
            this.btnClose = new Button();

            this.txtOldPass = new TextBox();
            this.txtNewPass = new TextBox();
            this.txtConfirmPass = new TextBox();
            this.lblOld = new Label();
            this.lblNew = new Label();
            this.lblConfirm = new Label();
            this.btnSavePassword = new Button();

            ((System.ComponentModel.ISupportInitialize)(this.picAvatar)).BeginInit();
            this.panelProfile.SuspendLayout();
            this.panelPassword.SuspendLayout();
            this.SuspendLayout();

            // panelProfile
            this.panelProfile.CornerRadius = 20;
            this.panelProfile.BackColor = Color.White;
            this.panelProfile.Size = new Size(520, 200);
            this.panelProfile.Location = new Point(20, 20);

            // Avatar
            this.picAvatar.Size = new Size(100, 100);
            this.picAvatar.Location = new Point(30, 40);
            this.picAvatar.SizeMode = PictureBoxSizeMode.Zoom;
            this.picAvatar.BorderStyle = BorderStyle.FixedSingle;

            // Name
            this.lblName.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            this.lblName.Location = new Point(150, 40);
            this.lblName.AutoSize = true;

            // Subject
            this.lblSubject.Font = new Font("Segoe UI", 12, FontStyle.Italic);
            this.lblSubject.Location = new Point(150, 80);
            this.lblSubject.AutoSize = true;

            // Email
            this.lblEmail.Font = new Font("Segoe UI", 11);
            this.lblEmail.Location = new Point(30, 150);
            this.lblEmail.AutoSize = true;

            // Phone
            this.lblPhone.Font = new Font("Segoe UI", 11);
            this.lblPhone.Location = new Point(250, 150);
            this.lblPhone.AutoSize = true;

            // Btn Change password
            this.btnTogglePasswordPanel.Text = "🔑 Đổi mật khẩu";
            this.btnTogglePasswordPanel.BackColor = Color.SeaGreen;
            this.btnTogglePasswordPanel.ForeColor = Color.White;
            this.btnTogglePasswordPanel.FlatStyle = FlatStyle.Flat;
            this.btnTogglePasswordPanel.Location = new Point(370, 20);
            this.btnTogglePasswordPanel.Size = new Size(120, 35);
            this.btnTogglePasswordPanel.Click += new EventHandler(this.btnTogglePasswordPanel_Click);

            // Btn Close
            this.btnClose.Text = "✖ Đóng";
            this.btnClose.BackColor = Color.DarkGray;
            this.btnClose.ForeColor = Color.White;
            this.btnClose.FlatStyle = FlatStyle.Flat;
            this.btnClose.Location = new Point(370, 60);
            this.btnClose.Size = new Size(120, 35);
            this.btnClose.Click += new EventHandler(this.btnClose_Click);

            // add profile controls
            this.panelProfile.Controls.Add(this.picAvatar);
            this.panelProfile.Controls.Add(this.lblName);
            this.panelProfile.Controls.Add(this.lblSubject);
            this.panelProfile.Controls.Add(this.lblEmail);
            this.panelProfile.Controls.Add(this.lblPhone);
            this.panelProfile.Controls.Add(this.btnTogglePasswordPanel);
            this.panelProfile.Controls.Add(this.btnClose);

            // panelPassword
            this.panelPassword.CornerRadius = 20;
            this.panelPassword.BackColor = Color.White;
            this.panelPassword.Size = new Size(520, 220);
            this.panelPassword.Location = new Point(20, 240);
            this.panelPassword.Visible = false;

            // Labels
            this.lblOld.Text = "Mật khẩu cũ:";
            this.lblOld.Font = new Font("Segoe UI", 11);
            this.lblOld.Location = new Point(30, 30);
            this.lblOld.AutoSize = true;

            this.lblNew.Text = "Mật khẩu mới:";
            this.lblNew.Font = new Font("Segoe UI", 11);
            this.lblNew.Location = new Point(30, 80);
            this.lblNew.AutoSize = true;

            this.lblConfirm.Text = "Nhập lại:";
            this.lblConfirm.Font = new Font("Segoe UI", 11);
            this.lblConfirm.Location = new Point(30, 130);
            this.lblConfirm.AutoSize = true;

            // TextBoxes
            this.txtOldPass.Font = new Font("Segoe UI", 11);
            this.txtOldPass.Location = new Point(150, 25);
            this.txtOldPass.Size = new Size(200, 30);
            this.txtOldPass.PasswordChar = '•';

            this.txtNewPass.Font = new Font("Segoe UI", 11);
            this.txtNewPass.Location = new Point(150, 75);
            this.txtNewPass.Size = new Size(200, 30);
            this.txtNewPass.PasswordChar = '•';

            this.txtConfirmPass.Font = new Font("Segoe UI", 11);
            this.txtConfirmPass.Location = new Point(150, 125);
            this.txtConfirmPass.Size = new Size(200, 30);
            this.txtConfirmPass.PasswordChar = '•';

            // Save Password
            this.btnSavePassword.Text = "💾 Lưu";
            this.btnSavePassword.BackColor = Color.DodgerBlue;
            this.btnSavePassword.ForeColor = Color.White;
            this.btnSavePassword.FlatStyle = FlatStyle.Flat;
            this.btnSavePassword.Location = new Point(370, 80);
            this.btnSavePassword.Size = new Size(120, 40);
            this.btnSavePassword.Click += new EventHandler(this.btnSavePassword_Click);

            // add password controls
            this.panelPassword.Controls.Add(this.lblOld);
            this.panelPassword.Controls.Add(this.txtOldPass);
            this.panelPassword.Controls.Add(this.lblNew);
            this.panelPassword.Controls.Add(this.txtNewPass);
            this.panelPassword.Controls.Add(this.lblConfirm);
            this.panelPassword.Controls.Add(this.txtConfirmPass);
            this.panelPassword.Controls.Add(this.btnSavePassword);

            // ProfileForm
            this.BackColor = Color.FromArgb(235, 240, 250);
            this.ClientSize = new Size(560, 480);
            this.Controls.Add(this.panelProfile);
            this.Controls.Add(this.panelPassword);
            this.Load += new EventHandler(this.ProfileForm_Load);

            ((System.ComponentModel.ISupportInitialize)(this.picAvatar)).EndInit();
            this.panelProfile.ResumeLayout(false);
            this.panelProfile.PerformLayout();
            this.panelPassword.ResumeLayout(false);
            this.panelPassword.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}
