using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace N6
{
    public partial class login : Form
    {
        public login()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (this.ClientRectangle.Width == 0 || this.ClientRectangle.Height == 0)
            {
                return;
            }

            using (LinearGradientBrush brush = new LinearGradientBrush(
                this.ClientRectangle,
                Color.FromArgb(179, 102, 255), // Tím nhạt
                Color.FromArgb(255, 102, 204), // Hồng
                90f))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
        }

        private void login_Load(object sender, EventArgs e)
        {
            paneluser1.BorderStyle = BorderStyle.None;
            paneluser2.BorderStyle = BorderStyle.None;
            paneluser3.BorderStyle = BorderStyle.None;
            paneluser4.BorderStyle = BorderStyle.None;

            labelGreeting.BackColor = Color.Transparent;
            labelInstruction.BackColor = Color.Transparent;

            MakePanelRound(paneluser1);
            MakePanelRound(paneluser2);
            MakePanelRound(paneluser3);
            MakePanelRound(paneluser4);

            AddContentToPanel(paneluser1, "GV1");
            AddContentToPanel(paneluser2, "GV2");
            AddContentToPanel(paneluser3, "GV3");
            AddPlusSignToPanel(paneluser4);
        }

        private void MakePanelRound(Panel panel)
        {
            if (panel.Width <= 0 || panel.Height <= 0)
                return;

            GraphicsPath path = new GraphicsPath();
            int cornerRadius = 30;

            path.AddArc(0, 0, cornerRadius * 2, cornerRadius * 2, 180, 90);
            path.AddArc(panel.Width - cornerRadius * 2, 0, cornerRadius * 2, cornerRadius * 2, 270, 90);
            path.AddArc(panel.Width - cornerRadius * 2, panel.Height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);
            path.AddArc(0, panel.Height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);
            path.CloseAllFigures();

            panel.Region = new Region(path);
        }

        private void AddContentToPanel(Panel panel, string name)
        {
            panel.Controls.Clear();

            PictureBox avatarBox = new PictureBox();
            avatarBox.Size = new Size(150, 150);
            avatarBox.Location = new Point((panel.Width - avatarBox.Width) / 2, 30);
            avatarBox.SizeMode = PictureBoxSizeMode.Zoom;
            avatarBox.BackColor = Color.LightGray;

            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddEllipse(0, 0, avatarBox.Width - 1, avatarBox.Height - 1);
                avatarBox.Region = new Region(path);
            }

            Label nameLabel = new Label();
            nameLabel.Text = name;
            nameLabel.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            nameLabel.ForeColor = Color.Black;
            nameLabel.AutoSize = true;
            nameLabel.Location = new Point((panel.Width - nameLabel.Width) / 2, avatarBox.Bottom + 20);

            panel.Controls.Add(avatarBox);
            panel.Controls.Add(nameLabel);
        }

        private void AddPlusSignToPanel(Panel panel)
        {
            panel.Controls.Clear();

            Label plusLabel = new Label();
            plusLabel.Text = "+";
            plusLabel.Font = new Font("Segoe UI", 60, FontStyle.Regular);
            plusLabel.ForeColor = Color.LightGray;
            plusLabel.AutoSize = true;
            plusLabel.Location = new Point((panel.Width - plusLabel.Width) / 2, (panel.Height - plusLabel.Height) / 2 - 20);

            Label otherTeacherLabel = new Label();
            otherTeacherLabel.Text = "Giáo viên khác";
            otherTeacherLabel.Font = new Font("Segoe UI", 12);
            otherTeacherLabel.ForeColor = Color.DimGray;
            otherTeacherLabel.AutoSize = true;
            otherTeacherLabel.Location = new Point((panel.Width - otherTeacherLabel.Width) / 2, plusLabel.Bottom + 10);

            panel.Controls.Add(plusLabel);
            panel.Controls.Add(otherTeacherLabel);
        }

        // --- Hiệu ứng nháy sáng khi di chuột ---
        private void paneluser_MouseEnter(object sender, EventArgs e)
        {
            Panel panel = sender as Panel;
            if (panel != null)
            {
                panel.BackColor = Color.FromArgb(240, 240, 240); // Sáng hơn
            }
        }

        private void paneluser_MouseLeave(object sender, EventArgs e)
        {
            Panel panel = sender as Panel;
            if (panel != null)
            {
                panel.BackColor = Color.White; // Trả lại màu trắng ban đầu
            }
        }

        // --- Các phương thức xử lý di chuyển và nút chức năng ---
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HTCAPTION = 0x2;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void panelTopBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        private void labelClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void labelMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void labelMaximize_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e) { }
        private void panel4_Paint(object sender, PaintEventArgs e) { }
        private void panel2_Paint(object sender, PaintEventArgs e) { }
        private void panel3_Paint(object sender, PaintEventArgs e) { }
        private void paneluser1_Paint(object sender, PaintEventArgs e) { }
    }
}