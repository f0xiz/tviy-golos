CREATE TABLE [dbo].[Promote]
(
	[Id] INT NOT NULL PRIMARY KEY identity, 
    [PostId] nvarchar(50) NOT NULL, 
    [UserId] nvarchar(20) NOT NULL references Users(UserId)
	
)
