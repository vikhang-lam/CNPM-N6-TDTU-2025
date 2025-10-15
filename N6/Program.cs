// File: Program.cs (Phiên bản cải tiến)
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


            while (true)
            {
                using (login loginForm = new login())
                {
          
                    if (loginForm.ShowDialog() == DialogResult.OK)
                    {
                        bool isAdmin = Properties.Settings.Default.isAdmin;


                        Form mainForm = isAdmin ? (Form)new MenuAdmin() : new dashboard();

                  
                        Application.Run(mainForm);

                        
                        if (string.IsNullOrEmpty(Properties.Settings.Default.CurrentUser?.ToString()))
                        {
                            continue; 
                        }
                        else
                        {
                            
                            break; 
                        }
                    }
                    else
                    {
                        // Nếu người dùng đóng form login mà không đăng nhập -> thoát ứng dụng.
                        break; // Thoát khỏi vòng lặp while
                    }
                }
            }
        }
    }
}