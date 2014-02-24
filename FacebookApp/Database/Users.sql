CREATE TABLE [dbo].[Users]
(
	[UserId] nvarchar(20) NOT NULL PRIMARY KEY, 
    [PodyakiCount] INT NOT NULL DEFAULT 0, 
    [Avtoritet] FLOAT NOT NULL DEFAULT 0, 
    [IsInPeopleVouting] BIT NOT NULL DEFAULT 0, 
    [Name] NVARCHAR(MAX) NOT NULL , 
    [PictureScr] NVARCHAR(MAX) NOT NULL , 
    [LastActivity] DATETIME NOT NULL, 
    [Link] NVARCHAR(MAX) NOT NULL DEFAULT '', 
    [IsAdmin] BIT NOT NULL DEFAULT 0, 
    [IsVIP] BIT NOT NULL DEFAULT 0, 
)
