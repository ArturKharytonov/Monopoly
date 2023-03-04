using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//ReSharper disable All

namespace Monopoly_
{
    public class Game
    {
        public Field Field { get; set; }
        public List<User> Users { get; set; }
        public List<User> UsersInPrison { get; set; }
        public List<Cell> ShopIndexes { get; set; }
        public List<ConsoleColor> Colors { get; set; }
        public List<User> NeedToSkipStep { get; set; }

        private const int _lengthForUserSquare = 25;
        private const int _maxCountOfCyclesInPledge = 15;
        public Game(Field field, List<User> users)
        {
            Field = field;
            Users = users;
            UsersInPrison = new List<User>();
            ShopIndexes = new List<Cell>();
            Colors = new List<ConsoleColor>();
            NeedToSkipStep = new List<User>();
        }

        public int GetAllMoney(User user)
        {
            int tempSum = user.Balance;
            for (int i = 0; i < user.ListOfUserBusinesses.Count; i++)
            {
                if (!user.ListOfUserBusinesses[i].IsPledged && user.ListOfUserBusinesses[i].GetType() == typeof(Business))
                {
                    int levels = user.ListOfUserBusinesses[i].LevelOfBusiness;
                    while (levels > 0)
                    {
                        tempSum += user.ListOfUserBusinesses[i].PriceForImprovement;
                        levels--;
                    }
                }
                if (!user.ListOfUserBusinesses[i].IsPledged) tempSum += user.ListOfUserBusinesses[i].PledgeOfBusiness;
            }
            return tempSum;
        }
        public User GetUserByName(string name)
        {
            for (int i = 0; i < Users.Count; i++)
            {
                if (Users[i].Name == name) return Users[i];
            }
            return null;
        }
        public bool CanPlayerSellBranch(User user, Cell business)
        {
            TypeOfBusiness type = ((BusinessType)business).Type;
            int minLevel = Int32.MaxValue;
            int maxLevel = Int32.MinValue;
            for (int i = 0; i < user.ListOfUserBusinesses.Count; i++)
            {
                if (user.ListOfUserBusinesses[i].Type == type && user.ListOfUserBusinesses[i].LevelOfBusiness > maxLevel) maxLevel = user.ListOfUserBusinesses[i].LevelOfBusiness;
                if (user.ListOfUserBusinesses[i].Type == type && user.ListOfUserBusinesses[i].LevelOfBusiness < minLevel) minLevel = user.ListOfUserBusinesses[i].LevelOfBusiness;
            }

            return (((Business)business).LevelOfBusiness == minLevel && minLevel != maxLevel);
        }
        public bool DoesItPossibleToBuyBranch(User user, BusinessType business)
        {
            for (int i = 0; i < user.ListOfUserBusinesses.Count; i++)
            {
                if (user.ListOfUserBusinesses[i].Type == business.Type && user.ListOfUserBusinesses[i].IsPledged) return false;
            }
            return true;
        }
        public void FillUserColors()
        {
            ConsoleColor[] colors = { ConsoleColor.Red, ConsoleColor.DarkBlue, ConsoleColor.DarkGreen, ConsoleColor.DarkCyan, ConsoleColor.DarkGray, ConsoleColor.DarkMagenta };
            for (int i = 0; i < Users.Count; i++)
            {
                Users[i].ConsoleColor = colors[i];
            }
        }
        public bool ShowAllUsers()
        {
            if (Users.Count < 1) return false;

            for (int i = 0; i < Users.Count; i++)
            {
                Users[i].ShowUser();
                Console.WriteLine();
            }
            return true;
        }

        public bool DoesNameExistInList(string name)
        {
            for (int i = 0; i < Users.Count; i++)
            {
                if (Users[i].Name == name) return true;
            }
            return false;
        }
        public bool DoesSignExist(char sign)
        {
            for (int i = 0; i < Users.Count; i++)
            {
                if (Users[i].Sign == sign) return true;
            }
            return false;
        }
        
        public bool DeleteUser(char sign)
        {
            for (int i = 0; i < Users.Count; i++)
            {
                if (Users[i].Sign == sign)
                {
                    Users.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }
        public User GetUser(char sign)
        {
            for (int i = 0; i < Users.Count; i++)
            {
                if (Users[i].Sign == sign) return Users[i];
            }
            return null;
        }
        public void ShowUsersUnderField()
        { 
            Console.WriteLine();
            for (int i = 0; i < Users.Count; i++)
            {
                for (int j = 0; j < _lengthForUserSquare; j++)
                {
                    Console.Write("-");
                }
                for (int j = 0; j < 10; j++)
                {
                    Console.Write(" ");
                }
            }
            Console.WriteLine();

            string text = "";
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < Users.Count; j++)
                {
                    Console.Write("|");
                    Console.BackgroundColor = Users[j].ConsoleColor;

                    if (i == 1)
                    {
                        text = "Name: " + Users[j].Name;
                    }
                    else if (i == 2)
                    {
                        text = "Balance: $" + Users[j].Balance.ToString();
                    }
                    else if (i == 3)
                    {
                        text = "Sign: " + Users[j].Sign.ToString();
                    }
                    Console.Write(text);
                    while (text.Length < _lengthForUserSquare - 2)
                    {
                        text += " ";
                        Console.Write(" ");
                    }
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write("|");
                    for (int k = 0; k < 10; k++)
                    {
                        Console.Write(" ");
                    }
                    text = "";
                }
                Console.WriteLine();
            }

            for (int i = 0; i < Users.Count; i++)
            {
                for (int j = 0; j < _lengthForUserSquare; j++)
                {
                    Console.Write("-");
                }
                for (int j = 0; j < 10; j++)
                {
                    Console.Write(" ");
                }
            }
            Console.WriteLine();
        }

        public void PledgeForPlayer(User user)
        {
            Console.Write("Enter index of business that u wanna pledge: ");
            int.TryParse(Console.ReadLine(), out int index);
            if (index >= 0 && index < Field.FieldArray.Count)
            {
                if (Field.DoesItPossibleToPledge(user, index))
                {
                    if ((CanIPledgeFromBranchTypes(user, ((BusinessType)Field.FieldArray[index]).Type) &&
                         Field.FieldArray[index].GetType() == typeof(Business)) ||
                        Field.FieldArray[index].GetType() != typeof(Business))
                    {
                        Cell pledgedCell = Field.PledgedOrRebuyBusiness(user, index);
                        if (pledgedCell != null)
                        {
                            if (!((BusinessType)pledgedCell).IsPledged)
                            {
                                ((BusinessType)pledgedCell).IsPledged = true;

                                user.Balance += ((BusinessType)pledgedCell).PledgeOfBusiness;
                                Console.WriteLine($"{user.Name}, business is succsessfuly pledged.");
                                Console.ReadLine();
                                Field.ShowField(ShopIndexes, Colors, UsersInPrison);
                                ShowUsersUnderField();
                            }
                            else Console.WriteLine("Business is already pledged.");
                        }
                        else Console.WriteLine("Error");
                    }
                    else Console.WriteLine("U can't pledge from Branch Types");
                }
                else Console.WriteLine("Company don't exist or not belongs to you");
            }
            else Console.WriteLine("Incorrect index");
        }
        public void SellBranchForPlayer(User user)
        {
            Console.Write("Enter index of business: ");
            int.TryParse(Console.ReadLine(), out int index);
            if (index >= 0 && index < Field.FieldArray.Count)
            {
                if (Field.DoesItPossibleToSellBranch(index, user))
                {
                    Cell tempBranch = Field.GetBranchToSell(index, user);
                    if (tempBranch != null && CanPlayerSellBranch(user, tempBranch))
                    {
                        user.Balance += ((BusinessType)tempBranch).PriceForImprovement;
                        ((BusinessType)tempBranch).LevelOfBusiness--;
                        Console.WriteLine($"You sold branch of {tempBranch.Name} and got {((BusinessType)tempBranch).PriceForImprovement}");
                        Console.ReadLine();
                        Field.ShowField(ShopIndexes, Colors, UsersInPrison);
                        ShowUsersUnderField();
                    }
                    else Console.WriteLine("Error.");
                }
                else Console.WriteLine("Company isn't found or u can't sell it as branch");
            }
            else Console.WriteLine("Incorrect index.");
        }

        public bool AddAndOfferCompany(int index, User tempOwner, List<Cell> cells)
        {
            if ((index > 0 && index < Field.FieldArray.Count) &&
                Field.FieldArray[index] is BusinessType &&
                ((BusinessType)Field.FieldArray[index]).OwnerOfBusiness == tempOwner.Name)
            {
                if (!DoesThatSellExistInList(Field.FieldArray[index], cells))
                {
                    if (Field.FieldArray[index].GetType() == typeof(Business))
                    {
                        if (DoesLevelOfThatTypeEqualsZero(((BusinessType)Field.FieldArray[index]).Type)) return true;

                        else
                        {
                            Console.WriteLine(
                                "U can't add company that belongs to branch and level of one company from that branch more than zero");
                            return false;
                        }

                    }
                    else return true;

                }
                else
                {
                    Console.WriteLine("U have already added that company to list");
                    return false;
                }
            }
            else Console.WriteLine("Company wasn't added.");
            return false;
        }
        public void SendOfferFromPlayer(User user, int whatMenu, Cell cell)
        {
            List<Cell> userWannaGet = new List<Cell>();
            List<Cell> userWannaGive = new List<Cell>();
            bool endOfOffer = false;
            char choice = ' ';
            User tempOwner = null;
            int index = 0;
            int moneyToGet = 0, moneyToOffer = 0;

            do
            {
                Console.Write("Enter sign of player to whom u want to send offer: ");
                char.TryParse(Console.ReadLine(), out choice);
            } while (!DoesSignExist(choice) || choice == user.Sign);
            tempOwner = GetUser(choice);

            do
            {
                OfferMenu offer = new OfferMenu();
                Console.WriteLine("----OFFER MENU----\n" +
                "1 - Get Company.\n" +
                "2 - Get Money.\n" +
                "3 - Offer Company.\n" +
                "4 - Offer Money.\n" +
                "5 - Send Offer.\n" +
                "6 - EXIT.\n");
                Console.Write("Enter your choice: ");
                Enum.TryParse(Console.ReadLine(), out offer);
                switch (offer)
                {
                    case OfferMenu.GetCompany:
                        {
                            Console.Write("Enter index of business that u want to get: ");
                            int.TryParse(Console.ReadLine(), out index);

                            if (AddAndOfferCompany(index, tempOwner, userWannaGet))
                            {
                                userWannaGet.Add(Field.FieldArray[index]);
                                Console.WriteLine("Company was added successfully.");
                            }
                        }
                        break;
                    case OfferMenu.GetMoney:
                        {
                            do
                            {
                                Console.Write("Enter sum of money that u want to get: ");
                                int.TryParse(Console.ReadLine(), out moneyToGet);
                            } while (moneyToGet < 0 || moneyToGet > tempOwner.Balance);
                        }
                        break;
                    case OfferMenu.OfferCompany:
                        {
                            Console.Write("Enter index of business that u want to offer: ");
                            int.TryParse(Console.ReadLine(), out index);

                            if (AddAndOfferCompany(index, tempOwner, userWannaGive))
                            {
                                userWannaGive.Add(Field.FieldArray[index]);
                                Console.WriteLine("Company was added successfully.");
                            }
                        }
                        break;
                    case OfferMenu.OfferMoney:
                        {
                            do
                            {
                                Console.Write("Enter sum of money that u want to offer: ");
                                int.TryParse(Console.ReadLine(), out moneyToOffer);
                            } while (moneyToOffer < 0 || moneyToOffer >= user.Balance);
                        }
                        break;
                    case OfferMenu.SendOffer:
                        {
                            if (DoesItPossibleToCreateOffer(new Offer(user, userWannaGive, moneyToOffer,
                                    tempOwner, userWannaGet, moneyToGet)))
                            {
                                if (SendOffer(cell, whatMenu, new Offer(user, userWannaGive, moneyToOffer,
                                        tempOwner, userWannaGet, moneyToGet)))
                                {
                                    ChangeCompanies(ShopIndexes, Colors, new Offer(user,userWannaGive,moneyToOffer,
                                        tempOwner,userWannaGet,moneyToGet));

                                    Console.WriteLine("Offer was accepted");
                                    Console.ReadLine();
                                    Field.ShowField(ShopIndexes, Colors, UsersInPrison);
                                    ShowUsersUnderField();
                                }
                                else Console.WriteLine("Bid failed.");
                                endOfOffer = true;
                            }
                            else
                            {
                                Console.WriteLine("U can't send offer, value of offer has to be not more than X2.");
                                endOfOffer = true;
                            }
                        }
                        break;
                    case OfferMenu.Exit:
                        endOfOffer = true;
                        break;
                    default:
                        Console.WriteLine("Error choice");
                        break;
                }
            } while (!endOfOffer);
        }
        public void PlayerSurrend(int index)
        {
            Console.WriteLine($"{Users[index].Name} surrended.");
            DeletePlayer(ShopIndexes, Colors, Users[index]);
            Users.RemoveAt(index);
        }
        public void StartGame()
        {
            Cell cell = null;
            MenuInGame menuInGame = new MenuInGame();
            ExtraMenu extraMenu = new ExtraMenu();
            Random random = new Random();
            User userOnAuction;
            ChanceRealization tempChance = 0;

            FillUserColors();
            Field.FieldFilling();
            bool needToSkipStep = false;
            int firstRes = 0;
            int secondRes = 0;
            int sumOfRes = 0;
            bool isEnd = false;
            bool notEnoughMoney = false;
            bool breakFreeFromJail = false;
            bool step;
            Field.InputAllSignsOnStart(Users);
            Field.ShowField(ShopIndexes, Colors, UsersInPrison);
            ShowUsersUnderField();

            do
            {
                for (int i = 0; i < Users.Count; i++)
                {
                    Users[i].WereUsedInStep = new List<TypeOfBusiness>();
                    if (Users.Count > 1)
                    {
                        step = false;
                        notEnoughMoney = false;
                        breakFreeFromJail = false;

                        while (!step) // Did user do step...
                        {
                            ///// PLAYERRR
                            if (Users[i].GetType() == typeof(Player) && !isEnd)
                            {
                                if (DoesUserNeedToSkipStep(Users[i], NeedToSkipStep))
                                {
                                    step = true;
                                    NeedToSkipStep.Remove(Users[i]);
                                    needToSkipStep = true;
                                }

                                else
                                {
                                    tempChance = 0;
                                    Console.WriteLine("----MENU----\n" +
                                        "1 - Pledge Business.\n" +
                                        "2 - Rebuy Business.\n" +
                                        "3 - Sell Branch.\n" +
                                        "4 - BuyBranch.\n" +
                                        "5 - Do Step.\n" +
                                        "6 - Offer.\n" +
                                        "7 - Surrend.\n" +
                                        "8 - Check balance.");
                                    Console.Write($"{Users[i].Name}, enter your choice: ");
                                    Enum.TryParse(Console.ReadLine(), out menuInGame);

                                    switch (menuInGame) // Player's menu
                                    {
                                        case MenuInGame.PledgeBusiness:
                                            PledgeForPlayer(Users[i]);
                                            break;
                                        case MenuInGame.RebuyBusiness:
                                            {
                                                Console.Write("Enter index of business that u wanna rebuy: ");
                                                int.TryParse(Console.ReadLine(), out int index);
                                                if (index >= 0 && index <= 39)
                                                {
                                                    if (Field.DoesItPossibleToRebuy(Users[i], index))
                                                    {
                                                        Cell pledgedCell = Field.PledgedOrRebuyBusiness(Users[i], index);
                                                        if (pledgedCell != null)
                                                        {
                                                            if (((BusinessType)pledgedCell).IsPledged)
                                                            {
                                                                if (Users[i].Balance >= ((BusinessType)pledgedCell).RedemptionOfBusiness)
                                                                {
                                                                    ((BusinessType)pledgedCell).IsPledged = false;
                                                                    ((BusinessType)pledgedCell).StepsDuringPledge = 15;
                                                                    Users[i].Balance -= ((BusinessType)pledgedCell).RedemptionOfBusiness;
                                                                    Console.WriteLine($"{Users[i].Name}, business is succsessfuly rebought.");
                                                                    Console.ReadLine();
                                                                    Field.ShowField(ShopIndexes, Colors, UsersInPrison);
                                                                    ShowUsersUnderField();
                                                                }
                                                                else Console.WriteLine("You don't have enough money");
                                                            }
                                                            else Console.WriteLine("Business isn't pledged");
                                                        }
                                                        else Console.WriteLine("Error");
                                                    }
                                                    else Console.WriteLine("Company don't exist or not belongs to you");
                                                }
                                                else Console.WriteLine("Incorrect index");
                                            }
                                            break;
                                        case MenuInGame.SellBranch:
                                            SellBranchForPlayer(Users[i]);
                                            break;
                                        case MenuInGame.BuyBranch:
                                            {
                                                Console.Write("Enter index of business: ");
                                                int.TryParse(Console.ReadLine(), out int index);
                                                if (index >= 0 && index <= 39)
                                                {
                                                    if (Field.DoesBusinessBelongsToUser(index, Users[i]))
                                                    {
                                                        if (DoesItPossibleToBuyBranch(Users[i], ((BusinessType)Field.FieldArray[index])))
                                                        {
                                                            Cell tempBranch = Field.GetBranchToBuy(index, Users[i]);
                                                            if (tempBranch != null)
                                                            {
                                                                if (DoesEnoughMoney(Users[i], ((BusinessType)tempBranch).PriceForImprovement))
                                                                {
                                                                    if (((BusinessType)tempBranch).LevelOfBusiness < 5)
                                                                    {
                                                                        if (!DoesUserBuiltBranchForThatType(Users[i], ((BusinessType)tempBranch).Type))
                                                                        {
                                                                            Users[i].Balance -= ((BusinessType)tempBranch).PriceForImprovement;
                                                                            ((BusinessType)tempBranch).LevelOfBusiness++;

                                                                            Users[i].WereUsedInStep.Add(((BusinessType)tempBranch).Type);
                                                                            Console.ReadLine();
                                                                            Field.ShowField(ShopIndexes, Colors, UsersInPrison);
                                                                            ShowUsersUnderField();
                                                                        }
                                                                        else Console.WriteLine("U have already built branch for that type...");
                                                                    }
                                                                    else Console.WriteLine("max level...");
                                                                }
                                                                else Console.WriteLine("Not enough money.");
                                                            }
                                                            else Console.WriteLine("You can't improve that cell.");

                                                        }
                                                        else Console.WriteLine("You can't build branch");
                                                    }
                                                    else Console.WriteLine("Incorrect index or business don't belongs to yours.");
                                                }
                                                else Console.WriteLine("Incorrect index");
                                            }
                                            break;
                                        case MenuInGame.DoStep:
                                            {
                                                bool wasInsideTempChance = false;
                                                do
                                                {
                                                    step = true;

                                                    if (!Field.DoSignInJail(UsersInPrison, Users[i].Sign))
                                                    {
                                                        if (!notEnoughMoney)
                                                        {
                                                            firstRes = random.Next(1, 7);
                                                            Console.WriteLine($"{Users[i].Name} got a - {firstRes}");
                                                            secondRes = random.Next(1, 7);
                                                            Console.WriteLine($"{Users[i].Name} got a - {secondRes}");
                                                            if (firstRes == secondRes) Users[i].CountOfDoubles++;
                                                            else Users[i].CountOfDoubles = 0;
                                                            sumOfRes = firstRes + secondRes;
                                                            Field.MoveSign(Users[i], sumOfRes);
                                                            cell = Field.WhereSignStopped(Users[i].Sign);
                                                            Console.WriteLine($"And stand on: {cell.Name}.");
                                                        }
                                                        if (Users[i].CountOfDoubles == 3)
                                                        {
                                                            Console.WriteLine($"{Users[i].Name} goes to jail.");
                                                            cell.UserSign.RemoveAt(cell.UserSign.Count - 1);
                                                            Field.AddSignInPrison(Users[i].Sign);
                                                            UsersInPrison.Add(Users[i]);
                                                            Users[i].CountOfDoubles = 0;
                                                        }

                                                        else
                                                        {
                                                            if(cell.GetType() != typeof(CellStart) &&
                                                               cell.GetType() != typeof(CellJail) &&
                                                               cell.GetType() != typeof(CellPoliceStation))
                                                            {
                                                                if (cell.GetType() == typeof(CellChance) && !wasInsideTempChance)
                                                                {
                                                                    tempChance = (ChanceRealization)random.Next(1, 13);
                                                                    switch (tempChance)
                                                                    {
                                                                        case ChanceRealization.PayForStudy:
                                                                            {
                                                                                Console.WriteLine($"{Users[i].Name} need to pay for education(1000).");
                                                                            }
                                                                            break;
                                                                        case ChanceRealization.PayForInsurance:
                                                                            {
                                                                                Console.WriteLine($"{Users[i].Name} need to pay for insurance(1250).");

                                                                            }
                                                                            break;
                                                                        case ChanceRealization.PayForCar:
                                                                            {
                                                                                Console.WriteLine($"{Users[i].Name} need to pay for damaged car(2000).");

                                                                            }
                                                                            break;
                                                                        case ChanceRealization.PayForCasino:
                                                                            {
                                                                                Console.WriteLine($"{Users[i].Name} need to pay for casino lost game(1k).");

                                                                            }
                                                                            break;
                                                                        case ChanceRealization.PayInBank:
                                                                            {
                                                                                Console.WriteLine($"{Users[i].Name} need to pay in bank(1500).");
                                                                            }
                                                                            break;
                                                                    }
                                                                    wasInsideTempChance = true;
                                                                }

                                                                if (cell.GetType() != typeof(CellChance) || (tempChance >= (ChanceRealization)1 && tempChance <= (ChanceRealization)5))
                                                                {
                                                                    if  (cell is BusinessType &&
                                                                        (((BusinessType)cell).OwnerOfBusiness == Users[i].Name ||
                                                                         ((BusinessType)cell).IsPledged))
                                                                        continue;
                                                                    else
                                                                    {
                                                                        do
                                                                        {
                                                                            Console.WriteLine("----EXTRA MENU----\n" +
                                                                                            "1 - Pledge Business.\n" +
                                                                                            "2 - Sell Branch.\n" +
                                                                                            "3 - Continue.\n" +
                                                                                            "4 - Offer.\n" +
                                                                                            "5 - Surrend.\n" +
                                                                                            "6 - Check balance.");
                                                                            Console.Write($"{Users[i].Name}, enter your choice: ");
                                                                            Enum.TryParse(Console.ReadLine(), out extraMenu);
                                                                            switch (extraMenu)
                                                                            {
                                                                                case ExtraMenu.PledgeBusiness:
                                                                                    PledgeForPlayer(Users[i]);
                                                                                    break;
                                                                                case ExtraMenu.SellBranch:
                                                                                    SellBranchForPlayer(Users[i]);
                                                                                    break;
                                                                                case ExtraMenu.Continue:
                                                                                    notEnoughMoney = false;
                                                                                    break;
                                                                                case ExtraMenu.Offer:
                                                                                    SendOfferFromPlayer(Users[i], 1, cell);
                                                                                    break;
                                                                                case ExtraMenu.Surrend:
                                                                                    {
                                                                                        PlayerSurrend(i);
                                                                                        step = true;
                                                                                        i--;
                                                                                    }
                                                                                    break;
                                                                                case ExtraMenu.CheckBalance:
                                                                                    Console.WriteLine($"{Users[i].Name}. Your balance: {Users[i].Balance}.");
                                                                                    break;
                                                                                default:
                                                                                    Console.WriteLine("Incorrect input...");
                                                                                    break;
                                                                            }
                                                                        } while (extraMenu != ExtraMenu.Continue && extraMenu != ExtraMenu.Surrend);
                                                                    }
                                                                }
                                                            }

                                                            if (extraMenu != ExtraMenu.Surrend)
                                                            {
                                                                if (cell.GetType() == typeof(Business) ||
                                                                cell.GetType() == typeof(GameBusiness) ||
                                                                cell.GetType() == typeof(CarBusiness))
                                                                {
                                                                    if (((BusinessType)cell).OwnerOfBusiness == "") // Немає власника
                                                                    {
                                                                        char choice = ' ';
                                                                        do
                                                                        {
                                                                            Console.Write($"{Users[i].Name}, would u like to buy {cell.Name}? (y/n):");
                                                                            Char.TryParse(Console.ReadLine().ToLower(), out choice);
                                                                        } while (choice != 'y' && choice != 'n');

                                                                        if (choice == 'y')
                                                                        {
                                                                            if (DoesEnoughMoney(Users[i], ((BusinessType)cell).Price))
                                                                            {
                                                                                Users[i].Balance -= ((BusinessType)cell).Price;
                                                                                ((BusinessType)cell).OwnerOfBusiness = Users[i].Name;
                                                                                Users[i].ListOfUserBusinesses.Add((BusinessType)cell);
                                                                                ShopIndexes.Add(cell);
                                                                                Colors.Add(Users[i].ConsoleColor);
                                                                            }
                                                                            else
                                                                            {
                                                                                Console.WriteLine($"{Users[i].Name} hasn't enough money.");
                                                                                step = false;
                                                                                notEnoughMoney = true;

                                                                            }
                                                                        }

                                                                        else if (choice == 'n')                 // Аукціон
                                                                        {
                                                                            Console.WriteLine("-----AUCTION-----");
                                                                            Console.WriteLine($"Auction for company - {cell.Name}");
                                                                            userOnAuction = Field.Auction(((BusinessType)cell).Price, Users[i], Users, cell);
                                                                            if (userOnAuction != null)
                                                                            {
                                                                                Console.WriteLine($"{userOnAuction.Name} won auction and get {cell.Name}.");
                                                                                ((BusinessType)cell).OwnerOfBusiness = userOnAuction.Name;
                                                                                ShopIndexes.Add(cell);
                                                                                Colors.Add(userOnAuction.ConsoleColor);
                                                                                userOnAuction.ListOfUserBusinesses.Add((BusinessType)cell);
                                                                            }
                                                                            else Console.WriteLine("All users declined to take part in auction.");
                                                                        }
                                                                    }

                                                                    else if (((BusinessType)cell).OwnerOfBusiness != "" &&
                                                                        Users[i].Name != ((BusinessType)cell).OwnerOfBusiness)   // Якщо вже є власник 
                                                                    {
                                                                        if (((BusinessType)cell).IsPledged == false) //поле не заложено
                                                                        {
                                                                            Console.WriteLine($"{Users[i].Name} pays for rent");
                                                                            if (cell.GetType() == typeof(GameBusiness))
                                                                            {
                                                                                if (DoesEnoughMoney(Users[i], ((BusinessType)cell).Rent[((BusinessType)cell).LevelOfBusiness] * (firstRes + secondRes)))
                                                                                {
                                                                                    Users[i].Balance -= ((BusinessType)cell).Rent[((BusinessType)cell).LevelOfBusiness] * (firstRes + secondRes);
                                                                                    Field.PayForRent(((BusinessType)cell).OwnerOfBusiness, ((BusinessType)cell).Rent[((BusinessType)cell).LevelOfBusiness] * (firstRes + secondRes), Users);
                                                                                }
                                                                                else
                                                                                {
                                                                                    Console.WriteLine($"{Users[i].Name} hasn't enough money.");
                                                                                    
                                                                                    if (!DoesEnoughMoneyToPayIfSellEverything(Users[i], ((BusinessType)cell).Rent[((BusinessType)cell).LevelOfBusiness] * (firstRes + secondRes)))
                                                                                    {
                                                                                        User tempUser = GetUserByName(((BusinessType)cell).OwnerOfBusiness);
                                                                                        if (tempUser != null) tempUser.Balance += GetAllMoney(Users[i]);

                                                                                         Console.WriteLine("U can't pay even if sell and pledge everything.");
                                                                                        Console.ReadLine();
                                                                                        DeletePlayer(ShopIndexes, Colors, Users[i]);
                                                                                        Console.WriteLine($"{Users[i].Name} surrended.");
                                                                                        Users.RemoveAt(i);
                                                                                        i--;
                                                                                        step = true;
                                                                                        extraMenu = ExtraMenu.Surrend;
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        step = false;
                                                                                        notEnoughMoney = true;
                                                                                    }
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                if (DoesEnoughMoney(Users[i], ((BusinessType)cell).Rent[((BusinessType)cell).LevelOfBusiness]))
                                                                                {
                                                                                    Users[i].Balance -= ((BusinessType)cell).Rent[((BusinessType)cell).LevelOfBusiness];
                                                                                    Field.PayForRent(((BusinessType)cell).OwnerOfBusiness, ((BusinessType)cell).Rent[((BusinessType)cell).LevelOfBusiness], Users);
                                                                                }
                                                                                else
                                                                                {
                                                                                    Console.WriteLine($"{Users[i].Name} doesn't has enough money.");
                                                                                    if (!DoesEnoughMoneyToPayIfSellEverything(Users[i], ((BusinessType)cell).Rent[((BusinessType)cell).LevelOfBusiness]))
                                                                                    {
                                                                                        User tempUser = GetUserByName(((BusinessType)cell).OwnerOfBusiness);
                                                                                        if (tempUser != null) tempUser.Balance += GetAllMoney(Users[i]);

                                                                                        Console.WriteLine("U can't pay even if sell and pledge everything.");
                                                                                        Console.ReadLine();
                                                                                        DeletePlayer(ShopIndexes, Colors, Users[i]);
                                                                                        Console.WriteLine($"{Users[i].Name} surrended.");
                                                                                        Users.RemoveAt(i);
                                                                                        i--;
                                                                                        step = true;
                                                                                        extraMenu = ExtraMenu.Surrend;
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        step = false;
                                                                                        notEnoughMoney = true;
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                        else Console.WriteLine("Cell is pledged"); // Поле заложено
                                                                    }
                                                                    else Console.WriteLine($"{Users[i].Name} is standing on his business");
                                                                }

                                                                else if (cell.GetType() == typeof(CellBank))
                                                                {
                                                                    if (DoesEnoughMoney(Users[i], 2000))
                                                                    {
                                                                        Console.WriteLine($"{Users[i].Name} paid 2k due to u got bank");
                                                                        Users[i].Balance -= 2000;
                                                                    }
                                                                    else
                                                                    {
                                                                        Console.WriteLine($"{Users[i].Name} doesn't has enough money.");
                                                                        step = false;
                                                                        notEnoughMoney = true;
                                                                    }
                                                                }

                                                                else if (cell.GetType() == typeof(CellTax))
                                                                {
                                                                    if (DoesEnoughMoney(Users[i], 1000))
                                                                    {
                                                                        Console.WriteLine("You paid 1k due to u got tax");
                                                                        Users[i].Balance -= 1000;
                                                                    }
                                                                    else
                                                                    {
                                                                        Console.WriteLine($"{Users[i].Name} doesn't has enough money.");
                                                                        step = false;
                                                                        notEnoughMoney = true;
                                                                    }
                                                                }

                                                                else if (cell.GetType() == typeof(CellJail))
                                                                {
                                                                    Console.WriteLine($"{Users[i].Name} decided to visit friends in jail :)");
                                                                }

                                                                else if (cell.GetType() == typeof(CellPoliceStation))
                                                                {
                                                                    Console.WriteLine($"{Users[i].Name} goes to jail.");
                                                                    cell.UserSign.RemoveAt(cell.UserSign.Count - 1);
                                                                    Field.AddSignInPrison(Users[i].Sign);
                                                                    UsersInPrison.Add(Users[i]);
                                                                }

                                                                else if (cell.GetType() == typeof(CellChance))
                                                                {
                                                                    Console.WriteLine($"{Users[i].Name} got chance");
                                                                    ChanceRealization chanceRealization = new ChanceRealization();
                                                                    if (tempChance != 0) chanceRealization = tempChance;

                                                                    else chanceRealization = (ChanceRealization)random.Next(1, 13);

                                                                    switch (chanceRealization)
                                                                    {
                                                                        case ChanceRealization.PayForStudy:
                                                                            {
                                                                                Console.WriteLine($"{Users[i].Name} need to pay for education(1000).");
                                                                                if (DoesEnoughMoney(Users[i], 1000)) Users[i].Balance -= 1000;
                                                                                else
                                                                                {
                                                                                    Console.WriteLine($"{Users[i].Name} hasn't enough money.");
                                                                                    step = false;
                                                                                    notEnoughMoney = true;
                                                                                }
                                                                            }
                                                                            break;
                                                                        case ChanceRealization.PayForInsurance:
                                                                            {
                                                                                Console.WriteLine($"{Users[i].Name} need to pay for insurance(1250).");
                                                                                if (DoesEnoughMoney(Users[i], 1250)) Users[i].Balance -= 1250;

                                                                                else
                                                                                {
                                                                                    Console.WriteLine($"{Users[i].Name} hasn't enough money.");
                                                                                    step = false;
                                                                                    notEnoughMoney = true;
                                                                                }
                                                                            }
                                                                            break;
                                                                        case ChanceRealization.PayForCar:
                                                                            {
                                                                                Console.WriteLine($"{Users[i].Name} need to pay for damaged car(2000).");
                                                                                if (DoesEnoughMoney(Users[i], 2000)) Users[i].Balance -= 2000;
                                                                                else
                                                                                {
                                                                                    Console.WriteLine($"{Users[i].Name} hasn't enough money.");
                                                                                    step = false;
                                                                                    notEnoughMoney = true;
                                                                                }
                                                                            }
                                                                            break;
                                                                        case ChanceRealization.PayForCasino:
                                                                            {
                                                                                Console.WriteLine($"{Users[i].Name} need to pay for casino lost game(1k).");
                                                                                if (DoesEnoughMoney(Users[i], 1000)) Users[i].Balance -= 1000;
                                                                                else
                                                                                {
                                                                                    Console.WriteLine($"{Users[i].Name} hasn't enough money.");
                                                                                    step = false;
                                                                                    notEnoughMoney = true;
                                                                                }
                                                                            }
                                                                            break;
                                                                        case ChanceRealization.PayInBank:
                                                                            {
                                                                                Console.WriteLine($"{Users[i].Name} need to pay in bank(1500).");
                                                                                if (DoesEnoughMoney(Users[i], 1500)) Users[i].Balance -= 1500;
                                                                                else
                                                                                {
                                                                                    Console.WriteLine($"{Users[i].Name} hasn't enough money.");
                                                                                    step = false;
                                                                                    notEnoughMoney = true;
                                                                                }
                                                                            }
                                                                            break;
                                                                        case ChanceRealization.EarnFromTax:
                                                                            {
                                                                                Console.WriteLine($"{Users[i].Name} gets 500 from tax.");
                                                                                Users[i].Balance += 500;
                                                                            }
                                                                            break;
                                                                        case ChanceRealization.EarnFromCasting:
                                                                            {
                                                                                Console.WriteLine($"{Users[i].Name} gets 1k from casting due to got second place.");
                                                                                Users[i].Balance += 1000;
                                                                            }
                                                                            break;
                                                                        case ChanceRealization.EarnFromBank:
                                                                            {
                                                                                Console.WriteLine($"{Users[i].Name} gets from bank 1k.");
                                                                                Users[i].Balance += 1000;
                                                                            }
                                                                            break;
                                                                        case ChanceRealization.EarnFromSportCompetition:
                                                                            {
                                                                                Console.WriteLine($"{Users[i].Name} gets 500 from sport competition due to got third place.");
                                                                                Users[i].Balance += 500;
                                                                            }
                                                                            break;
                                                                        case ChanceRealization.EarnFromBirthday:
                                                                            {
                                                                                Console.WriteLine($"{Users[i].Name} gets 1k from his birthday.");
                                                                                Users[i].Balance += 1000;
                                                                            }
                                                                            break;
                                                                        case ChanceRealization.MissStep:
                                                                            {
                                                                                Console.WriteLine($"{Users[i].Name} lost his dices and will skip step");
                                                                                NeedToSkipStep.Add(Users[i]);
                                                                            }
                                                                            break;
                                                                        case ChanceRealization.GotoJail:
                                                                            {
                                                                                Console.WriteLine($"{Users[i].Name} goes to jail .");
                                                                                cell.UserSign.RemoveAt(cell.UserSign.Count - 1);
                                                                                Field.AddSignInPrison(Users[i].Sign);
                                                                                UsersInPrison.Add(Users[i]);
                                                                            }
                                                                            break;
                                                                    }
                                                                }

                                                                else if (cell.GetType() == typeof(CellJackpot))
                                                                {
                                                                    Console.WriteLine($"{Users[i].Name} get casino.");
                                                                    char userChoice = ' ';
                                                                    do
                                                                    {
                                                                        Console.Write("Would u like to take part in casino[y/n]: ");
                                                                        Char.TryParse(Console.ReadLine(), out userChoice);
                                                                    } while (userChoice != 'y' && userChoice != 'n');

                                                                    if (userChoice == 'y')
                                                                    {
                                                                        if (DoesEnoughMoney(Users[i], 1000))
                                                                        {
                                                                            Users[i].Balance -= 1000;
                                                                            List<int> countOfCubes = new List<int>();
                                                                            int userValueOfCube = 0;

                                                                            Console.WriteLine("You can choose 3 dices.");
                                                                            do
                                                                            {
                                                                                Console.Write("Enter value(value to stop = 0): ");
                                                                                int.TryParse(Console.ReadLine(), out userValueOfCube);
                                                                                if (userValueOfCube >= 1 && userValueOfCube <= 6)
                                                                                {
                                                                                    if (countOfCubes.Count < 3 && !DoesExistInList(countOfCubes, userValueOfCube))
                                                                                    {
                                                                                        Console.WriteLine("Dice was successfully added to list.");
                                                                                        countOfCubes.Add(userValueOfCube);
                                                                                    }

                                                                                    else Console.WriteLine("You added maximum count of dices or that dice already exist in list.");
                                                                                }
                                                                                else if (userValueOfCube == 0 && countOfCubes.Count == 0) Console.WriteLine("You must enter 1 value of cube");

                                                                                else Console.WriteLine("Incorrect value of cube...");
                                                                            } while (userValueOfCube != 0 || countOfCubes.Count < 1);

                                                                            if (DoesUserWonInCasino(countOfCubes))
                                                                            {
                                                                                if (countOfCubes.Count == 3) Users[i].Balance += 2000;

                                                                                else if (countOfCubes.Count == 2) Users[i].Balance += 3000;

                                                                                else if (countOfCubes.Count == 1) Users[i].Balance += 6000;

                                                                                Console.WriteLine($"{Users[i].Name} won in casino");
                                                                            }
                                                                            else
                                                                            {
                                                                                Console.Write("His dices were: ");
                                                                                for (int j = 0; j < countOfCubes.Count; j++)
                                                                                    Console.Write($"{countOfCubes[j]} ");

                                                                                Console.WriteLine($"{Users[i].Name} lost in casino");
                                                                            }
                                                                        }
                                                                        else Console.WriteLine($"{Users[i].Name} hasn't enough money.");
                                                                    }
                                                                    else Console.WriteLine($"{Users[i].Name} refused to take part in casino");
                                                                }

                                                                else if (cell.GetType() == typeof(CellStart)) Console.WriteLine("Stand on start and get 1k");
                                                            }
                                                        }
                                                    }

                                                    else //in jail
                                                    {
                                                        int userChoice = -1;
                                                        if (Users[i].TriesToBreakFree < 3)
                                                        {
                                                            Console.WriteLine("\n1 - Same result on dices.\n" +
                                                            "2 - Pay 500.");
                                                            do
                                                            {
                                                                Console.Write("What type would u like to break free: ");
                                                                int.TryParse(Console.ReadLine(), out userChoice);
                                                            } while (userChoice != 1 && userChoice != 2);
                                                        }
                                                        else if (Users[i].TriesToBreakFree >= 3) userChoice = 1;


                                                        if (userChoice == 1)
                                                        {
                                                            Users[i].TriesToBreakFree++;
                                                            if(Users[i].TriesToBreakFree < 3)
                                                            {
                                                                firstRes = random.Next(1, 7);
                                                                Console.WriteLine($"{Users[i].Name} got a - {firstRes}");
                                                                secondRes = random.Next(1, 7);
                                                                Console.WriteLine($"{Users[i].Name} got a - {secondRes}");

                                                                if (firstRes == secondRes)
                                                                {
                                                                    Users[i].TriesToBreakFree = 0;
                                                                    Field.DeleteSignFromJail(UsersInPrison, Users[i].Sign);
                                                                    breakFreeFromJail = true;
                                                                }
                                                            }
                                                            
                                                            
                                                            else if(Users[i].TriesToBreakFree >= 3)
                                                            {
                                                                Console.WriteLine("You have already tried 3 times...\n" +
                                                                "You have to pay.");
                                                                if (DoesEnoughMoney(Users[i], 500))
                                                                {
                                                                    Users[i].Balance -= 500;
                                                                    Users[i].TriesToBreakFree = 0;
                                                                    Field.DeleteSignFromJail(UsersInPrison, Users[i].Sign);
                                                                    breakFreeFromJail = true;
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("Not enough money.");
                                                                    step = false;
                                                                }
                                                            }
                                                        }
                                                        else if (userChoice == 2)
                                                        {
                                                            if(DoesEnoughMoney(Users[i], 500))
                                                            {
                                                                Users[i].Balance -= 500;
                                                                Users[i].TriesToBreakFree = 0;
                                                                Field.DeleteSignFromJail(UsersInPrison, Users[i].Sign);
                                                                breakFreeFromJail = true;
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Not enough money.");
                                                                step = false;
                                                            }
                                                        }
                                                    } //jail

                                                } while (notEnoughMoney && extraMenu != ExtraMenu.Surrend);
                                            }
                                            break;
                                        case MenuInGame.Offer:
                                            SendOfferFromPlayer(Users[i], 0, cell);
                                            break;
                                        case MenuInGame.Surrend:
                                            {
                                                PlayerSurrend(i);
                                                step = true;
                                                i--;
                                            }
                                            break;
                                        case MenuInGame.CheckBalance:
                                            Console.WriteLine($"{Users[i].Name}. Your balance: {Users[i].Balance}.");
                                            break;
                                    }
                                }

                                if (menuInGame != MenuInGame.Surrend && extraMenu != ExtraMenu.Surrend)
                                {
                                    LevelOfCarsAndGames(Users[i]);
                                    
                                    if (breakFreeFromJail)
                                    {
                                        Console.WriteLine($"{Users[i].Name} broke free :)");
                                        i--;
                                    }
                                    else if ((firstRes == secondRes && step && !notEnoughMoney) && !Field.DoSignInJail(UsersInPrison, Users[i].Sign) && !DoesUserNeedToSkipStep(Users[i], NeedToSkipStep))
                                    {
                                        Console.WriteLine($"Player {Users[i].Name} got same results on dices, he will go 1 more time");
                                        i--;
                                    }
                                }
                                
                                ///Console.ReadLine();
                                ///Console.Clear();
                            }

                            ///// BOT
                            else if (Users[i].GetType() == typeof(Bot) && !isEnd)
                            {
                                if (DoesUserNeedToSkipStep(Users[i], NeedToSkipStep))
                                {
                                    NeedToSkipStep.Remove(Users[i]);
                                    needToSkipStep = true;
                                }

                                else
                                {
                                    Console.WriteLine($"{Users[i].Name} is making a choice...");
                                    ((Bot)Users[i]).BotStep(this, ref i);
                                }
                                step = true;
                                ///Console.ReadLine();
                                ///Console.Clear();
                            }
                        }
                        
                        if (!needToSkipStep && step)
                        {
                            Console.ReadLine();
                            Field.ShowField(ShopIndexes, Colors, UsersInPrison);
                            ShowUsersUnderField();
                            Console.ReadLine();
                        }
                        needToSkipStep = false;
                    }
                    else isEnd = true;
                }
                MinusStepDuringPledge(Users, ShopIndexes, Colors);
            } while (!isEnd);

            Console.WriteLine($"{Users[0].Name} won!!!");
            Users.RemoveAt(0);
            ShopIndexes.Clear();
            Colors.Clear();
        }
        public bool DoesEnoughMoneyToPayIfSellEverything(User user, int price)
        {
            int tempSum = user.Balance;
            for (int i = 0; i < user.ListOfUserBusinesses.Count; i++)
            {
                if (!user.ListOfUserBusinesses[i].IsPledged && user.ListOfUserBusinesses[i].GetType() == typeof(Business))
                {
                    int levels = user.ListOfUserBusinesses[i].LevelOfBusiness;
                    while (levels > 0)
                    {
                        tempSum += user.ListOfUserBusinesses[i].PriceForImprovement;
                        levels--;
                    }
                }
                if (!user.ListOfUserBusinesses[i].IsPledged) tempSum += user.ListOfUserBusinesses[i].PledgeOfBusiness;

                if (tempSum >= price) return true;
            }
            return false;
        }        
        public bool DoesUserBuiltBranchForThatType(User user, TypeOfBusiness type)
        {
            if (user.WereUsedInStep.Count == 0) return false;
            for (int i = 0; i < user.WereUsedInStep.Count; i++)
            {
                if (user.WereUsedInStep[i] == type) return true;
            }
            return false;
        }

        public bool DoesExistInList(List<int> resOfDices, int res)
        {
            for (int i = 0; i < resOfDices.Count; i++)
            {
                if (resOfDices[i] == res) return true;
            }
            return false;
        }
        public bool DoesLevelOfThatTypeEqualsZero(TypeOfBusiness type)
        {
            for (int i = 0; i < Field.FieldArray.Count; i++)
            {
                if (Field.FieldArray[i].GetType() == typeof(Business))
                {
                    if (((BusinessType)Field.FieldArray[i]).Type == type && 
                        ((BusinessType)Field.FieldArray[i]).LevelOfBusiness != 0) return false;
                    
                }
            }
            return true;
        }
        public bool DoesItPossibleToCreateOffer(Offer offer)
        {
            int sumToGet = 0, sumToOffer = 0;
            for (int i = 0; i < offer.WantsToGet.Count; i++)
            {
                sumToGet += ((BusinessType)offer.WantsToGet[i]).Price;
            }
            sumToGet += offer.SumOfMoneyThatWantsToGet;
            for (int i = 0; (i < offer.WantsToGive.Count); i++)
            {
                sumToOffer += ((BusinessType)offer.WantsToGive[i]).Price;
            }
            sumToOffer += offer.SumOfMoneyThatWantsToGive;

            if (sumToGet > 0 && sumToOffer > 0)
            {
                return sumToGet / sumToOffer <= 2 || sumToOffer / sumToGet <= 2;
            }

            return false;
            
        }
        public void DeletePlayer(List<Cell> shopIndexes, List<ConsoleColor> colors, User user)
        {
            for (int i = 0; i < shopIndexes.Count; i++)
            {
                if (((BusinessType)shopIndexes[i]).OwnerOfBusiness == user.Name)
                {
                    shopIndexes.RemoveAt(i);
                    colors.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0;i < user.ListOfUserBusinesses.Count; i++)
            {
                user.ListOfUserBusinesses[i].IsPledged = false;
                user.ListOfUserBusinesses[i].LevelOfBusiness = 0;
                user.ListOfUserBusinesses[i].StepsDuringPledge = 15;
                user.ListOfUserBusinesses[i].OwnerOfBusiness = "";
            }

            for (int i = 0; i < Field.FieldArray.Count; i++)
            {
                for (int j = 0; j < Field.FieldArray[i].UserSign.Count; j++)
                {
                    if (Field.FieldArray[i].UserSign[j] == user.Sign)
                    {
                        Field.FieldArray[i].UserSign.RemoveAt(j);
                        if (Field.FieldArray[i] is BusinessType)
                        {
                            User oponent = GetUserByName(((BusinessType)Field.FieldArray[i]).OwnerOfBusiness);
                            if(oponent != null) oponent.Balance += GetAllMoney(user);
                        }
                    }
                }
            }
        }
        public bool SendOffer(Cell wherePlayerStopped, int whatMenu, Offer offer) // 0 - main menu; 1 - extra menu
        {
            int sumOfMoneyThatWantToGive = offer.SumOfMoneyThatWantsToGive;
            int sumOfMoneyThatWantToGet = offer.SumOfMoneyThatWantsToGet;
            Console.WriteLine($"Dear {offer.Receiver.Name} - {offer.Sender.Name} offers:");
            
            if (offer.WantsToGive != null && offer.WantsToGive.Count > 0)
            {
                Console.Write("His:");
                for (int i = 0; i < offer.WantsToGive.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {offer.WantsToGive[i].Name}");
                    sumOfMoneyThatWantToGive += ((BusinessType)offer.WantsToGive[i]).Price;
                }
            }
            if (offer.SumOfMoneyThatWantsToGive > 0) Console.WriteLine($"Money - {offer.SumOfMoneyThatWantsToGive}");

            Console.Write("On your:");
            if (offer.WantsToGet != null && offer.WantsToGet.Count > 0 && offer.Receiver != null)
            {
                for (int i = 0; i < offer.WantsToGet.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {offer.WantsToGet[i].Name}");
                    sumOfMoneyThatWantToGet += ((BusinessType)offer.WantsToGet[i]).Price;
                }
            }
            if (offer.SumOfMoneyThatWantsToGet > 0) Console.WriteLine($"Money - {offer.SumOfMoneyThatWantsToGet}");
            
            if (offer.Receiver != null)
            {
                char choice = ' ';
                if (offer.Receiver.GetType() == typeof(Player))
                {
                    do
                    {
                        Console.Write($"{offer.Receiver.Name} would u like to accept(y/n): ");
                        char.TryParse(Console.ReadLine().ToLower(), out choice);
                    } while (choice != 'y' && choice != 'n');
                }

                else
                {
                    int countOfAlmostMonopoly = 0;
                    int countOfMonopoly = 0;

                    if (((Bot)offer.Receiver).DoesBotNeedToAcceptOffer(offer.WantsToGive, offer.Receiver, ref countOfMonopoly, Field) &&
                        (sumOfMoneyThatWantToGive >= sumOfMoneyThatWantToGet))
                    {
                        if(!((Bot)offer.Receiver).WillAnotherUserGetMoreMonopolies(offer.WantsToGet, offer.Sender, ref countOfMonopoly, Field)) choice = 'y';
                    }

                    else if (((Bot)offer.Receiver).DoesBotNeedLessImportantOffer(offer.WantsToGive, offer.Receiver, ref countOfAlmostMonopoly, Field) &&
                             !((Bot)offer.Receiver).WillUserGetMonopolyFromOffer(offer.WantsToGet, offer.Sender, Field) &&
                             (sumOfMoneyThatWantToGive >= sumOfMoneyThatWantToGet))
                    {
                        if (!((Bot)offer.Receiver).WillUserGetMoreAlmostMonopoly(offer.WantsToGet, offer.Sender, ref countOfAlmostMonopoly, Field)) 
                        {
                            if (whatMenu == 0) choice = 'y';
                            else if (whatMenu == 1 && !DoesTypeOfCellExistInList(offer.WantsToGet, wherePlayerStopped)) choice = 'y';
                        } 
                    }

                    else if (((Bot)offer.Receiver).DoesItEqualTrade(offer.WantsToGive, offer.WantsToGet, offer.Receiver, offer.Sender) &&
                             (sumOfMoneyThatWantToGive >= sumOfMoneyThatWantToGet)) choice = 'y';

                    else if (sumOfMoneyThatWantToGet < sumOfMoneyThatWantToGive &&
                             offer.WantsToGet.Count == 0 &&
                             offer.WantsToGive.Count == 0) choice = 'y';
                }
                if (choice == 'y') return true;
            }
            return false;
        }
        public bool DoesTypeOfCellExistInList(List<Cell> wantsToGet, Cell wherePlayerStopped)
        {
            for (int i = 0; i < wantsToGet.Count; i++)
            {
                if (((BusinessType)wantsToGet[i]).Type == ((BusinessType)wherePlayerStopped).Type) return true;
            }
            return false;
        }

        public void Change(User user, User tempOwner, Cell business, ConsoleColor color, Cell shopIndex)
        {
            user.ListOfUserBusinesses.Remove(((BusinessType)business));
            tempOwner.ListOfUserBusinesses.Add(((BusinessType)business));

            ((BusinessType)shopIndex).OwnerOfBusiness = tempOwner.Name;
            color = tempOwner.ConsoleColor;
        }
        public void ChangeCompanies(List<Cell> shopIndexes, List<ConsoleColor> colors, Offer offer)
        {
            offer.Receiver.Balance += offer.SumOfMoneyThatWantsToGive;
            offer.Receiver.Balance -= offer.SumOfMoneyThatWantsToGet;

            offer.Sender.Balance += offer.SumOfMoneyThatWantsToGet;
            offer.Sender.Balance -= offer.SumOfMoneyThatWantsToGive;

            if(offer.WantsToGive.Count > 0)
            {
                for (int i = 0; i < offer.WantsToGive.Count; i++)
                {
                    for (int j = 0; j < shopIndexes.Count; j++)
                    {
                        if (offer.WantsToGive[i].Name == shopIndexes[j].Name)
                            Change(offer.Sender, offer.Receiver, offer.WantsToGive[i], colors[j], shopIndexes[j]);
                    }
                }
            }
                
            if (offer.WantsToGet.Count > 0)
            {
                for (int i = 0; i < offer.WantsToGet.Count; i++)
                {
                    for (int j = 0; j < shopIndexes.Count; j++)
                    {
                        if (offer.WantsToGet[i].Name == shopIndexes[j].Name)
                            Change(offer.Receiver, offer.Sender, offer.WantsToGet[i], colors[j], shopIndexes[j]);
                    }
                }
            }
            
        }
        public bool DoesThatSellExistInList(Cell cell, List<Cell> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Name == cell.Name) return true;
            }
            return false;
        }
        public bool DoesEnoughMoney(User user, int priceForSmth)
        {
            return user.Balance >= priceForSmth;
        }

        public bool DoesUserNeedToSkipStep(User user, List<User> usersToSkip)
        {
            for (int i = 0; i < usersToSkip.Count; i++)
            {
                if (usersToSkip[i].Sign == user.Sign) return true;
            }
            return false;
        }
        public void LevelOfCarsAndGames(User user)
        {
            int countOfCars = 0;
            int gameBusinessCount = 0;
            for (int i = 0; i < user.ListOfUserBusinesses.Count; i++)
            {
                if (user.ListOfUserBusinesses[i].GetType() == typeof(CarBusiness)) countOfCars++;
                else if (user.ListOfUserBusinesses[i].GetType() == typeof(GameBusiness)) gameBusinessCount++;
            }

            for (int i = 0; i < user.ListOfUserBusinesses.Count; i++)
            {
                if (user.ListOfUserBusinesses[i].GetType() == typeof(CarBusiness)) 
                    user.ListOfUserBusinesses[i].LevelOfBusiness = countOfCars - 1;
                else if (user.ListOfUserBusinesses[i].GetType() == typeof(GameBusiness))
                    user.ListOfUserBusinesses[i].LevelOfBusiness = gameBusinessCount - 1;
            }
        }
        public bool DoesUserWonInCasino(List<int> resultsOfDice)
        {
            Random random = new Random();
            int resInCasino = random.Next(1, 7);
            Console.WriteLine($"Won dice: {resInCasino}.");
            for (int i = 0; i < resultsOfDice.Count; i++)
            {
                if (resInCasino == resultsOfDice[i]) return true;
            }
            return false;
        }

        public void MinusStepDuringPledge(List<User> users, List<Cell> boughtShops, List<ConsoleColor> shops)
        {
            for (int i = 0; i < users.Count; i++)
            {
                for (int j = 0; j < users[i].ListOfUserBusinesses.Count; j++)
                {
                    if (users[i].ListOfUserBusinesses[j].IsPledged) users[i].ListOfUserBusinesses[j].StepsDuringPledge -= 1;
                    

                    if (users[i].ListOfUserBusinesses[j].StepsDuringPledge == 0)
                    {
                        DeleteFromBoughtShopsAndColors(users[i].ListOfUserBusinesses[j].Name, boughtShops, shops);
                        users[i].ListOfUserBusinesses[j].StepsDuringPledge = _maxCountOfCyclesInPledge;
                        users[i].ListOfUserBusinesses[j].LevelOfBusiness = 0;
                        users[i].ListOfUserBusinesses[j].IsPledged = false;
                        users[i].ListOfUserBusinesses[j].OwnerOfBusiness = "";
                        users[i].ListOfUserBusinesses.RemoveAt(j);
                    }
                }
            }
        }
        public void DeleteFromBoughtShopsAndColors(string nameOfBusiness, List<Cell> boughtShops, List<ConsoleColor> shops)
        {
            for (int i = 0; i < boughtShops.Count; i++)
            {
                if (boughtShops[i].Name == nameOfBusiness)
                {
                    boughtShops.RemoveAt(i);
                    shops.RemoveAt(i);
                    break;
                }
            }
        }
        public bool CanIPledgeFromBranchTypes(User user, TypeOfBusiness type)
        {
            for (int i = 0; i < user.ListOfUserBusinesses.Count; i++)
            {
                if (type == user.ListOfUserBusinesses[i].Type && user.ListOfUserBusinesses[i].LevelOfBusiness > 0) return false;
            }
            return true;
        }
    }
}