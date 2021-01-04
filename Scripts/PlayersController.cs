using Events;
using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

  public GameObject Blocker;
  public GameObject UIBlocker;


  Dictionary<PowerType, bool> userSelectedPowers = new Dictionary<PowerType, bool>();


  [HideInInspector]
  public bool isPlayer1UsingShieldPower;

  [HideInInspector]
  public bool isPlayer2UsingShieldPower;

  DatabaseReference referenceOfGameInfo;

  private bool isCurrentPlayerTurn_Online = false;


  //Vector2Int player1SelectedTile = new Vector2Int(-1, -1);
  //Vector2Int player2SelectedTile = new Vector2Int(-1, -1);
  // Start is called before the first frame update
  public override void Initialize()
  {
    Button[] AllButtons = GetComponentsInChildren<Button>();
    foreach (Button btn in AllButtons)
    {
      btn.onClick.AddListener(() => onClick(btn));
    }
    mapSize = GameManager.Instance.mapSize;
    if (GameManager.Instance.CurrentGameMode == GameModes.Online)
    {
      DataSnapshot UserDataSnapshot = GameManager.Instance.dataSnapshotOfMatchMakingData;
      if (UserData.user.UserId == UserDataSnapshot.Child("turn").Value.ToString())
      {
        isCurrentPlayerTurn_Online = true;
        isAI = false;
        ShouldEnableBlocker(!isCurrentPlayerTurn_Online);
      }
      else
      {
        isCurrentPlayerTurn_Online = false;
        isAI = true;
        ShouldEnableBlocker(!isCurrentPlayerTurn_Online);
      }
    }
    turn = PlayerType.P1;
    ResetPowers();
    //player1ActiveGenerators = new Dictionary<GameObject, Vector2Int>();
    //player2ActiveGenerators = new Dictionary<GameObject, Vector2Int>();
  }



  private void ShouldEnableBlocker(bool enableBlocker)
  {
    Blocker.SetActive(enableBlocker);
    UIBlocker.SetActive(enableBlocker);
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
    if (GameManager.Instance.CurrentGameMode == GameModes.Online)
    {
      if (!isAI)
      {
        sendDataToServer();
      }
    }
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

    if (GameManager.Instance.CurrentGameMode == GameModes.Online)
    {
      isCurrentPlayerTurn_Online = !isCurrentPlayerTurn_Online;
      ShouldEnableBlocker(!isCurrentPlayerTurn_Online);
    }


    if (turn == PlayerType.P1)
    {
      turn = PlayerType.P2;
    }
    else
      turn = PlayerType.P1;

    isAI = !isAI;
    Utils.EventAsync(new PlayerTurnChangedTo(turn));

    ResetPowers();
  }

  private void sendDataToServer()
  {
    string[] userUsedPowers = new string[3];
    if (userSelectedPowers.ContainsKey(PowerType.Spawner))
    {
      if (userSelectedPowers[PowerType.Spawner])
      {
        userUsedPowers[0] = "1";
      }
      else
        userUsedPowers[0] = "0";
    }
    if (userSelectedPowers.ContainsKey(PowerType.Plus1))
    {
      if (userSelectedPowers[PowerType.Plus1])
      {
        userUsedPowers[1] = "1";
      }
      else
        userUsedPowers[1] = "0";
    }
    if (userSelectedPowers.ContainsKey(PowerType.Shield))
    {
      if (userSelectedPowers[PowerType.Shield])
      {
        userUsedPowers[2] = "1";
      }
      else
        userUsedPowers[2] = "0";
    }
    if (turn == PlayerType.P1)
    {
      //Debug.Log("PlayerTurnChanged turn p1 " + GameManager.Instance.player1SelectedTile.x + " " + GameManager.Instance.player1SelectedTile.y + " " + lastUsedSwipeDirectionForOnline + " " + userUsedPowers);
      PlayerTurnChanged(turn, GameManager.Instance.player1SelectedTile.x, GameManager.Instance.player1SelectedTile.y, lastUsedSwipeDirectionForOnline, userUsedPowers);
    }
    else
    {
      //Debug.Log("PlayerTurnChanged turn p2 " + GameManager.Instance.player2SelectedTile.x + " " + GameManager.Instance.player2SelectedTile.y + " " + lastUsedSwipeDirectionForOnline + " " + userUsedPowers);

      PlayerTurnChanged(turn, GameManager.Instance.player2SelectedTile.x, GameManager.Instance.player2SelectedTile.y, lastUsedSwipeDirectionForOnline, userUsedPowers);
    }
  }

  public bool didPressedW = false;
  public bool didPressedA = false;
  public bool didPressedS = false;
  public bool didPressedD = false;

  public bool AIEntrancePass = false;

  public bool doesHitSPOfP1Online = false;
  public bool doesHitSPOfP2Online = false;
  public bool isAI = false;
  public int hittedPipeId = -1;


  public delegate void PlayerTurnChangedEventHandler(PlayerType playerType, int pressedTileX, int pressedTileZ, SwipeDirection swipeDirection, string[] powersUsed);
  public static event PlayerTurnChangedEventHandler PlayerTurnChanged;

  private void onClick(Button btn)
  {
    switch (btn.name)
    {
      case "W":
        didPressedW = true;
        break;
      case "A":
        didPressedA = true;
        break;
      case "S":
        didPressedS = true;
        break;
      case "D":
        didPressedD = true;
        break;
    }
  }
  SwipeDirection lastUsedSwipeDirectionForOnline = SwipeDirection.Up;
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.W) || didPressedW)
    {
      didPressedW = false;
      if (turn == PlayerType.P1)
      {
        if (!GameManager.Instance.tiles[GameManager.Instance.player1SelectedTile.x, GameManager.Instance.player1SelectedTile.y].containsP1PipeGenerator)
          return;
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
          lastUsedSwipeDirectionForOnline = SwipeDirection.Up;
          SwapPlayerTurn();
          GameManager.Instance.Swipe(SwipeDirection.Up, PlayerType.P1);
        }
        else
        {
          Debug.LogWarning("Invalid Move");
        }
      }
      else
      {
        if (!GameManager.Instance.tiles[GameManager.Instance.player2SelectedTile.x, GameManager.Instance.player2SelectedTile.y].containsP2PipeGenerator)
          return;
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

          lastUsedSwipeDirectionForOnline = SwipeDirection.Up;
          SwapPlayerTurn();
          GameManager.Instance.Swipe(SwipeDirection.Up, PlayerType.P2);
        }
        else
        {
          Debug.LogWarning("Invalid Move");
        }
      }


    }
    else if (Input.GetKeyDown(KeyCode.S) || didPressedS)
    {
      didPressedS = false;
      if (turn == PlayerType.P1)
      {
        if (!GameManager.Instance.tiles[GameManager.Instance.player1SelectedTile.x, GameManager.Instance.player1SelectedTile.y].containsP1PipeGenerator)
          return;
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

          lastUsedSwipeDirectionForOnline = SwipeDirection.Down;
          SwapPlayerTurn();
          GameManager.Instance.Swipe(SwipeDirection.Down, PlayerType.P1);
        }
        else
        {
          Debug.LogWarning("Invalid Move");
        }
      }
      else
      {
        if (!GameManager.Instance.tiles[GameManager.Instance.player2SelectedTile.x, GameManager.Instance.player2SelectedTile.y].containsP2PipeGenerator)
          return;
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

          lastUsedSwipeDirectionForOnline = SwipeDirection.Down;
          SwapPlayerTurn();
          GameManager.Instance.Swipe(SwipeDirection.Down, PlayerType.P2);
        }
        else
        {
          Debug.LogWarning("Invalid Move");
        }
      }
    }
    else if (Input.GetKeyDown(KeyCode.A) || didPressedA)
    {
      didPressedA = false;
      if (turn == PlayerType.P1)
      {
        if (!GameManager.Instance.tiles[GameManager.Instance.player1SelectedTile.x, GameManager.Instance.player1SelectedTile.y].containsP1PipeGenerator)
          return;
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

          lastUsedSwipeDirectionForOnline = SwipeDirection.Left;
          SwapPlayerTurn();
          GameManager.Instance.Swipe(SwipeDirection.Left, PlayerType.P1);
        }
        else
        {
          Debug.LogWarning("Invalid Move");
        }
      }
      else
      {
        if (!GameManager.Instance.tiles[GameManager.Instance.player2SelectedTile.x, GameManager.Instance.player2SelectedTile.y].containsP2PipeGenerator)
          return;
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

          lastUsedSwipeDirectionForOnline = SwipeDirection.Left;
          SwapPlayerTurn();
          GameManager.Instance.Swipe(SwipeDirection.Left, PlayerType.P2);
        }
        else
        {
          Debug.LogWarning("Invalid Move");
        }
      }
    }
    else if (Input.GetKeyDown(KeyCode.D) || didPressedD)
    {
      didPressedD = false;
      if (turn == PlayerType.P1)
      {
        if (!GameManager.Instance.tiles[GameManager.Instance.player1SelectedTile.x, GameManager.Instance.player1SelectedTile.y].containsP1PipeGenerator)
          return;
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

          lastUsedSwipeDirectionForOnline = SwipeDirection.Right;
          SwapPlayerTurn();
          GameManager.Instance.Swipe(SwipeDirection.Right, PlayerType.P1);
        }
        else
        {
          Debug.LogWarning("Invalid Move");
        }
      }
      else
      {
        if (!GameManager.Instance.tiles[GameManager.Instance.player2SelectedTile.x, GameManager.Instance.player2SelectedTile.y].containsP2PipeGenerator)
          return;
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

          lastUsedSwipeDirectionForOnline = SwipeDirection.Right;
          SwapPlayerTurn();
          GameManager.Instance.Swipe(SwipeDirection.Right, PlayerType.P2);
        }
        else
        {
          Debug.LogWarning("Invalid Move");
        }
      }
    }
    if (Input.GetButtonDown("Fire1") || AIEntrancePass)
    {
      RaycastHit hit;
      bool doesHitStartingPipesP1 = false;
      bool doesHitStartingPipesP2 = false;
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      if (Physics.Raycast(ray, out hit) || AIEntrancePass)
      {
        if (!AIEntrancePass)
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
              if (tile.containsP1PipeGenerator && turn == PlayerType.P1)
              {
                GameManager.Instance.player1SelectedTile = tile.position;
              }
              else if (tile.containsP2PipeGenerator && turn == PlayerType.P2)
              {
                GameManager.Instance.player2SelectedTile = tile.position;
              }
            }
            //Debug.Log("Selected Tile " + player1SelectedTile.x + " " + player1SelectedTile.y);
          }
        }
        if (doesHitStartingPipesP1 || doesHitStartingPipesP2 || doesHitSPOfP1Online || doesHitSPOfP2Online)
        {
          //StartingPipes startingPipes = hit.collider.transform.GetComponent<StartingPipes>();
          //Vector2Int position = startingPipes.GetFrontTileIndex;
          if ((turn == PlayerType.P1 && GameManager.Instance.noOfPipeGeneratorsLeftForPlayer1 > 0 && doesHitStartingPipesP1 && userSelectedPowers[PowerType.Spawner]) || doesHitSPOfP1Online)
          {
            doesHitSPOfP1Online = false;
            if (AIEntrancePass)
            {
              Utils.EventAsync(new Events.GetStartingPipeGameObject(hittedPipeId, obj =>
              {
                GameManager.Instance.AddedNewPipeGenerator(obj.transform, turn);
              }));
            }
            else
            {
              hittedPipeId = hit.collider.transform.GetComponent<StartingPipes>().id;
              GameManager.Instance.AddedNewPipeGenerator(hit.collider.transform, turn);
            }
            if (GameOverCheck.CheckGameOverConditions(GameManager.Instance.tiles, turn, mapSize.x))
            {
              Utils.EventAsync(new Events.GameOverEvent(turn));

              Debug.LogError("--------------------Dude Game Over------------------------");
              Debug.Log("--------------------Dude Game Over------------------------");
            }
            SwapPlayerTurn();
            Utils.EventAsync(new Events.UserUsedSelectedPower(PowerType.Spawner, PlayerType.P1));
          }
          else if ((turn == PlayerType.P2 && GameManager.Instance.noOfPipeGeneratorsLeftForPlayer2 > 0 && doesHitStartingPipesP2 && userSelectedPowers[PowerType.Spawner]) || doesHitSPOfP2Online)
          {
            doesHitSPOfP2Online = false;
            if (AIEntrancePass)
            {
              Utils.EventAsync(new Events.GetStartingPipeGameObject(hittedPipeId, obj =>
              {
                GameManager.Instance.AddedNewPipeGenerator(obj.transform, turn);
              }));
            }
            else
            {
              hittedPipeId = hit.collider.transform.GetComponent<StartingPipes>().id;
              GameManager.Instance.AddedNewPipeGenerator(hit.collider.transform, turn);
            }
            if (GameOverCheck.CheckGameOverConditions(GameManager.Instance.tiles, turn, mapSize.x))
            {
              Utils.EventAsync(new Events.GameOverEvent(turn));

              Debug.LogError("--------------------Dude Game Over------------------------");
              Debug.Log("--------------------Dude Game Over------------------------");
            }
            SwapPlayerTurn();
            Utils.EventAsync(new Events.UserUsedSelectedPower(PowerType.Spawner, PlayerType.P2));
          }
        }
        AIEntrancePass = false;

      }
    }
  }
}
