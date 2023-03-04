using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monopoly_
{
    public class Bot : User
    {
        public List<TypeOfBusiness> BranchTypes { get; set; }

        private const int _maxLevel = 5;
        private const int _stepsForRebuy = 7;
        private const int _x2 = 2;
        private const int _checkingForOffer = 13;
        private Random random;

        public Bot(string name, char sign) : base(name, sign)
        {
            Name = name;
            Sign = sign;
            Balance = 15000;
            BranchTypes = new List<TypeOfBusiness>();
            random = new Random();
            WereUsedInStep = new List<TypeOfBusiness>();
            CountOfDoubles = 0;
            SentOffers = new List<Offer>();
        }

        private int GetAllSum()
        {
            int sum = Balance;
            for (int i = 0; i < ListOfUserBusinesses.Count; i++)
            {
                if (!ListOfUserBusinesses[i].IsPledged && ListOfUserBusinesses[i].GetType() == typeof(Business))
                {
                    int levels = ListOfUserBusinesses[i].LevelOfBusiness;
                    while (levels > 0)
                    {
                        sum += ListOfUserBusinesses[i].PriceForImprovement;
                        levels--;
                    }
                }
                if (!ListOfUserBusinesses[i].IsPledged) sum += ListOfUserBusinesses[i].PledgeOfBusiness;
            }
            return sum;
        }
        private User GetUserWhereBotStopped(Game game)
        {
            for (int i = 0; i < game.Field.FieldArray.Count; i++)
            {
                for (int j = 0; j < game.Field.FieldArray[i].UserSign.Count; j++)
                {
                    if (game.Field.FieldArray[i].UserSign[j] == Sign)
                    {
                        if (game.Field.FieldArray[i] is BusinessType)
                            return GetUserByName(game.Users, ((BusinessType)game.Field.FieldArray[i]).OwnerOfBusiness);

                        break;
                    }
                }
            }
            return null;
        }
        private bool DoesCountOfShopsEqualsBranch(TypeOfBusiness type)
        {
            if (((type == TypeOfBusiness.Perfumery || type == TypeOfBusiness.Electronics ||
                  type == TypeOfBusiness.GameDevelopment) && CountOfOneTypeCompany(this, type) == 2) ||
                (type == TypeOfBusiness.Cars && CountOfOneTypeCompany(this, type) == 4) ||
                (CountOfOneTypeCompany(this, type) == 3 && type != TypeOfBusiness.Cars)) return true;

            return false;
        }
        private bool CanIPledgeFromBranchTypes(TypeOfBusiness type)
        {
            for (int i = 0; i < ListOfUserBusinesses.Count; i++)
            {
                if (type == ListOfUserBusinesses[i].Type && 
                    ListOfUserBusinesses[i].LevelOfBusiness > 0) return false;
            }
            return true;
        }
        private bool DoesEnoughMoneyToPayIfSellEverything(int price)
        {
            return GetAllSum() >= price;
        }
        private User GetUserByName(List<User> users, string name)
        {
            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].Name == name) return users[i];
            }
            return null;
        }
        private string GetNameOfOpponentWhereBotStopped(Field field)
        {
            for (int i = 0; i < field.FieldArray.Count; i++)
            {
                if (field.FieldArray[i] is BusinessType)
                {
                    for (int j = 0; j < field.FieldArray[i].UserSign.Count; j++)
                    {
                        if (field.FieldArray[i].UserSign[j] == Sign) 
                            return ((BusinessType)field.FieldArray[i]).OwnerOfBusiness;
                    }
                }
            }
            return null;
        }
        private void RemoveSignAfterDeleting(Field field)
        {
            for (int i = 0; i < field.FieldArray.Count; i++)
            {
                for (int j = 0; j < field.FieldArray[i].UserSign.Count; j++)
                {
                    if (field.FieldArray[i].UserSign[j] == Sign)
                    {
                        field.FieldArray[i].UserSign.RemoveAt(j);
                        break;
                    }
                }
            }
        }
        private void LevelOfCarsAndGames()
        {
            int countOfCars = 0;
            int gameBusinessCount = 0;
            for (int i = 0; i < ListOfUserBusinesses.Count; i++)
            {
                if (ListOfUserBusinesses[i].GetType() == typeof(CarBusiness)) countOfCars++;
                else if (ListOfUserBusinesses[i].GetType() == typeof(GameBusiness)) gameBusinessCount++;
            }

            for (int i = 0; i < ListOfUserBusinesses.Count; i++)
            {
                if (ListOfUserBusinesses[i].GetType() == typeof(CarBusiness)) ListOfUserBusinesses[i].LevelOfBusiness = countOfCars - 1;
                else if (ListOfUserBusinesses[i].GetType() == typeof(GameBusiness)) ListOfUserBusinesses[i].LevelOfBusiness = gameBusinessCount - 1;
            }
        }
        public void BotStep(Game game, ref int currentElementOfBot)
        {
            Random random1 = new Random();
            ChanceRealization tempChance = 0;
            Cell tempCell;
            List<Cell> lowPlegdeCount = new List<Cell>();
            bool blockOnChance = false;
            bool botStep = false;
            bool botNeedMoney = false;
            bool resOfDices = false;
            int tempFirstRes = -1;
            int tempSecondRes = -2;
            int sumThatBotNeed = 0;
            bool blockToBuildBranchOrRebuy = false;

            do
            {
                int countOfBranchTypes = GetCountOfBranchesOnMap(game.Field); // к-сть монополій для якогось певного бота
                if (DoesItPossibleToBuyBranch(game.Field) && countOfBranchTypes != this.WereUsedInStep.Count && !blockToBuildBranchOrRebuy && !DoesBotExistInPrison(game)) // does it possible to buy branch
                {
                    int i = 0;
                    while (i < countOfBranchTypes) // поки іте менше за к-сть знайдених монополій
                    {
                        TypeOfBusiness businessType = GetType(game.Field); // ортимуєм тип монополії
                        if (businessType != 0)
                        {
                            this.WereUsedInStep.Add(businessType); // додаєм ті типи які вже використовувались під час ходу

                            if (DoTypeExistAsBranch(businessType)) ((Bot)this).BranchTypes.Add(businessType);

                            tempCell = GetBranchToBuy(businessType); // отримуєм компанію

                            if (tempCell != null)
                            {
                                if (DoesEnoughMoney(((BusinessType)tempCell).PriceForImprovement)) // якшо достатньо грошей купляєм
                                {
                                    Console.WriteLine($"{Name} build branch for {tempCell.Name}.");
                                    Balance -= ((BusinessType)tempCell).PriceForImprovement;
                                    ((BusinessType)tempCell).LevelOfBusiness++;
                                }
                            }
                            i++;
                        }
                    }
                } // buy branch

                lowPlegdeCount = LowPledgeCount(lowPlegdeCount);// спсиок бізнесів які пора викупляти для якогось певного бота
                if (lowPlegdeCount.Count >= 1 && !botNeedMoney && !blockToBuildBranchOrRebuy && !DoesBotExistInPrison(game)) // does bot need to rebuy smth
                {
                    do
                    {
                        tempCell = GetBusinessForRedemption(lowPlegdeCount); // отримуєм бізнес
                        if (tempCell != null)
                        {
                            if (DoesEnoughMoney(((BusinessType)tempCell).RedemptionOfBusiness)) // якщо достатньо грошей викупляєм
                            {
                                this.Balance -= ((BusinessType)tempCell).RedemptionOfBusiness;
                                ((BusinessType)tempCell).IsPledged = false;
                                ((BusinessType)tempCell).StepsDuringPledge = 15;
                                lowPlegdeCount.Remove(tempCell);
                                Console.WriteLine($"{Name} rebought company - {tempCell.Name}.");

                                if (tempCell.GetType() == typeof(Business))
                                {
                                    if (!DoesCountOfShopsEqualsBranch(((BusinessType)tempCell).Type))
                                    {
                                        this.Balance += ((BusinessType)tempCell).PledgeOfBusiness;
                                        ((BusinessType)tempCell).IsPledged = true;
                                        Console.WriteLine($"{Name} pledged company - {tempCell.Name}.");
                                    }
                                }
                            }
                        }
                    } while (tempCell != null);

                } // redemption of business

                if (botNeedMoney)
                {
                    if (!DoesEnoughMoneyToPayIfSellEverything(sumThatBotNeed)) // if bot can't replenish a money : deleting
                    {
                        User tempUser = GetUserWhereBotStopped(game);
                        if (tempUser != null) tempUser.Balance += GetAllSum();

                        ClearShopIndexesAndColors(game.ShopIndexes, game.Colors);
                        string opponentName = GetNameOfOpponentWhereBotStopped(game.Field);
                        if (opponentName != null)
                        {
                            User opponent = GetUserByName(game.Users, opponentName);
                            if (opponent != null) opponent.Balance += this.Balance;

                        }
                        RemoveSignAfterDeleting(game.Field);
                        game.Users.Remove(this);
                        botStep = true;

                        Console.WriteLine($"Results of dices were: {tempFirstRes} and {tempSecondRes}.");
                        Console.WriteLine($"{this.Name} hasn't any possible way to continue game...\n" +
                            $"{this.Name} surrends. Sum of money that he needed = {sumThatBotNeed}.");
                    } // якщо умова справдиться видаляєм бота оскільки йому не хватає грошей якшо продати все

                    else
                    {
                        while (!DoesEnoughMoney(sumThatBotNeed))
                        {
                            if (DoesItPossibleToPledge(game.Field)) // чи можна закласти
                            {
                                PledgeBusiness(game.Field);
                                botNeedMoney = false;
                            }
                            else if (DoesItPossibleToSellBranch()) // чи можна продати філіад
                            {
                                SellBranch();
                                botNeedMoney = false;
                            }
                        }
                    }
                } // ways to get money

                if (!botNeedMoney) // offer
                {
                    Cell almostBranchForBot = null;
                    Cell almostBranchForPLayer = null;
                    List<Cell> wantsToGive = new List<Cell>();
                    List<Cell> wantsToGet = new List<Cell>();

                    (Cell, Cell) returnOfFunction = DoesBotCanOfferMonopolyOnMonopoly(game); // отримуєм 2 бізнеса для обміну монополію на монополію
                    if (returnOfFunction.Item1 != null && returnOfFunction.Item2 != null)
                    {
                        User tempUser = GetUserByName(game.Users, ((BusinessType)returnOfFunction.Item1).OwnerOfBusiness);
                        if (!((BusinessType)returnOfFunction.Item1).IsPledged || (((BusinessType)returnOfFunction.Item1).IsPledged && ((BusinessType)returnOfFunction.Item1).StepsDuringPledge >= 4))
                        {
                            wantsToGive.Add(returnOfFunction.Item2);
                            wantsToGet.Add(returnOfFunction.Item1);

                            if (SendOffer(tempUser, returnOfFunction.Item1, returnOfFunction.Item2))
                            {
                                int botBuisness = game.ShopIndexes.IndexOf(returnOfFunction.Item2);
                                int anotherPlayerBuisness = game.ShopIndexes.IndexOf(returnOfFunction.Item1);
                                ((BusinessType)game.ShopIndexes[anotherPlayerBuisness]).OwnerOfBusiness = this.Name;
                                ((BusinessType)game.ShopIndexes[botBuisness]).OwnerOfBusiness = tempUser.Name;
                                game.Colors[anotherPlayerBuisness] = ConsoleColor;
                                game.Colors[botBuisness] = tempUser.ConsoleColor;
                                this.ListOfUserBusinesses.Remove((BusinessType)returnOfFunction.Item2);
                                tempUser.ListOfUserBusinesses.Remove((BusinessType)returnOfFunction.Item1);
                                this.ListOfUserBusinesses.Add(((BusinessType)returnOfFunction.Item1));
                                tempUser.ListOfUserBusinesses.Add(((BusinessType)returnOfFunction.Item2));

                                Console.Write($"{tempUser.Name} accepted offer.\n");

                                Console.ReadLine();
                                game.Field.ShowField(game.ShopIndexes, game.Colors, game.UsersInPrison);
                                game.ShowUsersUnderField();
                                Console.ReadLine();
                            }
                        }
                    }


                    if (DoesBotCanOfferAlmostBranch(game, ref almostBranchForBot, ref almostBranchForPLayer))
                    {
                        wantsToGive = new List<Cell>();
                        wantsToGet = new List<Cell>();
                        wantsToGive.Add(almostBranchForPLayer);
                        wantsToGet.Add(almostBranchForBot);

                        User tempUser = GetUserByName(((BusinessType)almostBranchForBot).OwnerOfBusiness, game);


                        if (!DoesOfferExistInAlreadySent(new Offer(this, wantsToGive, 0, tempUser, wantsToGet, 0)))
                        {
                            if (((BusinessType)almostBranchForPLayer).Price < ((BusinessType)almostBranchForBot).Price + tempUser.Balance)
                            {
                                if (SendOffer(tempUser, almostBranchForBot, almostBranchForPLayer))
                                {
                                    int botBuisness = game.ShopIndexes.IndexOf(almostBranchForPLayer);
                                    int anotherPlayerBuisness = game.ShopIndexes.IndexOf(almostBranchForBot);

                                    ((BusinessType)game.ShopIndexes[anotherPlayerBuisness]).OwnerOfBusiness = this.Name;
                                    ((BusinessType)game.ShopIndexes[botBuisness]).OwnerOfBusiness = tempUser.Name;
                                    game.Colors[anotherPlayerBuisness] = ConsoleColor;
                                    game.Colors[botBuisness] = tempUser.ConsoleColor;
                                    this.ListOfUserBusinesses.Remove((BusinessType)almostBranchForPLayer);
                                    tempUser.ListOfUserBusinesses.Remove((BusinessType)almostBranchForBot);
                                    this.ListOfUserBusinesses.Add(((BusinessType)almostBranchForBot));
                                    tempUser.ListOfUserBusinesses.Add(((BusinessType)almostBranchForPLayer));

                                    Console.Write($"{tempUser.Name} accepted offer.");
                                    Console.ReadLine();
                                    game.Field.ShowField(game.ShopIndexes, game.Colors, game.UsersInPrison);
                                    game.ShowUsersUnderField();
                                    Console.ReadLine();
                                }
                            }
                            else this.SentOffers.Add(new Offer(this, wantsToGive, 0, tempUser, wantsToGet, tempUser.Balance));
                        }
                    } // чи може бот запропонувати майже монополію
                } // offer

                if (!botNeedMoney && !botStep) // якшо боту не потрібні гроші
                {
                    if (DoStep(game, ref tempFirstRes, ref tempSecondRes, ref resOfDices, ref currentElementOfBot, ref blockOnChance, ref tempChance, ref sumThatBotNeed)) // робить хід
                    {
                        botStep = true;
                        botNeedMoney = false;
                        LevelOfCarsAndGames();
                        if (tempFirstRes == tempSecondRes) // якщо однаокові результати кубиків
                        {
                            if (!DoesBotExistInPrison(game) && !DoesBotNeedToSkipStep(game)) // якщо бот не мусить пропускати хід і не в тюрмі
                            {
                                Console.WriteLine($"{this.Name} got same results on dices, he will go one more time.");
                                currentElementOfBot--;
                            }
                        }
                    }
                    else
                    {
                        botNeedMoney = true;
                        blockToBuildBranchOrRebuy = true;
                    }
                } // do step
            } while (!botStep);
        }

        private bool DoesBotExistInPrison(Game game)
             => game.UsersInPrison.Count(u => u.Name.Equals(Name)) > 0;
        private bool DoesBotNeedToSkipStep(Game game)
        {
            for (int i = 0; i < game.NeedToSkipStep.Count; i++)
            {
                if (Name == game.NeedToSkipStep[i].Name) return true;
            }
            return false;
        }
        private bool DoTypeExistAsBranch(TypeOfBusiness type)
        {
            for (int i = 0; i < BranchTypes.Count; i++)
            {
                if (BranchTypes[i] == type) return false;
            }
            return true;
        }
        public bool DoesEnoughMoney(int priceForSmth)
        {
            return Balance >= priceForSmth;
        }
        private void ClearShopIndexesAndColors(List<Cell> shopIndexes, List<ConsoleColor> colors)
        {
            for (int i = 0; i < shopIndexes.Count; i++)
            {
                for (int j = 0; j < ListOfUserBusinesses.Count; j++)
                {
                    if (shopIndexes[i].Name == ListOfUserBusinesses[j].Name)
                    {
                        ((BusinessType)shopIndexes[i]).LevelOfBusiness = 0;
                        ((BusinessType)shopIndexes[i]).OwnerOfBusiness = "";
                        ((BusinessType)shopIndexes[i]).IsPledged = false;
                        ((BusinessType)shopIndexes[i]).StepsDuringPledge = 15;
                        shopIndexes.RemoveAt(i);
                        colors.RemoveAt(i);
                        i--;
                        break;
                    }
                }
            }
        }
        private bool MaxLevelOfBusinesses(TypeOfBusiness type)
        {
            bool temp = false;
            for (int i = 0; i < ListOfUserBusinesses.Count; i++)
            {
                if (ListOfUserBusinesses[i].Type == type &&
                    ListOfUserBusinesses[i].LevelOfBusiness < _maxLevel) temp = true;
            }
            return temp;
        }

        // buy branch
        private int GetCountOfBranchesOnMap(Field field)
        {
            bool areAllBusinessesUnpledged = true;
            TypeOfBusiness[] type = 
            { 
                TypeOfBusiness.Restaurant, TypeOfBusiness.Perfumery,
                TypeOfBusiness.Drink, TypeOfBusiness.Clothing,
                TypeOfBusiness.Airlines, TypeOfBusiness.Hotel,
                TypeOfBusiness.WebService, TypeOfBusiness.Electronics
            };

            int countOfBranchTypes = 0;

            for (int i = 0; i < type.Length; i++)
            {
                if (CountOfOneTypeCompany(this, type[i]) == field.CountOfBusinessesOnMap(type[i]) &&
                    MaxLevelOfBusinesses(type[i]))
                {
                    for (int j = 0; j < ListOfUserBusinesses.Count; j++)
                    {
                        if (ListOfUserBusinesses[i].IsPledged && ListOfUserBusinesses[i].Type == type[i])
                        {
                            areAllBusinessesUnpledged = false;
                            break;
                        }
                    }

                    if (areAllBusinessesUnpledged) countOfBranchTypes++;
                }
            }
            return countOfBranchTypes;
        }
        private bool DoesItPossibleToBuyBranch(Field field)
        {
            bool areAllBusinessesUnpledged = true;
            TypeOfBusiness[] type =
            {
                TypeOfBusiness.Restaurant, TypeOfBusiness.Perfumery,
                TypeOfBusiness.Drink, TypeOfBusiness.Clothing,
                TypeOfBusiness.Airlines, TypeOfBusiness.Hotel,
                TypeOfBusiness.WebService, TypeOfBusiness.Electronics
            };

            for (int i = 0; i < type.Length; i++)
            {
                if (CountOfOneTypeCompany(this, type[i]) == field.CountOfBusinessesOnMap(type[i]) &&
                    MaxLevelOfBusinesses(type[i]))
                {
                    for (int j = 0; j < ListOfUserBusinesses.Count; j++)
                    {
                        if (ListOfUserBusinesses[i].IsPledged && ListOfUserBusinesses[i].Type == type[i])
                        {
                            areAllBusinessesUnpledged = false;
                            break;
                        }
                    }

                    if (areAllBusinessesUnpledged) return true;

                }
            }
            return false;
        }
        private TypeOfBusiness GetType(Field field)
        {
            bool areAllBusinessesUnpledged = true;
            TypeOfBusiness[] type = { TypeOfBusiness.Restaurant, TypeOfBusiness.Perfumery, TypeOfBusiness.Drink, TypeOfBusiness.Clothing, TypeOfBusiness.Airlines, TypeOfBusiness.Hotel, TypeOfBusiness.WebService, TypeOfBusiness.Electronics };
            for (int i = 0; i < type.Length; i++)
            {

                if (!DoesUserBuiltBranchForThatType(type[i]))
                {
                    if (CountOfOneTypeCompany(this, type[i]) == field.CountOfBusinessesOnMap(type[i]) &&
                        MaxLevelOfBusinesses(type[i]))
                    {
                        for (int j = 0; j < ListOfUserBusinesses.Count; j++)
                        {
                            if (ListOfUserBusinesses[i].IsPledged && ListOfUserBusinesses[i].Type == type[i])
                            {
                                areAllBusinessesUnpledged = false;
                                break;
                            }
                        }

                        if (areAllBusinessesUnpledged) return type[i];

                    }
                }
            }
            return 0;
        }
        private bool CanBotImproveCompany(Cell business)
        {
            TypeOfBusiness type = ((BusinessType)business).Type;
            int minLevel = int.MaxValue;
            int maxLevel = int.MinValue;
            for (int i = 0; i < ListOfUserBusinesses.Count; i++)
            {
                if (ListOfUserBusinesses[i].Type == type &&
                    ListOfUserBusinesses[i].LevelOfBusiness > maxLevel) 
                    maxLevel = ListOfUserBusinesses[i].LevelOfBusiness;

                if (ListOfUserBusinesses[i].Type == type &&
                    ListOfUserBusinesses[i].LevelOfBusiness < minLevel) 
                    minLevel = ListOfUserBusinesses[i].LevelOfBusiness;
            }

            return (minLevel == maxLevel || ((Business)business).LevelOfBusiness == minLevel);
            
        }
        private Cell GetBranchToBuy(TypeOfBusiness type)
        {
            for (int i = 0; i < ListOfUserBusinesses.Count; i++)
            {
                if (ListOfUserBusinesses[i].Type == type)
                {
                    if (CanBotImproveCompany(ListOfUserBusinesses[i]) &&
                        (ListOfUserBusinesses[i]).LevelOfBusiness < _maxLevel) 
                        return ListOfUserBusinesses[i];
                }
            }
            return null;
        }
        private bool DoesUserBuiltBranchForThatType(TypeOfBusiness type)
        {
            if (WereUsedInStep.Count == 0) return false;
            for (int i = 0; i < WereUsedInStep.Count; i++)
            {
                if (WereUsedInStep[i] == type) return true;
            }
            return false;
        }
        // buy branch

        //Redemption
        private List<Cell> LowPledgeCount(List<Cell> lowPledgeCount)
        {
            for (int i = 0; i <ListOfUserBusinesses.Count; i++)
            {
                if (ListOfUserBusinesses[i].StepsDuringPledge < _stepsForRebuy)
                    lowPledgeCount.Add(ListOfUserBusinesses[i]);
            }
            return lowPledgeCount;
        }
        private Cell GetBusinessForRedemption(List<Cell> lowPledgeCount)
        {
            for (int i = 0; i < lowPledgeCount.Count; i++)
            {
                if (Balance >= ((BusinessType)lowPledgeCount[i]).RedemptionOfBusiness) 
                    return lowPledgeCount[i];
            }
            return null;
        }
        // Redemption

        //Pledge
        private void Pledge(BusinessType business)
        {
            Console.WriteLine($"{Name} pledged - {business.Name}");
            business.IsPledged = true;
            Balance += business.PledgeOfBusiness;
        }
        private bool DoesItPossibleToPledge(Field field)
        {
            for (int i = 0; i < ListOfUserBusinesses.Count; i++)
            {
                if (ListOfUserBusinesses[i].GetType() == typeof(Business))
                {
                    if(CountOfOneTypeCompany(this, ListOfUserBusinesses[i].Type) ==
                       field.CountOfBusinessesOnMap(ListOfUserBusinesses[i].Type) &&
                       CanIPledgeFromBranchTypes(ListOfUserBusinesses[i].Type) &&
                       !ListOfUserBusinesses[i].IsPledged) return true;

                    else if (!ListOfUserBusinesses[i].IsPledged &&
                             CountOfOneTypeCompany(this, ListOfUserBusinesses[i].Type) !=
                             field.CountOfBusinessesOnMap(ListOfUserBusinesses[i].Type)) return true;
                }
                else if (!ListOfUserBusinesses[i].IsPledged) return true;
            }
            return false;
        }
        private void PledgeBusiness(Field field)
        {
            for (int i = 0; i < ListOfUserBusinesses.Count; i++)
            {
                if (ListOfUserBusinesses[i].GetType() == typeof(Business))
                {
                    if (CountOfOneTypeCompany(this, ListOfUserBusinesses[i].Type) ==
                        field.CountOfBusinessesOnMap(ListOfUserBusinesses[i].Type) &&
                        CanIPledgeFromBranchTypes(ListOfUserBusinesses[i].Type) &&
                        !ListOfUserBusinesses[i].IsPledged)
                    {
                        Pledge(ListOfUserBusinesses[i]);
                        break;
                    }

                    else if (!ListOfUserBusinesses[i].IsPledged &&
                             CountOfOneTypeCompany(this, ListOfUserBusinesses[i].Type) !=
                             field.CountOfBusinessesOnMap(ListOfUserBusinesses[i].Type))
                    {
                        Pledge(ListOfUserBusinesses[i]);
                        break;
                    }
                }
                else if (!ListOfUserBusinesses[i].IsPledged)
                {
                    Pledge(ListOfUserBusinesses[i]);
                    break;
                }
            }
        }
        //Pledge

        //Sell Branch
        private bool CanBotSellThisCompany(Cell business)
        {
            TypeOfBusiness type = ((BusinessType)business).Type;
            int minLevel = int.MaxValue;
            int maxLevel = int.MinValue;
            for (int i = 0; i < ListOfUserBusinesses.Count; i++)
            {
                if (ListOfUserBusinesses[i].Type == type &&
                    ListOfUserBusinesses[i].LevelOfBusiness > maxLevel) 
                    maxLevel = ListOfUserBusinesses[i].LevelOfBusiness;

                if (ListOfUserBusinesses[i].Type == type &&
                    ListOfUserBusinesses[i].LevelOfBusiness < minLevel) 
                    minLevel = ListOfUserBusinesses[i].LevelOfBusiness;
            }
            return (minLevel == maxLevel || ((Business)business).LevelOfBusiness == maxLevel);
        }
        private bool DoesItPossibleToSellBranch()
        {
            for (int i = 0; i < ListOfUserBusinesses.Count; i++)
            {
                if (ListOfUserBusinesses[i].LevelOfBusiness > 0 &&
                    ListOfUserBusinesses[i].GetType() == typeof(Business) &&
                    CanBotSellThisCompany(ListOfUserBusinesses[i])) return true;
            }

            return false;
        }
        private void SellBranch()
        {
            for (int i = 0; i < ListOfUserBusinesses.Count; i++)
            {
                if (ListOfUserBusinesses[i].LevelOfBusiness > 0 &&
                    ListOfUserBusinesses[i].GetType() == typeof(Business) &&
                    CanBotSellThisCompany(ListOfUserBusinesses[i]))
                {
                    Console.WriteLine($"{Name} is selling a branch of {ListOfUserBusinesses[i].Name}");
                    ListOfUserBusinesses[i].LevelOfBusiness--;
                    Balance += ListOfUserBusinesses[i].PriceForImprovement;
                    break;
                }
            }
        }
        //Sell Branch

        //Offer smth
        private Cell GetCompany(TypeOfBusiness type, User tempUser)
        {
            for (int i = 0; i < tempUser.ListOfUserBusinesses.Count; i++)
            {
                if (tempUser.ListOfUserBusinesses[i].Type == type) return tempUser.ListOfUserBusinesses[i];
            }
            return null;
        }
        private bool DoesThirdBusinessIsEmpty(Game game, User player, TypeOfBusiness type)
        {
            if (type != TypeOfBusiness.Cars &&
                type != TypeOfBusiness.Electronics &&
                type != TypeOfBusiness.Perfumery &&
                type != TypeOfBusiness.GameDevelopment)
            {
                for (int i = 0; i < game.Users.Count; i++)
                {
                    if (game.Users[i].Name != Name &&
                        game.Users[i].Name != player.Name &&
                        CountOfOneTypeCompany(game.Users[i], type) != 0) return false;
                }
            }
            
            return true;
        }
        private User DoesSmbElseHasThatType(List<User> users, TypeOfBusiness type)
        {
            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].Name != Name &&
                    (CountOfOneTypeCompany(users[i], type) == 1)) 
                    return users[i];
            }
            return null;
        }
        private User GetUserByName(string name, Game game) 
            => game.Users.FirstOrDefault(x => x.Name.Equals(name));
        private BusinessType GetBusinessForBot(Game game,TypeOfBusiness type, User tempUser)
        {
            for (int j = 0; j < tempUser.ListOfUserBusinesses.Count; j++)
            {
                if (tempUser.ListOfUserBusinesses[j].Type != type &&
                    tempUser.ListOfUserBusinesses[j].GetType() == typeof(Business))
                {
                    if ((tempUser.ListOfUserBusinesses[j].Type != TypeOfBusiness.Perfumery &&
                         tempUser.ListOfUserBusinesses[j].Type != TypeOfBusiness.Electronics) &&
                        CountOfOneTypeCompany(tempUser, tempUser.ListOfUserBusinesses[j].Type) == 1 &&
                        CountOfOneTypeCompany(this, tempUser.ListOfUserBusinesses[j].Type) == 1 &&
                        DoesThirdBusinessIsEmpty(game, tempUser, tempUser.ListOfUserBusinesses[j].Type)) 
                        return tempUser.ListOfUserBusinesses[j];
                }
            }
            return null;
        }
        private bool DoesBotCanOfferAlmostBranch(Game game, ref Cell businessForBot, ref Cell businessForPLayer)
        {
            for (int i = 0; i < ListOfUserBusinesses.Count; i++) // Пошук бізнеса який може запропонувати бот
            {
                if (ListOfUserBusinesses[i].GetType() == typeof(Business))
                {
                    TypeOfBusiness temp = ListOfUserBusinesses[i].Type;

                    if (CountOfOneTypeCompany(this, temp) == 1 &&
                        (temp != TypeOfBusiness.Perfumery &&
                         temp != TypeOfBusiness.Electronics &&
                         temp != TypeOfBusiness.GameDevelopment &&
                         temp != TypeOfBusiness.Cars))
                    {
                        User tempUser = DoesSmbElseHasThatType(game.Users, temp);
                        if (tempUser != null && DoesThirdBusinessIsEmpty(game,tempUser, temp))
                        {
                            businessForPLayer = ListOfUserBusinesses[i];
                            businessForBot = GetBusinessForBot(game, temp, tempUser);
                            if (businessForBot != null && businessForPLayer != null)
                            {
                                if ((((BusinessType)businessForBot).IsPledged && !((BusinessType)businessForPLayer).IsPledged) ||
                                    (((BusinessType)businessForBot).StepsDuringPledge < ((BusinessType)businessForPLayer).StepsDuringPledge)) return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
        private bool CompareToCompanies(int sumToGet, int sumToOffer)
        {
            if (sumToGet > sumToOffer)
            {
                if ((double)sumToGet / sumToOffer <= _x2) return true;
            }
            else if (sumToOffer >= sumToGet)
            {
                if ((double)sumToOffer / sumToGet <= _x2) return true;
            }
            return false;
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
        private int CountOfOneTypeUnpledgedCompany(TypeOfBusiness type)
        {
            int count = 0;
            for (int i = 0; i < ListOfUserBusinesses.Count; i++)
            {
                if (ListOfUserBusinesses[i].Type == type &&
                    (!ListOfUserBusinesses[i].IsPledged || 
                     (ListOfUserBusinesses[i].IsPledged &&
                      ListOfUserBusinesses[i].StepsDuringPledge >= _checkingForOffer))) count++;
            }
            return count;
        }
        private (Cell, Cell) DoesBotCanOfferMonopolyOnMonopoly(Game game)
        {
            Cell buisnessForBot = null;
            Cell buisnessForPlayer = null;

            TypeOfBusiness[] type =
            {
                TypeOfBusiness.Restaurant, TypeOfBusiness.Perfumery,
                TypeOfBusiness.Airlines, TypeOfBusiness.Drink,
                TypeOfBusiness.Clothing, TypeOfBusiness.Hotel,
                TypeOfBusiness.WebService, TypeOfBusiness.Electronics
            };

            for (int i = 0; i < type.Length; i++)
            {
                
                if (CountOfOneTypeUnpledgedCompany(type[i]) == game.Field.CountOfBusinessesOnMap(type[i]) - 1)
                {
                    User tempUser = DoesSomebodyHasShopWithThatType(game.Users, type[i]);
                    if (tempUser != null)
                    {
                        TypeOfBusiness typeForBot = type[i];
                        buisnessForBot = GetCompany(typeForBot, tempUser);

                        TypeOfBusiness typeForPlayer = DoesBotCanOfferMonopolyForThatUser(type[i], tempUser, game.Field);

                        if (typeForPlayer != 0)
                        {
                            buisnessForPlayer = GetCompany(typeForPlayer, this);

                            return (buisnessForBot, buisnessForPlayer);
                        }
                    }
                }
            }
            return (buisnessForBot, buisnessForPlayer);
        }
        private User DoesSomebodyHasShopWithThatType(List<User> users, TypeOfBusiness type)
        {
            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].Name != Name)
                {
                    for (int j = 0; j < users[i].ListOfUserBusinesses.Count; j++)
                    {
                        if (users[i].ListOfUserBusinesses[j].Type == type &&
                            (!users[i].ListOfUserBusinesses[j].IsPledged || 
                             (users[i].ListOfUserBusinesses[j].IsPledged &&
                              users[i].ListOfUserBusinesses[j].StepsDuringPledge >= _checkingForOffer))) 
                            return users[i];
                    }
                }
            }
            return null;
        }
        private TypeOfBusiness DoesBotCanOfferMonopolyForThatUser(TypeOfBusiness botType, User tempUser, Field field)
        {
            TypeOfBusiness[] type =
            {
                TypeOfBusiness.Restaurant, TypeOfBusiness.Perfumery,
                TypeOfBusiness.Airlines, TypeOfBusiness.Drink,
                TypeOfBusiness.Clothing, TypeOfBusiness.Hotel,
                TypeOfBusiness.WebService, TypeOfBusiness.Electronics
            };

            for (int i = 0; i < type.Length; i++)
            {
                if (CountOfOneTypeCompany(tempUser, type[i]) == field.CountOfBusinessesOnMap(type[i]) - 1)
                {
                    if (type[i] != botType &&
                        DoesBotHasShopWithThatType(type[i])) 
                        return type[i];
                }
            }
            return 0;
        }
        private bool DoesBotHasShopWithThatType(TypeOfBusiness typeForUser)
        {
            return CountOfOneTypeCompany(this, typeForUser) == 1;
        }
        private bool DoesOfferExistInAlreadySent(Offer offer)
        {
            for (int i = 0; i < SentOffers.Count; i++)
            {
                if (SentOffers[i].SumOfMoneyThatWantsToGet == offer.SumOfMoneyThatWantsToGet && SentOffers[i].SumOfMoneyThatWantsToGive == offer.SumOfMoneyThatWantsToGive)
                {
                    bool doSameWantsToGive = true;
                    bool doSameWantsToGet = true;

                    bool wasFoundSameCompany = false;
                    for (int j = 0; j < SentOffers[i].WantsToGive.Count; j++)
                    {
                        for (int k = 0; k < offer.WantsToGive.Count; k++)
                        {
                            if (SentOffers[i].WantsToGive[j].Name == offer.WantsToGive[k].Name) wasFoundSameCompany = true;
                        }
                        if (!wasFoundSameCompany) doSameWantsToGive = false;
                    }

                    if (doSameWantsToGive)
                    {
                        wasFoundSameCompany = false;
                        for (int j = 0; j < SentOffers[i].WantsToGet.Count; j++)
                        {
                            for (int k = 0; k < offer.WantsToGet.Count; k++)
                            {
                                if (SentOffers[i].WantsToGet[j].Name == offer.WantsToGet[k].Name) wasFoundSameCompany = true;
                            }
                            if (!wasFoundSameCompany) doSameWantsToGet = false;
                        }
                    }
                    if (doSameWantsToGet && doSameWantsToGive &&
                        SentOffers[i].SumOfMoneyThatWantsToGet == offer.SumOfMoneyThatWantsToGet &&
                        SentOffers[i].SumOfMoneyThatWantsToGive == offer.SumOfMoneyThatWantsToGive) return true;

                }
            }
            return false;
        }
        private bool SendOffer(User tempUser, Cell business, Cell botBusiness)
        {
            List<Cell> wantsToGive = new List<Cell>();
            wantsToGive.Add(botBusiness);
            List<Cell> wantsToGet = new List<Cell>();
            wantsToGet.Add(business);
            int sumToGive = 0;
            int sumToGet = 0;

            if (((BusinessType)botBusiness).Price > ((BusinessType)business).Price)
            {
                if (tempUser.Balance >= (((BusinessType)botBusiness).Price - ((BusinessType)business).Price)) sumToGet = ((BusinessType)botBusiness).Price - ((BusinessType)business).Price;
                else sumToGet = tempUser.Balance;
            }
            else if (((BusinessType)botBusiness).Price < ((BusinessType)business).Price)
            {
                if (Balance >= (((BusinessType)business).Price - ((BusinessType)botBusiness).Price)) sumToGive = ((BusinessType)business).Price - ((BusinessType)botBusiness).Price;
                else sumToGive = Balance;
            }


            if (CompareToCompanies(((BusinessType)business).Price + sumToGet, ((BusinessType)botBusiness).Price + sumToGive))
            {
                if (!DoesOfferExistInAlreadySent(new Offer(this, wantsToGive, sumToGive, tempUser, wantsToGet, sumToGet)))
                {

                    Console.WriteLine($"Player {this.Name} gives: company - {botBusiness.Name}. And sum {sumToGive}.\n" +
                                      $"Player {tempUser.Name} gives: company - {business.Name}. And sum {sumToGet}.\n");

                    if (tempUser.GetType() == typeof(Bot))
                    {
                        if ((((BusinessType)botBusiness).Price + sumToGive) <= (((BusinessType)business).Price + sumToGet))
                        {
                            tempUser.Balance += sumToGive;
                            tempUser.Balance -= sumToGet;
                            Balance += sumToGet;
                            Balance -= sumToGive;
                            return true;
                        }
                        else Console.WriteLine($"{tempUser.Name} declined an offer.");
                    }
                    else
                    {
                        Console.Write("Enter your choice(y/n): ");
                        Char.TryParse(Console.ReadLine().ToLower(), out char tempChar);

                        while (tempChar != 'y' && tempChar != 'n')
                        {
                            Console.Write("Enter your choice(y/n): ");
                            tempChar = Char.Parse(Console.ReadLine().ToLower());
                        }

                        if (tempChar == 'y')
                        {
                            tempUser.Balance += sumToGive;
                            tempUser.Balance -= sumToGet;
                            Balance += sumToGet;
                            Balance -= sumToGive;
                            return true;
                        }
                        else Console.WriteLine($"{tempUser.Name} declined an offer.");
                    }

                    SentOffers.Add(new Offer(this, wantsToGive, sumToGive, tempUser, wantsToGet, sumToGet));
                }
            }
            return false;
        }
        public bool DoesBotNeedToAcceptOffer(List<Cell> wantsToGive, User bot, ref int countOfMonopolies, Field field)
        {
            TypeOfBusiness[] type =
            {
                TypeOfBusiness.Restaurant, TypeOfBusiness.Perfumery,
                TypeOfBusiness.Airlines, TypeOfBusiness.Drink,
                TypeOfBusiness.Clothing, TypeOfBusiness.Hotel,
                TypeOfBusiness.WebService, TypeOfBusiness.Electronics,
                TypeOfBusiness.GameDevelopment, TypeOfBusiness.Cars
            };

            int count = 0;

            for (int i = 0; i < type.Length; i++)
            {
                for (int j = 0; j < wantsToGive.Count; j++)
                {
                    if (((BusinessType)wantsToGive[j]).Type == type[i] &&
                        !((BusinessType)wantsToGive[j]).IsPledged ||
                        (((BusinessType)wantsToGive[j]).IsPledged &&
                         ((BusinessType)wantsToGive[j]).StepsDuringPledge >= _stepsForRebuy)) count++;
                }
                if (count + CountOfOneTypeCompany(bot, type[i]) == field.CountOfBusinessesOnMap(type[i])) countOfMonopolies++;
                count = 0;
            }

            if (countOfMonopolies > 0) return true;
            return false;
        }
        public bool WillAnotherUserGetMoreMonopolies(List<Cell> wantsToGet, User owner, ref int countOfMonopolies, Field field)
        {
            TypeOfBusiness[] type = { TypeOfBusiness.Restaurant, TypeOfBusiness.Perfumery, TypeOfBusiness.Airlines, TypeOfBusiness.Drink, TypeOfBusiness.Clothing, TypeOfBusiness.Hotel, TypeOfBusiness.WebService, TypeOfBusiness.Electronics, TypeOfBusiness.GameDevelopment, TypeOfBusiness.Cars };
            int count = 0;
            int monopoliesOfAnotherPlayer = 0;
            for (int i = 0; i < type.Length; i++)
            {
                for (int j = 0; j < wantsToGet.Count; j++)
                {
                    if (((BusinessType)wantsToGet[j]).Type == type[i]) count++;
                }
                if (count + CountOfOneTypeCompany(owner, type[i]) == field.CountOfBusinessesOnMap(type[i])) monopoliesOfAnotherPlayer++;
                count = 0;
            }

            if (countOfMonopolies < monopoliesOfAnotherPlayer) return true;
            return false;
        }
        public bool DoesBotNeedLessImportantOffer(List<Cell> wantsToGive, User bot, ref int countOfThatTypes, Field field)
        {
            TypeOfBusiness[] type = 
                { TypeOfBusiness.Restaurant, TypeOfBusiness.Perfumery,
                    TypeOfBusiness.Airlines, TypeOfBusiness.Drink,
                    TypeOfBusiness.Clothing, TypeOfBusiness.Hotel,
                    TypeOfBusiness.WebService, TypeOfBusiness.Electronics };
            int count = 0;
            bool wasInside = false;

            for (int i = 0; i < type.Length; i++)
            {
                for (int j = 0; j < wantsToGive.Count; j++)
                {
                    if (((BusinessType)wantsToGive[j]).Type == type[i] && !((BusinessType)wantsToGive[j]).IsPledged || (((BusinessType)wantsToGive[j]).IsPledged && ((BusinessType)wantsToGive[j]).StepsDuringPledge >= 7)) count++;
                }
                if (count + CountOfOneTypeCompany(bot, type[i]) == field.CountOfBusinessesOnMap(type[i]))
                {
                    wasInside = true;
                    countOfThatTypes++;
                }
                
                count = 0;
            }
            return wasInside;
        }
        public bool WillUserGetMonopolyFromOffer(List<Cell> wantsToGet, User owner, Field field)
        {
            TypeOfBusiness[] type = { TypeOfBusiness.Restaurant, TypeOfBusiness.Perfumery, TypeOfBusiness.Airlines, TypeOfBusiness.Drink, TypeOfBusiness.Clothing, TypeOfBusiness.Hotel, TypeOfBusiness.WebService, TypeOfBusiness.Electronics };
            int count = 0;

            for (int i = 0; i < type.Length; i++)
            {
                for (int j = 0; j < wantsToGet.Count; j++)
                {
                    if (((BusinessType)wantsToGet[j]).Type == type[i]) count++;
                }
                if (count + CountOfOneTypeCompany(owner, type[i]) == field.CountOfBusinessesOnMap(type[i])) return true;
                
                count = 0;
            }
            return false;
        }
        public bool WillUserGetMoreAlmostMonopoly(List<Cell> wantsToGet, User owner, ref int countOfThatTypes, Field field)
        {
            TypeOfBusiness[] type = { TypeOfBusiness.Restaurant, TypeOfBusiness.Perfumery, TypeOfBusiness.Airlines, TypeOfBusiness.Drink, TypeOfBusiness.Clothing, TypeOfBusiness.Hotel, TypeOfBusiness.WebService, TypeOfBusiness.Electronics };
            int count = 0;
            int countOfAnotherPlayerTypes = 0;

            for (int i = 0; i < type.Length; i++)
            {
                for (int j = 0; j < wantsToGet.Count; j++)
                {
                    if (((BusinessType)wantsToGet[j]).Type == type[i]) count++;
                }

                if (count + CountOfOneTypeCompany(owner, type[i]) == field.CountOfBusinessesOnMap(type[i]) - 1) countOfAnotherPlayerTypes++;

                count = 0;
            }
            if (countOfThatTypes < countOfAnotherPlayerTypes) return true;

            return false;
        }
        public bool DoesItEqualTrade(List<Cell> wantsToGive, List<Cell> wantsToGet, User bot, User owner)
        {
            TypeOfBusiness tempType = 0;

            TypeOfBusiness[] type =
            {
                TypeOfBusiness.Restaurant, TypeOfBusiness.Perfumery,
                TypeOfBusiness.Airlines, TypeOfBusiness.Drink,
                TypeOfBusiness.Clothing, TypeOfBusiness.Hotel,
                TypeOfBusiness.WebService, TypeOfBusiness.Electronics
            };

            for (int i = 0; i < type.Length; i++)
            {
                if (CountOfOneTypeCompany(bot, type[i]) == 0) tempType = type[i];
                for (int j = 0; j < wantsToGive.Count; j++)
                {
                    if (((BusinessType)wantsToGive[j]).Type == tempType &&
                        !((BusinessType)wantsToGive[j]).IsPledged ||
                        (((BusinessType)wantsToGive[j]).IsPledged &&
                        ((BusinessType)wantsToGive[j]).StepsDuringPledge >= 4))
                    {
                        if (EqualTrade(wantsToGet, owner)) return true;
                    }
                }
            }
            return false;
        }
        private bool EqualTrade(List<Cell> wantsToGet, User owner)
        {
            for (int i = 0; i < wantsToGet.Count; i++)
            {
                if (CountOfOneTypeCompany(owner, ((BusinessType)wantsToGet[i]).Type) != 0) return false;
            }
            return true;
        }
        //Offer smth

        //Do step
        private bool DoesUserWonInCasino(List<int> resultsOfDice)
        {
            Random random = new Random();
            int resInCasino = random.Next(1, 7);
            Console.WriteLine($"Won cube was {resInCasino}");
            for (int i = 0; i < resultsOfDice.Count; i++)
            {
                if (resInCasino == resultsOfDice[i]) return true;
            }
            return false;
        }
        private bool DoesThatNumberAlreadyExist(List<int> resultsOfDice, int number)
        {
            for (int i = 0; i < resultsOfDice.Count; i++)
            {
                if (resultsOfDice[i] == number) return true;
            }
            return false;
        }
        public bool IsThatCompanyImportant(Cell company, User bot, Field field)
        {
            if (((BusinessType)company).Type != TypeOfBusiness.Cars)
            {
                int tempCount = 0;

                if (CountOfOneTypeCompany(bot, ((BusinessType)company).Type) ==
                    field.CountOfBusinessesOnMap(((BusinessType)company).Type) - 1) return true;
            }
            return false;
        }
        public bool DoStep(Game game, ref int tempFirst, ref int tempSecond, ref bool resOfDices,
            ref int currentElementOfBot, ref bool blockOnChance, ref ChanceRealization tempChance, ref int sumThatBotNeed)
        {
            ChanceRealization chanceRealization = new ChanceRealization();
            User userOnAuction;
            Cell tempCell;
            int firstRes, secondRes;

            if (!game.Field.DoSignInJail(game.UsersInPrison, Sign))
            {
                if (!resOfDices)
                {
                    firstRes = random.Next(1, 7);
                    Console.WriteLine($"{Name} got a - {firstRes}");
                    secondRes = random.Next(1, 7);
                    Console.WriteLine($"{Name} got a - {secondRes}");
                    if (firstRes == secondRes) this.CountOfDoubles++;
                    else this.CountOfDoubles = 0;
                    tempFirst = firstRes;
                    tempSecond = secondRes;
                    resOfDices = true;
                    if (this.CountOfDoubles != 3) game.Field.MoveSign(this, firstRes + secondRes);
                }

                if (this.CountOfDoubles == 3)
                {
                    Console.WriteLine($"{this.Name} got 3 doubles in raw and go in jail... Reason: cheating!");

                    RemoveSignAfterDeleting(game.Field);
                    game.Field.AddSignInPrison(this.Sign);
                    game.UsersInPrison.Add(this);
                    this.CountOfDoubles = 0;
                }

                else
                {
                    tempCell = game.Field.WhereSignStopped(Sign);

                    if (tempCell is BusinessType)
                    {
                        if (((BusinessType)tempCell).OwnerOfBusiness == "") // Немає власника
                        {

                            if (DoesEnoughMoney(((BusinessType)tempCell).Price))
                            {
                                Balance -= ((BusinessType)tempCell).Price;
                                ((BusinessType)tempCell).OwnerOfBusiness = this.Name;
                                ListOfUserBusinesses.Add((BusinessType)tempCell);
                                game.ShopIndexes.Add(tempCell);
                                game.Colors.Add(ConsoleColor);
                                Console.WriteLine($"{Name} is buying {tempCell.Name}.");
                            }

                            //else if (IsThatCompanyImportant(tempCell, this))
                            //{
                            //    sumThatBotNeed = ((BusinessType)tempCell).Price;
                            //    return false;
                            //}

                            else
                            {
                                Console.WriteLine("-----AUCTION-----");
                                userOnAuction = game.Field.Auction(((BusinessType)tempCell).Price, this, game.Users, tempCell);
                                if (userOnAuction != null)
                                {
                                    Console.WriteLine($"{userOnAuction.Name} bought on auction {tempCell.Name}");
                                    ((BusinessType)tempCell).OwnerOfBusiness = userOnAuction.Name;
                                    game.ShopIndexes.Add(tempCell);
                                    game.Colors.Add(userOnAuction.ConsoleColor);
                                    userOnAuction.ListOfUserBusinesses.Add((BusinessType)tempCell);
                                }
                                else Console.WriteLine("All users declined to take part in auction.");
                            }
                        }
                        else if (((BusinessType)tempCell).OwnerOfBusiness != "" &&
                                Name != ((BusinessType)tempCell).OwnerOfBusiness)   // Якщо вже є власник 
                        {
                            if (((BusinessType)tempCell).IsPledged == false) //поле не заложено
                            {
                                if (tempCell.GetType() == typeof(GameBusiness))
                                {
                                    if (DoesEnoughMoney(((BusinessType)tempCell).Rent[((BusinessType)tempCell).LevelOfBusiness] * (tempFirst + tempSecond)))
                                    {
                                        Console.WriteLine($"{Name} pays for rent");
                                        Balance -= ((BusinessType)tempCell).Rent[((BusinessType)tempCell).LevelOfBusiness] * (tempFirst + tempSecond);
                                        game.Field.PayForRent(((BusinessType)tempCell).OwnerOfBusiness, ((BusinessType)tempCell).Rent[((BusinessType)tempCell).LevelOfBusiness] * (tempFirst + tempSecond), game.Users);
                                    }
                                    else
                                    {
                                        sumThatBotNeed = ((BusinessType)tempCell).Rent[((BusinessType)tempCell).LevelOfBusiness] * (tempFirst + tempSecond);
                                        return false;
                                    }
                                }
                                else
                                {
                                    if (DoesEnoughMoney(((BusinessType)tempCell).Rent[((BusinessType)tempCell).LevelOfBusiness]))
                                    {
                                        Console.WriteLine($"{Name} pays for rent");
                                        Balance -= ((BusinessType)tempCell).Rent[((BusinessType)tempCell).LevelOfBusiness];
                                        game.Field.PayForRent(((BusinessType)tempCell).OwnerOfBusiness, ((BusinessType)tempCell).Rent[((BusinessType)tempCell).LevelOfBusiness], game.Users);
                                    }
                                    else
                                    {
                                        sumThatBotNeed = ((BusinessType)tempCell).Rent[((BusinessType)tempCell).LevelOfBusiness];
                                        return false;
                                    }
                                }
                            }
                            else Console.WriteLine("Cell is pledged"); // Поле заложено
                        }

                        else Console.WriteLine($"{Name} stand on his field.");

                    }

                    else if (tempCell.GetType() == typeof(CellBank))
                    {
                        if (DoesEnoughMoney(2000))
                        {
                            Console.WriteLine($"{Name} paid 2k due to u got bank");
                            Balance -= 2000;
                        }
                        else
                        {
                            sumThatBotNeed = 2000;
                            return false;
                        }
                    }
                    else if (tempCell.GetType() == typeof(CellTax))
                    {
                        if (DoesEnoughMoney(1000))
                        {
                            Console.WriteLine("You paid 1k due to u got tax");
                            Balance -= 1000;
                        }
                        else
                        {
                            sumThatBotNeed = 1000;
                            return false;
                        }


                    }
                    else if (tempCell.GetType() == typeof(CellJail))
                    {
                        Console.WriteLine($"{Name} decided to visit friends in jail :)");
                    }

                    else if (tempCell.GetType() == typeof(CellPoliceStation))
                    {
                        Console.WriteLine($"{Name} goes to jail.");
                        tempCell.UserSign.RemoveAt(tempCell.UserSign.Count - 1);
                        game.Field.AddSignInPrison(Sign);
                        game.UsersInPrison.Add(this);
                    }

                    else if (tempCell.GetType() == typeof(CellChance))
                    {
                        if (!blockOnChance) Console.WriteLine($"{Name} got chance");
                        if (!blockOnChance) chanceRealization = (ChanceRealization)random.Next(1, 13);

                        else chanceRealization = tempChance;

                        switch (chanceRealization)
                        {
                            case ChanceRealization.PayForStudy:
                                {
                                    if (!blockOnChance) Console.WriteLine($"{Name} need to pay for education(1000).");

                                    if (DoesEnoughMoney(1000)) Balance -= 1000;
                                    else
                                    {
                                        if (!blockOnChance)
                                        {
                                            sumThatBotNeed = 1000;
                                        }
                                        blockOnChance = true;
                                        tempChance = chanceRealization;
                                        return false;
                                    }
                                }
                                break;
                            case ChanceRealization.PayForInsurance:
                                {
                                    if (!blockOnChance) Console.WriteLine($"{Name} need to pay for insurance(1250).");

                                    if (DoesEnoughMoney(1250)) Balance -= 1250;

                                    else
                                    {
                                        if (!blockOnChance)
                                        {
                                            sumThatBotNeed = 1250;
                                        }
                                        blockOnChance = true;
                                        tempChance = chanceRealization;
                                        return false;
                                    }
                                }
                                break;
                            case ChanceRealization.PayForCar:
                                {
                                    if (!blockOnChance) Console.WriteLine($"{Name} need to pay for damaged car(2000).");

                                    if (DoesEnoughMoney(2000)) Balance -= 2000;
                                    else
                                    {
                                        if (!blockOnChance)
                                        {
                                            sumThatBotNeed = 2000;
                                        }
                                        blockOnChance = true;
                                        tempChance = chanceRealization;
                                        return false;
                                    }
                                }
                                break;
                            case ChanceRealization.PayForCasino:
                                {
                                    if (!blockOnChance) Console.WriteLine($"{Name} need to pay for casino lost game(1k).");

                                    if (DoesEnoughMoney(1000)) Balance -= 1000;
                                    else
                                    {
                                        if (!blockOnChance)
                                        {
                                            sumThatBotNeed = 1000;
                                        }
                                        blockOnChance = true;
                                        tempChance = chanceRealization;
                                        return false;
                                    }
                                }
                                break;
                            case ChanceRealization.PayInBank:
                                {
                                    if (!blockOnChance) Console.WriteLine($"{Name} need to pay in bank(1500).");

                                    if (DoesEnoughMoney(1500)) Balance -= 1500;
                                    else
                                    {
                                        if (!blockOnChance)
                                        {
                                            sumThatBotNeed = 1500;
                                        }
                                        blockOnChance = true;
                                        tempChance = chanceRealization;
                                        return false;
                                    }
                                }
                                break;
                            case ChanceRealization.EarnFromTax:
                                {
                                    Console.WriteLine($"{Name} gets 500 from tax.");
                                    Balance += 500;
                                }
                                break;
                            case ChanceRealization.EarnFromCasting:
                                {
                                    Console.WriteLine($"{Name} gets 1k from casting due to got second place.");
                                    Balance += 1000;
                                }
                                break;
                            case ChanceRealization.EarnFromBank:
                                {
                                    Console.WriteLine($"{Name} gets from bank 1k.");
                                    Balance += 1000;
                                }
                                break;
                            case ChanceRealization.EarnFromSportCompetition:
                                {
                                    Console.WriteLine($"{Name} gets 500 from sport competition due to got third place.");
                                    Balance += 500;
                                }
                                break;
                            case ChanceRealization.EarnFromBirthday:
                                {
                                    Console.WriteLine($"{Name} gets 1k from his birthday.");
                                    Balance += 1000;
                                }
                                break;
                            case ChanceRealization.MissStep:
                                {
                                    Console.WriteLine($"{Name} lost his dices and will skip step");
                                    game.NeedToSkipStep.Add(this);
                                }
                                break;
                            case ChanceRealization.GotoJail:
                                {
                                    Console.WriteLine($"{Name} goes to jail .");
                                    tempCell.UserSign.RemoveAt(tempCell.UserSign.Count - 1);
                                    game.Field.AddSignInPrison(Sign);
                                    game.UsersInPrison.Add(this);
                                }
                                break;
                        }
                    }

                    else if (tempCell.GetType() == typeof(CellJackpot))
                    {
                        Console.WriteLine($"{Name} get casino.");

                        if (DoesEnoughMoney(1000))
                        {
                            int userValueOfCube = 0;
                            Balance -= 1000;
                            List<int> countOfCubes = new List<int>();
                            int count = random.Next(1, 4);
                            do
                            {
                                userValueOfCube = random.Next(1, 7);
                                if (countOfCubes.Count < count && !DoesThatNumberAlreadyExist(countOfCubes, userValueOfCube)) countOfCubes.Add(userValueOfCube);

                            } while (countOfCubes.Count < count);

                            if (DoesUserWonInCasino(countOfCubes))
                            {
                                if (countOfCubes.Count == 3)
                                {
                                    Balance += 2000;
                                    Console.WriteLine($"\n{Name} won 2k");
                                }

                                else if (countOfCubes.Count == 2)
                                {
                                    Balance += 3000;
                                    Console.WriteLine($"\n{Name} won 3k");
                                }

                                else if (countOfCubes.Count == 1)
                                {
                                    Balance += 6000;
                                    Console.WriteLine($"\n{Name} won 6k");
                                }

                                Console.Write("His dices were: ");
                                for (int j = 0; j < countOfCubes.Count; j++)
                                    Console.Write($"{countOfCubes[j]} ");
                            }
                            else
                            {
                                Console.Write("His dices were: ");
                                for (int j = 0; j < countOfCubes.Count; j++)
                                    Console.Write($"{countOfCubes[j]} ");

                                Console.WriteLine($"\n{Name} lost in casino.");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"{Name} refused to take part in casino.");
                        }
                    }
                    else if (tempCell.GetType() == typeof(CellStart)) Console.WriteLine("Stand on start and get 1k");
                }
            }

            else // in jail
            {
                if (this.TriesToBreakFree < 3)
                {
                    if (DoesEnoughMoney(500))
                    {
                        Console.WriteLine($"{Name} paid 500 and he is free.");
                        Balance -= 500;
                        TriesToBreakFree = 0;
                        game.Field.DeleteSignFromJail(game.UsersInPrison, Sign);
                        currentElementOfBot--;
                    }
                    else
                    {
                        if (this.TriesToBreakFree < 3)
                        {
                            int firstResOfDice = random.Next(1, 7);
                            Console.WriteLine($"{Name} in jail got a - {firstResOfDice}");
                            int secondResOfDice = random.Next(1, 7);
                            Console.WriteLine($"{Name} in jail got a - {secondResOfDice}");
                            if (firstResOfDice == secondResOfDice)
                            {
                                Console.WriteLine($"{Name} got same results on dices, he is free.");
                                this.TriesToBreakFree = 0;
                                game.Field.DeleteSignFromJail(game.UsersInPrison, Sign);
                                currentElementOfBot--;
                            }
                        }
                        this.TriesToBreakFree++;
                    }
                }

                if (this.TriesToBreakFree >= 3)
                {
                    if (DoesEnoughMoney(500))
                    {
                        Console.WriteLine($"{Name} paid 500 and he is free.");
                        Balance -= 500;
                        this.TriesToBreakFree = 0;
                        game.Field.DeleteSignFromJail(game.UsersInPrison, Sign);
                        currentElementOfBot--;
                    }
                    else
                    {
                        sumThatBotNeed = 500;
                        return false;
                    }
                }

            }
            return true;
        }
    }
}