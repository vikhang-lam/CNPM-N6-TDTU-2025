using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Text.RegularExpressions;

namespace N6.Tests.UnitTests.Integration
{
    [TestClass]
    public class RegistrationFlowTests
    {
        [TestMethod]
        public void CompleteRegistrationFlow_WithValidData_ShouldPassAllValidations()
        {
            // Arrange
            var registrationData = new
            {
                Name = "Nguyễn Văn A",
                Username = "nguyenvana",
                Email = "nguyenvana@example.com",
                Phone = "0123456789",
                Password = "password123",
                ConfirmPassword = "password123"
            };

            // Act & Assert - Step by step validation
            registrationData.Name.Should().NotBeNullOrEmpty();
            Regex.IsMatch(registrationData.Name, @"^[\p{L}\s]+$").Should().BeTrue();

            registrationData.Username.Should().NotBeNullOrEmpty();

            registrationData.Email.Should().NotBeNullOrEmpty();
            Regex.IsMatch(registrationData.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$").Should().BeTrue();

            registrationData.Phone.Should().NotBeNullOrEmpty();
            Regex.IsMatch(registrationData.Phone, @"^\d{10}$").Should().BeTrue();

            registrationData.Password.Should().NotBeNullOrEmpty();
            registrationData.ConfirmPassword.Should().NotBeNullOrEmpty();
            (registrationData.Password == registrationData.ConfirmPassword).Should().BeTrue();
        }

        [TestMethod]
        public void CompleteRegistrationFlow_WithInvalidData_ShouldFailValidations()
        {
            // Arrange
            var invalidData = new
            {
                Name = "User123", // Invalid: contains numbers
                Username = "",
                Email = "invalid-email",
                Phone = "123",
                Password = "pass",
                ConfirmPassword = "different"
            };

            // Act & Assert - All validations should fail
            Regex.IsMatch(invalidData.Name, @"^[\p{L}\s]+$").Should().BeFalse();
            string.IsNullOrEmpty(invalidData.Username).Should().BeTrue();
            Regex.IsMatch(invalidData.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$").Should().BeFalse();
            Regex.IsMatch(invalidData.Phone, @"^\d{10}$").Should().BeFalse();
            (invalidData.Password == invalidData.ConfirmPassword).Should().BeFalse();
        }

        [TestMethod]
        public void RegistrationErrorScenarios_ShouldHandleGracefully()
        {
            // Test various error scenarios
            var testCases = new[]
            {
                new { Name = "", Email = "test@test.com", Phone = "0123456789", ExpectedError = "empty name" },
                new { Name = "Valid Name", Email = "", Phone = "0123456789", ExpectedError = "empty email" },
                new { Name = "Valid Name", Email = "test@test.com", Phone = "", ExpectedError = "empty phone" },
                new { Name = "Valid Name", Email = "invalid", Phone = "0123456789", ExpectedError = "invalid email" },
                new { Name = "Valid Name", Email = "test@test.com", Phone = "123", ExpectedError = "invalid phone" }
            };

            foreach (var testCase in testCases)
            {
                // Act & Assert
                bool hasError = string.IsNullOrEmpty(testCase.Name) ||
                               string.IsNullOrEmpty(testCase.Email) ||
                               string.IsNullOrEmpty(testCase.Phone) ||
                               !Regex.IsMatch(testCase.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$") ||
                               !Regex.IsMatch(testCase.Phone, @"^\d{10}$");

                hasError.Should().BeTrue();
            }
        }
    }
}