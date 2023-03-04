using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monopoly_
{
    public class CellBank : Cell
    {
        public CellBank(int numberOfCell)
        {
            Name = "Bank";
            NumberOfCell = numberOfCell;
            UserSign = new List<char>();
        }

       
    }
}
