using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace N6
{
    public class RoundedButton : Button
    {
        private int cornerRadius = 25;
        public int CornerRadius
        {
            get => cornerRadius;
            set { cornerRadius = value; Invalidate(); }
        }

        // Border properties: default transparent, hover = light blue, thin
        private Color borderColor = Color.Transparent;
        public Color BorderColor
        {
            get => borderColor;
            set { borderColor = value; Invalidate(); }
        }

        private Color hoverBorderColor = Color.FromArgb(173, 216, 230);
        public Color HoverBorderColor
        {
            get => hoverBorderColor;
            set { hoverBorderColor = value; Invalidate(); }
        }

        private int borderThickness = 2;
        public int BorderThickness
        {
            get => borderThickness;
            set { borderThickness = Math.Max(0, value); Invalidate(); }
        }

        private bool isHovered = false;
        private bool isPressed = false;

        public RoundedButton()
        {
            // Avoid default system border drawing
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.DoubleBuffered = true;
            this.BackColor = this.BackColor;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            isHovered = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            isHovered = false;
            isPressed = false;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            base.OnMouseDown(mevent);
            if (mevent.Button == MouseButtons.Left)
            {
                isPressed = true;
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);
            if (isPressed)
            {
                isPressed = false;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            var g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Hình chữ nhật vẽ nền và viền, trừ bớt 1px ở cạnh phải/dưới để đảm bảo viền vẽ đẹp
            Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);

            // Bán kính được giới hạn bởi kích thước nút
            int rad = cornerRadius;
            if (rad * 2 > rect.Width) rad = rect.Width / 2;
            if (rad * 2 > rect.Height) rad = rect.Height / 2;

            using (GraphicsPath path = new GraphicsPath())
            {
                // 1. Tạo GraphicsPath cho nền (Nền và Region)
                // Dùng kích thước full (rect) để tạo region.
                path.AddArc(rect.X, rect.Y, rad * 2, rad * 2, 180, 90);
                path.AddArc(rect.Right - (rad * 2), rect.Y, rad * 2, rad * 2, 270, 90);
                path.AddArc(rect.Right - (rad * 2), rect.Bottom - (rad * 2), rad * 2, rad * 2, 0, 90);
                path.AddArc(rect.X, rect.Bottom - (rad * 2), rad * 2, rad * 2, 90, 90);
                path.CloseFigure();

                // Đặt nền theo trạng thái
                Color fillColor = this.BackColor;
                if (isPressed)
                {
                    fillColor = ControlPaint.Dark(this.BackColor, 0.12f);
                }
                else if (isHovered && this.Enabled) // Chỉ sáng lên khi nút được bật
                {
                    fillColor = ControlPaint.Light(this.BackColor, 0.06f);
                }

                using (var fillBrush = new SolidBrush(fillColor))
                {
                    g.FillPath(fillBrush, path);
                }

                // 2. Cắt Region (vùng tương tác) khớp với hình dạng
                this.Region = new Region(path);

                // 3. Vẽ viền
                if (borderThickness > 0)
                {
                    // Định nghĩa BorderRect: Dịch vào trong nửa độ dày viền
                    // và trừ đi 1px ở cuối để tránh bị cắt.
                    Rectangle borderRect = new Rectangle(
                        (int)Math.Floor(borderThickness / 2f),
                        (int)Math.Floor(borderThickness / 2f),
                        this.Width - (int)Math.Ceiling(borderThickness / 2f) - (int)Math.Floor(borderThickness / 2f),
                        this.Height - (int)Math.Ceiling(borderThickness / 2f) - (int)Math.Floor(borderThickness / 2f));

                    // Điều chỉnh kích thước để bán kính vẫn đúng
                    int rad2 = cornerRadius;
                    if (rad2 * 2 > borderRect.Width) rad2 = borderRect.Width / 2;
                    if (rad2 * 2 > borderRect.Height) rad2 = borderRect.Height / 2;

                    using (GraphicsPath borderPath = new GraphicsPath())
                    {
                        // Thêm viền bo tròn
                        // Trừ đi 1px ở Right và Bottom của borderRect.Width/Height khi tính toán để viền nằm hoàn toàn bên trong
                        borderPath.AddArc(borderRect.X, borderRect.Y, rad2 * 2, rad2 * 2, 180, 90);
                        borderPath.AddArc(borderRect.Right - (rad2 * 2) + 1, borderRect.Y, rad2 * 2, rad2 * 2, 270, 90);
                        borderPath.AddArc(borderRect.Right - (rad2 * 2) + 1, borderRect.Bottom - (rad2 * 2) + 1, rad2 * 2, rad2 * 2, 0, 90);
                        borderPath.AddArc(borderRect.X, borderRect.Bottom - (rad2 * 2) + 1, rad2 * 2, rad2 * 2, 90, 90);
                        borderPath.CloseFigure();

                        Color drawBorderColor = isHovered ? hoverBorderColor : borderColor;
                        using (Pen pen = new Pen(drawBorderColor, borderThickness))
                        {
                            pen.LineJoin = LineJoin.Round;
                            g.DrawPath(pen, borderPath);
                        }
                    }
                }

                // 4. Vẽ chữ (giữ nguyên, đây là cách vẽ chữ tốt nhất)
                TextFormatFlags flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;
                Rectangle textRect = rect;
                int pad = Math.Max(1, borderThickness);
                textRect.Inflate(-pad, -pad); // Co chữ vào trong viền
                TextRenderer.DrawText(g, this.Text, this.Font, textRect, this.ForeColor, flags);
            }
        }
    }
}