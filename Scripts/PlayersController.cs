using Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersController : AbstractController
{
  PlayerType turn;
  //public PlayerSymbol[,] MainAllTileArrays;
  Vector2Int mapSize;
  //public GameObject player1Prefab;
  //public GameObject player2Prefab;
  //Dictionary<GameObject, Vector2Int> player1ActiveGenerators;
  //Dictionary<GameObject, Vector2Int> player2ActiveGenerators;
  private float speed;

  Dictionary<PowerType, bool> userSelectedPowers = new Dictionary<PowerType, bool>();


  [HideInInspector]
  public bool isPlayer1UsingShieldPower;

  [HideInInspector]
  public bool isPlayer2UsingShieldPower;

  //Vector2Int player1SelectedTile = new Vector2Int(-1, -1);
  //Vector2Int player2SelectedTile = new Vector2Int(-1, -1);
  // Start is called before the first frame update
  public override void Initialize()
  {
    turn = PlayerType.P1;
    ResetPowers();
    //player1ActiveGenerators = new Dictionary<GameObject, Vector2Int>();
    //player2ActiveGenerators = new Dictionary<GameObject, Vector2Int>();
  }

  private void ResetPowers()
  {
    foreach (PowerType powerType in Enum.GetValues(typeof(PowerType)))
    {
      if (userSelectedPowers.ContainsKey(powerType))
      {
        userSelectedPowers[powerType] = false;
      }
      else
        userSelectedPowers.Add(powerType, false);
    }
  }

  public override void RegisterEvents()
  {
    EventManager.Instance.AddListener<UserSelectedPower>(onUserSelectedPower);
  }

  public override void UnRegisterEvents()
  {
    EventManager.Instance.RemoveListener<UserSelectedPower>(onUserSelectedPower);
  }


  private void onUserSelectedPower(UserSelectedPower e)
  {
    userSelectedPowers[e.powerType] = e.isUserSelectedPower;
    if (e.powerType == PowerType.Shield && turn == PlayerType.P1)
    {
      isPlayer1UsingShieldPower = e.isUserSelectedPower;
    }
    else if (e.powerType == PowerType.Shield && turn == PlayerType.P2)
    {
      isPlayer2UsingShieldPower = e.isUserSelectedPower;
    }
  }

  private void SwapPlayerTurn()
  {
    if (userSelectedPowers[PowerType.Plus1] == true)
    {
      userSelectedPowers[PowerType.Plus1] = false;
      Utils.EventAsync(new Events.UserUsedSelectedPower(PowerType.Plus1, turn));
      return;
    }

    if (turn == PlayerType.P1 && isPlayer2UsingShieldPower)
    {
      isPlayer2UsingShieldPower = false;
      Utils.EventAsync(new Events.UserUsedSelectedPower(PowerType.Shield, PlayerType.P2));

    }
    else if (turn == PlayerType.P2 && isPlayer1UsingShieldPower)
    {
      isPlayer1UsingShieldPower = false;
      Utils.EventAsync(new Events.UserUsedSelectedPower(PowerType.Shield, PlayerType.P1));

    }

    if (turn == PlayerType.P1)
    {
      turn = PlayerType.P2;
    }
    else
      turn = PlayerType.P1;

    Utils.EventAsync(new PlayerTurnChangedTo(turn));

    ResetPowers();
  }

  void Update()
  {
    if (Input.GetKeyDown(KeyCode.W))
    {
      if (turn == PlayerType.P1)
      {
        Vector2Int NextTilePosition = Virus.Utils.GetNextTile(new Vector2Int(GameManager.Instance.player1SelectedTile.x, GameManager.Instance.player1SelectedTile.y), SwipeDirection.Up, mapSize.x);
        if (NextTilePosition.x != -1 || NextTilePosition.y != -1)
        {
          if (isPlayer2UsingShieldPower && GameManager.Instance.tiles[NextTilePosition.x, NextTilePosition.y].tileType == PlayerSymbol.P2)
          {
            foreach (KeyValuePair<GameObject, Vector2Int> b in GameManager.Instance.player2ActiveGenerators)
            {
              if (b.Value == NextTilePosition)
              {
                return;
              }
            }
          }
          GameManager.Instance.Swipe(SwipeDirection.Up, turn);
          SwapPlayerTurn();
        }
        else
        {
          Debug.LogWarning("Invalid Move");
        }
      }
      else
      {
        Vector2Int NextTilePosition = Virus.Utils.GetNextTile(new Vector2Int(GameManager.Instance.player2SelectedTile.x, GameManager.Instance.player2SelectedTile.y), SwipeDirection.Up, mapSize.x);
        if (NextTilePosition.x != -1 || NextTilePosition.y != -1)
        {
          if (isPlayer1UsingShieldPower && GameManager.Instance.tiles[NextTilePosition.x, NextTilePosition.y].tileType == PlayerSymbol.P1)
          {
            foreach (KeyValuePair<GameObject, Vector2Int> b in GameManager.Instance.player1ActiveGenerators)
            {
              if (b.Value == NextTilePosition)
              {
                return;
              }
            }
          }
          GameManager.Instance.Swipe(SwipeDirection.Up, turn);
          SwapPlayerTurn();
        }
        else
        {
          Debug.LogWarning("Invalid Move");
        }
      }


    }
    else if (Input.GetKeyDown(KeyCode.S))
    {
      if (turn == PlayerType.P1)
      {
        Vector2Int NextTilePosition = Virus.Utils.GetNextTile(new Vector2Int(GameManager.Instance.player1SelectedTile.x, GameManager.Instance.player1SelectedTile.y), SwipeDirection.Down, mapSize.x);
        if (NextTilePosition.x != -1 || NextTilePosition.y != -1)
        {
          if (isPlayer2UsingShieldPower && GameManager.Instance.tiles[NextTilePosition.x, NextTilePosition.y].tileType == PlayerSymbol.P2)
          {
            foreach (KeyValuePair<GameObject, Vector2Int> b in GameManager.Instance.player2ActiveGenerators)
            {
              if (b.Value == NextTilePosition)
              {
                return;
              }
            }
          }
          GameManager.Instance.Swipe(SwipeDirection.Down, turn);
          SwapPlayerTurn();
        }
        else
        {
          Debug.LogWarning("Invalid Move");
        }
      }
      else
      {
        Vector2Int NextTilePosition = Virus.Utils.GetNextTile(new Vector2Int(GameManager.Instance.player2SelectedTile.x, GameManager.Instance.player2SelectedTile.y), SwipeDirection.Down, mapSize.x);
        if (NextTilePosition.x != -1 || NextTilePosition.y != -1)
        {
          if (isPlayer1UsingShieldPower && GameManager.Instance.tiles[NextTilePosition.x, NextTilePosition.y].tileType == PlayerSymbol.P1)
          {
            foreach (KeyValuePair<GameObject, Vector2Int> b in GameManager.Instance.player1ActiveGenerators)
            {
              if (b.Value == NextTilePosition)
              {
                return;
              }
            }
          }
          GameManager.Instance.Swipe(SwipeDirection.Down, turn);
          SwapPlayerTurn();
        }
        else
        {
          Debug.LogWarning("Invalid Move");
        }
      }
    }
    else if (Input.GetKeyDown(KeyCode.A))
    {
      if (turn == PlayerType.P1)
      {
        Vector2Int NextTilePosition = Virus.Utils.GetNextTile(new Vector2Int(GameManager.Instance.player1SelectedTile.x, GameManager.Instance.player1SelectedTile.y), SwipeDirection.Left, mapSize.x);
        if (NextTilePosition.x != -1 || NextTilePosition.y != -1)
        {
          if (isPlayer2UsingShieldPower && GameManager.Instance.tiles[NextTilePosition.x, NextTilePosition.y].tileType == PlayerSymbol.P2)
          {
            foreach (KeyValuePair<GameObject, Vector2Int> b in GameManager.Instance.player2ActiveGenerators)
            {
              if (b.Value == NextTilePosition)
              {
                return;
              }
            }
          }
          GameManager.Instance.Swipe(SwipeDirection.Left, turn);
          SwapPlayerTurn();
        }
        else
        {
          Debug.LogWarning("Invalid Move");
        }
      }
      else
      {
        Vector2Int NextTilePosition = Virus.Utils.GetNextTile(new Vector2Int(GameManager.Instance.player2SelectedTile.x, GameManager.Instance.player2SelectedTile.y), SwipeDirection.Left, mapSize.x);
        if (NextTilePosition.x != -1 || NextTilePosition.y != -1)
        {
          if (isPlayer1UsingShieldPower && GameManager.Instance.tiles[NextTilePosition.x, NextTilePosition.y].tileType == PlayerSymbol.P1)
          {
            foreach (KeyValuePair<GameObject, Vector2Int> b in GameManager.Instance.player1ActiveGenerators)
            {
              if (b.Value == NextTilePosition)
              {
                return;
              }
            }
          }
          GameManager.Instance.Swipe(SwipeDirection.Left, turn);
          SwapPlayerTurn();
        }
        else
        {
          Debug.LogWarning("Invalid Move");
        }
      }
    }
    else if (Input.GetKeyDown(KeyCode.D))
    {
      if (turn == PlayerType.P1)
      {
        Vector2Int NextTilePosition = Virus.Utils.GetNextTile(new Vector2Int(GameManager.Instance.player1SelectedTile.x, GameManager.Instance.player1SelectedTile.y), SwipeDirection.Right, mapSize.x);
        if (NextTilePosition.x != -1 || NextTilePosition.y != -1)
        {
          if (isPlayer2UsingShieldPower && GameManager.Instance.tiles[NextTilePosition.x, NextTilePosition.y].tileType == PlayerSymbol.P2)
          {
            foreach (KeyValuePair<GameObject, Vector2Int> b in GameManager.Instance.player2ActiveGenerators)
            {
              if (b.Value == NextTilePosition)
              {
                return;
              }
            }
          }
          GameManager.Instance.Swipe(SwipeDirection.Right, turn);
          SwapPlayerTurn();
        }
        else
        {
          Debug.LogWarning("Invalid Move");
        }
      }
      else
      {
        Vector2Int NextTilePosition = Virus.Utils.GetNextTile(new Vector2Int(GameManager.Instance.player2SelectedTile.x, GameManager.Instance.player2SelectedTile.y), SwipeDirection.Right, mapSize.x);
        if (NextTilePosition.x != -1 || NextTilePosition.y != -1)
        {
          if (isPlayer1UsingShieldPower && GameManager.Instance.tiles[NextTilePosition.x, NextTilePosition.y].tileType == PlayerSymbol.P1)
          {
            foreach (KeyValuePair<GameObject, Vector2Int> b in GameManager.Instance.player1ActiveGenerators)
            {
              if (b.Value == NextTilePosition)
              {
                return;
              }
            }
          }
          GameManager.Instance.Swipe(SwipeDirection.Right, turn);
          SwapPlayerTurn();
        }
        else
        {
          Debug.LogWarning("Invalid Move");
        }
      }
    }
    if (Input.GetButtonDown("Fire1"))
    {
      RaycastHit hit;
      bool doesHitStartingPipesP1 = false;
      bool doesHitStartingPipesP2 = false;
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      if (Physics.Raycast(ray, out hit))
      {
        if (hit.collider != null)
        {
          if (hit.collider.transform.tag == "StartingPipes")
            doesHitStartingPipesP1 = true;
          else if (hit.collider.transform.tag == "StartingPipesP2")
            doesHitStartingPipesP2 = true;
          else if (hit.collider.transform.tag == "Walkable")
          {
            Tile tile = hit.collider.transform.GetComponent<Tile>();
            if (tile.containsPipeGenerator && tile.tileType == PlayerSymbol.P1 && turn == PlayerType.P1)
            {
              GameManager.Instance.player1SelectedTile = tile.position;
            }
            else if (tile.containsPipeGenerator && tile.tileType == PlayerSymbol.P2 && turn == PlayerType.P2)
            {
              GameManager.Instance.player2SelectedTile = tile.position;
            }
          }
          //Debug.Log("Selected Tile " + player1SelectedTile.x + " " + player1SelectedTile.y);
        }
        if (doesHitStartingPipesP1 || doesHitStartingPipesP2)
        {
          //StartingPipes startingPipes = hit.collider.transform.GetComponent<StartingPipes>();
          //Vector2Int position = startingPipes.GetFrontTileIndex;
          if (turn == PlayerType.P1 && GameManager.Instance.noOfPipeGeneratorsLeftForPlayer1 > 0 && doesHitStartingPipesP1)
          {
            Utils.EventAsync(new Events.UserUsedSelectedPower(PowerType.Spawner, turn));
            GameManager.Instance.AddedNewPipeGenerator(hit.collider.transform, turn);
            SwapPlayerTurn();
          }
          else if (turn == PlayerType.P2 && GameManager.Instance.noOfPipeGeneratorsLeftForPlayer2 > 0 && doesHitStartingPipesP2)
          {
            Utils.EventAsync(new Events.UserUsedSelectedPower(PowerType.Spawner, turn));
            GameManager.Instance.AddedNewPipeGenerator(hit.collider.transform, turn);
            SwapPlayerTurn();
          }

        }
      }
    }
  }
}
