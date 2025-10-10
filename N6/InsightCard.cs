using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace N6 // <-- Đảm bảo namespace này khớp với project của bạn
{
    public partial class InsightCard : UserControl // <-- DÒNG NÀY RẤT QUAN TRỌNG
    {
        public InsightCard()
        {
            InitializeComponent();
            // Bo tròn góc cho card
            this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
        }

        // Hàm API của Windows để tạo vùng bo tròn
        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern System.IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        public void SetData(string icon, string title, Color color, List<HocSinhAnalysisResult> students)
        {
            lblIcon.Text = icon;
            lblTitle.Text = title;
            this.BackColor = color;
            rtbContent.BackColor = color;

            rtbContent.Clear();
            if (students == null || students.Count == 0)
            {
                rtbContent.AppendText("Không có học sinh nào trong danh sách này.");
                return;
            }

            foreach (var student in students)
            {
                rtbContent.SelectionFont = new Font("Segoe UI", 9, FontStyle.Bold);
                rtbContent.AppendText(student.HoTen);
                rtbContent.SelectionFont = new Font("Segoe UI", 8, FontStyle.Regular);
                rtbContent.SelectionColor = Color.DimGray;
                rtbContent.AppendText($" ({student.TenLop})\n");
                rtbContent.SelectionFont = new Font("Segoe UI", 8, FontStyle.Italic);
                rtbContent.SelectionColor = Color.DarkSlateGray;
                rtbContent.AppendText($"   Lý do: {student.LyDo}\n\n");
            }
        }

        // Vẽ lại vùng bo tròn khi kích thước control thay đổi
        protected override void OnResize(System.EventArgs e)
        {
            base.OnResize(e);
            this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
        }
    }
}