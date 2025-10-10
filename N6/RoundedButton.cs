using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public class RoundedButton : Button
{
    private int _cornerRadius = 15;

    public int CornerRadius
    {
        get { return _cornerRadius; }
        set { _cornerRadius = value; Invalidate(); }
    }

    public RoundedButton()
    {
        this.FlatStyle = FlatStyle.Flat;
        this.FlatAppearance.BorderSize = 0;
        this.BackColor = Color.DodgerBlue;
        this.ForeColor = Color.White;
        this.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
    }

    protected override void OnPaint(PaintEventArgs pevent)
    {
        GraphicsPath path = new GraphicsPath();
        Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);

        int radius = _cornerRadius * 2;
        if (radius <= 0) radius = 1; // Ensure radius is positive
        if (radius > rect.Width) radius = rect.Width;
        if (radius > rect.Height) radius = rect.Height;

        path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
        path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
        path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
        path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
        path.CloseFigure();

        pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        this.Region = new Region(path);

        pevent.Graphics.FillPath(new SolidBrush(this.BackColor), path);

        TextRenderer.DrawText(pevent.Graphics, this.Text, this.Font,
            this.ClientRectangle, this.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
    }
}