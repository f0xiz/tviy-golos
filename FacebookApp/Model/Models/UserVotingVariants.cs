using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class UserVotingVariants
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public double Score { get; set; }

        public bool IsClose { get; set; }

        public bool IsVoted { get; set; }
    }
}
