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

            // Vòng lặp chính của ứng dụng
            while (true)
            {
                // Luôn bắt đầu bằng form login
                using (login loginForm = new login())
                {
                    // Hiển thị form login dưới dạng Dialog.
                    // Luồng code sẽ dừng ở đây cho đến khi form login được đóng.
                    if (loginForm.ShowDialog() == DialogResult.OK)
                    {
                        // Nếu đăng nhập thành công, kiểm tra vai trò
                        bool isAdmin = Properties.Settings.Default.isAdmin;

                        // Chọn form chính phù hợp để hiển thị
                        Form mainForm = isAdmin ? (Form)new MenuAdmin() : new dashboard();

                        // Chạy form chính. Luồng code sẽ dừng ở đây cho đến khi
                        // form chính (MenuAdmin hoặc dashboard) bị đóng (do đăng xuất hoặc thoát).
                        Application.Run(mainForm);

                        // Sau khi form chính đóng, kiểm tra xem người dùng có còn đăng nhập không.
                        // Nếu CurrentUser đã bị xóa (do đăng xuất), vòng lặp sẽ quay lại từ đầu -> hiển thị lại login.
                        if (string.IsNullOrEmpty(Properties.Settings.Default.CurrentUser?.ToString()))
                        {
                            continue; // Quay lại đầu vòng lặp để hiển thị form login
                        }
                        else
                        {
                            // Nếu form chính đóng nhưng CurrentUser vẫn còn -> người dùng đã thoát ứng dụng.
                            break; // Thoát khỏi vòng lặp while
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