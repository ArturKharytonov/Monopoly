using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monopoly_
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Field field = new Field();
            Game game = new Game(field, new List<User>());
            MainMenu mainMenu = new MainMenu();

            bool isEnd = false;
            while (!isEnd)
            {
                Console.WriteLine("----MENU----\n" +
                "1 - Add Bot.\n" +
                "2 - Add Player.\n" +
                "3 - Start Game.\n" +
                "4 - Show Players.\n" +
                "5 - DeletePlayer.\n" +
                "6 - Exit.\n");
                Console.Write("Enter your choice: ");
                Enum.TryParse(Console.ReadLine(), out mainMenu);
                Console.Clear();
                switch (mainMenu)
                {
                    case MainMenu.AddBot:
                        {
                            if (game.Users.Count < 6)
                            {
                                string name = "";
                                do
                                {
                                    Console.Write("Enter bot name: ");
                                    name = Console.ReadLine();
                                } while (name == "" || name.Length >= 17);
                                
                                if (!game.DoesNameExistInList(name))
                                {
                                    Console.Write("Enter bot sign: ");
                                    if (char.TryParse(Console.ReadLine(), out char botChar) && !game.DoesSignExist(botChar))
                                    {
                                        User bot = new Bot(name, botChar);
                                        game.Users.Add(bot);
                                        Console.WriteLine("Bot was Added");
                                    }
                                    else Console.WriteLine("Incorrect sign or that sign already exist.");
                                }
                                else Console.WriteLine("This name already exist");
                            }
                            else Console.WriteLine("Max count of players are created");
                        }
                        break;
                    case MainMenu.AddPlayer:
                        {
                            if (game.Users.Count < 6)
                            {
                                string name = "";
                                do
                                {
                                    Console.Write("Enter player name: ");
                                    name = Console.ReadLine();
                                } while (name == "" || name.Length >= 17);
                                if (!game.DoesNameExistInList(name))
                                {
                                    Console.Write("Enter player sign: ");
                                    if (char.TryParse(Console.ReadLine(), out char playerChar) && !game.DoesSignExist(playerChar))
                                    {
                                        User player = new Player(name, playerChar);
                                        game.Users.Add(player);
                                        Console.WriteLine("Player was added");
                                    }
                                    else Console.WriteLine("Incorrect sign or that sign already exist.");
                                }
                                else Console.WriteLine("This name already exist");
                            }
                            else Console.WriteLine("Max count of players are created");
                        }
                        break;
                    case MainMenu.StartGame:
                        {
                            if (game.Users.Count >= 2)
                            {
                                game.StartGame();
                                field = new Field();
                                game = new Game(field, new List<User>());
                            }
                            else Console.WriteLine("Not enough players to start.");
                        }
                        break;
                    case MainMenu.ShowPLayers:
                        {
                            if (!game.ShowAllUsers())
                            {
                                Console.WriteLine("No users.");
                            }
                        }
                        break;
                    case MainMenu.DeletePlayer:
                        {
                            Console.Write("Enter char to delete user: ");
                            if (char.TryParse(Console.ReadLine(), out char userChar) && game.DoesSignExist(userChar))
                            {
                                game.DeleteUser(userChar);
                                Console.WriteLine("User was successfuly deleted");
                            }
                            else Console.WriteLine("User was not found");
                        }
                        break;
                    case MainMenu.Exit:
                        Console.WriteLine("See ya");
                        isEnd = true;
                        break;
                    default:
                        Console.WriteLine("Incorrect choice");
                        break;
                }
            }
        }
    }
}
