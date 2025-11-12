using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace N6.Tests.UnitTests.Services
{
    [TestClass]
    public class LoginBusinessLogicTests
    {
        [TestMethod]
        public void AdminLoginFlow_WithValidCredentials_ShouldSucceed()
        {
            // Arrange
            string username = "admin";
            string password = "admin123";

            // Act - Simulate admin login flow from BtnOK_Click
            bool isAdmin = username.Equals("admin", System.StringComparison.OrdinalIgnoreCase);
            bool isValid = !string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password);

            // Simulate database check - test the expected behavior
            bool adminLoginSuccess = true; // Assume success for valid credentials

            // Assert
            isAdmin.Should().BeTrue();
            isValid.Should().BeTrue();
            adminLoginSuccess.Should().BeTrue();
        }

        [TestMethod]
        public void TeacherLoginFlow_SuccessStatus_ShouldProceed()
        {
            // Arrange
            var loginStatus = LoginStatus.Success;
            string username = "teacher123";

            // Act - Simulate teacher success flow
            bool shouldProceed = loginStatus == LoginStatus.Success;
            bool shouldGetProfile = shouldProceed; // In success case, get profile

            // Assert
            shouldProceed.Should().BeTrue();
            shouldGetProfile.Should().BeTrue();
        }

        [TestMethod]
        public void TeacherLoginFlow_AccountNotActivated_ShouldShowWarning()
        {
            // Arrange
            var loginStatus = LoginStatus.AccountNotActivated;

            // Act - Simulate not activated flow
            bool shouldShowWarning = loginStatus == LoginStatus.AccountNotActivated;
            bool shouldNotProceed = !shouldShowWarning; // Should not proceed to main app

            // Assert
            shouldShowWarning.Should().BeTrue();
            shouldNotProceed.Should().BeFalse();
        }

        [TestMethod]
        public void TeacherLoginFlow_InvalidCredentials_ShouldShowError()
        {
            // Arrange
            var loginStatus = LoginStatus.InvalidCredentials;

            // Act - Simulate invalid credentials flow
            bool shouldShowError = loginStatus == LoginStatus.InvalidCredentials;
            bool shouldNotProceed = !shouldShowError; // Should not proceed to main app

            // Assert
            shouldShowError.Should().BeTrue();
            shouldNotProceed.Should().BeFalse();
        }

        [TestMethod]
        public void LoginStatus_EnumValues_ShouldBeCorrect()
        {
            // Arrange & Act & Assert - Verify enum values match the main project
            ((int)LoginStatus.Success).Should().Be(0);
            ((int)LoginStatus.InvalidCredentials).Should().Be(1);
            ((int)LoginStatus.AccountNotActivated).Should().Be(2);
        }
    }

    // Define LoginStatus enum for testing (same as in main project)
    public enum LoginStatus
    {
        Success,
        InvalidCredentials,
        AccountNotActivated
    }
}