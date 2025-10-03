using System;
using System.Windows.Forms;

namespace N6
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool exitApp = false;

            while (!exitApp)
            {
                using (login loginForm = new login())
                {
                    if (loginForm.ShowDialog() == DialogResult.OK)
                    {
                        bool isAdmin = Properties.Settings.Default.isAdmin;

                        Form mainForm = isAdmin ? (Form)new MenuAdmin() : new dashboard();

                        Application.Run(mainForm);

                        // Nếu logout thì CurrentUser rỗng → quay lại login
                        if (string.IsNullOrEmpty(Properties.Settings.Default["CurrentUser"]?.ToString()))
                        {
                            continue;
                        }
                        else
                        {
                            exitApp = true; // đóng form chính mà vẫn còn user → thoát hẳn
                        }
                    }
                    else
                    {
                        exitApp = true; // người dùng bấm thoát ở login
                    }
                }
            }
        }
    }
}
