CREATE TABLE [dbo].[UserFriends]
(
	[Id] INT NOT NULL PRIMARY KEY identity,
	UserId nvarchar(20) not null references Users(UserId),
	FriendId nvarchar(20) not null,
	IsOwerAppUser bit not null
)
