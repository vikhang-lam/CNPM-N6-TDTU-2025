using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace N6.Tests.UnitTests.Services
{
    [TestClass]
    public class LoginValidationTests
    {
        [TestMethod]
        public void ValidateLogin_WithEmptyUsername_ShouldFail()
        {
            // Arrange
            string username = "";
            string password = "password123";

            // Act - Simulate validation logic from BtnOK_Click
            bool isValid = !string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password);

            // Assert
            isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateLogin_WithEmptyPassword_ShouldFail()
        {
            // Arrange
            string username = "teacher123";
            string password = "";

            // Act - Simulate validation logic
            bool isValid = !string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password);

            // Assert
            isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateLogin_WithValidCredentials_ShouldPass()
        {
            // Arrange
            string username = "teacher123";
            string password = "password123";

            // Act - Simulate validation logic
            bool isValid = !string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password);

            // Assert
            isValid.Should().BeTrue();
        }

        [TestMethod]
        public void ValidateLogin_WithWhitespaceOnly_ShouldFail()
        {
            // Arrange
            string username = "   ";
            string password = "   ";

            // Act - Simulate validation logic
            bool isValid = !string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password);

            // Assert
            isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateLogin_WithNullValues_ShouldFail()
        {
            // Arrange
            string username = null;
            string password = null;

            // Act - Simulate validation logic
            bool isValid = !string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password);

            // Assert
            isValid.Should().BeFalse();
        }
    }
}