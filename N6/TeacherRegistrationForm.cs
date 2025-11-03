using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace N6
{
    /// <summary>
    /// Form cho phép giáo viên mới gửi yêu cầu đăng ký tài khoản.
    /// </summary>
    public partial class TeacherRegistrationForm : Form
    {
        private readonly Dictionary<Control, Color> _borderColors = new Dictionary<Control, Color>();
        private Point _lastPoint;
        private readonly PaintEventHandler _pnlBorderPaintHandler;

        public TeacherRegistrationForm()
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
            pictureBoxIcon.Image = MakeRegisterIcon();

            // Cài đặt placeholder và viền cho các TextBox
            SetupControl(txtName, pnlNameBorder, "Họ và tên");
            SetupControl(txtUsername, pnlUsernameBorder, "Tên đăng nhập");
            SetupControl(txtEmail, pnlEmailBorder, "Email");
            SetupControl(txtPhone, pnlPhoneBorder, "Số điện thoại");
            SetupControl(txtPassword, pnlPasswordBorder, "Mật khẩu", true);
            SetupControl(txtConfirmPassword, pnlConfirmPasswordBorder, "Xác nhận mật khẩu", true);

            // Gán sự kiện cho các control
            lblClose.Click += lblClose_Click;
            this.MouseDown += Form_MouseDown;
            this.MouseMove += Form_MouseMove;
            this.MouseUp += Form_MouseUp;
        }

        /// <summary>
        /// Gán sự kiện và placeholder cho một control và panel viền của nó.
        /// </summary>
        private void SetupControl(Control control, Panel pnl, string placeholder = null, bool isPassword = false)
        {
            _borderColors[pnl] = Color.Lavender; // Màu viền mặc định
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

        #region Events (Enter, Leave, Paint)

        /// <summary>
        /// Xử lý khi control nhận focus: đổi màu viền, xóa placeholder.
        /// </summary>
        private void Control_Enter(object sender, EventArgs e)
        {
            var control = sender as Control;
            if (control == null) return;
            var pnl = control.Parent;
            _borderColors[pnl] = Color.RoyalBlue; // Màu viền khi focus
            pnl.Invalidate();

            if (control is TextBox tb && tb.ForeColor == Color.Gray)
            {
                tb.Text = "";
                tb.ForeColor = Color.Black;
                if (tb == txtPassword || tb == txtConfirmPassword)
                {
                    tb.UseSystemPasswordChar = true;
                }
            }
        }

        /// <summary>
        /// Xử lý khi control mất focus: đổi màu viền, hiện lại placeholder nếu rỗng.
        /// </summary>
        private void Control_Leave(object sender, EventArgs e)
        {
            var control = sender as Control;
            if (control == null) return;
            var pnl = control.Parent;
            _borderColors[pnl] = Color.Lavender; // Màu viền mặc định
            pnl.Invalidate();

            if (control is TextBox tb && string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.ForeColor = Color.Gray;
                if (tb.Tag != null) tb.Text = tb.Tag.ToString();
                if (tb == txtPassword || tb == txtConfirmPassword)
                {
                    tb.UseSystemPasswordChar = false;
                }
            }
        }

        /// <summary>
        /// Sự kiện Paint cho các panel viền (để vẽ viền bo góc).
        /// </summary>
        private void PnlBorder_Paint(object sender, PaintEventArgs e)
        {
            var pnl = sender as Panel;
            if (pnl != null && _borderColors.ContainsKey(pnl))
            {
                DrawBorder(e.Graphics, pnl.ClientRectangle, _borderColors[pnl]);
            }
        }
        #endregion

        #region Form Loading and Submission

        private void TeacherRegistrationForm_Load(object sender, EventArgs e)
        {
            // (Không có logic load)
        }

        /// <summary>
        /// Xử lý sự kiện click nút "Gửi Yêu Cầu".
        /// </summary>
        private async void btnSubmit_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra các ô nhập liệu
            if (IsPlaceholder(txtName) || IsPlaceholder(txtUsername) || IsPlaceholder(txtEmail) || IsPlaceholder(txtPhone) || IsPlaceholder(txtPassword))
            {
                MessageBox.Show("Vui lòng điền đầy đủ các thông tin bắt buộc.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Kiểm tra định dạng (Regex)
            if (!Regex.IsMatch(txtName.Text.Trim(), @"^[\p{L}\s]+$"))
            {
                MessageBox.Show("Họ và tên không hợp lệ (chỉ chấp nhận chữ cái và khoảng trắng).", "Lỗi Định Dạng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!Regex.IsMatch(txtEmail.Text.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Địa chỉ email không hợp lệ.", "Lỗi Định Dạng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!Regex.IsMatch(txtPhone.Text.Trim(), @"^\d{10}$"))
            {
                MessageBox.Show("Số điện thoại không hợp lệ (phải là 10 chữ số).", "Lỗi Định Dạng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Mật khẩu và xác nhận mật khẩu không khớp.", "Lỗi Mật khẩu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 3. Xử lý logic nghiệp vụ
            btnSubmit.Enabled = false;
            btnSubmit.Text = "Đang xử lý...";
            try
            {
                string teacherName = txtName.Text.Trim();
                string teacherUsername = txtUsername.Text.Trim();
                string teacherEmail = txtEmail.Text.Trim();
                string teacherPhone = txtPhone.Text.Trim();

                // 3.1. Gửi yêu cầu vào Database (Lớp Dữ liệu)
                DatabaseHelper.CreateTeacherRequest(
                    teacherName,
                    teacherUsername,
                    txtPassword.Text,
                    teacherEmail,
                    teacherPhone
                );

                // 3.2. Gửi email xác nhận cho giáo viên (Lớp Nghiệp vụ Email)
                // Chạy bất đồng bộ để không treo UI
                await System.Threading.Tasks.Task.Run(() =>
                    EmailHelper.SendRegistrationConfirmationEmail(teacherEmail, teacherName)
                );

                // 3.3. LẤY EMAIL ADMIN TỪ DATABASE (Giả sử admin chính luôn có mã 'AD001')
                string adminEmailFromDb = DatabaseHelper.GetAdminEmail("AD001");

                // 3.4. Gửi email thông báo cho Admin (nếu tìm thấy email)
                if (!string.IsNullOrEmpty(adminEmailFromDb))
                {
                    await System.Threading.Tasks.Task.Run(() =>
                        EmailHelper.SendAdminNotificationEmail(
                            adminEmailFromDb,
                            teacherName,
                            teacherUsername,
                            teacherEmail
                        )
                    );
                }
                else
                {
                    // Ghi log lỗi nếu không tìm thấy email admin, nhưng không báo cho người dùng
                    Debug.WriteLine("CẢNH BÁO: Không tìm thấy email cho Admin 'AD001' trong CSDL.");
                }

                MessageBox.Show("Yêu cầu đã được gửi thành công. Vui lòng kiểm tra email và chờ admin phê duyệt.", "Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                // Bắt lỗi từ DatabaseHelper hoặc EmailHelper
                MessageBox.Show("Đã xảy ra lỗi khi gửi yêu cầu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnSubmit.Enabled = true;
                btnSubmit.Text = "Gửi Yêu Cầu";
            }
        }

        /// <summary>
        /// Kiểm tra xem TextBox có đang hiển thị placeholder không.
        /// </summary>
        private bool IsPlaceholder(TextBox tb)
        {
            return tb.ForeColor == Color.Gray;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void lblClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region UI Helpers (Di chuyển Form, Vẽ vời)

        /// <summary>
        /// Ghi lại vị trí chuột khi nhấn xuống (để di chuyển Form).
        /// </summary>
        private void Form_MouseDown(object sender, MouseEventArgs e) => _lastPoint = new Point(e.X, e.Y);

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
            {
                g.DrawPath(pen, path);
            }
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
        private void SetRoundedRegion(int radius)
        {
            this.Region = new Region(RoundedRect(this.ClientRectangle, radius));
        }

        /// <summary>
        /// Tạo icon đăng ký bằng GDI+.
        /// </summary>
        private Bitmap MakeRegisterIcon()
        {
            int size = 80;
            var bmp = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                var rect = new Rectangle(0, 0, size, size);
                using (var br = new LinearGradientBrush(rect, Color.FromArgb(230, 245, 255), Color.FromArgb(200, 230, 250), 45f))
                {
                    g.FillEllipse(br, rect);
                }
                using (var p = new Pen(Color.RoyalBlue, 4))
                {
                    p.StartCap = LineCap.Round;
                    p.EndCap = LineCap.Round;
                    g.DrawEllipse(p, 26, 18, 28, 28); // Đầu
                    g.DrawArc(p, 16, 40, 48, 30, 20, 140); // Thân
                    g.DrawLine(p, 55, 45, 70, 45); // Dấu cộng ngang
                    g.DrawLine(p, 62.5f, 38, 62.5f, 52); // Dấu cộng dọc
                }
            }
            return bmp;
        }
        #endregion

        /// <summary>
        /// Dọn dẹp tài nguyên và gỡ bỏ các trình xử lý sự kiện.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }

                // CHUẨN HÓA: Gỡ bỏ các sự kiện gán thủ công
                lblClose.Click -= lblClose_Click;
                this.MouseDown -= Form_MouseDown;
                this.MouseMove -= Form_MouseMove;
                this.MouseUp -= Form_MouseUp;

                // Dọn dẹp các sự kiện của panel
                foreach (var pnl in _borderColors.Keys.ToList()) // Dùng ToList() để tránh lỗi thay đổi collection
                {
                    pnl.Paint -= _pnlBorderPaintHandler;
                    if (pnl.Controls.Count > 0)
                    {
                        var control = pnl.Controls[0];
                        control.Enter -= Control_Enter;
                        control.Leave -= Control_Leave;
                    }
                }
                _borderColors.Clear();
            }
            base.Dispose(disposing);
        }
    }
}