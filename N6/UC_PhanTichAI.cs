using System;
using System.Drawing;
using System.Windows.Forms;

namespace N6
{
    public partial class UC_PhanTichAI : UserControl
    {
        public UC_PhanTichAI()
        {
            InitializeComponent();
        }

        private void btnPhanTich_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Phân tích AI đang được xử lý... (demo)", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
