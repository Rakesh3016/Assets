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
  }

  private void SwapPlayerTurn()
  {
    if (userSelectedPowers[PowerType.Plus1] == true)
    {
      userSelectedPowers[PowerType.Plus1] = false;
      Utils.EventAsync(new Events.UserUsedSelectedPower(PowerType.Plus1, turn));
      return;
    }
    if (turn == PlayerType.P1)
    {
      turn = PlayerType.P2;
    }
    else
      turn = PlayerType.P1;

    Utils.EventAsync(new PlayerTurnChanged(turn));

    ResetPowers();
  }

  void Update()
  {
    if (Input.GetKeyDown(KeyCode.W))
    {
      GameManager.Instance.Swipe(SwipeDirection.Up, turn);

      SwapPlayerTurn();
    }
    else if (Input.GetKeyDown(KeyCode.S))
    {
      GameManager.Instance.Swipe(SwipeDirection.Down, turn);
      SwapPlayerTurn();
    }
    else if (Input.GetKeyDown(KeyCode.A))
    {
      GameManager.Instance.Swipe(SwipeDirection.Left, turn);
      SwapPlayerTurn();
    }
    else if (Input.GetKeyDown(KeyCode.D))
    {
      GameManager.Instance.Swipe(SwipeDirection.Right, turn);
      SwapPlayerTurn();
    }
    if (Input.GetButtonDown("Fire1"))
    {
      RaycastHit hit;
      bool doesHitStartingPipes = false;
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      if (Physics.Raycast(ray, out hit))
      {
        if (hit.collider != null)
        {
          if (hit.collider.transform.tag == "StartingPipes")
            doesHitStartingPipes = true;
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
        if (doesHitStartingPipes)
        {
          //StartingPipes startingPipes = hit.collider.transform.GetComponent<StartingPipes>();
          //Vector2Int position = startingPipes.GetFrontTileIndex;
          if (turn == PlayerType.P1 && GameManager.Instance.noOfPipeGeneratorsLeftForPlayer1 > 0)
          {
            GameManager.Instance.AddedNewPipeGenerator(hit.collider.transform, turn);
            SwapPlayerTurn();
          }
          else if (turn == PlayerType.P2 && GameManager.Instance.noOfPipeGeneratorsLeftForPlayer2 > 0)
          {
            GameManager.Instance.AddedNewPipeGenerator(hit.collider.transform, turn);
            SwapPlayerTurn();
          }
        }
      }
    }
  }
}
