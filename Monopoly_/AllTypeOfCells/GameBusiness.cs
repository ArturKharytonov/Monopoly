using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monopoly_
{
    public class GameBusiness : BusinessType
    {
        public GameBusiness(string name, TypeOfBusiness type, int price,
            int[] rent, int numberOfCell, int pledgeOfBusiness, int redemptionOfBusiness)
        {
            Name = name;
            Type = type;
            Price = price;
            Rent = rent; // Множення 
            NumberOfCell = numberOfCell;
            PledgeOfBusiness = pledgeOfBusiness;
            IsPledged = false;
            StepsDuringPledge = 15;
            RedemptionOfBusiness = redemptionOfBusiness;
            OwnerOfBusiness = String.Empty;
            UserSign = new List<char>();
        }
    }
}
