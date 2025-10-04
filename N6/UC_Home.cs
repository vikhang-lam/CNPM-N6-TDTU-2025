using System;
using System.Drawing;
using System.Windows.Forms;

namespace N6
{
    public partial class UC_Home : UserControl
    {
        public event Action<string> ChonChucNang;

        public UC_Home(string tenGV = "Giáo viên")
        {
            InitializeComponent();
            lblLoiChao.Text = $"Xin chào, {tenGV}! 👋";
        }

        private void Card_MouseEnter(object sender, EventArgs e)
        {
            var card = sender as Panel;
            card.BackColor = Color.FromArgb(230, 243, 255); // xanh nhạt dịu
            card.BorderStyle = BorderStyle.FixedSingle;
            card.Cursor = Cursors.Hand;
            card.Refresh();
        }

        private void Card_MouseLeave(object sender, EventArgs e)
        {
            var card = sender as Panel;
            card.BackColor = Color.White;
            card.BorderStyle = BorderStyle.None;
        }

        private void Card_Click(object sender, EventArgs e)
        {
            if (sender is Panel p && p.Tag != null)
            {
                ChonChucNang?.Invoke(p.Tag.ToString());
            }
        }
    }
}
