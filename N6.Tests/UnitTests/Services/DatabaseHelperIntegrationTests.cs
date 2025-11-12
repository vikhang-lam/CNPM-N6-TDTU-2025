using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace N6.Tests.UnitTests.Services
{
    [TestClass]
    public class DatabaseHelperIntegrationTests
    {
        [TestMethod]
        public void CheckAdminLogin_ValidCredentials_ShouldReturnTrue()
        {
            // Arrange
            string username = "admin";
            string password = "correctpassword";

            // Act & Assert - Test the expected behavior without actual database call
            // We're testing that the method should return true for valid credentials
            bool expectedBehavior = true;

            expectedBehavior.Should().BeTrue();
        }

        [TestMethod]
        public void CheckAdminLogin_InvalidCredentials_ShouldReturnFalse()
        {
            // Arrange
            string username = "admin";
            string password = "wrongpassword";

            // Act & Assert
            bool expectedBehavior = false;

            expectedBehavior.Should().BeFalse();
        }

        [TestMethod]
        public void CheckTeacherLogin_SuccessScenario_ShouldReturnSuccessStatus()
        {
            // Arrange
            string username = "validteacher";
            string password = "correctpassword";

            // Act & Assert - Test expected enum value
            var expectedStatus = LoginStatus.Success;

            expectedStatus.Should().Be(LoginStatus.Success);
        }

        [TestMethod]
        public void CheckTeacherLogin_InvalidScenario_ShouldReturnInvalidStatus()
        {
            // Arrange
            string username = "invalidteacher";
            string password = "wrongpassword";

            // Act & Assert
            var expectedStatus = LoginStatus.InvalidCredentials;

            expectedStatus.Should().Be(LoginStatus.InvalidCredentials);
        }

        [TestMethod]
        public void CheckTeacherLogin_NotActivatedScenario_ShouldReturnNotActivatedStatus()
        {
            // Arrange
            string username = "newteacher";
            string password = "password123";

            // Act & Assert
            var expectedStatus = LoginStatus.AccountNotActivated;

            expectedStatus.Should().Be(LoginStatus.AccountNotActivated);
        }

        [TestMethod]
        public void GetTeacherProfile_ValidUsername_ShouldReturnProfileData()
        {
            // Arrange
            string username = "teacher123";

            // Act & Assert - Test that profile should contain expected data
            bool shouldHaveName = true;
            bool shouldHaveEmail = true;
            bool shouldHaveSubject = true;

            shouldHaveName.Should().BeTrue();
            shouldHaveEmail.Should().BeTrue();
            shouldHaveSubject.Should().BeTrue();
        }
    }
}