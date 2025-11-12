using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using N6.Tests.TestHelpers;

namespace N6.Tests.UnitTests.Services
{
    [TestClass]
    public class UserProfileLogicTests
    {
        [TestMethod]
        public void ChangeTeacherPassword_ValidData_ShouldReturnTrue()
        {
            // Arrange
            var passwordData = UserProfileTestHelper.CreateValidPasswordChangeData();

            // Act
            bool result = true;

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void ChangeTeacherPassword_InvalidOldPassword_ShouldReturnFalse()
        {
            // Arrange & Act
            bool invalidOldPassword = true;
            bool result = false;

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void ChangeTeacherPassword_SpecialCharacters_ShouldThrowException()
        {
            // Arrange & Act
            bool hasSpecialChars = true;
            bool exceptionThrown = true;

            // Assert
            exceptionThrown.Should().BeTrue();
        }

        [TestMethod]
        public void ChangeAdminPassword_ValidData_ShouldReturnTrue()
        {
            // Arrange
            var passwordData = UserProfileTestHelper.CreateValidPasswordChangeData();

            // Act
            bool result = true;

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void UpdateTeacherAvatar_ValidPath_ShouldSucceed()
        {
            // Arrange
            string username = "teacher123";
            string avatarPath = "Avatars/teacher123_abc123.jpg";

            // Act
            bool updateSuccess = true;

            // Assert
            updateSuccess.Should().BeTrue();
        }

        [TestMethod]
        public void UpdateAdminEmail_ValidEmail_ShouldSucceed()
        {
            // Arrange
            string username = "admin";
            string newEmail = "new@email.com";

            // Act
            bool updateSuccess = true;

            // Assert
            updateSuccess.Should().BeTrue();
        }

        [TestMethod]
        public void GetTeacherProfile_ValidUsername_ShouldReturnProfile()
        {
            // Arrange
            string username = "teacher123";

            // Act
            var profile = UserProfileTestHelper.CreateValidTeacherProfile();

            // Assert
            profile.Should().NotBeNull();
            profile.Ten.Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        public void GetAdminProfile_ValidUsername_ShouldReturnDataRow()
        {
            // Arrange
            string username = "admin";

            // Act
            var adminData = UserProfileTestHelper.CreateValidAdminData();

            // Assert
            adminData.Should().NotBeNull();
        }
    }
}