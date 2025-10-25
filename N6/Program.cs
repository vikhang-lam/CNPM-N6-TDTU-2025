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
                DialogResult loginChoiceResult;

                using (login loginForm = new login())
                {
                    loginChoiceResult = loginForm.ShowDialog();
                }

                if (loginChoiceResult == DialogResult.OK)
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
                else if (loginChoiceResult == DialogResult.Retry)
                {
                    using (TeacherRegistrationForm regForm = new TeacherRegistrationForm())
                    {
                        regForm.ShowDialog();
                    }
                    continue;
                }
                else if (loginChoiceResult == DialogResult.Ignore)
                {
                    using (ForgotPasswordForm forgotForm = new ForgotPasswordForm())
                    {
                        forgotForm.ShowDialog();
                    }
                    continue;
                }
                else
                {
                    break;
                }
            }
        }
    }
}