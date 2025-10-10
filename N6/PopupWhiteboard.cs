using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public class PopupWhiteboard : Form
{
    private PictureBox canvas;
    private Bitmap drawingBitmap;
    private Graphics drawingGraphics;
    private Pen currentPen;
    private Point? lastPoint = null;

    // ====> BIẾN TRẠNG THÁI MỚI <====
    private bool isDrawing = false;
    private bool isErasing = false;
    private bool isTextMode = false;

    public PopupWhiteboard()
    {
        this.Text = "Bảng Trắng";
        this.Size = new Size(800, 600);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = Color.White;
        this.TopMost = true;

        // Toolbar
        var toolPanel = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 45, Padding = new Padding(5), BackColor = Color.WhiteSmoke };

        var btnPen = new Button { Text = "Bút 🖌️", Width = 70 };
        var btnEraser = new Button { Text = "Tẩy 🧼", Width = 70 };
        // ====> THÊM NÚT GÕ CHỮ MỚI <====
        var btnText = new Button { Text = "Chữ A", Width = 70, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };

        var btnColor = new Button { Text = "Màu", Width = 60 };
        var btnClear = new Button { Text = "Xóa hết", Width = 70 };
        var btnSave = new Button { Text = "Lưu ảnh", Width = 70 };
        var sizeTrackBar = new TrackBar { Minimum = 1, Maximum = 20, Value = 5, Width = 100, TickStyle = TickStyle.None };
        var lblSize = new Label { Text = "Cỡ: 5", AutoSize = true, Padding = new Padding(5, 8, 0, 0) };

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
        canvas.Paint += (s, e) => e.Graphics.DrawImage(drawingBitmap, Point.Empty);

        // ====> CẬP NHẬT SỰ KIỆN CLICK CỦA CÁC NÚT <====
        btnPen.Click += (s, e) => {
            isErasing = false;
            isTextMode = false;
            canvas.Cursor = Cursors.Cross;
        };
        btnEraser.Click += (s, e) => {
            isErasing = true;
            isTextMode = false;
            canvas.Cursor = Cursors.Cross;
        };
        btnText.Click += (s, e) => {
            isTextMode = true;
            isErasing = false;
            canvas.Cursor = Cursors.IBeam; // Đổi con trỏ chuột thành dạng gõ chữ
        };

        btnColor.Click += BtnColor_Click;
        sizeTrackBar.ValueChanged += (s, e) => {
            currentPen.Width = sizeTrackBar.Value;
            lblSize.Text = $"Cỡ: {sizeTrackBar.Value}";
        };
        btnClear.Click += (s, e) => {
            drawingGraphics.Clear(Color.White);
            canvas.Invalidate();
        };
        btnSave.Click += BtnSave_Click;
    }

    private void InitializeDrawing()
    {
        currentPen = new Pen(Color.Black, 5);
        drawingBitmap = new Bitmap(canvas.Width > 0 ? canvas.Width : 1, canvas.Height > 0 ? canvas.Height : 1);
        drawingGraphics = Graphics.FromImage(drawingBitmap);
        drawingGraphics.SmoothingMode = SmoothingMode.AntiAlias;
        drawingGraphics.Clear(Color.White);
        canvas.Image = drawingBitmap;
    }

    private void Canvas_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left) return;

        // ====> XỬ LÝ KHI CLICK CHUỘT Ở CHẾ ĐỘ GÕ CHỮ <====
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
            using (var pen = new Pen(isErasing ? Color.White : currentPen.Color, currentPen.Width))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                drawingGraphics.DrawLine(pen, lastPoint.Value, e.Location);
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

    // ====> CÁC HÀM MỚI CHO CHỨC NĂNG GÕ CHỮ <====

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

        // Gán sự kiện để xử lý khi gõ xong
        tempTextbox.LostFocus += TempTextbox_LostFocus;
        tempTextbox.KeyDown += TempTextbox_KeyDown;

        canvas.Controls.Add(tempTextbox);
        tempTextbox.Focus();
    }

    private void TempTextbox_KeyDown(object sender, KeyEventArgs e)
    {
        // Khi nhấn Enter, hoàn tất việc gõ chữ
        if (e.KeyCode == Keys.Enter)
        {
            DrawTextAndRemoveTextbox(sender as TextBox);
            e.SuppressKeyPress = true; // Ngăn tiếng "beep"
        }
    }

    private void TempTextbox_LostFocus(object sender, EventArgs e)
    {
        // Khi click ra ngoài, cũng hoàn tất việc gõ chữ
        DrawTextAndRemoveTextbox(sender as TextBox);
    }

    private void DrawTextAndRemoveTextbox(TextBox textbox)
    {
        if (textbox == null) return;

        string text = textbox.Text;
        Point location = textbox.Location;
        Font font = textbox.Font;
        Color color = textbox.ForeColor;

        // Xóa textbox khỏi canvas trước
        canvas.Controls.Remove(textbox);
        textbox.Dispose();

        // Vẽ chữ lên bitmap nền
        if (!string.IsNullOrEmpty(text))
        {
            using (var brush = new SolidBrush(color))
            {
                drawingGraphics.DrawString(text, font, brush, location);
            }
            canvas.Invalidate(); // Yêu cầu vẽ lại canvas với chữ mới
        }
    }
    // =======================================================

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

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        if (canvas != null && canvas.Width > 0 && canvas.Height > 0 && drawingBitmap != null)
        {
            var newBitmap = new Bitmap(canvas.Width, canvas.Height);
            using (var newGraphics = Graphics.FromImage(newBitmap))
            {
                newGraphics.DrawImage(drawingBitmap, Point.Empty);
            }
            drawingBitmap.Dispose();
            drawingGraphics.Dispose();
            drawingBitmap = newBitmap;
            drawingGraphics = Graphics.FromImage(drawingBitmap);
            canvas.Image = drawingBitmap;
        }
    }
}