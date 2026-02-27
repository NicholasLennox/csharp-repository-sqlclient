USE master;
GO
 
-- Drop database if it exists
IF DB_ID('CompanyDb') IS NOT NULL
BEGIN
    ALTER DATABASE CompanyDb
    SET SINGLE_USER
    WITH ROLLBACK IMMEDIATE;
 
    DROP DATABASE CompanyDb;
END
GO
 
-- Create database
CREATE DATABASE CompanyDb;
GO
 
USE CompanyDb;
GO
 
-- Create Employees table
CREATE TABLE Employees
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL,
    Salary DECIMAL(10,2) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1
);
GO
 
-- Insert sample data
INSERT INTO Employees (Name, Salary, IsActive)
VALUES
('Lebo Mokoena', 55000.00, 1),
('Anna Johansen', 62000.00, 1),
('Jonas Berg', 48000.00, 1),
('Maria Ndlovu', 71000.00, 0);
GO