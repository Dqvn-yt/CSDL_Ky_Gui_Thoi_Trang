USE [master];
GO
SET NOCOUNT ON;
GO

/* ============================================================
   HƯỚNG DẪN RESTORE FILE .BAK
   - Chạy trên SQL Server 2025 (bản backup 17.0).
   - Sửa @BackupFile nếu đặt file ở đường dẫn khác.
   - Script sẽ xóa database cùng tên nếu đang tồn tại.
   ============================================================ */

DECLARE @BackupFile NVARCHAR(400) =
    N'E:\Quanap\bak\QuanLyKyGuiThoiTrang_Full_20261115.bak';

IF DB_ID(N'QuanLyKyGuiThoiTrang') IS NOT NULL
BEGIN
    ALTER DATABASE [QuanLyKyGuiThoiTrang] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [QuanLyKyGuiThoiTrang];
END

RESTORE DATABASE [QuanLyKyGuiThoiTrang]
FROM DISK = @BackupFile
WITH
    MOVE N'QuanLyKyGuiThoiTrang'
        TO N'C:\Program Files\Microsoft SQL Server\MSSQL17.MSSQLSERVER\MSSQL\DATA\QuanLyKyGuiThoiTrang.mdf',
    MOVE N'QuanLyKyGuiThoiTrang_log'
        TO N'C:\Program Files\Microsoft SQL Server\MSSQL17.MSSQLSERVER\MSSQL\DATA\QuanLyKyGuiThoiTrang_log.ldf',
    REPLACE, RECOVERY, CHECKSUM;
GO

ALTER DATABASE [QuanLyKyGuiThoiTrang] SET RECOVERY SIMPLE;
GO

PRINT N'Restore thành công. Kiểm tra nhanh:';
SELECT COUNT(*) AS SoSanPham FROM [QuanLyKyGuiThoiTrang].dbo.SAN_PHAM;
SELECT COUNT(*) AS SoDonHang FROM [QuanLyKyGuiThoiTrang].dbo.DON_HANG;
GO
