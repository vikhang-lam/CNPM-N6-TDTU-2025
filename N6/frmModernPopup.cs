using System.Drawing;
using System.Windows.Forms;

public class frmModernPopup : Form
{
    private bool isDragging = false;
    private Point dragStartPoint;
    private Label lblTitle;
    public Panel ContentPanel;

    public override string Text
    {
        get { return base.Text; }
        set { base.Text = value; if (lblTitle != null) lblTitle.Text = value; }
    }

    public frmModernPopup()
    {
        this.FormBorderStyle = FormBorderStyle.None;
        this.BackColor = Color.Gainsboro;
        this.Padding = new Padding(1);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.TransparencyKey = Color.Gainsboro;

        var mainPanel = new RoundedPanel
        {
            Dock = DockStyle.Fill,
            CornerRadius = 15,
            BackColor = Color.White
        };

        var topPanel = new Panel { Dock = DockStyle.Top, Height = 40, BackColor = Color.Transparent };
        lblTitle = new Label { Dock = DockStyle.Fill, Text = this.Text, Font = new Font("Segoe UI", 11F, FontStyle.Bold), ForeColor = Color.DimGray, TextAlign = ContentAlignment.MiddleCenter, Padding = new Padding(30, 0, 30, 0) };

        var btnClose = new Label { Text = "✕", Dock = DockStyle.Right, Width = 40, Font = new Font("Segoe UI", 12F), ForeColor = Color.Gray, TextAlign = ContentAlignment.MiddleCenter };
        btnClose.Click += (s, e) => this.Close();
        btnClose.MouseEnter += (s, e) => btnClose.BackColor = Color.LightCoral;
        btnClose.MouseLeave += (s, e) => btnClose.BackColor = Color.Transparent;

        topPanel.Controls.Add(lblTitle);
        topPanel.Controls.Add(btnClose);

        ContentPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent, Padding = new Padding(15) };

        mainPanel.Controls.Add(ContentPanel);
        mainPanel.Controls.Add(topPanel);
        this.Controls.Add(mainPanel);

        // Cho phép kéo thả form
        topPanel.MouseDown += OnMouseDown;
        topPanel.MouseMove += OnMouseMove;
        topPanel.MouseUp += OnMouseUp;
        lblTitle.MouseDown += OnMouseDown;
        lblTitle.MouseMove += OnMouseMove;
        lblTitle.MouseUp += OnMouseUp;
    }

    private void OnMouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left) { isDragging = true; dragStartPoint = new Point(e.X, e.Y); }
    }
    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (isDragging) { this.Location = new Point(this.Location.X + (e.X - dragStartPoint.X), this.Location.Y + (e.Y - dragStartPoint.Y)); }
    }
    private void OnMouseUp(object sender, MouseEventArgs e)
    {
        isDragging = false;
    }
}