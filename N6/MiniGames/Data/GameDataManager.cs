using System;
using System.Collections.Generic;
using System.Linq;

namespace N6
{
    public static class GameDataManager
    {
        private static string GetRawData(string maMNG) { try { return DatabaseHelper.GetGameData(maMNG); } catch { return ""; } }
        private static void SaveRawData(string maMNG, string rawData) { try { DatabaseHelper.SaveGameData(maMNG, rawData); } catch { } }

        public static List<QuizQuestion> GetQuizQuestions(string maMNG)
        {
            var data = new List<QuizQuestion>();
            string rawData = GetRawData(maMNG);
            if (string.IsNullOrWhiteSpace(rawData)) return data;
            var lines = rawData.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var parts = line.Split(';');
                if (parts.Length == 6) { data.Add(new QuizQuestion { QuestionText = parts[0], Options = new List<string> { parts[1], parts[2], parts[3], parts[4] }, CorrectAnswer = parts[5].Trim().ToUpper() }); }
            }
            return data;
        }
        public static void SaveQuizQuestions(string maMNG, List<QuizQuestion> questions) { SaveRawData(maMNG, string.Join("|", questions.Select(q => $"{q.QuestionText.Trim()};{string.Join(";", q.Options.Select(o => o.Trim()))};{q.CorrectAnswer.Trim().ToUpper()}"))); }
        public static List<string> GetListFromString(string maMNG)
        {
            string rawData = GetRawData(maMNG);
            if (string.IsNullOrWhiteSpace(rawData)) return new List<string>();
            return rawData.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
        }
        public static void SaveListToString(string maMNG, List<string> items) { SaveRawData(maMNG, string.Join(";", items.Select(s => s.Trim()))); }
        public static List<FlashcardItem> GetFlashcardItems(string maMNG)
        {
            var data = new List<FlashcardItem>();
            string rawData = GetRawData(maMNG);
            if (string.IsNullOrWhiteSpace(rawData)) return data;
            var lines = rawData.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var parts = line.Split(';');
                if (parts.Length == 2) { data.Add(new FlashcardItem { Term = parts[0].Trim(), Definition = parts[1].Trim() }); }
            }
            return data;
        }
        public static void SaveFlashcardItems(string maMNG, List<FlashcardItem> items) { SaveRawData(maMNG, string.Join("|", items.Select(item => $"{item.Term.Trim()};{item.Definition.Trim()}"))); }
        public static List<WordScrambleItem> GetWordScrambleItems(string maMNG)
        {
            var data = new List<WordScrambleItem>();
            string rawData = GetRawData(maMNG);
            if (string.IsNullOrWhiteSpace(rawData)) return data;
            var lines = rawData.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var parts = line.Split(';');
                if (parts.Length == 3) { data.Add(new WordScrambleItem { ImageHintResourceName = parts[0], Question = parts[1], Answer = parts[2].ToUpper() }); }
            }
            return data;
        }
        public static void SaveWordScrambleItems(string maMNG, List<WordScrambleItem> items) { SaveRawData(maMNG, string.Join("|", items.Select(item => $"{item.ImageHintResourceName};{item.Question};{item.Answer}"))); }
        public static List<SentenceScrambleItem> GetSentenceScrambleItems(string maMNG)
        {
            var data = new List<SentenceScrambleItem>();
            string rawData = GetRawData(maMNG);
            if (string.IsNullOrWhiteSpace(rawData)) return data;

            // Tách theo ký tự | để lấy từng câu riêng biệt
            var sentences = rawData.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var sentence in sentences)
            {
                var trimmedSentence = sentence.Trim();
                if (!string.IsNullOrEmpty(trimmedSentence))
                {
                    data.Add(new SentenceScrambleItem { CorrectSentence = trimmedSentence });
                }
            }
            return data;
        }

        public static void SaveSentenceScrambleItems(string maMNG, List<SentenceScrambleItem> items)
        {
            var sentences = items.Where(item => !string.IsNullOrWhiteSpace(item.CorrectSentence))
                                 .Select(item => item.CorrectSentence.Trim());
            SaveRawData(maMNG, string.Join("|", sentences));
        }

        public static SentenceScrambleItem GetSentenceScrambleItem(string maMNG) { return new SentenceScrambleItem { CorrectSentence = GetRawData(maMNG) ?? "" }; }
        public static void SaveSentenceScrambleItem(string maMNG, SentenceScrambleItem item) { SaveRawData(maMNG, item.CorrectSentence); }
        public static List<FillBlankQuestion> GetFillBlankQuestions(string maMNG)
        {
            var data = new List<FillBlankQuestion>();
            string rawData = GetRawData(maMNG);
            if (string.IsNullOrWhiteSpace(rawData)) return data;

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

        public static void SaveFillBlankQuestions(string maMNG, List<FillBlankQuestion> questions)
        {
            var lines = questions.Where(q => !string.IsNullOrWhiteSpace(q.QuestionText))
                                 .Select(q => $"{q.QuestionText.Trim()};{q.Answer.Trim()}");
            SaveRawData(maMNG, string.Join("|", lines));
        }
        public static FillBlankQuestion GetFillBlankQuestion(string maMNG)
        {
            string rawData = GetRawData(maMNG);
            if (string.IsNullOrWhiteSpace(rawData)) return new FillBlankQuestion { QuestionText = "", Answer = "" };
            var parts = rawData.Split(';');
            return new FillBlankQuestion { QuestionText = parts.Length > 0 ? parts[0] : "", Answer = parts.Length > 1 ? parts[1] : "" };
        }
        public static void SaveFillBlankQuestion(string maMNG, FillBlankQuestion item) { SaveRawData(maMNG, $"{item.QuestionText};{item.Answer}"); }
        public static List<ListenChooseItem> GetListenChooseItems(string maMNG)
        {
            var data = new List<ListenChooseItem>();
            string rawData = GetRawData(maMNG);
            if (string.IsNullOrWhiteSpace(rawData)) return data;
            var lines = rawData.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var parts = line.Split(';');
                if (parts.Length == 5)
                {
                    var choices = new List<ImageChoice> { new ImageChoice { ImageResourceName = parts[1], IsCorrect = true }, new ImageChoice { ImageResourceName = parts[2], IsCorrect = false }, new ImageChoice { ImageResourceName = parts[3], IsCorrect = false }, new ImageChoice { ImageResourceName = parts[4], IsCorrect = false } };
                    data.Add(new ListenChooseItem { SoundResourceName = parts[0], Choices = choices });
                }
            }
            return data;
        }
        public static void SaveListenChooseItems(string maMNG, List<ListenChooseItem> items) { SaveRawData(maMNG, string.Join("|", items.Select(item => $"{item.SoundResourceName};{item.Choices.First(c => c.IsCorrect).ImageResourceName};{string.Join(";", item.Choices.Where(c => !c.IsCorrect).Select(c => c.ImageResourceName))}"))); }
    }
}
