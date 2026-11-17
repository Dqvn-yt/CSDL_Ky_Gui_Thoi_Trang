USE [master];
GO
SET NOCOUNT ON;
GO

DECLARE @BackupFile NVARCHAR(400) =
    N'C:\Program Files\Microsoft SQL Server\MSSQL17.MSSQLSERVER\MSSQL\Backup\QuanLyKyGuiThoiTrang_Full_20261115.bak';

BACKUP DATABASE [QuanLyKyGuiThoiTrang]
TO DISK = @BackupFile
WITH INIT, COMPRESSION, CHECKSUM, STATS = 10,
     NAME = N'QuanLyKyGuiThoiTrang - Full backup 2026-11-15';
GO

DECLARE @BackupFile NVARCHAR(400) =
    N'C:\Program Files\Microsoft SQL Server\MSSQL17.MSSQLSERVER\MSSQL\Backup\QuanLyKyGuiThoiTrang_Full_20261115.bak';

RESTORE VERIFYONLY
FROM DISK = @BackupFile
WITH CHECKSUM;
GO

PRINT N'Backup created and verified.';
GO
