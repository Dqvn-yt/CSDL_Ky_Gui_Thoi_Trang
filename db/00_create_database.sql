USE master;
GO

IF DB_ID(N'QuanLyKyGuiThoiTrang') IS NOT NULL
BEGIN
    ALTER DATABASE [QuanLyKyGuiThoiTrang] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [QuanLyKyGuiThoiTrang];
END
GO

CREATE DATABASE [QuanLyKyGuiThoiTrang]
ON PRIMARY
(
    NAME = N'QuanLyKyGuiThoiTrang',
    FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL17.MSSQLSERVER\MSSQL\DATA\QuanLyKyGuiThoiTrang.mdf',
    SIZE = 16MB,
    FILEGROWTH = 8MB
)
LOG ON
(
    NAME = N'QuanLyKyGuiThoiTrang_log',
    FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL17.MSSQLSERVER\MSSQL\DATA\QuanLyKyGuiThoiTrang_log.ldf',
    SIZE = 8MB,
    FILEGROWTH = 8MB
)
COLLATE Vietnamese_CI_AS;
GO

ALTER DATABASE [QuanLyKyGuiThoiTrang] SET RECOVERY SIMPLE;
GO

USE [QuanLyKyGuiThoiTrang];
GO

PRINT N'Database QuanLyKyGuiThoiTrang created.';
GO
