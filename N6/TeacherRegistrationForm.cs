using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.Windows.Forms;

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

            // Sử dụng phương thức thiết lập control chung
            SetupControl(txtName, pnlNameBorder, "Họ và tên");
            SetupControl(txtUsername, pnlUsernameBorder, "Tên đăng nhập");
            SetupControl(txtEmail, pnlEmailBorder, "Email");
            SetupControl(txtPhone, pnlPhoneBorder, "Số điện thoại");
            SetupControl(txtPassword, pnlPasswordBorder, "Mật khẩu", true);
            SetupControl(txtConfirmPassword, pnlConfirmPasswordBorder, "Xác nhận mật khẩu", true);
            // Đã xóa SetupControl cho cmbSubject

            lblClose.Click += (s, e) => this.Close();
            this.MouseDown += Form_MouseDown;
            this.MouseMove += Form_MouseMove;
            this.MouseUp += Form_MouseUp;
        }

        // Tái cấu trúc thành một hàm SetupControl chung
        private void SetupControl(Control control, Panel pnl, string placeholder = null, bool isPassword = false)
        {
            _borderColors[pnl] = Color.Lavender;
            pnl.Paint += _pnlBorderPaintHandler;
            control.Enter += Control_Enter; // Sử dụng sự kiện Enter
            control.Leave += Control_Leave; // Sử dụng sự kiện Leave

            if (control is TextBox tb)
            {
                tb.Text = placeholder;
                tb.Tag = placeholder;
                tb.ForeColor = Color.Gray;
                if (isPassword)
                {
                    tb.UseSystemPasswordChar = false;
                }
            }
        }

        #region Events (Enter, Leave, Paint) - ĐÃ CẬP NHẬT
        // Sự kiện Enter thay cho GotFocus
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
                // Chỉ thay đổi UseSystemPasswordChar cho các ô mật khẩu
                if (tb == txtPassword || tb == txtConfirmPassword)
                {
                    tb.UseSystemPasswordChar = true;
                }
            }
        }

        // Sự kiện Leave thay cho LostFocus
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
                if (tb.Tag != null)
                {
                    tb.Text = tb.Tag.ToString();
                }

                // Chỉ thay đổi UseSystemPasswordChar cho các ô mật khẩu
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
            // Đã xóa logic tải môn học
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (IsPlaceholder(txtName) || IsPlaceholder(txtUsername) || IsPlaceholder(txtEmail) || IsPlaceholder(txtPhone) || IsPlaceholder(txtPassword))
            {
                MessageBox.Show("Vui lòng điền đầy đủ các thông tin bắt buộc.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!Regex.IsMatch(txtName.Text.Trim(), @"^[\p{L}\s]+$"))
            {
                MessageBox.Show("Họ và tên không hợp lệ. Vui lòng chỉ nhập chữ cái và khoảng trắng.", "Lỗi Định Dạng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Regex.IsMatch(txtEmail.Text.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Địa chỉ email không hợp lệ. Vui lòng nhập lại.", "Lỗi Định Dạng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Regex.IsMatch(txtPhone.Text.Trim(), @"^\d{10}$"))
            {
                MessageBox.Show("Số điện thoại không hợp lệ. Vui lòng nhập chính xác 10 chữ số.", "Lỗi Định Dạng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Mật khẩu và xác nhận mật khẩu không khớp. Vui lòng nhập lại.", "Lỗi Mật khẩu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Đã xóa kiểm tra cmbSubject

            try
            {

                DatabaseHelper.CreateTeacherRequest(
                    txtName.Text.Trim(),
                    txtUsername.Text.Trim(),
                    txtPassword.Text,
                    txtEmail.Text.Trim(),
                    txtPhone.Text.Trim()
                );

                MessageBox.Show("Yêu cầu đã được gửi thành công. Vui lòng chờ admin xác nhận.", "Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        #region UI Helpers
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
                // Dòng g.Clear() đã được bỏ đi, đây là điều đúng đắn
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

        // Cải thiện phương thức Dispose để hủy đăng ký sự kiện, tránh rò rỉ bộ nhớ
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }

                // Hủy đăng ký tất cả các sự kiện đã dùng
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