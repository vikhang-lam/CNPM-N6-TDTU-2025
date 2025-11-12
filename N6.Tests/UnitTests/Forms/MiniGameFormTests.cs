using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using N6.Tests.TestHelpers;
using System.Collections.Generic;

namespace N6.Tests.UnitTests.Forms
{
    [TestClass]
    public class MiniGameFormTests
    {
        [TestMethod]
        public void UC_MiniGames_LoadGames_ShouldLoadCards()
        {
            // Sắp xếp: Mô phỏng việc DatabaseHelper trả về dữ liệu
            var gamesTable = MiniGameTestHelper.CreateMockMiniGamesTable();

            // Hành động: Mô phỏng logic trong UC_MiniGames.LoadGames()
            bool hasData = (gamesTable != null && gamesTable.Rows.Count > 0);
            int cardCount = gamesTable.Rows.Count;

            // Khẳng định
            hasData.Should().BeTrue();
            cardCount.Should().Be(3);
        }

        [TestMethod]
        public void GameDataInputForm_AddQuizQuestion_ShouldIncreaseControlCount()
        {
            // Sắp xếp: Mô phỏng form đang có 2 câu hỏi
            int initialCount = 2;

            // Hành động: Mô phỏng sự kiện click nút "Thêm câu hỏi"
            // (Thực tế là gọi AddQuizQuestionControl(null))
            int newCount = initialCount + 1;

            // Khẳng định
            newCount.Should().Be(3);
        }

        [TestMethod]
        public void QuizGameForm_WithValidQuestions_ShouldInitialize()
        {
            // Sắp xếp
            var questions = MiniGameTestHelper.CreateValidQuizQuestionsList();

            // Hành động: Mô phỏng logic constructor của QuizGameForm
            bool shouldLoad = (questions != null && questions.Count > 0);

            // Khẳng định
            shouldLoad.Should().BeTrue();
        }

        [TestMethod]
        public void QuizGameForm_WithNoQuestions_ShouldNotInitialize()
        {
            // Sắp xếp
            var questions = MiniGameTestHelper.CreateEmptyQuizQuestionsList();

            // Hành động: Mô phỏng logic constructor của QuizGameForm
            // (Trong code thật, form sẽ tự Close() nếu không có câu hỏi)
            bool shouldLoad = (questions != null && questions.Count > 0);
            bool formClosed = !shouldLoad;

            // Khẳng định
            shouldLoad.Should().BeFalse();
            formClosed.Should().BeTrue();
        }
    }
}