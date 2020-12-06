using Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
  public List<AbstractController> controllers;
  //public MapGeneratorController mapGenerator;
  //public PlayersController player;
  public Tile[,] tiles;

  [HideInInspector]
  public int noOfPipeGeneratorsLeftForPlayer1 = 1;
  [HideInInspector]
  public int noOfPipeGeneratorsLeftForPlayer2 = 1;

  public Vector2Int player1SelectedTile = new Vector2Int(-1, -1);
  public Vector2Int player2SelectedTile = new Vector2Int(-1, -1);

  public GameObject player1Prefab;
  public GameObject player2Prefab;
  [HideInInspector]
  public Dictionary<GameObject, Vector2Int> player1ActiveGenerators;
  [HideInInspector]
  public Dictionary<GameObject, Vector2Int> player2ActiveGenerators;

  Dictionary<PowerType, int> player1AquiredPowersAndCount;
  Dictionary<PowerType, int> player2AquiredPowersAndCount;

  private Vector2Int mapSize;

  public static GameManager Instance
  {
    get;
    private set;
  }

  //public Vector2Int getMapSize
  //{
  //  get
  //  {
  //    return mapGenerator.mapSize;
  //  }
  //}
  private void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
    }
    Initialize();
  }

  private void Initialize()
  {
    player1ActiveGenerators = new Dictionary<GameObject, Vector2Int>();
    player2ActiveGenerators = new Dictionary<GameObject, Vector2Int>();

    player1AquiredPowersAndCount = new Dictionary<PowerType, int>();
    player2AquiredPowersAndCount = new Dictionary<PowerType, int>();

    foreach (IController controller in controllers)
    {
      controller.Initialize();
      controller.RegisterEvents();
    }
    //mapGenerator.Initialize();
    //tiles = mapGenerator.GenerateMap();
    //player.Initialize();

    Utils.EventAsync(new Events.GenerateMapEvent(onGetGeneratedTiles));
    Utils.EventAsync(new Events.GetMapSize(onGetMapSize));

    RegisterEvents();

    Utils.EventAsync(new Events.UserAquiredPower(PlayerType.P1, PowerType.Spawner, 1));
    Utils.EventAsync(new Events.UserAquiredPower(PlayerType.P2, PowerType.Spawner, 1));

  }

  private void RegisterEvents()
  {
    EventManager.Instance.AddListener<PlayerTurnChangedTo>(playerTurnChanged);

  }


  private void onGetMapSize(Vector2Int obj)
  {
    mapSize = obj;
  }

  private void onGetGeneratedTiles(Tile[,] obj)
  {
    tiles = obj;
  }

  public void AddedNewPipeGenerator(Transform startingPipe, PlayerType playerType)
  {
    StartingPipes startingPipes = startingPipe.GetComponent<StartingPipes>();
    Vector2Int position = startingPipes.GetFrontTileIndex;
    if (playerType == PlayerType.P1 && noOfPipeGeneratorsLeftForPlayer1 > 0)
    {
      GameObject player1Instance = Instantiate(player1Prefab, startingPipe.position, Quaternion.identity);
      Player player = player1Instance.GetComponent<Player>();
      player1ActiveGenerators.Add(player1Instance, position);
      noOfPipeGeneratorsLeftForPlayer1--;

      NextTileOperations(playerType, PlayerSymbol.P1, position, position);

      //player.MoveTowards(new Vector3(tiles[position.x, position.y].transform.position.x, player.transform.position.y, tiles[position.x, position.y].transform.position.z));
      //player1SelectedTile = position;// hit.collider.transform.GetComponent<Tile>().position;
      //tiles[position.x, position.y].setPlayer1Data();
    }
    else if (playerType == PlayerType.P2 && noOfPipeGeneratorsLeftForPlayer2 > 0)
    {
      GameObject player2Instance = Instantiate(player2Prefab, startingPipe.position, Quaternion.identity);
      Player player = player2Instance.GetComponent<Player>();
      player2ActiveGenerators.Add(player2Instance, position);
      noOfPipeGeneratorsLeftForPlayer2--;

      NextTileOperations(playerType, PlayerSymbol.P2, position, position);
      //player.MoveTowards(new Vector3(tiles[position.x, position.y].transform.position.x, player.transform.position.y, tiles[position.x, position.y].transform.position.z));
      //player2SelectedTile = position;// hit.collider.transform.GetComponent<Tile>().position;
      //tiles[position.x, position.y].setPlayer2Data();
    }
    //playerTurnChanged(playerType);

  }
  public void Swipe(SwipeDirection swipeDirection, PlayerType turn)
  {
    Debug.Log("Button Down Pressed");

    PlayerSymbol currentPlayerSymbol;
    Vector2Int currentPosition;
    if (turn == PlayerType.P1)
    {
      currentPlayerSymbol = PlayerSymbol.P1;
      currentPosition = player1SelectedTile;
    }
    else
    {
      currentPlayerSymbol = PlayerSymbol.P2;
      currentPosition = player2SelectedTile;
    }
    //Tile[,] previousStateTileArray = new Tile[mapGenerator.mapSize.x, mapGenerator.mapSize.y];
    //for (int x = 0; x < mapGenerator.mapSize.x; x++)
    //{
    //  for (int y = 0; y < mapGenerator.mapSize.y; y++)
    //  {
    //    previousStateTileArray[x, y] = (Tile)tiles[x, y].Shallowcopy();
    //  }
    //}
    //Tile[,] previousStateTileArray = Utils.DeepClone<Tile[,]>(tiles);
    //Tile[,] previousStateTileArray = new Tile[mapGenerator.mapSize.x, mapGenerator.mapSize.y];
    //Array.Copy(tiles, previousStateTileArray, tiles.Length);
    //Tile[,] previousStateTileArray = tiles.Clone() as Tile[,];

    //for (int x = 0; x < mapGenerator.mapSize.x; x++)
    //{
    //  for (int y = 0; y < mapGenerator.mapSize.y; y++)
    //  {
    if (tiles[currentPosition.x, currentPosition.y].tileType == currentPlayerSymbol)
    {
      Vector2Int NextTilePosition = Virus.Utils.GetNextTile(new Vector2Int(currentPosition.x, currentPosition.y), swipeDirection, mapSize.x);

      if (NextTilePosition.x != -1 || NextTilePosition.y != -1)
      {
        tiles[currentPosition.x, currentPosition.y].containsPipeGenerator = false;
        NextTileOperations(turn, currentPlayerSymbol, currentPosition, NextTilePosition);

        //Check For Game End Condition
        if (GameOverCheck.CheckGameOverConditions(tiles, turn, mapSize.x))
        {
          Utils.EventAsync(new Events.GameOverEvent(turn));

          Debug.LogError("--------------------Dude Game Over------------------------");
          Debug.Log("--------------------Dude Game Over------------------------");
        }
      }
    }

    //  }
    //}
  }

  private void NextTileOperations(PlayerType turn, PlayerSymbol currentPlayerSymbol, Vector2Int currentPosition, Vector2Int NextTilePosition)
  {
    if (tiles[NextTilePosition.x, NextTilePosition.y].tileType == PlayerSymbol.Walkable)
    {
      if (turn == PlayerType.P1)
      {
        setPlayersCommonFunctionality(currentPosition, NextTilePosition, turn);
        tiles[NextTilePosition.x, NextTilePosition.y].setPlayer1Data();
      }
      else
      {
        setPlayersCommonFunctionality(currentPosition, NextTilePosition, turn);

        tiles[NextTilePosition.x, NextTilePosition.y].setPlayer2Data();
      }
    }
    else if (tiles[NextTilePosition.x, NextTilePosition.y].tileType == PlayerSymbol.Blocker)
    {
      if (turn == PlayerType.P1)
      {
        if (player1AquiredPowersAndCount.ContainsKey(tiles[NextTilePosition.x, NextTilePosition.y].powerType))
        {
          player1AquiredPowersAndCount[tiles[NextTilePosition.x, NextTilePosition.y].powerType] = player1AquiredPowersAndCount[tiles[NextTilePosition.x, NextTilePosition.y].powerType] + 1;
        }
        else
        {
          player1AquiredPowersAndCount.Add(tiles[NextTilePosition.x, NextTilePosition.y].powerType, 1);
        }
        if (tiles[NextTilePosition.x, NextTilePosition.y].powerType == PowerType.Spawner)
        {
          noOfPipeGeneratorsLeftForPlayer1++;
        }
        Utils.EventAsync(new Events.UserAquiredPower(turn, tiles[NextTilePosition.x, NextTilePosition.y].powerType, player1AquiredPowersAndCount[tiles[NextTilePosition.x, NextTilePosition.y].powerType]));

        tiles[NextTilePosition.x, NextTilePosition.y].setPlayer1Data();
      }
      else
      {
        if (player2AquiredPowersAndCount.ContainsKey(tiles[NextTilePosition.x, NextTilePosition.y].powerType))
        {
          player2AquiredPowersAndCount[tiles[NextTilePosition.x, NextTilePosition.y].powerType] = player2AquiredPowersAndCount[tiles[NextTilePosition.x, NextTilePosition.y].powerType] + 1;
        }
        else
        {
          player2AquiredPowersAndCount.Add(tiles[NextTilePosition.x, NextTilePosition.y].powerType, 1);
        }
        if (tiles[NextTilePosition.x, NextTilePosition.y].powerType == PowerType.Spawner)
        {
          noOfPipeGeneratorsLeftForPlayer2++;
        }
        Utils.EventAsync(new Events.UserAquiredPower(turn, tiles[NextTilePosition.x, NextTilePosition.y].powerType, player2AquiredPowersAndCount[tiles[NextTilePosition.x, NextTilePosition.y].powerType]));

        tiles[NextTilePosition.x, NextTilePosition.y].setPlayer2Data();
      }
      tiles[NextTilePosition.x, NextTilePosition.y].powerType = PowerType.None;
      setPlayersCommonFunctionality(currentPosition, NextTilePosition, turn);
    }
    else if (tiles[NextTilePosition.x, NextTilePosition.y].tileType != currentPlayerSymbol)
    {
      DestroyIfTheTileHasOppositePlayer(turn, NextTilePosition);
      //tiles[NextTilePosition.x, NextTilePosition.y].setWalkableData();
      setPlayersCommonFunctionality(currentPosition, NextTilePosition, turn);
      if (turn == PlayerType.P1)
      {
        tiles[NextTilePosition.x, NextTilePosition.y].setPlayer1Data();
      }
      else
      {
        tiles[NextTilePosition.x, NextTilePosition.y].setPlayer2Data();
      }
    }
    else if (tiles[NextTilePosition.x, NextTilePosition.y].tileType == currentPlayerSymbol)
    {
      setPlayersCommonFunctionality(currentPosition, NextTilePosition, turn);
    }
  }

  private void playerTurnChanged(PlayerTurnChangedTo playerTurnChanged)
  {
    foreach (Tile tempTile in tiles)
    {
      if (tempTile.tileType == PlayerSymbol.Walkable)
      {
        tempTile.setPlayerIndicator(playerTurnChanged.playerType);
      }
    }
  }

  private void DestroyIfTheTileHasOppositePlayer(PlayerType turn, Vector2Int nextTilePosition)
  {
    if (turn == PlayerType.P1)
    {
      GameObject seletedPipesGenerator = null;
      foreach (KeyValuePair<GameObject, Vector2Int> b in player2ActiveGenerators)
      {
        if (b.Value == nextTilePosition)
        {
          seletedPipesGenerator = b.Key;
          break;
        }
      }
      if (seletedPipesGenerator != null)
      {
        player2ActiveGenerators.Remove(seletedPipesGenerator);
        Destroy(seletedPipesGenerator);
        if (player2ActiveGenerators.Count <= 0)
        {
          Utils.EventAsync(new Events.GameOverEvent(turn));
        }
      }
    }
    else
    {
      GameObject seletedPipesGenerator = null;
      foreach (KeyValuePair<GameObject, Vector2Int> b in player1ActiveGenerators)
      {
        if (b.Value == nextTilePosition)
        {
          seletedPipesGenerator = b.Key;
          break;
        }
      }
      if (seletedPipesGenerator != null)
      {
        player1ActiveGenerators.Remove(seletedPipesGenerator);
        Destroy(seletedPipesGenerator);
        if (player1ActiveGenerators.Count <= 0)
        {
          Utils.EventAsync(new Events.GameOverEvent(turn));
        }
      }
    }
  }

  public void setPlayersCommonFunctionality(Vector2Int currentPosition, Vector2Int NextTilePosition, PlayerType turn)
  {
    if (turn == PlayerType.P1)
    {
      GameObject seletedPipesGenerator = null;
      foreach (KeyValuePair<GameObject, Vector2Int> b in player1ActiveGenerators)
      {
        if (b.Value == currentPosition)
        {
          seletedPipesGenerator = b.Key;
          break;
        }
      }
      if (seletedPipesGenerator != null)
      {

        Player player = seletedPipesGenerator.GetComponent<Player>();
        player.MoveTowards(new Vector3(tiles[NextTilePosition.x, NextTilePosition.y].transform.position.x, player.transform.position.y, tiles[NextTilePosition.x, NextTilePosition.y].transform.position.z));
        player1ActiveGenerators[seletedPipesGenerator] = NextTilePosition;
      }
      player1SelectedTile = NextTilePosition;
    }
    else
    {
      GameObject seletedPipesGenerator = null;
      foreach (KeyValuePair<GameObject, Vector2Int> b in player2ActiveGenerators)
      {
        if (b.Value == currentPosition)
        {
          seletedPipesGenerator = b.Key;
          break;
        }
      }
      if (seletedPipesGenerator != null)
      {

        Player player = seletedPipesGenerator.GetComponent<Player>();
        player.MoveTowards(new Vector3(tiles[NextTilePosition.x, NextTilePosition.y].transform.position.x, player.transform.position.y, tiles[NextTilePosition.x, NextTilePosition.y].transform.position.z));
        player2ActiveGenerators[seletedPipesGenerator] = NextTilePosition;
      }
      tiles[NextTilePosition.x, NextTilePosition.y].setPlayer2Data();
      player2SelectedTile = NextTilePosition;
    }
  }

  System.Random random = new System.Random();
  public int GetRandomNumber(int min, int max)
  {
    return random.Next(min, max);
  }

}
