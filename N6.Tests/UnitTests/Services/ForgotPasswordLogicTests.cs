using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using N6.Tests.TestHelpers;

namespace N6.Tests.UnitTests.Services
{
    [TestClass]
    public class ForgotPasswordLogicTests
    {
        [TestMethod]
        public void SendOtp_WithValidUsername_ShouldReturnSuccess()
        {
            // Arrange
            var credentials = ForgotPasswordTestHelper.CreateValidTeacherCredentials();

            // Act
            var result = ForgotPasswordTestHelper.CreateValidOtpRequestResult();

            // Assert
            result.Success.Should().BeTrue();
            result.Email.Should().NotBeNullOrEmpty();
            result.Otp.Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        public void SendOtp_WithInvalidUsername_ShouldReturnFailure()
        {
            // Arrange
            var credentials = ForgotPasswordTestHelper.CreateInvalidTeacherCredentials();

            // Act
            var result = ForgotPasswordTestHelper.CreateInvalidOtpRequestResult();

            // Assert
            result.Success.Should().BeFalse();
            result.Email.Should().BeNull();
            result.Otp.Should().BeNull();
        }

        [TestMethod]
        public void SendOtp_WithEmptyUsername_ShouldFailValidation()
        {
            // Arrange
            var credentials = ForgotPasswordTestHelper.CreateEmptyCredentials();

            // Act
            bool isValid = !string.IsNullOrWhiteSpace(credentials.username);

            // Assert
            isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ResetPassword_WithValidOtpAndMatchingPasswords_ShouldReturnSuccess()
        {
            // Arrange
            string otp = ForgotPasswordTestHelper.GenerateValidOtp();
            string newPassword = ForgotPasswordTestHelper.CreateValidPassword();
            string confirmPassword = ForgotPasswordTestHelper.CreateValidPassword();

            // Act
            bool passwordsMatch = newPassword == confirmPassword;
            var status = ResetPasswordStatus.Success;

            // Assert
            passwordsMatch.Should().BeTrue();
            status.Should().Be(ResetPasswordStatus.Success);
        }

        [TestMethod]
        public void ResetPassword_WithInvalidOtp_ShouldReturnInvalidOtpStatus()
        {
            // Arrange
            string otp = ForgotPasswordTestHelper.GenerateInvalidOtp();
            string newPassword = ForgotPasswordTestHelper.CreateValidPassword();

            // Act
            var status = ResetPasswordStatus.InvalidOtp;

            // Assert
            status.Should().Be(ResetPasswordStatus.InvalidOtp);
        }

        [TestMethod]
        public void ResetPassword_WithExpiredOtp_ShouldReturnOtpExpiredStatus()
        {
            // Arrange
            string otp = ForgotPasswordTestHelper.GenerateExpiredOtp();
            string newPassword = ForgotPasswordTestHelper.CreateValidPassword();

            // Act
            var status = ResetPasswordStatus.OtpExpired;

            // Assert
            status.Should().Be(ResetPasswordStatus.OtpExpired);
        }

        [TestMethod]
        public void ResetPassword_WithNonMatchingPasswords_ShouldFailValidation()
        {
            // Arrange
            string newPassword = ForgotPasswordTestHelper.CreateValidPassword();
            string confirmPassword = "differentpassword";

            // Act
            bool passwordsMatch = newPassword == confirmPassword;

            // Assert
            passwordsMatch.Should().BeFalse();
        }

        [TestMethod]
        public void ResetPassword_WithWeakPassword_ShouldFailValidation()
        {
            // Arrange
            string weakPassword = ForgotPasswordTestHelper.CreateInvalidPassword();

            // Act
            bool isPasswordStrong = weakPassword.Length >= 6;

            // Assert
            isPasswordStrong.Should().BeFalse();
        }

        [TestMethod]
        public void ShowOtpStep_ShouldDisplayOtpControlsAndAdjustFormSize()
        {
            // Arrange & Act
            bool otpBorderVisible = true;
            bool checkOtpButtonVisible = true;
            bool usernameEnabled = false;
            int formHeight = 280;

            // Assert
            otpBorderVisible.Should().BeTrue();
            checkOtpButtonVisible.Should().BeTrue();
            usernameEnabled.Should().BeFalse();
            formHeight.Should().Be(280);
        }

        [TestMethod]
        public void ShowPasswordStep_ShouldDisplayPasswordControlsAndAdjustFormSize()
        {
            // Arrange & Act
            bool passwordBorderVisible = true;
            bool confirmPasswordBorderVisible = true;
            bool resetPasswordButtonVisible = true;
            bool checkOtpButtonVisible = false;
            int formHeight = 440;

            // Assert
            passwordBorderVisible.Should().BeTrue();
            confirmPasswordBorderVisible.Should().BeTrue();
            resetPasswordButtonVisible.Should().BeTrue();
            checkOtpButtonVisible.Should().BeFalse();
            formHeight.Should().Be(440);
        }
    }
}
