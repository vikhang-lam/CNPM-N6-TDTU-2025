using System;
using System.Data.SqlClient;

namespace N6
{
    public static class DatabaseHelper
    {
        private static readonly string connectionString =
            "Data Source=LAPTOP-3IRTDDBK;Initial Catalog=quanlilophoc_giangday;Integrated Security=True;";

        // Kiểm tra đăng nhập Admin
        public static bool CheckAdminLogin(string username, string password)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM Admin WHERE Username=@user AND Password=@pass";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@user", username);
                    cmd.Parameters.AddWithValue("@pass", password);

                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        // Kiểm tra đăng nhập Giáo viên
        public static bool CheckTeacherLogin(string username, string password)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM GiaoVien WHERE Username=@user AND Password=@pass";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@user", username);
                    cmd.Parameters.AddWithValue("@pass", password);

                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }
    }
}
