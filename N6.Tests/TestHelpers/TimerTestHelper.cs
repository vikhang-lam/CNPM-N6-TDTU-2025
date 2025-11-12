namespace N6.Tests.TestHelpers
{
    /// <summary>
    /// Cung cấp dữ liệu mẫu để khởi tạo frmTimerPopup.
    /// </summary>
    public static class TimerTestHelper
    {
        /// <summary>
        /// Tạo một bộ hẹn giờ 10 phút, cảnh báo 5 phút.
        /// </summary>
        public static (string subject, int duration, int warning) CreateStandardTimer()
        {
            return ("Toán", 10, 5);
        }

        /// <summary>
        /// Tạo một bộ hẹn giờ 1 phút, cảnh báo 1 phút (để test nhanh).
        /// </summary>
        public static (string subject, int duration, int warning) CreateShortTimer()
        {
            return ("Test Hết Giờ", 1, 1);
        }

        /// <summary>
        /// Mô phỏng trạng thái đã tạm dừng.
        /// </summary>
        public static (bool isPaused, string btnText, string status) GetPausedState()
        {
            return (true, "▶", "⏸️ Đã tạm dừng");
        }

        /// <summary>
        /// Mô phỏng trạng thái đang chạy.
        /// </summary>
        public static (bool isPaused, string btnText, string status) GetRunningState()
        {
            return (false, "⏸", "⏳ Đang đếm ngược...");
        }

        /// <summary>
        /// Mô phỏng trạng thái cảnh báo (5 phút cuối).
        /// </summary>
        public static (bool hasWarned, string status, bool muteVisible) GetWarningState()
        {
            return (true, "⚠️ Còn 5 phút!", true);
        }

        /// <summary>
        /// Mô phỏng trạng thái hết giờ.
        /// </summary>
        public static (string time, string status, bool finalAlarm) GetTimeUpState()
        {
            return ("00:00", "🔔 Đã hết giờ!", true);
        }
    }
}