using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Threading.Tasks; // Thêm
using System.Diagnostics; // Thêm

namespace N6
{
    /// <summary>
    /// Form xử lý logic quên mật khẩu, bao gồm gửi OTP và đặt lại mật khẩu.
    /// </summary>
    public partial class ForgotPasswordForm : Form
    {
        private readonly Dictionary<Control, Color> _borderColors = new Dictionary<Control, Color>();
        private Point _lastPoint;
        private readonly PaintEventHandler _pnlBorderPaintHandler;
        private bool otpAlreadyChecked = false;

        public ForgotPasswordForm()
        {
            InitializeComponent();
            _pnlBorderPaintHandler = new PaintEventHandler(PnlBorder_Paint);
            InitializeModernUI();
        }

        /// <summary>
        /// Cài đặt giao diện không viền, bo góc và gán sự kiện cho các control.
        /// </summary>
        private void InitializeModernUI()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            SetRoundedRegion(12);

            SetupControl(txtUsernameOrEmail, pnlUsernameBorder, "Tên đăng nhập hoặc Email");
            SetupControl(txtOtp, pnlOtpBorder, "Mã OTP (gửi về email)");
            SetupControl(txtNewPassword, pnlPasswordBorder, "Mật khẩu mới", true);
            SetupControl(txtConfirmPassword, pnlConfirmPasswordBorder, "Xác nhận mật khẩu mới", true);

            lblClose.Click += lblClose_Click;
            this.MouseDown += Form_MouseDown;
            this.MouseMove += Form_MouseMove;
            this.MouseUp += Form_MouseUp;
        }

        /// <summary>
        /// Khởi tạo trạng thái ban đầu của Form (ẩn các control nhập OTP).
        /// </summary>
        private void ForgotPasswordForm_Load(object sender, EventArgs e)
        {
            // Đặt chiều cao tối đa của form ngay từ đầu
            this.Height = 440;

            // 1. Phần Username
            pnlUsernameBorder.Visible = true;
            pnlUsernameBorder.Enabled = true;
            btnSendOtp.Visible = true;
            btnSendOtp.Enabled = true;

            // 2. Phần OTP
            pnlOtpBorder.Visible = true;
            pnlOtpBorder.Enabled = true;
            btnCheckOtp.Visible = true;
            btnCheckOtp.Enabled = true;

            // 3. Phần Mật khẩu
            pnlPasswordBorder.Visible = true;
            pnlPasswordBorder.Enabled = false; // Bị làm xám
            pnlConfirmPasswordBorder.Visible = true;
            pnlConfirmPasswordBorder.Enabled = false; // Bị làm xám

        }

        /// <summary>
        /// Xử lý sự kiện click nút "Gửi OTP".
        /// </summary>
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
                    // 2. Gửi email (tác vụ chạy nền)
                    bool emailSent = await Task.Run(() =>
                        EmailHelper.SendOtpEmail(result.Email, result.Otp)
                    );

                    if (emailSent)
                    {
                        MessageBox.Show($"Đã gửi mã OTP đến email: {result.Email}.\nVui lòng kiểm tra (có thể trong Spam).", "Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // 3. Vô hiệu hóa phần gửi OTP
                        txtUsernameOrEmail.Enabled = false;
                        pnlUsernameBorder.BackColor = Color.LightGray;
                        btnSendOtp.Enabled = false; // Đã làm ở trên, nhưng cho chắc chắn
                    }
                    // else: Lỗi đã được ném (throw) và bắt (catch) bởi EmailHelper
                }
                else
                {
                    MessageBox.Show("Không tìm thấy tài khoản giáo viên nào (đã xác nhận) với Tên đăng nhập hoặc Email này.", "Không tìm thấy", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                // Bắt lỗi từ EmailHelper.SendOtpEmail hoặc DatabaseHelper
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine(ex.ToString());
            }
            finally
            {
                // Chỉ bật lại nếu có lỗi VÀ email chưa được gửi
                if (txtUsernameOrEmail.Enabled == true) // Kiểm tra logic này
                {
                    btnSendOtp.Enabled = true;
                }
                btnSendOtp.Text = "Gửi OTP";
            }
        }

        /// <summary>
        /// Xử lý sự kiện click nút "Xác Nhận OTP".
        /// </summary>
        private void btnCheckOtp_Click(object sender, EventArgs e)
        {
            if (otpAlreadyChecked)
            {
                MessageBox.Show("Bạn đã xác nhận OTP thành công rồi, vui lòng nhập mật khẩu để thay đổi.",
                                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtOtp.Text) || txtOtp.Text.Contains("Nhập mã OTP"))
            {
                MessageBox.Show("Vui lòng nhập mã OTP.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string usernameOrEmail = txtUsernameOrEmail.Text.Trim();
                string otp = txtOtp.Text.Trim();

                var status = DatabaseHelper.CheckOtp(usernameOrEmail, otp);

                switch (status)
                {
                    case ResetPasswordStatus.Success:
                        otpAlreadyChecked = true;

                        // KÍCH HOẠT PHẦN MẬT KHẨU
                        pnlPasswordBorder.Enabled = true;
                        pnlConfirmPasswordBorder.Enabled = true;
                        btnResetPassword.Enabled = true;

                        // Cập nhật nút Check OTP
                        btnCheckOtp.Text = "Đã Xác Nhận";
                        btnCheckOtp.BackColor = Color.LightGreen;
                        btnCheckOtp.Enabled = false; // Vô hiệu hóa nút check sau khi thành công
                        break;

                    case ResetPasswordStatus.InvalidOtp:
                        MessageBox.Show("Mã OTP không chính xác. Vui lòng thử lại.", "Lỗi OTP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case ResetPasswordStatus.OtpExpired:
                        MessageBox.Show("Mã OTP đã hết hạn. Vui lòng nhấn 'Gửi OTP' để nhận mã mới.", "Lỗi OTP", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;

                    case ResetPasswordStatus.AccountNotFound:
                        MessageBox.Show("Không tìm thấy tài khoản phù hợp.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi xác thực OTP: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Xử lý sự kiện click nút "Đặt Lại Mật Khẩu".
        /// </summary>
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

                // Gọi DatabaseHelper để xác thực và đổi mật khẩu
                var status = DatabaseHelper.ResetPasswordWithOtp(usernameOrEmail, otp, newPassword);

                // Hiển thị kết quả cho người dùng
                switch (status)
                {
                    case ResetPasswordStatus.Success:
                        MessageBox.Show("Đổi mật khẩu thành công! Vui lòng đăng nhập lại.", "Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                        break;
                    case ResetPasswordStatus.InvalidOtp:
                        MessageBox.Show("Mã OTP không chính xác. Vui lòng thử lại.", "Lỗi OTP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case ResetPasswordStatus.OtpExpired:
                        MessageBox.Show("Mã OTP đã hết hạn. Vui lòng nhấn 'Gửi OTP' để nhận mã mới.", "Lỗi OTP", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    case ResetPasswordStatus.AccountNotFound:
                        MessageBox.Show("Không tìm thấy tài khoản. (Lỗi này không mong muốn)", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi đặt lại mật khẩu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region UI Helpers (Tái sử dụng)

        /// <summary>
        /// Cài đặt placeholder và sự kiện focus cho một control (TextBox) và panel viền của nó.
        /// </summary>
        private void SetupControl(Control control, Panel pnl, string placeholder = null, bool isPassword = false)
        {
            _borderColors[pnl] = Color.Lavender;
            pnl.Paint += _pnlBorderPaintHandler;
            control.Enter += Control_Enter;
            control.Leave += Control_Leave;

            if (control is TextBox tb)
            {
                tb.Text = placeholder;
                tb.Tag = placeholder; // Lưu placeholder vào Tag
                tb.ForeColor = Color.Gray;
                if (isPassword) tb.UseSystemPasswordChar = false;
            }
        }

        /// <summary>
        /// Kiểm tra xem TextBox có đang hiển thị placeholder không.
        /// </summary>
        private bool IsPlaceholder(TextBox tb) => tb.ForeColor == Color.Gray;

        /// <summary>
        /// Xử lý khi control nhận focus (đổi màu viền, xóa placeholder).
        /// </summary>
        private void Control_Enter(object sender, EventArgs e)
        {
            var pnl = (sender as Control)?.Parent;
            if (pnl == null) return;
            _borderColors[pnl] = Color.RoyalBlue;
            pnl.Invalidate(); // Vẽ lại panel

            if (sender is TextBox tb && tb.ForeColor == Color.Gray)
            {
                tb.Text = "";
                tb.ForeColor = Color.Black;
                if (tb == txtNewPassword || tb == txtConfirmPassword)
                    tb.UseSystemPasswordChar = true;
            }
        }

        /// <summary>
        /// Xử lý khi control mất focus (đổi màu viền, hiện lại placeholder nếu rỗng).
        /// </summary>
        private void Control_Leave(object sender, EventArgs e)
        {
            var pnl = (sender as Control)?.Parent;
            if (pnl == null) return;
            _borderColors[pnl] = Color.Lavender;
            pnl.Invalidate(); // Vẽ lại panel

            if (sender is TextBox tb && string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.ForeColor = Color.Gray;
                tb.Text = tb.Tag?.ToString();
                if (tb == txtNewPassword || tb == txtConfirmPassword)
                    tb.UseSystemPasswordChar = false;
            }
        }

        /// <summary>
        /// Sự kiện Paint cho các panel viền (vẽ viền bo góc).
        /// </summary>
        private void PnlBorder_Paint(object sender, PaintEventArgs e)
        {
            if (sender is Panel pnl && _borderColors.ContainsKey(pnl))
                DrawBorder(e.Graphics, pnl.ClientRectangle, _borderColors[pnl]);
        }

        /// <summary>
        /// Ghi lại vị trí chuột khi nhấn xuống (để di chuyển Form).
        /// </summary>
        private void Form_MouseDown(object sender, MouseEventArgs e) => _lastPoint = e.Location;

        /// <summary>
        /// Di chuyển Form theo chuột khi kéo.
        /// </summary>
        private void Form_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - _lastPoint.X;
                this.Top += e.Y - _lastPoint.Y;
            }
        }

        /// <summary>
        /// Xóa vị trí chuột khi nhả chuột.
        /// </summary>
        private void Form_MouseUp(object sender, MouseEventArgs e) => _lastPoint = Point.Empty;

        /// <summary>
        /// Vẽ một đường viền bo góc.
        /// </summary>
        private void DrawBorder(Graphics g, Rectangle rect, Color color)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (var pen = new Pen(color, 2))
            using (var path = RoundedRect(rect, 8))
                g.DrawPath(pen, path);
        }

        /// <summary>
        /// Tạo một đối tượng GraphicsPath hình chữ nhật bo góc.
        /// </summary>
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

        /// <summary>
        /// Áp dụng vùng bo góc cho toàn bộ Form.
        /// </summary>
        private void SetRoundedRegion(int radius) => this.Region = new Region(RoundedRect(this.ClientRectangle, radius));

        private void lblClose_Click(object sender, EventArgs e) => this.Close();

        /// <summary>
        /// Dọn dẹp tài nguyên và gỡ bỏ các trình xử lý sự kiện (event handler) thủ công.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null) components.Dispose();

                // Gỡ bỏ các sự kiện được gán thủ công
                lblClose.Click -= lblClose_Click;
                this.MouseDown -= Form_MouseDown;
                this.MouseMove -= Form_MouseMove;
                this.MouseUp -= Form_MouseUp;

                if (this.btnCheckOtp != null)
                    this.btnCheckOtp.Click -= btnCheckOtp_Click;

                // Gỡ bỏ sự kiện cho các panel viền
                foreach (var pnl in _borderColors.Keys)
                {
                    pnl.Paint -= _pnlBorderPaintHandler;
                    if (pnl.Controls.Count > 0)
                    {
                        // Gỡ sự kiện cho TextBox/Control bên trong panel
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