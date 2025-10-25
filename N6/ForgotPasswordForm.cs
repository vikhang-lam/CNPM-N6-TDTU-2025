using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace N6
{
    // Kế thừa từ TeacherRegistrationForm để dùng chung các hàm UI
    public partial class ForgotPasswordForm : Form
    {
        private readonly Dictionary<Control, Color> _borderColors = new Dictionary<Control, Color>();
        private Point _lastPoint;
        private readonly PaintEventHandler _pnlBorderPaintHandler;

        public ForgotPasswordForm()
        {
            InitializeComponent();
            _pnlBorderPaintHandler = new PaintEventHandler(PnlBorder_Paint);
            InitializeModernUI();
        }

        private void InitializeModernUI()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            SetRoundedRegion(12);

            SetupControl(txtUsernameOrEmail, pnlUsernameBorder, "Tên đăng nhập hoặc Email");
            SetupControl(txtOtp, pnlOtpBorder, "Mã OTP (gửi về email)");
            SetupControl(txtNewPassword, pnlPasswordBorder, "Mật khẩu mới", true);
            SetupControl(txtConfirmPassword, pnlConfirmPasswordBorder, "Xác nhận mật khẩu mới", true);

            lblClose.Click += (s, e) => this.Close();
            this.MouseDown += Form_MouseDown;
            this.MouseMove += Form_MouseMove;
            this.MouseUp += Form_MouseUp;
        }

        private void ForgotPasswordForm_Load(object sender, EventArgs e)
        {
            // Ẩn các control không cần thiết lúc đầu
            pnlOtpBorder.Visible = false;
            pnlPasswordBorder.Visible = false;
            pnlConfirmPasswordBorder.Visible = false;
            btnResetPassword.Visible = false;

            // Điều chỉnh kích thước form
            this.Height = 220;
            btnCancel.Location = new Point(this.Width / 2 - btnCancel.Width / 2, 170);
        }

        private async void btnSendOtp_Click(object sender, EventArgs e)
        {
            if (IsPlaceholder(txtUsernameOrEmail))
            {
                MessageBox.Show("Vui lòng nhập Tên đăng nhập hoặc Email.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string usernameOrEmail = txtUsernameOrEmail.Text.Trim();
            btnSendOtp.Enabled = false;
            btnSendOtp.Text = "Đang gửi...";

            try
            {
                // 1. Gọi DatabaseHelper để tạo OTP và lấy email
                var result = DatabaseHelper.RequestPasswordReset(usernameOrEmail);

                if (result.Success)
                {
                    // 2. Gửi email (đây là phần có thể mất thời gian)
                    bool emailSent = await System.Threading.Tasks.Task.Run(() =>
                        EmailHelper.SendOtpEmail(result.Email, result.Otp)
                    );

                    if (emailSent)
                    {
                        MessageBox.Show($"Đã gửi mã OTP đến email: {result.Email}.\nVui lòng kiểm tra (có thể trong Spam).", "Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // 3. Hiển thị các control bị ẩn
                        ShowResetControls();
                    }
                    else
                    {
                        // Lỗi đã được hiển thị bên trong EmailService
                    }
                }
                else
                {
                    MessageBox.Show("Không tìm thấy tài khoản giáo viên nào (đã xác nhận) với Tên đăng nhập hoặc Email này.", "Không tìm thấy", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSendOtp.Enabled = true;
                btnSendOtp.Text = "Gửi OTP";
            }
        }

        private void ShowResetControls()
        {
            // Hiển thị các control
            pnlOtpBorder.Visible = true;
            pnlPasswordBorder.Visible = true;
            pnlConfirmPasswordBorder.Visible = true;
            btnResetPassword.Visible = true;

            // Di chuyển và đổi kích thước form
            this.Height = 440;
            btnCancel.Location = new Point(315, 370);

            // Ẩn và vô hiệu hóa phần gửi OTP
            txtUsernameOrEmail.Enabled = false;
            btnSendOtp.Enabled = false;
            pnlUsernameBorder.BackColor = Color.LightGray;
        }

        private void btnResetPassword_Click(object sender, EventArgs e)
        {
            if (IsPlaceholder(txtOtp) || IsPlaceholder(txtNewPassword) || IsPlaceholder(txtConfirmPassword))
            {
                MessageBox.Show("Vui lòng nhập OTP và mật khẩu mới.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtNewPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Mật khẩu mới và xác nhận không khớp.", "Lỗi Mật khẩu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                string usernameOrEmail = txtUsernameOrEmail.Text.Trim();
                string otp = txtOtp.Text.Trim();
                string newPassword = txtNewPassword.Text;

                var status = DatabaseHelper.ResetPasswordWithOtp(usernameOrEmail, otp, newPassword);

                switch (status)
                {
                    case DatabaseHelper.ResetPasswordStatus.Success:
                        MessageBox.Show("Đổi mật khẩu thành công! Vui lòng đăng nhập lại.", "Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                        break;
                    case DatabaseHelper.ResetPasswordStatus.InvalidOtp:
                        MessageBox.Show("Mã OTP không chính xác. Vui lòng thử lại.", "Lỗi OTP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case DatabaseHelper.ResetPasswordStatus.OtpExpired:
                        MessageBox.Show("Mã OTP đã hết hạn. Vui lòng nhấn 'Gửi OTP' để nhận mã mới.", "Lỗi OTP", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        // Cân nhắc: có thể reset lại form về trạng thái ban đầu ở đây
                        break;
                    case DatabaseHelper.ResetPasswordStatus.AccountNotFound:
                        MessageBox.Show("Không tìm thấy tài khoản. (Lỗi này không mong muốn)", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi đặt lại mật khẩu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region UI Helpers (Copied from TeacherRegistrationForm)

        private void SetupControl(Control control, Panel pnl, string placeholder = null, bool isPassword = false)
        {
            _borderColors[pnl] = Color.Lavender;
            pnl.Paint += _pnlBorderPaintHandler;
            control.Enter += Control_Enter;
            control.Leave += Control_Leave;

            if (control is TextBox tb)
            {
                tb.Text = placeholder;
                tb.Tag = placeholder;
                tb.ForeColor = Color.Gray;
                if (isPassword) tb.UseSystemPasswordChar = false;
            }
        }

        private bool IsPlaceholder(TextBox tb) => tb.ForeColor == Color.Gray;

        private void Control_Enter(object sender, EventArgs e)
        {
            var pnl = (sender as Control)?.Parent;
            if (pnl == null) return;
            _borderColors[pnl] = Color.RoyalBlue;
            pnl.Invalidate();

            if (sender is TextBox tb && tb.ForeColor == Color.Gray)
            {
                tb.Text = "";
                tb.ForeColor = Color.Black;
                if (tb == txtNewPassword || tb == txtConfirmPassword)
                    tb.UseSystemPasswordChar = true;
            }
        }

        private void Control_Leave(object sender, EventArgs e)
        {
            var pnl = (sender as Control)?.Parent;
            if (pnl == null) return;
            _borderColors[pnl] = Color.Lavender;
            pnl.Invalidate();

            if (sender is TextBox tb && string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.ForeColor = Color.Gray;
                tb.Text = tb.Tag?.ToString();
                if (tb == txtNewPassword || tb == txtConfirmPassword)
                    tb.UseSystemPasswordChar = false;
            }
        }

        private void PnlBorder_Paint(object sender, PaintEventArgs e)
        {
            if (sender is Panel pnl && _borderColors.ContainsKey(pnl))
                DrawBorder(e.Graphics, pnl.ClientRectangle, _borderColors[pnl]);
        }

        private void Form_MouseDown(object sender, MouseEventArgs e) => _lastPoint = e.Location;
        private void Form_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - _lastPoint.X;
                this.Top += e.Y - _lastPoint.Y;
            }
        }
        private void Form_MouseUp(object sender, MouseEventArgs e) => _lastPoint = Point.Empty;

        private void DrawBorder(Graphics g, Rectangle rect, Color color)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (var pen = new Pen(color, 2))
            using (var path = RoundedRect(rect, 8))
                g.DrawPath(pen, path);
        }

        private GraphicsPath RoundedRect(Rectangle r, int radius)
        {
            r.Width--; r.Height--;
            int d = radius * 2;
            var path = new GraphicsPath();
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void SetRoundedRegion(int radius) => this.Region = new Region(RoundedRect(this.ClientRectangle, radius));

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null) components.Dispose();
                foreach (var pnl in _borderColors.Keys)
                {
                    pnl.Paint -= _pnlBorderPaintHandler;
                    if (pnl.Controls.Count > 0)
                    {
                        pnl.Controls[0].Enter -= Control_Enter;
                        pnl.Controls[0].Leave -= Control_Leave;
                    }
                }
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}