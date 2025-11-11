using System.Collections.Generic;

namespace N6
{
    /// <summary>
    /// Mẫu câu hỏi trắc nghiệm có chứa nội dung câu hỏi, các lựa chọn và câu trả lời đúng.
    /// </summary>
    public class QuizQuestion
    {
        public string QuestionText { get; set; }
        public List<string> Options { get; set; }
        public string CorrectAnswer { get; set; }
    }

    /// <summary>
    /// Cặp thẻ ghi nhớ thuật ngữ/định nghĩa đơn giản.
    /// </summary>
    public class FlashcardItem
    {
        public string Term { get; set; }
        public string Definition { get; set; }
    }

    /// <summary>
    /// Trò chơi ghép chữ với gợi ý hình ảnh tùy chọn, câu hỏi và câu trả lời.
    /// </summary>
    public class WordScrambleItem
    {
        public string ImageHintResourceName { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
    }

    /// <summary>
    /// Biểu thị lựa chọn hình ảnh được sử dụng bởi các mục ListenChoose.
    /// </summary>
    public class ImageChoice
    {
        public string ImageResourceName { get; set; }
        public bool IsCorrect { get; set; }
    }

    /// <summary>
    /// Vật phẩm cho trò chơi nhỏ Nghe rồi chọn: lựa chọn âm thanh + hình ảnh.
    /// </summary>
    public class ListenChooseItem
    {
        public string SoundResourceName { get; set; }
        public List<ImageChoice> Choices { get; set; }
    }

    /// <summary>
    /// Mục xáo chữ chứa câu đúng
    /// </summary>
    public class SentenceScrambleItem
    {
        public string CorrectSentence { get; set; }
    }

    /// <summary>
    /// Mô hình câu hỏi điền vào chỗ trống.
    /// </summary>
    public class FillBlankQuestion
    {
        public string QuestionText { get; set; }
        public string Answer { get; set; }
    }
}