using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace N6
{
    public partial class TeacherRegistrationForm : Form
    {
        private Dictionary<Control, Color> borderColors = new Dictionary<Control, Color>();
        private Point lastPoint;

        // === FIX START: Khai báo trường để giữ tham chiếu đến delegate Paint ===
        private readonly PaintEventHandler _pnlBorderPaintHandler;
        // === FIX END ===

        public TeacherRegistrationForm()
        {
            InitializeComponent();

            // === FIX START: Khởi tạo delegate một lần ===
            _pnlBorderPaintHandler = new PaintEventHandler(PnlBorder_Paint);
            // === FIX END ===

            InitializeModernUI();
        }

        private void InitializeModernUI()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            SetRoundedRegion(12);
            pictureBoxIcon.Image = MakeRegisterIcon();

            SetupTextBox(txtName, "Họ và tên", pnlNameBorder);
            SetupTextBox(txtUsername, "Tên đăng nhập", pnlUsernameBorder);
            SetupTextBox(txtEmail, "Email", pnlEmailBorder);
            SetupTextBox(txtPhone, "Số điện thoại", pnlPhoneBorder);
            SetupTextBox(txtPassword, "Mật khẩu", pnlPasswordBorder, true);
            SetupTextBox(txtConfirmPassword, "Xác nhận mật khẩu", pnlConfirmPasswordBorder, true);
            SetupComboBox(cmbSubject, pnlSubjectBorder);

            lblClose.Click += (s, e) => this.Close();
            this.MouseDown += Form_MouseDown;
            this.MouseMove += Form_MouseMove;
            this.MouseUp += Form_MouseUp;
        }

        private void SetupTextBox(TextBox tb, string placeholder, Panel pnl, bool isPassword = false)
        {
            borderColors[pnl] = Color.Lavender;
            tb.Text = placeholder;
            tb.ForeColor = Color.Gray;
            if (isPassword) tb.UseSystemPasswordChar = false;

            tb.GotFocus += TextBox_GotFocus;
            tb.LostFocus += TextBox_LostFocus;

            // === FIX START: Sử dụng delegate đã được lưu trữ ===
            pnl.Paint += _pnlBorderPaintHandler;
            // === FIX END ===
        }

        private void SetupComboBox(ComboBox cb, Panel pnl)
        {
            borderColors[pnl] = Color.Lavender;
            cb.GotFocus += ComboBox_GotFocus;
            cb.LostFocus += ComboBox_LostFocus;

            // === FIX START: Sử dụng delegate đã được lưu trữ ===
            pnl.Paint += _pnlBorderPaintHandler;
            // === FIX END ===
        }

        #region Events (GotFocus, LostFocus, Paint)
        private void TextBox_GotFocus(object sender, EventArgs e)
        {
            var tb = sender as TextBox;
            var pnl = tb.Parent;
            borderColors[pnl] = Color.RoyalBlue;
            pnl.Invalidate();

            if (tb.ForeColor == Color.Gray)
            {
                tb.Text = "";
                tb.ForeColor = Color.Black;
                if (tb == txtPassword || tb == txtConfirmPassword)
                {
                    tb.UseSystemPasswordChar = true;
                }
            }
        }

        private void TextBox_LostFocus(object sender, EventArgs e)
        {
            var tb = sender as TextBox;
            var pnl = tb.Parent;
            borderColors[pnl] = Color.Lavender;
            pnl.Invalidate();

            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.ForeColor = Color.Gray;
                tb.Text = tb.Tag.ToString();
                if (tb == txtPassword || tb == txtConfirmPassword)
                {
                    tb.UseSystemPasswordChar = false;
                }
            }
        }

        private void ComboBox_GotFocus(object sender, EventArgs e)
        {
            var pnl = (sender as ComboBox).Parent;
            borderColors[pnl] = Color.RoyalBlue;
            pnl.Invalidate();
        }

        private void ComboBox_LostFocus(object sender, EventArgs e)
        {
            var pnl = (sender as ComboBox).Parent;
            borderColors[pnl] = Color.Lavender;
            pnl.Invalidate();
        }

        private void PnlBorder_Paint(object sender, PaintEventArgs e)
        {
            var pnl = sender as Panel;
            if (pnl != null && borderColors.ContainsKey(pnl))
            {
                DrawBorder(e.Graphics, pnl.ClientRectangle, borderColors[pnl]);
            }
        }
        #endregion

        #region Form Loading and Submission
        private void TeacherRegistrationForm_Load(object sender, EventArgs e)
        {
            txtName.Tag = "Họ và tên";
            txtUsername.Tag = "Tên đăng nhập";
            txtEmail.Tag = "Email";
            txtPhone.Tag = "Số điện thoại";
            txtPassword.Tag = "Mật khẩu";
            txtConfirmPassword.Tag = "Xác nhận mật khẩu";

            DataTable dtSubjects = DatabaseHelper.GetAllMonHoc();
            DataRow placeholder = dtSubjects.NewRow();
            placeholder["MaMon"] = "";
            placeholder["TenMon"] = "Chọn môn học...";
            dtSubjects.Rows.InsertAt(placeholder, 0);

            cmbSubject.DataSource = dtSubjects;
            cmbSubject.DisplayMember = "TenMon";
            cmbSubject.ValueMember = "MaMon";
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (IsPlaceholder(txtName) || IsPlaceholder(txtUsername) || IsPlaceholder(txtEmail) || IsPlaceholder(txtPhone) || IsPlaceholder(txtPassword))
            {
                MessageBox.Show("Vui lòng điền đầy đủ các thông tin bắt buộc.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Mật khẩu và xác nhận mật khẩu không khớp. Vui lòng nhập lại.", "Lỗi Mật khẩu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (cmbSubject.SelectedIndex <= 0)
            {
                MessageBox.Show("Vui lòng chọn môn học.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                DatabaseHelper.CreateTeacherRequest(
                    txtName.Text.Trim(),
                    txtUsername.Text.Trim(),
                    txtPassword.Text,
                    cmbSubject.SelectedValue.ToString(),
                    txtEmail.Text.Trim(),
                    txtPhone.Text.Trim()
                );

                MessageBox.Show("Yêu cầu đã được gửi thành công. Vui lòng chờ admin xác nhận.", "Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        private void Form_MouseDown(object sender, MouseEventArgs e) => lastPoint = new Point(e.X, e.Y);
        private void Form_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - lastPoint.X;
                this.Top += e.Y - lastPoint.Y;
            }
        }
        private void Form_MouseUp(object sender, MouseEventArgs e) => lastPoint = Point.Empty;

        private void DrawBorder(Graphics g, Rectangle rect, Color color)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (Pen pen = new Pen(color, 2))
            using (GraphicsPath path = RoundedRect(rect, 8))
            {
                g.Clear(this.BackColor);
                g.DrawPath(pen, path);
            }
        }

        private GraphicsPath RoundedRect(Rectangle r, int radius)
        {
            r.Width--; r.Height--;
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
            this.Region = new Region(RoundedRect(this.ClientRectangle, radius));
        }

        private Bitmap MakeRegisterIcon()
        {
            int size = 80;
            Bitmap bmp = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (var br = new LinearGradientBrush(new Rectangle(0, 0, size, size), Color.FromArgb(230, 245, 255), Color.FromArgb(200, 230, 250), 45f))
                {
                    g.FillEllipse(br, 0, 0, size, size);
                }
                using (Pen p = new Pen(Color.RoyalBlue, 4))
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

        // === FIX START: Ghi đè Dispose để hủy đăng ký sự kiện ===
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Hủy đăng ký sự kiện Paint cho tất cả các panel
                pnlNameBorder.Paint -= _pnlBorderPaintHandler;
                pnlUsernameBorder.Paint -= _pnlBorderPaintHandler;
                pnlEmailBorder.Paint -= _pnlBorderPaintHandler;
                pnlPhoneBorder.Paint -= _pnlBorderPaintHandler;
                pnlPasswordBorder.Paint -= _pnlBorderPaintHandler;
                pnlConfirmPasswordBorder.Paint -= _pnlBorderPaintHandler;
                pnlSubjectBorder.Paint -= _pnlBorderPaintHandler;

                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        // === FIX END ===
    }
}