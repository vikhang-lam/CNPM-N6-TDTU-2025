using System;
using System.Windows.Forms;

namespace N6
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            login loginForm = new login();
            if (loginForm.ShowDialog() == DialogResult.OK)
            {
                // Check if admin or teacher based on saved setting
                bool isAdmin = Properties.Settings.Default.isAdmin;
                if (isAdmin == true)
                {
                    Application.Run(new MenuAdmin());
                }
                else
                {
                    Application.Run(new dashboard());
                }
            }
        }
    }
}