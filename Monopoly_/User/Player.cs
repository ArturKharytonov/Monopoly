using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monopoly_
{
    public class Player : User
    {
        public Player(string name, char sign) : base(name, sign)
        {
            Name = name;
            Sign = sign;
            Balance = 15000;
            WereUsedInStep = new List<TypeOfBusiness>();
            CountOfDoubles = 0;
        }
    }
}
