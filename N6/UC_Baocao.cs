using System;
using System.Drawing;
using System.Windows.Forms;

namespace N6
{
    public partial class UC_BaoCao : UserControl
    {
        public UC_BaoCao()
        {
            InitializeComponent();
        }

        private void btnXuatExcel_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng xuất Excel sẽ được cập nhật sau!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnXuatPDF_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng xuất PDF sẽ được cập nhật sau!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
