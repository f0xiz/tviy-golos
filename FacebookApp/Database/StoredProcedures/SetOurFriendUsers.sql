CREATE PROCEDURE [dbo].[SetOurFriendUsers]
AS
	UPDATE UserFriends
	Set IsOwerAppUser = 'true'
	From UserFriends inner join Users
	On UserFriends.FriendId = Users.UserId
RETURN 0
