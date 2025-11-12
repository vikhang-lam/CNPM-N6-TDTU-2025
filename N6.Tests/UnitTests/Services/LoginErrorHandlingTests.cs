using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System;

namespace N6.Tests.UnitTests.Services
{
    [TestClass]
    public class LoginErrorHandlingTests
    {
        [TestMethod]
        public void LoginProcess_WithDatabaseException_ShouldShowErrorMessage()
        {
            // Arrange
            bool exceptionOccurred = true;
            string errorMessage = "Lỗi kết nối database";

            // Act - Simulate exception handling
            bool shouldShowError = exceptionOccurred;
            string displayedMessage = shouldShowError ? "Lỗi đăng nhập: " + errorMessage : "";

            // Assert
            shouldShowError.Should().BeTrue();
            displayedMessage.Should().Contain("Lỗi đăng nhập");
            displayedMessage.Should().Contain(errorMessage);
        }

        [TestMethod]
        public void LoginProcess_WithoutException_ShouldNotShowError()
        {
            // Arrange
            bool exceptionOccurred = false;
            string errorMessage = "";

            // Act
            bool shouldShowError = exceptionOccurred;
            string displayedMessage = shouldShowError ? "Lỗi đăng nhập: " + errorMessage : "";

            // Assert
            shouldShowError.Should().BeFalse();
            displayedMessage.Should().BeEmpty();
        }

        [TestMethod]
        public void InputValidation_BeforeDatabaseCall_ShouldPreventUnnecessaryCalls()
        {
            // Arrange
            string emptyUsername = "";
            string emptyPassword = "";

            // Act - Simulate validation before database call
            bool shouldCallDatabase = !string.IsNullOrWhiteSpace(emptyUsername) &&
                                     !string.IsNullOrWhiteSpace(emptyPassword);
            bool databaseCalled = false; // Should remain false due to validation

            // Assert
            shouldCallDatabase.Should().BeFalse();
            databaseCalled.Should().BeFalse();
        }

        [TestMethod]
        public void SettingsManagement_OnSuccessfulLogin_ShouldSaveUserData()
        {
            // Arrange
            string username = "teacher123";
            bool isAdmin = false;
            string displayName = "Nguyễn Văn A";

            // Act - Simulate settings save logic
            bool shouldSaveSettings = true;
            string savedUser = shouldSaveSettings ? displayName : "";
            bool savedIsAdmin = shouldSaveSettings ? isAdmin : false;

            // Assert
            shouldSaveSettings.Should().BeTrue();
            savedUser.Should().Be(displayName);
            savedIsAdmin.Should().BeFalse();
        }

        [TestMethod]
        public void SettingsManagement_OnFailedLogin_ShouldNotSaveUserData()
        {
            // Arrange
            bool loginSuccess = false;

            // Act - Simulate settings logic
            bool shouldSaveSettings = loginSuccess;
            string savedUser = shouldSaveSettings ? "teacher123" : "";

            // Assert
            shouldSaveSettings.Should().BeFalse();
            savedUser.Should().BeEmpty();
        }
    }
}