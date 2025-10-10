using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

public partial class InsightCard : UserControl
{
    public InsightCard()
    {
        InitializeComponent();
        this.Paint += new PaintEventHandler(Card_Paint);
        this.Resize += (s, e) => this.Invalidate();
    }

    // Vẽ viền bo tròn
    private void Card_Paint(object sender, PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        using (var pen = new Pen(Color.FromArgb(220, 220, 220), 1))
        using (var path = CreateRoundedRectanglePath(this.ClientRectangle, 20))
        {
            this.Region = new Region(path);
            e.Graphics.DrawPath(pen, path);
        }
    }

    private GraphicsPath CreateRoundedRectanglePath(Rectangle bounds, int cornerRadius)
    {
        GraphicsPath path = new GraphicsPath();
        bounds.Width--; bounds.Height--;
        path.AddArc(bounds.X, bounds.Y, cornerRadius, cornerRadius, 180, 90);
        path.AddArc(bounds.Right - cornerRadius, bounds.Y, cornerRadius, cornerRadius, 270, 90);
        path.AddArc(bounds.Right - cornerRadius, bounds.Bottom - cornerRadius, cornerRadius, cornerRadius, 0, 90);
        path.AddArc(bounds.X, bounds.Bottom - cornerRadius, cornerRadius, cornerRadius, 90, 90);
        path.CloseFigure();
        return path;
    }

    public void SetData(string icon, string title, Color color, List<HocSinhAnalysisResult> students)
    {
        lblIcon.Text = icon;
        lblTitle.Text = title;
        this.BackColor = color;
        rtbContent.BackColor = color;

        rtbContent.Clear();
        if (students == null || students.Count == 0)
        {
            rtbContent.AppendText("Không có học sinh nào trong danh sách này.");
            return;
        }

        foreach (var student in students)
        {
            rtbContent.SelectionFont = new Font("Segoe UI", 9, FontStyle.Bold);
            rtbContent.AppendText(student.HoTen);
            rtbContent.SelectionFont = new Font("Segoe UI", 8, FontStyle.Regular);
            rtbContent.SelectionColor = Color.DimGray;
            rtbContent.AppendText($" ({student.TenLop})\n");
            rtbContent.SelectionFont = new Font("Segoe UI", 8, FontStyle.Italic);
            rtbContent.SelectionColor = Color.DarkSlateGray;
            rtbContent.AppendText($"   Lý do: {student.LyDo}\n\n");
        }
    }
}