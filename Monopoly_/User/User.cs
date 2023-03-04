using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monopoly_
{
    public class User
    {
        public string Name { get; set; }
        public char Sign { get; set; }
        public int Balance { get; set; }
        public int TriesToBreakFree { get; set; }
        public ConsoleColor ConsoleColor { get; set; }
        public List<BusinessType> ListOfUserBusinesses { get; set; }
        public List<TypeOfBusiness> WereUsedInStep { get; set; }
        public int CountOfDoubles { get; set; }
        public List<Offer> SentOffers { get; set; }
        public User(string name, char sign)
        {
            Name = name;
            Sign = sign;
            Balance = 15000;
            TriesToBreakFree = 0;
            ListOfUserBusinesses = new List<BusinessType>();
        }
        public void ShowUser()
        {
            Console.Write($"Name - {Name}. Sign - {Sign}. Balance - {Balance}.");
        }
    }
}
