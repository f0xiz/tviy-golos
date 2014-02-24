using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Model;

namespace RealTimeService
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        static Thread PodyakiLog = new Thread(PodyakiLogDo);
        static Thread Rebalanse = new Thread(RebalanceDo);

        protected override void OnStart(string[] args)
        {
            WriteToFile("Started");
            PodyakiLog.Start();
            Rebalanse.Start();
        }

        protected override void OnStop()
        {
            WriteToFile("Stop");
        }

        static void RebalanceDo()
        {
            try
            {

                while (true)
                {
                    if (DateTime.Now.Hour == 4 && DateTime.Now.Minute == 0)
                    {
                        FacebookEntities db = new FacebookEntities();

                        double SumAvtoritet = db.Users.Sum(u => u.Avtoritet);
                        double NewGlobalRaitingValue = (double)SumAvtoritet / (double)db.Users.Count();
                        if (NewGlobalRaitingValue > 1) db.ServiseData.Single(s => s.Id == 1).Value = NewGlobalRaitingValue.ToString();
                        db.SaveChanges();
                    }
                    Thread.Sleep(60000);
                }
            }
            catch (Exception e)
            {
                WriteToFile(e.ToString());
            }
        }

        static void PodyakiLogDo()
        {
            try
            {

                while (true)
                {
                    FacebookEntities db = new FacebookEntities();
                    var AvtoritetConst = double.Parse(db.ServiseData.Single(s => s.Id == 1).Value);

                    foreach (var log in db.PodyakaLog.Where(l => !l.IsDone))
                    {
                        log.IsDone = true;

                        if (log.FromUserId == log.ToUserId)
                        {
                            continue;
                        }

                        var promote = db.Promote.SingleOrDefault(p => p.PostId == log.PostId);
                        if (promote != null) db.Promote.Remove(promote);



                        var ToUser = db.Users.Single(u => u.UserId == log.ToUserId);
                        var FromUser = db.Users.Single(u => u.UserId == log.FromUserId);

                        var post = ToUser.Posts.SingleOrDefault(p => p.PostId == log.PostId);

                        if (post == null)
                        {
                            post = new Posts { PostId = log.PostId };
                            ToUser.Posts.Add(post);
                        }

                        var podyaka = post.Podyaki.SingleOrDefault(p => p.UserId == log.FromUserId);

                        if (podyaka == null)
                        {
                            if (log.IsLike)
                            {


                                DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                                var FromUserPrevPodyakas = FromUser.Podyaki.Where(po => po.Time > currentDate);

                                double newAvtoritet = ((FromUser.Avtoritet + AvtoritetConst) / 2.0) / (FromUserPrevPodyakas.Count() + 1);

                                foreach (var prevPodyaka in FromUserPrevPodyakas)
                                {
                                    prevPodyaka.Posts.Users.Avtoritet = prevPodyaka.Posts.Users.Avtoritet - prevPodyaka.Avtoritet + newAvtoritet;
                                    prevPodyaka.Avtoritet = newAvtoritet;
                                }

                                post.Podyaki.Add(new Podyaki { UserId = log.FromUserId, Time = log.Time, Avtoritet = newAvtoritet });
                                post.PodyakiCount = post.PodyakiCount + 1;
                                ToUser.PodyakiCount = ToUser.PodyakiCount + 1;
                                ToUser.Avtoritet += newAvtoritet;

                            }
                        }
                        else
                        {
                            if (!log.IsLike)
                            {
                                DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                                var FromUserPrevPodyakas = FromUser.Podyaki.Where(po => po.Time > currentDate);


                                post.PodyakiCount = post.PodyakiCount - 1;
                                ToUser.PodyakiCount = ToUser.PodyakiCount - 1;
                                ToUser.Avtoritet -= podyaka.Avtoritet;

                                double newAvtoritet = 0;
                                if (FromUserPrevPodyakas.Count() != 1) newAvtoritet = ((FromUser.Avtoritet + AvtoritetConst) / 2.0) / (FromUserPrevPodyakas.Count() - 1);

                                db.Podyaki.Remove(podyaka);

                                foreach (var prevPodyaka in FromUserPrevPodyakas)
                                {
                                    prevPodyaka.Posts.Users.Avtoritet = prevPodyaka.Posts.Users.Avtoritet - prevPodyaka.Avtoritet + newAvtoritet;
                                    prevPodyaka.Avtoritet = newAvtoritet;
                                }
                            }

                        }

                        log.IsDone = true;
                    }

                    db.SaveChanges();

                    Thread.Sleep(100);
                }
            }
            catch (Exception e)
            {
                WriteToFile(e.ToString());
            }
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
