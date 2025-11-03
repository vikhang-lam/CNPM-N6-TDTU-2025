using System;
using System.Drawing;
using System.Windows.Forms;
using N6;
using System.Collections.Generic; // Thêm
using System.Linq; // Thêm

/// <summary>
/// Form popup hiển thị các công cụ hỗ trợ nhanh (Bảng trắng, Ghi chú, Hẹn giờ).
/// </summary>
public class frmAssistiveMenu : frmDraggableRoundedPopup
{
    private string _maGV;

    // Biến lưu trữ các control động để gỡ sự kiện
    private List<Panel> menuButtons = new List<Panel>();
    private Dictionary<Control, EventHandler> clickHandlers = new Dictionary<Control, EventHandler>();
    private Dictionary<Control, EventHandler> mouseEnterHandlers = new Dictionary<Control, EventHandler>();
    private Dictionary<Control, EventHandler> mouseLeaveHandlers = new Dictionary<Control, EventHandler>();

    [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
    private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, string lParam);
    private const int EM_SETCUEBANNER = 0x1501;

    public frmAssistiveMenu(string maGV) : base()
    {
        _maGV = maGV;

        var tlp = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 1,
            Padding = new Padding(5)
        };

        tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
        tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
        tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
        tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));

        // Tạo các nút
        var btnWhiteboard = CreateMenuButton("Bảng Trắng", "🖊️", BtnWhiteboard_Click);
        var btnNoteTKB = CreateMenuButton("Ghi Chú TKB", "📅", BtnNoteTKB_Click);
        var btnNoteHS = CreateMenuButton("Ghi Chú HS", "👥", BtnNoteHS_Click);
        var btnTimer = CreateMenuButton("Hẹn Giờ", "⏰", BtnTimer_Click);

        // Lưu lại để Dispose
        menuButtons.AddRange(new[] { btnWhiteboard, btnNoteTKB, btnNoteHS, btnTimer });

        tlp.Controls.Add(btnWhiteboard, 0, 0);
        tlp.Controls.Add(btnNoteTKB, 1, 0);
        tlp.Controls.Add(btnNoteHS, 2, 0);
        tlp.Controls.Add(btnTimer, 3, 0);

        this.ContentPanel.Padding = new Padding(5);
        this.ContentPanel.Controls.Add(tlp);
        this.Size = new Size(430, 140);
        this.Text = "Công cụ Hỗ trợ";
    }

    /// <summary>
    /// Tạo một nút bấm (Panel) cho menu.
    /// </summary>
    private Panel CreateMenuButton(string text, string icon, EventHandler mainClickHandler)
    {
        var panel = new RoundedPanel
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(5),
            BackColor = Color.WhiteSmoke,
            CornerRadius = 15,
            Cursor = Cursors.Hand
        };
        var lblIcon = new Label { Text = icon, Dock = DockStyle.Top, Height = 45, Font = new Font("Segoe UI Emoji", 18), TextAlign = ContentAlignment.MiddleCenter, BackColor = Color.Transparent };
        var lblText = new Label { Text = text, Dock = DockStyle.Bottom, Height = 25, Font = new Font("Segoe UI", 9, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter, ForeColor = Color.DimGray, BackColor = Color.Transparent };

        panel.Controls.Add(lblText);
        panel.Controls.Add(lblIcon);

        Action<Color> setBackColor = (color) => { panel.BackColor = color; };

        // Lưu trữ các handler để gỡ bỏ sau
        var enterHandler = (EventHandler)((s, e) => setBackColor(Color.FromArgb(220, 235, 255)));
        var leaveHandler = (EventHandler)((s, e) => setBackColor(Color.WhiteSmoke));

        mouseEnterHandlers[panel] = enterHandler;
        mouseEnterHandlers[lblIcon] = enterHandler;
        mouseEnterHandlers[lblText] = enterHandler;

        mouseLeaveHandlers[panel] = leaveHandler;
        mouseLeaveHandlers[lblIcon] = leaveHandler;
        mouseLeaveHandlers[lblText] = leaveHandler;

        clickHandlers[panel] = mainClickHandler;
        clickHandlers[lblIcon] = mainClickHandler;
        clickHandlers[lblText] = mainClickHandler;

        // Gán sự kiện
        panel.MouseEnter += enterHandler;
        panel.MouseLeave += leaveHandler;
        panel.Click += mainClickHandler;

        lblIcon.MouseEnter += enterHandler;
        lblIcon.MouseLeave += leaveHandler;
        lblIcon.Click += mainClickHandler;

        lblText.MouseEnter += enterHandler;
        lblText.MouseLeave += leaveHandler;
        lblText.Click += mainClickHandler;

        return panel;
    }

    #region Click Handlers (Xử lý sự kiện)

    private void BtnWhiteboard_Click(object sender, EventArgs e)
    {
        var wb = new PopupWhiteboard();
        wb.Show();
        this.Close();
    }

    private void BtnNoteTKB_Click(object sender, EventArgs e)
    {
        var noteForm = new frmGhiChuTKB(_maGV);
        noteForm.Show();
        this.Close();
    }

    private void BtnNoteHS_Click(object sender, EventArgs e)
    {
        var noteForm = new frmGhiChuHocSinh(_maGV);
        noteForm.Show();
        this.Close();
    }

    private void BtnTimer_Click(object sender, EventArgs e)
    {
        using (Form settingsForm = new Form())
        {
            settingsForm.Text = "Cài đặt Hẹn giờ";
            settingsForm.Size = new Size(450, 300);
            settingsForm.StartPosition = FormStartPosition.CenterParent;
            settingsForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            settingsForm.MaximizeBox = false;
            settingsForm.MinimizeBox = false;
            settingsForm.ShowInTaskbar = false;
            settingsForm.TopMost = this.TopMost;

            Label lblTitle = new Label
            {
                Text = "⏰ Cài đặt đồng hồ hẹn giờ",
                Location = new Point(20, 20),
                AutoSize = true,
                Font = new Font("Segoe UI", 13F, FontStyle.Bold)
            };

            Label lblSubject = new Label
            {
                Text = "Tên môn học:",
                Location = new Point(30, 70),
                AutoSize = true,
                Font = new Font("Segoe UI", 10F)
            };

            TextBox txtSubject = new TextBox
            {
                Location = new Point(150, 68),
                Width = 260,
                Font = new Font("Segoe UI", 10F)
            };

            var _ = txtSubject.Handle;
            SendMessage(txtSubject.Handle, EM_SETCUEBANNER, 0, "VD: Toán, Ngữ văn...");

            Label lblDuration = new Label
            {
                Text = "Thời lượng (phút):",
                Location = new Point(30, 110),
                AutoSize = true,
                Font = new Font("Segoe UI", 10F)
            };

            NumericUpDown numDuration = new NumericUpDown
            {
                Location = new Point(150, 108),
                Width = 80,
                Minimum = 5,
                Maximum = 120,
                Value = 45,
                Font = new Font("Segoe UI", 10F)
            };

            Label lblWarning = new Label
            {
                Text = "Cảnh báo trước (phút):",
                Location = new Point(30, 150),
                AutoSize = true,
                Font = new Font("Segoe UI", 10F)
            };

            NumericUpDown numWarning = new NumericUpDown
            {
                Location = new Point(180, 148),
                Width = 50,
                Minimum = 1,
                Maximum = 30, // Sẽ được cập nhật ngay
                Value = 5,
                Font = new Font("Segoe UI", 10F)
            };

            // ### BẮT ĐẦU SỬA LỖI ###

            // 1. Đặt Maximum ban đầu một cách chính xác
            numWarning.Maximum = numDuration.Value;

            // 2. Thêm sự kiện ValueChanged cho numDuration
            numDuration.ValueChanged += (s_duration, e_duration) =>
            {
                // Cập nhật Maximum của numWarning
                numWarning.Maximum = numDuration.Value;

                // (FIX) Chủ động kẹp giá trị của numWarning nếu nó vi phạm Maximum mới
                if (numWarning.Value > numWarning.Maximum)
                {
                    numWarning.Value = numWarning.Maximum;
                }
            };

            // 3. (Tùy chọn) Thêm sự kiện cho numWarning (để đảm bảo an toàn tuyệt đối)
            numWarning.ValueChanged += (s_warning, e_warning) =>
            {
                if (numWarning.Value > numDuration.Value)
                {
                    numWarning.Value = numDuration.Value;
                }
            };

            // ### KẾT THÚC SỬA LỖI ###

            Button btnStart = new Button
            {
                Text = "🕐 Bắt đầu đếm ngược",
                Location = new Point(100, 200),
                Size = new Size(250, 45),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnStart.FlatAppearance.BorderSize = 0;

            btnStart.Click += (s, ev) =>
            {
                string subject = string.IsNullOrWhiteSpace(txtSubject.Text)
                    ? "Tiết học"
                    : txtSubject.Text.Trim();

                int minutes = (int)numDuration.Value;
                int warning = (int)numWarning.Value;

                // Kiểm tra logic một lần cuối trước khi bắt đầu
                if (warning > minutes)
                {
                    MessageBox.Show("Thời gian cảnh báo không thể lớn hơn tổng thời lượng.", "Lỗi Logic", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // Ngăn không cho form chạy
                }

                var timerPopup = new frmTimerPopup(subject, minutes, warning);
                timerPopup.Show();
                settingsForm.Close();
            };

            settingsForm.Controls.AddRange(new Control[]
            {
                lblTitle, lblSubject, txtSubject,
                lblDuration, numDuration,
                lblWarning, numWarning,
                btnStart
            });

            settingsForm.ShowDialog(this);
        }

        this.Close();
    }

    #endregion

    /// <summary>
    /// Dọn dẹp tài nguyên.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Gỡ bỏ tất cả các sự kiện đã gán động
            foreach (var panel in menuButtons)
            {
                foreach (Control child in panel.Controls)
                {
                    if (clickHandlers.ContainsKey(child)) child.Click -= clickHandlers[child];
                    if (mouseEnterHandlers.ContainsKey(child)) child.MouseEnter -= mouseEnterHandlers[child];
                    if (mouseLeaveHandlers.ContainsKey(child)) child.MouseLeave -= mouseLeaveHandlers[child];
                }
                if (clickHandlers.ContainsKey(panel)) panel.Click -= clickHandlers[panel];
                if (mouseEnterHandlers.ContainsKey(panel)) panel.MouseEnter -= mouseEnterHandlers[panel];
                if (mouseLeaveHandlers.ContainsKey(panel)) panel.MouseLeave -= mouseLeaveHandlers[panel];
            }
            // Xóa các dictionary
            menuButtons.Clear();
            clickHandlers.Clear();
            mouseEnterHandlers.Clear();
            mouseLeaveHandlers.Clear();
        }
        base.Dispose(disposing);
    }
}