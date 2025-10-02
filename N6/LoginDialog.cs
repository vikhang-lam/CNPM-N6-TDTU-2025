using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace N6
{
    public partial class LoginDialog : Form
    {
        public string Username =>
            (txtUsername.Text == "Nhập tên đăng nhập") ? "" : txtUsername.Text.Trim();

        public string Password =>
            (txtPassword.Text == "Nhập mật khẩu") ? "" : txtPassword.Text.Trim();

        private Color usernameBorderColor = Color.RoyalBlue;
        private Color passwordBorderColor = Color.RoyalBlue;

        public LoginDialog(bool requireUsername, string presetUsername = "", Image backgroundImage = null)
        {
            InitializeComponent();

            // Avatar gradient
            pictureBoxAvatar.Image = MakeAvatar();

            // Placeholder
            SetPlaceholder(txtUsername, "Nhập tên đăng nhập");
            SetPlaceholder(txtPassword, "Nhập mật khẩu");
            txtPassword.UseSystemPasswordChar = false;

            // Preset username
            if (!string.IsNullOrEmpty(presetUsername))
            {
                txtUsername.Text = presetUsername;
                txtUsername.ForeColor = Color.Black;
            }
            else
            {
                txtUsername.Text = "Nhập tên đăng nhập";
                txtUsername.ForeColor = Color.Gray;
            }
            txtUsername.ReadOnly = false;

            // Background optional
            if (backgroundImage != null)
            {
                pictureBoxBackground.Image = backgroundImage;
                pictureBoxBackground.SizeMode = PictureBoxSizeMode.CenterImage;
            }

            // Attach handlers
            txtUsername.GotFocus += TxtUsername_GotFocus;
            txtUsername.LostFocus += TxtUsername_LostFocus;

            txtPassword.GotFocus += TxtPassword_GotFocus;
            txtPassword.LostFocus += TxtPassword_LostFocus;

            picEye.Click += TogglePassword;

            btnOK.Click += BtnOK_Click;
            btnCancel.Click += BtnCancel_Click;
            lblClose.Click += LblClose_Click;

            pnlUsernameBorder.Paint += PnlUsernameBorder_Paint;
            pnlPasswordBorder.Paint += PnlPasswordBorder_Paint;

            this.Resize += LoginDialog_Resize;
            SetRoundedRegion(12);
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            string username = this.Username;
            string password = this.Password;

            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập!",
                    "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu!",
                    "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra login giáo viên
            if (DatabaseHelper.CheckTeacherLogin(username, password))
            {
                var profile = DatabaseHelper.GetTeacherProfile(username);
                if (profile != null)
                {
                    Properties.Settings.Default["CurrentUser"] = profile.Ten;       // Lưu tên GV
                    Properties.Settings.Default["CurrentSubject"] = profile.TenMon; // Lưu môn GV
                    Properties.Settings.Default.Save();
                }

                this.DialogResult = DialogResult.OK;
            }
            else if (DatabaseHelper.CheckAdminLogin(username, password))
            {
                // Nếu là admin
                Properties.Settings.Default["CurrentUser"] = "Quản trị viên";
                Properties.Settings.Default["CurrentSubject"] = "Hệ thống";
                Properties.Settings.Default.Save();

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu!",
                    "Đăng nhập thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TogglePassword(object sender, EventArgs e)
        {
            if (txtPassword.UseSystemPasswordChar)
            {
                txtPassword.UseSystemPasswordChar = false;
                picEye.Image = MakeEye(true);
            }
            else
            {
                txtPassword.UseSystemPasswordChar = true;
                picEye.Image = MakeEye(false);
            }
        }

        #region Placeholder + Border
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
            GraphicsPath path = RoundedRect(this.ClientRectangle, radius);
            this.Region = new Region(path);
        }
        #endregion

        #region Avatar + Eye
        private Bitmap MakeAvatar()
        {
            int size = 100;
            Bitmap bmp = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (LinearGradientBrush br = new LinearGradientBrush(
                    new Rectangle(0, 0, size, size),
                    Color.FromArgb(120, 200, 220),   // xám xanh nhạt
                    Color.FromArgb(120, 140, 160),
                    45f))
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
                    if (open)
                        g.FillEllipse(Brushes.DimGray, 7, 8, 4, 4);
                    else
                        g.DrawLine(p, 3, 13, 15, 5);
                }
            }
            return bmp;
        }
        #endregion

        // ===== Event Handlers =====
        private void TxtUsername_GotFocus(object sender, EventArgs e) =>
            FocusTextBox(txtUsername, pnlUsernameBorder, ref usernameBorderColor, "Nhập tên đăng nhập");

        private void TxtUsername_LostFocus(object sender, EventArgs e) =>
            UnfocusTextBox(txtUsername, pnlUsernameBorder, ref usernameBorderColor, "Nhập tên đăng nhập");

        private void TxtPassword_GotFocus(object sender, EventArgs e) =>
            FocusTextBox(txtPassword, pnlPasswordBorder, ref passwordBorderColor, "Nhập mật khẩu");

        private void TxtPassword_LostFocus(object sender, EventArgs e) =>
            UnfocusTextBox(txtPassword, pnlPasswordBorder, ref passwordBorderColor, "Nhập mật khẩu");

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void LblClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void PnlUsernameBorder_Paint(object sender, PaintEventArgs e) =>
            DrawBorder(e.Graphics, pnlUsernameBorder.ClientRectangle, usernameBorderColor);

        private void PnlPasswordBorder_Paint(object sender, PaintEventArgs e) =>
            DrawBorder(e.Graphics, pnlPasswordBorder.ClientRectangle, passwordBorderColor);

        private void LoginDialog_Resize(object sender, EventArgs e) =>
            SetRoundedRegion(12);
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                txtUsername.GotFocus -= TxtUsername_GotFocus;
                txtUsername.LostFocus -= TxtUsername_LostFocus;
                txtPassword.GotFocus -= TxtPassword_GotFocus;
                txtPassword.LostFocus -= TxtPassword_LostFocus;
                pnlUsernameBorder.Paint -= PnlUsernameBorder_Paint;
                pnlPasswordBorder.Paint -= PnlPasswordBorder_Paint;
                this.Resize -= LoginDialog_Resize;
                // Detach các event khác
            }
            base.Dispose(disposing);
        }
    }
}
