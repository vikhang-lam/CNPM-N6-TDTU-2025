using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace N6
{
    public class RoundedButton : Button
    {
        private int cornerRadius = 25;
        public int CornerRadius { get => cornerRadius; set { cornerRadius = value; Invalidate(); } }
        protected override void OnPaint(PaintEventArgs pevent)
        {
            GraphicsPath grPath = new GraphicsPath();
            if (cornerRadius > 0 && this.Width > 0 && this.Height > 0)
            {
                Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);
                int rad = cornerRadius;
                if (rad * 2 > rect.Width) rad = rect.Width / 2;
                if (rad * 2 > rect.Height) rad = rect.Height / 2;

                grPath.AddArc(rect.X, rect.Y, rad * 2, rad * 2, 180, 90);
                grPath.AddArc(rect.Right - (rad * 2), rect.Y, rad * 2, rad * 2, 270, 90);
                grPath.AddArc(rect.Right - (rad * 2), rect.Bottom - (rad * 2), rad * 2, rad * 2, 0, 90);
                grPath.AddArc(rect.X, rect.Bottom - (rad * 2), rad * 2, rad * 2, 90, 90);
                grPath.CloseFigure();
                this.Region = new Region(grPath);
            }
            base.OnPaint(pevent);
        }
    }
}
