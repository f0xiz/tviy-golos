CREATE TABLE [dbo].[PodyakaLog]
(
	[Id] INT NOT NULL PRIMARY KEY identity, 
    [FromUserId] nvarchar(20) NOT NULL, 
	[ToUserId] nvarchar(20) NOT NULL,
    [PostId] nvarchar(50) NOT NULL, 
    [IsLike] BIT NOT NULL DEFAULT (1), 
	[Time] DATETIME NOT NULL DEFAULT GETDATE(),

    [IsDone] BIT NOT NULL DEFAULT (0), 
)
