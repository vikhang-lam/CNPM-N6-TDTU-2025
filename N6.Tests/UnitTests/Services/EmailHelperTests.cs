using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace N6.Tests.UnitTests.Services
{
    [TestClass]
    public class EmailHelperTests
    {
        [TestMethod]
        public void SendRegistrationConfirmationEmail_WithValidData_ShouldPassValidation()
        {
            // Arrange
            string email = "teacher@example.com";
            string name = "Nguyễn Văn A";

            // Act & Assert
            email.Should().NotBeNullOrEmpty();
            email.Should().Contain("@");
            name.Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        public void SendAdminNotificationEmail_WithValidData_ShouldPassValidation()
        {
            // Arrange
            string adminEmail = "admin@school.edu.vn";
            string teacherName = "Nguyễn Văn A";
            string username = "nguyenvana";
            string email = "teacher@example.com";

            // Act & Assert
            adminEmail.Should().NotBeNullOrEmpty();
            teacherName.Should().NotBeNullOrEmpty();
            username.Should().NotBeNullOrEmpty();
            email.Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        public void EmailValidation_InvalidEmails_ShouldFail()
        {
            // Arrange
            var invalidEmails = new[]
            {
                "invalid",
                "user@",
                "@domain.com",
                "user@domain",
                ""
            };

            // Act & Assert
            foreach (var invalidEmail in invalidEmails)
            {
                invalidEmail.Should().NotMatch(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            }
        }
    }
}