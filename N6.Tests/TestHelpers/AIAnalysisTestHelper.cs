using System.Data;
using System.Collections.Generic;

namespace N6.Tests.TestHelpers
{
    /// <summary>
    /// Cung cấp dữ liệu DataTable giả cho việc kiểm thử UC_PhanTichAI và AIAnalyzer.
    /// </summary>
    public static class AIAnalysisTestHelper
    {
        /// <summary>
        /// Mô phỏng kết quả từ sp_GetScoresForAnalysis.
        /// Chứa đủ các trường hợp: Khen thưởng, Điểm thấp, và Thất thường.
        /// </summary>
        public static DataTable CreateMockScoresData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaHS", typeof(string));
            dt.Columns.Add("HoTen", typeof(string));
            dt.Columns.Add("TenLop", typeof(string));
            dt.Columns.Add("Diem", typeof(double));

            // Case 1: Khen thưởng (ĐTB >= 8.5)
            dt.Rows.Add("HS001", "Nguyễn Văn A", "Lớp 5A", 9.0);
            dt.Rows.Add("HS001", "Nguyễn Văn A", "Lớp 5A", 8.0); // ĐTB = 8.5

            // Case 2: Cần quan tâm (ĐTB < 5.0)
            dt.Rows.Add("HS002", "Trần Thị B", "Lớp 5A", 4.0);
            dt.Rows.Add("HS002", "Trần Thị B", "Lớp 5A", 5.0); // ĐTB = 4.5

            // Case 3: Thất thường (Độ lệch chuẩn cao > 2.0)
            dt.Rows.Add("HS003", "Lê Văn C", "Lớp 5B", 10.0);
            dt.Rows.Add("HS003", "Lê Văn C", "Lớp 5B", 2.0);
            dt.Rows.Add("HS003", "Lê Văn C", "Lớp 5B", 9.0); // ĐTB = 7.0, StdDev > 2.0

            // Case 4: Ổn định (Không thuộc case nào)
            dt.Rows.Add("HS004", "Phạm Thị D", "Lớp 5B", 7.0);
            dt.Rows.Add("HS004", "Phạm Thị D", "Lớp 5B", 8.0);
            dt.Rows.Add("HS004", "Phạm Thị D", "Lớp 5B", 7.5); // ĐTB = 7.5, StdDev < 2.0

            return dt;
        }

        /// <summary>
        /// Mô phỏng DataTable rỗng (không có dữ liệu).
        /// </summary>
        public static DataTable CreateEmptyScoresData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaHS", typeof(string));
            dt.Columns.Add("HoTen", typeof(string));
            dt.Columns.Add("TenLop", typeof(string));
            dt.Columns.Add("Diem", typeof(double));
            return dt;
        }

        /// <summary>
        /// Mô phỏng kết quả từ sp_GetStudentDataForPrediction (cho model ONNX).
        /// </summary>
        public static DataTable CreateMockPredictionData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("HoTen", typeof(string));
            dt.Columns.Add("TenLop", typeof(string));
            dt.Columns.Add("G1", typeof(float));
            dt.Columns.Add("G2", typeof(float));
            dt.Columns.Add("NumLowScores", typeof(int));
            dt.Columns.Add("Absences", typeof(int));

            // Case 1: Nguy cơ cao (Điểm G1, G2 thấp)
            dt.Rows.Add("Trần Thị B", "Lớp 5A", 4.0f, 5.0f, 3, 5);

            // Case 2: An toàn (Điểm G1, G2 cao)
            dt.Rows.Add("Nguyễn Văn A", "Lớp 5A", 9.0f, 8.0f, 0, 1);

            return dt;
        }

        /// <summary>
        /// Mô phỏng kết quả từ sp_GetClassesByTeacher.
        /// </summary>
        public static DataTable CreateMockClassesByTeacherData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaLop", typeof(string));
            dt.Columns.Add("TenLop", typeof(string));
            dt.Rows.Add("L5A", "Lớp 5A");
            dt.Rows.Add("L4B", "Lớp 4B");
            return dt;
        }

        /// <summary>
        /// Mô phỏng kết quả từ sp_GetAllSubjects.
        /// </summary>
        public static DataTable CreateMockAllSubjectsData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaMon", typeof(string));
            dt.Columns.Add("TenMon", typeof(string));
            dt.Rows.Add("TOAN", "Toán");
            dt.Rows.Add("TV", "Tiếng Việt");
            return dt;
        }
    }
}