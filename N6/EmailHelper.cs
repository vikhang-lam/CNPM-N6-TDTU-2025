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

        // (Không còn đọc AdminEmail từ App.config)

        /// <summary>
        /// Gửi email thông báo trạng thái tài khoản cho giáo viên
        /// </summary>
        // =================================================================
        // ### CẬP NHẬT 1: Thêm tham số "string adminEmail" ###
        // =================================================================
        public static bool SendAccountStatusEmail(string recipientEmail, string teacherName, bool isApproved, string adminEmail)
        {
            try
            {
                string subject = isApproved
                    ? "✅ Tài khoản của bạn đã được kích hoạt"
                    : "❌ Yêu cầu tạo tài khoản bị từ chối";

                // =================================================================
                // ### CẬP NHẬT 2: Sửa nội dung email từ chối theo yêu cầu của bạn ###
                // =================================================================
                string body = isApproved
                    ? $@"
                        <html>
                        <body style='font-family: Arial, sans-serif;'>
                            <h2 style='color: #28a745;'>Chào mừng {teacherName}!</h2>
                            <p>Tài khoản giáo viên của bạn đã được <b>kích hoạt thành công</b>.</p>
                            <p>Bạn có thể đăng nhập vào hệ thống ngay bây giờ.</p>
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
                            
                            <p>Nếu có câu hỏi gì, hãy liên hệ với quản trị viên qua email: <b>{adminEmail}</b></p>
                            
                            <hr/>
                            <small>Email này được gửi tự động từ Hệ Thống Quản Lý Trường Học</small>
                        </body>
                        </html>";

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(SenderEmail, SenderName);
                    mail.To.Add(recipientEmail);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;

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
                string errorMsg = $"Lỗi gửi email: {ex.Message}";
                Debug.WriteLine(errorMsg);
                MessageBox.Show(errorMsg, "Lỗi Gửi Email", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Gửi email chứa mã OTP để khôi phục mật khẩu (Giữ nguyên)
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

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(SenderEmail, SenderName);
                    mail.To.Add(recipientEmail);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;

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

        // =================================================================
        // ### HÀM MỚI: Gửi email xác nhận cho giáo viên ###
        // =================================================================
        public static bool SendRegistrationConfirmationEmail(string recipientEmail, string teacherName)
        {
            try
            {
                string subject = "✅ [Hệ Thống] Xác nhận đã nhận yêu cầu đăng ký";
                string body = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif;'>
                        <h2 style='color: #007bff;'>Chào {teacherName},</h2>
                        <p>Chúc mừng bạn đã gửi yêu cầu đăng ký tài khoản giáo viên thành công!</p>
                        <p>Yêu cầu của bạn đang được xem xét. Hệ thống sẽ phản hồi nhanh nhất có thể (thường là trong vòng 3 ngày làm việc).</p>
                        <hr/>
                        <small>Email này được gửi tự động từ Hệ Thống Quản Lý Trường Học</small>
                    </body>
                    </html>";

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(SenderEmail, SenderName);
                    mail.To.Add(recipientEmail);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;

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
                Debug.WriteLine($"Lỗi gửi email xác nhận đăng ký: {ex.Message}");
                return false;
            }
        }

        // =================================================================
        // ### HÀM ĐÃ SỬA: Gửi email thông báo cho Admin (nhận email làm tham số) ###
        // =================================================================
        public static bool SendAdminNotificationEmail(string adminRecipientEmail, string newTeacherName, string newTeacherUsername, string newTeacherEmail)
        {
            try
            {
                string subject = "🔔 [Hệ Thống] Có yêu cầu đăng ký tài khoản mới";
                string body = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif;'>
                        <h2 style='color: #fd7e14;'>Thông báo cho Admin,</h2>
                        <p>Có một giáo viên vừa gửi yêu cầu tạo tài khoản mới:</p>
                        <ul>
                            <li><strong>Họ và tên:</strong> {newTeacherName}</li>
                            <li><strong>Tên đăng nhập:</strong> {newTeacherUsername}</li>
                            <li><strong>Email:</strong> {newTeacherEmail}</li>
                        </ul>
                        <p>Vui lòng đăng nhập vào hệ thống quản trị để <b>Duyệt</b> hoặc <b>Từ chối</b> yêu cầu này.</p>
                    </body>
                    </html>";

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(SenderEmail, SenderName);
                    mail.To.Add(adminRecipientEmail); // Dùng email được truyền vào
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;

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
                Debug.WriteLine($"Lỗi gửi email thông báo cho admin: {ex.Message}");
                return false;
            }
        }
    }
}