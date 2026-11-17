USE master;
IF DB_ID(N'QuanLyKyGuiThoiTrang_RestoreTest') IS NOT NULL
BEGIN
    ALTER DATABASE [QuanLyKyGuiThoiTrang_RestoreTest] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [QuanLyKyGuiThoiTrang_RestoreTest];
END
RESTORE DATABASE [QuanLyKyGuiThoiTrang_RestoreTest]
FROM DISK = N'C:\Program Files\Microsoft SQL Server\MSSQL17.MSSQLSERVER\MSSQL\Backup\QuanLyKyGuiThoiTrang_Full_20261115.bak'
WITH MOVE N'QuanLyKyGuiThoiTrang' TO N'C:\Program Files\Microsoft SQL Server\MSSQL17.MSSQLSERVER\MSSQL\DATA\QuanLyKyGuiThoiTrang_RestoreTest.mdf',
     MOVE N'QuanLyKyGuiThoiTrang_log' TO N'C:\Program Files\Microsoft SQL Server\MSSQL17.MSSQLSERVER\MSSQL\DATA\QuanLyKyGuiThoiTrang_RestoreTest_log.ldf',
     REPLACE, RECOVERY, CHECKSUM;
SELECT COUNT(*) AS SoSanPham FROM [QuanLyKyGuiThoiTrang_RestoreTest].dbo.SAN_PHAM;
SELECT COUNT(*) AS SoDonHang FROM [QuanLyKyGuiThoiTrang_RestoreTest].dbo.DON_HANG;
SELECT COUNT(*) AS SoTrigger FROM [QuanLyKyGuiThoiTrang_RestoreTest].sys.triggers WHERE is_disabled = 0;
