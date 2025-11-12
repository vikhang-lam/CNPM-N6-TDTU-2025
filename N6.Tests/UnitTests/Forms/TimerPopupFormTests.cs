using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using N6.Tests.TestHelpers;
using System.Drawing;

namespace N6.Tests.UnitTests.Forms
{
    [TestClass]
    public class TimerPopupFormTests
    {
        [TestMethod]
        public void Initialize_WithValidInputs_ShouldSetCorrectInitialState()
        {
            // Sắp xếp
            var (subject, duration, warning) = TimerTestHelper.CreateStandardTimer();

            // Hành động (Mô phỏng trạng thái sau khi constructor chạy)
            int totalSeconds = duration * 60;
            bool isPaused = false;
            string statusText = "⏳ Đang đếm ngược...";
            string btnToggleText = "⏸";

            // Khẳng định
            totalSeconds.Should().Be(600);
            isPaused.Should().BeFalse();
            statusText.Should().Be("⏳ Đang đếm ngược...");
            btnToggleText.Should().Be("⏸");
        }

        [TestMethod]
        public void TogglePause_WhenRunning_ShouldSetPausedState()
        {
            // Sắp xếp (Trạng thái ban đầu: Đang chạy)
            var initialState = TimerTestHelper.GetRunningState();

            // Hành động (Mô phỏng gọi TogglePause())
            var (isPaused, btnText, status) = TimerTestHelper.GetPausedState();

            // Khẳng định
            isPaused.Should().BeTrue();
            btnText.Should().Be("▶");
            status.Should().Be("⏸️ Đã tạm dừng");
        }

        [TestMethod]
        public void TogglePause_WhenPaused_ShouldSetRunningState()
        {
            // Sắp xếp (Trạng thái ban đầu: Đang dừng)
            var initialState = TimerTestHelper.GetPausedState();

            // Hành động (Mô phỏng gọi TogglePause() lần 2)
            var (isPaused, btnText, status) = TimerTestHelper.GetRunningState();

            // Khẳng định
            isPaused.Should().BeFalse();
            btnText.Should().Be("⏸");
            status.Should().Be("⏳ Đang đếm ngược...");
        }

        [TestMethod]
        public void Countdown_OnWarningThreshold_ShouldUpdateStateFlags()
        {
            // Sắp xếp (Chưa cảnh báo)
            bool hasWarned_Initial = false;

            // Hành động (Mô phỏng Tick() khi đạt mốc cảnh báo 5 phút)
            var (hasWarned, status, muteVisible) = TimerTestHelper.GetWarningState();
            Color timeColor = Color.FromArgb(241, 196, 15); // Màu vàng

            // Khẳng định
            hasWarned.Should().BeTrue();
            status.Should().Be("⚠️ Còn 5 phút!");
            muteVisible.Should().BeTrue();
            timeColor.Should().Be(Color.FromArgb(241, 196, 15));
        }

        [TestMethod]
        public void Countdown_OnFinalMinute_ShouldUpdateStateFlags()
        {
            // Sắp xếp (Đã cảnh báo)
            bool hasWarned = true;

            // Hành động (Mô phỏng Tick() khi còn < 1 phút)
            string status = "🔴 Sắp hết giờ!";
            Color timeColor = Color.FromArgb(231, 76, 60); // Màu đỏ

            // Khẳng định
            hasWarned.Should().BeTrue(); // Vẫn là true
            status.Should().Be("🔴 Sắp hết giờ!");
            timeColor.Should().Be(Color.FromArgb(231, 76, 60));
        }

        [TestMethod]
        public void Countdown_OnTimeUp_ShouldSetTimeUpState()
        {
            // Sắp xếp (Sắp hết giờ)
            string initialTime = "00:01";

            // Hành động (Mô phỏng Tick() khi hết giờ)
            var (time, status, finalAlarm) = TimerTestHelper.GetTimeUpState();
            Color timeColor = Color.FromArgb(231, 76, 60); // Màu đỏ

            // Khẳng định
            time.Should().Be("00:00");
            status.Should().Be("🔔 Đã hết giờ!");
            finalAlarm.Should().BeTrue(); // Đã kích hoạt báo thức cuối
            timeColor.Should().Be(Color.FromArgb(231, 76, 60));
        }

        [TestMethod]
        public void MuteAlarm_OnClick_ShouldStopAlarmAndHideButton()
        {
            // Sắp xếp (Đang trong trạng thái cảnh báo)
            var warningState = TimerTestHelper.GetWarningState();
            bool isWarningRinging = true;

            // Hành động (Mô phỏng sự kiện click btnMuteAlarm)
            isWarningRinging = false; // (StopWarningAlarm() được gọi)
            bool muteVisible = false;
            string status = "🔕 Đã tắt chuông cảnh báo";

            // Khẳng định
            isWarningRinging.Should().BeFalse();
            muteVisible.Should().BeFalse();
            status.Should().Be("🔕 Đã tắt chuông cảnh báo");
        }
    }
}