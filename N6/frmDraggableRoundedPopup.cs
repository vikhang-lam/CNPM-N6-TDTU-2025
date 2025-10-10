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
        // ====> THAY ĐỔI 1: Đặt nền chính là màu trắng <====
        this.BackColor = Color.White;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.DoubleBuffered = true;
        this.TopMost = true;

        var topPanel = new Panel { Dock = DockStyle.Top, Height = 40, BackColor = Color.Transparent };
        lblTitle = new Label { Dock = DockStyle.Fill, Text = this.Text, Font = new Font("Segoe UI", 11F, FontStyle.Bold), ForeColor = Color.DimGray, TextAlign = ContentAlignment.MiddleCenter, Padding = new Padding(30, 0, 30, 0) };

        var btnClose = new Label { Text = "✕", Dock = DockStyle.Right, Width = 40, Font = new Font("Segoe UI", 12F), ForeColor = Color.Gray, TextAlign = ContentAlignment.MiddleCenter, Cursor = Cursors.Hand };
        btnClose.Click += (s, e) => this.Close();
        btnClose.MouseEnter += (s, e) => btnClose.BackColor = Color.FromArgb(255, 230, 230);
        btnClose.MouseLeave += (s, e) => btnClose.BackColor = Color.Transparent;

        topPanel.Controls.Add(lblTitle);
        topPanel.Controls.Add(btnClose);

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
            // Thu nhỏ rect một chút để đường viền không bị cắt
            rect.Width--;
            rect.Height--;

            int radius = cornerRadius * 2;
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseFigure();

            // Đặt vùng hiển thị của form theo đường path bo tròn
            this.Region = new Region(path);

            // ====> THAY ĐỔI 2: VẼ NỀN VÀ VIỀN <====

            // Vẽ nền trắng cho form
            using (var brush = new SolidBrush(this.BackColor))
            {
                e.Graphics.FillPath(brush, path);
            }

            // Vẽ đường viền màu đen xung quanh
            using (var pen = new Pen(Color.Black, 3))
            {
                e.Graphics.DrawPath(pen, path);
            }
        }
    }
}