using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public class frmDraggableRoundedPopup : Form
{
    public Panel ContentPanel;
    private Label lblTitle;
    private int cornerRadius = 20;

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

        var btnHost = new Panel
        {
            Dock = DockStyle.Right,
            Width = 88,
            BackColor = Color.Transparent,
            Padding = new Padding(0),
            Margin = new Padding(0)
        };

        // Minimize button
        bool hoverMin = false;
        var btnMinimize = new Label
        {
            Text = "−",
            Dock = DockStyle.Right,
            Width = 40,
            Font = new Font("Segoe UI", 12F, FontStyle.Bold),
            ForeColor = Color.Gray,
            TextAlign = ContentAlignment.MiddleCenter,
            Cursor = Cursors.Hand,
            BackColor = Color.Transparent,
            Margin = new Padding(0)
        };
        btnMinimize.Click += (s, e) => this.WindowState = FormWindowState.Minimized;
        btnMinimize.MouseEnter += (s, e) => { hoverMin = true; btnMinimize.Invalidate(); };
        btnMinimize.MouseLeave += (s, e) => { hoverMin = false; btnMinimize.Invalidate(); };
        btnMinimize.Paint += (s, e) =>
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

        // Close button
        bool hoverClose = false;
        var btnClose = new Label
        {
            Text = "✕",
            Dock = DockStyle.Right,
            Width = 40,
            Font = new Font("Segoe UI", 12F, FontStyle.Bold),
            ForeColor = Color.Gray,
            TextAlign = ContentAlignment.MiddleCenter,
            Cursor = Cursors.Hand,
            BackColor = Color.Transparent,
            Margin = new Padding(0)
        };
        btnClose.Click += (s, e) => this.Close();
        btnClose.MouseEnter += (s, e) => { hoverClose = true; btnClose.Invalidate(); };
        btnClose.MouseLeave += (s, e) => { hoverClose = false; btnClose.Invalidate(); };
        btnClose.Paint += (s, e) =>
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

        btnHost.Controls.Add(btnMinimize);
        btnHost.Controls.Add(btnClose);

        topPanel.Controls.Add(lblTitle);
        topPanel.Controls.Add(btnHost);

        ContentPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent, Padding = new Padding(15) };

        this.Controls.Add(ContentPanel);
        this.Controls.Add(topPanel);

        topPanel.MouseDown += DragForm_MouseDown;
        lblTitle.MouseDown += DragForm_MouseDown;
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

            this.Region = new Region(path);

            using (var brush = new SolidBrush(this.BackColor))
                e.Graphics.FillPath(brush, path);

            using (var pen = new Pen(Color.Black, 3))
                e.Graphics.DrawPath(pen, path);
        }
    }
}