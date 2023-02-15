CREATE DATABASE GameDb
GO
USE GameDb

CREATE TABLE Players(
[Id] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
[Name] NVARCHAR(30) NOT NULL,
Score FLOAT NOT NULL,
IsStar BIT NOT NULL DEFAULT(0)
)
GO
INSERT INTO Players([Name],[Score],[IsStar])
VALUES('Lebron',99,1),
('Stephan Curry',95,1),
('Maykl Jordan',67,0)

CREATE PROCEDURE ShowPlayersGreaterThanScore
(@pScore AS FLOAT)
AS
SELECT * FROM Players
WHERE Score>@pScore


EXEC ShowPlayersGreaterThanScore 10
