using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monopoly_
{
    public class CellTax : Cell
    {
        public CellTax(int numberOfCell)
        {
            Name = "Tax";
            NumberOfCell = numberOfCell;
            UserSign = new List<char>();
        }
    }
}
