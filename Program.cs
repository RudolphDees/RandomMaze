using System;
using System.Collections.Generic;
using System.Threading;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data.Common;
using System.Security.Cryptography.X509Certificates;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Maze
{
    class Program
    {
        
        static void Main(string[] args)
        {
            bool complete = false;
            int count = 0;
            Console.WriteLine("Please input the size of the maze you would like. For example, an input of 6 would create a 6 x 6 maze.");
            int mazeSize = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Please input the number of moves that can be used to complete the maze.");
            int moveCount = Convert.ToInt32(Console.ReadLine());
            Player player = new Player();
            int beginX = 2;
            int beginY = 2;
            int endX = mazeSize - 2;
            int endY = mazeSize - 2;
            Square[,] maze = Functions.CreateMaze(mazeSize, beginX, beginY, endX, endY);
            while (complete == false)
            {
                maze = Functions.CreateMaze(mazeSize, beginX, beginY, endX, endY);
                if (Functions.TestMaze(maze, moveCount, mazeSize, player, "no", endX, endY, beginX, beginY) == true && player.moveCount >= moveCount)
                {
                    Functions.ResetMaze(maze, mazeSize, player, beginX, beginY);
                    Functions.PrintMaze(maze, mazeSize);
                    Console.WriteLine("This maze takes " + player.moveCount + " moves!");
                    count++;
                    complete = true;
                }
                else
                count++;
            }
            Console.WriteLine("It took " + count + " tries to find a solvable maze");
            Console.WriteLine("Would you like to see how to solve this maze? Yes or No?");
            string solution = Console.ReadLine().ToLower();
            Console.WriteLine(solution);
            if (solution == "yes")
            {
                Functions.TestMaze(maze, moveCount, mazeSize, player, solution, endX, endY, beginX, beginY);
                Functions.ResetMaze(maze, mazeSize, player, beginX, beginY);
            }

        }
    }
}
public class Square
{
    public bool wall = false, occupied = false, begin = false, end = false, hasBeenUsed = false, deadSquare = false;
    public Square(bool wall)
    {
        this.wall = wall;
    }
}
public class Functions
{
    public static void PrintMaze(Square[,] maze, int mazeSize)
    {
        for (int j = 0; j < mazeSize; j++)
        {
            for (int i = 0; i < mazeSize; i++)
            {
                PrintSquare(maze[i, j]);
            }
            Console.WriteLine();
        }
    }
    public static void PrintSquare(Square square)
    {
        if (square.begin == true)
            Console.Write("B");
        else if (square.end == true)
            Console.Write("E");
        else if (square.occupied == true)
            Console.Write("O");
        else if (square.hasBeenUsed == true)
            Console.Write("U");
        else if (square.deadSquare == true)
            Console.Write("@");
        else if (square.wall == true)
            Console.Write("*");
        else Console.Write("A");
    }
    static public Square[,] CreateMaze(int mazeSize, int beginX, int beginY, int endX, int endY)
    {
        bool wall;
        Random rand = new Random();
        Square[,] maze = new Square[mazeSize, mazeSize];
        for (int j = 0; j < mazeSize; j++)
        {
            for (int i = 0; i < mazeSize; i++)
            {
                wall = false;
                int odds = rand.Next(0, 2);
                if (i == 0 || i == mazeSize - 1 || j == 0 || j == mazeSize - 1)
                    wall = true;
                else if (odds == 0)
                    wall = true;
                maze[i, j] = new Square(wall);
            }
        }
        maze[beginX, beginY].begin = true;
        maze[beginX, beginY].occupied = true;
        maze[endX, endY].end = true;
        return maze;
    }
    static public bool TestMaze(Square[,] maze, int moveCount, int mazeSize, Player player, string print, int endX, int endY, int beginX, int beginY)
    {
        player.xLoc = beginX;
        player.yLoc = beginY;
        return TestMazeRecurrsive(maze, mazeSize, player, endX, endY, print);
    }
    static public bool TestMazeRecurrsive(Square[,] maze, int mazeSize, Player player, int endX, int endY, string print)
    {
        int prevX = player.xLoc;
        int prevY = player.yLoc;
        MoveUpRec(maze, mazeSize, player, endX, endY, print);
        if (player.xLoc == endX && player.yLoc == endY)
            return true;
        else if (player.xLoc != prevX && player.yLoc != prevY)
            TestMazeRecurrsive(maze, mazeSize, player, endX, endY, print);
        MoveLeftRec(maze, mazeSize, player, endX, endY, print);
        if (player.xLoc == endX && player.yLoc == endY)
            return true;
        else if (player.xLoc != prevX && player.yLoc != prevY)
            TestMazeRecurrsive(maze, mazeSize, player, endX, endY, print);
        MoveRightRec(maze, mazeSize, player, endX, endY, print);
        if (player.xLoc == endX && player.yLoc == endY)
            return true;
        else if (player.xLoc != prevX && player.yLoc != prevY)
            TestMazeRecurrsive(maze, mazeSize, player, endX, endY, print);
        MoveDownRec(maze, mazeSize, player, endX, endY, print);
        if (player.xLoc == endX && player.yLoc == endY)
            return true;
        else if (player.xLoc != prevX && player.yLoc != prevY)
            TestMazeRecurrsive(maze, mazeSize, player, endX, endY, print);
        maze[player.xLoc, player.yLoc].deadSquare = true;
        return false;
    }
    static public void MoveDownRec(Square[,] maze, int mazeSize, Player player, int endX, int endY, string print)
    {
        if (maze[player.xLoc, player.yLoc + 1].wall == false && maze[player.xLoc, player.yLoc + 1].hasBeenUsed == false && maze[player.xLoc, player.yLoc + 1].deadSquare == false)
        {
            maze[player.xLoc, player.yLoc].occupied = false;
            maze[player.xLoc, player.yLoc + 1].occupied = true;
            maze[player.xLoc, player.yLoc].hasBeenUsed = true;
            player.yLoc++;
            player.moveCount++;
            if (print == "yes")
            {
                Thread.Sleep(500);
                PrintMaze(maze, mazeSize);
            }
            if (TestMazeRecurrsive(maze, mazeSize, player, endX, endY, print) == false)
            {
                maze[player.xLoc, player.yLoc - 1].occupied = true;
                maze[player.xLoc, player.yLoc].occupied = false;
                maze[player.xLoc, player.yLoc - 1].hasBeenUsed = false;
                player.yLoc--;
                player.moveCount--;
                if (print == "yes")
                {
                    Thread.Sleep(500);
                    PrintMaze(maze, mazeSize);
                }
            }
        }
    }
    static public void MoveUpRec(Square[,] maze, int mazeSize, Player player, int endX, int endY, string print)
    {
        if (maze[player.xLoc, player.yLoc - 1].wall == false && maze[player.xLoc, player.yLoc - 1].hasBeenUsed == false && maze[player.xLoc, player.yLoc - 1].deadSquare == false)
        {
            maze[player.xLoc, player.yLoc].occupied = false;
            maze[player.xLoc, player.yLoc - 1].occupied = true;
            maze[player.xLoc, player.yLoc].hasBeenUsed = true;
            player.yLoc--;
            player.moveCount++;
            if (print == "yes")
            {
                Thread.Sleep(500);
                PrintMaze(maze, mazeSize);
            }
            if (TestMazeRecurrsive(maze, mazeSize, player, endX, endY, print) == false)
            {
                maze[player.xLoc, player.yLoc + 1].occupied = true;
                maze[player.xLoc, player.yLoc].occupied = false;
                maze[player.xLoc, player.yLoc + 1].hasBeenUsed = false;
                player.yLoc++;
                player.moveCount--;
                if (print == "yes")
                {
                    Thread.Sleep(500);
                    PrintMaze(maze, mazeSize);
                }
            }
        }
    }
    static public void MoveLeftRec(Square[,] maze, int mazeSize, Player player, int endX, int endY, string print)
    {
        if (maze[player.xLoc - 1, player.yLoc].wall == false && maze[player.xLoc - 1, player.yLoc].hasBeenUsed == false && maze[player.xLoc - 1, player.yLoc].deadSquare == false)
        {
            maze[player.xLoc, player.yLoc].occupied = false;
            maze[player.xLoc - 1, player.yLoc].occupied = true;
            maze[player.xLoc, player.yLoc].hasBeenUsed = true;
            player.xLoc--;
            player.moveCount++;
            if (print == "yes")
            {
                Thread.Sleep(500);
                PrintMaze(maze, mazeSize);
            }
            if (TestMazeRecurrsive(maze, mazeSize, player, endX, endY, print) == false)
            {
                maze[player.xLoc + 1, player.yLoc].occupied = true;
                maze[player.xLoc, player.yLoc].occupied = false;
                maze[player.xLoc + 1, player.yLoc].hasBeenUsed = false;
                player.xLoc++;
                player.moveCount--;
                if (print == "yes")
                {
                    Thread.Sleep(500);
                    PrintMaze(maze, mazeSize);
                }
            }
        }
    }
    static public void MoveRightRec(Square[,] maze, int mazeSize, Player player, int endX, int endY, string print)
    {
        if (maze[player.xLoc + 1, player.yLoc].wall == false && maze[player.xLoc + 1, player.yLoc].hasBeenUsed == false && maze[player.xLoc + 1, player.yLoc].deadSquare == false)
        {
            maze[player.xLoc, player.yLoc].occupied = false;
            maze[player.xLoc + 1, player.yLoc].occupied = true;
            maze[player.xLoc, player.yLoc].hasBeenUsed = true;
            player.xLoc++;
            player.moveCount++;
            if (print == "yes")
            {
                Thread.Sleep(500);
                PrintMaze(maze, mazeSize);
            }
            if (TestMazeRecurrsive(maze, mazeSize, player, endX, endY, print) == false)
            {
                maze[player.xLoc - 1, player.yLoc].occupied = true;
                maze[player.xLoc, player.yLoc].occupied = false;
                maze[player.xLoc - 1, player.yLoc].hasBeenUsed = false;
                player.xLoc--;
                player.moveCount--;
                if (print == "yes")
                {
                    Thread.Sleep(500);
                    PrintMaze(maze, mazeSize);
                }
            }
        }
    }
    static public void ResetMaze(Square[,] maze, int mazeSize, Player player, int beginX, int beginY)
    {
        player.xLoc = beginX;
        player.yLoc = beginY;
        for (int j = 0; j < mazeSize; j++)
        {
            for (int i = 0; i < mazeSize; i++)
            {
                if (maze[i,j].wall == false)
                {
                    maze[i, j].occupied = false;
                    maze[i, j].hasBeenUsed = false;
                    maze[i, j].deadSquare = false;
                }
            }
        }
    }
}
public class Player
{
    public int xLoc, yLoc, moveCount;
    public Player()
    {
        this.xLoc = 0;
        this.yLoc = 0;
        this.moveCount = 0;
    }
}