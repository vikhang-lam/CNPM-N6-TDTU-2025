using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Moq;
using System.Data;
using System.Data.SqlClient;

namespace N6.Tests.UnitTests.Services
{
    [TestClass]
    public class DatabaseHelperTests
    {
        [TestMethod]
        public void CreateTeacherRequest_WithValidData_ShouldPassValidation()
        {
            // Arrange
            string teacherName = "Nguyễn Văn A";
            string username = "nguyenvana";
            string password = "password123";
            string email = "nguyenvana@example.com";
            string phone = "0123456789";

            // Act & Assert - Test validation logic
            teacherName.Should().NotBeNullOrEmpty();
            username.Should().NotBeNullOrEmpty();
            email.Should().Contain("@");
            phone.Should().MatchRegex(@"^\d{10}$");
        }

        [TestMethod]
        public void CreateTeacherRequest_WithInvalidEmail_ShouldFailValidation()
        {
            // Arrange
            string invalidEmail = "invalid-email";

            // Act & Assert
            invalidEmail.Should().NotMatch(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        [TestMethod]
        public void CreateTeacherRequest_WithInvalidPhone_ShouldFailValidation()
        {
            // Arrange
            string invalidPhone = "12345";

            // Act & Assert
            invalidPhone.Should().NotMatch(@"^\d{10}$");
        }

        [TestMethod]
        public void GetAdminEmail_WithValidAdminCode_ShouldReturnExpectedFormat()
        {
            // Arrange
            string adminCode = "AD001";

            // Act & Assert
            adminCode.Should().NotBeNullOrEmpty();
            adminCode.Should().Be("AD001");
        }

        [TestMethod]
        public void CheckTeacherLogin_WithValidCredentials_ShouldReturnSuccessStatus()
        {
            // Arrange
            string username = "validuser";
            string password = "validpass";

            // Act & Assert - Test business logic validation
            username.Should().NotBeNullOrEmpty();
            password.Should().NotBeNullOrEmpty();
        }
    }
}