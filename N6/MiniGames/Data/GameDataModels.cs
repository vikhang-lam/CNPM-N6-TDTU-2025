using System.Collections.Generic;

namespace N6
{
    #region Cấu trúc dữ liệu cho game
    public class QuizQuestion
    {
        public string QuestionText {get; set; }
        public List<string> Options { get; set; }
        public string CorrectAnswer { get; set; }
    }
    public class FlashcardItem
    {
        public string Term { get; set; }
        public string Definition { get; set; }
    }

    public class WordScrambleItem
    {
        public string ImageHintResourceName { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
    }

    public class ImageChoice
    {
        public string ImageResourceName { get; set; }
        public bool IsCorrect { get; set; }
    }

    public class ListenChooseItem
    {
        public string SoundResourceName { get; set; }
        public List<ImageChoice> Choices { get; set; }
    }

    public class SentenceScrambleItem
    {
        public string CorrectSentence { get; set; }
    }

    public class FillBlankQuestion
    {
        public string QuestionText { get; set; }
        public string Answer { get; set; }
    }

    #endregion
}
