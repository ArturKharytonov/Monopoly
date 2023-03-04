using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monopoly_
{
    public class CellChance : Cell
    {
        public CellChance(int numberOfCell)
        {
            Name = "Chance";
            NumberOfCell = numberOfCell;
            UserSign = new List<char>();
        }
    }
}
