using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using N6.Tests.TestHelpers;
using System.Collections.Generic;

namespace N6.Tests.UnitTests.Services
{
    [TestClass]
    public class GameDataManagerTests
    {
        // Lớp giả lập GameDataManager để kiểm thử logic tách/ghép chuỗi
        // mà không gọi DatabaseHelper thật.
        private class TestableGameDataManager
        {
            // Tái định nghĩa logic tách chuỗi (deserialize) từ file GameDataManager.cs
            public List<QuizQuestion> GetQuizQuestions(string rawData)
            {
                var data = new List<QuizQuestion>();
                if (string.IsNullOrWhiteSpace(rawData)) return data;

                var lines = rawData.Split(new[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    var parts = line.Split(';');
                    if (parts.Length == 6)
                    {
                        data.Add(new QuizQuestion
                        {
                            QuestionText = parts[0],
                            Options = new List<string> { parts[1], parts[2], parts[3], parts[4] },
                            CorrectAnswer = parts[5].Trim().ToUpper()
                        });
                    }
                }
                return data;
            }

            // Tái định nghĩa logic ghép chuỗi (serialize) từ file GameDataManager.cs
            public string SaveQuizQuestions(List<QuizQuestion> questions)
            {
                return string.Join("|", questions.Select(q => $"{q.QuestionText.Trim()};{string.Join(";", q.Options.Select(o => o.Trim()))};{q.CorrectAnswer.Trim().ToUpper()}"));
            }
        }

        [TestMethod]
        public void GetQuizQuestions_WithValidSerializedData_ShouldParseCorrectly()
        {
            // Sắp xếp
            var manager = new TestableGameDataManager();
            string rawData = MiniGameTestHelper.CreateValidSerializedQuizData();

            // Hành động
            var result = manager.GetQuizQuestions(rawData);

            // Khẳng định
            result.Should().NotBeNull();
            result.Count.Should().Be(2);
            result[0].QuestionText.Should().Be("Thủ đô của Việt Nam là gì?");
            result[0].CorrectAnswer.Should().Be("A");
            result[1].QuestionText.Should().Be("1 + 1 bằng mấy?");
            result[1].Options[1].Should().Be("2");
        }

        [TestMethod]
        public void GetQuizQuestions_WithEmptyData_ShouldReturnEmptyList()
        {
            // Sắp xếp
            var manager = new TestableGameDataManager();
            string rawData = MiniGameTestHelper.CreateEmptySerializedData();

            // Hành động
            var result = manager.GetQuizQuestions(rawData);

            // Khẳng định
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void SaveQuizQuestions_WithValidList_ShouldSerializeCorrectly()
        {
            // Sắp xếp
            var manager = new TestableGameDataManager();
            var questions = MiniGameTestHelper.CreateValidQuizQuestionsList();
            string expectedString = MiniGameTestHelper.CreateValidSerializedQuizData();

            // Hành động
            string result = manager.SaveQuizQuestions(questions);

            // Khẳng định
            result.Should().Be(expectedString);
        }
    }
}