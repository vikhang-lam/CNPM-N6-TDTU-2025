using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

/// <summary>
/// Lớp Form cơ sở tùy chỉnh, không viền, bo góc và có thể kéo thả.
/// </summary>
public class frmDraggableRoundedPopup : Form
{
    public Panel ContentPanel;
    private Label lblTitle;
    private int cornerRadius = 20;

    // Biến cho các nút tùy chỉnh (để gỡ sự kiện)
    private Label btnMinimize, btnClose;
    private bool hoverMin = false;
    private bool hoverClose = false;
    private PaintEventHandler minimizePaintHandler, closePaintHandler;
    private EventHandler minimizeClickHandler, closeClickHandler, minimizeEnterHandler, minimizeLeaveHandler, closeEnterHandler, closeLeaveHandler;
    private MouseEventHandler dragHandler;


    public const int WM_NCLBUTTONDOWN = 0xA1;
    public const int HT_CAPTION = 0x2;
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    public static extern int SendMessage(System.IntPtr hWnd, int Msg, int wParam, int lParam);
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    public static extern bool ReleaseCapture();

    public override string Text
    {
        get { return base.Text; }
        set { base.Text = value; if (lblTitle != null) lblTitle.Text = value; }
    }

    public frmDraggableRoundedPopup()
    {
        this.FormBorderStyle = FormBorderStyle.None;
        this.BackColor = Color.White;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.DoubleBuffered = true;
        this.TopMost = true;

        var topPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 40,
            BackColor = Color.Transparent,
            Padding = new Padding(8, 6, 8, 0)
        };

        lblTitle = new Label
        {
            Dock = DockStyle.Fill,
            Text = this.Text,
            Font = new Font("Segoe UI", 11F, FontStyle.Bold),
            ForeColor = Color.DimGray,
            TextAlign = ContentAlignment.MiddleCenter,
            Padding = new Padding(30, 0, 30, 0)
        };

        var btnHost = new Panel { Dock = DockStyle.Right, Width = 88, BackColor = Color.Transparent, Padding = new Padding(0), Margin = new Padding(0) };

        // --- Nút Thu nhỏ ---
        btnMinimize = new Label { Text = "−", Dock = DockStyle.Right, Width = 40, Font = new Font("Segoe UI", 12F, FontStyle.Bold), ForeColor = Color.Gray, TextAlign = ContentAlignment.MiddleCenter, Cursor = Cursors.Hand, BackColor = Color.Transparent, Margin = new Padding(0) };

        minimizeClickHandler = (s, e) => this.WindowState = FormWindowState.Minimized;
        btnMinimize.Click += minimizeClickHandler;

        minimizeEnterHandler = (s, e) => { hoverMin = true; btnMinimize.Invalidate(); };
        btnMinimize.MouseEnter += minimizeEnterHandler;

        minimizeLeaveHandler = (s, e) => { hoverMin = false; btnMinimize.Invalidate(); };
        btnMinimize.MouseLeave += minimizeLeaveHandler;

        minimizePaintHandler = (s, e) =>
        {
            if (hoverMin)
            {
                var inner = Rectangle.Inflate(btnMinimize.ClientRectangle, -4, -3);
                using (var br = new SolidBrush(Color.FromArgb(235, 244, 255)))
                    e.Graphics.FillRectangle(br, inner);
            }
            TextRenderer.DrawText(e.Graphics, "−", btnMinimize.Font, btnMinimize.ClientRectangle, btnMinimize.ForeColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        };
        btnMinimize.Paint += minimizePaintHandler;

        // --- Nút Đóng ---
        btnClose = new Label { Text = "✕", Dock = DockStyle.Right, Width = 40, Font = new Font("Segoe UI", 12F, FontStyle.Bold), ForeColor = Color.Gray, TextAlign = ContentAlignment.MiddleCenter, Cursor = Cursors.Hand, BackColor = Color.Transparent, Margin = new Padding(0) };

        closeClickHandler = (s, e) => this.Close();
        btnClose.Click += closeClickHandler;

        closeEnterHandler = (s, e) => { hoverClose = true; btnClose.Invalidate(); };
        btnClose.MouseEnter += closeEnterHandler;

        closeLeaveHandler = (s, e) => { hoverClose = false; btnClose.Invalidate(); };
        btnClose.MouseLeave += closeLeaveHandler;

        closePaintHandler = (s, e) =>
        {
            if (hoverClose)
            {
                var inner = Rectangle.Inflate(btnClose.ClientRectangle, -4, -3);
                using (var br = new SolidBrush(Color.FromArgb(255, 230, 230)))
                    e.Graphics.FillRectangle(br, inner);
            }
            TextRenderer.DrawText(e.Graphics, "✕", btnClose.Font, btnClose.ClientRectangle, btnClose.ForeColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        };
        btnClose.Paint += closePaintHandler;

        btnHost.Controls.Add(btnMinimize);
        btnHost.Controls.Add(btnClose);

        topPanel.Controls.Add(lblTitle);
        topPanel.Controls.Add(btnHost);

        ContentPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent, Padding = new Padding(15) };

        this.Controls.Add(ContentPanel);
        this.Controls.Add(topPanel);

        // --- Sự kiện kéo thả ---
        dragHandler = new MouseEventHandler(DragForm_MouseDown);
        topPanel.MouseDown += dragHandler;
        lblTitle.MouseDown += dragHandler;
    }

    private void DragForm_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            ReleaseCapture();
            SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e); // Gọi base.OnPaint trước
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

        using (var path = new GraphicsPath())
        {
            var rect = ClientRectangle;
            rect.Width--;
            rect.Height--;

            int radius = cornerRadius * 2;
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseFigure();

            this.Region = new Region(path); // Áp dụng bo góc

            // Vẽ viền
            using (var pen = new Pen(Color.Black, 3))
            {
                e.Graphics.DrawPath(pen, path);
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Gỡ bỏ các sự kiện kéo thả
            if (lblTitle != null) lblTitle.MouseDown -= dragHandler;
            if (this.Controls.Count > 0 && this.Controls[1] is Panel topPanel) // [1] là topPanel
            {
                topPanel.MouseDown -= dragHandler;
            }

            // Gỡ bỏ sự kiện các nút tùy chỉnh
            if (btnMinimize != null)
            {
                btnMinimize.Click -= minimizeClickHandler;
                btnMinimize.MouseEnter -= minimizeEnterHandler;
                btnMinimize.MouseLeave -= minimizeLeaveHandler;
                btnMinimize.Paint -= minimizePaintHandler;
            }
            if (btnClose != null)
            {
                btnClose.Click -= closeClickHandler;
                btnClose.MouseEnter -= closeEnterHandler;
                btnClose.MouseLeave -= closeLeaveHandler;
                btnClose.Paint -= closePaintHandler;
            }
        }
        base.Dispose(disposing);
    }
}