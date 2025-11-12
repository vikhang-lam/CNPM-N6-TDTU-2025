using System.Collections.Generic;
using System.Data;

namespace N6.Tests.TestHelpers
{
    /// <summary>
    /// Cung cấp dữ liệu giả và các đối tượng mẫu cho việc kiểm thử Mini-games.
    /// </summary>
    public static class MiniGameTestHelper
    {
        /// <summary>
        /// Tạo một danh sách các câu hỏi Quiz hợp lệ.
        /// </summary>
        public static List<QuizQuestion> CreateValidQuizQuestionsList()
        {
            return new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    QuestionText = "Thủ đô của Việt Nam là gì?",
                    Options = new List<string> { "Hà Nội", "TP. HCM", "Đà Nẵng", "Hải Phòng" },
                    CorrectAnswer = "A"
                },
                new QuizQuestion
                {
                    QuestionText = "1 + 1 bằng mấy?",
                    Options = new List<string> { "1", "2", "3", "4" },
                    CorrectAnswer = "B"
                }
            };
        }

        /// <summary>
        /// Tạo một danh sách câu hỏi Quiz rỗng.
        /// </summary>
        public static List<QuizQuestion> CreateEmptyQuizQuestionsList()
        {
            return new List<QuizQuestion>();
        }

        /// <summary>
        /// Mô phỏng chuỗi dữ liệu thô (raw string) được lưu trong CSDL cho MNG01.
        /// Định dạng: Question;Opt1;Opt2;Opt3;Opt4;CorrectAnswer|...
        /// </summary>
        public static string CreateValidSerializedQuizData()
        {
            return "Thủ đô của Việt Nam là gì?;Hà Nội;TP. HCM;Đà Nẵng;Hải Phòng;A|1 + 1 bằng mấy?;1;2;3;4;B";
        }

        /// <summary>
        /// Mô phỏng dữ liệu thô rỗng.
        /// </summary>
        public static string CreateEmptySerializedData()
        {
            return "";
        }

        /// <summary>
        /// Mô phỏng kết quả DataTable từ sp_GetMiniGames.
        /// </summary>
        public static DataTable CreateMockMiniGamesTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaMNG", typeof(string));
            dt.Columns.Add("Ten", typeof(string));

            dt.Rows.Add("MNG01", "Quiz nhanh");
            dt.Rows.Add("MNG02", "Vòng quay may mắn");
            dt.Rows.Add("MNG03", "Flashcard");

            return dt;
        }

        /// <summary>
        /// Mô phỏng kết quả chơi game (ví dụ: đúng 2/2 câu).
        /// </summary>
        public static (int Score, int Total) CreateValidQuizGameResult(int totalQuestions)
        {
            return (totalQuestions, totalQuestions);
        }

        /// <summary>
        /// Mô phỏng kết quả chơi game thất bại.
        /// </summary>
        public static (int Score, int Total) CreateFailedQuizGameResult(int totalQuestions)
        {
            return (0, totalQuestions);
        }
    }
}