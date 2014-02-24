CREATE TABLE [dbo].[DailyPosts]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [PostId] nvarchar(50) not NULL references Posts(PostId)
)
