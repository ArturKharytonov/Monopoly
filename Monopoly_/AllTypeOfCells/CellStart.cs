using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monopoly_
{
    public class CellStart : Cell
    {
        public CellStart()
        {
            Name = "Start";
            NumberOfCell = 0;
            UserSign = new List<char>();
        }

        
    }
}
