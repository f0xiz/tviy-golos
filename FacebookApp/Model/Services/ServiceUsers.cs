using Microsoft.AspNet.Mvc.Facebook;
using Microsoft.AspNet.Mvc.Facebook.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Services
{
    public class ServiceUsers : ServiceBase
    {
        public IEnumerable<FacebookSimpleUser> GetUsersForRating(FacebookContext context, int from)
        {
            List<FacebookSimpleUser> result = new List<FacebookSimpleUser>();

            var dbUsers = db.GetUsersSortedByAvtoritet(from + 10);

            var dbNeedUsers = GetNext<GetUsersSortedByAvtoritet_Result>(dbUsers, from,10);

            foreach (var dbUser in dbNeedUsers)
            {
                var ratingUser = new FacebookSimpleUser(db.Users.Single(u => u.UserId == dbUser.UserId));
                ratingUser.PositionInAvtoritet = (int)dbUser.RowNumber.Value;
                ratingUser.Avtoritet = Math.Round(dbUser.Avtoritet + AvtoritetConts, 2);
                result.Add(ratingUser);
            }


            return result;
        }

        public FacebookSimpleUser GetUserInfo(FacebookContext context, string UserId)
        {
            var dbUser = db.Users.Single(u => u.UserId == UserId);
            var fbUser = new FacebookSimpleUser(dbUser);

            fbUser.Avtoritet = Math.Round(dbUser.Avtoritet + AvtoritetConts,2);
            fbUser.IsInPeopleVoting = dbUser.IsInPeopleVouting;
            fbUser.PositionInAvtoritet = (int)db.GetAvtoritetRank(UserId).First().Value; 


            fbUser.PotionInVoting = 0;

            if (fbUser.IsInPeopleVoting)
            {
                var lastPeopleVoting = db.Votings.FirstOrDefault(v => v.IsPeople && !v.IsClosed);
                if (lastPeopleVoting != null)
                {
                    var votingVariant = dbUser.VotingVariants.Single(vv => vv.VotingId == lastPeopleVoting.Id);
                    fbUser.PotionInVoting = (int)db.GetVoteRank(votingVariant.Id, votingVariant.VotingId).First().Value;
                }
            }

            return fbUser;
        }

        public void AddUser(UserForMainPage fbUser)
        {
            var user = db.Users.SingleOrDefault(u => u.UserId == fbUser.Id);
            if (user == null)
            {
                user = new Users { UserId = fbUser.Id, Name = fbUser.Name, PictureScr = fbUser.Picture.Data.Url, LastActivity = DateTime.Now,Link = fbUser.Link };

                foreach (var friend in fbUser.Friends.Data)
                {
                    user.UserFriends.Add(new UserFriends { FriendId = friend.Id, IsOwerAppUser = false });
                }
                db.Users.Add(user);

            }
            else
            {
                user.Name = fbUser.Name;
                user.PictureScr = fbUser.Picture.Data.Url;
                user.LastActivity = DateTime.Now;
                user.Link = fbUser.Link;


                foreach (var friend in fbUser.Friends.Data)
                {
                    if (!user.UserFriends.Any(uf => uf.FriendId == friend.Id))
                        user.UserFriends.Add(new UserFriends { FriendId = friend.Id, IsOwerAppUser = false });
                }

                foreach (var friend in user.UserFriends.ToArray())
                {
                    if (!fbUser.Friends.Data.Any(uf => uf.Id == friend.FriendId))
                        db.UserFriends.Remove(friend);
                }
            }
            db.SaveChanges();

            db.SetOurFriendUsers();
        }

        public async Task<bool> UpdateAllUsersInfo(FacebookContext context)
        {
            foreach (var user in db.Users.ToArray())
            {
                var fbUser = await context.Client.GetFacebookObjectAsync<UserForMainPage>(user.UserId);
                if (fbUser.Id != null)
                AddUser(fbUser);
            }


            return true;
        }

        public async Task<bool> AdministrationAvtoritetAdd(FacebookContext context, string UserId, string Description)
        {
            var fbUser = await context.Client.GetFacebookObjectAsync<FacebookSimpleUser>(UserId);

            var User = db.Users.SingleOrDefault(u => u.UserId == fbUser.Id);
            if (User == null) return false;

            var AvtoritetConst = double.Parse(db.ServiseData.Single(s => s.Id == 1).Value);
            User.AdministationPodyakas.Add(new AdministationPodyakas { Description = Description, Time = DateTime.Now, Score = AvtoritetConst /2.0});
            User.Avtoritet += AvtoritetConst / 2.0;

            db.SaveChanges();

            return true;
        }

        public bool IsInRole (string UserId,string Role)
        {
            var dbUser = db.Users.SingleOrDefault(u=>u.UserId == UserId);

            if (Role == "Administrator") return dbUser.IsAdmin;
            if (Role == "VIP") return dbUser.IsVIP;
            return false;
        }
    }
}
