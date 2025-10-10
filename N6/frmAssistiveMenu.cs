using System;
using System.Drawing;
using System.Windows.Forms;
using N6;

// Kế thừa từ frmDraggableRoundedPopup để có sẵn nền trắng, bo tròn và kéo thả
public class frmAssistiveMenu : frmDraggableRoundedPopup
{
    private string _maGV;

    public frmAssistiveMenu(string maGV) : base() // Gọi constructor của lớp cha
    {
        _maGV = maGV;

        // ====> CÁC DÒNG GÂY LỖI ĐÃ ĐƯỢC XÓA <====
        // this.BackColor = Color.White; // Không cần thiết, lớp cha đã làm
        // this.TransparencyKey = Color.Transparent; // Dòng này gây ra lỗi trong suốt

        var tlp = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 1,
            Padding = new Padding(5)
        };
        tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
        tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
        tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));

        var btnWhiteboard = CreateMenuButton("Bảng Trắng", "🖊️");
        var btnNoteTKB = CreateMenuButton("Ghi Chú TKB", "📅");
        var btnNoteHS = CreateMenuButton("Ghi Chú HS", "👥");

        btnWhiteboard.Click += BtnWhiteboard_Click;
        btnNoteTKB.Click += BtnNoteTKB_Click;
        btnNoteHS.Click += BtnNoteHS_Click;

        // Gán sự kiện click cho cả Panel và các Label bên trong
        foreach (Control c in btnWhiteboard.Controls) c.Click += BtnWhiteboard_Click;
        foreach (Control c in btnNoteTKB.Controls) c.Click += BtnNoteTKB_Click;
        foreach (Control c in btnNoteHS.Controls) c.Click += BtnNoteHS_Click;

        tlp.Controls.Add(btnWhiteboard, 0, 0);
        tlp.Controls.Add(btnNoteTKB, 1, 0);
        tlp.Controls.Add(btnNoteHS, 2, 0);

        this.ContentPanel.Padding = new Padding(5);
        this.ContentPanel.Controls.Add(tlp);
        this.Size = new Size(330, 140);
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
        var lblIcon = new Label { Text = icon, Dock = DockStyle.Top, Height = 45, Font = new Font("Segoe UI Emoji", 18), TextAlign = ContentAlignment.MiddleCenter, BackColor = Color.Transparent };
        var lblText = new Label { Text = text, Dock = DockStyle.Bottom, Height = 25, Font = new Font("Segoe UI", 9, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter, ForeColor = Color.DimGray, BackColor = Color.Transparent };
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
        var noteForm = new frmGhiChuHocSinh(_maGV, DatabaseHelper.GetMonByTeacher(_maGV));
        noteForm.Show();
        this.Close();
    }
}