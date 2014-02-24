CREATE TABLE [dbo].[Podyaki]
(
	[Id] INT NOT NULL PRIMARY KEY identity,
	[UserId] nvarchar(20) not null references Users(UserId),
	[PostId] nvarchar(50) not null references Posts(PostId),
	[Time] DATETIME NOT NULL, 
    [Avtoritet] FLOAT NOT NULL DEFAULT 0,
)
