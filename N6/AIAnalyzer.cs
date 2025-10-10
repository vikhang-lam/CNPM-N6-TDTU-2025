using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;

// Lớp lưu kết quả phân tích học sinh
public class HocSinhAnalysisResult
{
    public string HoTen { get; set; }
    public string TenLop { get; set; }
    public string LyDo { get; set; }
}

public static class AIAnalyzer
{
    // Mô hình 1: Tìm học sinh có điểm trung bình thấp
    public static List<HocSinhAnalysisResult> TimHocSinhDiemThap(DataTable scores, double nguongDiem)
    {
        return scores.AsEnumerable()
            .GroupBy(row => new { MaHS = row.Field<string>("MaHS"), HoTen = row.Field<string>("HoTen"), TenLop = row.Field<string>("TenLop") })
            .Select(g => new { g.Key.HoTen, g.Key.TenLop, DiemTB = g.Average(r => r.Field<double?>("Diem") ?? 0) })
            .Where(s => s.DiemTB < nguongDiem && s.DiemTB > 0)
            .OrderBy(s => s.DiemTB)
            .Select(s => new HocSinhAnalysisResult { HoTen = s.HoTen, TenLop = s.TenLop, LyDo = $"Điểm TB thấp ({s.DiemTB:F1})" })
            .ToList();
    }

    // Mô hình 2: Tìm học sinh có điểm thất thường
    public static List<HocSinhAnalysisResult> TimHocSinhDiemThatThuong(DataTable scores, double nguongBienDong)
    {
        return scores.AsEnumerable()
            .GroupBy(row => new { MaHS = row.Field<string>("MaHS"), HoTen = row.Field<string>("HoTen"), TenLop = row.Field<string>("TenLop") })
            .Select(g => {
                var diemList = g.Select(r => r.Field<double?>("Diem") ?? 0).ToList();
                if (diemList.Count < 3) return null;
                double avg = diemList.Average();
                double stdDev = Math.Sqrt(diemList.Sum(d => Math.Pow(d - avg, 2)) / diemList.Count);
                return new { g.Key.HoTen, g.Key.TenLop, DoBienDong = stdDev };
            })
            .Where(s => s != null && s.DoBienDong > nguongBienDong)
            .OrderByDescending(s => s.DoBienDong)
            .Select(s => new HocSinhAnalysisResult { HoTen = s.HoTen, TenLop = s.TenLop, LyDo = $"Biến động lớn (±{s.DoBienDong:F1})" })
            .ToList();
    }

    // Mô hình 3: Tìm học sinh đáng khen thưởng (tiến bộ hoặc điểm trung bình rất cao)
    public static List<HocSinhAnalysisResult> TimHocSinhKhenThuong(DataTable scores, double diemCaoThreshold)
    {
        var hocSinhKhenThuong = new List<HocSinhAnalysisResult>();

        var hocSinhGroups = scores.AsEnumerable()
            .GroupBy(row => new { MaHS = row.Field<string>("MaHS"), HoTen = row.Field<string>("HoTen"), TenLop = row.Field<string>("TenLop") });

        foreach (var group in hocSinhGroups)
        {
            var diemList = group.Select(r => r.Field<double?>("Diem") ?? 0).ToList();
            var diemTB = diemList.Average();

            if (diemTB >= diemCaoThreshold)
            {
                hocSinhKhenThuong.Add(new HocSinhAnalysisResult { HoTen = group.Key.HoTen, TenLop = group.Key.TenLop, LyDo = $"Thành tích xuất sắc (ĐTB: {diemTB:F1})" });
            }
            else // Kiểm tra sự tiến bộ
            {
                var diemKi1 = group.Where(r => r.Field<string>("Loai").Contains("Ki1")).Select(r => r.Field<double?>("Diem") ?? 0).ToList();
                var diemKi2 = group.Where(r => r.Field<string>("Loai").Contains("Ki2")).Select(r => r.Field<double?>("Diem") ?? 0).ToList();

                if (diemKi1.Any() && diemKi2.Any())
                {
                    var avgKi1 = diemKi1.Average();
                    var avgKi2 = diemKi2.Average();
                    if (avgKi2 > avgKi1 + 1.5) // Nếu điểm kì 2 tăng hơn 1.5 so với kì 1 -> tiến bộ
                    {
                        hocSinhKhenThuong.Add(new HocSinhAnalysisResult { HoTen = group.Key.HoTen, TenLop = group.Key.TenLop, LyDo = $"Tiến bộ vượt bậc (từ {avgKi1:F1} lên {avgKi2:F1})" });
                    }
                }
            }
        }
        return hocSinhKhenThuong;
    }
}