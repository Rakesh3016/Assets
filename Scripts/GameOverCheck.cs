using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameOverCheck
{
  static int mapsize = 0;
  public static bool CheckGameOverConditions(Tile[,] maze, PlayerType playerType, int mapSize)
  {
    mapsize = mapSize;
    if (playerType == PlayerType.P1)
    {
      for (int i = 0; i < mapSize; i++)
      {
        if (solveMaze(maze, new Vector2Int(i, mapSize - 1), PlayerSymbol.P1))
        {
          return true;
        }
      }
    }
    else
    {
      for (int i = 0; i < mapSize; i++)
      {
        if (solveMaze(maze, new Vector2Int(mapSize - 1, i), PlayerSymbol.P2))
        {
          return true;
        }
      }
    }
    return false;
  }
  /* This function solves the Maze problem  
using Backtracking. It mainly uses  
solveMazeUtil() to solve the problem.  
It returns false if no path is possible,  
otherwise return true and prints the path  
in the form of 1s. Please note that there  
may be more than one solutions, this  
function prints one of the feasible  
solutions.*/
  private static bool solveMaze(Tile[,] maze, Vector2Int startingPosition, PlayerSymbol playerSymbol)
  {
    //int sol[N][N] = { { 0, 0, 0, 0 }, 
    //                  { 0, 0, 0, 0 }, 
    //                  { 0, 0, 0, 0 }, 
    //                  { 0, 0, 0, 0 } }; 
    int[,] sol = new int[mapsize, mapsize];
    for (int i = 0; i < mapsize; i++)
    {
      for (int j = 0; j < mapsize; j++)
      {
        sol[i, j] = 0;
      }
    }

    if (solveMazeUtil(maze, startingPosition, sol, playerSymbol)
        == false)
    {
      // printf("Solution doesn't exist");
      return false;
    }

    //printSolution(sol);
    return true;
  }

  /* A recursive utility function  
to solve Maze problem */
  private static bool solveMazeUtil(Tile[,] maze, Vector2Int position, int[,] sol, PlayerSymbol playerSymbol)
  {
    // if (x, y is goal) return true 
    if (maze[position.x, position.y].tileType == playerSymbol)
    {
      if (playerSymbol == PlayerSymbol.P1 && maze[position.x, position.y].isTilePlayer1EndGoal)
      {
        sol[position.x, position.y] = 1;
        return true;
      }
      else if (playerSymbol == PlayerSymbol.P2 && maze[position.x, position.y].isTilePlayer2EndGoal)
      {
        sol[position.x, position.y] = 1;
        return true;
      }

    }

    // Check if maze[x][y] is valid 
    if (isSafe(maze, position.x, position.y, playerSymbol) == true)
    {
      // mark x, y as part of solution path 
      sol[position.x, position.y] = 1;

      /* Move up in x direction */
      Vector2Int NextTilePosition = Virus.Utils.GetNextTile(new Vector2Int(position.x, position.y), SwipeDirection.Up, mapsize);
      if (NextTilePosition.x != -1 && NextTilePosition.y != -1 && sol[NextTilePosition.x,NextTilePosition.y] == 0)
      {
        if (solveMazeUtil(maze, NextTilePosition, sol, playerSymbol) == true)
          return true;
      }


      /* If moving in x direction 
         doesn't give solution then  
         Move down in y direction  */
      NextTilePosition = Virus.Utils.GetNextTile(new Vector2Int(position.x, position.y), SwipeDirection.Left, mapsize);
      if (NextTilePosition.x != -1 && NextTilePosition.y != -1 && sol[NextTilePosition.x, NextTilePosition.y] == 0)
      {
        if (solveMazeUtil(maze, NextTilePosition, sol, playerSymbol) == true)
          return true;
      }

      NextTilePosition = Virus.Utils.GetNextTile(new Vector2Int(position.x, position.y), SwipeDirection.Down, mapsize);
      if (NextTilePosition.x != -1 && NextTilePosition.y != -1 && sol[NextTilePosition.x, NextTilePosition.y] == 0)
      {
        if (solveMazeUtil(maze, NextTilePosition, sol, playerSymbol) == true)
          return true;
      }

      NextTilePosition = Virus.Utils.GetNextTile(new Vector2Int(position.x, position.y), SwipeDirection.Right, mapsize);
      if (NextTilePosition.x != -1 && NextTilePosition.y != -1 && sol[NextTilePosition.x, NextTilePosition.y] == 0)
      {
        if (solveMazeUtil(maze, NextTilePosition, sol, playerSymbol) == true)
          return true;
      }
      /* If none of the above movements  
         work then BACKTRACK: unmark  
         x, y as part of solution path */
      sol[position.x, position.y] = 0;
      return false;
    }

    return false;
  }

  /* A utility function to check if x,  
y is valid index for N*N maze */
  private static bool isSafe(Tile[,] maze, int x, int y, PlayerSymbol playerSymbol)
  {
    // if (x, y outside maze) return false 
    if (maze[x, y].tileType == playerSymbol)
      return true;

    return false;
  }
}
