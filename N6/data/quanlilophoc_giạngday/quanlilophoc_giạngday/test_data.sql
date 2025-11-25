USE quanlilophoc_giangday;
GO

-- ==============================================================
-- 1. XÓA SẠCH DỮ LIỆU CŨ (GIỮ CẤU TRÚC)
-- ==============================================================
PRINT 'Dang don dep du lieu...';
EXEC sp_MSforeachtable "ALTER TABLE ? NOCHECK CONSTRAINT all"
DELETE FROM ChiTietDiemLuuTru;
DELETE FROM HoSoLuuTru;
DELETE FROM KetQuaHocTap;
DELETE FROM DiemDanh;
DELETE FROM ThoiKhoaBieu;
DELETE FROM PhanCongGiangDay;
DELETE FROM GiaoVien_MonHoc;
DELETE FROM TaiLieu;
DELETE FROM QuyLop;
DELETE FROM HocSinh;
DELETE FROM LopHoc;
DELETE FROM GiaoVien;
DELETE FROM MonHoc;
DELETE FROM Admin;
DELETE FROM NamHoc;
DELETE FROM ThoiHanDiem;
EXEC sp_MSforeachtable "ALTER TABLE ? WITH CHECK CHECK CONSTRAINT all"
GO

-- ==============================================================
-- 2. KHỞI TẠO DỮ LIỆU HỆ THỐNG CẦN THIẾT
-- ==============================================================
PRINT 'Khoi tao du lieu he thong...';

-- 1. Năm học 2024-2025 (Hiện tại)
INSERT INTO NamHoc(MaNamHoc, TenNamHoc, IsCurrent) 
VALUES ('2024-2025', N'Năm học 2024 - 2025', 1);

-- 2. Admin
INSERT INTO Admin (MaAdmin, Username, Password, Email) 
VALUES ('AD001', 'admin', '123456', 'admin@test.com');

-- 3. Môn học (Chỉ 2 môn theo yêu cầu)
INSERT INTO MonHoc (MaMon, TenMon) VALUES 
('TOAN', N'Toán'),
('ANH', N'Tiếng Anh');

-- 4. Giáo viên (2 GV đại diện)
INSERT INTO GiaoVien (MaGV, Ten, Username, Password, Email, SDT, MaAdmin, TrangThai) VALUES
('GV001', N'Cô Minh Anh', 'minhanh', '123456', 'gv1@test.com', '0901', 'AD001', N'Đã xác nhận'),
('GV002', N'Thầy Quốc Trung', 'quoctrung', '123456', 'gv2@test.com', '0902', 'AD001', N'Đã xác nhận');

-- Phân công chuyên môn (Để họ có quyền nhập điểm)
INSERT INTO GiaoVien_MonHoc (MaGV, MaMon) VALUES
('GV001', 'TOAN'), ('GV001', 'ANH'),
('GV002', 'TOAN'), ('GV002', 'ANH');

-- ==============================================================
-- 3. TẠO LỚP VÀ PHÂN CÔNG
-- ==============================================================
PRINT 'Tao lop va phan cong...';

-- Tạo 3 lớp: 
-- 1A1: Cần xét lên lớp
-- 5A1: Cần xét tốt nghiệp
-- 2A1: Lớp đích (Rỗng) để chứa học sinh 1A1 lên lớp
INSERT INTO LopHoc (MaLop, TenLop, Khoi, NamHoc, MaGVCN) VALUES
('1A1', N'Lớp 1A1', N'Khối 1', '2024-2025', 'GV001'),
('5A1', N'Lớp 5A1', N'Khối 5', '2024-2025', 'GV002'),
('2A1', N'Lớp 2A1', N'Khối 2', '2024-2025', NULL); 

-- Phân công giảng dạy (GV1 dạy hết 1A1, GV2 dạy hết 5A1 cho đơn giản)
INSERT INTO PhanCongGiangDay (MaGV, MaLop, MaMon) VALUES
('GV001', '1A1', 'TOAN'), ('GV001', '1A1', 'ANH'),
('GV002', '5A1', 'TOAN'), ('GV002', '5A1', 'ANH');

-- ==============================================================
-- 4. TẠO HỌC SINH (15 EM)
-- ==============================================================
PRINT 'Tao hoc sinh...';

-- Lớp 1A1 (7 học sinh: 5 Đậu, 2 Rớt)
INSERT INTO HocSinh (MaHS, MaLop, HoTen, NgaySinh, GioiTinh) VALUES
('HS101', '1A1', N'Nguyễn Văn An (Đậu)', '2018-01-01', N'Nam'),
('HS102', '1A1', N'Trần Thị Bình (Đậu)', '2018-02-02', N'Nữ'),
('HS103', '1A1', N'Lê Văn Cường (Đậu)', '2018-03-03', N'Nam'),
('HS104', '1A1', N'Phạm Thị Dung (Đậu)', '2018-04-04', N'Nữ'),
('HS105', '1A1', N'Hoàng Văn Em (Đậu)', '2018-05-05', N'Nam'),
('HS106', '1A1', N'Vũ Thị Fail (Rớt)', '2018-06-06', N'Nữ'),    -- Sẽ set điểm thấp
('HS107', '1A1', N'Đặng Văn Gớt (Rớt)', '2018-07-07', N'Nam'); -- Sẽ set điểm thấp

-- Lớp 5A1 (8 học sinh: 6 Tốt nghiệp, 2 Lưu ban)
INSERT INTO HocSinh (MaHS, MaLop, HoTen, NgaySinh, GioiTinh) VALUES
('HS501', '5A1', N'Nguyễn Tuấn Hùng (TN)', '2014-01-01', N'Nam'),
('HS502', '5A1', N'Trần Lan Hương (TN)', '2014-02-02', N'Nữ'),
('HS503', '5A1', N'Lê Minh Khôi (TN)', '2014-03-03', N'Nam'),
('HS504', '5A1', N'Phạm Ngọc Lan (TN)', '2014-04-04', N'Nữ'),
('HS505', '5A1', N'Hoàng Quốc Minh (TN)', '2014-05-05', N'Nam'),
('HS506', '5A1', N'Vũ Thùy Na (TN)', '2014-06-06', N'Nữ'),
('HS507', '5A1', N'Đặng Hùng Oai (LB)', '2014-07-07', N'Nam'), -- Điểm thấp
('HS508', '5A1', N'Bùi Thị Phượng (LB)', '2014-08-08', N'Nữ'); -- Điểm thấp

-- ==============================================================
-- 5. CẤU HÌNH ĐIỂM & NHẬP ĐIỂM GIẢ LẬP
-- ==============================================================
PRINT 'Cau hinh va nhap diem...';

-- Cấu hình thời hạn nhập điểm (Chỉ cần Giữa Kỳ 1 và Cuối Kỳ 2 để test nhanh)
INSERT INTO ThoiHanDiem (MaCotDiem, TenHienThi, Khoi, HocKy, NgayMoDiem, NgayKhoaDiem, KhoaThuCong)
VALUES
-- Khối 1
('GiuaKi1', N'Giữa Kỳ 1', N'Khối 1', 1, '2024-09-01', '2025-12-31', 0),
('CuoiKi2', N'Cuối Kỳ 2', N'Khối 1', 2, '2024-09-01', '2025-12-31', 0),
-- Khối 5
('GiuaKi1', N'Giữa Kỳ 1', N'Khối 5', 1, '2024-09-01', '2025-12-31', 0),
('CuoiKi2', N'Cuối Kỳ 2', N'Khối 5', 2, '2024-09-01', '2025-12-31', 0);

-- Tạo bảng điểm rỗng (Sử dụng SP hệ thống)
EXEC sp_TaoKetQuaHocTapMacDinh;

-- CẬP NHẬT ĐIỂM SỐ (UPDATE TRỰC TIẾP)

-- 1. NHÓM ĐẬU / TỐT NGHIỆP (Điểm 9.0 - 10.0)
UPDATE KetQuaHocTap SET Diem = 9.5 
WHERE MaHS IN ('HS101','HS102','HS103','HS104','HS105', 'HS501','HS502','HS503','HS504','HS505','HS506');

-- 2. NHÓM RỚT / LƯU BAN (Điểm 2.0 - 3.0)
UPDATE KetQuaHocTap SET Diem = 3.0 
WHERE MaHS IN ('HS106','HS107', 'HS507','HS508');

PRINT 'HOAN TAT! DATA DA SAN SANG DE TEST.';
USE quanlilophoc_giangday;
GO

PRINT 'Dang cap nhat du lieu cho Bao Cao Thang...';

-- 1. Đảm bảo cấu hình thời hạn điểm cho các tháng đã tồn tại (Phòng hờ)
-- (Script trước đã có, nhưng chạy lại để chắc chắn không bị thiếu)
IF NOT EXISTS (SELECT 1 FROM ThoiHanDiem WHERE MaCotDiem = 'Thang1_Ki1')
BEGIN
    INSERT INTO ThoiHanDiem (MaCotDiem, TenHienThi, Khoi, HocKy, NgayMoDiem, NgayKhoaDiem, KhoaThuCong)
    VALUES 
    ('Thang1_Ki1', N'Điểm Tháng 1 (Kỳ 1)', N'Khối 1', 1, '2024-09-01', '2025-12-31', 0),
    ('Thang2_Ki1', N'Điểm Tháng 2 (Kỳ 1)', N'Khối 1', 1, '2024-10-01', '2025-12-31', 0),
    ('Thang3_Ki1', N'Điểm Tháng 3 (Kỳ 1)', N'Khối 1', 1, '2024-11-01', '2025-12-31', 0),
    ('Thang1_Ki1', N'Điểm Tháng 1 (Kỳ 1)', N'Khối 5', 1, '2024-09-01', '2025-12-31', 0),
    ('Thang2_Ki1', N'Điểm Tháng 2 (Kỳ 1)', N'Khối 5', 1, '2024-10-01', '2025-12-31', 0),
    ('Thang3_Ki1', N'Điểm Tháng 3 (Kỳ 1)', N'Khối 5', 1, '2024-11-01', '2025-12-31', 0);
END

-- 2. Tạo các dòng điểm còn thiếu (nếu có) cho cột Tháng
-- (Vì script trước chỉ insert GiuaKi và CuoiKi)
INSERT INTO KetQuaHocTap (MaKQ, MaMon, MaHS, NgayNhap, Loai)
SELECT LEFT(NEWID(), 8), m.MaMon, hs.MaHS, GETDATE(), l.MaCotDiem
FROM HocSinh hs
CROSS JOIN MonHoc m
CROSS JOIN ThoiHanDiem l
WHERE l.MaCotDiem LIKE 'Thang%' -- Chỉ lấy cột tháng
AND NOT EXISTS (
    SELECT 1 FROM KetQuaHocTap kq 
    WHERE kq.MaHS = hs.MaHS AND kq.MaMon = m.MaMon AND kq.Loai = l.MaCotDiem
);

-- 3. CẬP NHẬT ĐIỂM SỐ CHO CÁC THÁNG (Để báo cáo có số liệu)
-- Set điểm ngẫu nhiên từ 7 đến 9 cho tất cả học sinh ở các tháng 1, 2, 3
UPDATE KetQuaHocTap 
SET Diem = 8, NgayNhap = GETDATE()
WHERE Loai IN ('Thang1_Ki1', 'Thang2_Ki1', 'Thang3_Ki1')
AND Diem IS NULL; -- Chỉ update những dòng chưa có điểm

-- Cập nhật một vài em điểm thấp để báo cáo sinh động hơn
UPDATE KetQuaHocTap 
SET Diem = 4 
WHERE MaHS IN ('HS106', 'HS107', 'HS507', 'HS508') -- Các em học sinh yếu trong kịch bản trước
AND Loai = 'Thang2_Ki1';

PRINT 'DA CAP NHAT DIEM THANG. BAO CAO DA HOAT DONG LAI!';
GO
GO