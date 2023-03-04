using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monopoly_
{
    public class Business : BusinessType
    {
        public Business(string name, TypeOfBusiness type,int price, int[] rent, int numberOfCell, int priceForImprovement, int pledgeOfBusiness, int redemptionOfBusiness)
        {
            Name = name;
            Type = type;
            Price = price;
            Rent = rent;
            NumberOfCell = numberOfCell;
            PriceForImprovement = priceForImprovement;
            LevelOfBusiness = 0;
            PledgeOfBusiness = pledgeOfBusiness;
            IsPledged = false;
            StepsDuringPledge = 15;
            RedemptionOfBusiness = redemptionOfBusiness;
            OwnerOfBusiness = String.Empty;
            UserSign = new List<char>();
        }
    }
}
