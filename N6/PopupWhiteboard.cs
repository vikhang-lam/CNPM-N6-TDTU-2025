using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

/// <summary>
/// Form popup Bảng trắng (Whiteboard) cho phép vẽ, tẩy, và gõ chữ.
/// </summary>
public class PopupWhiteboard : Form
{
    private PictureBox canvas;
    private Bitmap drawingBitmap;
    private Graphics drawingGraphics;
    private Pen currentPen;
    private Point? lastPoint = null;

    private bool isDrawing = false;
    private bool isErasing = false;
    private bool isTextMode = false;

    // Biến lưu trữ control và sự kiện để Dispose
    private Button btnPen, btnEraser, btnText, btnColor, btnClear, btnSave;
    private TrackBar sizeTrackBar;
    private Label lblSize;

    public PopupWhiteboard()
    {
        this.Text = "Bảng Trắng";
        this.Size = new Size(800, 600);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = Color.White;
        this.TopMost = true;

        // Toolbar
        var toolPanel = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 45, Padding = new Padding(5), BackColor = Color.WhiteSmoke };

        btnPen = new Button { Text = "Bút 🖌️", Width = 70 };
        btnEraser = new Button { Text = "Tẩy 🧼", Width = 70 };
        btnText = new Button { Text = "Chữ A", Width = 70, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
        btnColor = new Button { Text = "Màu", Width = 60 };
        btnClear = new Button { Text = "Xóa hết", Width = 70 };
        btnSave = new Button { Text = "Lưu ảnh", Width = 70 };
        sizeTrackBar = new TrackBar { Minimum = 1, Maximum = 20, Value = 5, Width = 100, TickStyle = TickStyle.None };
        lblSize = new Label { Text = "Cỡ: 5", AutoSize = true, Padding = new Padding(5, 8, 0, 0) };

        toolPanel.Controls.AddRange(new Control[] { btnPen, btnEraser, btnText, btnColor, new Label { Text = "Cỡ:", Padding = new Padding(10, 8, 0, 0) }, sizeTrackBar, lblSize, btnClear, btnSave });

        // Canvas
        canvas = new PictureBox { Dock = DockStyle.Fill, Cursor = Cursors.Cross };

        this.Controls.Add(canvas);
        this.Controls.Add(toolPanel);

        // Khởi tạo
        InitializeDrawing();

        // Gán sự kiện
        canvas.MouseDown += Canvas_MouseDown;
        canvas.MouseMove += Canvas_MouseMove;
        canvas.MouseUp += Canvas_MouseUp;
        canvas.Paint += Canvas_Paint; // Tách ra hàm riêng để gỡ bỏ

        btnPen.Click += BtnPen_Click;
        btnEraser.Click += BtnEraser_Click;
        btnText.Click += BtnText_Click;
        btnColor.Click += BtnColor_Click;
        sizeTrackBar.ValueChanged += SizeTrackBar_ValueChanged;
        btnClear.Click += BtnClear_Click;
        btnSave.Click += BtnSave_Click;
    }

    /// <summary>
    /// Khởi tạo Bitmap và Graphics để vẽ.
    /// </summary>
    private void InitializeDrawing()
    {
        currentPen = new Pen(Color.Black, 5);
        // Đảm bảo kích thước hợp lệ khi khởi tạo
        int width = canvas.Width > 0 ? canvas.Width : 1;
        int height = canvas.Height > 0 ? canvas.Height : 1;

        drawingBitmap = new Bitmap(width, height);
        drawingGraphics = Graphics.FromImage(drawingBitmap);
        drawingGraphics.SmoothingMode = SmoothingMode.AntiAlias;
        drawingGraphics.Clear(Color.White);
        canvas.Image = drawingBitmap;
    }

    private void Canvas_Paint(object sender, PaintEventArgs e)
    {
        if (drawingBitmap != null)
        {
            e.Graphics.DrawImage(drawingBitmap, Point.Empty);
        }
    }

    #region Mouse & Tool Events (Sự kiện chuột và công cụ)

    private void Canvas_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left) return;

        if (isTextMode)
        {
            CreateTextboxAtPoint(e.Location);
        }
        else // Chế độ vẽ/tẩy
        {
            isDrawing = true;
            lastPoint = e.Location;
        }
    }

    private void Canvas_MouseMove(object sender, MouseEventArgs e)
    {
        if (isDrawing && lastPoint.HasValue)
        {
            // Dùng Pen mới trong khối 'using' để đảm bảo an toàn thread
            using (var pen = new Pen(isErasing ? Color.White : currentPen.Color, currentPen.Width))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                if (drawingGraphics != null)
                {
                    drawingGraphics.DrawLine(pen, lastPoint.Value, e.Location);
                }
            }
            lastPoint = e.Location;
            canvas.Invalidate();
        }
    }

    private void Canvas_MouseUp(object sender, MouseEventArgs e)
    {
        isDrawing = false;
        lastPoint = null;
    }

    // --- Nút Công Cụ ---
    private void BtnPen_Click(object sender, EventArgs e)
    {
        isErasing = false;
        isTextMode = false;
        canvas.Cursor = Cursors.Cross;
    }

    private void BtnEraser_Click(object sender, EventArgs e)
    {
        isErasing = true;
        isTextMode = false;
        canvas.Cursor = Cursors.Cross;
    }

    private void BtnText_Click(object sender, EventArgs e)
    {
        isTextMode = true;
        isErasing = false;
        canvas.Cursor = Cursors.IBeam; // Đổi con trỏ chuột
    }

    private void BtnColor_Click(object sender, EventArgs e)
    {
        using (var colorDialog = new ColorDialog())
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                currentPen.Color = colorDialog.Color;
                isErasing = false;
                isTextMode = false;
                canvas.Cursor = Cursors.Cross;
            }
        }
    }

    private void SizeTrackBar_ValueChanged(object sender, EventArgs e)
    {
        currentPen.Width = sizeTrackBar.Value;
        lblSize.Text = $"Cỡ: {sizeTrackBar.Value}";
    }

    private void BtnClear_Click(object sender, EventArgs e)
    {
        drawingGraphics.Clear(Color.White);
        canvas.Invalidate();
    }

    private void BtnSave_Click(object sender, EventArgs e)
    {
        using (var sfd = new SaveFileDialog())
        {
            sfd.Filter = "PNG Image|*.png|JPEG Image|*.jpg";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                drawingBitmap.Save(sfd.FileName);
            }
        }
    }

    #endregion

    #region Text Mode (Chế độ gõ chữ)

    /// <summary>
    /// Tạo một TextBox động tại vị trí click chuột.
    /// </summary>
    private void CreateTextboxAtPoint(Point location)
    {
        var tempTextbox = new TextBox
        {
            Location = location,
            BorderStyle = BorderStyle.FixedSingle,
            Font = new Font("Segoe UI", 8 + (currentPen.Width * 1.5f)), // Cỡ chữ theo cỡ nét
            ForeColor = currentPen.Color,
            AutoSize = true
        };

        // Gán sự kiện (sẽ được gỡ trong hàm LostFocus/KeyDown)
        tempTextbox.LostFocus += TempTextbox_LostFocus;
        tempTextbox.KeyDown += TempTextbox_KeyDown;

        canvas.Controls.Add(tempTextbox);
        tempTextbox.Focus();
    }

    /// <summary>
    /// Xử lý khi nhấn Enter (hoàn tất gõ chữ).
    /// </summary>
    private void TempTextbox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            DrawTextAndRemoveTextbox(sender as TextBox);
            e.SuppressKeyPress = true; // Ngăn tiếng "beep"
        }
    }

    /// <summary>
    /// Xử lý khi click ra ngoài (hoàn tất gõ chữ).
    /// </summary>
    private void TempTextbox_LostFocus(object sender, EventArgs e)
    {
        DrawTextAndRemoveTextbox(sender as TextBox);
    }

    /// <summary>
    /// Vẽ chữ lên Bitmap và xóa TextBox.
    /// </summary>
    private void DrawTextAndRemoveTextbox(TextBox textbox)
    {
        if (textbox == null) return;

        string text = textbox.Text;
        Point location = textbox.Location;
        Font font = textbox.Font;
        Color color = textbox.ForeColor;

        // Gỡ sự kiện trước khi xóa
        textbox.LostFocus -= TempTextbox_LostFocus;
        textbox.KeyDown -= TempTextbox_KeyDown;
        canvas.Controls.Remove(textbox);
        textbox.Dispose();

        if (!string.IsNullOrEmpty(text) && drawingGraphics != null)
        {
            using (var brush = new SolidBrush(color))
            {
                drawingGraphics.DrawString(text, font, brush, location);
            }
            canvas.Invalidate(); // Vẽ lại canvas với chữ mới
        }
    }

    #endregion

    #region Resize & Dispose

    /// <summary>
    /// Xử lý khi Form thay đổi kích thước: tạo lại Bitmap với kích thước mới.
    /// </summary>
    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        if (canvas != null && canvas.Width > 0 && canvas.Height > 0 && drawingBitmap != null)
        {
            // Tạo bitmap mới với kích thước mới
            var newBitmap = new Bitmap(canvas.Width, canvas.Height);
            using (var newGraphics = Graphics.FromImage(newBitmap))
            {
                // Vẽ nội dung bitmap cũ lên bitmap mới
                newGraphics.DrawImage(drawingBitmap, Point.Empty);
            }

            // Hủy tài nguyên cũ
            drawingBitmap.Dispose();
            drawingGraphics.Dispose();

            // Gán tài nguyên mới
            drawingBitmap = newBitmap;
            drawingGraphics = Graphics.FromImage(drawingBitmap);
            drawingGraphics.SmoothingMode = SmoothingMode.AntiAlias;
            canvas.Image = drawingBitmap;
        }
    }

    /// <summary>
    /// Dọn dẹp tài nguyên (Bitmap, Graphics, Pen, Sự kiện).
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Gỡ bỏ sự kiện của canvas
            if (canvas != null)
            {
                canvas.MouseDown -= Canvas_MouseDown;
                canvas.MouseMove -= Canvas_MouseMove;
                canvas.MouseUp -= Canvas_MouseUp;
                canvas.Paint -= Canvas_Paint;
            }

            // Gỡ bỏ sự kiện của các nút
            if (btnPen != null) btnPen.Click -= BtnPen_Click;
            if (btnEraser != null) btnEraser.Click -= BtnEraser_Click;
            if (btnText != null) btnText.Click -= BtnText_Click;
            if (btnColor != null) btnColor.Click -= BtnColor_Click;
            if (sizeTrackBar != null) sizeTrackBar.ValueChanged -= SizeTrackBar_ValueChanged;
            if (btnClear != null) btnClear.Click -= BtnClear_Click;
            if (btnSave != null) btnSave.Click -= BtnSave_Click;

            // Hủy các đối tượng GDI+
            currentPen?.Dispose();
            drawingGraphics?.Dispose();
            drawingBitmap?.Dispose();
        }
        base.Dispose(disposing);
    }

    #endregion
}