using System;
using System.Drawing;
using System.Windows.Forms;

namespace N6
{
    public abstract class BaseGameForm : Form
    {
        public Font GameFont(float size, FontStyle style = FontStyle.Bold) { return new Font("Lexend", size, style, GraphicsUnit.Point, ((byte)(0))); }
        public readonly Color BgColor = Color.FromArgb(240, 247, 255);
        public readonly Color PrimaryColor = Color.FromArgb(87, 187, 247);
        public readonly Color SecondaryColor = Color.FromArgb(255, 189, 89);
        public readonly Color CorrectColor = Color.FromArgb(29, 209, 161);
        public readonly Color IncorrectColor = Color.FromArgb(255, 118, 117);
        public readonly Color TextColor = Color.FromArgb(64, 64, 64);
        public readonly Color MutedTextColor = Color.FromArgb(150, 150, 150);

        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        public static extern IntPtr CreateRoundRectRgn(int l, int t, int r, int b, int w, int h);

        protected void CloseWithWarning(string message = "Không có dữ liệu để bắt đầu game.")
        {
            try
            {
                MessageBox.Show(message, "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch { }
            try
            {
                if (!this.IsDisposed)
                {
                    if (this.IsHandleCreated)
                    {
                        this.Visible = false;
                    }
                    this.Dispose();
                }
            }
            catch { }
        }
    }
}
