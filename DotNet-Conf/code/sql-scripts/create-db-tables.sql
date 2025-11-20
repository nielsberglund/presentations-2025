IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'MCPTestDB')
BEGIN
    DROP DATABASE MCPTestDB;
END

CREATE DATABASE MCPTestDB;
GO


SELECT * FROM sys.databases;
GO


USE MCPTestDB;
GO

CREATE TABLE tb_Employee (
    EmployeeID INT PRIMARY KEY,
    FirstName NVARCHAR(50),
    LastName NVARCHAR(50),
    HireDate DATE
);

SELECT * FROM tb_Employee;

SELECT * FROM sys.tables;


SELECT COLUMN_NAME as name, DATA_TYPE as type 
FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'dbo.tb_Employee';