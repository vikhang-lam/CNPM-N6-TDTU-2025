--================================================================
-- HỦY VÀ TẠO MỚI DATABASE
--================================================================

CREATE DATABASE quanlilophoc_giangday;
GO
USE quanlilophoc_giangday;
GO
PRINT 'ĐÃ TẠO DATABASE MỚI: quanlilophoc_giangday.';

--================================================================
-- BƯỚC 1: TẠO CÁC BẢNG (ĐÃ SỬA LỖI FOREIGN KEY)
--================================================================
CREATE TABLE Admin (
    MaAdmin VARCHAR(10) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL,
    Password VARCHAR(30) NOT NULL,
    Email NVARCHAR(50) NOT NULL
);

CREATE TABLE MonHoc (
    MaMon VARCHAR(10) PRIMARY KEY,
    TenMon NVARCHAR(100) NOT NULL
);

CREATE TABLE Minigame (
    MaMNG VARCHAR(10) PRIMARY KEY,
    Ten NVARCHAR(100) NOT NULL,
    DuLieu NVARCHAR(MAX)
);

CREATE TABLE GiaoVien (
    MaGV VARCHAR(10) PRIMARY KEY,
    Ten NVARCHAR(100) NOT NULL,
    Username NVARCHAR(50) NOT NULL,
    Password VARCHAR(30) NOT NULL,
    Email NVARCHAR(50),
    SDT VARCHAR(15),
    MaAdmin VARCHAR(10),
    AnhDaiDien NVARCHAR(200),
    TrangThai NVARCHAR(20) DEFAULT N'Chưa xác nhận',
    FOREIGN KEY (MaAdmin) REFERENCES Admin(MaAdmin)
);

CREATE TABLE GiaoVien_MonHoc (
    MaGV VARCHAR(10) NOT NULL,
    MaMon VARCHAR(10) NOT NULL,
    PRIMARY KEY (MaGV, MaMon),
    FOREIGN KEY (MaGV) REFERENCES GiaoVien(MaGV) ON DELETE CASCADE,
    FOREIGN KEY (MaMon) REFERENCES MonHoc(MaMon) ON DELETE CASCADE -- <<< ĐÃ SỬA
);

CREATE TABLE LopHoc (
    MaLop VARCHAR(10) PRIMARY KEY,
    TenLop NVARCHAR(50) NOT NULL,
    Khoi NVARCHAR(20),
    NamHoc VARCHAR(10),
    MaGVCN VARCHAR(10),
    FOREIGN KEY (MaGVCN) REFERENCES GiaoVien(MaGV)
);

CREATE TABLE HocSinh (
    MaHS VARCHAR(10) PRIMARY KEY,
    MaLop VARCHAR(10),
    HoTen NVARCHAR(100) NOT NULL,
    DanToc NVARCHAR(50),
    GioiTinh NVARCHAR(10),
    SDTPhuHuynh VARCHAR(15),
    DiaChi NVARCHAR(200),
    NgaySinh DATE,
    FOREIGN KEY (MaLop) REFERENCES LopHoc(MaLop)
);

CREATE TABLE PhanCongGiangDay (
    MaGV VARCHAR(10) NOT NULL,
    MaLop VARCHAR(10) NOT NULL,
    MaMon VARCHAR(10) NOT NULL,
    PRIMARY KEY (MaLop, MaMon), 
    FOREIGN KEY (MaGV) REFERENCES GiaoVien(MaGV) ON DELETE CASCADE,
    FOREIGN KEY (MaLop) REFERENCES LopHoc(MaLop) ON DELETE CASCADE, -- <<< ĐÃ SỬA
    FOREIGN KEY (MaMon) REFERENCES MonHoc(MaMon) ON DELETE CASCADE  -- <<< ĐÃ SỬA
);

CREATE TABLE DiemDanh (
    MaDD VARCHAR(10) PRIMARY KEY,
    MaHS VARCHAR(10),
    NgayDD DATETIME  DEFAULT GETDATE(),
    Buoi NVARCHAR(10),
    TrangThai NVARCHAR(20),
	ThoiGianCapNhat DATETIME,
    FOREIGN KEY (MaHS) REFERENCES HocSinh(MaHS) ON DELETE CASCADE
);

CREATE TABLE KetQuaHocTap (
    MaKQ VARCHAR(10) PRIMARY KEY,
    MaMon VARCHAR(10),
    MaHS VARCHAR(10),
    NgayNhap DATE,
    NhanXet NVARCHAR(200),
    GhiChu NVARCHAR(200),
    Loai NVARCHAR(20),
    Diem FLOAT,
    FOREIGN KEY (MaMon) REFERENCES MonHoc(MaMon), -- <<< ĐÃ SỬA
    FOREIGN KEY (MaHS) REFERENCES HocSinh(MaHS) ON DELETE CASCADE
);

CREATE TABLE TaiLieu (
    MaTL VARCHAR(10) PRIMARY KEY,
    TenTL NVARCHAR(100) NOT NULL,
    MoTa NVARCHAR(200),
    Kieu NVARCHAR(200),
    NgayTaiLen DATE,
    TrangThaiChiaSe NVARCHAR(20),
    MaGV VARCHAR(10),
    FOREIGN KEY (MaGV) REFERENCES GiaoVien(MaGV) ON DELETE CASCADE
);

CREATE TABLE ThoiKhoaBieu (
    MaTKB VARCHAR(10) PRIMARY KEY,
    Ngay DATE,
    Tiet INT,
    MaMon VARCHAR(10),
    GhiChu NVARCHAR(200),
    MaGV VARCHAR(10),
    MaLop VARCHAR(10),
    MauSac VARCHAR(20) NULL,
    FOREIGN KEY (MaMon) REFERENCES MonHoc(MaMon), -- <<< ĐÃ SỬA
    FOREIGN KEY (MaGV) REFERENCES GiaoVien(MaGV),
    FOREIGN KEY (MaLop) REFERENCES LopHoc(MaLop)  -- <<< ĐÃ SỬA
);

CREATE TABLE BaoCao (
    MaBC VARCHAR(10) PRIMARY KEY,
    TenBC NVARCHAR(100),
    ThoiGian DATETIME,
    DinhDang NVARCHAR(20),
    TenFolder NVARCHAR(100),
    MaGV VARCHAR(10),
    NoiDung NVARCHAR(500),
    FOREIGN KEY (MaGV) REFERENCES GiaoVien(MaGV)
);

CREATE TABLE QuyLop (
    MaQL VARCHAR(10) PRIMARY KEY,
    MaLop VARCHAR(10),
    SoTien DECIMAL(12,2),
    Ngay DATE,
    GhiChu NVARCHAR(200),
    Loai NVARCHAR(10),
    FOREIGN KEY (MaLop) REFERENCES LopHoc(MaLop)
);

CREATE TABLE ThoiHanDiem (
    MaCotDiem VARCHAR(20) NOT NULL,
    TenHienThi NVARCHAR(100) NOT NULL,
    Khoi NVARCHAR(20) NOT NULL,
    HocKy INT NOT NULL,
    NgayMoDiem DATE,
    NgayKhoaDiem DATE,
    KhoaThuCong BIT NOT NULL DEFAULT 0,
    PRIMARY KEY (MaCotDiem, Khoi, HocKy)
);

PRINT 'ĐÃ TẠO TẤT CẢ CÁC BẢNG (ĐÃ SỬA LỖI FK).';
GO
--================================================================
-- BƯỚC 2: CHÈN DỮ LIỆU MẪU
--================================================================

-- 1. Admin
INSERT INTO Admin (MaAdmin, Username, Password, Email) VALUES ('AD001', 'admin', '123456', 'admin@example.com');

-- 2. Môn Học
INSERT INTO MonHoc (MaMon, TenMon) VALUES 
('TV', N'Tiếng Việt'),
('TOAN', N'Toán'),
('KH', N'Khoa học'),
('LS_DL', N'Lịch sử và Địa lí'),
('ANH', N'Tiếng Anh'),
('DD', N'Đạo đức'),
('AN', N'Âm nhạc'),
('MT', N'Mĩ thuật'),
('TIN', N'Tin học và Công nghệ (Tin học)'),
('CN', N'Tin học và Công nghệ (Công nghệ)'),
('GDTC', N'Giáo dục thể chất'),
('TDT', N'Tiếng dân tộc'),
('HDTN', N'Hoạt động trải nghiệm');

-- 3. Lớp Học
INSERT INTO LopHoc (MaLop, TenLop, Khoi, NamHoc) VALUES
('1A1', N'Lớp 1A1', N'Khối 1', '2025'),
('1A2', N'Lớp 1A2', N'Khối 1', '2025'),
('1A3', N'Lớp 1A3', N'Khối 1', '2025'),
('2A1', N'Lớp 2A1', N'Khối 2', '2025'),
('2A2', N'Lớp 2A2', N'Khối 2', '2025'),
('2A3', N'Lớp 2A3', N'Khối 2', '2025'),
('3A1', N'Lớp 3A1', N'Khối 3', '2025'),
('3A2', N'Lớp 3A2', N'Khối 3', '2025'),
('3A3', N'Lớp 3A3', N'Khối 3', '2025'),
('4A1', N'Lớp 4A1', N'Khối 4', '2025'),
('4A2', N'Lớp 4A2', N'Khối 4', '2025'),
('4A3', N'Lớp 4A3', N'Khối 4', '2025'),
('5A1', N'Lớp 5A1', N'Khối 5', '2025'),
('5A2', N'Lớp 5A2', N'Khối 5', '2025'),
('5A3', N'Lớp 5A3', N'Khối 5', '2025');

-- 4. Giáo Viên
INSERT INTO GiaoVien (MaGV, Ten, Username, Password, Email, SDT, MaAdmin, TrangThai) VALUES
('GV001', N'Cô Minh Anh', 'minhanh', '123456', 'minhanh@example.com', '0905123001', 'AD001', N'Đã xác nhận'),
('GV002', N'Thầy Quốc Hưng', 'quochung', '123456', 'quochung@example.com', '0912345002', 'AD001', N'Đã xác nhận'),
('GV003', N'Cô Thu Hà', 'thuha', '123456', 'thuha@example.com', '0912345003', 'AD001', N'Đã xác nhận'),
('GV004', N'Thầy Bá Trung', 'batrung', '123456', 'batrung@example.com', '0912345004', 'AD001', N'Đã xác nhận'),
('GV005', N'Cô Thanh Tâm', 'thanhtam', '123456', 'tam@example.com', '0912345005', 'AD001', N'Đã xác nhận'),
('GV006', N'Thầy Văn Toàn', 'vantoan', '123456', 'toan@example.com', '0912345006', 'AD001', N'Đã xác nhận'),
('GV007', N'Cô Bích Phương', 'bichphuong', '123456', 'phuong@example.com', '0912345007', 'AD001', N'Đã xác nhận'),
('GV008', N'Thầy Trọng Tấn', 'trongtan', '123456', 'tan@example.com', '0912345008', 'AD001', N'Đã xác nhận'),
('GV009', N'Cô Mỹ Linh', 'mylinh', '123456', 'linh@example.com', '0912345009', 'AD001', N'Đã xác nhận'),
('GV010', N'Thầy Đức Thắng', 'ducthang', '123456', 'thang@example.com', '0912345010', 'AD001', N'Đã xác nhận'),
('GV011', N'Cô Hoài An', 'hoaian', '123456', 'an@example.com', '0912345011', 'AD001', N'Đã xác nhận'),
('GV012', N'Thầy Nam Sơn', 'namson', '123456', 'son@example.com', '0912345012', 'AD001', N'Đã xác nhận'),
('GV013', N'Cô Ánh Tuyết', 'anhtuyet', '123456', 'tuyet@example.com', '0912345013', 'AD001', N'Đã xác nhận'),
('GV014', N'Thầy Việt Hoàng', 'viethoang', '123456', 'hoang@example.com', '0912345014', 'AD001', N'Đã xác nhận'),
('GV015', N'Cô Mai Lan', 'mailan', '123456', 'lan@example.com', '0912345015', 'AD001', N'Đã xác nhận'),
('GV016', N'Thầy Đình Phong', 'dinhphong', '123456', 'phong@example.com', '0912345016', 'AD001', N'Chưa xác nhận'),
('GV017', N'Cô Thùy Chi', 'thuychi', '123456', 'chi@example.com', '0912345017', 'AD001', N'Đã xác nhận'),
('GV018', N'Thầy Hùng Dũng', 'hungdung', '123456', 'dung@example.com', '0912345018', 'AD001', N'Đã xác nhận'),
('GV019', N'Cô Bảo Trâm', 'baotram', '123456', 'tram@example.com', '0912345019', 'AD001', N'Đã xác nhận'),
('GV020', N'Thầy Quang Minh', 'quangminh', '123456', 'minh@example.com', '0912345020', 'AD001', N'Đã xác nhận');

-- 5. Giáo Viên - Môn Học
INSERT INTO GiaoVien_MonHoc (MaGV, MaMon) VALUES
('GV001', 'TV'), ('GV001', 'DD'), ('GV001', 'HDTN'),
('GV002', 'TOAN'), ('GV002', 'KH'),
('GV003', 'ANH'),
('GV004', 'TIN'), ('GV004', 'CN'),
('GV005', 'TV'), ('GV005', 'LS_DL'),
('GV006', 'TOAN'), ('GV006', 'KH'),
('GV007', 'TV'), ('GV007', 'LS_DL'),
('GV008', 'GDTC'),
('GV009', 'AN'),
('GV010', 'MT'),
('GV011', 'TV'), ('GV011', 'DD'), ('GV011', 'HDTN'),
('GV012', 'TOAN'), ('GV012', 'KH'),
('GV013', 'TV'), ('GV013', 'LS_DL'),
('GV014', 'TOAN'), ('GV014', 'KH'),
('GV015', 'TV'), ('GV015', 'LS_DL'),
('GV017', 'ANH'),
('GV018', 'GDTC'),
('GV019', 'TIN'), ('GV019', 'CN'),
('GV020', 'TDT');

-- 6. Phân Công GVCN cho Lớp
UPDATE LopHoc SET MaGVCN = 'GV001' WHERE MaLop = '1A1';
UPDATE LopHoc SET MaGVCN = 'GV011' WHERE MaLop = '1A2';
UPDATE LopHoc SET MaGVCN = 'GV011' WHERE MaLop = '1A3';
UPDATE LopHoc SET MaGVCN = 'GV002' WHERE MaLop = '2A1';
UPDATE LopHoc SET MaGVCN = 'GV012' WHERE MaLop = '2A2';
UPDATE LopHoc SET MaGVCN = 'GV002' WHERE MaLop = '2A3';
UPDATE LopHoc SET MaGVCN = 'GV005' WHERE MaLop = '3A1';
UPDATE LopHoc SET MaGVCN = 'GV013' WHERE MaLop = '3A2';
UPDATE LopHoc SET MaGVCN = 'GV005' WHERE MaLop = '3A3';
UPDATE LopHoc SET MaGVCN = 'GV006' WHERE MaLop = '4A1';
UPDATE LopHoc SET MaGVCN = 'GV014' WHERE MaLop = '4A2';
UPDATE LopHoc SET MaGVCN = 'GV006' WHERE MaLop = '4A3';
UPDATE LopHoc SET MaGVCN = 'GV007' WHERE MaLop = '5A1';
UPDATE LopHoc SET MaGVCN = 'GV015' WHERE MaLop = '5A2';
UPDATE LopHoc SET MaGVCN = 'GV007' WHERE MaLop = '5A3';

-- 7. Phân Công Giảng Dạy
INSERT INTO PhanCongGiangDay (MaGV, MaLop, MaMon) VALUES
('GV007', '5A1', 'TV'), ('GV014', '5A1', 'TOAN'), ('GV003', '5A1', 'ANH'), ('GV014', '5A1', 'KH'), ('GV007', '5A1', 'LS_DL'), ('GV004', '5A1', 'TIN'), ('GV008', '5A1', 'GDTC'), ('GV009', '5A1', 'AN'), ('GV010', '5A1', 'MT'), ('GV004', '5A1', 'CN'), ('GV001', '5A1', 'DD'), ('GV001', '5A1', 'HDTN'),
('GV015', '5A2', 'TV'), ('GV006', '5A2', 'TOAN'), ('GV017', '5A2', 'ANH'), ('GV006', '5A2', 'KH'), ('GV015', '5A2', 'LS_DL'), ('GV019', '5A2', 'TIN'), ('GV018', '5A2', 'GDTC'), ('GV009', '5A2', 'AN'), ('GV010', '5A2', 'MT'), ('GV019', '5A2', 'CN'), ('GV005', '5A2', 'DD'), ('GV005', '5A2', 'HDTN'),
('GV007', '5A3', 'TV'), ('GV014', '5A3', 'TOAN'), ('GV003', '5A3', 'ANH'), ('GV014', '5A3', 'KH'), ('GV007', '5A3', 'LS_DL'), ('GV004', '5A3', 'TIN'), ('GV008', '5A3', 'GDTC'), ('GV009', '5A3', 'AN'), ('GV010', '5A3', 'MT'), ('GV004', '5A3', 'CN'), ('GV001', '5A3', 'DD'), ('GV001', '5A3', 'HDTN'),
('GV001', '1A1', 'TV'), ('GV002', '1A1', 'TOAN'), ('GV003', '1A1', 'ANH'), ('GV001', '1A1', 'DD'), ('GV009', '1A1', 'AN'), ('GV010', '1A1', 'MT'), ('GV008', '1A1', 'GDTC'), ('GV001', '1A1', 'HDTN'),
('GV005', '3A1', 'TV'), ('GV012', '3A1', 'TOAN'), ('GV017', '3A1', 'ANH'), ('GV004', '3A1', 'TIN'), ('GV005', '3A1', 'DD'), ('GV009', '3A1', 'AN'), ('GV010', '3A1', 'MT'), ('GV018', '3A1', 'GDTC'), ('GV005', '3A1', 'HDTN');
GO

-- 8. Học Sinh (300 HỌC SINH)
-- Lớp 1A1
INSERT INTO HocSinh (MaHS, MaLop, HoTen, DanToc, GioiTinh, SDTPhuHuynh, DiaChi, NgaySinh) VALUES
('HS001', '1A1', N'Nguyễn Hoàng An', N'Kinh', N'Nam', '0912345001', N'Hà Nội', '2019-01-01'),
('HS002', '1A1', N'Trần Bảo Bình', N'Kinh', N'Nữ', '0912345002', N'Hà Nội', '2019-02-02'),
('HS003', '1A1', N'Lê Gia Cát', N'Kinh', N'Nam', '0912345003', N'Hà Nội', '2019-03-03'),
('HS004', '1A1', N'Phạm Minh Dũng', N'Kinh', N'Nam', '0912345004', N'Hà Nội', '2019-04-04'),
('HS005', '1A1', N'Đỗ Phương Giang', N'Kinh', N'Nữ', '0912345005', N'Hà Nội', '2019-05-05'),
('HS006', '1A1', N'Vũ Gia Hân', N'Kinh', N'Nữ', '0912345006', N'Hà Nội', '2019-06-06'),
('HS007', '1A1', N'Hoàng Tuấn Kiệt', N'Kinh', N'Nam', '0912345007', N'Hà Nội', '2019-07-07'),
('HS008', '1A1', N'Bùi Khánh Linh', N'Kinh', N'Nữ', '0912345008', N'Hà Nội', '2019-08-08'),
('HS009', '1A1', N'Đặng Quốc Minh', N'Kinh', N'Nam', '0912345009', N'Hà Nội', '2019-09-09'),
('HS010', '1A1', N'Ngô Bảo Nam', N'Kinh', N'Nam', '0912345010', N'Hà Nội', '2019-10-10'),
('HS011', '1A1', N'Hồ Thùy Oanh', N'Kinh', N'Nữ', '0912345011', N'Hà Nội', '2019-11-11'),
('HS012', '1A1', N'Dương Minh Phúc', N'Kinh', N'Nam', '0912345012', N'Hà Nội', '2019-12-12'),
('HS013', '1A1', N'Mai Tú Quyên', N'Kinh', N'Nữ', '0912345013', N'Hà Nội', '2019-01-13'),
('HS014', '1A1', N'Phan Hoàng Quân', N'Kinh', N'Nam', '0912345014', N'Hà Nội', '2019-02-14'),
('HS015', '1A1', N'Lý Gia Hân', N'Kinh', N'Nữ', '0912345015', N'Hà Nội', '2019-03-15'),
('HS016', '1A1', N'Vương Minh Tâm', N'Kinh', N'Nam', '0912345016', N'Hà Nội', '2019-04-16'),
('HS017', '1A1', N'Tô Phương Uyên', N'Kinh', N'Nữ', '0912345017', N'Hà Nội', '2019-05-17'),
('HS018', '1A1', N'Trịnh Tuấn Vũ', N'Kinh', N'Nam', '0912345018', N'Hà Nội', '2019-06-18'),
('HS019', '1A1', N'Cao Hoàng Yến', N'Kinh', N'Nữ', '0912345019', N'Hà Nội', '2019-07-19'),
('HS020', '1A1', N'Giang Minh Triết', N'Kinh', N'Nam', '0912345020', N'Hà Nội', '2019-08-20');
-- Lớp 1A2
INSERT INTO HocSinh (MaHS, MaLop, HoTen, DanToc, GioiTinh, SDTPhuHuynh, DiaChi, NgaySinh) VALUES
('HS021', '1A2', N'Nguyễn Văn A', N'Kinh', N'Nam', '0922345001', N'Hải Phòng', '2019-01-01'),
('HS022', '1A2', N'Trần Thị B', N'Kinh', N'Nữ', '0922345002', N'Hải Phòng', '2019-02-02'),
('HS023', '1A2', N'Lê Văn C', N'Kinh', N'Nam', '0922345003', N'Hải Phòng', '2019-03-03'),
('HS024', '1A2', N'Phạm Thị D', N'Kinh', N'Nữ', '0922345004', N'Hải Phòng', '2019-04-04'),
('HS025', '1A2', N'Đỗ Văn E', N'Kinh', N'Nam', '0922345005', N'Hải Phòng', '2019-05-05'),
('HS026', '1A2', N'Vũ Thị F', N'Kinh', N'Nữ', '0922345006', N'Hải Phòng', '2019-06-06'),
('HS027', '1A2', N'Hoàng Văn G', N'Kinh', N'Nam', '0922345007', N'Hải Phòng', '2019-07-07'),
('HS028', '1A2', N'Bùi Thị H', N'Kinh', N'Nữ', '0922345008', N'Hải Phòng', '2019-08-08'),
('HS029', '1A2', N'Đặng Văn I', N'Kinh', N'Nam', '0922345009', N'Hải Phòng', '2019-09-09'),
('HS030', '1A2', N'Ngô Văn K', N'Kinh', N'Nam', '0922345010', N'Hải Phòng', '2019-10-10'),
('HS031', '1A2', N'Hồ Thị L', N'Kinh', N'Nữ', '0922345011', N'Hải Phòng', '2019-11-11'),
('HS032', '1A2', N'Dương Văn M', N'Kinh', N'Nam', '0922345012', N'Hải Phòng', '2019-12-12'),
('HS033', '1A2', N'Mai Thị N', N'Kinh', N'Nữ', '0922345013', N'Hải Phòng', '2019-01-13'),
('HS034', '1A2', N'Phan Văn P', N'Kinh', N'Nam', '0922345014', N'Hải Phòng', '2019-02-14'),
('HS035', '1A2', N'Lý Thị Q', N'Kinh', N'Nữ', '0922345015', N'Hải Phòng', '2019-03-15'),
('HS036', '1A2', N'Vương Văn R', N'Kinh', N'Nam', '0922345016', N'Hải Phòng', '2019-04-16'),
('HS037', '1A2', N'Tô Thị S', N'Kinh', N'Nữ', '0922345017', N'Hải Phòng', '2019-05-17'),
('HS038', '1A2', N'Trịnh Văn T', N'Kinh', N'Nam', '0922345018', N'Hải Phòng', '2019-06-18'),
('HS039', '1A2', N'Cao Thị U', N'Kinh', N'Nữ', '0922345019', N'Hải Phòng', '2019-07-19'),
('HS040', '1A2', N'Giang Văn V', N'Kinh', N'Nam', '0922345020', N'Hải Phòng', '2019-08-20');
-- Lớp 1A3
INSERT INTO HocSinh (MaHS, MaLop, HoTen, DanToc, GioiTinh, SDTPhuHuynh, DiaChi, NgaySinh) VALUES
('HS041', '1A3', N'Nguyễn Ánh Dương', N'Kinh', N'Nam', '0932345001', N'Đà Nẵng', '2019-01-01'),
('HS042', '1A3', N'Trần Ngọc Bích', N'Kinh', N'Nữ', '0932345002', N'Đà Nẵng', '2019-02-02'),
('HS043', '1A3', N'Lê Minh Châu', N'Kinh', N'Nam', '0932345003', N'Đà Nẵng', '2019-03-03'),
('HS044', '1A3', N'Phạm Hải Đăng', N'Kinh', N'Nam', '0932345004', N'Đà Nẵng', '2019-04-04'),
('HS045', '1A3', N'Đỗ Hà Giang', N'Kinh', N'Nữ', '0932345005', N'Đà Nẵng', '2019-05-05'),
('HS046', '1A3', N'Vũ Hoàng Hải', N'Kinh', N'Nam', '0932345006', N'Đà Nẵng', '2019-06-06'),
('HS047', '1A3', N'Hoàng Khánh Huyền', N'Kinh', N'Nữ', '0932345007', N'Đà Nẵng', '2019-07-07'),
('HS048', '1A3', N'Bùi Minh Khang', N'Kinh', N'Nam', '0932345008', N'Đà Nẵng', '2019-08-08'),
('HS049', '1A3', N'Đặng Tuệ Lâm', N'Kinh', N'Nữ', '0932345009', N'Đà Nẵng', '2019-09-09'),
('HS050', '1A3', N'Ngô Gia Long', N'Kinh', N'Nam', '0932345010', N'Đà Nẵng', '2019-10-10'),
('HS051', '1A3', N'Hồ Ngọc Mai', N'Kinh', N'Nữ', '0932345011', N'Đà Nẵng', '2019-11-11'),
('HS052', '1A3', N'Dương Quốc Phong', N'Kinh', N'Nam', '0932345012', N'Đà Nẵng', '2019-12-12'),
('HS053', '1A3', N'Mai Bảo Quyên', N'Kinh', N'Nữ', '0932345013', N'Đà Nẵng', '2019-01-13'),
('HS054', '1A3', N'Phan Minh Sơn', N'Kinh', N'Nam', '0932345014', N'Đà Nẵng', '2019-02-14'),
('HS055', '1A3', N'Lý Thảo Trang', N'Kinh', N'Nữ', '0932345015', N'Đà Nẵng', '2019-03-15'),
('HS056', '1A3', N'Vương Anh Tuấn', N'Kinh', N'Nam', '0932345016', N'Đà Nẵng', '2019-04-16'),
('HS057', '1A3', N'Tô Diệp Vy', N'Kinh', N'Nữ', '0932345017', N'Đà Nẵng', '2019-05-17'),
('HS058', '1A3', N'Trịnh Xuân Trường', N'Kinh', N'Nam', '0932345018', N'Đà Nẵng', '2019-06-18'),
('HS059', '1A3', N'Cao Thùy Anh', N'Kinh', N'Nữ', '0932345019', N'Đà Nẵng', '2019-07-19'),
('HS060', '1A3', N'Giang Tuấn Phong', N'Kinh', N'Nam', '0932345020', N'Đà Nẵng', '2019-08-20');
-- Lớp 2A1
INSERT INTO HocSinh (MaHS, MaLop, HoTen, DanToc, GioiTinh, SDTPhuHuynh, DiaChi, NgaySinh) VALUES
('HS061', '2A1', N'Nguyễn Hoàng An', N'Kinh', N'Nam', '0912345001', N'Hà Nội', '2018-01-01'),
('HS062', '2A1', N'Trần Bảo Bình', N'Kinh', N'Nữ', '0912345002', N'Hà Nội', '2018-02-02'),
('HS063', '2A1', N'Lê Gia Cát', N'Kinh', N'Nam', '0912345003', N'Hà Nội', '2018-03-03'),
('HS064', '2A1', N'Phạm Minh Dũng', N'Kinh', N'Nam', '0912345004', N'Hà Nội', '2018-04-04'),
('HS065', '2A1', N'Đỗ Phương Giang', N'Kinh', N'Nữ', '0912345005', N'Hà Nội', '2018-05-05'),
('HS066', '2A1', N'Vũ Gia Hân', N'Kinh', N'Nữ', '0912345006', N'Hà Nội', '2018-06-06'),
('HS067', '2A1', N'Hoàng Tuấn Kiệt', N'Kinh', N'Nam', '0912345007', N'Hà Nội', '2018-07-07'),
('HS068', '2A1', N'Bùi Khánh Linh', N'Kinh', N'Nữ', '0912345008', N'Hà Nội', '2018-08-08'),
('HS069', '2A1', N'Đặng Quốc Minh', N'Kinh', N'Nam', '0912345009', N'Hà Nội', '2018-09-09'),
('HS070', '2A1', N'Ngô Bảo Nam', N'Kinh', N'Nam', '0912345010', N'Hà Nội', '2018-10-10'),
('HS071', '2A1', N'Hồ Thùy Oanh', N'Kinh', N'Nữ', '0912345011', N'Hà Nội', '2018-11-11'),
('HS072', '2A1', N'Dương Minh Phúc', N'Kinh', N'Nam', '0912345012', N'Hà Nội', '2018-12-12'),
('HS073', '2A1', N'Mai Tú Quyên', N'Kinh', N'Nữ', '0912345013', N'Hà Nội', '2018-01-13'),
('HS074', '2A1', N'Phan Hoàng Quân', N'Kinh', N'Nam', '0912345014', N'Hà Nội', '2018-02-14'),
('HS075', '2A1', N'Lý Gia Hân', N'Kinh', N'Nữ', '0912345015', N'Hà Nội', '2018-03-15'),
('HS076', '2A1', N'Vương Minh Tâm', N'Kinh', N'Nam', '0912345016', N'Hà Nội', '2018-04-16'),
('HS077', '2A1', N'Tô Phương Uyên', N'Kinh', N'Nữ', '0912345017', N'Hà Nội', '2018-05-17'),
('HS078', '2A1', N'Trịnh Tuấn Vũ', N'Kinh', N'Nam', '0912345018', N'Hà Nội', '2018-06-18'),
('HS079', '2A1', N'Cao Hoàng Yến', N'Kinh', N'Nữ', '0912345019', N'Hà Nội', '2018-07-19'),
('HS080', '2A1', N'Giang Minh Triết', N'Kinh', N'Nam', '0912345020', N'Hà Nội', '2018-08-20');
-- Lớp 2A2
INSERT INTO HocSinh (MaHS, MaLop, HoTen, DanToc, GioiTinh, SDTPhuHuynh, DiaChi, NgaySinh) VALUES
('HS081', '2A2', N'Nguyễn Văn A', N'Kinh', N'Nam', '0922345001', N'Hải Phòng', '2018-01-01'),
('HS082', '2A2', N'Trần Thị B', N'Kinh', N'Nữ', '0922345002', N'Hải Phòng', '2018-02-02'),
('HS083', '2A2', N'Lê Văn C', N'Kinh', N'Nam', '0922345003', N'Hải Phòng', '2018-03-03'),
('HS084', '2A2', N'Phạm Thị D', N'Kinh', N'Nữ', '0922345004', N'Hải Phòng', '2018-04-04'),
('HS085', '2A2', N'Đỗ Văn E', N'Kinh', N'Nam', '0922345005', N'Hải Phòng', '2018-05-05'),
('HS086', '2A2', N'Vũ Thị F', N'Kinh', N'Nữ', '0922345006', N'Hải Phòng', '2018-06-06'),
('HS087', '2A2', N'Hoàng Văn G', N'Kinh', N'Nam', '0922345007', N'Hải Phòng', '2018-07-07'),
('HS088', '2A2', N'Bùi Thị H', N'Kinh', N'Nữ', '0922345008', N'Hải Phòng', '2018-08-08'),
('HS089', '2A2', N'Đặng Văn I', N'Kinh', N'Nam', '0922345009', N'Hải Phòng', '2018-09-09'),
('HS090', '2A2', N'Ngô Văn K', N'Kinh', N'Nam', '0922345010', N'Hải Phòng', '2018-10-10'),
('HS091', '2A2', N'Hồ Thị L', N'Kinh', N'Nữ', '0922345011', N'Hải Phòng', '2018-11-11'),
('HS092', '2A2', N'Dương Văn M', N'Kinh', N'Nam', '0922345012', N'Hải Phòng', '2018-12-12'),
('HS093', '2A2', N'Mai Thị N', N'Kinh', N'Nữ', '0922345013', N'Hải Phòng', '2018-01-13'),
('HS094', '2A2', N'Phan Văn P', N'Kinh', N'Nam', '0922345014', N'Hải Phòng', '2018-02-14'),
('HS095', '2A2', N'Lý Thị Q', N'Kinh', N'Nữ', '0922345015', N'Hải Phòng', '2018-03-15'),
('HS096', '2A2', N'Vương Văn R', N'Kinh', N'Nam', '0922345016', N'Hải Phòng', '2018-04-16'),
('HS097', '2A2', N'Tô Thị S', N'Kinh', N'Nữ', '0922345017', N'Hải Phòng', '2018-05-17'),
('HS098', '2A2', N'Trịnh Văn T', N'Kinh', N'Nam', '0922345018', N'Hải Phòng', '2018-06-18'),
('HS099', '2A2', N'Cao Thị U', N'Kinh', N'Nữ', '0922345019', N'Hải Phòng', '2018-07-19'),
('HS100', '2A2', N'Giang Văn V', N'Kinh', N'Nam', '0922345020', N'Hải Phòng', '2018-08-20');
-- Lớp 2A3
INSERT INTO HocSinh (MaHS, MaLop, HoTen, DanToc, GioiTinh, SDTPhuHuynh, DiaChi, NgaySinh) VALUES
('HS101', '2A3', N'Nguyễn Ánh Dương', N'Kinh', N'Nam', '0932345001', N'Đà Nẵng', '2018-01-01'),
('HS102', '2A3', N'Trần Ngọc Bích', N'Kinh', N'Nữ', '0932345002', N'Đà Nẵng', '2018-02-02'),
('HS103', '2A3', N'Lê Minh Châu', N'Kinh', N'Nam', '0932345003', N'Đà Nẵng', '2018-03-03'),
('HS104', '2A3', N'Phạm Hải Đăng', N'Kinh', N'Nam', '0932345004', N'Đà Nẵng', '2018-04-04'),
('HS105', '2A3', N'Đỗ Hà Giang', N'Kinh', N'Nữ', '0932345005', N'Đà Nẵng', '2018-05-05'),
('HS106', '2A3', N'Vũ Hoàng Hải', N'Kinh', N'Nam', '0932345006', N'Đà Nẵng', '2018-06-06'),
('HS107', '2A3', N'Hoàng Khánh Huyền', N'Kinh', N'Nữ', '0932345007', N'Đà Nẵng', '2018-07-07'),
('HS108', '2A3', N'Bùi Minh Khang', N'Kinh', N'Nam', '0932345008', N'Đà Nẵng', '2018-08-08'),
('HS109', '2A3', N'Đặng Tuệ Lâm', N'Kinh', N'Nữ', '0932345009', N'Đà Nẵng', '2018-09-09'),
('HS110', '2A3', N'Ngô Gia Long', N'Kinh', N'Nam', '0932345010', N'Đà Nẵng', '2018-10-10'),
('HS111', '2A3', N'Hồ Ngọc Mai', N'Kinh', N'Nữ', '0932345011', N'Đà Nẵng', '2018-11-11'),
('HS112', '2A3', N'Dương Quốc Phong', N'Kinh', N'Nam', '0932345012', N'Đà Nẵng', '2018-12-12'),
('HS113', '2A3', N'Mai Bảo Quyên', N'Kinh', N'Nữ', '0932345013', N'Đà Nẵng', '2018-01-13'),
('HS114', '2A3', N'Phan Minh Sơn', N'Kinh', N'Nam', '0932345014', N'Đà Nẵng', '2018-02-14'),
('HS115', '2A3', N'Lý Thảo Trang', N'Kinh', N'Nữ', '0932345015', N'Đà Nẵng', '2018-03-15'),
('HS116', '2A3', N'Vương Anh Tuấn', N'Kinh', N'Nam', '0932345016', N'Đà Nẵng', '2018-04-16'),
('HS117', '2A3', N'Tô Diệp Vy', N'Kinh', N'Nữ', '0932345017', N'Đà Nẵng', '2018-05-17'),
('HS118', '2A3', N'Trịnh Xuân Trường', N'Kinh', N'Nam', '0932345018', N'Đà Nẵng', '2018-06-18'),
('HS119', '2A3', N'Cao Thùy Anh', N'Kinh', N'Nữ', '0932345019', N'Đà Nẵng', '2018-07-19'),
('HS120', '2A3', N'Giang Tuấn Phong', N'Kinh', N'Nam', '0932345020', N'Đà Nẵng', '2018-08-20');
-- Lớp 3A1
INSERT INTO HocSinh (MaHS, MaLop, HoTen, DanToc, GioiTinh, SDTPhuHuynh, DiaChi, NgaySinh) VALUES
('HS121', '3A1', N'Nguyễn Hoàng An', N'Kinh', N'Nam', '0912345001', N'Hà Nội', '2017-01-01'),
('HS122', '3A1', N'Trần Bảo Bình', N'Kinh', N'Nữ', '0912345002', N'Hà Nội', '2017-02-02'),
('HS123', '3A1', N'Lê Gia Cát', N'Kinh', N'Nam', '0912345003', N'Hà Nội', '2017-03-03'),
('HS124', '3A1', N'Phạm Minh Dũng', N'Kinh', N'Nam', '0912345004', N'Hà Nội', '2017-04-04'),
('HS125', '3A1', N'Đỗ Phương Giang', N'Kinh', N'Nữ', '0912345005', N'Hà Nội', '2017-05-05'),
('HS126', '3A1', N'Vũ Gia Hân', N'Kinh', N'Nữ', '0912345006', N'Hà Nội', '2017-06-06'),
('HS127', '3A1', N'Hoàng Tuấn Kiệt', N'Kinh', N'Nam', '0912345007', N'Hà Nội', '2017-07-07'),
('HS128', '3A1', N'Bùi Khánh Linh', N'Kinh', N'Nữ', '0912345008', N'Hà Nội', '2017-08-08'),
('HS129', '3A1', N'Đặng Quốc Minh', N'Kinh', N'Nam', '0912345009', N'Hà Nội', '2017-09-09'),
('HS130', '3A1', N'Ngô Bảo Nam', N'Kinh', N'Nam', '0912345010', N'Hà Nội', '2017-10-10'),
('HS131', '3A1', N'Hồ Thùy Oanh', N'Kinh', N'Nữ', '0912345011', N'Hà Nội', '2017-11-11'),
('HS132', '3A1', N'Dương Minh Phúc', N'Kinh', N'Nam', '0912345012', N'Hà Nội', '2017-12-12'),
('HS133', '3A1', N'Mai Tú Quyên', N'Kinh', N'Nữ', '0912345013', N'Hà Nội', '2017-01-13'),
('HS134', '3A1', N'Phan Hoàng Quân', N'Kinh', N'Nam', '0912345014', N'Hà Nội', '2017-02-14'),
('HS135', '3A1', N'Lý Gia Hân', N'Kinh', N'Nữ', '0912345015', N'Hà Nội', '2017-03-15'),
('HS136', '3A1', N'Vương Minh Tâm', N'Kinh', N'Nam', '0912345016', N'Hà Nội', '2017-04-16'),
('HS137', '3A1', N'Tô Phương Uyên', N'Kinh', N'Nữ', '0912345017', N'Hà Nội', '2017-05-17'),
('HS138', '3A1', N'Trịnh Tuấn Vũ', N'Kinh', N'Nam', '0912345018', N'Hà Nội', '2017-06-18'),
('HS139', '3A1', N'Cao Hoàng Yến', N'Kinh', N'Nữ', '0912345019', N'Hà Nội', '2017-07-19'),
('HS140', '3A1', N'Giang Minh Triết', N'Kinh', N'Nam', '0912345020', N'Hà Nội', '2017-08-20');
-- Lớp 3A2
INSERT INTO HocSinh (MaHS, MaLop, HoTen, DanToc, GioiTinh, SDTPhuHuynh, DiaChi, NgaySinh) VALUES
('HS141', '3A2', N'Nguyễn Văn A', N'Kinh', N'Nam', '0922345001', N'Hải Phòng', '2017-01-01'),
('HS142', '3A2', N'Trần Thị B', N'Kinh', N'Nữ', '0922345002', N'Hải Phòng', '2017-02-02'),
('HS143', '3A2', N'Lê Văn C', N'Kinh', N'Nam', '0922345003', N'Hải Phòng', '2017-03-03'),
('HS144', '3A2', N'Phạm Thị D', N'Kinh', N'Nữ', '0922345004', N'Hải Phòng', '2017-04-04'),
('HS145', '3A2', N'Đỗ Văn E', N'Kinh', N'Nam', '0922345005', N'Hải Phòng', '2017-05-05'),
('HS146', '3A2', N'Vũ Thị F', N'Kinh', N'Nữ', '0922345006', N'Hải Phòng', '2017-06-06'),
('HS147', '3A2', N'Hoàng Văn G', N'Kinh', N'Nam', '0922345007', N'Hải Phòng', '2017-07-07'),
('HS148', '3A2', N'Bùi Thị H', N'Kinh', N'Nữ', '0922345008', N'Hải Phòng', '2017-08-08'),
('HS149', '3A2', N'Đặng Văn I', N'Kinh', N'Nam', '0922345009', N'Hải Phòng', '2017-09-09'),
('HS150', '3A2', N'Ngô Văn K', N'Kinh', N'Nam', '0922345010', N'Hải Phòng', '2017-10-10'),
('HS151', '3A2', N'Hồ Thị L', N'Kinh', N'Nữ', '0922345011', N'Hải Phòng', '2017-11-11'),
('HS152', '3A2', N'Dương Văn M', N'Kinh', N'Nam', '0922345012', N'Hải Phòng', '2017-12-12'),
('HS153', '3A2', N'Mai Thị N', N'Kinh', N'Nữ', '0922345013', N'Hải Phòng', '2017-01-13'),
('HS154', '3A2', N'Phan Văn P', N'Kinh', N'Nam', '0922345014', N'Hải Phòng', '2017-02-14'),
('HS155', '3A2', N'Lý Thị Q', N'Kinh', N'Nữ', '0922345015', N'Hải Phòng', '2017-03-15'),
('HS156', '3A2', N'Vương Văn R', N'Kinh', N'Nam', '0922345016', N'Hải Phòng', '2017-04-16'),
('HS157', '3A2', N'Tô Thị S', N'Kinh', N'Nữ', '0922345017', N'Hải Phòng', '2017-05-17'),
('HS158', '3A2', N'Trịnh Văn T', N'Kinh', N'Nam', '0922345018', N'Hải Phòng', '2017-06-18'),
('HS159', '3A2', N'Cao Thị U', N'Kinh', N'Nữ', '0922345019', N'Hải Phòng', '2017-07-19'),
('HS160', '3A2', N'Giang Văn V', N'Kinh', N'Nam', '0922345020', N'Hải Phòng', '2017-08-20');
-- Lớp 3A3
INSERT INTO HocSinh (MaHS, MaLop, HoTen, DanToc, GioiTinh, SDTPhuHuynh, DiaChi, NgaySinh) VALUES
('HS161', '3A3', N'Nguyễn Ánh Dương', N'Kinh', N'Nam', '0932345001', N'Đà Nẵng', '2017-01-01'),
('HS162', '3A3', N'Trần Ngọc Bích', N'Kinh', N'Nữ', '0932345002', N'Đà Nẵng', '2017-02-02'),
('HS163', '3A3', N'Lê Minh Châu', N'Kinh', N'Nam', '0932345003', N'Đà Nẵng', '2017-03-03'),
('HS164', '3A3', N'Phạm Hải Đăng', N'Kinh', N'Nam', '0932345004', N'Đà Nẵng', '2017-04-04'),
('HS165', '3A3', N'Đỗ Hà Giang', N'Kinh', N'Nữ', '0932345005', N'Đà Nẵng', '2017-05-05'),
('HS166', '3A3', N'Vũ Hoàng Hải', N'Kinh', N'Nam', '0932345006', N'Đà Nẵng', '2017-06-06'),
('HS167', '3A3', N'Hoàng Khánh Huyền', N'Kinh', N'Nữ', '0932345007', N'Đà Nẵng', '2017-07-07'),
('HS168', '3A3', N'Bùi Minh Khang', N'Kinh', N'Nam', '0932345008', N'Đà Nẵng', '2017-08-08'),
('HS169', '3A3', N'Đặng Tuệ Lâm', N'Kinh', N'Nữ', '0932345009', N'Đà Nẵng', '2017-09-09'),
('HS170', '3A3', N'Ngô Gia Long', N'Kinh', N'Nam', '0932345010', N'Đà Nẵng', '2017-10-10'),
('HS171', '3A3', N'Hồ Ngọc Mai', N'Kinh', N'Nữ', '0932345011', N'Đà Nẵng', '2017-11-11'),
('HS172', '3A3', N'Dương Quốc Phong', N'Kinh', N'Nam', '0932345012', N'Đà Nẵng', '2017-12-12'),
('HS173', '3A3', N'Mai Bảo Quyên', N'Kinh', N'Nữ', '0932345013', N'Đà Nẵng', '2017-01-13'),
('HS174', '3A3', N'Phan Minh Sơn', N'Kinh', N'Nam', '0932345014', N'Đà Nẵng', '2017-02-14'),
('HS175', '3A3', N'Lý Thảo Trang', N'Kinh', N'Nữ', '0932345015', N'Đà Nẵng', '2017-03-15'),
('HS176', '3A3', N'Vương Anh Tuấn', N'Kinh', N'Nam', '0932345016', N'Đà Nẵng', '2017-04-16'),
('HS177', '3A3', N'Tô Diệp Vy', N'Kinh', N'Nữ', '0932345017', N'Đà Nẵng', '2017-05-17'),
('HS178', '3A3', N'Trịnh Xuân Trường', N'Kinh', N'Nam', '0932345018', N'Đà Nẵng', '2017-06-18'),
('HS179', '3A3', N'Cao Thùy Anh', N'Kinh', N'Nữ', '0932345019', N'Đà Nẵng', '2017-07-19'),
('HS180', '3A3', N'Giang Tuấn Phong', N'Kinh', N'Nam', '0932345020', N'Đà Nẵng', '2017-08-20');
-- Lớp 4A1
INSERT INTO HocSinh (MaHS, MaLop, HoTen, DanToc, GioiTinh, SDTPhuHuynh, DiaChi, NgaySinh) VALUES
('HS181', '4A1', N'Nguyễn Hoàng An', N'Kinh', N'Nam', '0912345001', N'Hà Nội', '2016-01-01'),
('HS182', '4A1', N'Trần Bảo Bình', N'Kinh', N'Nữ', '0912345002', N'Hà Nội', '2016-02-02'),
('HS183', '4A1', N'Lê Gia Cát', N'Kinh', N'Nam', '0912345003', N'Hà Nội', '2016-03-03'),
('HS184', '4A1', N'Phạm Minh Dũng', N'Kinh', N'Nam', '0912345004', N'Hà Nội', '2016-04-04'),
('HS185', '4A1', N'Đỗ Phương Giang', N'Kinh', N'Nữ', '0912345005', N'Hà Nội', '2016-05-05'),
('HS186', '4A1', N'Vũ Gia Hân', N'Kinh', N'Nữ', '0912345006', N'Hà Nội', '2016-06-06'),
('HS187', '4A1', N'Hoàng Tuấn Kiệt', N'Kinh', N'Nam', '0912345007', N'Hà Nội', '2016-07-07'),
('HS188', '4A1', N'Bùi Khánh Linh', N'Kinh', N'Nữ', '0912345008', N'Hà Nội', '2016-08-08'),
('HS189', '4A1', N'Đặng Quốc Minh', N'Kinh', N'Nam', '0912345009', N'Hà Nội', '2016-09-09'),
('HS190', '4A1', N'Ngô Bảo Nam', N'Kinh', N'Nam', '0912345010', N'Hà Nội', '2016-10-10'),
('HS191', '4A1', N'Hồ Thùy Oanh', N'Kinh', N'Nữ', '0912345011', N'Hà Nội', '2016-11-11'),
('HS192', '4A1', N'Dương Minh Phúc', N'Kinh', N'Nam', '0912345012', N'Hà Nội', '2016-12-12'),
('HS193', '4A1', N'Mai Tú Quyên', N'Kinh', N'Nữ', '0912345013', N'Hà Nội', '2016-01-13'),
('HS194', '4A1', N'Phan Hoàng Quân', N'Kinh', N'Nam', '0912345014', N'Hà Nội', '2016-02-14'),
('HS195', '4A1', N'Lý Gia Hân', N'Kinh', N'Nữ', '0912345015', N'Hà Nội', '2016-03-15'),
('HS196', '4A1', N'Vương Minh Tâm', N'Kinh', N'Nam', '0912345016', N'Hà Nội', '2016-04-16'),
('HS197', '4A1', N'Tô Phương Uyên', N'Kinh', N'Nữ', '0912345017', N'Hà Nội', '2016-05-17'),
('HS198', '4A1', N'Trịnh Tuấn Vũ', N'Kinh', N'Nam', '0912345018', N'Hà Nội', '2016-06-18'),
('HS199', '4A1', N'Cao Hoàng Yến', N'Kinh', N'Nữ', '0912345019', N'Hà Nội', '2016-07-19'),
('HS200', '4A1', N'Giang Minh Triết', N'Kinh', N'Nam', '0912345020', N'Hà Nội', '2016-08-20');
-- Lớp 4A2
INSERT INTO HocSinh (MaHS, MaLop, HoTen, DanToc, GioiTinh, SDTPhuHuynh, DiaChi, NgaySinh) VALUES
('HS201', '4A2', N'Nguyễn Văn A', N'Kinh', N'Nam', '0922345001', N'Hải Phòng', '2016-01-01'),
('HS202', '4A2', N'Trần Thị B', N'Kinh', N'Nữ', '0922345002', N'Hải Phòng', '2016-02-02'),
('HS203', '4A2', N'Lê Văn C', N'Kinh', N'Nam', '0922345003', N'Hải Phòng', '2016-03-03'),
('HS204', '4A2', N'Phạm Thị D', N'Kinh', N'Nữ', '0922345004', N'Hải Phòng', '2016-04-04'),
('HS205', '4A2', N'Đỗ Văn E', N'Kinh', N'Nam', '0922345005', N'Hải Phòng', '2016-05-05'),
('HS206', '4A2', N'Vũ Thị F', N'Kinh', N'Nữ', '0922345006', N'Hải Phòng', '2016-06-06'),
('HS207', '4A2', N'Hoàng Văn G', N'Kinh', N'Nam', '0922345007', N'Hải Phòng', '2016-07-07'),
('HS208', '4A2', N'Bùi Thị H', N'Kinh', N'Nữ', '0922345008', N'Hải Phòng', '2016-08-08'),
('HS209', '4A2', N'Đặng Văn I', N'Kinh', N'Nam', '0922345009', N'Hải Phòng', '2016-09-09'),
('HS210', '4A2', N'Ngô Văn K', N'Kinh', N'Nam', '0922345010', N'Hải Phòng', '2016-10-10'),
('HS211', '4A2', N'Hồ Thị L', N'Kinh', N'Nữ', '0922345011', N'Hải Phòng', '2016-11-11'),
('HS212', '4A2', N'Dương Văn M', N'Kinh', N'Nam', '0922345012', N'Hải Phòng', '2016-12-12'),
('HS213', '4A2', N'Mai Thị N', N'Kinh', N'Nữ', '0922345013', N'Hải Phòng', '2016-01-13'),
('HS214', '4A2', N'Phan Văn P', N'Kinh', N'Nam', '0922345014', N'Hải Phòng', '2016-02-14'),
('HS215', '4A2', N'Lý Thị Q', N'Kinh', N'Nữ', '0922345015', N'Hải Phòng', '2016-03-15'),
('HS216', '4A2', N'Vương Văn R', N'Kinh', N'Nam', '0922345016', N'Hải Phòng', '2016-04-16'),
('HS217', '4A2', N'Tô Thị S', N'Kinh', N'Nữ', '0922345017', N'Hải Phòng', '2016-05-17'),
('HS218', '4A2', N'Trịnh Văn T', N'Kinh', N'Nam', '0922345018', N'Hải Phòng', '2016-06-18'),
('HS219', '4A2', N'Cao Thị U', N'Kinh', N'Nữ', '0922345019', N'Hải Phòng', '2016-07-19'),
('HS220', '4A2', N'Giang Văn V', N'Kinh', N'Nam', '0922345020', N'Hải Phòng', '2016-08-20');
-- Lớp 4A3
INSERT INTO HocSinh (MaHS, MaLop, HoTen, DanToc, GioiTinh, SDTPhuHuynh, DiaChi, NgaySinh) VALUES
('HS221', '4A3', N'Nguyễn Ánh Dương', N'Kinh', N'Nam', '0932345001', N'Đà Nẵng', '2016-01-01'),
('HS222', '4A3', N'Trần Ngọc Bích', N'Kinh', N'Nữ', '0932345002', N'Đà Nẵng', '2016-02-02'),
('HS223', '4A3', N'Lê Minh Châu', N'Kinh', N'Nam', '0932345003', N'Đà Nẵng', '2016-03-03'),
('HS224', '4A3', N'Phạm Hải Đăng', N'Kinh', N'Nam', '0932345004', N'Đà Nẵng', '2016-04-04'),
('HS225', '4A3', N'Đỗ Hà Giang', N'Kinh', N'Nữ', '0932345005', N'Đà Nẵng', '2016-05-05'),
('HS226', '4A3', N'Vũ Hoàng Hải', N'Kinh', N'Nam', '0932345006', N'Đà Nẵng', '2016-06-06'),
('HS227', '4A3', N'Hoàng Khánh Huyền', N'Kinh', N'Nữ', '0932345007', N'Đà Nẵng', '2016-07-07'),
('HS228', '4A3', N'Bùi Minh Khang', N'Kinh', N'Nam', '0932345008', N'Đà Nẵng', '2016-08-08'),
('HS229', '4A3', N'Đặng Tuệ Lâm', N'Kinh', N'Nữ', '0932345009', N'Đà Nẵng', '2016-09-09'),
('HS230', '4A3', N'Ngô Gia Long', N'Kinh', N'Nam', '0932345010', N'Đà Nẵng', '2016-10-10'),
('HS231', '4A3', N'Hồ Ngọc Mai', N'Kinh', N'Nữ', '0932345011', N'Đà Nẵng', '2016-11-11'),
('HS232', '4A3', N'Dương Quốc Phong', N'Kinh', N'Nam', '0932345012', N'Đà Nẵng', '2016-12-12'),
('HS233', '4A3', N'Mai Bảo Quyên', N'Kinh', N'Nữ', '0932345013', N'Đà Nẵng', '2016-01-13'),
('HS234', '4A3', N'Phan Minh Sơn', N'Kinh', N'Nam', '0932345014', N'Đà Nẵng', '2016-02-14'),
('HS235', '4A3', N'Lý Thảo Trang', N'Kinh', N'Nữ', '0932345015', N'Đà Nẵng', '2016-03-15'),
('HS236', '4A3', N'Vương Anh Tuấn', N'Kinh', N'Nam', '0932345016', N'Đà Nẵng', '2016-04-16'),
('HS237', '4A3', N'Tô Diệp Vy', N'Kinh', N'Nữ', '0932345017', N'Đà Nẵng', '2016-05-17'),
('HS238', '4A3', N'Trịnh Xuân Trường', N'Kinh', N'Nam', '0932345018', N'Đà Nẵng', '2016-06-18'),
('HS239', '4A3', N'Cao Thùy Anh', N'Kinh', N'Nữ', '0932345019', N'Đà Nẵng', '2016-07-19'),
('HS240', '4A3', N'Giang Tuấn Phong', N'Kinh', N'Nam', '0932345020', N'Đà Nẵng', '2016-08-20');
-- Lớp 5A1
INSERT INTO HocSinh (MaHS, MaLop, HoTen, DanToc, GioiTinh, SDTPhuHuynh, DiaChi, NgaySinh) VALUES
('HS241', '5A1', N'Nguyễn Hoàng An', N'Kinh', N'Nam', '0912345001', N'Hà Nội', '2015-01-01'),
('HS242', '5A1', N'Trần Bảo Bình', N'Kinh', N'Nữ', '0912345002', N'Hà Nội', '2015-02-02'),
('HS243', '5A1', N'Lê Gia Cát', N'Kinh', N'Nam', '0912345003', N'Hà Nội', '2015-03-03'),
('HS244', '5A1', N'Phạm Minh Dũng', N'Kinh', N'Nam', '0912345004', N'Hà Nội', '2015-04-04'),
('HS245', '5A1', N'Đỗ Phương Giang', N'Kinh', N'Nữ', '0912345005', N'Hà Nội', '2015-05-05'),
('HS246', '5A1', N'Vũ Gia Hân', N'Kinh', N'Nữ', '0912345006', N'Hà Nội', '2015-06-06'),
('HS247', '5A1', N'Hoàng Tuấn Kiệt', N'Kinh', N'Nam', '0912345007', N'Hà Nội', '2015-07-07'),
('HS248', '5A1', N'Bùi Khánh Linh', N'Kinh', N'Nữ', '0912345008', N'Hà Nội', '2015-08-08'),
('HS249', '5A1', N'Đặng Quốc Minh', N'Kinh', N'Nam', '0912345009', N'Hà Nội', '2015-09-09'),
('HS250', '5A1', N'Ngô Bảo Nam', N'Kinh', N'Nam', '0912345010', N'Hà Nội', '2015-10-10'),
('HS251', '5A1', N'Hồ Thùy Oanh', N'Kinh', N'Nữ', '0912345011', N'Hà Nội', '2015-11-11'),
('HS252', '5A1', N'Dương Minh Phúc', N'Kinh', N'Nam', '0912345012', N'Hà Nội', '2015-12-12'),
('HS253', '5A1', N'Mai Tú Quyên', N'Kinh', N'Nữ', '0912345013', N'Hà Nội', '2015-01-13'),
('HS254', '5A1', N'Phan Hoàng Quân', N'Kinh', N'Nam', '0912345014', N'Hà Nội', '2015-02-14'),
('HS255', '5A1', N'Lý Gia Hân', N'Kinh', N'Nữ', '0912345015', N'Hà Nội', '2015-03-15'),
('HS256', '5A1', N'Vương Minh Tâm', N'Kinh', N'Nam', '0912345016', N'Hà Nội', '2015-04-16'),
('HS257', '5A1', N'Tô Phương Uyên', N'Kinh', N'Nữ', '0912345017', N'Hà Nội', '2015-05-17'),
('HS258', '5A1', N'Trịnh Tuấn Vũ', N'Kinh', N'Nam', '0912345018', N'Hà Nội', '2015-06-18'),
('HS259', '5A1', N'Cao Hoàng Yến', N'Kinh', N'Nữ', '0912345019', N'Hà Nội', '2015-07-19'),
('HS260', '5A1', N'Giang Minh Triết', N'Kinh', N'Nam', '0912345020', N'Hà Nội', '2015-08-20');
-- Lớp 5A2
INSERT INTO HocSinh (MaHS, MaLop, HoTen, DanToc, GioiTinh, SDTPhuHuynh, DiaChi, NgaySinh) VALUES
('HS261', '5A2', N'Nguyễn Văn A', N'Kinh', N'Nam', '0922345001', N'Hải Phòng', '2015-01-01'),
('HS262', '5A2', N'Trần Thị B', N'Kinh', N'Nữ', '0922345002', N'Hải Phòng', '2015-02-02'),
('HS263', '5A2', N'Lê Văn C', N'Kinh', N'Nam', '0922345003', N'Hải Phòng', '2015-03-03'),
('HS264', '5A2', N'Phạm Thị D', N'Kinh', N'Nữ', '0922345004', N'Hải Phòng', '2015-04-04'),
('HS265', '5A2', N'Đỗ Văn E', N'Kinh', N'Nam', '0922345005', N'Hải Phòng', '2015-05-05'),
('HS266', '5A2', N'Vũ Thị F', N'Kinh', N'Nữ', '0922345006', N'Hải Phòng', '2015-06-06'),
('HS267', '5A2', N'Hoàng Văn G', N'Kinh', N'Nam', '0922345007', N'Hải Phòng', '2015-07-07'),
('HS268', '5A2', N'Bùi Thị H', N'Kinh', N'Nữ', '0922345008', N'Hải Phòng', '2015-08-08'),
('HS269', '5A2', N'Đặng Văn I', N'Kinh', N'Nam', '0922345009', N'Hải Phòng', '2015-09-09'),
('HS270', '5A2', N'Ngô Văn K', N'Kinh', N'Nam', '0922345010', N'Hải Phòng', '2015-10-10'),
('HS271', '5A2', N'Hồ Thị L', N'Kinh', N'Nữ', '0922345011', N'Hải Phòng', '2015-11-11'),
('HS272', '5A2', N'Dương Văn M', N'Kinh', N'Nam', '0922345012', N'Hải Phòng', '2015-12-12'),
('HS273', '5A2', N'Mai Thị N', N'Kinh', N'Nữ', '0922345013', N'Hải Phòng', '2015-01-13'),
('HS274', '5A2', N'Phan Văn P', N'Kinh', N'Nam', '0922345014', N'Hải Phòng', '2015-02-14'),
('HS275', '5A2', N'Lý Thị Q', N'Kinh', N'Nữ', '0922345015', N'Hải Phòng', '2015-03-15'),
('HS276', '5A2', N'Vương Văn R', N'Kinh', N'Nam', '0922345016', N'Hải Phòng', '2015-04-16'),
('HS277', '5A2', N'Tô Thị S', N'Kinh', N'Nữ', '0922345017', N'Hải Phòng', '2015-05-17'),
('HS278', '5A2', N'Trịnh Văn T', N'Kinh', N'Nam', '0922345018', N'Hải Phòng', '2015-06-18'),
('HS279', '5A2', N'Cao Thị U', N'Kinh', N'Nữ', '0922345019', N'Hải Phòng', '2015-07-19'),
('HS280', '5A2', N'Giang Văn V', N'Kinh', N'Nam', '0922345020', N'Hải Phòng', '2015-08-20');
-- Lớp 5A3
INSERT INTO HocSinh (MaHS, MaLop, HoTen, DanToc, GioiTinh, SDTPhuHuynh, DiaChi, NgaySinh) VALUES
('HS281', '5A3', N'Nguyễn Ánh Dương', N'Kinh', N'Nam', '0932345001', N'Đà Nẵng', '2015-01-01'),
('HS282', '5A3', N'Trần Ngọc Bích', N'Kinh', N'Nữ', '0932345002', N'Đà Nẵng', '2015-02-02'),
('HS283', '5A3', N'Lê Minh Châu', N'Kinh', N'Nam', '0932345003', N'Đà Nẵng', '2015-03-03'),
('HS284', '5A3', N'Phạm Hải Đăng', N'Kinh', N'Nam', '0932345004', N'Đà Nẵng', '2015-04-04'),
('HS285', '5A3', N'Đỗ Hà Giang', N'Kinh', N'Nữ', '0932345005', N'Đà Nẵng', '2015-05-05'),
('HS286', '5A3', N'Vũ Hoàng Hải', N'Kinh', N'Nam', '0932345006', N'Đà Nẵng', '2015-06-06'),
('HS287', '5A3', N'Hoàng Khánh Huyền', N'Kinh', N'Nữ', '0932345007', N'Đà Nẵng', '2015-07-07'),
('HS288', '5A3', N'Bùi Minh Khang', N'Kinh', N'Nam', '0932345008', N'Đà Nẵng', '2015-08-08'),
('HS289', '5A3', N'Đặng Tuệ Lâm', N'Kinh', N'Nữ', '0932345009', N'Đà Nẵng', '2015-09-09'),
('HS290', '5A3', N'Ngô Gia Long', N'Kinh', N'Nam', '0932345010', N'Đà Nẵng', '2015-10-10'),
('HS291', '5A3', N'Hồ Ngọc Mai', N'Kinh', N'Nữ', '0932345011', N'Đà Nẵng', '2015-11-11'),
('HS292', '5A3', N'Dương Quốc Phong', N'Kinh', N'Nam', '0932345012', N'Đà Nẵng', '2015-12-12'),
('HS293', '5A3', N'Mai Bảo Quyên', N'Kinh', N'Nữ', '0932345013', N'Đà Nẵng', '2015-01-13'),
('HS294', '5A3', N'Phan Minh Sơn', N'Kinh', N'Nam', '0932345014', N'Đà Nẵng', '2015-02-14'),
('HS295', '5A3', N'Lý Thảo Trang', N'Kinh', N'Nữ', '0932345015', N'Đà Nẵng', '2015-03-15'),
('HS296', '5A3', N'Vương Anh Tuấn', N'Kinh', N'Nam', '0932345016', N'Đà Nẵng', '2015-04-16'),
('HS297', '5A3', N'Tô Diệp Vy', N'Kinh', N'Nữ', '0932345017', N'Đà Nẵng', '2015-05-17'),
('HS298', '5A3', N'Trịnh Xuân Trường', N'Kinh', N'Nam', '0932345018', N'Đà Nẵng', '2015-06-18'),
('HS299', '5A3', N'Cao Thùy Anh', N'Kinh', N'Nữ', '0932345019', N'Đà Nẵng', '2015-07-19'),
('HS300', '5A3', N'Giang Tuấn Phong', N'Kinh', N'Nam', '0932345020', N'Đà Nẵng', '2015-08-20');
GO
PRINT N'Cập nhật điểm cho HS241 (Nguyễn Hoàng An)...';
-- Môn TV (Tiếng Việt)
UPDATE KetQuaHocTap SET Diem = 7.0 WHERE MaHS = 'HS241' AND MaMon = 'TV' AND Loai = 'GiuaKi1';
UPDATE KetQuaHocTap SET Diem = 8.0 WHERE MaHS = 'HS241' AND MaMon = 'TV' AND Loai = 'CuoiKi1';
-- Môn TOAN (Toán)
UPDATE KetQuaHocTap SET Diem = 8.5 WHERE MaHS = 'HS241' AND MaMon = 'TOAN' AND Loai = 'GiuaKi1';
UPDATE KetQuaHocTap SET Diem = 9.0 WHERE MaHS = 'HS241' AND MaMon = 'TOAN' AND Loai = 'CuoiKi1';
-- Môn ANH (Tiếng Anh)
UPDATE KetQuaHocTap SET Diem = 8.0 WHERE MaHS = 'HS241' AND MaMon = 'ANH' AND Loai = 'GiuaKi1';
UPDATE KetQuaHocTap SET Diem = 9.0 WHERE MaHS = 'HS241' AND MaMon = 'ANH' AND Loai = 'CuoiKi1';
-- Môn KH (Khoa học)
UPDATE KetQuaHocTap SET Diem = 7.5 WHERE MaHS = 'HS241' AND MaMon = 'KH' AND Loai = 'GiuaKi1';
UPDATE KetQuaHocTap SET Diem = 8.5 WHERE MaHS = 'HS241' AND MaMon = 'KH' AND Loai = 'CuoiKi1';
-- Môn LS_DL (Lịch sử và Địa lí)
UPDATE KetQuaHocTap SET Diem = 8.0 WHERE MaHS = 'HS241' AND MaMon = 'LS_DL' AND Loai = 'GiuaKi1';
UPDATE KetQuaHocTap SET Diem = 8.0 WHERE MaHS = 'HS241' AND MaMon = 'LS_DL' AND Loai = 'CuoiKi1';
-- Môn TIN (Tin học)
UPDATE KetQuaHocTap SET Diem = 9.0 WHERE MaHS = 'HS241' AND MaMon = 'TIN' AND Loai = 'GiuaKi1';
UPDATE KetQuaHocTap SET Diem = 9.5 WHERE MaHS = 'HS241' AND MaMon = 'TIN' AND Loai = 'CuoiKi1';
-- Môn GDTC (Giáo dục thể chất)
UPDATE KetQuaHocTap SET Diem = 10  WHERE MaHS = 'HS241' AND MaMon = 'GDTC' AND Loai = 'GiuaKi1';
UPDATE KetQuaHocTap SET Diem = 10  WHERE MaHS = 'HS241' AND MaMon = 'GDTC' AND Loai = 'CuoiKi1';
-- Môn AN (Âm nhạc)
UPDATE KetQuaHocTap SET Diem = 9.0 WHERE MaHS = 'HS241' AND MaMon = 'AN' AND Loai = 'GiuaKi1';
UPDATE KetQuaHocTap SET Diem = 9.0 WHERE MaHS = 'HS241' AND MaMon = 'AN' AND Loai = 'CuoiKi1';
-- Môn MT (Mĩ thuật)
UPDATE KetQuaHocTap SET Diem = 9.0 WHERE MaHS = 'HS241' AND MaMon = 'MT' AND Loai = 'GiuaKi1';
UPDATE KetQuaHocTap SET Diem = 9.5 WHERE MaHS = 'HS241' AND MaMon = 'MT' AND Loai = 'CuoiKi1';
-- Môn CN (Công nghệ)
UPDATE KetQuaHocTap SET Diem = 8.0 WHERE MaHS = 'HS241' AND MaMon = 'CN' AND Loai = 'GiuaKi1';
UPDATE KetQuaHocTap SET Diem = 8.5 WHERE MaHS = 'HS241' AND MaMon = 'CN' AND Loai = 'CuoiKi1';
-- Môn DD (Đạo đức)
UPDATE KetQuaHocTap SET Diem = 10  WHERE MaHS = 'HS241' AND MaMon = 'DD' AND Loai = 'GiuaKi1';
UPDATE KetQuaHocTap SET Diem = 10  WHERE MaHS = 'HS241' AND MaMon = 'DD' AND Loai = 'CuoiKi1';
-- Môn HDTN (Hoạt động trải nghiệm)
UPDATE KetQuaHocTap SET Diem = 9.0 WHERE MaHS = 'HS241' AND MaMon = 'HDTN' AND Loai = 'GiuaKi1';
UPDATE KetQuaHocTap SET Diem = 9.0 WHERE MaHS = 'HS241' AND MaMon = 'HDTN' AND Loai = 'CuoiKi1';

-- --- Học Kỳ 2 (HS241) ---
UPDATE KetQuaHocTap SET Diem = 8.5 WHERE MaHS = 'HS241' AND MaMon = 'TV' AND Loai = 'GiuaKi2';
UPDATE KetQuaHocTap SET Diem = 8.0 WHERE MaHS = 'HS241' AND MaMon = 'TV' AND Loai = 'CuoiKi2';
UPDATE KetQuaHocTap SET Diem = 9.0 WHERE MaHS = 'HS241' AND MaMon = 'TOAN' AND Loai = 'GiuaKi2';
UPDATE KetQuaHocTap SET Diem = 9.5 WHERE MaHS = 'HS241' AND MaMon = 'TOAN' AND Loai = 'CuoiKi2';
UPDATE KetQuaHocTap SET Diem = 9.0 WHERE MaHS = 'HS241' AND MaMon = 'ANH' AND Loai = 'GiuaKi2';
UPDATE KetQuaHocTap SET Diem = 8.5 WHERE MaHS = 'HS241' AND MaMon = 'ANH' AND Loai = 'CuoiKi2';
UPDATE KetQuaHocTap SET Diem = 8.5 WHERE MaHS = 'HS241' AND MaMon = 'KH' AND Loai = 'GiuaKi2';
UPDATE KetQuaHocTap SET Diem = 9.0 WHERE MaHS = 'HS241' AND MaMon = 'KH' AND Loai = 'CuoiKi2';
UPDATE KetQuaHocTap SET Diem = 8.0 WHERE MaHS = 'HS241' AND MaMon = 'LS_DL' AND Loai = 'GiuaKi2';
UPDATE KetQuaHocTap SET Diem = 9.0 WHERE MaHS = 'HS241' AND MaMon = 'LS_DL' AND Loai = 'CuoiKi2';
UPDATE KetQuaHocTap SET Diem = 9.5 WHERE MaHS = 'HS241' AND MaMon = 'TIN' AND Loai = 'GiuaKi2';
UPDATE KetQuaHocTap SET Diem = 10  WHERE MaHS = 'HS241' AND MaMon = 'TIN' AND Loai = 'CuoiKi2';
UPDATE KetQuaHocTap SET Diem = 10  WHERE MaHS = 'HS241' AND MaMon = 'GDTC' AND Loai = 'GiuaKi2';
UPDATE KetQuaHocTap SET Diem = 10  WHERE MaHS = 'HS241' AND MaMon = 'GDTC' AND Loai = 'CuoiKi2';
UPDATE KetQuaHocTap SET Diem = 9.0 WHERE MaHS = 'HS241' AND MaMon = 'AN' AND Loai = 'GiuaKi2';
UPDATE KetQuaHocTap SET Diem = 9.5 WHERE MaHS = 'HS241' AND MaMon = 'AN' AND Loai = 'CuoiKi2';
UPDATE KetQuaHocTap SET Diem = 9.5 WHERE MaHS = 'HS241' AND MaMon = 'MT' AND Loai = 'GiuaKi2';
UPDATE KetQuaHocTap SET Diem = 9.5 WHERE MaHS = 'HS241' AND MaMon = 'MT' AND Loai = 'CuoiKi2';
UPDATE KetQuaHocTap SET Diem = 8.5 WHERE MaHS = 'HS241' AND MaMon = 'CN' AND Loai = 'GiuaKi2';
UPDATE KetQuaHocTap SET Diem = 9.0 WHERE MaHS = 'HS241' AND MaMon = 'CN' AND Loai = 'CuoiKi2';
UPDATE KetQuaHocTap SET Diem = 10  WHERE MaHS = 'HS241' AND MaMon = 'DD' AND Loai = 'GiuaKi2';
UPDATE KetQuaHocTap SET Diem = 10  WHERE MaHS = 'HS241' AND MaMon = 'DD' AND Loai = 'CuoiKi2';
UPDATE KetQuaHocTap SET Diem = 9.0 WHERE MaHS = 'HS241' AND MaMon = 'HDTN' AND Loai = 'GiuaKi2';
UPDATE KetQuaHocTap SET Diem = 10  WHERE MaHS = 'HS241' AND MaMon = 'HDTN' AND Loai = 'CuoiKi2';
GO

-- =================================================================
-- HỌC SINH 2: Trần Bảo Bình (HS242) - LỚP 5A1
-- =================================================================

-- --- Học Kỳ 1 (HS242) ---
PRINT N'Cập nhật điểm cho HS242 (Trần Bảo Bình)...';
-- Môn TV (Tiếng Việt)
UPDATE KetQuaHocTap SET Diem = 8.0 WHERE MaHS = 'HS242' AND MaMon = 'TV' AND Loai = 'GiuaKi1';
UPDATE KetQuaHocTap SET Diem = 7.5 WHERE MaHS = 'HS242' AND MaMon = 'TV' AND Loai = 'CuoiKi1';
-- Môn TOAN (Toán)
UPDATE KetQuaHocTap SET Diem = 9.0 WHERE MaHS = 'HS242' AND MaMon = 'TOAN' AND Loai = 'GiuaKi1';
UPDATE KetQuaHocTap SET Diem = 9.5 WHERE MaHS = 'HS242' AND MaMon = 'TOAN' AND Loai = 'CuoiKi1';
-- Môn ANH (Tiếng Anh)
UPDATE KetQuaHocTap SET Diem = 8.5 WHERE MaHS = 'HS242' AND MaMon = 'ANH' AND Loai = 'GiuaKi1';
UPDATE KetQuaHocTap SET Diem = 8.0 WHERE MaHS = 'HS242' AND MaMon = 'ANH' AND Loai = 'CuoiKi1';
-- Môn KH (Khoa học)
UPDATE KetQuaHocTap SET Diem = 8.5 WHERE MaHS = 'HS242' AND MaMon = 'KH' AND Loai = 'GiuaKi1';
UPDATE KetQuaHocTap SET Diem = 9.0 WHERE MaHS = 'HS242' AND MaMon = 'KH' AND Loai = 'CuoiKi1';
-- Môn LS_DL (Lịch sử và Địa lí)
UPDATE KetQuaHocTap SET Diem = 7.0 WHERE MaHS = 'HS242' AND MaMon = 'LS_DL' AND Loai = 'GiuaKi1';
UPDATE KetQuaHocTap SET Diem = 8.5 WHERE MaHS = 'HS242' AND MaMon = 'LS_DL' AND Loai = 'CuoiKi1';
-- Môn TIN (Tin học)
UPDATE KetQuaHocTap SET Diem = 10  WHERE MaHS = 'HS242' AND MaMon = 'TIN' AND Loai = 'GiuaKi1';
UPDATE KetQuaHocTap SET Diem = 10  WHERE MaHS = 'HS242' AND MaMon = 'TIN' AND Loai = 'CuoiKi1';
-- Môn GDTC (Giáo dục thể chất)
UPDATE KetQuaHocTap SET Diem = 9.0 WHERE MaHS = 'HS242' AND MaMon = 'GDTC' AND Loai = 'GiuaKi1';
UPDATE KetQuaHocTap SET Diem = 10  WHERE MaHS = 'HS242' AND MaMon = 'GDTC' AND Loai = 'CuoiKi1';
-- Môn AN (Âm nhạc)
UPDATE KetQuaHocTap SET Diem = 9.5 WHERE MaHS = 'HS242' AND MaMon = 'AN' AND Loai = 'GiuaKi1';
UPDATE KetQuaHocTap SET Diem = 9.0 WHERE MaHS = 'HS242' AND MaMon = 'AN' AND Loai = 'CuoiKi1';
-- Môn MT (Mĩ thuật)
UPDATE KetQuaHocTap SET Diem = 8.5 WHERE MaHS = 'HS242' AND MaMon = 'MT' AND Loai = 'GiuaKi1';
UPDATE KetQuaHocTap SET Diem = 9.0 WHERE MaHS = 'HS242' AND MaMon = 'MT' AND Loai = 'CuoiKi1';
-- Môn CN (Công nghệ)
UPDATE KetQuaHocTap SET Diem = 8.0 WHERE MaHS = 'HS242' AND MaMon = 'CN' AND Loai = 'GiuaKi1';
UPDATE KetQuaHocTap SET Diem = 9.0 WHERE MaHS = 'HS242' AND MaMon = 'CN' AND Loai = 'CuoiKi1';
-- Môn DD (Đạo đức)
UPDATE KetQuaHocTap SET Diem = 10  WHERE MaHS = 'HS242' AND MaMon = 'DD' AND Loai = 'GiuaKi1';
UPDATE KetQuaHocTap SET Diem = 9.0 WHERE MaHS = 'HS242' AND MaMon = 'DD' AND Loai = 'CuoiKi1';
-- Môn HDTN (Hoạt động trải nghiệm)
UPDATE KetQuaHocTap SET Diem = 9.0 WHERE MaHS = 'HS242' AND MaMon = 'HDTN' AND Loai = 'GiuaKi1';
UPDATE KetQuaHocTap SET Diem = 10  WHERE MaHS = 'HS242' AND MaMon = 'HDTN' AND Loai = 'CuoiKi1';

-- --- Học Kỳ 2 (HS242) ---
UPDATE KetQuaHocTap SET Diem = 8.5 WHERE MaHS = 'HS242' AND MaMon = 'TV' AND Loai = 'GiuaKi2';
UPDATE KetQuaHocTap SET Diem = 8.5 WHERE MaHS = 'HS242' AND MaMon = 'TV' AND Loai = 'CuoiKi2';
UPDATE KetQuaHocTap SET Diem = 9.5 WHERE MaHS = 'HS242' AND MaMon = 'TOAN' AND Loai = 'GiuaKi2';
UPDATE KetQuaHocTap SET Diem = 10  WHERE MaHS = 'HS242' AND MaMon = 'TOAN' AND Loai = 'CuoiKi2';
UPDATE KetQuaHocTap SET Diem = 8.0 WHERE MaHS = 'HS242' AND MaMon = 'ANH' AND Loai = 'GiuaKi2';
UPDATE KetQuaHocTap SET Diem = 9.0 WHERE MaHS = 'HS242' AND MaMon = 'ANH' AND Loai = 'CuoiKi2';
UPDATE KetQuaHocTap SET Diem = 9.0 WHERE MaHS = 'HS242' AND MaMon = 'KH' AND Loai = 'GiuaKi2';
UPDATE KetQuaHocTap SET Diem = 9.0 WHERE MaHS = 'HS242' AND MaMon = 'KH' AND Loai = 'CuoiKi2';
UPDATE KetQuaHocTap SET Diem = 8.5 WHERE MaHS = 'HS242' AND MaMon = 'LS_DL' AND Loai = 'GiuaKi2';
UPDATE KetQuaHocTap SET Diem = 8.0 WHERE MaHS = 'HS242' AND MaMon = 'LS_DL' AND Loai = 'CuoiKi2';
UPDATE KetQuaHocTap SET Diem = 10  WHERE MaHS = 'HS242' AND MaMon = 'TIN' AND Loai = 'GiuaKi2';
UPDATE KetQuaHocTap SET Diem = 10  WHERE MaHS = 'HS242' AND MaMon = 'TIN' AND Loai = 'CuoiKi2';
UPDATE KetQuaHocTap SET Diem = 10  WHERE MaHS = 'HS242' AND MaMon = 'GDTC' AND Loai = 'GiuaKi2';
UPDATE KetQuaHocTap SET Diem = 10  WHERE MaHS = 'HS242' AND MaMon = 'GDTC' AND Loai = 'CuoiKi2';
UPDATE KetQuaHocTap SET Diem = 9.0 WHERE MaHS = 'HS242' AND MaMon = 'AN' AND Loai = 'GiuaKi2';
UPDATE KetQuaHocTap SET Diem = 9.5 WHERE MaHS = 'HS242' AND MaMon = 'AN' AND Loai = 'CuoiKi2';
UPDATE KetQuaHocTap SET Diem = 9.0 WHERE MaHS = 'HS242' AND MaMon = 'MT' AND Loai = 'GiuaKi2';
UPDATE KetQuaHocTap SET Diem = 9.0 WHERE MaHS = 'HS242' AND MaMon = 'MT' AND Loai = 'CuoiKi2';
UPDATE KetQuaHocTap SET Diem = 9.0 WHERE MaHS = 'HS242' AND MaMon = 'CN' AND Loai = 'GiuaKi2';
UPDATE KetQuaHocTap SET Diem = 8.5 WHERE MaHS = 'HS242' AND MaMon = 'CN' AND Loai = 'CuoiKi2';
UPDATE KetQuaHocTap SET Diem = 10  WHERE MaHS = 'HS242' AND MaMon = 'DD' AND Loai = 'GiuaKi2';
UPDATE KetQuaHocTap SET Diem = 10  WHERE MaHS = 'HS242' AND MaMon = 'DD' AND Loai = 'CuoiKi2';
UPDATE KetQuaHocTap SET Diem = 10  WHERE MaHS = 'HS242' AND MaMon = 'HDTN' AND Loai = 'GiuaKi2';
UPDATE KetQuaHocTap SET Diem = 9.5 WHERE MaHS = 'HS242' AND MaMon = 'HDTN' AND Loai = 'CuoiKi2';
GO

-- =================================================================
-- HỌC SINH 3: Lê Gia Cát (HS243) - LỚP 5A1 (THÊM MỘT SỐ ĐIỂM HỌC KỲ 1)
-- =================================================================
PRINT N'Cập nhật điểm cho HS243 (Lê Gia Cát)...';
UPDATE KetQuaHocTap SET Diem = 7.5 WHERE MaHS = 'HS243' AND MaMon = 'TV' AND Loai = 'GiuaKi1';
UPDATE KetQuaHocTap SET Diem = 8.0 WHERE MaHS = 'HS243' AND MaMon = 'TV' AND Loai = 'CuoiKi1';
UPDATE KetQuaHocTap SET Diem = 8.0 WHERE MaHS = 'HS243' AND MaMon = 'TOAN' AND Loai = 'GiuaKi1';
UPDATE KetQuaHocTap SET Diem = 7.0 WHERE MaHS = 'HS243' AND MaMon = 'TOAN' AND Loai = 'CuoiKi1';
UPDATE KetQuaHocTap SET Diem = 6.5 WHERE MaHS = 'HS243' AND MaMon = 'ANH' AND Loai = 'GiuaKi1';
UPDATE KetQuaHocTap SET Diem = 7.5 WHERE MaHS = 'HS243' AND MaMon = 'ANH' AND Loai = 'CuoiKi1';
UPDATE KetQuaHocTap SET Diem = 8.0 WHERE MaHS = 'HS243' AND MaMon = 'KH' AND Loai = 'GiuaKi1';
UPDATE KetQuaHocTap SET Diem = 8.0 WHERE MaHS = 'HS243' AND MaMon = 'KH' AND Loai = 'CuoiKi1';
UPDATE KetQuaHocTap SET Diem = 9.0 WHERE MaHS = 'HS243' AND MaMon = 'TIN' AND Loai = 'GiuaKi1';
UPDATE KetQuaHocTap SET Diem = 9.5 WHERE MaHS = 'HS243' AND MaMon = 'TIN' AND Loai = 'CuoiKi1';
UPDATE KetQuaHocTap SET Diem = 10  WHERE MaHS = 'HS243' AND MaMon = 'DD' AND Loai = 'GiuaKi1';
UPDATE KetQuaHocTap SET Diem = 9.0 WHERE MaHS = 'HS243' AND MaMon = 'DD' AND Loai = 'CuoiKi1';
GO
UPDATE KetQuaHocTap SET Diem = 10
WHERE MaHS = 'HS241' AND MaMon = 'TOAN' AND Loai = 'Thang1_Ki1';

-- Em 2 (Trần Bảo Bình - HS242): 9.5 điểm, Nữ, Kinh
UPDATE KetQuaHocTap SET Diem = 9.5
WHERE MaHS = 'HS242' AND MaMon = 'TOAN' AND Loai = 'Thang1_Ki1';

-- Em 3 (Lê Gia Cát - HS243): 8.0 điểm, Nam, Kinh
UPDATE KetQuaHocTap SET Diem = 8.0
WHERE MaHS = 'HS243' AND MaMon = 'TOAN' AND Loai = 'Thang1_Ki1';

-- Em 4 (Phạm Minh Dũng - HS244): 7.5 điểm, Nam, Kinh
UPDATE KetQuaHocTap SET Diem = 7.5
WHERE MaHS = 'HS244' AND MaMon = 'TOAN' AND Loai = 'Thang1_Ki1';

-- Em 5 (Đỗ Phương Giang - HS245): 6.0 điểm, Nữ, Kinh
UPDATE KetQuaHocTap SET Diem = 6.0
WHERE MaHS = 'HS245' AND MaMon = 'TOAN' AND Loai = 'Thang1_Ki1';

-- Em 6 (Vũ Gia Hân - HS246): 5.0 điểm, Nữ, Kinh
UPDATE KetQuaHocTap SET Diem = 5.0
WHERE MaHS = 'HS246' AND MaMon = 'TOAN' AND Loai = 'Thang1_Ki1';

-- Em 7 (Hoàng Tuấn Kiệt - HS247): 4.0 điểm, Nam, Kinh
UPDATE KetQuaHocTap SET Diem = 4.0
WHERE MaHS = 'HS247' AND MaMon = 'TOAN' AND Loai = 'Thang1_Ki1';

-- Em 8 (Bùi Khánh Linh - HS248): 9.0 điểm, Nữ, Dân tộc thiểu số (để test)
UPDATE HocSinh SET DanToc = N'Tày' WHERE MaHS = 'HS248';
UPDATE KetQuaHocTap SET Diem = 9.0
WHERE MaHS = 'HS248' AND MaMon = 'TOAN' AND Loai = 'Thang1_Ki1';
Go
-- 9. Thời Khóa Biểu
INSERT INTO ThoiKhoaBieu (MaTKB, Ngay, Tiet, MaMon, GhiChu, MaGV, MaLop) VALUES
('TKB001', '2025-10-06', 1, 'TV', N'Ôn tập chương 1', 'GV007', '5A1'),
('TKB002', '2025-10-06', 2, 'TOAN', N'Luyện tập cộng trừ phân số', 'GV014', '5A1'),
('TKB003', '2025-10-07', 1, 'ANH', N'Học từ vựng chủ đề gia đình', 'GV003', '5A1'),
('TKB004', '2025-10-07', 2, 'TOAN', N'Bài tập ứng dụng thực tế', 'GV014', '5A1'),
('TKB005', '2025-10-08', 3, 'TIN', N'Luyện gõ 10 ngón', 'GV004', '5A1'),
('TKB006', '2025-10-06', 1, 'TV', N'Học vần', 'GV001', '1A1'),
('TKB007', '2025-10-06', 2, 'TOAN', N'Học số đếm 1-10', 'GV002', '1A1');

-- 10. Minigame
INSERT INTO Minigame (MaMNG, Ten, DuLieu) VALUES
('MNG01', N'Quiz nhanh', NULL), ('MNG02', N'Gọi tên ngẫu nhiên', NULL), ('MNG03', N'Flashcard', NULL),
('MNG04', N'Ghép chữ', NULL), ('MNG05', N'Nghe - chọn hình', NULL), ('MNG06', N'Sắp xếp câu', NULL),
('MNG07', N'Điền từ', NULL), ('MNG08', N'Lật thẻ', NULL), ('MNG09', N'Random số', NULL), ('MNG10', N'Pass a ball', NULL);

-- 11. DỮ LIỆU MẪU MỚI: ThoiHanDiem
-- Khối 1
INSERT INTO ThoiHanDiem (MaCotDiem, TenHienThi, Khoi, HocKy, NgayMoDiem, NgayKhoaDiem, KhoaThuCong)
VALUES
('Thang1_Ki1', N'Điểm Tháng 1 (Kỳ 1)', N'Khối 1', 1, '2024-09-01', '2024-09-30', 0),
('Thang2_Ki1', N'Điểm Tháng 2 (Kỳ 1)', N'Khối 1', 1, '2024-10-01', '2024-10-31', 0),
('Thang3_Ki1', N'Điểm Tháng 3 (Kỳ 1)', N'Khối 1', 1, '2024-11-01', '2024-11-30', 0),
('GiuaKi1', N'Điểm Giữa Kỳ 1', N'Khối 1', 1, '2024-11-05', '2024-11-20', 0),
('CuoiKi1', N'Điểm Cuối Kỳ 1', N'Khối 1', 1, '2024-12-15', '2025-01-15', 0),
('Thang1_Ki2', N'Điểm Tháng 1 (Kỳ 2)', N'Khối 1', 2, '2025-01-20', '2025-02-28', 0),
('Thang2_Ki2', N'Điểm Tháng 2 (Kỳ 2)', N'Khối 1', 2, '2025-03-01', '2025-03-31', 0),
('Thang3_Ki2', N'Điểm Tháng 3 (Kỳ 2)', N'Khối 1', 2, '2025-04-01', '2025-04-30', 0),
('GiuaKi2', N'Điểm Giữa Kỳ 2', N'Khối 1', 2, '2025-04-05', '2025-04-20', 0),
('CuoiKi2', N'Điểm Cuối Kỳ 2', N'Khối 1', 2, '2025-05-15', '2025-06-15', 0);
-- Khối 2
INSERT INTO ThoiHanDiem (MaCotDiem, TenHienThi, Khoi, HocKy, NgayMoDiem, NgayKhoaDiem, KhoaThuCong)
VALUES
('Thang1_Ki1', N'Điểm Tháng 1 (Kỳ 1)', N'Khối 2', 1, '2024-09-01', '2024-09-30', 0),
('Thang2_Ki1', N'Điểm Tháng 2 (Kỳ 1)', N'Khối 2', 1, '2024-10-01', '2024-10-31', 0),
('Thang3_Ki1', N'Điểm Tháng 3 (Kỳ 1)', N'Khối 2', 1, '2024-11-01', '2024-11-30', 0),
('GiuaKi1', N'Điểm Giữa Kỳ 1', N'Khối 2', 1, '2024-11-05', '2024-11-20', 0),
('CuoiKi1', N'Điểm Cuối Kỳ 1', N'Khối 2', 1, '2024-12-15', '2025-01-15', 0),
('Thang1_Ki2', N'Điểm Tháng 1 (Kỳ 2)', N'Khối 2', 2, '2025-01-20', '2025-02-28', 0),
('Thang2_Ki2', N'Điểm Tháng 2 (Kỳ 2)', N'Khối 2', 2, '2025-03-01', '2025-03-31', 0),
('Thang3_Ki2', N'Điểm Tháng 3 (Kỳ 2)', N'Khối 2', 2, '2025-04-01', '2025-04-30', 0),
('GiuaKi2', N'Điểm Giữa Kỳ 2', N'Khối 2', 2, '2025-04-05', '2025-04-20', 0),
('CuoiKi2', N'Điểm Cuối Kỳ 2', N'Khối 2', 2, '2025-05-15', '2025-06-15', 0);
-- Khối 3
INSERT INTO ThoiHanDiem (MaCotDiem, TenHienThi, Khoi, HocKy, NgayMoDiem, NgayKhoaDiem, KhoaThuCong)
VALUES
('Thang1_Ki1', N'Điểm Tháng 1 (Kỳ 1)', N'Khối 3', 1, '2024-09-01', '2024-09-30', 0),
('Thang2_Ki1', N'Điểm Tháng 2 (Kỳ 1)', N'Khối 3', 1, '2024-10-01', '2024-10-31', 0),
('Thang3_Ki1', N'Điểm Tháng 3 (Kỳ 1)', N'Khối 3', 1, '2024-11-01', '2024-11-30', 0),
('GiuaKi1', N'Điểm Giữa Kỳ 1', N'Khối 3', 1, '2024-11-01', '2024-11-15', 0),
('CuoiKi1', N'Điểm Cuối Kỳ 1', N'Khối 3', 1, '2024-12-15', '2025-01-15', 0),
('Thang1_Ki2', N'Điểm Tháng 1 (Kỳ 2)', N'Khối 3', 2, '2025-01-20', '2025-02-28', 0),
('Thang2_Ki2', N'Điểm Tháng 2 (Kỳ 2)', N'Khối 3', 2, '2025-03-01', '2025-03-31', 0),
('Thang3_Ki2', N'Điểm Tháng 3 (Kỳ 2)', N'Khối 3', 2, '2025-04-01', '2025-04-30', 0),
('GiuaKi2', N'Điểm Giữa Kỳ 2', N'Khối 3', 2, '2025-04-01', '2025-04-15', 0),
('CuoiKi2', N'Điểm Cuối Kỳ 2', N'Khối 3', 2, '2025-05-15', '2025-06-15', 0);
-- Khối 4
INSERT INTO ThoiHanDiem (MaCotDiem, TenHienThi, Khoi, HocKy, NgayMoDiem, NgayKhoaDiem, KhoaThuCong)
VALUES
('Thang1_Ki1', N'Điểm Tháng 1 (Kỳ 1)', N'Khối 4', 1, '2024-09-01', '2024-09-30', 0),
('Thang2_Ki1', N'Điểm Tháng 2 (Kỳ 1)', N'Khối 4', 1, '2024-10-01', '2024-10-31', 0),
('Thang3_Ki1', N'Điểm Tháng 3 (Kỳ 1)', N'Khối 4', 1, '2024-11-01', '2024-11-30', 0),
('GiuaKi1', N'Điểm Giữa Kỳ 1', N'Khối 4', 1, '2024-11-01', '2024-11-15', 0),
('CuoiKi1', N'Điểm Cuối Kỳ 1', N'Khối 4', 1, '2024-12-15', '2025-01-15', 0),
('Thang1_Ki2', N'Điểm Tháng 1 (Kỳ 2)', N'Khối 4', 2, '2025-01-20', '2025-02-28', 0),
('Thang2_Ki2', N'Điểm Tháng 2 (Kỳ 2)', N'Khối 4', 2, '2025-03-01', '2025-03-31', 0),
('Thang3_Ki2', N'Điểm Tháng 3 (Kỳ 2)', N'Khối 4', 2, '2025-04-01', '2025-04-30', 0),
('GiuaKi2', N'Điểm Giữa Kỳ 2', N'Khối 4', 2, '2025-04-01', '2025-04-15', 0),
('CuoiKi2', N'Điểm Cuối Kỳ 2', N'Khối 4', 2, '2025-05-15', '2025-06-15', 0);
-- Khối 5
INSERT INTO ThoiHanDiem (MaCotDiem, TenHienThi, Khoi, HocKy, NgayMoDiem, NgayKhoaDiem, KhoaThuCong)
VALUES
('Thang1_Ki1', N'Điểm Tháng 1 (Kỳ 1)', N'Khối 5', 1, '2024-09-01', '2024-09-30', 0),
('Thang2_Ki1', N'Điểm Tháng 2 (Kỳ 1)', N'Khối 5', 1, '2024-10-01', '2024-10-31', 0),
('Thang3_Ki1', N'Điểm Tháng 3 (Kỳ 1)', N'Khối 5', 1, '2024-11-01', '2024-11-30', 0),
('GiuaKi1', N'Điểm Giữa Kỳ 1', N'Khối 5', 1, '2024-10-20', '2024-11-10', 0),
('CuoiKi1', N'Điểm Cuối Kỳ 1', N'Khối 5', 1, '2024-12-10', '2025-01-10', 0),
('Thang1_Ki2', N'Điểm Tháng 1 (Kỳ 2)', N'Khối 5', 2, '2025-01-20', '2025-02-28', 0),
('Thang2_Ki2', N'Điểm Tháng 2 (Kỳ 2)', N'Khối 5', 2, '2025-03-01', '2025-03-31', 0),
('Thang3_Ki2', N'Điểm Tháng 3 (Kỳ 2)', N'Khối 5', 2, '2025-04-01', '2025-04-30', 0),
('GiuaKi2', N'Điểm Giữa Kỳ 2', N'Khối 5', 2, '2025-03-20', '2025-04-10', 0),
('CuoiKi2', N'Điểm Cuối Kỳ 2', N'Khối 5', 2, '2025-05-10', '2025-06-10', 0);
GO

PRINT 'ĐÃ CHÈN TẤT CẢ DỮ LIỆU MẪU.';
GO
--================================================================
-- BƯỚC 3: TẠO CÁC TRIGGERS VÀ SP KHỞI TẠO
--================================================================

CREATE PROCEDURE sp_TaoDiemDanhMacDinh
    @MaLop VARCHAR(10),
    @Ngay DATE = NULL,
    @Buoi NVARCHAR(10) = N'Sáng'
AS
BEGIN
    SET NOCOUNT ON;
    IF @Ngay IS NULL SET @Ngay = CAST(GETDATE() AS DATE);

    INSERT INTO DiemDanh (MaDD, MaHS, NgayDD, Buoi, TrangThai)
    SELECT LEFT(NEWID(), 8), hs.MaHS, CAST(@Ngay AS DATETIME), @Buoi, N'Có mặt'
    FROM HocSinh hs
    WHERE hs.MaLop = @MaLop
      AND NOT EXISTS (
          SELECT 1 FROM DiemDanh dd
          WHERE dd.MaHS = hs.MaHS AND CAST(dd.NgayDD AS DATE) = @Ngay AND dd.Buoi = @Buoi
      );
END;
GO

CREATE TRIGGER trg_TaoDiemDanhHocSinhMoi ON HocSinh AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @today DATE = CAST(GETDATE() AS DATE);
    INSERT INTO DiemDanh (MaDD, MaHS, NgayDD, Buoi, TrangThai)
    SELECT LEFT(NEWID(), 8), i.MaHS, @today, N'Sáng', N'Có mặt'
    FROM INSERTED i;
END;
GO

CREATE PROCEDURE sp_TaoKetQuaHocTapMacDinh
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @loai TABLE (Loai NVARCHAR(20));
    INSERT INTO @loai (Loai)
    SELECT DISTINCT MaCotDiem FROM ThoiHanDiem;

    IF NOT EXISTS (SELECT 1 FROM @loai)
    BEGIN
        PRINT N'Không có loại điểm nào trong ThoiHanDiem. Bỏ qua khởi tạo KetQuaHocTap.';
        RETURN;
    END

    INSERT INTO KetQuaHocTap (MaKQ, MaMon, MaHS, NgayNhap, Loai)
    SELECT LEFT(NEWID(), 8), m.MaMon, hs.MaHS, GETDATE(), l.Loai
    FROM HocSinh hs
    CROSS JOIN MonHoc m
    CROSS JOIN @loai l
    WHERE NOT EXISTS (
        SELECT 1 FROM KetQuaHocTap kq
        WHERE kq.MaHS = hs.MaHS AND kq.MaMon = m.MaMon AND kq.Loai = l.Loai
    );
END;
GO


CREATE TRIGGER trg_TaoKetQuaHocTapHocSinhMoi ON HocSinh AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @loai TABLE (Loai NVARCHAR(20));
    INSERT INTO @loai (Loai)
    SELECT DISTINCT MaCotDiem FROM ThoiHanDiem;

    IF NOT EXISTS (SELECT 1 FROM @loai)
    BEGIN
        PRINT N'Không có loại điểm nào trong ThoiHanDiem. Bỏ qua Trigger trg_TaoKetQuaHocTapHocSinhMoi.';
        RETURN;
    END

    INSERT INTO KetQuaHocTap (MaKQ, MaMon, MaHS, NgayNhap, Loai)
    SELECT LEFT(NEWID(), 8), m.MaMon, i.MaHS, GETDATE(), l.Loai
    FROM INSERTED i
    CROSS JOIN MonHoc m
    CROSS JOIN @loai l;
END;
GO
CREATE TRIGGER trg_UpdateDiemDanhTimestamp
ON DiemDanh
AFTER UPDATE
AS
BEGIN
    IF UPDATE(TrangThai)
    BEGIN
        UPDATE DiemDanh
        SET ThoiGianCapNhat = GETDATE()
        FROM DiemDanh
        INNER JOIN inserted ON DiemDanh.MaDD = inserted.MaDD;
    END
END;
GO

CREATE PROCEDURE sp_GetHomeroomGradebook
    @MaLop VARCHAR(10),
    @LoaiDiem NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @cols AS NVARCHAR(MAX),
            @query AS NVARCHAR(MAX);

    SELECT @cols = STUFF((SELECT DISTINCT ',' + QUOTENAME(mh.TenMon) 
                    FROM KetQuaHocTap kq
                    JOIN MonHoc mh ON kq.MaMon = mh.MaMon
                    JOIN HocSinh hs ON kq.MaHS = hs.MaHS
                    WHERE hs.MaLop = @MaLop AND kq.Loai = @LoaiDiem AND kq.Diem IS NOT NULL
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'')

    IF @cols IS NULL
    BEGIN
        SELECT MaHS, HoTen FROM HocSinh WHERE MaLop = @MaLop ORDER BY HoTen;
        RETURN;
    END

    SET @query = 'SELECT MaHS, HoTen, ' + @cols + ' from 
            (
                SELECT 
                    hs.MaHS,
                    hs.HoTen,
                    mh.TenMon,
                    kq.Diem
                FROM KetQuaHocTap kq
                JOIN HocSinh hs ON kq.MaHS = hs.MaHS
                JOIN MonHoc mh ON kq.MaMon = mh.MaMon
                WHERE hs.MaLop = ''' + @MaLop + ''' AND kq.Loai = ''' + @LoaiDiem + '''
            ) x
            pivot 
            (
                MAX(Diem)
                for TenMon in (' + @cols + ')
            ) p 
            ORDER BY HoTen'

    EXECUTE(@query);
END
GO
--------------------------------------------------
-- KHỞI TẠO DỮ LIỆU BAN ĐẦU
--------------------------------------------------
PRINT N'Đang khởi tạo điểm danh mặc định cho 15 lớp...';
EXEC sp_TaoDiemDanhMacDinh @MaLop = '1A1';
EXEC sp_TaoDiemDanhMacDinh @MaLop = '1A2';
EXEC sp_TaoDiemDanhMacDinh @MaLop = '1A3';
EXEC sp_TaoDiemDanhMacDinh @MaLop = '2A1';
EXEC sp_TaoDiemDanhMacDinh @MaLop = '2A2';
EXEC sp_TaoDiemDanhMacDinh @MaLop = '2A3';
EXEC sp_TaoDiemDanhMacDinh @MaLop = '3A1';
EXEC sp_TaoDiemDanhMacDinh @MaLop = '3A2';
EXEC sp_TaoDiemDanhMacDinh @MaLop = '3A3';
EXEC sp_TaoDiemDanhMacDinh @MaLop = '4A1';
EXEC sp_TaoDiemDanhMacDinh @MaLop = '4A2';
EXEC sp_TaoDiemDanhMacDinh @MaLop = '4A3';
EXEC sp_TaoDiemDanhMacDinh @MaLop = '5A1';
EXEC sp_TaoDiemDanhMacDinh @MaLop = '5A2';
EXEC sp_TaoDiemDanhMacDinh @MaLop = '5A3';
PRINT N'Đang khởi tạo kết quả học tập mặc định cho 300 học sinh...';
EXEC sp_TaoKetQuaHocTapMacDinh;
GO

-- Cập nhật điểm mẫu
UPDATE KetQuaHocTap SET Diem = 8.5 WHERE MaHS = 'HS241' AND MaMon = 'TOAN' AND Loai = 'GiuaKi1';
UPDATE KetQuaHocTap SET Diem = 7.0 WHERE MaHS = 'HS241' AND MaMon = 'TV' AND Loai = 'GiuaKi1';
UPDATE KetQuaHocTap SET Diem = 9.0 WHERE MaHS = 'HS241' AND MaMon = 'ANH' AND Loai = 'CuoiKi1';
UPDATE KetQuaHocTap SET Diem = 9.5 WHERE MaHS = 'HS242' AND MaMon = 'TOAN' AND Loai = 'CuoiKi1';
UPDATE KetQuaHocTap SET Diem = 10  WHERE MaHS = 'HS242' AND MaMon = 'TIN' AND Loai = 'GiuaKi1';
UPDATE KetQuaHocTap SET Diem = 8.0 WHERE MaHS = 'HS001' AND MaMon = 'TOAN' AND Loai = 'GiuaKi1';
UPDATE KetQuaHocTap SET Diem = 9.0 WHERE MaHS = 'HS001' AND MaMon = 'TV' AND Loai = 'GiuaKi1';
UPDATE KetQuaHocTap SET Diem = 7.5 WHERE MaHS = 'HS061' AND MaMon = 'TOAN' AND Loai = 'GiuaKi1';
UPDATE KetQuaHocTap SET Diem = 8.0 WHERE MaHS = 'HS121' AND MaMon = 'TIN' AND Loai = 'GiuaKi1';
GO

--================================================================
-- BƯỚC 4: TẠO CÁC TABLE TYPES (CHO IMPORT)
--================================================================

CREATE TYPE ut_HocSinhImport AS TABLE(
    MaHS VARCHAR(10) PRIMARY KEY,
    MaLop VARCHAR(10) NULL,
    HoTen NVARCHAR(100) NOT NULL,
    NgaySinh DATE NOT NULL,
    GioiTinh NVARCHAR(10) NULL,
    SDTPhuHuynh VARCHAR(15) NULL,
    DiaChi NVARCHAR(200) NULL,
    DanToc NVARCHAR(50) NULL
);
GO
CREATE TYPE ut_DateList AS TABLE(
    Ngay DATE PRIMARY KEY
);
GO
CREATE TYPE ut_TKBImport AS TABLE(
    Ngay DATE,
    Tiet INT,
    TenMon NVARCHAR(100),
    TenLop NVARCHAR(50),
    GhiChu NVARCHAR(200) NULL,
    MauSac VARCHAR(20) NULL
);
GO
CREATE TYPE ut_MaMonList AS TABLE(
    MaMon VARCHAR(10) PRIMARY KEY
);
GO
PRINT 'ĐÃ TẠO CÁC TABLE TYPES.';
GO
--================================================================
-- BƯỚC 5: TẠO TẤT CẢ CÁC STORED PROCEDURE
--================================================================
go
-- #region Đăng nhập
CREATE PROCEDURE sp_CheckTeacherLogin
    @user NVARCHAR(50),
    @pass VARCHAR(30)
AS
BEGIN
    SELECT TrangThai 
    FROM GiaoVien 
    WHERE Username=@user AND Password=@pass;
END;
GO
CREATE PROCEDURE sp_CheckAdminLogin
    @user NVARCHAR(50),
    @pass VARCHAR(30)
AS
BEGIN
    SELECT COUNT(*) 
    FROM Admin 
    WHERE Username=@user AND Password=@pass;
END;
GO
-- #endregion

-- #region Thông tin giáo viên
CREATE PROCEDURE sp_GetTeacherProfile
    @user NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        gv.Ten, 
        gv.Email, 
        gv.SDT, 
        gv.AnhDaiDien,
        ISNULL(STUFF((
            SELECT N', ' + mh.TenMon
            FROM GiaoVien_MonHoc gvm
            JOIN MonHoc mh ON gvm.MaMon = mh.MaMon
            WHERE gvm.MaGV = gv.MaGV
            ORDER BY mh.TenMon
            FOR XML PATH('')
        ), 1, 2, N''), N'Chưa có môn') AS TenMon
    FROM GiaoVien gv
    WHERE gv.Username = @user OR gv.Ten = @user;
END;
GO
CREATE PROCEDURE sp_UpdateTeacherAvatar
    @user NVARCHAR(50),
    @avatar NVARCHAR(200)
AS
BEGIN
    UPDATE GiaoVien 
    SET AnhDaiDien = @avatar 
    WHERE Username = @user OR Ten = @user;
END;
GO
CREATE PROCEDURE sp_GetMonByTeacher
    @id NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1 gvm.MaMon
    FROM GiaoVien gv
    JOIN GiaoVien_MonHoc gvm ON gv.MaGV = gvm.MaGV
    WHERE gv.Username=@id OR gv.Ten=@id
    ORDER BY gvm.MaMon;
END;
GO
-- #endregion

-- #region Học sinh
CREATE PROCEDURE sp_GetHocSinhByLop
    @malop VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        ROW_NUMBER() OVER (ORDER BY HoTen) AS STT, 
        MaHS, 
        HoTen, 
        GioiTinh, 
        NgaySinh, 
        DiaChi, 
        DanToc, 
        SDTPhuHuynh 
    FROM HocSinh 
    WHERE MaLop = @malop 
    ORDER BY HoTen;
END;
GO
CREATE PROCEDURE sp_GetHocSinhProfile
    @maHS VARCHAR(10)
AS
BEGIN
    SELECT * FROM HocSinh 
    WHERE MaHS=@maHS;
END;
GO
CREATE PROCEDURE sp_UpdateHocSinhProfile
    @MaHS VARCHAR(10),
    @HoTen NVARCHAR(100),
    @GioiTinh NVARCHAR(10),
    @NgaySinh DATE,
    @DiaChi NVARCHAR(200)
AS
BEGIN
    UPDATE HocSinh 
    SET HoTen=@HoTen, GioiTinh=@GioiTinh, NgaySinh=@NgaySinh, DiaChi=@DiaChi
    WHERE MaHS=@MaHS;
END;
GO
-- #endregion

-- #region Điểm danh
CREATE PROCEDURE sp_GetDiemDanhByLop
    @maLop VARCHAR(10)
AS
BEGIN
    SELECT 
        dd.MaDD,
        hs.MaHS,
        hs.HoTen,
        dd.NgayDD,
        dd.Buoi,
        dd.TrangThai,
        dd.ThoiGianCapNhat
    FROM HocSinh hs
    LEFT JOIN DiemDanh dd ON hs.MaHS = dd.MaHS
    WHERE hs.MaLop = @maLop
    ORDER BY hs.HoTen, dd.NgayDD;
END;
GO
CREATE PROCEDURE sp_UpsertDiemDanh
    @MaHS VARCHAR(10),
    @Ngay DATETIME,
    @Buoi NVARCHAR(10),
    @TrangThai NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Buoi IS NULL OR @Buoi = '' SET @Buoi = N'Sáng';
    IF @TrangThai IS NULL OR @TrangThai = '' SET @TrangThai = N'Có mặt';

    DECLARE @ExistingMaDD VARCHAR(10);
    DECLARE @NgayDate DATE = CAST(@Ngay AS DATE);

    SELECT @ExistingMaDD = MaDD 
    FROM DiemDanh 
    WHERE MaHS = @MaHS AND CAST(NgayDD AS DATE) = @NgayDate AND Buoi = @Buoi;

    IF @ExistingMaDD IS NOT NULL
    BEGIN
        UPDATE DiemDanh 
        SET TrangThai = @TrangThai, NgayDD = @Ngay
        WHERE MaDD = @ExistingMaDD;
    END
    ELSE
    BEGIN
        INSERT INTO DiemDanh(MaDD, MaHS, NgayDD, Buoi, TrangThai)
        VALUES(LEFT(NEWID(), 8), @MaHS, @Ngay, @Buoi, @TrangThai);
    END
END;
GO
CREATE PROCEDURE sp_GetDiemDanhByLopAndDate
    @maLop VARCHAR(10),
    @ngay DATE,
    @buoi NVARCHAR(10)
AS
BEGIN
    SELECT
        hs.MaHS,
        hs.HoTen,
        dd.MaDD,
        dd.NgayDD,
        dd.Buoi,
        dd.TrangThai,
        dd.ThoiGianCapNhat
    FROM HocSinh hs
    LEFT JOIN (
        SELECT MaDD, MaHS, NgayDD, Buoi, TrangThai, ThoiGianCapNhat
        FROM DiemDanh
        WHERE CAST(NgayDD AS date) = @ngay
        AND (@buoi IS NULL OR Buoi = @buoi)
    ) dd ON hs.MaHS = dd.MaHS
    WHERE hs.MaLop = @maLop
    ORDER BY hs.HoTen;
END;
GO
CREATE PROCEDURE sp_UpdateDiemDanh
    @MaDD VARCHAR(10),
    @TrangThai NVARCHAR(20)
AS
BEGIN
    UPDATE DiemDanh SET TrangThai=@TrangThai WHERE MaDD=@MaDD;
END;
GO
-- #endregion

-- #region Kết quả học tập

create PROCEDURE sp_InsertKetQuaHocTap
    @MaHS VARCHAR(10),
    @MaMon VARCHAR(10),
    @Diem FLOAT,
    @NhanXet NVARCHAR(200)
AS
BEGIN
    INSERT INTO KetQuaHocTap(MaKQ, MaMon, MaHS, NgayNhap, Diem, NhanXet, Loai) 
    VALUES(LEFT(NEWID(), 8), @MaMon, @MaHS, GETDATE(), @Diem, @NhanXet, N'GiuaKi1');
END;
GO

ALTER PROCEDURE sp_UpsertKetQuaHocTap
    @MaHS VARCHAR(30),
    @MaMon VARCHAR(20),
    @Loai VARCHAR(30),
    @Diem FLOAT = NULL,
    @NhanXet NVARCHAR(200) = NULL, -- ĐÃ THÊM
    @GhiChu NVARCHAR(200) = NULL   -- ĐÃ THÊM
AS
BEGIN
    SET NOCOUNT ON;

    -- BƯỚC 1A: TÌM KHỐI CỦA HỌC SINH
    DECLARE @KhoiCuaHS NVARCHAR(20);
    SELECT @KhoiCuaHS = lh.Khoi 
    FROM HocSinh hs 
    JOIN LopHoc lh ON hs.MaLop = lh.MaLop 
    WHERE hs.MaHS = @MaHS;

    IF @KhoiCuaHS IS NULL
    BEGIN
        RAISERROR (N'Lỗi: Không tìm thấy học sinh [%s] hoặc học sinh chưa được xếp lớp.', 16, 1, @MaHS);
        RETURN;
    END

    -- BƯỚC 1B: KIỂM TRA THỜI HẠN NHẬP ĐIỂM (Chỉ kiểm tra nếu @Diem có giá trị)
    IF @Diem IS NOT NULL
    BEGIN
        DECLARE @DaKhoa BIT = 0;
        DECLARE @TenCotDiem NVARCHAR(100);
        DECLARE @HomNay DATE = CAST(GETDATE() AS DATE); 
        DECLARE @HocKyCuaLoaiDiem INT;

        BEGIN TRY
            SELECT 
                @TenCotDiem = ISNULL(TenHienThi, @Loai),
                @HocKyCuaLoaiDiem = HocKy,
                @DaKhoa = CASE 
                    WHEN KhoaThuCong = 1 THEN 1 
                    WHEN @HomNay > CAST(NgayKhoaDiem AS DATE) THEN 1 
                    WHEN @HomNay < CAST(NgayMoDiem AS DATE) THEN 1  
                    ELSE 0
                END
            FROM 
                ThoiHanDiem
            WHERE 
                MaCotDiem = @Loai
                AND Khoi = @KhoiCuaHS;

            IF @TenCotDiem IS NULL
            BEGIN
                RAISERROR (N'Lỗi: Cột điểm [%s] cho khối [%s] không được định nghĩa trong Bảng Thời Hạn Điểm. Vui lòng liên hệ Admin.', 
                           16, 1, @Loai, @KhoiCuaHS);
                RETURN;
            END

            IF @DaKhoa = 1
            BEGIN
                RAISERROR (N'Lỗi: Cột điểm [%s] (Khối %s) đã bị khóa hoặc chưa đến hạn nhập điểm. Không thể lưu.', 
                           16, 1, @TenCotDiem, @KhoiCuaHS);
                RETURN;
            END
        END TRY
        BEGIN CATCH
            DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
            RAISERROR (@ErrorMessage, 16, 1);
            RETURN;
        END CATCH
    END -- Hết kiểm tra khóa điểm

    -- BƯỚC 2: THỰC HIỆN UPSERT
    BEGIN TRY
        IF EXISTS (SELECT 1 
                   FROM KetQuaHocTap 
                   WHERE MaHS = @MaHS 
                     AND MaMon = @MaMon 
                     AND Loai = @Loai)
        BEGIN
            UPDATE KetQuaHocTap
            SET 
                -- Cập nhật có điều kiện: chỉ cập nhật nếu giá trị được truyền vào KHÔNG NULL
                Diem = CASE WHEN @Diem IS NOT NULL THEN @Diem ELSE Diem END,
                NhanXet = CASE WHEN @NhanXet IS NOT NULL THEN @NhanXet ELSE NhanXet END,
                GhiChu = CASE WHEN @GhiChu IS NOT NULL THEN @GhiChu ELSE GhiChu END,
                NgayNhap = GETDATE()
            WHERE 
                MaHS = @MaHS 
                AND MaMon = @MaMon 
                AND Loai = @Loai;
        END
        ELSE
        BEGIN
            -- Chỉ chèn nếu có ít nhất 1 giá trị
            IF @Diem IS NOT NULL OR @NhanXet IS NOT NULL OR @GhiChu IS NOT NULL
            BEGIN
                INSERT INTO KetQuaHocTap (MaKQ, MaHS, MaMon, Loai, Diem, NhanXet, GhiChu, NgayNhap)
                VALUES (LEFT(NEWID(), 8), @MaHS, @MaMon, @Loai, @Diem, @NhanXet, @GhiChu, GETDATE());
            END
        END
    END TRY
    BEGIN CATCH
        DECLARE @UpsertErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR (N'Lỗi khi lưu điểm: %s', 16, 1, @UpsertErrorMessage);
        RETURN;
    END CATCH
END
GO

CREATE PROCEDURE sp_GetKetQuaHocTapByLop
    @malop VARCHAR(10)
AS
BEGIN
    SELECT hs.MaHS, hs.HoTen, mh.TenMon, kq.Diem, kq.NhanXet, kq.GhiChu, kq.Loai
    FROM HocSinh hs
    INNER JOIN KetQuaHocTap kq ON hs.MaHS = kq.MaHS
    INNER JOIN MonHoc mh ON kq.MaMon = mh.MaMon
    WHERE hs.MaLop = @malop
    ORDER BY hs.MaHS, mh.MaMon;
END;
GO
CREATE PROCEDURE sp_GetBangDiemPivot
    @malop VARCHAR(10),
    @ki INT,
    @maMon VARCHAR(10)
AS
BEGIN
    DECLARE @khoi NVARCHAR(20);
    SELECT @khoi = Khoi FROM LopHoc WHERE MaLop = @malop;

    DECLARE @Thang1Loai NVARCHAR(20), @Thang2Loai NVARCHAR(20), @Thang3Loai NVARCHAR(20), @GiuaKiLoai NVARCHAR(20), @CuoiKiLoai NVARCHAR(20);

    SELECT @Thang1Loai = MaCotDiem FROM ThoiHanDiem WHERE Khoi = @khoi AND HocKy = @ki AND MaCotDiem LIKE 'Thang1%';
    SELECT @Thang2Loai = MaCotDiem FROM ThoiHanDiem WHERE Khoi = @khoi AND HocKy = @ki AND MaCotDiem LIKE 'Thang2%';
    SELECT @Thang3Loai = MaCotDiem FROM ThoiHanDiem WHERE Khoi = @khoi AND HocKy = @ki AND MaCotDiem LIKE 'Thang3%';
    SELECT @GiuaKiLoai = MaCotDiem FROM ThoiHanDiem WHERE Khoi = @khoi AND HocKy = @ki AND MaCotDiem LIKE 'GiuaKi%';
    SELECT @CuoiKiLoai = MaCotDiem FROM ThoiHanDiem WHERE Khoi = @khoi AND HocKy = @ki AND MaCotDiem LIKE 'CuoiKi%';

    SET @Thang1Loai = ISNULL(@Thang1Loai, 'Thang1_Ki' + CAST(@ki AS VARCHAR));
    SET @Thang2Loai = ISNULL(@Thang2Loai, 'Thang2_Ki' + CAST(@ki AS VARCHAR));
    SET @Thang3Loai = ISNULL(@Thang3Loai, 'Thang3_Ki' + CAST(@ki AS VARCHAR));
    SET @GiuaKiLoai = ISNULL(@GiuaKiLoai, 'GiuaKi' + CAST(@ki AS VARCHAR));
    SET @CuoiKiLoai = ISNULL(@CuoiKiLoai, 'CuoiKi' + CAST(@ki AS VARCHAR));
    
    DECLARE @loaiFilter NVARCHAR(10) = N'%Ki' + CAST(@ki AS VARCHAR) + N'%';
    DECLARE @sql NVARCHAR(MAX);

    SET @sql = N'
    SELECT 
        hs.MaHS, 
        hs.HoTen,
        lh.TenLop,
        MAX(CASE WHEN kq.Loai = @Thang1Loai THEN kq.Diem END) AS Thang1,
        MAX(CASE WHEN kq.Loai = @Thang2Loai THEN kq.Diem END) AS Thang2,
        MAX(CASE WHEN kq.Loai = @Thang3Loai THEN kq.Diem END) AS Thang3,
        MAX(CASE WHEN kq.Loai = @GiuaKiLoai THEN kq.Diem END) AS GiuaKi,
        MAX(CASE WHEN kq.Loai = @CuoiKiLoai THEN kq.Diem END) AS CuoiKi,
        MAX(CASE WHEN kq.Loai LIKE @loaiFilter THEN kq.NhanXet END) AS NhanXet,
        MAX(CASE WHEN kq.Loai LIKE @loaiFilter THEN kq.GhiChu END) AS GhiChu
    FROM HocSinh hs
    JOIN LopHoc lh ON hs.MaLop = lh.MaLop
    LEFT JOIN KetQuaHocTap kq ON hs.MaHS = kq.MaHS AND kq.MaMon = @maMon
    WHERE hs.MaLop = @malop
    GROUP BY hs.MaHS, hs.HoTen, lh.TenLop
    ORDER BY hs.HoTen';

    EXEC sp_executesql @sql, 
        N'@malop VARCHAR(10), @maMon VARCHAR(10), @loaiFilter NVARCHAR(10), @Thang1Loai NVARCHAR(20), @Thang2Loai NVARCHAR(20), @Thang3Loai NVARCHAR(20), @GiuaKiLoai NVARCHAR(20), @CuoiKiLoai NVARCHAR(20)', 
        @malop, @maMon, @loaiFilter, @Thang1Loai, @Thang2Loai, @Thang3Loai, @GiuaKiLoai, @CuoiKiLoai;
END;
GO
-- #endregion

-- #region Hồ sơ cá nhân (Profile)
CREATE PROCEDURE sp_UpdateTeacherProfile
    @u NVARCHAR(50),
    @e NVARCHAR(50),
    @s VARCHAR(15),
    @a NVARCHAR(200)
AS
BEGIN
    UPDATE GiaoVien 
    SET Email=@e, SDT=@s, AnhDaiDien=@a
    WHERE Username=@u;
END;
GO
CREATE PROCEDURE sp_ChangeTeacherPassword
    @u NVARCHAR(50),
    @oldPass VARCHAR(30),
    @newPass VARCHAR(30)
AS
BEGIN
    DECLARE @currentPass VARCHAR(30);
    SELECT @currentPass = Password FROM GiaoVien WHERE Username=@u OR Ten =@u;

    IF @currentPass IS NULL OR @currentPass != @oldPass
    BEGIN
        SELECT 0;
        RETURN;
    END

    UPDATE GiaoVien SET Password=@newPass WHERE Username=@u OR Ten =@u;
    SELECT 1;
END;
GO
CREATE PROCEDURE sp_GetAdminProfile
    @u NVARCHAR(50)
AS
BEGIN
    SELECT * FROM Admin WHERE Username=@u;
END;
GO
CREATE PROCEDURE sp_UpdateAdminEmail
    @u NVARCHAR(50),
    @e NVARCHAR(50)
AS
BEGIN
    UPDATE Admin SET Email=@e WHERE Username=@u;
END;
GO
CREATE PROCEDURE sp_ChangeAdminPassword
    @u NVARCHAR(50),
    @oldPass VARCHAR(30),
    @newPass VARCHAR(30)
AS
BEGIN
    DECLARE @currentPass VARCHAR(30);
    SELECT @currentPass = Password FROM Admin WHERE Username=@u;

    IF @currentPass IS NULL OR @currentPass != @oldPass
    BEGIN
        SELECT 0;
        RETURN;
    END

    UPDATE Admin SET Password=@newPass WHERE Username=@u;
    SELECT 1;
END;
GO
-- #endregion

-- #region Tài liệu
CREATE PROCEDURE sp_GetTaiLieuByGV
    @gv VARCHAR(10)
AS
BEGIN
    SELECT MaTL, TenTL, MoTa, Kieu, NgayTaiLen, TrangThaiChiaSe 
    FROM TaiLieu 
    WHERE MaGV=@gv;
END;
GO
CREATE PROCEDURE sp_InsertTaiLieu
    @gv VARCHAR(10),
    @ten NVARCHAR(100),
    @moTa NVARCHAR(200),
    @kieu NVARCHAR(200),
    @tt NVARCHAR(20)
AS
BEGIN
    INSERT INTO TaiLieu (MaTL, TenTL, MoTa, Kieu, NgayTaiLen, TrangThaiChiaSe, MaGV)
    VALUES(LEFT(NEWID(), 10), @ten, @moTa, @kieu, GETDATE(), @tt, @gv);
END;
GO
CREATE PROCEDURE sp_GetTaiLieuShared
AS
BEGIN
    SELECT MaTL, TenTL, MoTa, Kieu, NgayTaiLen, TrangThaiChiaSe 
    FROM TaiLieu 
    WHERE TrangThaiChiaSe=N'Chia sẻ';
END;
GO
CREATE PROCEDURE sp_DeleteTaiLieu
    @id VARCHAR(10)
AS
BEGIN
    DELETE FROM TaiLieu WHERE MaTL=@id;
END;
GO
CREATE PROCEDURE sp_ShareTaiLieu
    @id VARCHAR(10)
AS
BEGIN
    UPDATE TaiLieu SET TrangThaiChiaSe=N'Chia sẻ' WHERE MaTL=@id;
END;
GO
CREATE PROCEDURE sp_GetMaGVByUsername
    @u NVARCHAR(50)
AS
BEGIN
    SELECT MaGV FROM GiaoVien WHERE Username=@u OR Ten=@u;
END;
GO
-- #endregion

-- #region Thời khóa biểu
CREATE PROCEDURE sp_GetTKBByGV
    @MaGV VARCHAR(10),
    @Monday DATE,
    @Sunday DATE
AS
BEGIN
    SELECT 
        t.Ngay, 
        t.Tiet, 
        ISNULL(m.TenMon, '') AS TenMon,
        ISNULL(l.TenLop, '') AS TenLop,
        ISNULL(t.GhiChu, '') AS GhiChu,
        ISNULL(t.MauSac, '') AS MauSac
    FROM ThoiKhoaBieu t
    LEFT JOIN MonHoc m ON t.MaMon = m.MaMon
    LEFT JOIN LopHoc l ON t.MaLop = l.MaLop
    WHERE t.MaGV = @MaGV 
      AND t.Ngay >= @Monday 
      AND t.Ngay <= @Sunday;
END;
GO
CREATE PROCEDURE sp_UpsertTKBColor
    @MaGV VARCHAR(10),
    @Ngay DATE,
    @Tiet INT,
    @MauSac VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @MaTKB VARCHAR(10);
    SELECT @MaTKB = MaTKB FROM ThoiKhoaBieu WHERE MaGV=@MaGV AND Ngay=@Ngay AND Tiet=@Tiet;

    IF @MaTKB IS NOT NULL
    BEGIN
        UPDATE ThoiKhoaBieu SET MauSac=@MauSac WHERE MaTKB=@MaTKB;
    END
    ELSE
    BEGIN
        INSERT INTO ThoiKhoaBieu (MaTKB, Ngay, Tiet, MauSac, MaGV) 
        VALUES (LEFT(NEWID(), 10), @Ngay, @Tiet, @MauSac, @MaGV);
    END
END;
GO
CREATE PROCEDURE sp_UpsertTKBGhiChu
    @MaGV VARCHAR(10),
    @Ngay DATE,
    @Tiet INT,
    @Note NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @MaTKB VARCHAR(10);
    SELECT @MaTKB = MaTKB FROM ThoiKhoaBieu WHERE MaGV=@MaGV AND Ngay=@Ngay AND Tiet=@Tiet;

    IF @MaTKB IS NOT NULL
    BEGIN
        UPDATE ThoiKhoaBieu SET GhiChu=@Note WHERE MaTKB=@MaTKB;
    END
    ELSE
    BEGIN
        INSERT INTO ThoiKhoaBieu (MaTKB, Ngay, Tiet, GhiChu, MaGV) 
        VALUES (LEFT(NEWID(), 10), @Ngay, @Tiet, @Note, @MaGV);
    END
END;
GO
-- #endregion

-- #region Minigame
CREATE PROCEDURE sp_GetMiniGames
AS
BEGIN
    SELECT MaMNG, Ten, DuLieu FROM Minigame ORDER BY MaMNG;
END;
GO
CREATE PROCEDURE sp_GetGameData
    @maMNG VARCHAR(10)
AS
BEGIN
    SELECT DuLieu FROM Minigame WHERE MaMNG = @maMNG;
END;
GO
CREATE PROCEDURE sp_SaveGameData
    @maMNG VARCHAR(10),
    @data NVARCHAR(MAX)
AS
BEGIN
    UPDATE Minigame SET DuLieu = @data WHERE MaMNG = @maMNG;
END;
GO
-- #endregion

-- #region Quản lý
CREATE PROCEDURE sp_DeleteTKBGhiChu
    @MaGV VARCHAR(10),
    @Ngay DATE,
    @Tiet INT
AS
BEGIN
    DECLARE @maMon VARCHAR(10);
    DECLARE @maTKB VARCHAR(10);

    SELECT @maTKB = MaTKB, @maMon = MaMon 
    FROM ThoiKhoaBieu 
    WHERE MaGV=@MaGV AND Ngay=@Ngay AND Tiet=@Tiet;

    IF @maTKB IS NULL RETURN;

    IF @maMon IS NULL
    BEGIN
        DELETE FROM ThoiKhoaBieu WHERE MaTKB=@maTKB;
    END
    ELSE
    BEGIN
        UPDATE ThoiKhoaBieu SET GhiChu = NULL WHERE MaTKB=@maTKB;
    END
END;
GO
CREATE PROCEDURE sp_GetGiaoVienByTrangThai
    @tt NVARCHAR(20)
AS
BEGIN
    SELECT MaGV, Ten, Username, Email, SDT, TrangThai 
    FROM GiaoVien 
    WHERE TrangThai = @tt;
END;
GO
create PROCEDURE sp_UpdateTrangThaiGiaoVien
    @id VARCHAR(10),
    @tt NVARCHAR(20),
    @Email NVARCHAR(50) OUTPUT,
    @Ten NVARCHAR(100) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- 1. Thực hiện Update
    UPDATE GiaoVien 
    SET TrangThai=@tt 
    WHERE MaGV=@id;
    
    -- 2. Lấy giá trị trả về (để C# có thể gửi email)
    SELECT @Email = Email, @Ten = Ten
    FROM GiaoVien
    WHERE MaGV = @id;
END;
GO
CREATE PROCEDURE sp_UpdateGiaoVien
    @id VARCHAR(10),
    @t NVARCHAR(100),
    @e NVARCHAR(50),
    @s VARCHAR(15)
AS
BEGIN
    UPDATE GiaoVien SET Ten=@t, Email=@e, SDT=@s WHERE MaGV=@id;
END;
GO
CREATE PROCEDURE sp_DeleteGiaoVien
    @id VARCHAR(10)
AS
BEGIN
    DELETE FROM GiaoVien WHERE MaGV=@id;
END;
GO
CREATE PROCEDURE sp_GetAllGiaoVien
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        gv.MaGV, 
        gv.Ten, 
        gv.Username, 
        gv.Email, 
        gv.SDT, 
        gv.TrangThai,
        ISNULL(STUFF((
            SELECT N', ' + mh.TenMon
            FROM GiaoVien_MonHoc gvm
            JOIN MonHoc mh ON gvm.MaMon = mh.MaMon
            WHERE gvm.MaGV = gv.MaGV
            ORDER BY mh.TenMon
            FOR XML PATH('')
        ), 1, 2, N''), N'Chưa có môn') AS CacMonDay
    FROM GiaoVien gv;
END;
GO
CREATE PROCEDURE sp_GetAllHocSinh
AS
BEGIN
    SELECT MaHS, MaLop, HoTen, NgaySinh, GioiTinh, SDTPhuHuynh, DiaChi, DanToc 
    FROM HocSinh 
    ORDER BY MaLop, HoTen;
END;
GO
CREATE PROCEDURE sp_InsertHocSinh
    @MaHS VARCHAR(10),
    @MaLop VARCHAR(10),
    @HoTen NVARCHAR(100),
    @NgaySinh DATE,
    @GioiTinh NVARCHAR(10),
    @SDT VARCHAR(15),
    @DiaChi NVARCHAR(200),
    @DanToc NVARCHAR(50)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM HocSinh WHERE MaHS=@MaHS)
    BEGIN
        DECLARE @ErrorMsg NVARCHAR(100) = N'Học sinh ' + @MaHS + N' đã tồn tại.';
        RAISERROR(@ErrorMsg, 16, 1);
        RETURN;
    END

    INSERT INTO HocSinh (MaHS, MaLop, HoTen, NgaySinh, GioiTinh, SDTPhuHuynh, DiaChi, DanToc)
    VALUES (@MaHS, @MaLop, @HoTen, @NgaySinh, @GioiTinh, @SDT, @DiaChi, @DanToc);
END;
GO
CREATE PROCEDURE sp_UpdateHocSinh
    @MaHS VARCHAR(10),
    @HoTen NVARCHAR(100),
    @NgaySinh DATE,
    @GioiTinh NVARCHAR(10),
    @SDT VARCHAR(15),
    @DiaChi NVARCHAR(200),
    @DanToc NVARCHAR(50)
AS
BEGIN
    UPDATE HocSinh
    SET HoTen=@HoTen, NgaySinh=@NgaySinh, GioiTinh=@GioiTinh,
        SDTPhuHuynh=@SDT, DiaChi=@DiaChi, DanToc=@DanToc
    WHERE MaHS=@MaHS;
END;
GO
CREATE PROCEDURE sp_DeleteHocSinh
    @MaHS VARCHAR(10)
AS
BEGIN
    DELETE FROM HocSinh WHERE MaHS=@MaHS;
END;
GO
CREATE PROCEDURE sp_ImportHocSinh
    @HocSinhData ut_HocSinhImport READONLY
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT * INTO #TempHocSinh FROM @HocSinhData;

    DECLARE @Skipped INT;
    SELECT @Skipped = COUNT(t.MaHS) 
    FROM #TempHocSinh t
    INNER JOIN HocSinh hs ON t.MaHS = hs.MaHS;

    INSERT INTO HocSinh (MaHS, MaLop, HoTen, NgaySinh, GioiTinh, SDTPhuHuynh, DiaChi, DanToc)
    SELECT 
        t.MaHS, t.MaLop, t.HoTen, t.NgaySinh, t.GioiTinh, t.SDTPhuHuynh, t.DiaChi, t.DanToc
    FROM #TempHocSinh t
    WHERE NOT EXISTS (
        SELECT 1 FROM HocSinh hs 
        WHERE hs.MaHS = t.MaHS
    );

    DECLARE @Success INT;
    SET @Success = @@ROWCOUNT;

    SELECT @Success AS [Success], @Skipped AS [Skipped];

    DROP TABLE #TempHocSinh;
END;
GO
-- #endregion

-- #region Báo cáo
CREATE PROCEDURE sp_GetLopByGiaoVien
    @maGV VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT DISTINCT l.MaLop, l.TenLop 
    FROM LopHoc l
    JOIN PhanCongGiangDay pc ON l.MaLop = pc.MaLop
    WHERE pc.MaGV = @maGV
    
    UNION 
    
    SELECT MaLop, TenLop 
    FROM LopHoc
    WHERE MaGVCN = @maGV;
END;
GO
CREATE PROCEDURE sp_GetBangDiemHocKy
    @maLop VARCHAR(10),
    @hocKy INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @monHocCols NVARCHAR(MAX), 
            @monHocColsSelect NVARCHAR(MAX), 
            @tongMon NVARCHAR(MAX),
            @sql NVARCHAR(MAX);
    DECLARE @loaiFilter NVARCHAR(10);
    DECLARE @monCount INT;

    DECLARE @khoi NVARCHAR(20);
    SELECT @khoi = Khoi FROM LopHoc WHERE MaLop = @maLop;

    SELECT @monHocCols = STUFF((SELECT DISTINCT ',' + QUOTENAME(mh.TenMon) 
                                FROM PhanCongGiangDay pcg
                                JOIN MonHoc mh ON pcg.MaMon = mh.MaMon
                                WHERE pcg.MaLop = @maLop
                                ORDER BY 1 FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'),1,1,'');
    
    SELECT @monHocColsSelect = STUFF((SELECT DISTINCT ',ROUND(ISNULL(' + QUOTENAME(mh.TenMon) + ', 0), 2) AS ' + QUOTENAME(mh.TenMon)
                                    FROM PhanCongGiangDay pcg
                                    JOIN MonHoc mh ON pcg.MaMon = mh.MaMon
                                    WHERE pcg.MaLop = @maLop
                                    ORDER BY 1 FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'),1,1,'');
    
    SELECT @tongMon = STUFF((SELECT DISTINCT ' + ISNULL(' + QUOTENAME(mh.TenMon) + ', 0)'
                            FROM PhanCongGiangDay pcg
                            JOIN MonHoc mh ON pcg.MaMon = mh.MaMon
                            WHERE pcg.MaLop = @maLop
                            ORDER BY 1 FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'),1,3,'');

    SELECT @monCount = COUNT(DISTINCT MaMon) FROM PhanCongGiangDay WHERE MaLop = @maLop;
    
    IF @monCount = 0 OR @monHocCols IS NULL
    BEGIN
        SELECT MaHS, HoTen FROM HocSinh WHERE MaLop = @maLop;
        RETURN;
    END;
    
    SET @loaiFilter = CASE WHEN @hocKy = 3 THEN N'%' ELSE CAST(@hocKy AS NVARCHAR) END;

    SET @sql = N'
    ;WITH DiemTB AS (
        SELECT 
            hs.MaHS,
            hs.HoTen,
            lh.TenLop,
            mh.TenMon,
            AVG(kq.Diem) AS DiemTB
        FROM HocSinh hs
        INNER JOIN LopHoc lh ON hs.MaLop = lh.MaLop
        INNER JOIN PhanCongGiangDay pcg ON hs.MaLop = pcg.MaLop
        INNER JOIN MonHoc mh ON pcg.MaMon = mh.MaMon
        LEFT JOIN KetQuaHocTap kq 
            ON hs.MaHS = kq.MaHS 
            AND mh.MaMon = kq.MaMon 
            AND kq.Loai IN (
                SELECT MaCotDiem 
                FROM ThoiHanDiem 
                WHERE Khoi = @khoi AND CAST(HocKy AS NVARCHAR) LIKE @loaiFilter
            )
        WHERE hs.MaLop = @maLop
        GROUP BY hs.MaHS, hs.HoTen, lh.TenLop, mh.TenMon
    ),
    PivotData AS (
        SELECT MaHS, HoTen, TenLop, ' + @monHocCols + N'
        FROM DiemTB
        PIVOT
        (
            AVG(DiemTB)
            FOR TenMon IN (' + @monHocCols + N')
        ) AS PivotTable
    )
    SELECT MaHS, HoTen, ' + @monHocColsSelect + N',
           ROUND((' + @tongMon + N') / NULLIF(' + CAST(@monCount AS VARCHAR) + N', 0), 2) AS [Trung bình chung]
    FROM PivotData
    ORDER BY HoTen;';

    EXEC sp_executesql @sql, N'@maLop VARCHAR(10), @khoi NVARCHAR(20), @loaiFilter NVARCHAR(10)', @maLop, @khoi, @loaiFilter;
END;
GO
CREATE PROCEDURE sp_GetHoSoHocSinh
    @maLop VARCHAR(10)
AS
BEGIN
    SELECT 
        MaHS, HoTen, GioiTinh, NgaySinh, DanToc, DiaChi, SDTPhuHuynh
    FROM HocSinh 
    WHERE MaLop = @maLop
    ORDER BY HoTen;
END;
GO
alter PROCEDURE sp_GetThongKeKhoi
    @khoi NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @khoiFilter NVARCHAR(25) = @khoi;

    DECLARE @loaiList TABLE (Loai NVARCHAR(20));
    INSERT INTO @loaiList (Loai)
    SELECT MaCotDiem FROM ThoiHanDiem WHERE Khoi = @khoiFilter;

    WITH StudentCounts AS (
        SELECT
            l.MaLop,
            l.TenLop,
            COUNT(hs.MaHS) AS SoHocSinh,
            SUM(CASE WHEN hs.GioiTinh = N'Nam' THEN 1 ELSE 0 END) AS SoNam,
            SUM(CASE WHEN hs.GioiTinh = N'Nữ' THEN 1 ELSE 0 END) AS SoNu
        FROM LopHoc l
        LEFT JOIN HocSinh hs ON l.MaLop = hs.MaLop
        WHERE l.Khoi = @khoiFilter
        GROUP BY l.MaLop, l.TenLop
    ),
    AvgScores AS (
        SELECT
            l.MaLop,
            ROUND(AVG(kq.Diem), 2) AS DiemTrungBinh
        FROM LopHoc l
        LEFT JOIN HocSinh hs ON l.MaLop = hs.MaLop
        LEFT JOIN KetQuaHocTap kq ON hs.MaHS = kq.MaHS
        WHERE l.Khoi = @khoiFilter 
          AND kq.Diem IS NOT NULL
          AND kq.Loai IN (SELECT Loai FROM @loaiList)
        GROUP BY l.MaLop
    )
    SELECT
        sc.TenLop,
        sc.SoHocSinh,
        ISNULL(av.DiemTrungBinh, 0) AS DiemTrungBinh,
        sc.SoNam,
        sc.SoNu
    FROM StudentCounts sc
    LEFT JOIN AvgScores av ON sc.MaLop = av.MaLop
    ORDER BY sc.TenLop;
END;
GO
-- #endregion
go
CREATE PROCEDURE sp_UpdateHocSinhLop
    @MaHS VARCHAR(10),
    @MaLopMoi VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Ghi chú: Nếu @MaLopMoi là NULL, học sinh sẽ bị "ra khỏi trường" (không thuộc lớp nào)
    -- Nếu bạn muốn ngăn điều này, hãy thêm kiểm tra IF @MaLopMoi IS NOT NULL
    
    UPDATE HocSinh
    SET MaLop = @MaLopMoi
    WHERE MaHS = @MaHS;
END;
GO
-- #region Hỗ trợ Giảng dạy
CREATE PROCEDURE sp_AddGhiChuTKB
    @MaGV VARCHAR(10),
    @MaLop VARCHAR(10),
    @Ngay DATE,
    @Tiet INT,
    @GhiChu NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @MaTKB VARCHAR(10);
    SELECT @MaTKB = MaTKB 
    FROM ThoiKhoaBieu 
    WHERE MaLop = @MaLop AND Ngay = @Ngay AND Tiet = @Tiet;

    IF @MaTKB IS NOT NULL
    BEGIN
        UPDATE ThoiKhoaBieu 
        SET GhiChu = ISNULL(GhiChu, '') + NCHAR(13) + NCHAR(10) + @GhiChu, 
            MaGV = @MaGV 
        WHERE MaTKB = @MaTKB;
    END
    ELSE
    BEGIN
        INSERT INTO ThoiKhoaBieu (MaTKB, Ngay, Tiet, GhiChu, MaGV, MaLop)
        VALUES (LEFT(NEWID(), 10), @Ngay, @Tiet, @GhiChu, @MaGV, @MaLop);
    END
END;
GO
CREATE PROCEDURE sp_AddGhiChuChoHocSinh
    @MaHS VARCHAR(10),
    @MaMon VARCHAR(10),
    @GhiChu NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Loai NVARCHAR(20);
    SELECT @Loai = thd.MaCotDiem
    FROM ThoiHanDiem thd
    JOIN HocSinh hs ON thd.Khoi = (SELECT Khoi FROM LopHoc WHERE MaLop = hs.MaLop)
    WHERE hs.MaHS = @MaHS AND thd.MaCotDiem LIKE 'CuoiKi2%';

    IF @Loai IS NULL SET @Loai = 'CuoiKi2';

    IF EXISTS (SELECT 1 FROM KetQuaHocTap WHERE MaHS = @MaHS AND MaMon = @MaMon AND Loai = @Loai)
    BEGIN
        UPDATE KetQuaHocTap SET GhiChu = @GhiChu WHERE MaHS = @MaHS AND MaMon = @MaMon AND Loai = @Loai;
    END
    ELSE
    BEGIN
        INSERT INTO KetQuaHocTap (MaKQ, MaMon, MaHS, NgayNhap, GhiChu, Loai)
        VALUES (LEFT(NEWID(), 10), @MaMon, @MaHS, GETDATE(), @GhiChu, @Loai);
    END
END;
GO
CREATE PROCEDURE sp_GetAllKetQuaHocTap
AS
BEGIN
    SELECT 
        hs.MaHS, hs.HoTen, lh.MaLop, lh.TenLop,
        mh.MaMon, mh.TenMon, kq.Loai, kq.Diem
    FROM KetQuaHocTap kq
    JOIN HocSinh hs ON kq.MaHS = hs.MaHS
    JOIN LopHoc lh ON hs.MaLop = lh.MaLop
    JOIN MonHoc mh ON kq.MaMon = mh.MaMon
    WHERE kq.Diem IS NOT NULL;
END;
GO
-- #endregion

-- #region Chung
CREATE PROCEDURE sp_GetAllMonHoc
AS
BEGIN
    SELECT MaMon, TenMon FROM MonHoc ORDER BY TenMon;
END;
GO
CREATE PROCEDURE sp_GetScoresForAnalysis
    @maGV VARCHAR(10),
    @phamVi NVARCHAR(20),
    @chiTiet NVARCHAR(50),
    @maMon VARCHAR(10),
    @hocKy INT
AS
BEGIN
    DECLARE @kyFilter NVARCHAR(10) = N'%' + CAST(@hocKy AS VARCHAR);
    DECLARE @sql NVARCHAR(MAX);

    SET @sql = N'
    SELECT 
        hs.MaHS, hs.HoTen, lh.MaLop, lh.TenLop,
        mh.MaMon, mh.TenMon, kq.Loai, kq.Diem
    FROM KetQuaHocTap kq
    JOIN HocSinh hs ON kq.MaHS = hs.MaHS
    JOIN LopHoc lh ON hs.MaLop = lh.MaLop
    JOIN MonHoc mh ON kq.MaMon = mh.MaMon
    JOIN ThoiHanDiem thd ON kq.Loai = thd.MaCotDiem AND lh.Khoi = thd.Khoi
    WHERE kq.Diem IS NOT NULL
    AND thd.HocKy = @hocKy';

    IF @phamVi = 'LopGV'
    BEGIN
        SET @sql = @sql + N' AND lh.MaLop = @chiTiet';
    END
    ELSE IF @phamVi = 'Khoi'
    BEGIN
        SET @sql = @sql + N' AND lh.Khoi = @chiTiet';
    END

    IF @maMon IS NOT NULL AND @maMon != 'ALL'
    BEGIN
        SET @sql = @sql + N' AND mh.MaMon = @maMon';
    END

    EXEC sp_executesql @sql, 
        N'@hocKy INT, @chiTiet NVARCHAR(50), @maMon VARCHAR(10)', 
        @hocKy, @chiTiet, @maMon;
END;
GO
create PROCEDURE sp_CreateTeacherRequest
    @Ten NVARCHAR(100),
    @Username NVARCHAR(50),
    @Password VARCHAR(30),
    @Email NVARCHAR(50),
    @SDT VARCHAR(15)
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM GiaoVien WHERE Username=@Username)
    BEGIN
        RAISERROR(N'Tên đăng nhập này đã tồn tại. Vui lòng chọn tên khác.', 16, 1);
        RETURN;
    END

    DECLARE @newId INT;
    SELECT @newId = ISNULL(MAX(CAST(SUBSTRING(MaGV, 3, LEN(MaGV)) AS INT)), 0) + 1 FROM GiaoVien;
    
    DECLARE @newMaGV VARCHAR(10) = 'GV' + RIGHT('00' + CAST(@newId AS VARCHAR), 3);

    INSERT INTO GiaoVien (MaGV, Ten, Username, Password, Email, SDT, MaAdmin, TrangThai) 
    VALUES (@newMaGV, @Ten, @Username, @Password, @Email, @SDT, 'AD001', N'Chưa xác nhận');

END;
go
CREATE PROCEDURE sp_GetTaiLieuSharedWithUploader
AS
BEGIN
    SELECT 
        tl.MaTL, tl.TenTL, tl.MoTa, tl.Kieu, tl.NgayTaiLen, 
        gv.Ten AS TenGV 
    FROM TaiLieu tl
    INNER JOIN GiaoVien gv ON tl.MaGV = gv.MaGV
    WHERE tl.TrangThaiChiaSe = N'Chia sẻ';
END;
GO
CREATE PROCEDURE sp_UnshareTaiLieu
    @MaTL VARCHAR(10)
AS
BEGIN
    UPDATE TaiLieu SET TrangThaiChiaSe = N'Riêng tư' WHERE MaTL = @MaTL;
END;
GO
CREATE PROCEDURE sp_GetHomeroomClassesByTeacher
    @maGV VARCHAR(10)
AS
BEGIN
    SELECT MaLop, TenLop FROM LopHoc WHERE MaGVCN = @maGV;
END;
GO
CREATE PROCEDURE sp_GetHomeroomClassNameByTeacherId
    @MaGV VARCHAR(10)
AS
BEGIN
    SELECT TenLop FROM LopHoc WHERE MaGVCN = @maGV;
END;
GO
Create PROCEDURE sp_GetBaoCaoChuyenCan
    @maLop VARCHAR(10),
    @hocKy INT
AS
BEGIN
    DECLARE @CurrentDate DATE = GETDATE();
    DECLARE @CurrentMonth INT = MONTH(@CurrentDate);
    DECLARE @CurrentYear INT = YEAR(@CurrentDate);
    
    DECLARE @NamHocStartYear INT;
    
    -- ================================================================
    -- SỬA LỖI LOGIC:
    -- Luôn tính năm học dựa trên ngày hiện tại, 
    -- vì giáo viên luôn muốn xem báo cáo của năm học hiện tại.
    -- (Giả sử năm học mới bắt đầu từ tháng 8)
    IF @CurrentMonth >= 8 
        SET @NamHocStartYear = @CurrentYear;
    ELSE 
        SET @NamHocStartYear = @CurrentYear - 1;
    -- ================================================================

    /* -- Bỏ logic cũ dựa trên cột NamHoc có thể đã lỗi thời
    DECLARE @NamHocStr VARCHAR(10);
    SELECT @NamHocStr = NamHoc FROM LopHoc WHERE MaLop = @maLop;
    SET @NamHocStartYear = CAST(LEFT(@NamHocStr, 4) AS INT) - 1;
    IF @NamHocStr IS NULL
    BEGIN
        IF @CurrentMonth >= 8 
            SET @NamHocStartYear = @CurrentYear;
        ELSE 
            SET @NamHocStartYear = @CurrentYear - 1;
    END;
    */
    
    DECLARE @StartDate DATE, @EndDate DATE;

    IF @hocKy = 1 
    BEGIN
        SET @StartDate = DATEFROMPARTS(@NamHocStartYear, 8, 1);
        SET @EndDate = DATEFROMPARTS(@NamHocStartYear, 12, 31);
    END
    ELSE IF @hocKy = 2 
    BEGIN
        SET @StartDate = DATEFROMPARTS(@NamHocStartYear + 1, 1, 1);
        SET @EndDate = DATEFROMPARTS(@NamHocStartYear + 1, 5, 31);
    END
    ELSE
    BEGIN
        SET @StartDate = DATEFROMPARTS(@NamHocStartYear, 8, 1);
        SET @EndDate = DATEFROMPARTS(@NamHocStartYear + 1, 5, 31);
    END;

    -- Truy vấn SELECT giữ nguyên
    SELECT 
        hs.MaHS, hs.HoTen,
        COUNT(CASE WHEN dd.TrangThai = N'Có mặt' THEN 1 END) as SoBuoiCoMat,
        COUNT(CASE WHEN dd.TrangThai = N'Vắng' THEN 1 END) as SoBuoiVang,
        COUNT(CASE WHEN dd.TrangThai LIKE N'%Có phép%' THEN 1 END) as SoBuoiVangCoPhep,
        COUNT(dd.MaDD) as TongSoBuoi,
        CAST(
            (COUNT(CASE WHEN dd.TrangThai = N'Có mặt' THEN 1 END) * 100.0) / NULLIF(COUNT(dd.MaDD), 0) 
            AS DECIMAL(5,0)
        ) as TyLeChuyenCan
    FROM HocSinh hs
    LEFT JOIN DiemDanh dd ON hs.MaHS = dd.MaHS AND CAST(dd.NgayDD AS DATE) BETWEEN @StartDate AND @EndDate
    WHERE hs.MaLop = @maLop
    GROUP BY hs.MaHS, hs.HoTen
    ORDER BY hs.HoTen;
END;
GO
CREATE PROCEDURE sp_GetMonHocByGiaoVienAndLop
    @maGV VARCHAR(10),
    @maLop VARCHAR(10)
AS
BEGIN
    SELECT DISTINCT m.MaMon, m.TenMon 
    FROM PhanCongGiangDay pc
    JOIN MonHoc m ON pc.MaMon = m.MaMon
    WHERE pc.MaGV = @maGV AND pc.MaLop = @maLop
    ORDER BY m.TenMon;
END;
GO
CREATE PROCEDURE sp_GetAllLopHoc
AS
BEGIN
    SELECT MaLop, TenLop, Khoi FROM LopHoc ORDER BY Khoi, TenLop;
END;
GO
CREATE PROCEDURE sp_GetUnassignedHomeroomTeachers
AS
BEGIN
    SELECT MaGV, Ten 
    FROM GiaoVien 
    WHERE TrangThai = N'Đã xác nhận' 
      AND MaGV NOT IN (SELECT DISTINCT MaGVCN FROM LopHoc WHERE MaGVCN IS NOT NULL);
END;
GO
CREATE PROCEDURE sp_UpdateGvcnForLop
    @MaLop VARCHAR(10),
    @MaGV VARCHAR(10)
AS
BEGIN
    UPDATE LopHoc SET MaGVCN = @MaGV WHERE MaLop = @MaLop;
END;
GO
CREATE PROCEDURE sp_GetLopHocDetails
    @MaLop VARCHAR(10)
AS
BEGIN
    SELECT 
        l.MaLop, l.TenLop, l.Khoi, l.NamHoc, 
        ISNULL(gv.Ten, N'Chưa có') AS TenGVCN,
        l.MaGVCN,
        (SELECT COUNT(*) FROM HocSinh WHERE MaLop = l.MaLop) AS SiSo
    FROM LopHoc l
    LEFT JOIN GiaoVien gv ON l.MaGVCN = gv.MaGV
    WHERE l.MaLop = @MaLop;
END;
GO
CREATE PROCEDURE sp_GetPhanCongGiangDayByLop
    @MaLop VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        m.MaMon, 
        m.TenMon,
        ISNULL(pc.MaGV, '') AS MaGV, 
        ISNULL(gv.Ten, 'Chưa phân công') AS TenGV
    FROM MonHoc m
    LEFT JOIN PhanCongGiangDay pc ON m.MaMon = pc.MaMon AND pc.MaLop = @MaLop
    LEFT JOIN GiaoVien gv ON pc.MaGV = gv.MaGV
    ORDER BY m.TenMon;
END;
GO
CREATE PROCEDURE sp_UpdatePhanCong
    @MaLop VARCHAR(10),
    @MaMon VARCHAR(10),
    @NewMaGV VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        
        DELETE FROM PhanCongGiangDay 
        WHERE MaLop = @MaLop AND MaMon = @MaMon;

        IF @NewMaGV IS NOT NULL AND @NewMaGV != ''
        BEGIN
            IF NOT EXISTS (SELECT 1 FROM GiaoVien_MonHoc WHERE MaGV = @NewMaGV AND MaMon = @MaMon)
            BEGIN
                DECLARE @TenGV NVARCHAR(100), @TenMon NVARCHAR(100);
                SELECT @TenGV = Ten FROM GiaoVien WHERE MaGV = @NewMaGV;
                SELECT @TenMon = TenMon FROM MonHoc WHERE MaMon = @MaMon;
                
                RAISERROR(N'Không thể phân công. Giáo viên [%s] không dạy Môn [%s].', 16, 1, @TenGV, @TenMon);
                ROLLBACK TRANSACTION;
                RETURN;
            END

            INSERT INTO PhanCongGiangDay (MaGV, MaLop, MaMon) 
            VALUES (@NewMaGV, @MaLop, @MaMon);
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END;
GO
CREATE PROCEDURE sp_ImportHocSinhToLop
    @MaLopTarget VARCHAR(10),
    @HocSinhData ut_HocSinhImport READONLY
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT * INTO #TempHocSinh FROM @HocSinhData;

    DECLARE @Skipped INT;
    SELECT @Skipped = COUNT(t.MaHS) 
    FROM #TempHocSinh t
    INNER JOIN HocSinh hs ON t.MaHS = hs.MaHS;

    INSERT INTO HocSinh (MaHS, MaLop, HoTen, NgaySinh, GioiTinh, SDTPhuHuynh, DiaChi, DanToc)
    SELECT 
        t.MaHS, @MaLopTarget, t.HoTen, t.NgaySinh, t.GioiTinh, t.SDTPhuHuynh, t.DiaChi, t.DanToc
    FROM #TempHocSinh t
    WHERE NOT EXISTS (
        SELECT 1 FROM HocSinh hs 
        WHERE hs.MaHS = t.MaHS
    );

    DECLARE @Success INT;
    SET @Success = @@ROWCOUNT;

    SELECT @Success AS [Success], @Skipped AS [Skipped];

    DROP TABLE #TempHocSinh;
END;
GO
CREATE PROCEDURE sp_ImportTKBForGV
    @MaGV VARCHAR(10),
    @NgayList ut_DateList READONLY,
    @TKBData ut_TKBImport READONLY
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        DELETE TKB
        FROM ThoiKhoaBieu TKB
        INNER JOIN @NgayList DL ON CAST(TKB.Ngay AS DATE) = DL.Ngay
        WHERE TKB.MaGV = @MaGV;

        SELECT * INTO #TempTKB FROM @TKBData;

        DECLARE @Failed INT = 0;
        DECLARE @Success INT = 0;

        INSERT INTO ThoiKhoaBieu (MaTKB, Ngay, Tiet, MaMon, MaLop, GhiChu, MauSac, MaGV)
        SELECT
            LEFT(NEWID(), 10),
            t.Ngay, t.Tiet, m.MaMon, l.MaLop,
            NULLIF(t.GhiChu, ''),
            NULLIF(t.MauSac, ''),
            @MaGV
        FROM #TempTKB t
        INNER JOIN MonHoc m ON t.TenMon = m.TenMon
        INNER JOIN LopHoc l ON t.TenLop = l.TenLop;

        SET @Success = @@ROWCOUNT;

        SELECT @Failed = COUNT(*)
        FROM #TempTKB t
        LEFT JOIN MonHoc m ON t.TenMon = m.TenMon
        LEFT JOIN LopHoc l ON t.TenLop = l.TenLop
        WHERE m.MaMon IS NULL OR l.MaLop IS NULL;

        COMMIT TRANSACTION;

        SELECT @Success AS [Success], @Failed AS [Failed];
        DROP TABLE #TempTKB;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        RAISERROR(N'Lỗi import TKB. Đã hoàn tác.', 16, 1);
        SELECT 0 AS [Success], 0 AS [Failed];
        IF OBJECT_ID('tempdb..#TempTKB') IS NOT NULL DROP TABLE #TempTKB;
    END CATCH
END;
GO
CREATE PROCEDURE sp_GetTeacherNameById
    @MaGV VARCHAR(10)
AS
BEGIN
    SELECT Ten FROM GiaoVien WHERE MaGV = @MaGV;
END;
GO
CREATE PROCEDURE sp_GetQuyLopByLop
    @MaLop VARCHAR(10)
AS
BEGIN
    SELECT 
        MaQL,
        Ngay,
        GhiChu,
        Loai,
        SoTien
    FROM QuyLop 
    WHERE MaLop = @MaLop 
    ORDER BY Ngay, MaQL;
END;
GO
CREATE PROCEDURE sp_InsertQuyLop
    @MaLop VARCHAR(10),
    @Loai NVARCHAR(10),
    @SoTien DECIMAL(12,2),
    @Ngay DATE,
    @GhiChu NVARCHAR(200)
AS
BEGIN
    INSERT INTO QuyLop (MaQL, MaLop, Loai, SoTien, Ngay, GhiChu)
    VALUES (LEFT(NEWID(), 10), @MaLop, @Loai, @SoTien, @Ngay, @GhiChu);
END;
GO
CREATE PROCEDURE sp_DeleteQuyLop
    @MaQL VARCHAR(10)
AS
BEGIN
    DELETE FROM QuyLop WHERE MaQL = @MaQL;
END;
GO
-- #endregion

-- SP MỚI (QUẢN LÝ GIÁO VIÊN)
CREATE PROCEDURE sp_GetMonHocByGiaoVien
    @MaGV VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT MaMon FROM GiaoVien_MonHoc WHERE MaGV = @MaGV;
END;
GO

CREATE PROCEDURE sp_UpdateGiaoVien_MonHoc
    @MaGV VARCHAR(10),
    @MonHocList ut_MaMonList READONLY
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        DELETE FROM GiaoVien_MonHoc WHERE MaGV = @MaGV;

        INSERT INTO GiaoVien_MonHoc (MaGV, MaMon)
        SELECT @MaGV, MaMon FROM @MonHocList
        WHERE MaMon IS NOT NULL AND MaMon != '';

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        ;THROW; 
    END CATCH
END;
GO

-- SP MỚI (QUẢN LÝ TRƯỜNG HỌC)
CREATE PROCEDURE sp_InsertMonHoc
    @TenMon NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @NewMaMon VARCHAR(10);
    SET @NewMaMon = UPPER(SUBSTRING(REPLACE(REPLACE(REPLACE(@TenMon, ' ', ''), '(', ''), ')', ''), 1, 10));

    IF EXISTS (SELECT 1 FROM MonHoc WHERE MaMon = @NewMaMon OR TenMon = @TenMon)
    BEGIN
        RAISERROR(N'Mã môn hoặc Tên môn này đã tồn tại.', 16, 1);
        RETURN;
    END
    INSERT INTO MonHoc (MaMon, TenMon) VALUES (@NewMaMon, @TenMon);
END;
GO

CREATE PROCEDURE sp_UpdateMonHoc
    @MaMon VARCHAR(10),
    @TenMon NVARCHAR(100)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM MonHoc WHERE TenMon = @TenMon AND MaMon != @MaMon)
    BEGIN
        RAISERROR(N'Tên môn này đã tồn tại ở một mã môn khác.', 16, 1);
        RETURN;
    END
    UPDATE MonHoc SET TenMon = @TenMon WHERE MaMon = @MaMon;
END;
GO

CREATE PROCEDURE sp_DeleteMonHoc
    @MaMon VARCHAR(10)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM GiaoVien_MonHoc WHERE MaMon = @MaMon) OR
       EXISTS (SELECT 1 FROM PhanCongGiangDay WHERE MaMon = @MaMon) OR
       EXISTS (SELECT 1 FROM KetQuaHocTap WHERE MaMon = @MaMon) OR
       EXISTS (SELECT 1 FROM ThoiKhoaBieu WHERE MaMon = @MaMon)
    BEGIN
        RAISERROR(N'Không thể xóa. Môn học này đang được sử dụng trong bảng phân công, kết quả học tập hoặc TKB.', 16, 1);
        RETURN;
    END
    DELETE FROM MonHoc WHERE MaMon = @MaMon;
END;
GO

CREATE PROCEDURE sp_GetThoiHanDiem
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        MaCotDiem, 
        TenHienThi, 
        Khoi,
        HocKy,
        NgayMoDiem, 
        NgayKhoaDiem, 
        KhoaThuCong,
        CASE 
            WHEN (GETDATE() NOT BETWEEN NgayMoDiem AND NgayKhoaDiem) OR (KhoaThuCong = 1) THEN 1 
            ELSE 0 
        END AS DaKhoa
    FROM ThoiHanDiem
    ORDER BY Khoi, HocKy, NgayMoDiem;
END;
GO

CREATE PROCEDURE sp_UpdateThoiHanDiem
    @MaCotDiem VARCHAR(20),
    @Khoi NVARCHAR(20),
    @HocKy INT,
    @NgayMoDiem DATE,
    @NgayKhoaDiem DATE,
    @KhoaThuCong BIT
AS
BEGIN
    UPDATE ThoiHanDiem
    SET 
        NgayMoDiem = @NgayMoDiem,
        NgayKhoaDiem = @NgayKhoaDiem,
        KhoaThuCong = @KhoaThuCong
    WHERE MaCotDiem = @MaCotDiem
      AND Khoi = @Khoi
      AND HocKy = @HocKy;
END;
GO

CREATE PROCEDURE sp_ProcessStudentPromotion
    @MaLopCu VARCHAR(10),
    @MaLopMoi_LenLop VARCHAR(10),
    @MaLopMoi_OLaiLop VARCHAR(10),
    @IsLop5_TotNghiep BIT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @KhoiCu NVARCHAR(20);
    SELECT @KhoiCu = Khoi FROM LopHoc WHERE MaLop = @MaLopCu;
    
    CREATE TABLE #TempDiemTB (
        MaHS VARCHAR(10) PRIMARY KEY,
        DTB_CaNam FLOAT
    );

    DECLARE @monHocCols NVARCHAR(MAX), 
            @tongMon NVARCHAR(MAX),
            @sql NVARCHAR(MAX);
    DECLARE @monCount INT;

    SELECT @monHocCols = STUFF((SELECT DISTINCT ',' + QUOTENAME(mh.TenMon) 
                                FROM PhanCongGiangDay pcg
                                JOIN MonHoc mh ON pcg.MaMon = mh.MaMon
                                WHERE pcg.MaLop = @MaLopCu
                                FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'),1,1,'');
    SELECT @tongMon = STUFF((SELECT DISTINCT ' + ISNULL(' + QUOTENAME(mh.TenMon) + ', 0)'
                            FROM PhanCongGiangDay pcg
                            JOIN MonHoc mh ON pcg.MaMon = mh.MaMon
                            WHERE pcg.MaLop = @MaLopCu
                            FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'),1,3,'');
    SELECT @monCount = COUNT(DISTINCT MaMon) FROM PhanCongGiangDay WHERE MaLop = @MaLopCu;

    IF @monCount = 0 
    BEGIN
        PRINT N'Lớp cũ không có môn học nào được phân công. Không thể tính điểm.';
        SELECT 0 AS SoHSLenLop, 0 AS SoHSOLaiLop, 0 AS SoHSTotNghiep;
        IF OBJECT_ID('tempdb..#TempDiemTB') IS NOT NULL DROP TABLE #TempDiemTB;
        RETURN;
    END; 

    SET @sql = N'
    ;WITH DiemTB AS (
        SELECT 
            hs.MaHS,
            mh.TenMon,
            AVG(kq.Diem) AS DiemTB
        FROM HocSinh hs
        INNER JOIN PhanCongGiangDay pcg ON hs.MaLop = pcg.MaLop
        INNER JOIN MonHoc mh ON pcg.MaMon = mh.MaMon
        LEFT JOIN KetQuaHocTap kq 
            ON hs.MaHS = kq.MaHS 
            AND mh.MaMon = kq.MaMon 
            AND kq.Loai IN (
                SELECT MaCotDiem 
                FROM ThoiHanDiem 
                WHERE Khoi = @KhoiCu
            )
        WHERE hs.MaLop = @MaLopCu
        GROUP BY hs.MaHS, mh.TenMon
    ),
    PivotData AS (
        SELECT MaHS, ' + @monHocCols + N'
        FROM DiemTB
        PIVOT (AVG(DiemTB) FOR TenMon IN (' + @monHocCols + N')) AS PivotTable
    )
    INSERT INTO #TempDiemTB (MaHS, DTB_CaNam)
    SELECT MaHS, 
           ROUND((' + @tongMon + N') / NULLIF(' + CAST(@monCount AS VARCHAR) + N', 0), 2)
    FROM PivotData;';

    EXEC sp_executesql @sql, N'@MaLopCu VARCHAR(10), @KhoiCu NVARCHAR(20)', @MaLopCu, @KhoiCu;

    CREATE TABLE #PhanLoai (MaHS VARCHAR(10), HanhDong INT);

    INSERT INTO #PhanLoai (MaHS, HanhDong)
    SELECT 
        hs.MaHS,
        CASE WHEN ISNULL(td.DTB_CaNam, 0) >= 5.0 THEN 1 ELSE 0 END
    FROM HocSinh hs
    LEFT JOIN #TempDiemTB td ON hs.MaHS = td.MaHS
    WHERE hs.MaLop = @MaLopCu;

    DECLARE @SoHocSinhLenLop INT = 0;
    DECLARE @SoHocSinhOLaiLop INT = 0;
    DECLARE @SoHocSinhTotNghiep INT = 0;

    UPDATE HocSinh
    SET MaLop = @MaLopMoi_OLaiLop
    WHERE MaHS IN (SELECT MaHS FROM #PhanLoai WHERE HanhDong = 0);
    SET @SoHocSinhOLaiLop = @@ROWCOUNT;

    IF @IsLop5_TotNghiep = 0
    BEGIN
        UPDATE HocSinh
        SET MaLop = @MaLopMoi_LenLop
        WHERE MaHS IN (SELECT MaHS FROM #PhanLoai WHERE HanhDong = 1);
        SET @SoHocSinhLenLop = @@ROWCOUNT;
    END
    ELSE
    BEGIN
        UPDATE HocSinh
        SET MaLop = NULL
        WHERE MaHS IN (SELECT MaHS FROM #PhanLoai WHERE HanhDong = 1);
        SET @SoHocSinhTotNghiep = @@ROWCOUNT;
    END

    SELECT @SoHocSinhLenLop AS SoHSLenLop, 
           @SoHocSinhOLaiLop AS SoHSOLaiLop, 
           @SoHocSinhTotNghiep AS SoHSTotNghiep;

    DROP TABLE #TempDiemTB;
    DROP TABLE #PhanLoai;
END;
GO
GO
PRINT N'Tạo Table Type [ut_MaHSList] để chuyển lớp';
GO
-- Tạo một kiểu dữ liệu bảng để truyền danh sách Mã Học Sinh
CREATE TYPE ut_MaHSList AS TABLE(
    MaHS VARCHAR(10) PRIMARY KEY
);
GO
PRINT N'Tạo SP mới [sp_UpdateHocSinhLop_Multi]';
GO
-- Tạo SP mới để chuyển nhiều học sinh
CREATE PROCEDURE sp_UpdateHocSinhLop_Multi
    @MaHSList ut_MaHSList READONLY,
    @MaLopMoi VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE HocSinh
    SET MaLop = @MaLopMoi
    WHERE MaHS IN (SELECT MaHS FROM @MaHSList);
    
    -- Trả về số lượng học sinh đã được chuyển
    SELECT @@ROWCOUNT AS SoHocSinhDaChuyen;
END;
GO
GO
PRINT N'Tạo SP [sp_Admin_GetBaoCaoChuyenCan]';
GO
CREATE PROCEDURE sp_Admin_GetBaoCaoChuyenCan
    @Khoi NVARCHAR(20) = NULL, -- Ví dụ: 'Khối 1', NULL cho tất cả
    @MaLop VARCHAR(10) = NULL, -- Ưu tiên hơn Khoi nếu được cung cấp
    @HocKy INT -- 1: HK1, 2: HK2, 3 (hoặc khác): Cả năm
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @StartDate DATE, @EndDate DATE;
    DECLARE @NamHocStr VARCHAR(10);
    DECLARE @NamHocStartYear INT;

    -- Xác định năm học (ưu tiên năm học của lớp nếu có)
    IF @MaLop IS NOT NULL
        SELECT @NamHocStr = NamHoc FROM LopHoc WHERE MaLop = @MaLop;

    IF @NamHocStr IS NULL -- Hoặc nếu không có lớp cụ thể, lấy năm học hiện tại
    BEGIN
        DECLARE @CurrentMonth INT = MONTH(GETDATE());
        DECLARE @CurrentYear INT = YEAR(GETDATE());
        IF @CurrentMonth >= 8 
            SET @NamHocStartYear = @CurrentYear;
        ELSE 
            SET @NamHocStartYear = @CurrentYear - 1;
    END
    ELSE
    BEGIN
         -- Lấy năm bắt đầu từ chuỗi NamHoc (vd: '2025' -> 2024)
         SET @NamHocStartYear = CAST(@NamHocStr AS INT) - 1;
    END;

    -- Xác định ngày bắt đầu và kết thúc dựa trên học kỳ
    IF @HocKy = 1 
    BEGIN
        SET @StartDate = DATEFROMPARTS(@NamHocStartYear, 8, 1); -- Tháng 8 năm bắt đầu
        SET @EndDate = DATEFROMPARTS(@NamHocStartYear, 12, 31); -- Tháng 12 năm bắt đầu
    END
    ELSE IF @HocKy = 2 
    BEGIN
        SET @StartDate = DATEFROMPARTS(@NamHocStartYear + 1, 1, 1); -- Tháng 1 năm sau
        SET @EndDate = DATEFROMPARTS(@NamHocStartYear + 1, 5, 31); -- Tháng 5 năm sau
    END
    ELSE -- Cả năm
    BEGIN
        SET @StartDate = DATEFROMPARTS(@NamHocStartYear, 8, 1);
        SET @EndDate = DATEFROMPARTS(@NamHocStartYear + 1, 5, 31);
    END;

    -- Truy vấn dữ liệu
    SELECT 
        hs.MaHS, 
        hs.HoTen,
        lh.TenLop, -- Thêm tên lớp cho Admin
        COUNT(CASE WHEN dd.TrangThai = N'Có mặt' THEN 1 END) as SoBuoiCoMat,
        COUNT(CASE WHEN dd.TrangThai = N'Vắng' THEN 1 END) as SoBuoiVang, -- Vắng không phép
        COUNT(CASE WHEN dd.TrangThai LIKE N'%Có phép%' THEN 1 END) as SoBuoiVangCoPhep,
        COUNT(dd.MaDD) as TongSoBuoi,
        CAST(
            (COUNT(CASE WHEN dd.TrangThai = N'Có mặt' THEN 1 END) * 100.0) / NULLIF(COUNT(dd.MaDD), 0) 
            AS DECIMAL(5,0)
        ) as TyLeChuyenCan
    FROM HocSinh hs
    INNER JOIN LopHoc lh ON hs.MaLop = lh.MaLop
    LEFT JOIN DiemDanh dd ON hs.MaHS = dd.MaHS AND CAST(dd.NgayDD AS DATE) BETWEEN @StartDate AND @EndDate
    WHERE 
        (@MaLop IS NOT NULL AND hs.MaLop = @MaLop) -- Lọc theo lớp cụ thể
        OR 
        (@MaLop IS NULL AND @Khoi IS NOT NULL AND lh.Khoi = @Khoi) -- Lọc theo khối nếu không có lớp
        OR
        (@MaLop IS NULL AND @Khoi IS NULL) -- Không lọc gì cả (toàn trường)
    GROUP BY hs.MaHS, hs.HoTen, lh.TenLop
    ORDER BY lh.TenLop, hs.HoTen;
END;
GO
GO
PRINT N'Tạo SP [sp_Admin_GetBangDiemHocKy]';
GO
CREATE PROCEDURE sp_Admin_GetBangDiemHocKy
    @Khoi NVARCHAR(20) = NULL,
    @MaLop VARCHAR(10) = NULL,
    @HocKy INT -- 1, 2, hoặc 3 (Cả năm)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @loaiFilter NVARCHAR(20);
    DECLARE @khoiFilter NVARCHAR(20) = @Khoi; -- Lưu lại để dùng trong CTE

    -- Xác định các loại điểm cần lấy dựa trên học kỳ
    IF @HocKy = 1 SET @loaiFilter = N'%Ki1';
    ELSE IF @HocKy = 2 SET @loaiFilter = N'%Ki2';
    ELSE SET @loaiFilter = N'%Ki%'; -- Lấy cả 2 kỳ

    -- Nếu MaLop được cung cấp, lấy Khoi từ MaLop đó để lọc đúng ThoiHanDiem
    IF @MaLop IS NOT NULL
    BEGIN
        SELECT @khoiFilter = Khoi FROM LopHoc WHERE MaLop = @MaLop;
    END

    ;WITH RelevantScores AS (
        SELECT 
            kq.MaHS,
            kq.MaMon,
            kq.Diem
        FROM KetQuaHocTap kq
        INNER JOIN HocSinh hs ON kq.MaHS = hs.MaHS
        INNER JOIN LopHoc lh ON hs.MaLop = lh.MaLop
        WHERE kq.Diem IS NOT NULL
          AND kq.Loai IN (SELECT MaCotDiem 
                          FROM ThoiHanDiem thd
                          WHERE thd.MaCotDiem LIKE @loaiFilter 
                            -- Lọc theo khối cụ thể nếu có, nếu không thì lấy tất cả
                            AND (@khoiFilter IS NULL OR thd.Khoi = @khoiFilter)) 
          AND (
                (@MaLop IS NOT NULL AND hs.MaLop = @MaLop)
                OR (@MaLop IS NULL AND @Khoi IS NOT NULL AND lh.Khoi = @Khoi)
                OR (@MaLop IS NULL AND @Khoi IS NULL)
              )
    ),
    AvgMon AS (
        SELECT
            MaHS,
            MaMon,
            AVG(Diem) AS DiemTBMon
        FROM RelevantScores
        GROUP BY MaHS, MaMon
    ),
    AvgCaNhan AS (
         SELECT 
             MaHS,
             AVG(DiemTBMon) AS DiemTBCaNhan
         FROM AvgMon
         GROUP BY MaHS
    )
    SELECT 
        hs.MaHS, 
        hs.HoTen, 
        lh.TenLop,
        ISNULL(acn.DiemTBCaNhan, 0) AS [Trung bình chung] -- Đổi tên cột để khớp C#
    FROM HocSinh hs
    INNER JOIN LopHoc lh ON hs.MaLop = lh.MaLop
    LEFT JOIN AvgCaNhan acn ON hs.MaHS = acn.MaHS
    WHERE 
        (@MaLop IS NOT NULL AND hs.MaLop = @MaLop)
        OR 
        (@MaLop IS NULL AND @Khoi IS NOT NULL AND lh.Khoi = @Khoi)
        OR
        (@MaLop IS NULL AND @Khoi IS NULL)
    ORDER BY lh.TenLop, hs.HoTen;

END;
GO
GO
PRINT N'Tạo SP [sp_Admin_GetHoSoHocSinh]';
GO
CREATE PROCEDURE sp_Admin_GetHoSoHocSinh
    @Khoi NVARCHAR(20) = NULL,
    @MaLop VARCHAR(10) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        hs.MaHS, 
        hs.HoTen, 
        lh.TenLop, -- Thêm tên lớp
        hs.GioiTinh, 
        hs.NgaySinh, 
        hs.DanToc, 
        hs.DiaChi, 
        hs.SDTPhuHuynh
    FROM HocSinh hs
    INNER JOIN LopHoc lh ON hs.MaLop = lh.MaLop
    WHERE 
        (@MaLop IS NOT NULL AND hs.MaLop = @MaLop)
        OR 
        (@MaLop IS NULL AND @Khoi IS NOT NULL AND lh.Khoi = @Khoi)
        OR
        (@MaLop IS NULL AND @Khoi IS NULL)
    ORDER BY lh.TenLop, hs.HoTen;
END;
GO
GO
PRINT N'Sửa đổi SP [sp_GetThongKeKhoi] để hỗ trợ Admin';
GO
Create PROCEDURE sp_GetThongKeKhoi_Admin
    @khoi NVARCHAR(20) = NULL -- Cho phép NULL để lấy toàn trường
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @khoiFilter NVARCHAR(25) = @khoi; -- Giữ nguyên để tương thích

    -- Xác định các loại điểm cần tính TB (lấy tất cả nếu không có khối cụ thể)
    DECLARE @loaiList TABLE (Loai NVARCHAR(20));
    INSERT INTO @loaiList (Loai)
    SELECT MaCotDiem 
    FROM ThoiHanDiem 
    WHERE (@khoiFilter IS NULL OR Khoi = @khoiFilter);

    -- Tính toán sĩ số và giới tính
    WITH StudentCounts AS (
        SELECT
            l.Khoi, -- Thêm cột Khối
            l.MaLop,
            l.TenLop,
            COUNT(hs.MaHS) AS SoHocSinh,
            SUM(CASE WHEN hs.GioiTinh = N'Nam' THEN 1 ELSE 0 END) AS SoNam,
            SUM(CASE WHEN hs.GioiTinh = N'Nữ' THEN 1 ELSE 0 END) AS SoNu
        FROM LopHoc l
        LEFT JOIN HocSinh hs ON l.MaLop = hs.MaLop
        WHERE (@khoiFilter IS NULL OR l.Khoi = @khoiFilter) -- Lọc theo khối nếu có
        GROUP BY l.Khoi, l.MaLop, l.TenLop
    ),
    -- Tính điểm trung bình
    AvgScores AS (
        SELECT
            l.MaLop,
            ROUND(AVG(kq.Diem), 2) AS DiemTrungBinh
        FROM LopHoc l
        LEFT JOIN HocSinh hs ON l.MaLop = hs.MaLop
        LEFT JOIN KetQuaHocTap kq ON hs.MaHS = kq.MaHS
        WHERE (@khoiFilter IS NULL OR l.Khoi = @khoiFilter) 
          AND kq.Diem IS NOT NULL
          AND kq.Loai IN (SELECT Loai FROM @loaiList) -- Chỉ tính điểm trong danh sách loại hợp lệ
        GROUP BY l.MaLop
    )
    -- Kết hợp kết quả
    SELECT
        sc.Khoi, -- Trả về cột Khối
        sc.TenLop,
        sc.SoHocSinh,
        ISNULL(av.DiemTrungBinh, 0) AS DiemTrungBinh,
        sc.SoNam,
        sc.SoNu
    FROM StudentCounts sc
    LEFT JOIN AvgScores av ON sc.MaLop = av.MaLop
    ORDER BY sc.Khoi, sc.TenLop; -- Sắp xếp theo Khối rồi đến Tên lớp
END;
GO

GO
CREATE PROCEDURE sp_GetMonthlyScoreTypes
    @MaLop VARCHAR(10),
    @HocKy INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Khoi NVARCHAR(20);
    SELECT @Khoi = Khoi FROM LopHoc WHERE MaLop = @MaLop;

    IF @Khoi IS NULL
    BEGIN
        SELECT TOP 0 '' AS MaCotDiem, '' AS TenHienThi;
        RETURN;
    END

    SELECT MaCotDiem, TenHienThi 
    FROM ThoiHanDiem
    WHERE Khoi = @Khoi
      AND HocKy = @HocKy
      AND MaCotDiem LIKE 'Thang%'
    ORDER BY NgayMoDiem;
END;
GO
PRINT N'Tạo SP [sp_GetBaoCaoThang_ThongKe]';
GO
CREATE PROCEDURE sp_GetBaoCaoThang_ThongKe
    @MaLop VARCHAR(10),
    @MaMon VARCHAR(10),
    @LoaiDiem VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Lấy dữ liệu thô cho lớp/môn/loại điểm cụ thể
    ;WITH RawData AS (
        SELECT 
            hs.GioiTinh,
            hs.DanToc,
            kq.Diem
        FROM KetQuaHocTap kq
        JOIN HocSinh hs ON kq.MaHS = hs.MaHS
        WHERE hs.MaLop = @MaLop
          AND kq.MaMon = @MaMon
          AND kq.Loai = @LoaiDiem
          AND kq.Diem IS NOT NULL
    ),
    -- 2. Phân loại điểm và các thuộc tính
    ClassifiedData AS (
        SELECT
            CASE 
                WHEN Diem = 10 THEN '10'
                WHEN Diem >= 9 AND Diem < 10 THEN '9'
                WHEN Diem >= 8 AND Diem < 9 THEN '8'
                WHEN Diem >= 7 AND Diem < 8 THEN '7'
                WHEN Diem >= 6 AND Diem < 7 THEN '6'
                WHEN Diem >= 5 AND Diem < 6 THEN '5'
                ELSE '<5'
            END AS NhomDiem,
            CASE 
                WHEN Diem >= 7 THEN 'T'
                WHEN Diem >= 5 THEN 'H'
                ELSE 'C'
            END AS XepLoai,
            CASE WHEN GioiTinh = N'Nữ' THEN 1 ELSE 0 END AS IsNu,
            CASE WHEN DanToc IS NOT NULL AND DanToc != N'Kinh' THEN 1 ELSE 0 END AS IsDanTocThieuSo,
            CASE WHEN GioiTinh = N'Nữ' AND (DanToc IS NOT NULL AND DanToc != N'Kinh') THEN 1 ELSE 0 END AS IsNuDanTocThieuSo
        FROM RawData
    )
    -- 3. Tổng hợp cho Bảng Điểm
    SELECT 
        'Diem' AS LoaiThongKe,
        NhomDiem AS PhanLoai,
        COUNT(*) AS TS,
        SUM(IsNu) AS Nu,
        SUM(IsDanTocThieuSo) AS DanToc,
        SUM(IsNuDanTocThieuSo) AS NDT
    FROM ClassifiedData
    GROUP BY NhomDiem

    UNION ALL

    -- 4. Tổng hợp cho Bảng Xếp Loại
    SELECT 
        'XepLoai' AS LoaiThongKe,
        XepLoai AS PhanLoai,
        COUNT(*) AS TS,
        SUM(IsNu) AS Nu,
        SUM(IsDanTocThieuSo) AS DanToc,
        SUM(IsNuDanTocThieuSo) AS NDT
    FROM ClassifiedData
    GROUP BY XepLoai;
END;
GO
CREATE PROCEDURE sp_Admin_GetBaoCaoThang_ThongKe
    @Khoi NVARCHAR(20) = NULL, -- "Khối 5"
    @MaMon VARCHAR(10),
    @LoaiDiem VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Lấy dữ liệu thô (giống SP cũ nhưng lọc theo @Khoi thay vì @MaLop)
    ;WITH RawData AS (
        SELECT 
            hs.GioiTinh,
            hs.DanToc,
            kq.Diem
        FROM KetQuaHocTap kq
        JOIN HocSinh hs ON kq.MaHS = hs.MaHS
        JOIN LopHoc lh ON hs.MaLop = lh.MaLop -- Cần Join LopHoc để lọc theo Khối
        WHERE 
            (@Khoi IS NULL OR lh.Khoi = @Khoi) -- Lọc theo Khối (hoặc toàn trường nếu NULL)
          AND kq.MaMon = @MaMon
          AND kq.Loai = @LoaiDiem
          AND kq.Diem IS NOT NULL
    ),
    -- 2. Phân loại
    ClassifiedData AS (
        SELECT
            CASE 
                WHEN Diem = 10 THEN '10'
                WHEN Diem >= 9 AND Diem < 10 THEN '9'
                WHEN Diem >= 8 AND Diem < 9 THEN '8'
                WHEN Diem >= 7 AND Diem < 8 THEN '7'
                WHEN Diem >= 6 AND Diem < 7 THEN '6'
                WHEN Diem >= 5 AND Diem < 6 THEN '5'
                ELSE N'Dưới 5' -- Thay đổi theo hình mới
            END AS NhomDiem,
            CASE 
                WHEN Diem >= 7 THEN 'T'
                WHEN Diem >= 5 THEN 'H'
                ELSE 'C'
            END AS XepLoai,
            CASE WHEN GioiTinh = N'Nữ' THEN 1 ELSE 0 END AS IsNu,
            CASE WHEN DanToc IS NOT NULL AND DanToc != N'Kinh' THEN 1 ELSE 0 END AS IsDanTocThieuSo,
            CASE WHEN GioiTinh = N'Nữ' AND (DanToc IS NOT NULL AND DanToc != N'Kinh') THEN 1 ELSE 0 END AS IsNuDanTocThieuSo
        FROM RawData
    ),
    -- 3. Tổng hợp Bảng Điểm
    DiemStats AS (
        SELECT 
            'Diem' AS LoaiThongKe,
            NhomDiem AS PhanLoai,
            COUNT(*) AS TS,
            SUM(IsNu) AS Nu,
            SUM(IsDanTocThieuSo) AS DanToc,
            SUM(IsNuDanTocThieuSo) AS NDT,
            CAST(NULL AS FLOAT) AS TyLe -- Cột này chỉ dùng cho Xếp Loại
        FROM ClassifiedData
        GROUP BY NhomDiem
    ),
    -- 4. Tổng hợp Bảng Xếp Loại
    XepLoaiStats AS (
        SELECT 
            'XepLoai' AS LoaiThongKe,
            XepLoai AS PhanLoai,
            COUNT(*) AS TS,
            NULL AS Nu, -- Không cần cho Xếp Loại
            NULL AS DanToc, -- Không cần
            NULL AS NDT, -- Không cần
            -- Tính tỷ lệ %
            CAST( (COUNT(*) * 100.0) / NULLIF((SELECT COUNT(*) FROM RawData), 0) AS DECIMAL(5, 1)) AS TyLe
        FROM ClassifiedData
        GROUP BY XepLoai
    )
    -- 5. Kết hợp
    SELECT * FROM DiemStats
    UNION ALL
    SELECT * FROM XepLoaiStats;
END;
GO
PRINT 'TẤT CẢ STORED PROCEDURES ĐÃ ĐƯỢC TẠO.';
PRINT 'QUÁ TRÌNH TÁI TẠO HOÀN TẤT!';