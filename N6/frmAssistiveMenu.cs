using System;
using System.Drawing;
using System.Windows.Forms;
using N6;

public class frmAssistiveMenu : frmDraggableRoundedPopup
{
    private string _maGV;

    // P/Invoke for TextBox placeholder on .NET Framework
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

        var btnWhiteboard = CreateMenuButton("Bảng Trắng", "🖊️");
        var btnNoteTKB = CreateMenuButton("Ghi Chú TKB", "📅");
        var btnNoteHS = CreateMenuButton("Ghi Chú HS", "👥");
        var btnTimer = CreateMenuButton("Hẹn Giờ", "⏰");

        btnWhiteboard.Click += BtnWhiteboard_Click;
        btnNoteTKB.Click += BtnNoteTKB_Click;
        btnNoteHS.Click += BtnNoteHS_Click;
        btnTimer.Click += BtnTimer_Click;

        foreach (Control c in btnWhiteboard.Controls) c.Click += BtnWhiteboard_Click;
        foreach (Control c in btnNoteTKB.Controls) c.Click += BtnNoteTKB_Click;
        foreach (Control c in btnNoteHS.Controls) c.Click += BtnNoteHS_Click;
        foreach (Control c in btnTimer.Controls) c.Click += BtnTimer_Click;

        tlp.Controls.Add(btnWhiteboard, 0, 0);
        tlp.Controls.Add(btnNoteTKB, 1, 0);
        tlp.Controls.Add(btnNoteHS, 2, 0);
        tlp.Controls.Add(btnTimer, 3, 0);

        this.ContentPanel.Padding = new Padding(5);
        this.ContentPanel.Controls.Add(tlp);
        this.Size = new Size(430, 140);
        this.Text = "Công cụ Hỗ trợ";
    }

    private Panel CreateMenuButton(string text, string icon)
    {
        var panel = new RoundedPanel
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(5),
            BackColor = Color.WhiteSmoke,
            CornerRadius = 15,
            Cursor = Cursors.Hand
        };
        var lblIcon = new Label
        {
            Text = icon,
            Dock = DockStyle.Top,
            Height = 45,
            Font = new Font("Segoe UI Emoji", 18),
            TextAlign = ContentAlignment.MiddleCenter,
            BackColor = Color.Transparent
        };
        var lblText = new Label
        {
            Text = text,
            Dock = DockStyle.Bottom,
            Height = 25,
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Color.DimGray,
            BackColor = Color.Transparent
        };
        panel.Controls.Add(lblText);
        panel.Controls.Add(lblIcon);

        Action<Color> setBackColor = (color) => { panel.BackColor = color; };

        panel.MouseEnter += (s, e) => setBackColor(Color.FromArgb(220, 235, 255));
        panel.MouseLeave += (s, e) => setBackColor(Color.WhiteSmoke);
        lblIcon.MouseEnter += (s, e) => setBackColor(Color.FromArgb(220, 235, 255));
        lblIcon.MouseLeave += (s, e) => setBackColor(Color.WhiteSmoke);
        lblText.MouseEnter += (s, e) => setBackColor(Color.FromArgb(220, 235, 255));
        lblText.MouseLeave += (s, e) => setBackColor(Color.WhiteSmoke);

        return panel;
    }

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
            settingsForm.StartPosition = FormStartPosition.CenterParent; // will work once owner is set
            settingsForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            settingsForm.MaximizeBox = false;
            settingsForm.MinimizeBox = false;
            settingsForm.ShowInTaskbar = false;
            settingsForm.TopMost = this.TopMost; // ensure above owner which is TopMost

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
            // Cue banner for .NET Framework
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
                Maximum = 30,
                Value = 5,
                Font = new Font("Segoe UI", 10F)
            };

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

                var timerPopup = new frmTimerPopup(subject, minutes, warning);
                timerPopup.Show(); // frmTimerPopup is TopMost via base
                settingsForm.Close();
            };

            settingsForm.Controls.AddRange(new Control[]
            {
                lblTitle, lblSubject, txtSubject,
                lblDuration, numDuration,
                lblWarning, numWarning,
                btnStart
            });

            // IMPORTANT: pass owner so dialog is above the AssistiveMenu
            settingsForm.ShowDialog(this);
        }

        this.Close();
    }
}