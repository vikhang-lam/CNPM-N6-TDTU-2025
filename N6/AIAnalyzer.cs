using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using Microsoft.ML.OnnxRuntime; // Cần cho dự đoán
using Microsoft.ML.OnnxRuntime.Tensors; // Cần cho dự đoán
using System.Diagnostics; // Cần cho Debug.WriteLine

#region Các lớp chứa kết quả phân tích

/// <summary>
/// Đại diện cho kết quả phân tích chung về một học sinh.
/// </summary>
public class HocSinhAnalysisResult
{
    public string HoTen { get; set; }
    public string TenLop { get; set; }
    public string LyDo { get; set; } // Lý do được đưa vào danh sách
}

/// <summary>
/// Đại diện cho kết quả phân tích chuyên cần.
/// </summary>
public class ChuyenCanResult
{
    public string HoTen { get; set; }
    public string TenLop { get; set; }
    public int SoBuoiVang { get; set; }
    public double TyLeChuyenCan { get; set; }
}

/// <summary>
/// Đại diện cho kết quả so sánh tổng hợp giữa các lớp.
/// </summary>
public class SoSanhLopResult
{
    public string TenLop { get; set; }
    public double DiemTB { get; set; }
    public int SoHocSinhGioi { get; set; }
    public int SiSo { get; set; }
}

/// <summary>
/// Đại diện cho kết quả dự đoán nguy cơ của học sinh.
/// </summary>
public class HocSinhDuDoanResult
{
    public string HoTen { get; set; }
    public string TenLop { get; set; }
    public float DiemDuDoan { get; set; }
    public string MonHocCanHoTro { get; set; }
}
#endregion

/// <summary>
/// Lớp tĩnh chứa các thuật toán phân tích dữ liệu học tập.
/// Không chứa bất kỳ tham chiếu UI nào (tuân thủ Separation of Concerns).
/// </summary>
public static class AIAnalyzer
{
    #region Phân tích Điểm số

    /// <summary>
    /// Tìm các học sinh có điểm trung bình thấp hơn một ngưỡng nhất định.
    /// </summary>
    /// <param name="scores">DataTable chứa điểm (phải có cột MaHS, HoTen, TenLop, Diem).</param>
    /// <param name="nguongDiem">Ngưỡng điểm (ví dụ: 5.0).</param>
    /// <returns>Danh sách học sinh cần quan tâm.</returns>
    public static List<HocSinhAnalysisResult> TimHocSinhDiemThap(DataTable scores, double nguongDiem)
    {
        if (scores == null || scores.Rows.Count == 0)
        {
            return new List<HocSinhAnalysisResult>();
        }

        return scores.AsEnumerable()
            .GroupBy(row => new { MaHS = row.Field<string>("MaHS"), HoTen = row.Field<string>("HoTen"), TenLop = row.Field<string>("TenLop") })
            .Select(g => new { g.Key.HoTen, g.Key.TenLop, DiemTB = g.Average(r => r.Field<double?>("Diem") ?? 0) })
            .Where(s => s.DiemTB < nguongDiem && s.DiemTB > 0) // Chỉ xét HS có điểm
            .OrderBy(s => s.DiemTB)
            .Select(s => new HocSinhAnalysisResult { HoTen = s.HoTen, TenLop = s.TenLop, LyDo = $"Điểm TB thấp ({s.DiemTB:F1})" })
            .ToList();
    }

    /// <summary>
    /// Tìm các học sinh có điểm số biến động cao (độ lệch chuẩn lớn).
    /// </summary>
    /// <param name="scores">DataTable chứa điểm.</param>
    /// <param name="nguongBienDong">Ngưỡng độ lệch chuẩn (ví dụ: 2.0).</param>
    /// <returns>Danh sách học sinh có phong độ thất thường.</returns>
    public static List<HocSinhAnalysisResult> TimHocSinhDiemThatThuong(DataTable scores, double nguongBienDong)
    {
        if (scores == null || scores.Rows.Count == 0)
        {
            return new List<HocSinhAnalysisResult>();
        }

        return scores.AsEnumerable()
            .GroupBy(row => new { MaHS = row.Field<string>("MaHS"), HoTen = row.Field<string>("HoTen"), TenLop = row.Field<string>("TenLop") })
            .Select(g => {
                var diemList = g.Select(r => r.Field<double?>("Diem") ?? 0).ToList();
                if (diemList.Count < 3) return null; // Cần ít nhất 3 cột điểm để tính
                double avg = diemList.Average();
                double stdDev = Math.Sqrt(diemList.Sum(d => Math.Pow(d - avg, 2)) / diemList.Count);
                return new { g.Key.HoTen, g.Key.TenLop, DoBienDong = stdDev };
            })
            .Where(s => s != null && s.DoBienDong > nguongBienDong)
            .OrderByDescending(s => s.DoBienDong)
            .Select(s => new HocSinhAnalysisResult { HoTen = s.HoTen, TenLop = s.TenLop, LyDo = $"Biến động lớn (±{s.DoBienDong:F1})" })
            .ToList();
    }

    /// <summary>
    /// Tìm các học sinh có thành tích xuất sắc (điểm trung bình cao).
    /// </summary>
    /// <param name="scores">DataTable chứa điểm.</param>
    /// <param name="diemCaoThreshold">Ngưỡng điểm cao (ví dụ: 8.5).</param>
    /// <returns>Danh sách học sinh được khen thưởng.</returns>
    public static List<HocSinhAnalysisResult> TimHocSinhKhenThuong(DataTable scores, double diemCaoThreshold)
    {
        if (scores == null || scores.Rows.Count == 0)
        {
            return new List<HocSinhAnalysisResult>();
        }

        var hocSinhKhenThuong = new List<HocSinhAnalysisResult>();
        var hocSinhGroups = scores.AsEnumerable()
            .GroupBy(row => new { MaHS = row.Field<string>("MaHS"), HoTen = row.Field<string>("HoTen"), TenLop = row.Field<string>("TenLop") });

        foreach (var group in hocSinhGroups)
        {
            var diemList = group.Select(r => r.Field<double?>("Diem") ?? 0).ToList();
            var diemTB = diemList.Any() ? diemList.Average() : 0;

            if (diemTB >= diemCaoThreshold)
            {
                hocSinhKhenThuong.Add(new HocSinhAnalysisResult { HoTen = group.Key.HoTen, TenLop = group.Key.TenLop, LyDo = $"Thành tích xuất sắc (ĐTB: {diemTB:F1})" });
            }
        }
        return hocSinhKhenThuong.OrderByDescending(hs => hs.LyDo).ToList();
    }
    #endregion

    #region Phân tích Chuyên cần và So sánh

    /// <summary>
    /// Phân tích dữ liệu chuyên cần (yêu cầu DataTable từ CSDL).
    /// </summary>
    /// <param name="attendanceData">DataTable chứa dữ liệu chuyên cần.</param>
    /// <returns>Danh sách học sinh có số buổi vắng > 3.</returns>
    public static List<ChuyenCanResult> PhanTichChuyenCan(DataTable attendanceData)
    {
        if (attendanceData == null || attendanceData.Rows.Count == 0)
        {
            return new List<ChuyenCanResult>();
        }

        return attendanceData.AsEnumerable()
            .GroupBy(row => new { MaHS = row.Field<string>("MaHS"), HoTen = row.Field<string>("HoTen"), TenLop = row.Field<string>("TenLop") })
            .Select(g => {
                int tongSoBuoi = g.Count();
                int soBuoiVang = g.Count(r => r.Field<string>("TrangThai") == "Vắng");
                return new ChuyenCanResult
                {
                    HoTen = g.Key.HoTen,
                    TenLop = g.Key.TenLop,
                    SoBuoiVang = soBuoiVang,
                    TyLeChuyenCan = (tongSoBuoi > 0) ? (100.0 * (tongSoBuoi - soBuoiVang) / tongSoBuoi) : 100.0
                };
            })
            .Where(r => r.SoBuoiVang > 3) // Lọc những HS vắng nhiều
            .OrderByDescending(r => r.SoBuoiVang)
            .ToList();
    }

    /// <summary>
    /// So sánh học tập giữa các lớp (yêu cầu DataTable từ CSDL).
    /// </summary>
    /// <param name="scoreData">DataTable chứa điểm của nhiều lớp.</param>
    /// <returns>Danh sách tổng hợp của mỗi lớp.</returns>
    public static List<SoSanhLopResult> SoSanhHocTap(DataTable scoreData)
    {
        if (scoreData == null || scoreData.Rows.Count == 0)
        {
            return new List<SoSanhLopResult>();
        }

        return scoreData.AsEnumerable()
            .GroupBy(row => row.Field<string>("TenLop"))
            .Select(g => {
                var diemList = g.Select(r => r.Field<double?>("Diem") ?? 0).ToList();
                return new SoSanhLopResult
                {
                    TenLop = g.Key,
                    SiSo = g.Select(r => r.Field<string>("MaHS")).Distinct().Count(),
                    DiemTB = diemList.Any() ? diemList.Average() : 0,
                    SoHocSinhGioi = diemList.Count(d => d >= 8.0) // Đếm HS đạt điểm 8 trở lên
                };
            })
            .OrderByDescending(r => r.DiemTB)
            .ToList();
    }
    #endregion

    #region Phân tích Dự đoán (AI/ML)

    // Tạo session cache để không phải tải lại model mỗi lần
    private static InferenceSession _onnxSession;
    private static readonly object _sessionLock = new object();
    private const string MODEL_FILE_NAME = "student_g3_predictor.onnx";

    /// <summary>
    /// Chạy mô hình ONNX để dự đoán học sinh có nguy cơ điểm cuối kỳ thấp.
    /// </summary>
    /// <param name="predictionData">DataTable từ sp_GetStudentDataForPrediction.</param>
    /// <param name="tenMon">Tên môn học để đưa vào khuyến nghị.</param>
    /// <param name="nguongNguyCo">Ngưỡng điểm dự đoán (ví dụ: 5.0).</param>
    /// <returns>Danh sách học sinh cần hỗ trợ.</returns>
    public static List<HocSinhDuDoanResult> DuDoanHocSinhNguyCo(DataTable predictionData, string tenMon, double nguongNguyCo = 5.0)
    {
        var results = new List<HocSinhDuDoanResult>();
        if (predictionData == null || predictionData.Rows.Count == 0)
            return results;

        try
        {
            // 1. Khởi tạo session (an toàn đa luồng)
            lock (_sessionLock)
            {
                if (_onnxSession == null)
                {
                    string modelPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, MODEL_FILE_NAME);
                    if (!System.IO.File.Exists(modelPath))
                    {
                        Debug.WriteLine($"[AIAnalyzer Error] Không tìm thấy tệp model: {modelPath}");
                        return results; // Không tìm thấy model
                    }
                    _onnxSession = new InferenceSession(modelPath);
                }
            }

            // 2. Lấy tên input đầu tiên của model
            // (Thường là 'float_input' nếu xuất từ scikit-learn)
            var inputName = _onnxSession.InputMetadata.Keys.First();

            // 3. Lặp qua từng học sinh để dự đoán
            foreach (DataRow row in predictionData.Rows)
            {
                // 4. Lấy features và ép kiểu về float
                // Features: ["G1", "G2", "num_low_scores", "absences"]
                var features = new float[4];
                features[0] = Convert.ToSingle(row["G1"]);
                features[1] = Convert.ToSingle(row["G2"]);
                features[2] = Convert.ToSingle(row["NumLowScores"]);
                features[3] = Convert.ToSingle(row["Absences"]);

                // 5. Tạo Tensor (Batch size = 1, Số lượng features = 4)
                var dimensions = new int[] { 1, 4 };
                var inputTensor = new DenseTensor<float>(features, dimensions);

                // 6. Chuẩn bị input cho model
                var inputs = new List<NamedOnnxValue>
                {
                    NamedOnnxValue.CreateFromTensor(inputName, inputTensor)
                };

                // 7. Chạy dự đoán
                using (var outputs = _onnxSession.Run(inputs))
                {
                    // 8. Lấy kết quả (thường là 1x1 tensor)
                    var predictedScoreTensor = outputs.First().AsTensor<float>();
                    float predictedScore = predictedScoreTensor.GetValue(0);

                    // 9. Thêm vào danh sách nếu điểm thấp
                    if (predictedScore < nguongNguyCo)
                    {
                        results.Add(new HocSinhDuDoanResult
                        {
                            HoTen = row.Field<string>("HoTen"),
                            TenLop = row.Field<string>("TenLop"),
                            DiemDuDoan = predictedScore,
                            MonHocCanHoTro = tenMon
                        });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[ONNX Error] {ex.Message}");
            // Trả về ds rỗng thay vì làm crash ứng dụng
        }

        return results.OrderBy(r => r.DiemDuDoan).ToList();
    }

    #endregion
}