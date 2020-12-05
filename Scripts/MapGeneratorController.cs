using Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneratorController : AbstractController
{
  public Vector2Int mapSize;
  public Tile tilePrefab;
  public GameObject startingPipeP1;
  public GameObject startingPipeP2;
  public Transform startingPipesParent;
  public int seed = 2;
  public bool shouldGenerateRandomSeed = false;
  private int tileGap = 2;
  private Vector3 startingTileOffset;

  private Tile[,] tiles;
  private System.Random random;

  public override void Initialize()
  {
    random = new System.Random();
    if (shouldGenerateRandomSeed)
    {
      seed = random.Next(0, 1000);
    }
    tiles = new Tile[mapSize.x, mapSize.y];
  }

  public override void RegisterEvents()
  {
    EventManager.Instance.AddListener<GenerateMapEvent>(onGetGenerateMapEvent);
    EventManager.Instance.AddListener<GetMapSize>(onGetMapSize);
  }
  public override void UnRegisterEvents()
  {
    EventManager.Instance.RemoveListener<GenerateMapEvent>(onGetGenerateMapEvent);
    EventManager.Instance.RemoveListener<GetMapSize>(onGetMapSize);
  }

  private void onGetMapSize(GetMapSize e)
  {
    e.getMapSize(mapSize);
  }

  //public Tile[,] GenerateMap()
  //{

  //}

  private void onGetGenerateMapEvent(GenerateMapEvent e)
  {
    GenerateStartingPipes();
    e.generatedTiles(createTiles());
  }

  private void GenerateStartingPipes()
  {
    int noOfIterations = mapSize.x * 4 + 4;
    int tempRotateNumber = 0;
    startingTileOffset = new Vector3(1, 0, 1);
    int noOfNonSkippedIterations = 0;
    for (int x = 0; x < noOfIterations; x++)
    {
      if (x % (mapSize.x + 1) == 0)
      {
        tempRotateNumber++;
        noOfNonSkippedIterations = 0;
      }
      else
      {

        GameObject startingPipePrefab;
        if (tempRotateNumber == 1 || tempRotateNumber == 3)
        {
          startingPipePrefab = Instantiate(startingPipeP2, new Vector3(0, 0, 0), Quaternion.identity);
        }
        else
        {
          startingPipePrefab = Instantiate(startingPipeP1, new Vector3(0, 0, 0), Quaternion.identity);
        }
        startingPipePrefab.transform.parent = startingPipesParent;
        int tempRotate2 = 0;
        for (int y = 0; y < tempRotateNumber; y++)
        {
          tempRotate2++;
          startingPipePrefab.transform.localRotation = Quaternion.Euler(0, tempRotate2 * 90, 0);//(0, tempRotateNumber * 90, 0, Space.Self);
          int distanceToTravel = 0;
          if (x > (mapSize.x + 1) * tempRotate2)
          {
            //This is to jump 6 blocks directly
            distanceToTravel = mapSize.x + 1;
            startingPipePrefab.transform.position = (-startingPipePrefab.transform.right * /*tempRotate2**/ ((tileGap * distanceToTravel) - 2)) + startingPipePrefab.transform.position;
          }
          else
          {
            //First it will come to here
            distanceToTravel = x % (mapSize.x + 1);
            startingPipePrefab.transform.position = (-startingPipePrefab.transform.right * /*tempRotate2**/ ((tileGap * distanceToTravel) - 1)) + startingPipePrefab.transform.position;
          }
        }
        //startingPipePrefab.transform.GetComponent<StartingPipes>().GetFrontTileIndex = ;
        Debug.Log("noOfNonSkippedIterations " + tempRotateNumber);
        if (tempRotateNumber == 1)
        {
          startingPipePrefab.transform.GetComponent<StartingPipes>().GetFrontTileIndex = new Vector2Int(0, noOfNonSkippedIterations);
        }
        else if (tempRotateNumber == 2)
        {
          startingPipePrefab.transform.GetComponent<StartingPipes>().GetFrontTileIndex = new Vector2Int(noOfNonSkippedIterations, mapSize.x - 1);
        }
        else if (tempRotateNumber == 3)
        {
          startingPipePrefab.transform.GetComponent<StartingPipes>().GetFrontTileIndex = new Vector2Int(mapSize.x - 1, (mapSize.x - 1) - noOfNonSkippedIterations);
        }
        else if (tempRotateNumber == 4)
        {
          startingPipePrefab.transform.GetComponent<StartingPipes>().GetFrontTileIndex = new Vector2Int((mapSize.x - 1) - noOfNonSkippedIterations, 0);
        }
        //Debug.Log("noOfNonSkippedIterations " + noOfNonSkippedIterations % mapSize.x);
        noOfNonSkippedIterations++;

      }
    }
    startingPipesParent.transform.position = startingPipesParent.transform.position - startingTileOffset;
  }

  private Tile[,] createTiles()
  {
    generateFishersRandomArray(seed);
    for (int x = 0; x < mapSize.x; x++)
    {
      for (int y = 0; y < mapSize.y; y++)
      {
        PlayerSymbol type = playerSymbolsGeneratedBlockets[x, y];//GetRandomTileType();
        Tile tempTile = Instantiate(tilePrefab, new Vector3(x * 2, 0f, y * 2), Quaternion.identity);
        //tempTile.transform.parent = this.transform;
        tempTile.setInitialData(type, new Vector2Int(x, y));
        if (x == 0)
        {
          tempTile.isTilePlayer2EndGoal = true;
        }
        if (y == 0)
        {
          tempTile.isTilePlayer1EndGoal = true;
        }
        tiles[x, y] = tempTile;
      }
    }
    return tiles;
  }

  private PlayerSymbol GetRandomTileType()
  {
    if (random.Next(0, 10) > 7)
    {
      return PlayerSymbol.Blocker;
    }
    else
      return PlayerSymbol.Walkable;
  }
  PlayerSymbol[,] playerSymbolsGeneratedBlockets;

  private void generateFishersRandomArray(int seed)
  {
    int[] playerSymbolsBlockers = new int[mapSize.x];
    for (int k = 0; k < mapSize.x; k++)
    {
      playerSymbolsBlockers[k] = k;
    }
    playerSymbolsBlockers = RanNum.RanNumGenerator(playerSymbolsBlockers, seed, mapSize.x);
    playerSymbolsGeneratedBlockets = new PlayerSymbol[mapSize.x, mapSize.y];
    for (int i = 0; i < mapSize.x; i++)
    {
      playerSymbolsGeneratedBlockets[i, playerSymbolsBlockers[i]] = PlayerSymbol.Blocker;
    }
  }


}
