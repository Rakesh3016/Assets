using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
  public MapGenerator mapGenerator;
  public PlayersController player;
  public Tile[,] tiles;
  public int noOfPipeGeneratorsLeftForPlayer1 = 3;
  public int noOfPipeGeneratorsLeftForPlayer2 = 3;

  public Vector2Int player1SelectedTile = new Vector2Int(-1, -1);
  public Vector2Int player2SelectedTile = new Vector2Int(-1, -1);

  public GameObject player1Prefab;
  public GameObject player2Prefab;

  Dictionary<GameObject, Vector2Int> player1ActiveGenerators;
  Dictionary<GameObject, Vector2Int> player2ActiveGenerators;

  public static GameManager Instance
  {
    get;
    private set;
  }

  public Vector2Int getMapSize
  {
    get
    {
      return mapGenerator.mapSize;
    }
  }
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

    mapGenerator.Initialize();
    tiles = mapGenerator.GenerateMap();
    player.Initialize();
  }

  public void SwipeOld(SwipeDirection swipeDirection, PlayerType turn)
  {
    Debug.Log("Button Down Pressed");

    PlayerSymbol currentPlayerSymbol;
    if (turn == PlayerType.P1)
      currentPlayerSymbol = PlayerSymbol.P1;
    else
      currentPlayerSymbol = PlayerSymbol.P2;

    Tile[,] previousStateTileArray = new Tile[mapGenerator.mapSize.x, mapGenerator.mapSize.y];
    for (int x = 0; x < mapGenerator.mapSize.x; x++)
    {
      for (int y = 0; y < mapGenerator.mapSize.y; y++)
      {
        previousStateTileArray[x, y] = (Tile)tiles[x, y].Shallowcopy();
      }
    }
    //Tile[,] previousStateTileArray = Utils.DeepClone<Tile[,]>(tiles);
    //Tile[,] previousStateTileArray = new Tile[mapGenerator.mapSize.x, mapGenerator.mapSize.y];
    //Array.Copy(tiles, previousStateTileArray, tiles.Length);
    //Tile[,] previousStateTileArray = tiles.Clone() as Tile[,];

    for (int x = 0; x < mapGenerator.mapSize.x; x++)
    {
      for (int y = 0; y < mapGenerator.mapSize.y; y++)
      {
        if (previousStateTileArray[x, y].tileType == currentPlayerSymbol)
        {
          Vector2Int NextTilePosition = GetNextTile(new Vector2Int(x, y), swipeDirection);
          if (NextTilePosition.x != -1 || NextTilePosition.y != -1)
          {
            if (previousStateTileArray[NextTilePosition.x, NextTilePosition.y].tileType == PlayerSymbol.Walkable)
            {
              if (turn == PlayerType.P1)
                tiles[NextTilePosition.x, NextTilePosition.y].setPlayer1Data();
              else
                tiles[NextTilePosition.x, NextTilePosition.y].setPlayer2Data();
            }
            else if (previousStateTileArray[NextTilePosition.x, NextTilePosition.y].tileType != currentPlayerSymbol)
            {
              tiles[NextTilePosition.x, NextTilePosition.y].setWalkableData();
            }
          }
        }
      }
    }
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
      Vector2Int NextTilePosition = GetNextTile(new Vector2Int(currentPosition.x, currentPosition.y), swipeDirection);
      tiles[currentPosition.x, currentPosition.y].containsPipeGenerator = false;

      if (NextTilePosition.x != -1 || NextTilePosition.y != -1)
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
        else if (tiles[NextTilePosition.x, NextTilePosition.y].tileType != currentPlayerSymbol)
        {
          DestroyIfTheTileHasOppositePlayer(turn, NextTilePosition);
          tiles[NextTilePosition.x, NextTilePosition.y].setWalkableData();
        }
        else if (tiles[NextTilePosition.x, NextTilePosition.y].tileType == currentPlayerSymbol)
        {
          setPlayersCommonFunctionality(currentPosition, NextTilePosition, turn);
        }
      }
    }

    playerTurnChanged(turn);
    //  }
    //}
  }

  private void playerTurnChanged(PlayerType turn)
  {
    foreach (Tile tempTile in tiles)
    {
      if (tempTile.tileType == PlayerSymbol.Walkable)
      {
        tempTile.setPlayerIndicator(turn);
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
      player.MoveTowards(new Vector3(tiles[position.x, position.y].transform.position.x, player.transform.position.y, tiles[position.x, position.y].transform.position.z));
      player1SelectedTile = position;// hit.collider.transform.GetComponent<Tile>().position;
      tiles[position.x, position.y].setPlayer1Data();
    }
    else if (playerType == PlayerType.P2 && noOfPipeGeneratorsLeftForPlayer2 > 0)
    {
      GameObject player2Instance = Instantiate(player2Prefab, startingPipe.position, Quaternion.identity);
      Player player = player2Instance.GetComponent<Player>();
      player2ActiveGenerators.Add(player2Instance, position);
      noOfPipeGeneratorsLeftForPlayer2--;
      player.MoveTowards(new Vector3(tiles[position.x, position.y].transform.position.x, player.transform.position.y, tiles[position.x, position.y].transform.position.z));
      player2SelectedTile = position;// hit.collider.transform.GetComponent<Tile>().position;
      tiles[position.x, position.y].setPlayer2Data();
    }
    playerTurnChanged(playerType);

  }

  private Vector2Int GetNextTile(Vector2Int currentTilePosition, SwipeDirection direction)
  {
    Vector2Int returnPosition = new Vector2Int(-1, -1);
    switch (direction)
    {
      case SwipeDirection.Up:
        if (currentTilePosition.y == 0)
          returnPosition = new Vector2Int(-1, -1);
        else
          returnPosition = new Vector2Int(currentTilePosition.x, currentTilePosition.y - 1);
        break;
      case SwipeDirection.Down:
        if (currentTilePosition.y == mapGenerator.mapSize.y - 1)
          returnPosition = new Vector2Int(-1, -1);
        else
          returnPosition = new Vector2Int(currentTilePosition.x, currentTilePosition.y + 1);
        break;
      case SwipeDirection.Left:
        if (currentTilePosition.x == mapGenerator.mapSize.x - 1)
          returnPosition = new Vector2Int(-1, -1);
        else
          returnPosition = new Vector2Int(currentTilePosition.x + 1, currentTilePosition.y);
        break;
      case SwipeDirection.Right:
        if (currentTilePosition.x == 0)
          returnPosition = new Vector2Int(-1, -1);
        else
          returnPosition = new Vector2Int(currentTilePosition.x - 1, currentTilePosition.y);
        break;
    }
    return returnPosition;
  }
}
