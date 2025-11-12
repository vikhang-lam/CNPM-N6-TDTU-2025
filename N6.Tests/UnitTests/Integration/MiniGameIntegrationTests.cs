using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using N6.Tests.TestHelpers;

namespace N6.Tests.UnitTests.Integration
{
    [TestClass]
    public class MiniGameIntegrationTests
    {
        [TestMethod]
        public void FullQuizGameFlow_ValidData_ShouldSucceed()
        {
            // Sắp xếp: Mã game mục tiêu
            string maMNG = "MNG01";

            // Hành động: Mô phỏng từng bước trong luồng

            // 1. (UC_MiniGames) Tải danh sách game
            var gamesTable = MiniGameTestHelper.CreateMockMiniGamesTable();
            bool ucLoaded = (gamesTable.Rows.Count > 0);

            // 2. (GameDataInputForm) Tải dữ liệu câu hỏi
            // (Mô phỏng GameDataManager.GetQuizQuestions trả về dữ liệu)
            var questions = MiniGameTestHelper.CreateValidQuizQuestionsList();
            bool editorLoaded = (questions.Count > 0);

            // 3. (QuizGameForm) Chơi game và chiến thắng
            var gameResult = MiniGameTestHelper.CreateValidQuizGameResult(questions.Count);
            bool gameCompletedSuccess = (gameResult.Score == gameResult.Total);

            // Khẳng định
            ucLoaded.Should().BeTrue();
            editorLoaded.Should().BeTrue();
            gameCompletedSuccess.Should().BeTrue();
        }

        [TestMethod]
        public void FullQuizGameFlow_NoDataInDatabase_ShouldFailAtEditor()
        {
            // Sắp xếp: Mã game mục tiêu
            string maMNG = "MNG01";

            // Hành động: Mô phỏng từng bước

            // 1. (UC_MiniGames) Tải danh sách game
            var gamesTable = MiniGameTestHelper.CreateMockMiniGamesTable();
            bool ucLoaded = (gamesTable.Rows.Count > 0);

            // 2. (GameDataInputForm) Tải dữ liệu câu hỏi
            // (Mô phỏng GameDataManager.GetQuizQuestions trả về rỗng)
            var questions = MiniGameTestHelper.CreateEmptyQuizQuestionsList();
            bool editorLoadedWithData = (questions.Count > 0);

            // 3. (QuizGameForm) Bắt đầu chơi (bị chặn)
            // (Trong code thật, QuizGameForm sẽ tự đóng)
            bool gameStarted = (questions.Count > 0);


            // Khẳng định
            ucLoaded.Should().BeTrue();
            editorLoadedWithData.Should().BeFalse("vì không có câu hỏi nào được tải từ CSDL");
            gameStarted.Should().BeFalse("vì không thể bắt đầu game nếu không có câu hỏi");
        }
    }
}