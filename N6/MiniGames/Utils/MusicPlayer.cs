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
        private static SoundPlayer _player;
        private static readonly Random Rng = new Random();

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
                _player = new SoundPlayer(filePath);
                _player.PlayLooping();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi phát nhạc SoundPlayer: {ex.Message}\nĐường dẫn: {filePath}", "Lỗi Phát Nhạc");
            }
        }

        public static void Stop()
        {
            if (_player != null)
            {
                _player.Stop();
                _player.Dispose();
                _player = null;
            }
        }

        public static void PlayRandom()
        {
            Stop();
            string resourcesPath = GetResourcesDirectory();

            if (!Directory.Exists(resourcesPath)) return;

            // 1. Lấy tất cả các file .wav
            string[] allWavFiles = Directory.GetFiles(resourcesPath, "*.wav");

            // 2. Lọc các file theo mẫu: musicGame[số].wav
            string pattern = @"^musicGame\d+\.wav$";

            string[] musicFiles = allWavFiles
                .Where(file => Regex.IsMatch(Path.GetFileName(file), pattern, RegexOptions.IgnoreCase))
                .ToArray();

            if (musicFiles.Length == 0) return;

            // Chọn ngẫu nhiên một file
            string randomFile = musicFiles[Rng.Next(musicFiles.Length)];

            Play(randomFile);
        }

        public static string GetResourcesDirectory()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            string standardPath = Path.Combine(baseDirectory, "Resources");

            if (Directory.Exists(standardPath))
            {
                return standardPath;
            }

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

            return standardPath;
        }
    }
}
