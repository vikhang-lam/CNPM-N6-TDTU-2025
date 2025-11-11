using System;
using System.Collections.Generic;
using System.Linq;

namespace N6
{
    public static class GameDataManager
    {
        #region Private Helpers

        /// <summary>
        /// Lấy dữ liệu thô NVARCHAR(MAX) cho minigame từ cơ sở dữ liệu.
        /// </summary>
        /// <param name="maMNG">Mã minigame (MaMNG).</param>
        /// <returns>Chuỗi thô được lưu hoặc chuỗi rỗng nếu có lỗi.</returns>
        private static string GetRawData(string maMNG)
        {
            try
            {
                return DatabaseHelper.GetGameData(maMNG);
            }
            catch
            {
                // Nếu gọi DB thất bại, trả về chuỗi rỗng để giữ an toàn cho các caller.
                return "";
            }
        }

        /// <summary>
        /// Lưu dữ liệu thô NVARCHAR(MAX) cho minigame vào cơ sở dữ liệu.
        /// </summary>
        /// <param name="maMNG">Mã minigame (MaMNG).</param>
        /// <param name="rawData">Chuỗi đã được serialize để lưu.</param>
        private static void SaveRawData(string maMNG, string rawData)
        {
            try
            {
                DatabaseHelper.SaveGameData(maMNG, rawData);
            }
            catch
            {
                // Bỏ qua ngoại lệ để tránh phá vỡ luồng UI (hành vi hiện tại).
            }
        }

        #endregion

        #region Quiz (QuizQuestion)

        /// <summary>
        /// Lấy danh sách câu hỏi quiz cho minigame tương ứng.
        /// Mỗi dòng trong dữ liệu lưu trữ: Question;Opt1;Opt2;Opt3;Opt4;CorrectAnswer, phân tách bằng '|'.
        /// </summary>
        /// <param name="maMNG">Mã minigame (MaMNG).</param>
        /// <returns>Danh sách <see cref="QuizQuestion"/> được phân tích từ lưu trữ. Trả về danh sách rỗng nếu không có dữ liệu.</returns>
        public static List<QuizQuestion> GetQuizQuestions(string maMNG)
        {
            var data = new List<QuizQuestion>();
            string rawData = GetRawData(maMNG);
            if (string.IsNullOrWhiteSpace(rawData))
            {
                return data;
            }

            // Tách theo '|' để lấy từng dòng câu hỏi; bỏ qua entry rỗng.
            var lines = rawData.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var parts = line.Split(';');
                // Mong đợi chính xác 6 phần: question + 4 options + correct answer
                if (parts.Length == 6)
                {
                    data.Add(new QuizQuestion
                    {
                        QuestionText = parts[0],
                        Options = new List<string>
                        {
                            parts[1],
                            parts[2],
                            parts[3],
                            parts[4]
                        },
                        // Chuẩn hóa đáp án đúng: trim và chữ hoa để so sánh.
                        CorrectAnswer = parts[5].Trim().ToUpper()
                    });
                }
            }

            return data;
        }

        /// <summary>
        /// Lưu danh sách câu hỏi quiz cho một minigame.
        /// Định dạng lưu: QuestionText;Opt1;Opt2;Opt3;Opt4;CorrectAnswer, các câu hỏi phân tách bằng '|'.
        /// </summary>
        /// <param name="maMNG">Mã minigame (MaMNG).</param>
        /// <param name="questions">Danh sách câu hỏi cần lưu.</param>
        public static void SaveQuizQuestions(string maMNG, List<QuizQuestion> questions)
        {
            // Trim các giá trị và ghép bằng ';' cho mỗi câu hỏi, sau đó ghép các câu hỏi bằng '|'.
            SaveRawData(maMNG, string.Join("|", questions.Select(q => $"{q.QuestionText.Trim()};{string.Join(";", q.Options.Select(o => o.Trim()))};{q.CorrectAnswer.Trim().ToUpper()}")));
        }

        #endregion

        #region Simple List

        /// <summary>
        /// Tải một danh sách đơn giản được phân tách bằng dấu chấm phẩy từ storage.
        /// </summary>
        /// <param name="maMNG">Mã minigame (MaMNG).</param>
        /// <returns>Danh sách chuỗi đã được trim. Trả về danh sách rỗng khi không có dữ liệu.</returns>
        public static List<string> GetListFromString(string maMNG)
        {
            string rawData = GetRawData(maMNG);
            if (string.IsNullOrWhiteSpace(rawData))
            {
                return new List<string>();
            }

            // Tách theo ';' và trim từng phần tử.
            return rawData.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                          .Select(s => s.Trim())
                          .ToList();
        }

        /// <summary>
        /// Lưu một danh sách chuỗi dưới dạng chuỗi phân tách bởi dấu chấm phẩy.
        /// </summary>
        /// <param name="maMNG">Mã minigame (MaMNG).</param>
        /// <param name="items">Các phần tử cần lưu.</param>
        public static void SaveListToString(string maMNG, List<string> items)
        {
            SaveRawData(maMNG, string.Join(";", items.Select(s => s.Trim())));
        }

        #endregion

        #region Flashcards

        /// <summary>
        /// Tải các mục flashcard từ storage.
        /// Định dạng lưu mỗi mục: Term;Definition phân tách bằng '|'.
        /// </summary>
        /// <param name="maMNG">Mã minigame (MaMNG).</param>
        /// <returns>Danh sách <see cref="FlashcardItem"/>.</returns>
        public static List<FlashcardItem> GetFlashcardItems(string maMNG)
        {
            var data = new List<FlashcardItem>();
            string rawData = GetRawData(maMNG);
            if (string.IsNullOrWhiteSpace(rawData))
            {
                return data;
            }

            var lines = rawData.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var parts = line.Split(';');
                // Mong đợi Term;Definition
                if (parts.Length == 2)
                {
                    data.Add(new FlashcardItem
                    {
                        Term = parts[0].Trim(),
                        Definition = parts[1].Trim()
                    });
                }
            }

            return data;
        }

        /// <summary>
        /// Lưu các mục flashcard vào storage.
        /// </summary>
        /// <param name="maMNG">Mã minigame (MaMNG).</param>
        /// <param name="items">Các flashcard cần lưu.</param>
        public static void SaveFlashcardItems(string maMNG, List<FlashcardItem> items)
        {
            SaveRawData(maMNG, string.Join("|", items.Select(item => $"{item.Term.Trim()};{item.Definition.Trim()}")));
        }

        #endregion

        #region Word Scramble

        /// <summary>
        /// Tải các mục word-scramble.
        /// Định dạng lưu mỗi dòng: ImageHintResourceName;Question;Answer phân tách bằng '|'.
        /// </summary>
        /// <param name="maMNG">Mã minigame (MaMNG).</param>
        /// <returns>Danh sách <see cref="WordScrambleItem"/>.</returns>
        public static List<WordScrambleItem> GetWordScrambleItems(string maMNG)
        {
            var data = new List<WordScrambleItem>();
            string rawData = GetRawData(maMNG);
            if (string.IsNullOrWhiteSpace(rawData))
            {
                return data;
            }

            var lines = rawData.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var parts = line.Split(';');
                if (parts.Length == 3)
                {
                    data.Add(new WordScrambleItem
                    {
                        ImageHintResourceName = parts[0],
                        Question = parts[1],
                        Answer = parts[2].ToUpper()
                    });
                }
            }

            return data;
        }

        /// <summary>
        /// Lưu các mục word-scramble.
        /// </summary>
        /// <param name="maMNG">Mã minigame (MaMNG).</param>
        /// <param name="items">Các mục cần lưu.</param>
        public static void SaveWordScrambleItems(string maMNG, List<WordScrambleItem> items)
        {
            SaveRawData(maMNG, string.Join("|", items.Select(item => $"{item.ImageHintResourceName};{item.Question};{item.Answer}")));
        }

        #endregion

        #region Sentence Scramble

        /// <summary>
        /// Tải các mục sentence-scramble (mỗi câu phân tách bằng '|').
        /// </summary>
        /// <param name="maMNG">Mã minigame (MaMNG).</param>
        /// <returns>Danh sách <see cref="SentenceScrambleItem"/>.</returns>
        public static List<SentenceScrambleItem> GetSentenceScrambleItems(string maMNG)
        {
            var data = new List<SentenceScrambleItem>();
            string rawData = GetRawData(maMNG);
            if (string.IsNullOrWhiteSpace(rawData))
            {
                return data;
            }

            // Tách theo '|' để lấy từng câu; trim và bỏ qua entry rỗng.
            var sentences = rawData.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var sentence in sentences)
            {
                var trimmedSentence = sentence.Trim();
                if (!string.IsNullOrEmpty(trimmedSentence))
                {
                    data.Add(new SentenceScrambleItem
                    {
                        CorrectSentence = trimmedSentence
                    });
                }
            }

            return data;
        }

        /// <summary>
        /// Lưu tập hợp các mục sentence-scramble.
        /// </summary>
        /// <param name="maMNG">Mã minigame (MaMNG).</param>
        /// <param name="items">Các mục cần lưu.</param>
        public static void SaveSentenceScrambleItems(string maMNG, List<SentenceScrambleItem> items)
        {
            var sentences = items.Where(item => !string.IsNullOrWhiteSpace(item.CorrectSentence))
                                 .Select(item => item.CorrectSentence.Trim());
            SaveRawData(maMNG, string.Join("|", sentences));
        }

        /// <summary>
        /// Lấy một mục sentence-scramble đơn lẻ (giá trị thô).
        /// </summary>
        /// <param name="maMNG">Mã minigame (MaMNG).</param>
        /// <returns><see cref="SentenceScrambleItem"/> với câu đúng được lưu.</returns>
        public static SentenceScrambleItem GetSentenceScrambleItem(string maMNG)
        {
            return new SentenceScrambleItem
            {
                CorrectSentence = GetRawData(maMNG) ?? ""
            };
        }

        /// <summary>
        /// Lưu một mục sentence-scramble đơn lẻ.
        /// </summary>
        /// <param name="maMNG">Mã minigame (MaMNG).</param>
        /// <param name="item">Mục cần lưu.</param>
        public static void SaveSentenceScrambleItem(string maMNG, SentenceScrambleItem item)
        {
            SaveRawData(maMNG, item.CorrectSentence);
        }

        #endregion

        #region Fill-the-Blank

        /// <summary>
        /// Tải các câu hỏi điền từ.
        /// Định dạng lưu mỗi mục: QuestionText;Answer phân tách bằng '|'.
        /// </summary>
        /// <param name="maMNG">Mã minigame (MaMNG).</param>
        /// <returns>Danh sách <see cref="FillBlankQuestion"/>.</returns>
        public static List<FillBlankQuestion> GetFillBlankQuestions(string maMNG)
        {
            var data = new List<FillBlankQuestion>();
            string rawData = GetRawData(maMNG);
            if (string.IsNullOrWhiteSpace(rawData))
            {
                return data;
            }

            var lines = rawData.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var parts = line.Split(';');
                if (parts.Length == 2 && !string.IsNullOrWhiteSpace(parts[0]))
                {
                    data.Add(new FillBlankQuestion
                    {
                        QuestionText = parts[0].Trim(),
                        Answer = parts[1].Trim()
                    });
                }
            }

            return data;
        }

        /// <summary>
        /// Lưu các câu hỏi điền từ.
        /// </summary>
        /// <param name="maMNG">Mã minigame (MaMNG).</param>
        /// <param name="questions">Các câu hỏi cần lưu.</param>
        public static void SaveFillBlankQuestions(string maMNG, List<FillBlankQuestion> questions)
        {
            var lines = questions.Where(q => !string.IsNullOrWhiteSpace(q.QuestionText))
                                 .Select(q => $"{q.QuestionText.Trim()};{q.Answer.Trim()}");
            SaveRawData(maMNG, string.Join("|", lines));
        }

        /// <summary>
        /// Tải một câu hỏi điền từ đơn lẻ lưu dưới dạng "Question;Answer".
        /// </summary>
        /// <param name="maMNG">Mã minigame (MaMNG).</param>
        /// <returns>Đối tượng <see cref="FillBlankQuestion"/> đã phân tích (giá trị rỗng khi không có).</returns>
        public static FillBlankQuestion GetFillBlankQuestion(string maMNG)
        {
            string rawData = GetRawData(maMNG);
            if (string.IsNullOrWhiteSpace(rawData))
            {
                return new FillBlankQuestion
                {
                    QuestionText = "",
                    Answer = ""
                };
            }

            var parts = rawData.Split(';');
            return new FillBlankQuestion
            {
                QuestionText = parts.Length > 0 ? parts[0] : "",
                Answer = parts.Length > 1 ? parts[1] : ""
            };
        }

        /// <summary>
        /// Lưu một câu hỏi điền từ đơn lẻ.
        /// </summary>
        /// <param name="maMNG">Mã minigame (MaMNG).</param>
        /// <param name="item">Mục cần lưu.</param>
        public static void SaveFillBlankQuestion(string maMNG, FillBlankQuestion item)
        {
            SaveRawData(maMNG, $"{item.QuestionText};{item.Answer}");
        }

        #endregion

        #region Listen & Choose (Image choices)

        /// <summary>
        /// Tải các mục listen-choose, mỗi dòng: Sound;CorrectImage;Wrong1;Wrong2;Wrong3 phân tách bằng '|'.
        /// </summary>
        /// <param name="maMNG">Mã minigame (MaMNG).</param>
        /// <returns>Danh sách <see cref="ListenChooseItem"/>.</returns>
        public static List<ListenChooseItem> GetListenChooseItems(string maMNG)
        {
            var data = new List<ListenChooseItem>();
            string rawData = GetRawData(maMNG);
            if (string.IsNullOrWhiteSpace(rawData))
            {
                return data;
            }

            var lines = rawData.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var parts = line.Split(';');
                if (parts.Length == 5)
                {
                    // Tạo danh sách lựa chọn với phần tử đầu là đúng (theo hợp đồng hiện có)
                    var choices = new List<ImageChoice>
                    {
                        new ImageChoice
                        {
                            ImageResourceName = parts[1],
                            IsCorrect = true
                        },
                        new ImageChoice
                        {
                            ImageResourceName = parts[2],
                            IsCorrect = false
                        },
                        new ImageChoice
                        {
                            ImageResourceName = parts[3],
                            IsCorrect = false
                        },
                        new ImageChoice
                        {
                            ImageResourceName = parts[4],
                            IsCorrect = false
                        }
                    };

                    data.Add(new ListenChooseItem
                    {
                        SoundResourceName = parts[0],
                        Choices = choices
                    });
                }
            }

            return data;
        }

        /// <summary>
        /// Lưu các mục listen-choose. Giữ nguyên định dạng: Sound;CorrectImage;Wrong1;Wrong2;Wrong3 cho mỗi item.
        /// </summary>
        /// <param name="maMNG">Mã minigame (MaMNG).</param>
        /// <param name="items">Các mục cần lưu.</param>
        public static void SaveListenChooseItems(string maMNG, List<ListenChooseItem> items)
        {
            // Giữ logic hiện tại: phần tử đúng là phần tử có IsCorrect == true, các phần tử còn lại là sai.
            SaveRawData(maMNG,
                        string.Join("|",
                                    items.Select(item =>
                                    {
                                        var correct = item.Choices.First(c => c.IsCorrect).ImageResourceName;
                                        var wrongs = string.Join(";", item.Choices.Where(c => !c.IsCorrect).Select(c => c.ImageResourceName));
                                        return $"{item.SoundResourceName};{correct};{wrongs}";
                                    })));
        }

        #endregion
    }
}