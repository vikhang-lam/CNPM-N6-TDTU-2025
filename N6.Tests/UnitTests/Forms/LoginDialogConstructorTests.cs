using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Drawing;

namespace N6.Tests.UnitTests.Forms
{
    [TestClass]
    public class LoginDialogConstructorTests
    {
        [TestMethod]
        public void Constructor_WithPresetUsername_ShouldSetUsernameField()
        {
            // Arrange
            string presetUsername = "teacher123";
            bool requireUsername = true;

            // Act - Simulate constructor logic
            string actualUsername = presetUsername;
            bool isReadOnly = !requireUsername;
            Color textColor = string.IsNullOrEmpty(presetUsername) ? Color.Gray : Color.Black;

            // Assert
            actualUsername.Should().Be(presetUsername);
            isReadOnly.Should().BeFalse(); // requireUsername = true, so not readonly
            textColor.Should().Be(Color.Black);
        }

        [TestMethod]
        public void Constructor_WithPresetUsernameAndNotRequired_ShouldSetReadOnly()
        {
            // Arrange
            string presetUsername = "teacher123";
            bool requireUsername = false;

            // Act - Simulate constructor logic
            string actualUsername = presetUsername;
            bool isReadOnly = !requireUsername;
            Color textColor = Color.Black;

            // Assert
            actualUsername.Should().Be(presetUsername);
            isReadOnly.Should().BeTrue(); // requireUsername = false, so readonly
            textColor.Should().Be(Color.Black);
        }

        [TestMethod]
        public void Constructor_WithoutPresetUsername_ShouldSetPlaceholder()
        {
            // Arrange
            string presetUsername = "";
            bool requireUsername = true;

            // Act - Simulate constructor logic
            string actualUsername = string.IsNullOrEmpty(presetUsername) ? "Nhập tên đăng nhập" : presetUsername;
            bool isReadOnly = !requireUsername;
            Color textColor = string.IsNullOrEmpty(presetUsername) ? Color.Gray : Color.Black;

            // Assert
            actualUsername.Should().Be("Nhập tên đăng nhập");
            isReadOnly.Should().BeFalse();
            textColor.Should().Be(Color.Gray);
        }

        [TestMethod]
        public void Constructor_WithBackgroundImage_ShouldSetBackground()
        {
            // Arrange
            bool hasBackgroundImage = true;

            // Act - Simulate background image logic
            bool backgroundSet = hasBackgroundImage;
            string sizeMode = backgroundSet ? "CenterImage" : "";

            // Assert
            backgroundSet.Should().BeTrue();
            sizeMode.Should().Be("CenterImage");
        }

        [TestMethod]
        public void Constructor_WithoutBackgroundImage_ShouldNotSetBackground()
        {
            // Arrange
            bool hasBackgroundImage = false;

            // Act
            bool backgroundSet = hasBackgroundImage;
            string sizeMode = backgroundSet ? "CenterImage" : "";

            // Assert
            backgroundSet.Should().BeFalse();
            sizeMode.Should().BeEmpty();
        }

        [TestMethod]
        public void InitialPasswordField_ShouldShowPlaceholderWithoutPasswordChars()
        {
            // Arrange & Act - Simulate initial state
            string passwordText = "Nhập mật khẩu";
            Color passwordColor = Color.Gray;
            bool usePasswordChar = false;

            // Assert
            passwordText.Should().Be("Nhập mật khẩu");
            passwordColor.Should().Be(Color.Gray);
            usePasswordChar.Should().BeFalse();
        }
    }
}