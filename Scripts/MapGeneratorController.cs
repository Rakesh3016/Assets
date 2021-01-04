using Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneratorController : AbstractController
{
  private Vector2Int mapSize;
  public Tile tilePrefab;
  public GameObject startingPipeP1;
  public GameObject startingPipeP2;
  public Transform startingPipesParent;
  public Transform walkablesParent;
  private int mapSeed;
  private int powerSeed;

  private int tileGap = 2;
  private Vector3 startingTileOffset;

  private Tile[,] tiles;

  public override void Initialize()
  {
  }

  public override void RegisterEvents()
  {
    EventManager.Instance.AddListener<GenerateMapEvent>(onGetGenerateMapEvent);
    EventManager.Instance.AddListener<GetMapSize>(onGetMapSize);
    EventManager.Instance.AddListener<GetStartingPipeGameObject>(onGetStartingPipeGameObject);
  }


  public override void UnRegisterEvents()
  {
    EventManager.Instance.RemoveListener<GenerateMapEvent>(onGetGenerateMapEvent);
    EventManager.Instance.RemoveListener<GetStartingPipeGameObject>(onGetStartingPipeGameObject);
  }

  private void onGetMapSize(GetMapSize e)
  {
    e.getMapSize(mapSize);
  }

  private void onGetStartingPipeGameObject(GetStartingPipeGameObject e)
  {
    if (startingPipesDict.ContainsKey(e.ID))
    {
      e.action(startingPipesDict[e.ID]);
    }
  }
  //public Tile[,] GenerateMap()
  //{

  //}

  private void onGetGenerateMapEvent(GenerateMapEvent e)
  {
    mapSize = e.mapSize;
    mapSeed = e.seed;
    powerSeed = e.powerSeed;
    Debug.Log("Seeed map " + mapSeed + " power " + powerSeed);
    tiles = new Tile[mapSize.x, mapSize.y];
    GenerateStartingPipes();
    e.generatedTiles(createTiles());
  }

  Dictionary<int, GameObject> startingPipesDict = new Dictionary<int, GameObject>();

  private void GenerateStartingPipes()
  {
    startingPipesDict = new Dictionary<int, GameObject>();
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
        startingPipesDict.Add(x, startingPipePrefab);
        startingPipePrefab.transform.GetComponent<StartingPipes>().id = x;

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
        //Debug.Log("noOfNonSkippedIterations " + tempRotateNumber);
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
    float half = mapSize.x - 1;
    startingPipesParent.transform.position = new Vector3(startingPipesParent.position.x - half, startingPipesParent.position.y, startingPipesParent.position.z-half);
  }

  private Tile[,] createTiles()
  {
    generateFishersRandomArray(mapSeed);
    for (int x = 0; x < mapSize.x; x++)
    {
      for (int y = 0; y < mapSize.y; y++)
      {
        PlayerSymbol type = playerSymbolsGeneratedBlockets[x, y];//GetRandomTileType();
        Tile tempTile = Instantiate(tilePrefab, new Vector3(x * 2, 0f, y * 2), Quaternion.identity);
        tempTile.gameObject.transform.parent = walkablesParent;
        //tempTile.transform.parent = this.transform;
        tempTile.setInitialData(type, playerSymbolsGeneratedBlocketsPowers[x, y], new Vector2Int(x, y));
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
    float half = mapSize.x - 1;
    walkablesParent.position = new Vector3(walkablesParent.position.x- half, walkablesParent.position.y, walkablesParent.position.z- half);
    return tiles;
  }

  //private PlayerSymbol GetRandomTileType()
  //{
  //  if (random.Next(0, 10) > 7)
  //  { 
  //    return PlayerSymbol.Blocker;
  //  }
  //  else
  //    return PlayerSymbol.Walkable;
  //}
  PlayerSymbol[,] playerSymbolsGeneratedBlockets;
  PowerType[,] playerSymbolsGeneratedBlocketsPowers;
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


    int enumLength = Enum.GetNames(typeof(PowerType)).Length;
    PowerType[] powerTypes = new PowerType[mapSize.x];
    for (int l = 0; l < mapSize.x; l++)
    {
      int remainder = l % (enumLength - 1);// because of none
      //int storeValue = l / divider;
      powerTypes[l] = (PowerType)(Enum.GetValues(typeof(PowerType))).GetValue(remainder + 1);
    }
    powerTypes = RanNum.RanNumGenerator(powerTypes, powerSeed, mapSize.x);


    playerSymbolsGeneratedBlocketsPowers = new PowerType[mapSize.x, mapSize.y];
    for (int i = 0; i < mapSize.x; i++)
    {
      playerSymbolsGeneratedBlocketsPowers[i, playerSymbolsBlockers[i]] = powerTypes[i];
      Debug.LogWarning("Powers i " + i + " j " + playerSymbolsBlockers[i] + " Power " + powerTypes[i]);
    }

  }


}
