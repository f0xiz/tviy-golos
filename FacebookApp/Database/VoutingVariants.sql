CREATE TABLE [dbo].[VotingVariants]
(
	[Id] INT NOT NULL PRIMARY KEY identity, 
    [Text] NVARCHAR(MAX) NOT NULL, 
    [Score] FLOAT NOT NULL, 
    [UserId] nvarchar(20) NULL references Users(UserId), 
    [VotingId] INT NOT NULL references Votings(Id),

)
