using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;


namespace Model.Services
{
    public class ServiceVoting : ServiceBase
    {
        public IEnumerable<Votings> GetAllVotings()
        {
            var result = new List<Votings>();

            foreach (var vote in db.Votings.Where(v=>v.IsPeople).OrderBy(v=>v.IsClosed))
            {
                result.Add(vote);
            }

            foreach (var vote in db.Votings.Where(v=>!v.IsPeople).OrderBy(v=>v.IsClosed))
            {
                result.Add(vote);
            }

            return result;
        }

        public Votings GetVotingById(int Id)
        {
            return db.Votings.Single(v=>v.Id == Id);
        }

        public IEnumerable<UserVotingVariants> GetUserVotes(int VotingId,string UserId)
        {
            var result = new List<UserVotingVariants>();
                 
            var voting = db.Votings.Single(v=>v.Id == VotingId);

            foreach(var voteVariant in voting.VotingVariants)
            {
                var userVote = new UserVotingVariants();
                userVote.Id = voteVariant.Id;
                userVote.Text = voteVariant.Text;
                userVote.Score = Math.Round(voteVariant.Score, 2);
                userVote.IsVoted = voteVariant.Vote.Any(u => u.UserId == UserId);
                userVote.IsClose = voteVariant.Votings.IsClosed;

                result.Add(userVote);
            }

            result = result.OrderByDescending(v=>v.Score).ToList();

            return result;
        }

        public int Vote(string UserId,int voteVariantId,bool IsVote)
        {
            var VoteVariant = db.VotingVariants.Single(v => v.Id == voteVariantId);

            var User = db.Users.Single(u=>u.UserId == UserId);

            var Vote = User.Vote.SingleOrDefault(v => v.VotingVariantId == voteVariantId);

            var AvtoritetConst = double.Parse(db.ServiseData.Single(s => s.Id == 1).Value);

            if (Vote == null)
            {
                if (IsVote == true)
                {
                    var PrevVotes = User.Vote.Where(v=>v.VotingVariants.VotingId == VoteVariant.VotingId);

                    double newScore = ((User.Avtoritet + AvtoritetConst) / AvtoritetConst) / (PrevVotes.Count() + 1);

                    foreach(var prevVote in PrevVotes)
                    {
                        prevVote.VotingVariants.Score = prevVote.VotingVariants.Score - prevVote.Score + newScore;
                        prevVote.Score = newScore;
                    }

                    User.Vote.Add(new Vote {VotingVariantId = voteVariantId,Score = newScore });
                    VoteVariant.Score += newScore;
                }
            }
            else
            {
                if (IsVote == false)
                {
                    var PrevVotes = User.Vote.Where(v => v.VotingVariants.VotingId == VoteVariant.VotingId);

                    double newScore = 0;
                    if (PrevVotes.Count() != 1) newScore = ((User.Avtoritet + AvtoritetConst) / AvtoritetConst) / (PrevVotes.Count() - 1);

                    VoteVariant.Score -= Vote.Score;
                    db.Vote.Remove(Vote);

                    foreach (var prevVote in PrevVotes)
                    {
                        prevVote.VotingVariants.Score = prevVote.VotingVariants.Score - prevVote.Score + newScore;
                        prevVote.Score = newScore;
                    }
                }
            }


            db.SaveChanges();

            return VoteVariant.VotingId;
        }

        public bool ChangePersonVotingStatus(string userId,bool IsInVoting)
        {
            var dbUser = db.Users.Single(u=>u.UserId == userId);

            if (!dbUser.IsInPeopleVouting && IsInVoting)
            {
                
                foreach (var voting in db.Votings.Where(v => v.IsPeople))
                {
                    if (!voting.VotingVariants.Any(vv => vv.UserId == userId))
                        voting.VotingVariants.Add(new VotingVariants { UserId = userId, Text = dbUser.UserId});
                }
                dbUser.IsInPeopleVouting = true;
            }

            if (dbUser.IsInPeopleVouting && !IsInVoting)
            {
                foreach (var voting in db.Votings.Where(v => v.IsPeople))
                {
                    var votingVariant = voting.VotingVariants.SingleOrDefault(vv => vv.UserId == userId);

                    if (votingVariant != null)
                        db.VotingVariants.Remove(votingVariant);
                }
                dbUser.IsInPeopleVouting = false;
            }

            db.SaveChanges();

            return true;
        }
    }
}