CREATE PROCEDURE [dbo].[GetAvtoritetRank]
	@UserId nvarchar(20)
AS
	SET NOCOUNT ON;

	select rowNomber from 
	(
	SELECT UserId, ROW_NUMBER() OVER (ORDER BY Users.Avtoritet DESC) As rowNomber
	FROM Users
	)
	as rowNomber
	where UserId = @UserId
RETURN 0
