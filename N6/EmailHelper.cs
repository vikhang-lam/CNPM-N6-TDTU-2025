using System;
using System.Net;
using System.Net.Mail;
using System.Configuration;
using System.Diagnostics;
using System.Windows.Forms;

namespace N6
{
    public static class EmailHelper
    {
        // Cấu hình email server (SMTP)
        private static readonly string SmtpServer = "smtp.gmail.com";
        private static readonly int SmtpPort = 587;

        // Đọc cấu hình từ App.config
        private static readonly string SenderEmail = ConfigurationManager.AppSettings["SmtpEmail"];
        private static readonly string SenderPassword = ConfigurationManager.AppSettings["SmtpPassword"];
        private static readonly string SenderName = "Hệ Thống Quản Lý Trường Học";

        /// <summary>
        /// Gửi email thông báo trạng thái tài khoản cho giáo viên
        /// </summary>
        public static bool SendAccountStatusEmail(string recipientEmail, string teacherName, bool isApproved)
        {
            try
            {
                // Tạo tiêu đề và nội dung email
                string subject = isApproved
                    ? "✅ Tài khoản của bạn đã được kích hoạt"
                    : "❌ Yêu cầu tạo tài khoản bị từ chối";

                string body = isApproved
                    ? $@"
                        <html>
                        <body style='font-family: Arial, sans-serif;'>
                            <h2 style='color: #28a745;'>Chào mừng {teacherName}!</h2>
                            <p>Tài khoản giáo viên của bạn đã được <b>kích hoạt thành công</b>.</p>
                            <p>Bạn có thể đăng nhập vào hệ thống ngay bây giờ.</p>
                            <p>Chúc bạn có trải nghiệm tốt!</p>
                            <hr/>
                            <small>Email này được gửi tự động từ Hệ Thống Quản Lý Trường Học</small>
                        </body>
                        </html>"
                    : $@"
                        <html>
                        <body style='font-family: Arial, sans-serif;'>
                            <h2 style='color: #dc3545;'>Thông báo từ Hệ Thống</h2>
                            <p>Kính gửi {teacherName},</p>
                            <p>Yêu cầu tạo tài khoản giáo viên của bạn <b>đã bị từ chối</b>.</p>
                            <p>Vui lòng liên hệ với ban quản trị để biết thêm chi tiết.</p>
                            <hr/>
                            <small>Email này được gửi tự động từ Hệ Thống Quản Lý Trường Học</small>
                        </body>
                        </html>";

                // Tạo MailMessage
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(SenderEmail, SenderName);
                    mail.To.Add(recipientEmail);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;

                    // Cấu hình SMTP
                    using (SmtpClient smtp = new SmtpClient(SmtpServer, SmtpPort))
                    {
                        smtp.Credentials = new NetworkCredential(SenderEmail, SenderPassword);
                        smtp.EnableSsl = true;

                        // Gửi email
                        smtp.Send(mail);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                string errorMsg = $"Lỗi gửi email: {ex.Message}\n\n" +
                                  $"Hãy đảm bảo App.config đã được cấu hình đúng với Email và 'Mật khẩu ứng dụng' (App Password).";
                Debug.WriteLine(errorMsg);
                MessageBox.Show(errorMsg, "Lỗi Gửi Email", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Gửi email chứa mã OTP để khôi phục mật khẩu
        /// </summary>
        public static bool SendOtpEmail(string recipientEmail, string otp)
        {
            try
            {
                string subject = "Yêu cầu đặt lại mật khẩu - Hệ thống Quản lý";
                string body = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif;'>
                        <h2 style='color: #007bff;'>Yêu cầu đặt lại mật khẩu</h2>
                        <p>Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn.</p>
                        <p>Mã OTP của bạn là:</p>
                        <div style='font-size: 24px; font-weight: bold; color: #dc3545; border: 1px dashed #ccc; padding: 10px; display: inline-block;'>
                            {otp}
                        </div>
                        <p>Mã này sẽ hết hạn trong <b>10 phút</b>.</p>
                        <p>Nếu bạn không yêu cầu điều này, vui lòng bỏ qua email.</p>
                        <hr/>
                        <small>Email này được gửi tự động từ Hệ Thống Quản Lý Trường Học</small>
                    </body>
                    </html>";

                // Tạo MailMessage
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(SenderEmail, SenderName);
                    mail.To.Add(recipientEmail);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;

                    // Cấu hình SMTP
                    using (SmtpClient smtp = new SmtpClient(SmtpServer, SmtpPort))
                    {
                        smtp.Credentials = new NetworkCredential(SenderEmail, SenderPassword);
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                string errorMsg = $"Lỗi gửi email OTP: {ex.Message}\n\n" +
                                  $"Hãy đảm bảo App.config đã được cấu hình đúng với Email và 'Mật khẩu ứng dụng' (App Password).";
                Debug.WriteLine(errorMsg);
                MessageBox.Show(errorMsg, "Lỗi Gửi Email", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}