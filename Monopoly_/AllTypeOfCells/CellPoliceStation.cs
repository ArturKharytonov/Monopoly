using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monopoly_
{
    public class CellPoliceStation : Cell
    {
        public CellPoliceStation(int numberOfCell)
        {
            Name = "Police Station";
            NumberOfCell = numberOfCell;
            UserSign = new List<char>();
        }
    }
}
