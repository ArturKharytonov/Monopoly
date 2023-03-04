using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monopoly_
{
    public class CellJail : Cell
    {
        public CellJail(int numberOfCell)
        {
            Name = "Jail";
            NumberOfCell = numberOfCell;
            UserSign = new List<char>();
        }
        
    }
}
