CREATE TABLE [dbo].[Posts]
(
	[PostId] nvarchar(50) NOT NULL PRIMARY KEY, 
    [UserId] nvarchar(20) Not NULL references Users(UserId), 
    [PodyakiCount] INT NOT NULL DEFAULT 0
)
