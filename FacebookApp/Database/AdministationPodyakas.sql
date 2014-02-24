CREATE TABLE [dbo].[AdministationPodyakas]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [UserId] nvarchar(20) not NULL references Users(UserId), 
    [Score] FLOAT NOT NULL, 
    [Time] DATETIME NOT NULL, 
    [Description] NVARCHAR(MAX) NULL,

)
