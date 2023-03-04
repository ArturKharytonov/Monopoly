using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// ReSharper disable All

namespace Monopoly_
{
    public class Field
    {
        public List<Cell> FieldArray { get; set; }

        private const int _height1 = 9;
        private const int _width1 = 11;
        private const int _maxLengthForCorners = 25;
        private const int _maxLengthForVerticalColumn = 15;
        private const int _height2 = 9;
        private const int _width2 = 3;

        public Field()
        {
            FieldArray = new List<Cell>();
        }

        public bool DoesUserHaveMonopoly(User user, Cell business)
        {
            TypeOfBusiness tempType = ((Business)business).Type;
            int countOfShops = 0;
            for (int i = 0; i < user.ListOfUserBusinesses.Count; i++)
            {
                if (user.ListOfUserBusinesses[i].Type == tempType) countOfShops++;
            }
            switch (tempType)
            {
                case TypeOfBusiness.Perfumery:
                    if (countOfShops == 2) return true;
                    break;
                case TypeOfBusiness.Clothing:
                    if (countOfShops == 3) return true;
                    break;
                case TypeOfBusiness.WebService:
                    if (countOfShops == 3) return true;
                    break;
                case TypeOfBusiness.Drink:
                    if (countOfShops == 3) return true;
                    break;
                case TypeOfBusiness.Airlines:
                    if (countOfShops == 3) return true;
                    break;
                case TypeOfBusiness.Restaurant:
                    if (countOfShops == 3) return true;
                    break;
                case TypeOfBusiness.Hotel:
                    if (countOfShops == 3) return true;
                    break;
                case TypeOfBusiness.Electronics:
                    if (countOfShops == 2) return true;
                    break;
            }
            return false;
        }
        public Cell GetBranchToBuy(int index, User owner)
        {
            for (int i = 0; i < FieldArray.Count; i++)
            {
                if (i == index && ((BusinessType)FieldArray[i]).OwnerOfBusiness == owner.Name)
                {
                    if (DoesUserHaveMonopoly(owner, FieldArray[i]) && CanUserImproveCompany(owner, FieldArray[i]))
                    {
                        if (((BusinessType)FieldArray[i]).LevelOfBusiness < 5) return FieldArray[i];
                    }
                }
            }
            return null;
        }
        public bool CanUserImproveCompany(User user, Cell business)
        {
            TypeOfBusiness type = ((Business)business).Type;
            int minLevel = 5;
            int maxLevel = -1;
            for (int i = 0; i < user.ListOfUserBusinesses.Count; i++)
            {
                if (user.ListOfUserBusinesses[i].Type == type && user.ListOfUserBusinesses[i].LevelOfBusiness > maxLevel) maxLevel = user.ListOfUserBusinesses[i].LevelOfBusiness;
                if (user.ListOfUserBusinesses[i].Type == type && user.ListOfUserBusinesses[i].LevelOfBusiness < minLevel) minLevel = user.ListOfUserBusinesses[i].LevelOfBusiness;
            }

            if (minLevel == maxLevel) return true;
            else if (((Business)business).LevelOfBusiness == minLevel) return true;
            return false;
        }
        public bool DoesBusinessBelongsToUser(int index, User owner)
        {
            for (int i = 0; i < FieldArray.Count; i++)
            {
                if (FieldArray[i].GetType() == typeof(Business))
                {
                    if(i == index && ((BusinessType)FieldArray[i]).OwnerOfBusiness == owner.Name) return true;
                }
            }
            return false;
        }
        public Cell GetBranchToSell(int index, User owner)
        {
            for (int i = 0; i < FieldArray.Count; i++)
            {
                if (i == index &&
                    ((BusinessType)FieldArray[i]).LevelOfBusiness >= 1 &&
                    ((BusinessType)FieldArray[i]).OwnerOfBusiness == owner.Name &&
                    FieldArray[i].GetType() == typeof(Business)) return FieldArray[i];
            }
            return null;
        }
        public bool DoesItPossibleToSellBranch(int index, User owner)
        {
            if(FieldArray[index].GetType() == typeof(Business))
            {
                for (int i = 0; i < FieldArray.Count; i++)
                {
                    if (i == index &&
                    ((BusinessType)FieldArray[i]).LevelOfBusiness >= 1 &&
                    ((BusinessType)FieldArray[i]).OwnerOfBusiness == owner.Name) return true;
                }
            }
            return false;
        }
        public Cell PledgedOrRebuyBusiness(User user, int index)
        {
            for (int i = 0; i < FieldArray.Count; i++)
            {
                if (i == index)
                {
                    if (FieldArray[i].GetType() == typeof(GameBusiness))
                    {
                        if (((GameBusiness)FieldArray[i]).OwnerOfBusiness == user.Name)
                            return FieldArray[i];
                    }
                    else if (FieldArray[i].GetType() == typeof(CarBusiness))
                    {
                        if (((CarBusiness)FieldArray[i]).OwnerOfBusiness == user.Name)
                            return FieldArray[i];
                    }
                    else if(FieldArray[i].GetType() == typeof(Business))
                    {
                        if (((BusinessType)FieldArray[i]).OwnerOfBusiness == user.Name)
                            return FieldArray[i];
                    }
                    else return null;
                }
            }
            return null;
        }

        public bool DoesItPossibleToPledge(User user, int index)
        {
            if ((FieldArray[index].GetType() == typeof(GameBusiness) ||
                 FieldArray[index].GetType() == typeof(CarBusiness) ||
                 FieldArray[index].GetType() == typeof(Business)) &&
                !((BusinessType)FieldArray[index]).IsPledged &&
                ((BusinessType)FieldArray[index]).OwnerOfBusiness == user.Name) return true;
            return false;
        }
        public bool DoesItPossibleToRebuy(User user, int index)
        {
            if ((FieldArray[index].GetType() == typeof(GameBusiness) ||
                 FieldArray[index].GetType() == typeof(CarBusiness) ||
                 FieldArray[index].GetType() == typeof(Business)) &&
                ((BusinessType)FieldArray[index]).IsPledged &&
                ((BusinessType)FieldArray[index]).OwnerOfBusiness == user.Name) return true;
            return false;
        }

        public void InputAllSignsOnStart(List<User> users)
        {
            for (int i = 0; i < users.Count; i++)
            {
                FieldArray[0].UserSign.Add(users[i].Sign);
            }
        }
        public void ForFieldFilling(int verticalCord, int currentElementOfArray, bool sideLines, List<Cell> indexes, List<ConsoleColor> colors, List<User> usersInPrison) // 2 списки: ліст магазинів і відповідний йому ліст кольорів
        {
            if (indexes.Count > 0)
            {
                for (int i = 0; i < indexes.Count; i++)
                {
                    if (currentElementOfArray == indexes[i].NumberOfCell)
                    {
                        Console.BackgroundColor = colors[i];
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                }
            }

            if (!sideLines)
            {
                if (verticalCord == 0 && (currentElementOfArray == 0 || currentElementOfArray == 10 || currentElementOfArray == 30 || currentElementOfArray == 20))
                {
                    Console.Write("|");
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    Console.ForegroundColor = ConsoleColor.Black;
                    string text = FieldArray[currentElementOfArray].Name;
                    text += $" {currentElementOfArray}";
                    Console.Write(text);
                    while (text.Length < _maxLengthForCorners)
                    {
                        Console.Write(" ");
                        text += " ";
                    }
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("|  ");
                }
                else if (verticalCord == 8 && (currentElementOfArray == 0 || currentElementOfArray == 10 || currentElementOfArray == 30 || currentElementOfArray == 20))
                {
                    string text = "";
                    Console.Write("|");
                    if (FieldArray[currentElementOfArray].GetType() == typeof(CellStart) ||
                        (IsNotBusinessType(currentElementOfArray) && FieldArray[currentElementOfArray].GetType() != typeof(CellTax)) ||
                        FieldArray[currentElementOfArray].GetType() == typeof(CellPoliceStation))
                    {
                        Console.BackgroundColor = ConsoleColor.Yellow;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    bool temp = false;
                    for (int i = 0; i < FieldArray[currentElementOfArray].UserSign.Count; i++)
                    {
                        for (int j = 0; j < usersInPrison.Count; j++)
                        {
                            if (FieldArray[currentElementOfArray].UserSign[i] == usersInPrison[j].Sign)
                            {
                                text += FieldArray[currentElementOfArray].UserSign[i] + "* ";
                                temp = true;
                            }
                        }
                        if (!temp)
                        {
                            text += FieldArray[currentElementOfArray].UserSign[i] + " ";
                        }
                        temp = false;
                    }

                    Console.Write(text);
                    while (text.Length < _maxLengthForCorners)
                    {
                        Console.Write(" ");
                        text += " ";
                    }
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("|  ");
                }
                else if (currentElementOfArray == 0 || currentElementOfArray == 10 || currentElementOfArray == 30 || currentElementOfArray == 20)
                {
                    Console.Write("|");
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    Console.ForegroundColor = ConsoleColor.Black;
                    string text = "";
                    while (text.Length < _maxLengthForCorners)
                    {
                        Console.Write(" ");
                        text += " ";
                    }
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("|  ");
                }

                else
                {
                    if (verticalCord == 0)
                    {
                        Console.Write("|");
                        if (IsNotBusinessType(currentElementOfArray))
                        {
                            Console.BackgroundColor = ConsoleColor.Yellow;
                            Console.ForegroundColor = ConsoleColor.Black;
                        }
                        string text = FieldArray[currentElementOfArray].Name;
                        text += $" {currentElementOfArray}";

                        Console.Write($"{text}");
                        while (text.Length < _maxLengthForVerticalColumn)
                        {
                            Console.Write(" ");
                            text += " ";
                        }
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("|  ");
                    }
                    else if (verticalCord == 1)
                    {
                        string text = "";
                        Console.Write("|");
                        if (IsNotBusinessType(currentElementOfArray))
                        {
                            Console.BackgroundColor = ConsoleColor.Yellow;
                            Console.ForegroundColor = ConsoleColor.Black;
                        }
                        else text = WriteTypeInText(currentElementOfArray);


                        if (FieldArray[currentElementOfArray].GetType() == typeof(Business) ||
                            FieldArray[currentElementOfArray].GetType() == typeof(GameBusiness) ||
                            FieldArray[currentElementOfArray].GetType() == typeof(CarBusiness))
                        {
                            if (((BusinessType)FieldArray[currentElementOfArray]).IsPledged)
                            {
                                text += $"* {((BusinessType)FieldArray[currentElementOfArray]).StepsDuringPledge}";
                            }
                        }

                        Console.Write(text);
                        while (text.Length < _maxLengthForVerticalColumn)
                        {
                            Console.Write(" ");
                            text += " ";
                        }
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("|  ");
                    }
                    else if (verticalCord == 2)
                    {
                        string text = "";
                        Console.Write("|");
                        if (IsNotBusinessType(currentElementOfArray))
                        {
                            Console.BackgroundColor = ConsoleColor.Yellow;
                            Console.ForegroundColor = ConsoleColor.Black;
                        }
                        else
                        {
                            text = ((BusinessType)FieldArray[currentElementOfArray]).Price.ToString();
                        }
                        Console.Write(text);
                        while (text.Length < _maxLengthForVerticalColumn)
                        {
                            Console.Write(" ");
                            text += " ";
                        }
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("|  ");
                    }
                    else if (verticalCord == 3)
                    {
                        string text = "";
                        Console.Write("|");
                        if (IsNotBusinessType(currentElementOfArray))
                        {
                            Console.BackgroundColor = ConsoleColor.Yellow;
                            Console.ForegroundColor = ConsoleColor.Black;
                        }
                        else
                        {
                            text = ((BusinessType)FieldArray[currentElementOfArray]).RedemptionOfBusiness.ToString();
                        }
                        Console.Write(text);
                        while (text.Length < _maxLengthForVerticalColumn)
                        {
                            Console.Write(" ");
                            text += " ";
                        }
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("|  ");
                    }
                    else if (verticalCord == 4)
                    {
                        string text = "";
                        Console.Write("|");
                        if (IsNotBusinessType(currentElementOfArray))
                        {
                            Console.BackgroundColor = ConsoleColor.Yellow;
                            Console.ForegroundColor = ConsoleColor.Black;
                        }
                        else
                        {
                            text = ((BusinessType)FieldArray[currentElementOfArray]).PledgeOfBusiness.ToString();
                        }
                        Console.Write(text);
                        while (text.Length < _maxLengthForVerticalColumn)
                        {
                            Console.Write(" ");
                            text += " ";
                        }
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("|  ");
                    }
                    else if (verticalCord == 5)
                    {
                        string text = "";
                        Console.Write("|");
                        if (IsNotBusinessType(currentElementOfArray))
                        {
                            Console.BackgroundColor = ConsoleColor.Yellow;
                            Console.ForegroundColor = ConsoleColor.Black;
                        }
                        else
                        {
                            text = ((BusinessType)FieldArray[currentElementOfArray]).LevelOfBusiness.ToString();
                        }
                        Console.Write(text);
                        while (text.Length < _maxLengthForVerticalColumn)
                        {
                            Console.Write(" ");
                            text += " ";
                        }
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("|  ");
                    }
                    else if (verticalCord == 6)
                    {
                        string text = "";
                        Console.Write("|");
                        if (IsNotBusinessType(currentElementOfArray))
                        {
                            Console.BackgroundColor = ConsoleColor.Yellow;
                            Console.ForegroundColor = ConsoleColor.Black;
                        }
                        else
                        {
                            text = ((BusinessType)FieldArray[currentElementOfArray]).Rent[((BusinessType)FieldArray[currentElementOfArray]).LevelOfBusiness].ToString();
                        }
                        Console.Write(text);
                        while (text.Length < _maxLengthForVerticalColumn)
                        {
                            Console.Write(" ");
                            text += " ";
                        }
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("|  ");
                    }
                    else if (verticalCord == 7)
                    {
                        string text = "";
                        Console.Write("|");
                        if (IsNotBusinessType(currentElementOfArray))
                        {
                            Console.BackgroundColor = ConsoleColor.Yellow;
                            Console.ForegroundColor = ConsoleColor.Black;
                        }
                        else
                        {
                            text = ((BusinessType)FieldArray[currentElementOfArray]).PriceForImprovement.ToString();
                        }
                        Console.Write(text);
                        while (text.Length < _maxLengthForVerticalColumn)
                        {
                            Console.Write(" ");
                            text += " ";
                        }
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("|  ");
                    }
                    else if (verticalCord == 8)
                    {
                        string text = "";
                        Console.Write("|");
                        if (IsNotBusinessType(currentElementOfArray))
                        {
                            Console.BackgroundColor = ConsoleColor.Yellow;
                            Console.ForegroundColor = ConsoleColor.Black;
                        }
                        bool temp = false;
                        for (int i = 0; i < FieldArray[currentElementOfArray].UserSign.Count; i++)
                        {
                            for (int j = 0; j < usersInPrison.Count; j++)
                            {
                                if (FieldArray[currentElementOfArray].UserSign[i] == usersInPrison[j].Sign)
                                {
                                    text += FieldArray[currentElementOfArray].UserSign[i] + "* ";
                                    temp = true;
                                }
                            }
                            if (!temp)
                            {
                                text += FieldArray[currentElementOfArray].UserSign[i] + " ";
                            }
                            temp = false;
                        }
                        
                        Console.Write(text);
                        while (text.Length < _maxLengthForVerticalColumn)
                        {
                            Console.Write(" ");
                            text += " ";
                        }
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("|  ");
                    }
                }
            }

            else
            {
                if (verticalCord == 0)
                {
                    Console.Write("|");
                    if (IsNotBusinessType(currentElementOfArray))
                    {
                        Console.BackgroundColor = ConsoleColor.Yellow;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    string text = FieldArray[currentElementOfArray].Name;

                    text += $" {currentElementOfArray}";
                    Console.Write($"{text}");
                    while (text.Length < _maxLengthForCorners)
                    {
                        Console.Write(" ");
                        text += " ";
                    }
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("|  ");

                    if (currentElementOfArray == 11) Console.Write("1 - Name. And digits next to name mean index of square.");
                    
                    
                }
                else if (verticalCord == 1)
                {
                    string text = "";
                    Console.Write("|");
                    if (IsNotBusinessType(currentElementOfArray))
                    {
                        Console.BackgroundColor = ConsoleColor.Yellow;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    else text = WriteTypeInText(currentElementOfArray);


                    if (FieldArray[currentElementOfArray].GetType() == typeof(Business) ||
                            FieldArray[currentElementOfArray].GetType() == typeof(GameBusiness) ||
                            FieldArray[currentElementOfArray].GetType() == typeof(CarBusiness))
                    {
                        if (((BusinessType)FieldArray[currentElementOfArray]).IsPledged)
                        {
                            text += $"* {((BusinessType)FieldArray[currentElementOfArray]).StepsDuringPledge}";
                        }
                    }
                    Console.Write(text);
                    while (text.Length < _maxLengthForCorners)
                    {
                        Console.Write(" ");
                        text += " ";
                    }
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("|  ");

                    if (currentElementOfArray == 11) Console.Write("2 - Type.(* - means buisness pledged / digit - means cycles to extinction)");
                }
                else if (verticalCord == 2)
                {
                    string text = "";
                    Console.Write("|");
                    if (IsNotBusinessType(currentElementOfArray))
                    {
                        Console.BackgroundColor = ConsoleColor.Yellow;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    else
                    {
                        text = ((BusinessType)FieldArray[currentElementOfArray]).Price.ToString();
                    }
                    Console.Write(text);
                    while (text.Length < _maxLengthForCorners)
                    {
                        Console.Write(" ");
                        text += " ";
                    }
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("|  ");

                    if (currentElementOfArray == 11) Console.Write("3 - Price.");
                }
                else if (verticalCord == 3)
                {
                    string text = "";
                    Console.Write("|");
                    if (IsNotBusinessType(currentElementOfArray))
                    {
                        Console.BackgroundColor = ConsoleColor.Yellow;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    else
                    {
                        text = ((BusinessType)FieldArray[currentElementOfArray]).RedemptionOfBusiness.ToString();
                    }
                    Console.Write(text);
                    while (text.Length < _maxLengthForCorners)
                    {
                        Console.Write(" ");
                        text += " ";
                    }
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("|  ");

                    if (currentElementOfArray == 11) Console.Write("4 - Redemption of buisness.");
                }
                else if (verticalCord == 4)
                {
                    string text = "";
                    Console.Write("|");
                    if (IsNotBusinessType(currentElementOfArray))
                    {
                        Console.BackgroundColor = ConsoleColor.Yellow;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    else
                    {
                        text = ((BusinessType)FieldArray[currentElementOfArray]).PledgeOfBusiness.ToString();
                    }
                    Console.Write(text);
                    while (text.Length < _maxLengthForCorners)
                    {
                        Console.Write(" ");
                        text += " ";
                    }
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("|  ");

                    if (currentElementOfArray == 11) Console.Write("5 - Pledge of buisness.");
                }
                else if (verticalCord == 5)
                {
                    string text = "";
                    Console.Write("|");
                    if (IsNotBusinessType(currentElementOfArray))
                    {
                        Console.BackgroundColor = ConsoleColor.Yellow;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    else
                    {
                        text = ((BusinessType)FieldArray[currentElementOfArray]).LevelOfBusiness.ToString();
                    }
                    Console.Write(text);
                    while (text.Length < _maxLengthForCorners)
                    {
                        Console.Write(" ");
                        text += " ";
                    }
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("|  ");

                    if (currentElementOfArray == 11) Console.Write("6 - Level of buisness.");
                }
                else if (verticalCord == 6)
                {
                    string text = "";
                    Console.Write("|");
                    if (IsNotBusinessType(currentElementOfArray))
                    {
                        Console.BackgroundColor = ConsoleColor.Yellow;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    else
                    {
                        text = ((BusinessType)FieldArray[currentElementOfArray]).Rent[((BusinessType)FieldArray[currentElementOfArray]).LevelOfBusiness].ToString();
                    }
                    Console.Write(text);
                    while (text.Length < _maxLengthForCorners)
                    {
                        Console.Write(" ");
                        text += " ";
                    }
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("|  ");

                    if (currentElementOfArray == 11) Console.Write("7 - Rent.");
                }
                else if (verticalCord == 7)
                {
                    string text = "";
                    Console.Write("|");
                    if (IsNotBusinessType(currentElementOfArray))
                    {
                        Console.BackgroundColor = ConsoleColor.Yellow;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    else
                    {
                        text = ((BusinessType)FieldArray[currentElementOfArray]).PriceForImprovement.ToString();
                    }
                    Console.Write(text);
                    while (text.Length < _maxLengthForCorners)
                    {
                        Console.Write(" ");
                        text += " ";
                    }
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("|  ");

                    if (currentElementOfArray == 11) Console.Write("8 - Price for improvement.");
                }
                else if (verticalCord == 8)
                {
                    string text = "";
                    Console.Write("|");
                    if (IsNotBusinessType(currentElementOfArray))
                    {
                        Console.BackgroundColor = ConsoleColor.Yellow;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    for (int i = 0; i < FieldArray[currentElementOfArray].UserSign.Count; i++)
                    {
                        text += FieldArray[currentElementOfArray].UserSign[i] + " ";
                    }
                    Console.Write(text);
                    while (text.Length < _maxLengthForCorners)
                    {
                        Console.Write(" ");
                        text += " ";
                    }
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("|  ");

                    if (currentElementOfArray == 11) Console.Write("9 - All Signs.");
                }
            }
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor= ConsoleColor.White;
        }

        public string WriteTypeInText(int currentElementOfArray)
        {
            switch (((BusinessType)FieldArray[currentElementOfArray]).Type)
            {
                case TypeOfBusiness.Perfumery:
                    return "Perfumery";

                case TypeOfBusiness.Cars:
                    return "Cars";

                case TypeOfBusiness.Clothing:
                    return "Clothing";

                case TypeOfBusiness.WebService:
                    return "Web-services";

                case TypeOfBusiness.GameDevelopment:
                    return "Game dev";

                case TypeOfBusiness.Drink:
                    return "Drinks";

                case TypeOfBusiness.Airlines:
                    return "Airlines";

                case TypeOfBusiness.Restaurant:
                    return "Restaurant";

                case TypeOfBusiness.Hotel:
                    return "Hotel";

                case TypeOfBusiness.Electronics:
                    return "Electronics";
            }
            return "";
        }
        public bool IsNotBusinessType(int currentElementOfArray)
        {
            if (FieldArray[currentElementOfArray].GetType() == typeof(CellBank) ||
                FieldArray[currentElementOfArray].GetType() == typeof(CellChance) ||
                FieldArray[currentElementOfArray].GetType() == typeof(CellJail) ||
                FieldArray[currentElementOfArray].GetType() == typeof(CellJackpot) ||
                FieldArray[currentElementOfArray].GetType() == typeof(CellTax)) return true;
            return false;
        }
        public void ShowField(List<Cell> indexes, List<ConsoleColor> colors, List<User> usersInPrison)
        {
            bool sideLines = false;
            for (int i = 0; i < 11; i++)
            {
                if (i == 0 || i == 10)
                {
                    for (int j = 0; j < 27; j++)
                    {
                        Console.Write("-");
                    }
                    Console.Write("  ");
                }
                else
                {
                    for (int j = 0; j < 17; j++)
                    {
                        Console.Write("-");
                    }
                    Console.Write("  ");
                }
            }
            Console.WriteLine();
            for (int i = 0; i < _height1; i++)
            {
                for (int j = 0; j < _width1; j++)
                {
                    ForFieldFilling(i, j, sideLines, indexes, colors, usersInPrison);
                }
                Console.WriteLine();
            }
            for (int i = 0; i < 11; i++)
            {
                if (i == 0 || i == 10)
                {
                    for (int j = 0; j < 27; j++)
                    {
                        Console.Write("-");
                    }
                    Console.Write("  ");
                }
                else
                {
                    for (int j = 0; j < 17; j++)
                    {
                        Console.Write("-");
                    }
                    Console.Write("  ");
                }
            }
            Console.WriteLine();

            sideLines = true;
            for (int i = 0; i < 3; i++)
            {
                if (i == 0 || i == 2)
                {
                    for (int j = 0; j < 27; j++)
                    {
                        Console.Write("-");
                    }
                    Console.Write("  ");
                }
                else
                {
                    for (int j = 0; j < 169; j++)
                    {
                        Console.Write("-");
                    }
                    Console.Write("  ");
                }
            }

            Console.WriteLine();

            int difference = 28;
            for (int i = FieldArray.Count - 1; i > FieldArray.Count - 10; i--)
            {
                for (int j = 0; j < _height2; j++)
                {
                    for (int k = 0; k < _width2; k++)
                    {
                        if (k == 1)
                        {
                            Console.Write("|");
                            for (int l = 0; l < 167; l++)
                            {
                                Console.Write(" ");
                            }
                            Console.Write("|  ");
                        }
                        else if (k == 0)
                        {
                            ForFieldFilling(j, i, sideLines, indexes, colors, usersInPrison);
                        }
                        else ForFieldFilling(j, i - difference, sideLines, indexes, colors, usersInPrison);
                    }
                    Console.WriteLine();
                }
                for (int j = 0; j < 3; j++)
                {
                    if (j == 0 || j == 2)
                    {
                        for (int k = 0; k < 27; k++)
                        {
                            Console.Write("-");
                        }
                        Console.Write("  ");
                    }
                    else
                    {
                        if (i == 31)
                        {
                            for (int k = 0; k < 169; k++)
                            {
                                Console.Write("-");
                            }
                            Console.Write("  ");
                        }
                        else
                        {
                            for (int k = 0; k < 169; k++)
                            {
                                Console.Write(" ");
                            }
                            Console.Write("  ");
                        }
                    }
                }
                difference -= 2;
                Console.WriteLine();
            }

            sideLines = false;
            for (int i = 0; i < 11; i++)
            {
                if (i == 0 || i == 10)
                {
                    for (int j = 0; j < 27; j++)
                    {
                        Console.Write("-");
                    }
                    Console.Write("  ");
                }
                else
                {
                    for (int j = 0; j < 17; j++)
                    {
                        Console.Write("-");
                    }
                    Console.Write("  ");
                }
            }

            Console.WriteLine();

            for (int i = 0; i < _height1; i++)
            {
                for (int j = FieldArray.Count - 10; j > 19; j--)
                {
                    ForFieldFilling(i, j, sideLines, indexes, colors, usersInPrison);
                }
                Console.WriteLine();
            }

            for (int i = 0; i < 11; i++)
            {
                if (i == 0 || i == 10)
                {
                    for (int j = 0; j < 27; j++)
                    {
                        Console.Write("-");
                    }
                    Console.Write("  ");
                }
                else
                {
                    for (int j = 0; j < 17; j++)
                    {
                        Console.Write("-");
                    }
                    Console.Write("  ");
                }
            }
            Console.WriteLine();
        }
        public void FieldFilling()
        {
            FieldArray.Add(new CellStart());
            FieldArray.Add(new Business("Chanel", TypeOfBusiness.Perfumery, 600, new int[]{ 20, 100, 300, 900, 1600, 2500 }, 1, 500, 300, 360));
            FieldArray.Add(new CellChance(2));
            FieldArray.Add(new Business("HugoBoss", TypeOfBusiness.Perfumery, 600, new int[] { 40, 200, 600, 1800, 3200, 4500 }, 3, 500, 300, 360));
            FieldArray.Add(new CellBank(4));
            FieldArray.Add(new CarBusiness("Mercedes", TypeOfBusiness.Cars, 2000, new int[] {250, 500, 1000, 2000}, 5, 1000, 1200)); 
            FieldArray.Add(new Business("Adidas", TypeOfBusiness.Clothing, 1000, new int[] { 60, 300, 900, 2700, 4000, 5500 }, 6, 500, 500, 600));
            FieldArray.Add(new CellChance(7));
            FieldArray.Add(new Business("Puma", TypeOfBusiness.Clothing, 1000, new int[] { 60, 300, 900, 2700, 4000, 5500 }, 8, 500, 500, 600));
            FieldArray.Add(new Business("Lacoste", TypeOfBusiness.Clothing, 1200, new int[] { 80, 400, 1000, 3000, 4500, 6000 }, 9, 500, 600, 720));
            FieldArray.Add(new CellJail(10));
            FieldArray.Add(new Business("VK", TypeOfBusiness.WebService, 1400, new int[] { 100, 500, 1500, 4500, 6250, 7500 }, 11, 750, 700, 840));
            FieldArray.Add(new GameBusiness("Rockstar Games", TypeOfBusiness.GameDevelopment, 1500, new int[] {100, 250 }, 12, 750, 900));
            FieldArray.Add(new Business("FaceBook", TypeOfBusiness.WebService, 1400, new int[] { 100, 500, 1500, 4500, 6250, 7500 }, 13, 750, 700, 840));
            FieldArray.Add(new Business("Twitter", TypeOfBusiness.WebService, 1600, new int[] { 120, 600, 1800, 5000, 7000, 9000 }, 14, 750, 800, 960));
            FieldArray.Add(new CarBusiness("Audi", TypeOfBusiness.Cars, 2000, new int[] { 250, 500, 1000, 2000 }, 15, 1000, 1200));
            FieldArray.Add(new Business("Coca-Cola", TypeOfBusiness.Drink, 1800, new int[] { 140, 700, 2000, 5500, 7500, 9500 }, 16, 1000, 900, 1000));
            FieldArray.Add(new CellChance(17));
            FieldArray.Add(new Business("Pepsi", TypeOfBusiness.Drink, 1800, new int[] { 140, 700, 2000, 5500, 7500, 9500 }, 18, 1000, 900, 1000));
            FieldArray.Add(new Business("Fanta", TypeOfBusiness.Drink, 2000, new int[] { 160, 800, 2200, 6000, 8000, 10000 }, 19, 1000, 1000, 1200));
            FieldArray.Add(new CellJackpot());
            FieldArray.Add(new Business("USA Airlines", TypeOfBusiness.Airlines, 2200, new int[] { 180, 900, 2500, 7000, 8750, 10500 }, 21, 1250, 1100, 1320));
            FieldArray.Add(new CellChance(22));
            FieldArray.Add(new Business("Lufthansa", TypeOfBusiness.Airlines, 2200, new int[] { 180, 900, 2500, 7000, 8750, 10500 }, 23, 1250, 1100, 1320));
            FieldArray.Add(new Business("British Air", TypeOfBusiness.Airlines, 2400, new int[] { 200, 1000, 3000, 7500, 9250, 11000 }, 24, 1250, 1200, 1440));
            FieldArray.Add(new CarBusiness("Ford", TypeOfBusiness.Cars, 2000, new int[] {250, 500, 1000, 2000 }, 25, 1000, 1200));
            FieldArray.Add(new Business("McDonald's", TypeOfBusiness.Restaurant, 2600, new int[] { 220, 1100, 3300, 8000, 9750, 11500 }, 26, 1500, 1300, 1560));
            FieldArray.Add(new Business("Burger King", TypeOfBusiness.Restaurant, 2600, new int[] { 220, 1100, 3300, 8000, 9750, 11500 }, 27, 1500, 1300, 1560));
            FieldArray.Add(new GameBusiness("Blizzard", TypeOfBusiness.GameDevelopment, 1500, new int[] {100, 250 }, 28, 750, 900));
            FieldArray.Add(new Business("KFC", TypeOfBusiness.Restaurant, 2800, new int[] { 240, 1100, 3300, 8000, 9750, 11500 }, 29, 1500, 1400, 1680));
            FieldArray.Add(new CellPoliceStation(30));
            FieldArray.Add(new Business("Holiday Inn", TypeOfBusiness.Hotel, 3000, new int[] { 260, 1300, 3900, 9000, 11000, 12750 }, 31, 1750, 1500, 1800));
            FieldArray.Add(new Business("Radisson Blu", TypeOfBusiness.Hotel, 3000, new int[] { 260, 1300, 3900, 9000, 11000, 12750 }, 32, 1750, 1500, 1800));
            FieldArray.Add(new CellChance(33));
            FieldArray.Add(new Business("Novotel", TypeOfBusiness.Hotel, 3200, new int[] { 280, 1500, 4500, 10000, 12000, 14000 }, 34, 1750, 1600, 1920));
            FieldArray.Add(new CarBusiness("Land Rover", TypeOfBusiness.Cars, 2000, new int[] { 250, 500, 1000, 2000 }, 35, 1000, 1200));
            FieldArray.Add(new CellTax(36));
            FieldArray.Add(new Business("Apple", TypeOfBusiness.Electronics, 3500, new int[] { 350, 1750, 5000, 11000, 13000, 15000 }, 37, 2000, 1750, 2100));
            FieldArray.Add(new CellChance(38));
            FieldArray.Add(new Business("Samsung", TypeOfBusiness.Electronics, 4000, new int[] { 500, 2000, 6000, 14000, 17000, 20000 }, 39, 2000, 2000, 2400));
        }
        public void MoveSign(User user, int resOfCubes)
        {
            bool isEnd = false;
            for (int i = 0; i < FieldArray.Count && !isEnd; i++)
            {
                for (int j = 0; j < FieldArray[i].UserSign.Count; j++)
                {
                    if (FieldArray[i].UserSign[j] == user.Sign)
                    {
                        FieldArray[i].UserSign.RemoveAt(j);
                        resOfCubes += i;
                        if (resOfCubes >= FieldArray.Count)
                        {
                            Console.WriteLine("Passed the circle and got 2k.");
                            resOfCubes -= 40;
                            user.Balance += 2000;

                            if (resOfCubes == 0)
                            {
                                user.Balance += 1000;
                            }
                            
                        }
                        FieldArray[resOfCubes].UserSign.Add(user.Sign);
                        isEnd = true;
                    }
                }
            }
        }
        public Cell WhereSignStopped(char sign)
        {
            for (int i = 0; i < FieldArray.Count; i++)
            {
                for (int j = 0; j < FieldArray[i].UserSign.Count; j++)
                {
                    if (FieldArray[i].UserSign[j] == sign)
                    {
                        return FieldArray[i];
                    }
                }
            }
            return null;
        }

        public int FindAJail()
        {
            for (int i = 0; i < FieldArray.Count; i++)
            {
                if (FieldArray[i].GetType() == typeof(CellJail)) return i;
            }
            return -1; 
        }
        public void AddSignInPrison(char sign)
        {
            FieldArray[FindAJail()].UserSign.Add(sign);
        }

        public bool DoSignInJail(List<User> userInJail, char sign)
        {
            for (int i = 0; i < userInJail.Count; i++)
            {
                if (userInJail[i].Sign == sign) return true;
            }
            return false;
        }
        public void DeleteSignFromJail(List<User> userInJail, char sign)
        {
            for (int i = 0; i < userInJail.Count; i++)
            {
                if (userInJail[i].Sign == sign) 
                {
                    userInJail.RemoveAt(i);
                    break;
                }
            }
        }
        public void PayForRent(string ownerName, int rent, List<User> users)
        {
            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].Name == ownerName)
                {
                    users[i].Balance += rent;
                }
            }
        }
        public User GetUserByName(List<User> users, string name)
        {
            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].Name == name) return users[i];
            }
            return null;
        }
        public int CountOfOneTypeCompany(User bot, TypeOfBusiness type)
        {
            int count = 0;
            for (int i = 0; i < bot.ListOfUserBusinesses.Count; i++)
            {
                if (bot.ListOfUserBusinesses[i].Type == type) count++;
            }
            return count;
        }
        public int CountOfBusinessesOnMap(TypeOfBusiness type)
        {
            int count = 0;
            for (int i = 0; i < FieldArray.Count; i++)
            {
                if (FieldArray[i].GetType() == typeof(Business) && ((BusinessType)FieldArray[i]).Type == type) count++;
            }
            return count;
        }

        public bool DoesBotNeedToBuyCompanyToBlockBranch(Cell business, TypeOfBusiness type, User bot, List<User> users)
        {
            if (business.GetType() == typeof(Business))
            {
                string nameOfAnotherPlayer = "";
                for (int i = 0; i < FieldArray.Count; i++)
                {
                    if (FieldArray[i].GetType() == typeof(Business) && ((BusinessType)FieldArray[i]).Type == type)
                    {
                        if (((BusinessType)FieldArray[i]).OwnerOfBusiness != bot.Name && ((BusinessType)FieldArray[i]).OwnerOfBusiness != "") nameOfAnotherPlayer = ((BusinessType)FieldArray[i]).OwnerOfBusiness;
                        else if (((BusinessType)FieldArray[i]).OwnerOfBusiness == bot.Name) return false;
                    }
                }

                if (nameOfAnotherPlayer != "" && CountOfOneTypeCompany(GetUserByName(users, nameOfAnotherPlayer), type) == CountOfBusinessesOnMap(type) - 1) return true;
            }
            return false;
        }
        public bool AreAllCompaniesEmpty(Cell business, TypeOfBusiness type, User bot)
        {
            if (business.GetType() == typeof(Business))
            {
                for (int i = 0; i < FieldArray.Count; i++)
                {
                    if (FieldArray[i].GetType() == typeof(Business))
                    {
                        if (((BusinessType)FieldArray[i]).Type == type)
                        {
                            if (((BusinessType)FieldArray[i]).OwnerOfBusiness != bot.Name && ((BusinessType)FieldArray[i]).OwnerOfBusiness != "") return false;
                        }
                    }
                }
                return true;
            }
            
            return false;
        }
        public bool WillBotGetAlmostMonopoly(Cell business, User bot)
        {
            if ((((BusinessType)business).Type == TypeOfBusiness.Perfumery || ((BusinessType)business).Type == TypeOfBusiness.Electronics) && ((Bot)bot).CountOfOneTypeCompany(bot, ((BusinessType)business).Type) == 0) return true;
            else if (((Bot)bot).CountOfOneTypeCompany(bot, ((BusinessType)business).Type) == 1) return true;
            return false;
        }
        public double GetRandomNumber(double minimum, double maximum)
        {
            Random random = new Random();
            return random.NextDouble() * (maximum - minimum) + minimum;
        }
        public bool AuctionForBot(List<User> allUsers, int tempPrice, int i, int startPrice, Cell cell)
        {
            Console.WriteLine($"Player {allUsers[i].Name}. Price is - {tempPrice}.");

            if (((Bot)allUsers[i]).IsThatCompanyImportant(cell, allUsers[i], this) &&
                (startPrice * GetRandomNumber(2.5, 3) > tempPrice) &&
                ((Bot)allUsers[i]).DoesEnoughMoney(tempPrice)) return true;

            else if ((cell.GetType() == typeof(CarBusiness) || cell.GetType() == typeof(GameBusiness)) && ((Bot)allUsers[i]).DoesEnoughMoney(tempPrice) && (startPrice * GetRandomNumber(1.2, 1.4) > tempPrice)) return true;

            else if (cell.GetType() == typeof(Business) &&
                     (DoesBotNeedToBuyCompanyToBlockBranch(cell, ((BusinessType)cell).Type, allUsers[i], allUsers) && ((Bot)allUsers[i]).DoesEnoughMoney(tempPrice) && (startPrice * GetRandomNumber(1.7, 2) > tempPrice)) ||
                     (WillBotGetAlmostMonopoly(cell, allUsers[i]) && ((Bot)allUsers[i]).DoesEnoughMoney(tempPrice) && (startPrice * GetRandomNumber(1.7, 1.9) > tempPrice)) ||
                     (AreAllCompaniesEmpty(cell, ((BusinessType)cell).Type, allUsers[i]) && ((Bot)allUsers[i]).DoesEnoughMoney(tempPrice) && (startPrice * GetRandomNumber(1.2, 1.4) > tempPrice))) return true;
            return false;
        }
        public User Auction(int startPrice, User user, List<User> allUsers, Cell cell)
        {
            bool auction = false;
            int tempPrice = startPrice + 100;
            List<User> users = new List<User>();
            Console.WriteLine($"Company: {cell.Name}");
            for (int i = 0; i < allUsers.Count; i++)
            {
                if (allUsers[i].Name != user.Name)
                {
                    if (allUsers[i].GetType() == typeof(Player))
                    {
                        if (allUsers[i].Balance >= tempPrice)
                        {
                            Console.Write($"Price is - {tempPrice}.\n" +
                                $"{allUsers[i].Name},would u like to take part in auction[y/n]: ");

                            char.TryParse(Console.ReadLine().ToLower(), out char takePart);
                            if (takePart == 'y')
                            {
                                Console.WriteLine($"{allUsers[i].Name} takes part in auction");
                                users.Add(allUsers[i]);
                                tempPrice += 100;
                            }
                            else
                            {
                                Console.WriteLine($"{allUsers[i].Name} declined take part in auction");
                            }
                        }
                    }

                    else
                    {
                        if (AuctionForBot(allUsers, tempPrice, i, startPrice, cell))
                        {
                            Console.WriteLine($"{allUsers[i].Name} takes part in auction");
                            users.Add(allUsers[i]);
                            tempPrice += 100;
                        }
                        else Console.WriteLine($"{allUsers[i].Name} declined take part in auction");
                    }
                }
            }

            if (users.Count > 1)
            {
                while (!auction)
                {
                    for (int i = 0; i < users.Count; i++)
                    {
                        if (users[i].GetType() == typeof(Player))
                        {
                            if (users[i].Balance >= tempPrice)
                            {
                                Console.Write($"Price is - {tempPrice}.\n" +
                                        $"{users[i].Name},would u like to continue in auction[y/n]: ");
                                char.TryParse(Console.ReadLine().ToLower(), out char takePart);
                                if (takePart == 'y' && users[i].Balance >= tempPrice)
                                {
                                    Console.WriteLine($"{users[i].Name} continue play in the auction");
                                    tempPrice += 100;
                                }

                                else
                                {
                                    Console.WriteLine($"{users[i].Name} refused to continue participating in the auction");
                                    users.RemoveAt(i);
                                }
                            }
                            else
                            {
                                Console.WriteLine($"{users[i].Name} hasn't enough money.");
                                users.RemoveAt(i);
                            }
                        }

                        else
                        {
                            if (AuctionForBot(users, tempPrice, i, startPrice, cell))
                            {
                                Console.WriteLine($"{users[i].Name} takes part in auction");
                                tempPrice += 100;
                            }
                            else
                            {
                                Console.WriteLine($"{users[i].Name} refused to continue participating in the auction");
                                users.RemoveAt(i);
                            }
                        }
                    }

                    if (users.Count == 1)
                    {
                        auction = true;
                        users[0].Balance -= tempPrice;
                        return users[0];
                    }
                    //else if (users.Count == 0) auction = true;
                }
            }

            else if (users.Count == 1)
            {
                users[0].Balance -= tempPrice - 100;
                return users[0];
            }
            return null;
        }
    }
}
