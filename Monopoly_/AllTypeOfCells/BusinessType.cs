using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monopoly_
{
    public abstract class BusinessType : Cell
    {
        public TypeOfBusiness Type { get; set; }
        public int Price { get; set; }
        public int[] Rent { get; set; } 
        public int LevelOfBusiness { get; set; } // Level of buisness
        public int PriceForImprovement { get; set; }
        public int PledgeOfBusiness { get; set; } // Залог бізнеса
        public bool IsPledged { get; set; }
        public int StepsDuringPledge { get; set; }
        public int RedemptionOfBusiness { get; set; } // Викуп бізнеса
        public string OwnerOfBusiness { get; set; }

        public BusinessType()
        {
            
        }
    }
}
