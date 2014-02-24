CREATE PROCEDURE [dbo].[GetVoteRank]
	@VoteVariantId int ,
	@VotingId int
AS
	SET NOCOUNT ON;

	select rowNomber from 
	(
	SELECT Id, ROW_NUMBER() OVER (ORDER BY VotingVariants.Score DESC) As rowNomber
	FROM VotingVariants
	Where VotingId = @VotingId
	)
	as rowNomber
	where Id = @VoteVariantId
RETURN 0
