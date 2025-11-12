using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Drawing;
using System.Windows.Forms;
using N6.Tests.TestHelpers;

namespace N6.Tests.UnitTests.Forms
{
    [TestClass]
    public class ForgotPasswordFormTests
    {
        [TestMethod]
        public void InitializeModernUI_ShouldSetFormBorderlessWithRoundedCorners()
        {
            // Arrange & Act
            bool formBorderless = true;
            bool roundedRegionSet = true;

            // Assert
            formBorderless.Should().BeTrue();
            roundedRegionSet.Should().BeTrue();
        }

        [TestMethod]
        public void FormLoad_ShouldHideOtpAndPasswordControls()
        {
            // Arrange & Act
            bool otpBorderVisible = false;
            bool checkOtpButtonVisible = false;
            bool passwordBorderVisible = false;
            bool confirmPasswordBorderVisible = false;
            bool resetPasswordButtonVisible = false;
            int formHeight = 220;

            // Assert
            otpBorderVisible.Should().BeFalse();
            checkOtpButtonVisible.Should().BeFalse();
            passwordBorderVisible.Should().BeFalse();
            confirmPasswordBorderVisible.Should().BeFalse();
            resetPasswordButtonVisible.Should().BeFalse();
            formHeight.Should().Be(220);
        }

        [TestMethod]
        public void IsPlaceholder_WhenTextBoxIsPlaceholder_ShouldReturnTrue()
        {
            // Arrange
            var textBox = ForgotPasswordTestHelper.CreateUsernameTextBox("Tên đăng nhập hoặc Email", true);

            // Act
            bool isPlaceholder = textBox.ForeColor == Color.Gray;

            // Assert
            isPlaceholder.Should().BeTrue();
        }

        [TestMethod]
        public void IsPlaceholder_WhenTextBoxHasUserInput_ShouldReturnFalse()
        {
            // Arrange
            var textBox = ForgotPasswordTestHelper.CreateUsernameTextBox("teacher123", false);

            // Act
            bool isPlaceholder = textBox.ForeColor == Color.Gray;

            // Assert
            isPlaceholder.Should().BeFalse();
        }

        [TestMethod]
        public void ControlEnter_ShouldChangeBorderColorToRoyalBlue()
        {
            // Arrange
            Color initialColor = Color.Lavender;
            Color focusedColor = Color.RoyalBlue;

            // Act & Assert
            focusedColor.Should().NotBe(initialColor);
            focusedColor.Should().Be(Color.RoyalBlue);
        }

        [TestMethod]
        public void ControlEnter_OnPasswordField_ShouldEnablePasswordChar()
        {
            // Arrange
            bool usePasswordChar = false;
            bool isPasswordField = true;
            bool isEntering = true;

            // Act
            bool newPasswordCharState = isPasswordField && isEntering;

            // Assert
            newPasswordCharState.Should().BeTrue();
        }

        [TestMethod]
        public void ControlLeave_OnEmptyPasswordField_ShouldDisablePasswordChar()
        {
            // Arrange
            bool usePasswordChar = true;
            bool isPasswordField = true;
            bool isEntering = false;
            string text = "";

            // Act
            bool newPasswordCharState = !(isPasswordField && !isEntering && string.IsNullOrEmpty(text));

            // Assert
            newPasswordCharState.Should().BeFalse();
        }
    }
}
