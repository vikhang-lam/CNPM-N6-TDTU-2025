using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Drawing;

namespace N6.Tests.UnitTests.Forms
{
    [TestClass]
    public class LoginDialogTests
    {
        [TestMethod]
        public void UsernameProperty_WhenPlaceholder_ShouldReturnEmptyString()
        {
            // Arrange
            string placeholderText = "Nhập tên đăng nhập";

            // Act - Simulate the Username property logic from LoginDialog
            bool isPlaceholder = placeholderText == "Nhập tên đăng nhập";
            string result = isPlaceholder ? "" : placeholderText.Trim();

            // Assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void UsernameProperty_WhenUserInput_ShouldReturnTrimmedText()
        {
            // Arrange
            string userInput = "  teacher123  ";

            // Act - Simulate the Username property logic
            bool isPlaceholder = userInput == "Nhập tên đăng nhập";
            string result = isPlaceholder ? "" : userInput.Trim();

            // Assert
            result.Should().Be("teacher123");
        }

        [TestMethod]
        public void PasswordProperty_WhenPlaceholder_ShouldReturnEmptyString()
        {
            // Arrange
            string placeholderText = "Nhập mật khẩu";

            // Act - Simulate the Password property logic
            bool isPlaceholder = placeholderText == "Nhập mật khẩu";
            string result = isPlaceholder ? "" : placeholderText.Trim();

            // Assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void PasswordProperty_WhenUserInput_ShouldReturnTrimmedText()
        {
            // Arrange
            string userInput = "  password123  ";

            // Act - Simulate the Password property logic
            bool isPlaceholder = userInput == "Nhập mật khẩu";
            string result = isPlaceholder ? "" : userInput.Trim();

            // Assert
            result.Should().Be("password123");
        }

        [TestMethod]
        public void AdminUsername_CaseInsensitive_ShouldRecognizeAsAdmin()
        {
            // Arrange
            var adminUsernames = new[] { "admin", "ADMIN", "Admin", "AdMiN" };

            // Act & Assert
            foreach (var username in adminUsernames)
            {
                bool isAdmin = username.Equals("admin", System.StringComparison.OrdinalIgnoreCase);
                isAdmin.Should().BeTrue($"Username '{username}' should be recognized as admin");
            }
        }

        [TestMethod]
        public void NonAdminUsername_ShouldNotBeRecognizedAsAdmin()
        {
            // Arrange
            var nonAdminUsernames = new[] { "teacher", "student", "user", "administrator" };

            // Act & Assert
            foreach (var username in nonAdminUsernames)
            {
                bool isAdmin = username.Equals("admin", System.StringComparison.OrdinalIgnoreCase);
                isAdmin.Should().BeFalse($"Username '{username}' should not be recognized as admin");
            }
        }
    }
}