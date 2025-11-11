using System;
using System.IO;
using System.Linq;
using System.Media;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace N6
{
    public static class MusicPlayer
    {
        #region Fields

        private static SoundPlayer _player;
        private static readonly Random Rng = new Random();

        #endregion

        #region Music Methods

        /// <summary>
        /// Phát file .wav được chỉ định ở chế độ lặp (looping).
        /// </summary>
        /// <param name="filePath">Đường dẫn đầy đủ tới file .wav cần phát.</param>
        public static void Play(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                MessageBox.Show($"Lỗi Play: File không tồn tại hoặc đường dẫn trống: {filePath}", "Lỗi Nhạc");
                return;
            }

            Stop();

            try
            {
                // Tạo SoundPlayer mới và phát lặp
                _player = new SoundPlayer(filePath);
                _player.PlayLooping();
            }
            catch (Exception ex)
            {
                // Hiển thị lỗi để hỗ trợ gỡ lỗi khi phát nhạc thất bại
                MessageBox.Show($"Lỗi phát nhạc SoundPlayer: {ex.Message}\nĐường dẫn: {filePath}", "Lỗi Phát Nhạc");
            }
        }

        /// <summary>
        /// Dừng phát nhạc hiện tại và giải phóng tài nguyên SoundPlayer.
        /// </summary>
        public static void Stop()
        {
            if (_player != null)
            {
                _player.Stop();
                _player.Dispose();
                _player = null;
            }
        }

        /// <summary>
        /// Phát nhạc nền cố định dựa trên mã Mini Game (maMNG).
        /// Nếu mã không được nhận diện hoặc file không tồn tại, sẽ gọi PlayRandom() làm dự phòng.
        /// </summary>
        /// <param name="maMNG">Mã Mini Game (ví dụ: "MNG01", "MNG03").</param>
        public static void PlaySpecificMusic(string maMNG)
        {
            string musicNumber;

            switch (maMNG.ToUpper())
            {
                case "MNG01": // Quiz Game
                    musicNumber = "1";
                    break;

                case "MNG03": // Flashcard
                    musicNumber = "2";
                    break;

                case "MNG04": // Ghép chữ
                    musicNumber = "3";
                    break;

                case "MNG06": // Sắp xếp câu
                    musicNumber = "4";
                    break;

                case "MNG07": // Điền từ
                    musicNumber = "5";
                    break;

                default:
                    // Nếu không nhận diện mã, chuyển sang nhạc ngẫu nhiên
                    PlayRandom();
                    return;
            }

            Stop();

            string resourcesPath = GetResourcesDirectory();

            // Tên file theo quy ước: musicGame[Số].wav
            string fileName = $"musicGame{musicNumber}.wav";
            string filePath = Path.Combine(resourcesPath, fileName);

            if (File.Exists(filePath))
            {
                // Dùng hàm Play đã có để phát lặp
                Play(filePath);
            }
            else
            {
                // Nếu file không tồn tại, thông báo và fallback sang nhạc random
                MessageBox.Show($"Cảnh báo: Không tìm thấy file nhạc: {fileName}. Chuyển sang Random.", "Lỗi Nhạc");
                PlayRandom();
            }
        }

        /// <summary>
        /// Chọn ngẫu nhiên một file theo mẫu "musicGameN.wav" trong thư mục Resources và phát.
        /// </summary>
        public static void PlayRandom()
        {
            Stop();

            string resourcesPath = GetResourcesDirectory();

            if (!Directory.Exists(resourcesPath))
            {
                // Nếu thư mục Resources không tồn tại thì không làm gì
                return;
            }

            // 1. Lấy tất cả file .wav trong thư mục Resources
            string[] allWavFiles = Directory.GetFiles(resourcesPath, "*.wav");

            // 2. Lọc các file theo mẫu: musicGame[số].wav
            string pattern = @"^musicGame\d+\.wav$";

            string[] musicFiles = allWavFiles
                .Where(file => Regex.IsMatch(Path.GetFileName(file), pattern, RegexOptions.IgnoreCase))
                .ToArray();

            if (musicFiles.Length == 0)
            {
                // Không tìm thấy file phù hợp
                return;
            }

            // Chọn ngẫu nhiên một file và phát
            string randomFile = musicFiles[Rng.Next(musicFiles.Length)];

            Play(randomFile);
        }

        /// <summary>
        /// Trả về đường dẫn tới thư mục Resources. Hàm thử nhiều đường dẫn để hỗ trợ môi trường phát triển và triển khai.
        /// </summary>
        /// <returns>Đường dẫn tới thư mục Resources (nếu không tồn tại thì trả về đường dẫn tiêu chuẩn).</returns>
        public static string GetResourcesDirectory()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            string standardPath = Path.Combine(baseDirectory, "Resources");

            if (Directory.Exists(standardPath))
            {
                return standardPath;
            }

            // Các đường dẫn fallback khi chạy trong môi trường phát triển (từ thư mục bin)
            string devPath = Path.GetFullPath(Path.Combine(baseDirectory, @"..\..\..\Resources"));

            if (Directory.Exists(devPath))
            {
                return devPath;
            }

            string oldDevPath = Path.GetFullPath(Path.Combine(baseDirectory, @"..\..\Resources"));

            if (Directory.Exists(oldDevPath))
            {
                return oldDevPath;
            }

            // Nếu không tìm thấy, trả về standardPath (có thể không tồn tại)
            return standardPath;
        }

        #endregion
    }
}