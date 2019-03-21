using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace _8_Puzzle
{
    class Program
    {
        enum Directions{
            Left = 0,
            Up = 1,
            Right = 2,
            Down = 3
        }

        private static string inputFile = "";

        private static bool printSolutionSequence = false;
        // solution sequence (List??)

        private static bool printCost = false;
        private static int cost = 0;

        private static bool printVisitedNodes = false;
        // visited nodes (List??)

        private static int heuristics = 0;

        private static bool useRandomState = false;
        private static int dimensions = 0;
        private static int numberOfMovments = 0;

        static void Main(string[] args)
        {
            //getting command line args
            if (args.Length > 0)
            {
                if (!SuccesfullyGotCommandLineArgs(args))
                {
                    return;
                }
            }

            if (useRandomState)
            {
                //generate random initial state

            }
            else
            {
                //get state from given file

            }

            if(heuristics == 1)
            {
                //A* with number of wrong positions

            }
            if(heuristics == 2)
            {
                //A* with Manhattan distance
            }
            Console.ReadKey();
        }

        private static bool SuccesfullyGotCommandLineArgs(string[] args)
        {
            bool error = false;
            for (int i = 0; i < args.Length; ++i)
            {
                if (error)
                {
                    return false;
                }
                switch (args[i])
                {
                    case "-input":  //-input <File>
                        if (i + 1 < args.Length)
                        {
                            i++;
                            if (args[i].Length > 2)
                            {
                                inputFile = args[i];
                            }
                            else
                            {
                                Console.WriteLine("Wrong file name given!");
                                error = true;
                            }
                        }
                        break;

                    case "-solseq":
                        printSolutionSequence = true;
                        break;

                    case "-pcost":
                        printCost = true;
                        break;

                    case "-nvisited":
                        printVisitedNodes = true;
                        break;

                    case "-h":  // -h <H> = type of the heuristics
                        if (i + 1 < args.Length)
                        {
                            i++;
                            switch (args[i])
                            {
                                case "1":
                                    heuristics = 1;
                                    break;
                                case "2":
                                    heuristics = 2;
                                    break;
                                default:
                                    heuristics = 0;
                                    break;
                            }
                        }
                        break;

                    case "-rand":  //–rand <N> <M>
                        useRandomState = true;
                        if (i + 2 < args.Length)
                        {
                            i++;
                            int number;
                            bool result = Int32.TryParse(args[i], out number);
                            if (result)
                            {
                                dimensions = number;
                            }
                            else
                            {
                                Console.WriteLine("Wrong parameters for generating random initial state!");
                                error = true;
                            }

                            i++;
                            result = Int32.TryParse(args[i], out number);
                            if (result)
                            {
                                numberOfMovments = number;
                            }
                            else
                            {
                                Console.WriteLine("Wrong parameters for generating random initial state!");
                                error = true;
                            }
                        }
                        break;
                    default:
                        Console.WriteLine("Unknown argument: " + args[i]);
                        break;
                }
            }

            //checking arguments to be correct
            if (heuristics == 0)
            {
                Console.WriteLine("Wrong heuristics given! Heuristics should be 1 or 2");
                return false;
            }

            return true;
        }

        private static void printData()
        {

        }

        private static int[,] ReadBoardFromFile(string fileName)
        {
            string[] lines;
            List<string> list = new List<string>();
            int n = 0;

            //reading the size of the game and the tile of game to a string
            using (StreamReader streamReader = new StreamReader(fileName))
            {
                String line;
                int isFirst = 1;
                while ((line = streamReader.ReadLine()) != null)
                {
                    if (isFirst == 1)
                    {
                        n = int.Parse(line);
                        isFirst = 0;
                    }
                    else
                    {
                        list.Add(line);
                    }

                }
            }
            lines = list.ToArray();

            int[,] board = new int[n, n];
            String row;
            String[] split;

            //filling the board with the given values, 0 means the empty field
            for (int i = 0; i < n; i++)
            {
                row = lines[i];
                split = split = row.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                for (int j = 0; j < n; j++)
                {
                    board[i, j] = int.Parse(split[j]);
                }
            }

            return board;
        }

        private static void PrintBoard(int[,] board)
        {
            int n = board.GetLength(0);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (board[i, j] != 0)
                    {
                        Console.Write("{0} ", board[i, j]);
                    }
                    else
                    {   //empty
                        Console.Write("_ ");
                    }
                }
                Console.WriteLine("");
            }
        }

        private static int[,] ReadBoardFromConsole()
        {
            Console.Write("Width of the game: ");
            int n = int.Parse(Console.ReadLine());

            int[,] board = new int[n, n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    //TODO some formating...
                    Console.Write("[{0},{1}]: ", i, j);
                    board[i, j] = int.Parse(Console.ReadLine());
                }
            }

            if (!isOK(board))
            {
                return new int[0, 0];
            }
            else
            {
                return board;
            }
        }

        //checking a filled board if it's legal
        private static bool isOK(int[,] board)
        {
            //check if the board have a legal size
            int n = board.GetLength(0);
            if (n < 1)
            {
                return false;
            }

            //creating a number statistic, first filled with zeros
            int[] numbers = new int[n * n];
            for (int i = 0; i < n; i++) { numbers[i] = 0; }

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    //if the number is legal in the game, ex. int a 3x3 game doesn't exist number 9 or bigger
                    if (board[i, j] >= n * n || board[i, j] < 0)
                    {
                        return false;
                    }
                    else
                    {
                        //count the presence of the numbers
                        numbers[board[i, j]]++;
                    }

                    //in a legal board the tile of 2 doesn't appear more than once
                    if (numbers[board[i, j]] > 1)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool CanMove(int i, int j, int n, int direction)
        {
            switch (direction) //0 - left, 1 - up, 2 - right, 3 - down 
            {
                case (int)Directions.Left:
                    if (j == 0)
                    {
                        return false;
                    }
                    break;
                case (int)Directions.Up:
                    if (i == 0)
                    {
                        return false;
                    }
                    break;
                case (int)Directions.Right:
                    if (j == n - 1)
                    {
                        return false;
                    }
                    break;
                case (int)Directions.Down:
                    if (i == n - 1)
                    {
                        return false;
                    }
                    break;

            }
            return true;
        }

        private static void Step(int[,] board, ref int positionI, ref int positionJ, int direction)
        {
            int N = board.GetLength(0);
            int newTile;
            int originalTile = board[positionI, positionJ];

            if (!CanMove(positionI, positionJ, N, direction))
            {
                Console.WriteLine("Tile can't move that direction!");
                return;
            }

            switch (direction) //0 - left, 1 - up, 2 - right, 3 - down 
            {
                case (int)Directions.Left:
                    positionJ--;
                    newTile = board[positionI, positionJ];

                    board[positionI, positionJ] = originalTile; //the empty tile
                    board[positionI, positionJ + 1] = newTile;

                    break;
                case (int)Directions.Up:
                    positionI--;
                    newTile = board[positionI, positionJ];

                    board[positionI, positionJ] = originalTile;
                    board[positionI + 1, positionJ] = newTile;

                    break;
                case (int)Directions.Right:
                    positionJ++;
                    newTile = board[positionI, positionJ];

                    board[positionI, positionJ] = originalTile;
                    board[positionI, positionJ - 1] = newTile;
                    break;
                case (int)Directions.Down:
                    positionI++;
                    newTile = board[positionI, positionJ];

                    board[positionI, positionJ] = originalTile;
                    board[positionI - 1, positionJ] = newTile;
                    break;

            }
        }

        private static void Step(int[,] board, int direction)
        {
            int posI = -1, posJ = -1, N = board.GetLength(0);

            //searching the empty tile
            for (int i = 0; i < N; i++)
            {
                for(int j = 0; j < N; j++)
                {
                    //if this is the empty tile
                    if(board[i,j] == 0)
                    {
                        posI = i;
                        posJ = j;
                        break;
                    }
                }
            }
            if(posI == -1 || posJ == -1)
            {
                Console.WriteLine("Missing empty tile!");
                return;
            }
            else
            {
                Step(board, ref posI, ref posJ, direction);
            }
        }

        private static int[,] GenerateTable(int N, int M)
        {
            int[,] board = new int[N, N];
            int temp = 0;

            //generating the solved table
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    board[i, j] = temp;
                    temp++;
                }
            }

            //taking M random step with the empty tile
            Random rand = new Random();
            int direction;

            //the starting position of the empty tile after generating
            int positionI = 0;
            int positionJ = 0;

            while (M > 0)
            {
                do
                {
                    direction = rand.Next(4);
                } while (!CanMove(positionI, positionJ, N, direction));

                Step(board, ref positionI, ref positionJ, direction);

                M--;
            }

            return board;
        }


    }
}
