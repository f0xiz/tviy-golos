CREATE PROCEDURE [dbo].[GetUsersSortedByAvtoritet]
	@Count int = 10000
AS
	SET NOCOUNT ON;

    
	SELECT TOP (@Count) UserId, Avtoritet, ROW_NUMBER() OVER (ORDER BY Users.Avtoritet DESC) As RowNumber
	FROM Users
	Order by RowNumber
RETURN 0
