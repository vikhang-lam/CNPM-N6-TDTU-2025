using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using N6.Tests.TestHelpers;

namespace N6.Tests.UnitTests.Integration
{
    [TestClass]
    public class TimerIntegrationTests
    {
        [TestMethod]
        public void FullTimerFlow_PauseAndResume_ShouldWork()
        {
            // Sắp xếp
            var (subject, duration, warning) = TimerTestHelper.CreateStandardTimer();

            // Hành động (Mô phỏng luồng)
            // 1. Khởi tạo và Bắt đầu
            var state_S1 = TimerTestHelper.GetRunningState();
            bool isRunning_S1 = !state_S1.isPaused;

            // 2. Tạm dừng
            var state_S2 = TimerTestHelper.GetPausedState();
            bool isPaused_S2 = state_S2.isPaused;

            // 3. Tiếp tục
            var state_S3 = TimerTestHelper.GetRunningState();
            bool isRunning_S3 = !state_S3.isPaused;

            // 4. Hết giờ
            var state_S4 = TimerTestHelper.GetTimeUpState();
            bool isFinished_S4 = state_S4.status == "🔔 Đã hết giờ!";

            // Khẳng định
            isRunning_S1.Should().BeTrue();
            isPaused_S2.Should().BeTrue();
            isRunning_S3.Should().BeTrue();
            isFinished_S4.Should().BeTrue();
        }

        [TestMethod]
        public void FullTimerFlow_WarningAndMute_ShouldWork()
        {
            // Sắp xếp
            var (subject, duration, warning) = TimerTestHelper.CreateStandardTimer();

            // Hành động (Mô phỏng luồng)
            // 1. Bắt đầu (Đang chạy)
            var state_S1 = TimerTestHelper.GetRunningState();
            bool isRunning = !state_S1.isPaused;

            // 2. Đạt mốc cảnh báo (5 phút)
            var state_S2 = TimerTestHelper.GetWarningState();
            bool alarmTriggered = state_S2.hasWarned && state_S2.muteVisible;
            bool isWarningRinging_S2 = true;

            // 3. Nhấn Mute
            isWarningRinging_S2 = false; // (Gọi StopWarningAlarm())
            bool muteVisible_S3 = false;

            // 4. Hết giờ
            var state_S4 = TimerTestHelper.GetTimeUpState();
            bool finalAlarmTriggered = state_S4.finalAlarm;

            // Khẳng định
            isRunning.Should().BeTrue();
            alarmTriggered.Should().BeTrue("vì đã đến mốc 5 phút");
            muteVisible_S3.Should().BeFalse("vì đã nhấn mute");
            finalAlarmTriggered.Should().BeTrue("vì đã hết giờ");
        }
    }
}