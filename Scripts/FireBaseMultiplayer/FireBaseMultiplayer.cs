using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Unity.Editor;
using Firebase.Database;
using System;
using System.Collections.Generic;
using Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FireBaseMultiplayer : AbstractController
{
  public PlayersController player;

  PlayerType playerType = PlayerType.P1;
  DatabaseReference reference;
  DatabaseReference referenceOfUsersData;

  string[] CommonGameInfoarr;

  public override void Initialize()
  {
    StartGame();
  }

  public override void RegisterEvents()
  {
    EventManager.Instance.AddListener<GameOverEvent>(onGameOverEvent);

  }

  public override void UnRegisterEvents()
  {
    EventManager.Instance.RemoveListener<GameOverEvent>(onGameOverEvent);
    reference.ValueChanged -= HandleValueChanged;
    reference.Parent.ChildRemoved -= onOtherPlayerDisconnected;
    PlayersController.PlayerTurnChanged -= OnPlayerTurnChanged;

  }

  private void onGameOverEvent(GameOverEvent e)
  {
    if (playerType == PlayerType.P1)
    {
      if (e.playerWon == PlayerType.P1)
      {
        onUpdateUserStatsToServer(true);
      }
      else
      {
        onUpdateUserStatsToServer(false);
      }
    }
    else if (playerType == PlayerType.P2)
    {
      if (e.playerWon == PlayerType.P2)
      {
        onUpdateUserStatsToServer(true);
      }
      else
      {
        onUpdateUserStatsToServer(false);
      }
    }
  }

  private void StartGame()
  {
    FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://pipes-ca9d5-default-rtdb.firebaseio.com/");
    PlayersController.PlayerTurnChanged += OnPlayerTurnChanged;

    string gameId = PlayerPrefs.GetString("gameId");
    reference = FirebaseDatabase.DefaultInstance.GetReference("games/" + gameId + "/common");
    referenceOfUsersData = FirebaseDatabase.DefaultInstance.GetReference("Users");

    if (GameManager.Instance.CurrentGameMode == GameModes.Online)
    {
      //GameObject playerGO = GameObject.Find("Player");
      //player = playerGO.GetComponent<PlayersController>();

      DataSnapshot UserDataSnapshot = GameManager.Instance.dataSnapshotOfMatchMakingData;
      if (UserData.user.UserId == UserDataSnapshot.Child("turn").Value.ToString())
      {
        playerType = PlayerType.P1;
      }
      else
      {
        playerType = PlayerType.P2;
      }

      onUpdateUserStatsToServerInitially();
      reference.ValueChanged += HandleValueChanged;
      reference.Parent.ChildRemoved += onOtherPlayerDisconnected;
      OnDisconnect disconnect = reference.Parent.OnDisconnect();
      disconnect.RemoveValue();

    }
  }

  private void onOtherPlayerDisconnected(object sender, ChildChangedEventArgs e)
  {
    Debug.Log("other Player Disconnected");
    Utils.EventAsync(new Events.OpponentDisconnected(onClickedDisconnectClose));
  }

  private void onClickedDisconnectClose()
  {
    Utils.EventAsync(new Events.GameOverEvent(playerType));
  }

  private void OnPlayerTurnChanged(PlayerType playerType, int pressedTileX, int pressedTileZ, SwipeDirection swipeDirection, string[] powersUsed)
  {
    string PlayerType = Enum.GetName(typeof(PlayerType), playerType);
    int swipedirection = (int)swipeDirection;
    reference.SetValueAsync(PlayerType + ";" + pressedTileX + "," + pressedTileZ + ";" + swipedirection + ";" + powersUsed[0] + "," + powersUsed[1] + "," + powersUsed[2] + ";" + player.hittedPipeId);
    // Debug.Log(PlayerType + ";" + pressedTileX + "," + pressedTileZ + ";" + swipedirection + ";" + powersUsed[0] + "," + powersUsed[1] + "," + powersUsed[2] + ";" + player.hittedPipeId);
  }

  private void HandleValueChanged(object sender, ValueChangedEventArgs args)
  {
    if (args.DatabaseError == null)
    {
      if (args.Snapshot.Value != null)
      {
        string s = args.Snapshot.Value.ToString();
        CommonGameInfoarr = s.Split(';');
        /*
        //{common: PlayerType; Selected; Direction; PowerType; StartingPipeIndex}, PlayerType is Who has updated the values.
                      0           1         2           3             4
        Sample 
        common : PlayerType;Selected;Direction;PowerType;StartingPipeIndex
        common : P1;-1,-1;-1;1,0,0;14
        common : P2;-1,-1;-1;1,0,0;2
        common : P1;0,1;0;0,0,0;-1
        */

        Debug.Log(s);

        if (CommonGameInfoarr[0] == "P1")
        {
          //there are my changes so dont care about it.
          if (playerType == PlayerType.P1)
          {
            Debug.Log("I dont think its comming here");
            //player.isAI = false;
          }
          else
          {
            string[] powers = CommonGameInfoarr[3].Split(',');

            //Plus1
            if (powers[1] == "1")
            {
              Utils.EventAsync(new Events.UserSelectedPower(PowerType.Plus1, true, PlayerType.P1));
            }
            //Shield
            if (powers[2] == "1")
            {
              Utils.EventAsync(new Events.UserSelectedPower(PowerType.Shield, true, PlayerType.P1));
            }

            //Spawner
            if (powers[0] == "1")
            {
              Utils.EventAsync(new Events.UserSelectedPower(PowerType.Spawner, true, PlayerType.P1));
              player.hittedPipeId = int.Parse(CommonGameInfoarr[4]);
              player.doesHitSPOfP1Online = true;
              player.isAI = true;
              player.AIEntrancePass = true;
              //string[] selectedTile = CommonGameInfoarr[1].Split(',');
              //GameManager.Instance.player1SelectedTile = new Vector2Int(int.Parse(selectedTile[0]), int.Parse(selectedTile[1]));
              return;
            }

            //No Spawner Power Used
            string[] selectedTileArray = CommonGameInfoarr[1].Split(',');
            GameManager.Instance.player1SelectedTile = new Vector2Int(int.Parse(selectedTileArray[0]), int.Parse(selectedTileArray[1]));
            string direction = CommonGameInfoarr[2];
            switch (direction)
            {
              case "0":
                //UP
                player.didPressedW = true;
                break;
              case "1":
                //down
                player.didPressedS = true;
                break;
              case "2":
                //left
                player.didPressedA = true;
                break;
              case "3":
                //right
                player.didPressedD = true;
                break;
            }

          }
        }
        else if (CommonGameInfoarr[0] == "P2")
        {
          //there are my changes so dont care about it.
          if (playerType == PlayerType.P2)
          {
            Debug.Log("I dont think its comming here");
            //player.isAI = false;
          }
          else
          {
            string[] powers = CommonGameInfoarr[3].Split(',');

            //Plus1
            if (powers[1] == "1")
            {
              Utils.EventAsync(new Events.UserSelectedPower(PowerType.Plus1, true, PlayerType.P2));
            }
            //Shield
            if (powers[2] == "1")
            {
              Utils.EventAsync(new Events.UserSelectedPower(PowerType.Shield, true, PlayerType.P2));
            }

            //Spawner
            if (powers[0] == "1")
            {
              Utils.EventAsync(new Events.UserSelectedPower(PowerType.Spawner, true, PlayerType.P2));
              player.hittedPipeId = int.Parse(CommonGameInfoarr[4]);
              player.doesHitSPOfP2Online = true;
              player.isAI = true;
              player.AIEntrancePass = true;
              //string[] selectedTile = CommonGameInfoarr[1].Split(',');
              //GameManager.Instance.player1SelectedTile = new Vector2Int(int.Parse(selectedTile[0]), int.Parse(selectedTile[1]));
              return;
            }

            //No Spawner Power Used
            string[] selectedTileArray = CommonGameInfoarr[1].Split(',');
            GameManager.Instance.player2SelectedTile = new Vector2Int(int.Parse(selectedTileArray[0]), int.Parse(selectedTileArray[1]));
            string direction = CommonGameInfoarr[2];
            switch (direction)
            {
              case "0":
                //UP
                player.didPressedW = true;
                break;
              case "1":
                //down
                player.didPressedS = true;
                break;
              case "2":
                //left
                player.didPressedA = true;
                break;
              case "3":
                //right
                player.didPressedD = true;
                break;
            }

          }
        }
      }
    }
  }

  private void onUpdateUserStatsToServer(bool isUserWon)
  {
    int[] matchesInfo = UserData.ParseMatchesInfo();
    if (isUserWon)
    {
      ++matchesInfo[1];
    }

    if (matchesInfo[0] != 0)
    {
      matchesInfo[2] = (matchesInfo[1] / matchesInfo[0]) * 100;
    }    //NumberOfMatches, numberOfWons, WinPercentage
    UserData.matchesInfo = matchesInfo[0] + "," + matchesInfo[1] + "," + matchesInfo[2];
    referenceOfUsersData.UpdateChildrenAsync(UserData.ToDictionaryForUserData());


  }

  private void onUpdateUserStatsToServerInitially()
  {
    int[] matchesInfo = UserData.ParseMatchesInfo();
    ++matchesInfo[0];
    if (matchesInfo[0] != 0)
    {
      matchesInfo[2] = (matchesInfo[1] / matchesInfo[0]) * 100;
    }
    //NumberOfMatches, numberOfWons, WinPercentage
    UserData.matchesInfo = matchesInfo[0] + "," + matchesInfo[1] + "," + matchesInfo[2];
    referenceOfUsersData.UpdateChildrenAsync(UserData.ToDictionaryForUserData());
  }
}
