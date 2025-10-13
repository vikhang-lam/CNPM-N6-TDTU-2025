using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace N6
{
    public partial class LoginDialog : Form
    {
        public string Username => (txtUsername.Text == "Nhập tên đăng nhập") ? "" : txtUsername.Text.Trim();
        public string Password => (txtPassword.Text == "Nhập mật khẩu") ? "" : txtPassword.Text.Trim();

        private Color usernameBorderColor = Color.RoyalBlue;
        private Color passwordBorderColor = Color.RoyalBlue;
        private readonly Color linkHoverColor = ColorTranslator.FromHtml("#2fcaf5");
        private readonly Color linkIdleColor = Color.Gray;
        private readonly PaintEventHandler _usernameBorderPaintHandler;
        private readonly PaintEventHandler _passwordBorderPaintHandler;

        public LoginDialog(bool requireUsername, string presetUsername = "", Image backgroundImage = null)
        {
            InitializeComponent();
            _usernameBorderPaintHandler = new PaintEventHandler(PnlUsernameBorder_Paint);
            _passwordBorderPaintHandler = new PaintEventHandler(PnlPasswordBorder_Paint);
            pictureBoxAvatar.Image = MakeAvatar();
            SetPlaceholder(txtUsername, "Nhập tên đăng nhập");
            SetPlaceholder(txtPassword, "Nhập mật khẩu");
            txtPassword.UseSystemPasswordChar = false;

            if (!string.IsNullOrEmpty(presetUsername))
            {
                txtUsername.Text = presetUsername;
                txtUsername.ForeColor = Color.Black;
                if (!requireUsername)
                {
                    txtUsername.ReadOnly = true;
                    this.ActiveControl = txtPassword;
                }
            }
            else
            {
                txtUsername.Text = "Nhập tên đăng nhập";
                txtUsername.ForeColor = Color.Gray;
                txtUsername.ReadOnly = false;
            }

            if (backgroundImage != null)
            {
                pictureBoxBackground.Image = backgroundImage;
                pictureBoxBackground.SizeMode = PictureBoxSizeMode.CenterImage;
            }

            txtUsername.GotFocus += TxtUsername_GotFocus;
            txtUsername.LostFocus += TxtUsername_LostFocus;
            txtPassword.GotFocus += TxtPassword_GotFocus;
            txtPassword.LostFocus += TxtPassword_LostFocus;
            picEye.Click += TogglePassword;
            btnOK.Click += BtnOK_Click;
            btnCancel.Click += BtnCancel_Click;
            lblClose.Click += LblClose_Click;
            lblNewTeacherLink.Click += LblNewTeacherLink_Click;
            lblForgotPassword.Click += LblForgotPassword_Click;
            lblNewTeacherLink.MouseEnter += Link_MouseEnter;
            lblNewTeacherLink.MouseLeave += Link_MouseLeave;
            lblForgotPassword.MouseEnter += Link_MouseEnter;
            lblForgotPassword.MouseLeave += Link_MouseLeave;
            pnlUsernameBorder.Paint += _usernameBorderPaintHandler;
            pnlPasswordBorder.Paint += _passwordBorderPaintHandler;
            this.Resize += LoginDialog_Resize;
            SetRoundedRegion(12);
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            string username = this.Username;
            string password = this.Password;

            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập!", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsername.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu!", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            try
            {
                if (username.Equals("admin", StringComparison.OrdinalIgnoreCase))
                {
                    if (DatabaseHelper.CheckAdminLogin(username, password))
                    {
                        Properties.Settings.Default["CurrentUser"] = "Admin";
                        Properties.Settings.Default["isAdmin"] = true;
                        Properties.Settings.Default.Save();
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Sai tài khoản hoặc mật khẩu của Admin!", "Đăng nhập thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }

                LoginStatus status = DatabaseHelper.CheckTeacherLogin(username, password);
                switch (status)
                {
                    case LoginStatus.Success:
                        var profile = DatabaseHelper.GetTeacherProfile(username);
                        Properties.Settings.Default["CurrentUser"] = (profile != null) ? profile.Ten : username;
                        Properties.Settings.Default["isAdmin"] = false;
                        Properties.Settings.Default.Save();
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                        break;
                    case LoginStatus.AccountNotActivated:
                        MessageBox.Show("Tài khoản của bạn đang chờ quản trị viên xác nhận.", "Tài khoản chưa kích hoạt", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    case LoginStatus.InvalidCredentials:
                        MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu!", "Đăng nhập thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi đăng nhập: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TogglePassword(object sender, EventArgs e)
        {
            if (txtPassword.Text == "Nhập mật khẩu") return;
            txtPassword.UseSystemPasswordChar = !txtPassword.UseSystemPasswordChar;
            picEye.Image = MakeEye(!txtPassword.UseSystemPasswordChar);
        }

        private void SetPlaceholder(TextBox tb, string text)
        {
            tb.Text = text;
            tb.ForeColor = Color.Gray;
        }

        private void FocusTextBox(TextBox tb, Panel panel, ref Color borderColor, string placeholder)
        {
            borderColor = Color.RoyalBlue;
            panel.Invalidate();
            if (tb.Text == placeholder && tb.ForeColor == Color.Gray)
            {
                tb.Text = "";
                tb.ForeColor = Color.Black;
                if (tb == txtPassword) txtPassword.UseSystemPasswordChar = true;
            }
        }

        private void UnfocusTextBox(TextBox tb, Panel panel, ref Color borderColor, string placeholder)
        {
            borderColor = Color.Lavender;
            panel.Invalidate();
            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = placeholder;
                tb.ForeColor = Color.Gray;
                if (tb == txtPassword) txtPassword.UseSystemPasswordChar = false;
            }
        }

        private void DrawBorder(Graphics g, Rectangle rect, Color color)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (Pen pen = new Pen(color, 2))
            using (GraphicsPath path = RoundedRect(rect, 8))
            {
                g.Clear(Color.White);
                g.DrawPath(pen, path);
            }
        }

        private GraphicsPath RoundedRect(Rectangle r, int radius)
        {
            int d = radius * 2;
            GraphicsPath path = new GraphicsPath();
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void SetRoundedRegion(int radius)
        {
            if (this.Width > 0 && this.Height > 0)
            {
                GraphicsPath path = RoundedRect(this.ClientRectangle, radius);
                this.Region = new Region(path);
            }
        }

        private Bitmap MakeAvatar()
        {
            int size = 100;
            Bitmap bmp = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (LinearGradientBrush br = new LinearGradientBrush(new Rectangle(0, 0, size, size), Color.FromArgb(120, 200, 220), Color.FromArgb(120, 140, 160), 45f))
                {
                    g.FillEllipse(br, 0, 0, size, size);
                }
                using (Pen p = new Pen(Color.White, 6))
                {
                    g.DrawEllipse(p, 28, 20, 44, 44);
                    g.DrawArc(p, 20, 50, 60, 40, 20, 140);
                }
            }
            return bmp;
        }

        private Bitmap MakeEye(bool open)
        {
            int size = 18;
            Bitmap bmp = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (Pen p = new Pen(Color.DimGray, 2))
                {
                    g.DrawEllipse(p, 2, 5, 14, 8);
                    if (open) g.FillEllipse(Brushes.DimGray, 7, 8, 4, 4);
                    else g.DrawLine(p, 3, 13, 15, 5);
                }
            }
            return bmp;
        }

        private void TxtUsername_GotFocus(object sender, EventArgs e) => FocusTextBox(txtUsername, pnlUsernameBorder, ref usernameBorderColor, "Nhập tên đăng nhập");
        private void TxtUsername_LostFocus(object sender, EventArgs e) => UnfocusTextBox(txtUsername, pnlUsernameBorder, ref usernameBorderColor, "Nhập tên đăng nhập");
        private void TxtPassword_GotFocus(object sender, EventArgs e) => FocusTextBox(txtPassword, pnlPasswordBorder, ref passwordBorderColor, "Nhập mật khẩu");
        private void TxtPassword_LostFocus(object sender, EventArgs e) => UnfocusTextBox(txtPassword, pnlPasswordBorder, ref passwordBorderColor, "Nhập mật khẩu");

        private void BtnCancel_Click(object sender, EventArgs e) => this.Close();
        private void LblClose_Click(object sender, EventArgs e) => this.Close();

        private void PnlUsernameBorder_Paint(object sender, PaintEventArgs e) => DrawBorder(e.Graphics, pnlUsernameBorder.ClientRectangle, usernameBorderColor);
        private void PnlPasswordBorder_Paint(object sender, PaintEventArgs e) => DrawBorder(e.Graphics, pnlPasswordBorder.ClientRectangle, passwordBorderColor);

        private void LoginDialog_Resize(object sender, EventArgs e) => SetRoundedRegion(12);

        private void LblNewTeacherLink_Click(object sender, EventArgs e)
        {
            using (var registrationForm = new TeacherRegistrationForm())
            {
                registrationForm.ShowDialog();
            }
        }

        private void LblForgotPassword_Click(object sender, EventArgs e) => MessageBox.Show("Chức năng này đang được phát triển.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        private void Link_MouseEnter(object sender, EventArgs e) { if (sender is Label label) { label.ForeColor = linkHoverColor; } }
        private void Link_MouseLeave(object sender, EventArgs e) { if (sender is Label label) { label.ForeColor = linkIdleColor; } }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                txtUsername.GotFocus -= TxtUsername_GotFocus;
                txtUsername.LostFocus -= TxtUsername_LostFocus;
                txtPassword.GotFocus -= TxtPassword_GotFocus;
                txtPassword.LostFocus -= TxtPassword_LostFocus;
                this.Resize -= LoginDialog_Resize;
                lblNewTeacherLink.Click -= LblNewTeacherLink_Click;
                lblNewTeacherLink.MouseEnter -= Link_MouseEnter;
                lblNewTeacherLink.MouseLeave -= Link_MouseLeave;
                lblForgotPassword.Click -= LblForgotPassword_Click;
                lblForgotPassword.MouseEnter -= Link_MouseEnter;
                lblForgotPassword.MouseLeave -= Link_MouseLeave;

                if (pnlUsernameBorder != null) pnlUsernameBorder.Paint -= _usernameBorderPaintHandler;
                if (pnlPasswordBorder != null) pnlPasswordBorder.Paint -= _passwordBorderPaintHandler;
                if (components != null) components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}