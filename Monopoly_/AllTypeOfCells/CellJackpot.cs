using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monopoly_
{
    public class CellJackpot : Cell
    {
        public CellJackpot()
        {
            Name = "Jackpot";
            NumberOfCell = 20;
            UserSign = new List<char>();
        }
    }
}
