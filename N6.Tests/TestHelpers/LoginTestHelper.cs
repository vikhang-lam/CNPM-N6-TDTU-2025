using N6;

namespace N6.Tests.TestHelpers
{
    public static class LoginTestHelper
    {
        public static (string username, string password) CreateValidAdminCredentials()
        {
            return ("admin", "admin123");
        }

        public static (string username, string password) CreateValidTeacherCredentials()
        {
            return ("teacher123", "password123");
        }

        public static (string username, string password) CreateInvalidCredentials()
        {
            return ("invaliduser", "wrongpassword");
        }

        public static (string username, string password) CreateEmptyCredentials()
        {
            return ("", "");
        }

        public static TeacherProfile CreateSampleTeacherProfile()
        {
            return new TeacherProfile
            {
                Ten = "Nguyễn Văn A",
                TenMon = "Toán",
                Email = "teacher@example.com",
                SDT = "0123456789",
                AnhDaiDien = null
            };
        }
    }
}