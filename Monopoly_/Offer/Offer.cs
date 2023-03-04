using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monopoly_
{
    public class Offer
    {
        public User Sender { get; set; }
        public List<Cell> WantsToGive { get; set; }
        public int SumOfMoneyThatWantsToGive { get; set; }
        public User Receiver { get; set; }
        public List<Cell> WantsToGet { get; set; }
        public int SumOfMoneyThatWantsToGet { get; set; }

        public Offer(User sender, List<Cell> wantsToGive, int sumToGive, User receiver, List<Cell> wantsToGet, int sumToGet)
        {
            Sender = sender;
            WantsToGive = wantsToGive;
            SumOfMoneyThatWantsToGive = sumToGive;
            Receiver = receiver;
            WantsToGet = wantsToGet;
            SumOfMoneyThatWantsToGet = sumToGet;
        }
    }
}
