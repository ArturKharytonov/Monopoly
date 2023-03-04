using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monopoly_
{
    public abstract class Cell
    {
        public string Name { get; set; }
        public int NumberOfCell { get; set; }
        public List<char> UserSign { get; set; }

        public Cell()
        {
            Name = " ";
            NumberOfCell = 0;
            UserSign = new List<char>();
        }
    }
}
