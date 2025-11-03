using System; // Thêm
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

/// <summary>
/// Một UserControl tùy chỉnh (Thẻ) để hiển thị kết quả phân tích AI.
/// Hỗ trợ bo góc và hiển thị danh sách học sinh.
/// </summary>
public partial class InsightCard : UserControl
{
    // Biến lưu trữ sự kiện để gỡ bỏ
    private PaintEventHandler cardPaintHandler;
    private EventHandler resizeHandler;

    public InsightCard()
    {
        InitializeComponent();

        // Lưu trữ handler để gỡ bỏ trong Dispose
        cardPaintHandler = new PaintEventHandler(Card_Paint);
        resizeHandler = (s, e) => this.Invalidate(); // Invalidate khi resize

        // Gán sự kiện
        this.Paint += cardPaintHandler;
        this.Resize += resizeHandler;
    }

    /// <summary>
    /// Tùy chỉnh việc vẽ vời của UserControl để tạo hiệu ứng bo góc.
    /// </summary>
    private void Card_Paint(object sender, PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        using (var pen = new Pen(Color.FromArgb(220, 220, 220), 1))
        using (var path = CreateRoundedRectanglePath(this.ClientRectangle, 20))
        {
            // Áp dụng bo góc cho control
            this.Region = new Region(path);
            // Vẽ đường viền
            e.Graphics.DrawPath(pen, path);
        }
    }

    /// <summary>
    /// Tạo một đối tượng GraphicsPath hình chữ nhật bo góc.
    /// </summary>
    /// <param name="bounds">Kích thước của hình chữ nhật.</param>
    /// <param name="cornerRadius">Bán kính bo góc.</param>
    private GraphicsPath CreateRoundedRectanglePath(Rectangle bounds, int cornerRadius)
    {
        GraphicsPath path = new GraphicsPath();
        bounds.Width--;
        bounds.Height--;
        // Tính toán đường kính
        int diameter = cornerRadius * 2;
        if (diameter <= 0) // Trường hợp bán kính bằng 0
        {
            path.AddRectangle(bounds);
            return path;
        }

        path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
        path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
        path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        return path;
    }

    /// <summary>
    /// Điền dữ liệu phân tích (icon, tiêu đề, màu sắc, danh sách học sinh) vào thẻ.
    /// </summary>
    /// <param name="icon">Biểu tượng emoji (ví dụ: ⭐).</param>
    /// <param name="title">Tiêu đề của thẻ.</param>
    /// <param name="color">Màu nền của thẻ.</param>
    /// <param name="students">Danh sách kết quả phân tích học sinh.</param>
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

        // Định dạng RichTextBox để hiển thị danh sách
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

    /// <summary>
    /// Dọn dẹp tài nguyên và gỡ bỏ các trình xử lý sự kiện.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Gỡ bỏ các sự kiện đã gán thủ công
            if (cardPaintHandler != null)
            {
                this.Paint -= cardPaintHandler;
            }
            if (resizeHandler != null)
            {
                this.Resize -= resizeHandler;
            }

            if (components != null)
            {
                components.Dispose();
            }
        }
        base.Dispose(disposing);
    }
}