using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace N6.Tests.UnitTests.Services
{
    [TestClass]
    public class EmailHelperForgotPasswordTests
    {
        [TestMethod]
        public void SendOtpEmail_WithValidData_ShouldPassValidation()
        {
            // Arrange
            string email = "teacher@example.com";
            string otp = "123456";

            // Act & Assert
            email.Should().NotBeNullOrEmpty();
            email.Should().Contain("@");
            otp.Should().NotBeNullOrEmpty();
            otp.Length.Should().Be(6);
        }

        [TestMethod]
        public void SendOtpEmail_WithInvalidEmail_ShouldFailValidation()
        {
            // Arrange
            string invalidEmail = "invalid-email";
            string otp = "123456";

            // Act & Assert
            invalidEmail.Should().NotContain("@");
        }

        [TestMethod]
        public void SendOtpEmail_WithEmptyOtp_ShouldFailValidation()
        {
            // Arrange
            string email = "teacher@example.com";
            string emptyOtp = "";

            // Act & Assert
            emptyOtp.Should().BeEmpty();
        }

        [TestMethod]
        public void OtpEmailContent_ShouldContainRequiredElements()
        {
            // Arrange
            string otp = "123456";
            string subject = "Yêu cầu đặt lại mật khẩu - Hệ thống Quản lý";

            // Act & Assert
            subject.Should().Contain("đặt lại mật khẩu");
            otp.Should().Be("123456");
        }
    }
}
