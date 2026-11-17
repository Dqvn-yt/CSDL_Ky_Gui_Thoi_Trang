USE [QuanLyKyGuiThoiTrang];
GO
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

/* ============================================================
   vw_SanPham_DayDu - tra cứu sản phẩm kèm người ký gửi
   ============================================================ */
CREATE OR ALTER VIEW dbo.vw_SanPham_DayDu
AS
SELECT
    sp.MaSP,
    sp.TenSP,
    dm.MaDM,
    dm.TenDanhMuc,
    th.MaTH,
    th.TenThuongHieu,
    sp.KichThuoc,
    sp.MauSac,
    sp.ChatLieu,
    sp.TinhTrang,
    sp.GiaDeXuat,
    sp.GiaDangBan,
    sp.TrangThaiTiepNhan,
    sp.TrangThaiBan,
    sp.NgayDangBan,
    pkg.MaPKG,
    pkg.NgayKyGui,
    pkg.TrangThai AS TrangThaiPhieu,
    kh.MaKH,
    kh.HoTen AS TenNguoiKyGui,
    kh.SoDienThoai AS DienThoaiNguoiKyGui,
    nv.HoTen AS TenNhanVienTiepNhan
FROM dbo.SAN_PHAM sp
JOIN dbo.DANH_MUC dm ON dm.MaDM = sp.MaDM
LEFT JOIN dbo.THUONG_HIEU th ON th.MaTH = sp.MaTH
JOIN dbo.PHIEU_KY_GUI pkg ON pkg.MaPKG = sp.MaPKG
JOIN dbo.KHACH_HANG kh ON kh.MaKH = pkg.MaKH
JOIN dbo.NHAN_VIEN nv ON nv.MaNV = pkg.MaNV;
GO

/* ============================================================
   vw_DonHang_TongHop - tổng tiền, đã thu, còn lại của đơn
   ============================================================ */
CREATE OR ALTER VIEW dbo.vw_DonHang_TongHop
AS
SELECT
    d.MaDH,
    d.NgayLap,
    d.HanThanhToan,
    d.TrangThai,
    d.LyDoHuy,
    d.GhiChu,
    kh.MaKH,
    kh.HoTen AS TenKhachHang,
    kh.SoDienThoai AS DienThoaiKhachHang,
    nv.MaNV,
    nv.HoTen AS TenNhanVien,
    ISNULL(ct.SoSanPham, 0) AS SoSanPham,
    ISNULL(ct.TongTien, 0) AS TongTien,
    ISNULL(tt.DaThu, 0) AS DaThu,
    ISNULL(ct.TongTien, 0) - ISNULL(tt.DaThu, 0) AS ConLai,
    ISNULL(tt.SoGiaoDichThanhCong, 0) AS SoGiaoDichThanhCong,
    CASE
        WHEN d.TrangThai = N'Chờ thanh toán' AND d.HanThanhToan < SYSDATETIME() THEN N'Quá hạn'
        WHEN d.TrangThai = N'Chờ thanh toán' THEN N'Trong hạn'
        ELSE N'Không áp dụng'
    END AS TinhTrangHan
FROM dbo.DON_HANG d
JOIN dbo.KHACH_HANG kh ON kh.MaKH = d.MaKH
JOIN dbo.NHAN_VIEN nv ON nv.MaNV = d.MaNV
OUTER APPLY
(
    SELECT COUNT(*) AS SoSanPham,
           SUM(c.GiaBanThucTe) AS TongTien
    FROM dbo.CHI_TIET_DON_HANG c
    WHERE c.MaDH = d.MaDH
) ct
OUTER APPLY
(
    SELECT SUM(t.SoTien) AS DaThu,
           COUNT(*) AS SoGiaoDichThanhCong
    FROM dbo.THANH_TOAN_DON_HANG t
    WHERE t.MaDH = d.MaDH AND t.TrangThai = N'Thành công'
) tt;
GO

/* ============================================================
   vw_DoanhThu_TheoThang - doanh thu từ đơn đã hoàn tất
   ============================================================ */
CREATE OR ALTER VIEW dbo.vw_DoanhThu_TheoThang
AS
SELECT
    FORMAT(d.NgayLap, 'yyyy-MM') AS Thang,
    COUNT(DISTINCT d.MaDH) AS SoDon,
    COUNT(*) AS SoSanPham,
    SUM(c.GiaBanThucTe) AS DoanhThu,
    SUM(CAST(ROUND(c.GiaBanThucTe * 0.20, 0) AS DECIMAL(12,0))) AS TienHoaHong,
    SUM(c.GiaBanThucTe - CAST(ROUND(c.GiaBanThucTe * 0.20, 0) AS DECIMAL(12,0))) AS TienTraNguoiKyGui
FROM dbo.DON_HANG d
JOIN dbo.CHI_TIET_DON_HANG c ON c.MaDH = d.MaDH
WHERE d.TrangThai = N'Hoàn tất'
GROUP BY FORMAT(d.NgayLap, 'yyyy-MM');
GO

/* ============================================================
   vw_CongNo_NguoiKyGui - tiền bán chưa quyết toán cho khách
   ============================================================ */
CREATE OR ALTER VIEW dbo.vw_CongNo_NguoiKyGui
AS
SELECT
    kh.MaKH,
    kh.HoTen,
    kh.SoDienThoai,
    COUNT(*) AS SoSanPhamChuaQuyetToan,
    SUM(c.GiaBanThucTe) AS TongGiaBan,
    SUM(CAST(ROUND(c.GiaBanThucTe * 0.20, 0) AS DECIMAL(12,0))) AS TienHoaHong,
    SUM(c.GiaBanThucTe - CAST(ROUND(c.GiaBanThucTe * 0.20, 0) AS DECIMAL(12,0))) AS TienPhaiTra
FROM dbo.CHI_TIET_DON_HANG c
JOIN dbo.DON_HANG d ON d.MaDH = c.MaDH AND d.TrangThai = N'Hoàn tất'
JOIN dbo.SAN_PHAM sp ON sp.MaSP = c.MaSP
JOIN dbo.PHIEU_KY_GUI pkg ON pkg.MaPKG = sp.MaPKG
JOIN dbo.KHACH_HANG kh ON kh.MaKH = pkg.MaKH
LEFT JOIN dbo.CHI_TIET_THANH_TOAN_KY_GUI ct ON ct.MaCTDH = c.MaCTDH
WHERE ct.MaCTTTKG IS NULL
GROUP BY kh.MaKH, kh.HoTen, kh.SoDienThoai;
GO

/* ============================================================
   vw_TonKho_TheoTrangThai - tồn kho theo danh mục/trạng thái
   ============================================================ */
CREATE OR ALTER VIEW dbo.vw_TonKho_TheoTrangThai
AS
SELECT
    dm.MaDM,
    dm.TenDanhMuc,
    sp.TrangThaiTiepNhan,
    ISNULL(sp.TrangThaiBan, N'(Chưa có)') AS TrangThaiBan,
    COUNT(*) AS SoLuong,
    ISNULL(SUM(sp.GiaDangBan), 0) AS TongGiaTriNiemYet
FROM dbo.SAN_PHAM sp
JOIN dbo.DANH_MUC dm ON dm.MaDM = sp.MaDM
GROUP BY dm.MaDM, dm.TenDanhMuc, sp.TrangThaiTiepNhan, sp.TrangThaiBan;
GO

/* ============================================================
   vw_PhieuKyGui_TienDo - tiến độ xử lý của phiếu ký gửi
   ============================================================ */
CREATE OR ALTER VIEW dbo.vw_PhieuKyGui_TienDo
AS
SELECT
    p.MaPKG,
    p.NgayKyGui,
    p.TrangThai,
    p.GhiChu,
    kh.MaKH,
    kh.HoTen AS TenNguoiKyGui,
    kh.SoDienThoai,
    nv.HoTen AS TenNhanVien,
    COUNT(sp.MaSP) AS TongSanPham,
    SUM(CASE WHEN sp.TrangThaiTiepNhan = N'Chờ xử lý' THEN 1 ELSE 0 END) AS ChoDuyet,
    SUM(CASE WHEN sp.TrangThaiTiepNhan = N'Đã chấp nhận' THEN 1 ELSE 0 END) AS DaChapNhan,
    SUM(CASE WHEN sp.TrangThaiTiepNhan = N'Từ chối' THEN 1 ELSE 0 END) AS TuChoi,
    SUM(CASE WHEN sp.TrangThaiBan = N'Chưa đăng bán' THEN 1 ELSE 0 END) AS ChuaDangBan,
    SUM(CASE WHEN sp.TrangThaiBan = N'Đang bán' THEN 1 ELSE 0 END) AS DangBan,
    SUM(CASE WHEN sp.TrangThaiBan = N'Đang giữ' THEN 1 ELSE 0 END) AS DangGiu,
    SUM(CASE WHEN sp.TrangThaiBan = N'Đã bán' THEN 1 ELSE 0 END) AS DaBan,
    SUM(CASE WHEN sp.TrangThaiBan = N'Đã trả lại' THEN 1 ELSE 0 END) AS DaTraLai
FROM dbo.PHIEU_KY_GUI p
JOIN dbo.KHACH_HANG kh ON kh.MaKH = p.MaKH
JOIN dbo.NHAN_VIEN nv ON nv.MaNV = p.MaNV
LEFT JOIN dbo.SAN_PHAM sp ON sp.MaPKG = p.MaPKG
GROUP BY p.MaPKG, p.NgayKyGui, p.TrangThai, p.GhiChu, kh.MaKH, kh.HoTen, kh.SoDienThoai, nv.HoTen;
GO

/* ============================================================
   vw_DoiSoat_ThanhToan - đối soát trả thiếu / đủ / thừa
   ============================================================ */
CREATE OR ALTER VIEW dbo.vw_DoiSoat_ThanhToan
AS
SELECT
    v.MaDH,
    v.NgayLap,
    v.MaKH,
    v.TenKhachHang,
    v.TrangThai,
    v.TongTien,
    v.DaThu,
    v.ConLai,
    v.DaThu - v.TongTien AS ChenhLech,
    CASE
        WHEN v.DaThu = v.TongTien THEN N'Đủ'
        WHEN v.DaThu < v.TongTien THEN N'Trả thiếu'
        ELSE N'Trả thừa'
    END AS KetQuaDoiSoat
FROM dbo.vw_DonHang_TongHop v
WHERE v.TrangThai <> N'Đã hủy';
GO

/* ============================================================
   vw_HieuSuat_NhanVien - hiệu suất làm việc của nhân viên
   ============================================================ */
CREATE OR ALTER VIEW dbo.vw_HieuSuat_NhanVien
AS
SELECT
    nv.MaNV,
    nv.HoTen,
    nv.ChucVu,
    nv.TrangThai,
    ISNULL(pkg.SoPhieuKyGui, 0) AS SoPhieuKyGui,
    ISNULL(pkgs.SoSanPhamTiepNhan, 0) AS SoSanPhamTiepNhan,
    ISNULL(dh.SoDonHang, 0) AS SoDonHang,
    ISNULL(dh.DoanhThu, 0) AS DoanhThu,
    ISNULL(pt.SoPhieuQuyetToan, 0) AS SoPhieuQuyetToan
FROM dbo.NHAN_VIEN nv
OUTER APPLY
(
    SELECT COUNT(*) AS SoPhieuKyGui
    FROM dbo.PHIEU_KY_GUI p
    WHERE p.MaNV = nv.MaNV
) pkg
OUTER APPLY
(
    SELECT COUNT(*) AS SoSanPhamTiepNhan
    FROM dbo.SAN_PHAM sp
    JOIN dbo.PHIEU_KY_GUI p ON p.MaPKG = sp.MaPKG
    WHERE p.MaNV = nv.MaNV
) pkgs
OUTER APPLY
(
    SELECT COUNT(*) AS SoDonHang,
           ISNULL(SUM(CASE WHEN x.TrangThai = N'Hoàn tất' THEN x.TongTien ELSE 0 END), 0) AS DoanhThu
    FROM
    (
        SELECT d.MaDH,
               d.TrangThai,
               (SELECT ISNULL(SUM(c.GiaBanThucTe), 0) FROM dbo.CHI_TIET_DON_HANG c WHERE c.MaDH = d.MaDH) AS TongTien
        FROM dbo.DON_HANG d
        WHERE d.MaNV = nv.MaNV
    ) x
) dh
OUTER APPLY
(
    SELECT COUNT(*) AS SoPhieuQuyetToan
    FROM dbo.PHIEU_THANH_TOAN_KY_GUI ptt
    WHERE ptt.MaNV = nv.MaNV
) pt;
GO

/* ============================================================
   vw_QuyetToan_SanPham - mọi sản phẩm đã bán kèm trạng thái quyết toán
   ============================================================ */
CREATE OR ALTER VIEW dbo.vw_QuyetToan_SanPham
AS
SELECT
    c.MaCTDH,
    d.MaDH,
    d.NgayLap,
    sp.MaSP,
    sp.TenSP,
    sp.KichThuoc,
    sp.MauSac,
    kh.MaKH,
    kh.HoTen AS TenNguoiKyGui,
    kh.SoDienThoai,
    c.GiaBanThucTe AS GiaBan,
    CAST(ROUND(c.GiaBanThucTe * 0.20, 0) AS DECIMAL(12,0)) AS TienHoaHong,
    c.GiaBanThucTe - CAST(ROUND(c.GiaBanThucTe * 0.20, 0) AS DECIMAL(12,0)) AS TienThucNhan,
    ct.MaCTTTKG,
    ptt.MaPTTKG,
    ISNULL(ptt.TrangThai, N'Chưa quyết toán') AS TrangThaiQuyetToan,
    ptt.NgayLap AS NgayLapPhieu,
    ptt.NgayThanhToan
FROM dbo.CHI_TIET_DON_HANG c
JOIN dbo.DON_HANG d ON d.MaDH = c.MaDH AND d.TrangThai = N'Hoàn tất'
JOIN dbo.SAN_PHAM sp ON sp.MaSP = c.MaSP
JOIN dbo.PHIEU_KY_GUI pkg ON pkg.MaPKG = sp.MaPKG
JOIN dbo.KHACH_HANG kh ON kh.MaKH = pkg.MaKH
LEFT JOIN dbo.CHI_TIET_THANH_TOAN_KY_GUI ct ON ct.MaCTDH = c.MaCTDH
LEFT JOIN dbo.PHIEU_THANH_TOAN_KY_GUI ptt ON ptt.MaPTTKG = ct.MaPTTKG;
GO

PRINT N'Views created: 9 views.';
GO
