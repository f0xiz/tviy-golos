CREATE TABLE [dbo].[Vote]
(
	[Id] INT NOT NULL PRIMARY KEY identity,
	UserId nvarchar(20) not null references Users(UserId),
	VotingVariantId int not null references VotingVariants(Id),
	Score float not null DEFAULT 0
)
