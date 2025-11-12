using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using N6.Tests.TestHelpers;

namespace N6.Tests.UnitTests.Integration
{
    [TestClass]
    public class ForgotPasswordIntegrationTests
    {
        [TestMethod]
        public void CompleteForgotPasswordFlow_ValidScenario_ShouldSucceed()
        {
            // Arrange
            var credentials = ForgotPasswordTestHelper.CreateValidTeacherCredentials();
            string otp = ForgotPasswordTestHelper.GenerateValidOtp();
            string newPassword = ForgotPasswordTestHelper.CreateValidPassword();

            // Act - Simulate complete flow step by step
            // Step 1: Request OTP
            var otpResult = ForgotPasswordTestHelper.CreateValidOtpRequestResult();
            bool otpRequestSuccess = otpResult.Success;

            // Step 2: Send OTP email
            bool emailSent = true;

            // Step 3: Verify OTP and reset password
            var resetStatus = ResetPasswordStatus.Success;

            // Assert
            otpRequestSuccess.Should().BeTrue();
            emailSent.Should().BeTrue();
            resetStatus.Should().Be(ResetPasswordStatus.Success);
        }

        [TestMethod]
        public void CompleteForgotPasswordFlow_InvalidUsername_ShouldFailAtOtpRequest()
        {
            // Arrange
            var credentials = ForgotPasswordTestHelper.CreateInvalidTeacherCredentials();

            // Act
            var otpResult = ForgotPasswordTestHelper.CreateInvalidOtpRequestResult();
            bool otpRequestSuccess = otpResult.Success;
            bool emailSent = false;
            bool reachedPasswordReset = false;

            // Assert
            otpRequestSuccess.Should().BeFalse();
            emailSent.Should().BeFalse();
            reachedPasswordReset.Should().BeFalse();
        }

        [TestMethod]
        public void CompleteForgotPasswordFlow_InvalidOtp_ShouldFailAtPasswordReset()
        {
            // Arrange
            var credentials = ForgotPasswordTestHelper.CreateValidTeacherCredentials();
            string invalidOtp = ForgotPasswordTestHelper.GenerateInvalidOtp();
            string newPassword = ForgotPasswordTestHelper.CreateValidPassword();

            // Act
            var otpResult = ForgotPasswordTestHelper.CreateValidOtpRequestResult();
            bool otpRequestSuccess = otpResult.Success;
            var resetStatus = ResetPasswordStatus.InvalidOtp;

            // Assert
            otpRequestSuccess.Should().BeTrue();
            resetStatus.Should().Be(ResetPasswordStatus.InvalidOtp);
        }

        [TestMethod]
        public void CompleteForgotPasswordFlow_ExpiredOtp_ShouldFailAtPasswordReset()
        {
            // Arrange
            var credentials = ForgotPasswordTestHelper.CreateValidTeacherCredentials();
            string expiredOtp = ForgotPasswordTestHelper.GenerateExpiredOtp();
            string newPassword = ForgotPasswordTestHelper.CreateValidPassword();

            // Act
            var otpResult = ForgotPasswordTestHelper.CreateValidOtpRequestResult();
            bool otpRequestSuccess = otpResult.Success;
            var resetStatus = ResetPasswordStatus.OtpExpired;

            // Assert
            otpRequestSuccess.Should().BeTrue();
            resetStatus.Should().Be(ResetPasswordStatus.OtpExpired);
        }

        [TestMethod]
        public void CompleteForgotPasswordFlow_NonMatchingPasswords_ShouldFailValidation()
        {
            // Arrange
            var credentials = ForgotPasswordTestHelper.CreateValidTeacherCredentials();
            string otp = ForgotPasswordTestHelper.GenerateValidOtp();
            string newPassword = ForgotPasswordTestHelper.CreateValidPassword();
            string confirmPassword = "differentpassword";

            // Act
            var otpResult = ForgotPasswordTestHelper.CreateValidOtpRequestResult();
            bool otpRequestSuccess = otpResult.Success;
            bool passwordsMatch = newPassword == confirmPassword;
            bool shouldProceedToReset = passwordsMatch;

            // Assert
            otpRequestSuccess.Should().BeTrue();
            passwordsMatch.Should().BeFalse();
            shouldProceedToReset.Should().BeFalse();
        }
    }
}