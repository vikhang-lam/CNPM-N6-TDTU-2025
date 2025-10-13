using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public class RoundedButton : Button
{
    // --- Fields ---
    private int _cornerRadius = 15;
    private Color _originalBackColor; // To store the original color
    private Color _hoverBackColor = Color.FromArgb(0, 102, 204); // A slightly darker blue
    private Color _clickBackColor = Color.FromArgb(0, 82, 184); // An even darker blue

    // --- Properties for Customization ---
    public int CornerRadius
    {
        get { return _cornerRadius; }
        set { _cornerRadius = value; Invalidate(); }
    }

    public Color HoverBackColor
    {
        get { return _hoverBackColor; }
        set { _hoverBackColor = value; }
    }

    public Color ClickBackColor
    {
        get { return _clickBackColor; }
        set { _clickBackColor = value; }
    }

    // --- Constructor ---
    public RoundedButton()
    {
        this.FlatStyle = FlatStyle.Flat;
        this.FlatAppearance.BorderSize = 0;
        this.BackColor = Color.DodgerBlue;
        this.ForeColor = Color.White;
        this.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

        // Store the initial BackColor
        _originalBackColor = this.BackColor;
    }

    // --- Helper method to create the rounded rectangle path ---
    private GraphicsPath GetFigurePath(Rectangle rect, int radius)
    {
        GraphicsPath path = new GraphicsPath();
        if (radius <= 0)
        {
            path.AddRectangle(rect);
            return path;
        }

        int diameter = radius * 2;
        // Adjust diameter if it's larger than the button dimensions
        if (diameter > rect.Width) diameter = rect.Width;
        if (diameter > rect.Height) diameter = rect.Height;

        path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
        path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
        path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();

        return path;
    }

    // --- Overridden Paint Method ---
    protected override void OnPaint(PaintEventArgs pevent)
    {
        Rectangle clientRect = this.ClientRectangle;
        pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

        // Use the helper method to get the path
        using (GraphicsPath path = GetFigurePath(clientRect, _cornerRadius))
        {
            this.Region = new Region(path);

            // Use 'using' for the brush to ensure it's disposed of properly
            using (SolidBrush backgroundBrush = new SolidBrush(this.BackColor))
            {
                pevent.Graphics.FillPath(backgroundBrush, path);
            }
        }

        // Draw the text in the center
        TextRenderer.DrawText(pevent.Graphics, this.Text, this.Font,
            clientRect, this.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
    }

    // --- Event Handlers for Interactivity ---
    protected override void OnMouseEnter(EventArgs e)
    {
        base.OnMouseEnter(e);
        _originalBackColor = this.BackColor; // Save current color
        this.BackColor = _hoverBackColor;
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        this.BackColor = _originalBackColor;
    }

    protected override void OnMouseDown(MouseEventArgs mevent)
    {
        base.OnMouseDown(mevent);
        this.BackColor = _clickBackColor;
    }

    protected override void OnMouseUp(MouseEventArgs mevent)
    {
        base.OnMouseUp(mevent);
        // When mouse is released, it might be over the button (hover state) or outside (original state)
        if (this.ClientRectangle.Contains(mevent.Location))
        {
            this.BackColor = _hoverBackColor;
        }
        else
        {
            this.BackColor = _originalBackColor;
        }
    }
}