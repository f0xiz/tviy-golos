CREATE TABLE [dbo].[Votings]
(
	[Id] INT NOT NULL PRIMARY KEY identity, 
    [Name] NVARCHAR(MAX) NOT NULL, 
    [Description] NVARCHAR(MAX) NOT NULL, 
    [IsPeople] BIT NOT NULL DEFAULT 0, 
    [IsClosed] BIT NOT NULL DEFAULT 0,
)
