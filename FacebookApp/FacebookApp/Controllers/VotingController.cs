using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model;
using Model.Services;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.Facebook;

namespace FacebookApp.Controllers
{
    public class VotingController : Controller
    {
        private ServiceVoting serviceVoting = new ServiceVoting();

        public class pmVotingForIndex
        {
            public int Id { get; set; }
            public string Name { get; set; }

            public bool IsLock { get; set; }

            public bool IsPersonal { get; set; }

            public int VoutersCount { get; set; }
        }

        public ActionResult Index(FacebookContext context)
        {
            Session["context"] = context;

            if (ModelState.IsValid)
            {
                var pm = new List<pmVotingForIndex>();

                var dbVotings = serviceVoting.GetAllVotings();

                foreach (var dbVoting in dbVotings)
                {
                    pm.Add(new pmVotingForIndex { Id = dbVoting.Id, Name = dbVoting.Name,IsLock=dbVoting.IsClosed,IsPersonal=dbVoting.IsPeople,VoutersCount = dbVoting.VotingVariants.Sum(vv=>vv.Vote.Count) });
                }

                pm = pm.OrderBy(p=>p.VoutersCount).ToList();
                return View(pm);
            }

            return View("Error");
        }

        public class pmForVotingIndex
        {
            public pmVotingForIndex VotingInfo { get; set; }

            public IEnumerable<UserVotingVariants> VotingVariants { get; set; }
        }

        public ActionResult VotingIndex (FacebookContext context,int VotingId)
        {
            Session["context"] = context;

            if (ModelState.IsValid)
            {
                var pm = new pmForVotingIndex();
                var dbVoting = serviceVoting.GetVotingById(VotingId);
                pm.VotingInfo = new pmVotingForIndex { Id = dbVoting.Id, Name = dbVoting.Name, IsLock = dbVoting.IsClosed, IsPersonal = dbVoting.IsPeople };
                pm.VotingVariants= serviceVoting.GetUserVotes(VotingId, context.UserId);

                return View(pm);
            }

            return View("Error");
        }

        public ActionResult Vote (FacebookContext context,int votingVariantId,bool IsVote)
        {

                if (context == null) context = (FacebookContext)Session["context"];

                int votingId = serviceVoting.Vote(context.UserId, votingVariantId, IsVote);

                var newResults = serviceVoting.GetUserVotes(votingId, context.UserId);

                return Json(newResults, JsonRequestBehavior.AllowGet);      
        }

        static private void WriteToFile(string data)
        {
            var log = System.IO.File.AppendText(@"C:\Logs\Add.txt");
            log.WriteLine("-------------------" + DateTime.Now + "--------------------");
            log.WriteLine(data);
            log.Close();
        }
    }
}
