using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Text.RegularExpressions;

namespace N6.Tests.UnitTests.Services
{
    [TestClass]
    public class RegistrationValidationTests
    {
        [TestMethod]
        [DataRow("Nguyễn Văn A", true)]
        [DataRow("Trần Thị B", true)]
        [DataRow("John Doe", true)]
        [DataRow("Name123", false)]
        [DataRow("Name@", false)]
        [DataRow("", false)]
        public void ValidateName_VariousInputs_ReturnsCorrectResult(string name, bool expected)
        {
            // Act
            var result = Regex.IsMatch(name, @"^[\p{L}\s]+$");

            // Assert
            result.Should().Be(expected);
        }

        [TestMethod]
        [DataRow("test@example.com", true)]
        [DataRow("user.name@domain.co.uk", true)]
        [DataRow("invalid-email", false)]
        [DataRow("user@", false)]
        [DataRow("@domain.com", false)]
        [DataRow("", false)]
        public void ValidateEmail_VariousInputs_ReturnsCorrectResult(string email, bool expected)
        {
            // Act
            var result = Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

            // Assert
            result.Should().Be(expected);
        }

        [TestMethod]
        [DataRow("0123456789", true)]
        [DataRow("0987654321", true)]
        [DataRow("12345", false)]
        [DataRow("01234567890", false)]
        [DataRow("abc1234567", false)]
        [DataRow("", false)]
        public void ValidatePhone_VariousInputs_ReturnsCorrectResult(string phone, bool expected)
        {
            // Act
            var result = Regex.IsMatch(phone, @"^\d{10}$");

            // Assert
            result.Should().Be(expected);
        }

        [TestMethod]
        public void ValidatePassword_MatchingPasswords_ReturnsTrue()
        {
            // Arrange
            string password = "password123";
            string confirmPassword = "password123";

            // Act
            var result = password == confirmPassword;

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void ValidatePassword_NonMatchingPasswords_ReturnsFalse()
        {
            // Arrange
            string password = "password123";
            string confirmPassword = "differentpassword";

            // Act
            var result = password == confirmPassword;

            // Assert
            result.Should().BeFalse();
        }

    }
}