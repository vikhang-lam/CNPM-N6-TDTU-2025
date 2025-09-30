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

        private Color usernameBorderColor = Color.LightPink;
        private Color passwordBorderColor = Color.LightPink;

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
            if (requireUsername)
            {
                txtUsername.ReadOnly = false;
                txtUsername.Text = "";
            }
            else
            {
                txtUsername.ReadOnly = true;
                txtUsername.Text = presetUsername;
                txtUsername.ForeColor = Color.Black;
            }

            // Background optional
            if (backgroundImage != null)
            {
                pictureBoxBackground.Image = backgroundImage;
                pictureBoxBackground.SizeMode = PictureBoxSizeMode.CenterImage;
            }

            // Events
            txtUsername.GotFocus += (s, e) => FocusTextBox(txtUsername, pnlUsernameBorder, ref usernameBorderColor, "Nhập tên đăng nhập");
            txtUsername.LostFocus += (s, e) => UnfocusTextBox(txtUsername, pnlUsernameBorder, ref usernameBorderColor, "Nhập tên đăng nhập");

            txtPassword.GotFocus += (s, e) => FocusTextBox(txtPassword, pnlPasswordBorder, ref passwordBorderColor, "Nhập mật khẩu");
            txtPassword.LostFocus += (s, e) => UnfocusTextBox(txtPassword, pnlPasswordBorder, ref passwordBorderColor, "Nhập mật khẩu");

            picEye.Click += TogglePassword;

            btnOK.Click += BtnOK_Click;
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            lblClose.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            pnlUsernameBorder.Paint += (s, e) => DrawBorder(e.Graphics, pnlUsernameBorder.ClientRectangle, usernameBorderColor);
            pnlPasswordBorder.Paint += (s, e) => DrawBorder(e.Graphics, pnlPasswordBorder.ClientRectangle, passwordBorderColor);

            this.Resize += (s, e) => SetRoundedRegion(12);
            SetRoundedRegion(12);
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPassword.Text) || txtPassword.Text == "Nhập mật khẩu")
            {
                MessageBox.Show("Vui lòng nhập mật khẩu!",
                    "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || txtUsername.Text == "Nhập tên đăng nhập")
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập!",
                    "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
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
            borderColor = Color.DeepPink;
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
            borderColor = Color.LightPink;
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
                    Color.FromArgb(155, 81, 224),
                    Color.FromArgb(244, 143, 177),
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

        
        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x84;
            const int HTCLIENT = 1;
            const int HTCAPTION = 2;

            if (m.Msg == WM_NCHITTEST)
            {
                base.WndProc(ref m);
                if ((int)m.Result == HTCLIENT)
                    m.Result = (IntPtr)HTCAPTION;
                return;
            }
            base.WndProc(ref m);
        }
    }
}
