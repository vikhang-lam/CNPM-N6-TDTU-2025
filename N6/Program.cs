using System;
using System.Windows.Forms;

namespace N6
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Bắt lỗi toàn cục (UI thread)
            Application.ThreadException += (sender, e) =>
            {
                ShowError(e.Exception, "Lỗi ứng dụng");
                // Quan trọng: không cho app thoát, chỉ báo lỗi
            };

            // Bắt lỗi không mong đợi (non-UI thread)
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                if (e.ExceptionObject is Exception ex)
                {
                    ShowError(ex, "Lỗi hệ thống");
                }
            };

            Application.Run(new login());
        }

        private static void ShowError(Exception ex, string title)
        {
            MessageBox.Show(
                "❌ Có lỗi xảy ra:\n\n" + ex.Message,
                title,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
            // Không Application.Exit(); -> vẫn ở lại form cũ
        }
    }
}
