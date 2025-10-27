using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Diagnostics; // <-- THÊM MỚI

namespace N6
{
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

        private void InitializeModernUI()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            SetRoundedRegion(12);
            pictureBoxIcon.Image = MakeRegisterIcon();

            SetupControl(txtName, pnlNameBorder, "Họ và tên");
            SetupControl(txtUsername, pnlUsernameBorder, "Tên đăng nhập");
            SetupControl(txtEmail, pnlEmailBorder, "Email");
            SetupControl(txtPhone, pnlPhoneBorder, "Số điện thoại");
            SetupControl(txtPassword, pnlPasswordBorder, "Mật khẩu", true);
            SetupControl(txtConfirmPassword, pnlConfirmPasswordBorder, "Xác nhận mật khẩu", true);

            lblClose.Click += (s, e) => this.Close();
            this.MouseDown += Form_MouseDown;
            this.MouseMove += Form_MouseMove;
            this.MouseUp += Form_MouseUp;
        }

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

        #region Events (Enter, Leave, Paint)
        private void Control_Enter(object sender, EventArgs e)
        {
            var control = sender as Control;
            if (control == null) return;
            var pnl = control.Parent;
            _borderColors[pnl] = Color.RoyalBlue;
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

        private void Control_Leave(object sender, EventArgs e)
        {
            var control = sender as Control;
            if (control == null) return;
            var pnl = control.Parent;
            _borderColors[pnl] = Color.Lavender;
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

        // =================================================================
        // ### CẬP NHẬT: btnSubmit_Click (đã sửa để gọi CSDL lấy email Admin) ###
        // =================================================================
        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (IsPlaceholder(txtName) || IsPlaceholder(txtUsername) || IsPlaceholder(txtEmail) || IsPlaceholder(txtPhone) || IsPlaceholder(txtPassword))
            {
                MessageBox.Show("Vui lòng điền đầy đủ các thông tin bắt buộc.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // (Giữ nguyên các kiểm tra Regex...)
            if (!Regex.IsMatch(txtName.Text.Trim(), @"^[\p{L}\s]+$"))
            {
                MessageBox.Show("Họ và tên không hợp lệ...", "Lỗi Định Dạng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!Regex.IsMatch(txtEmail.Text.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Địa chỉ email không hợp lệ...", "Lỗi Định Dạng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!Regex.IsMatch(txtPhone.Text.Trim(), @"^\d{10}$"))
            {
                MessageBox.Show("Số điện thoại không hợp lệ...", "Lỗi Định Dạng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Mật khẩu và xác nhận mật khẩu không khớp.", "Lỗi Mật khẩu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                string teacherName = txtName.Text.Trim();
                string teacherUsername = txtUsername.Text.Trim();
                string teacherEmail = txtEmail.Text.Trim();
                string teacherPhone = txtPhone.Text.Trim();

                // 1. Gửi yêu cầu vào Database
                DatabaseHelper.CreateTeacherRequest(
                    teacherName,
                    teacherUsername,
                    txtPassword.Text,
                    teacherEmail,
                    teacherPhone
                );

                // 2. Gửi email xác nhận cho giáo viên
                EmailHelper.SendRegistrationConfirmationEmail(teacherEmail, teacherName);

                // 3. LẤY EMAIL ADMIN TỪ DATABASE
                // (Giả sử admin chính luôn có mã 'AD001')
                string adminEmailFromDb = DatabaseHelper.GetAdminEmail("AD001");

                // 4. Gửi email thông báo cho Admin (nếu tìm thấy email)
                if (!string.IsNullOrEmpty(adminEmailFromDb))
                {
                    EmailHelper.SendAdminNotificationEmail(
                        adminEmailFromDb, // Truyền email admin vào đây
                        teacherName,
                        teacherUsername,
                        teacherEmail
                    );
                }
                else
                {
                    Debug.WriteLine("CẢNH BÁO: Không tìm thấy email cho Admin 'AD001' trong CSDL.");
                }

                MessageBox.Show("Yêu cầu đã được gửi thành công. Vui lòng kiểm tra email và chờ admin phê duyệt.", "Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi gửi yêu cầu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool IsPlaceholder(TextBox tb)
        {
            return tb.ForeColor == Color.Gray;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        #endregion

        #region UI Helpers (Giữ nguyên)
        private void Form_MouseDown(object sender, MouseEventArgs e) => _lastPoint = new Point(e.X, e.Y);
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
            {
                g.DrawPath(pen, path);
            }
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

        private void SetRoundedRegion(int radius)
        {
            this.Region = new Region(RoundedRect(this.ClientRectangle, radius));
        }

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
                    g.DrawEllipse(p, 26, 18, 28, 28);
                    g.DrawArc(p, 16, 40, 48, 30, 20, 140);
                    g.DrawLine(p, 55, 45, 70, 45);
                    g.DrawLine(p, 62.5f, 38, 62.5f, 52);
                }
            }
            return bmp;
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }

                foreach (var pnl in _borderColors.Keys)
                {
                    pnl.Paint -= _pnlBorderPaintHandler;
                    if (pnl.Controls.Count > 0)
                    {
                        var control = pnl.Controls[0];
                        control.Enter -= Control_Enter;
                        control.Leave -= Control_Leave;
                    }
                }
            }
            base.Dispose(disposing);
        }
    }
}