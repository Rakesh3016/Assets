using Events;
using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerUIController : AbstractController
{

  PlayerType playerType = PlayerType.P1;
  public Text currentTurn;
  public override void Initialize()
  {
    if (GameManager.Instance.CurrentGameMode == GameModes.Offline)
    {
      this.gameObject.SetActive(false);
      return;
    }
    else
    {
      this.gameObject.SetActive(true);
      DataSnapshot UserDataSnapshot = GameManager.Instance.dataSnapshotOfMatchMakingData;
      if (UserData.user.UserId == UserDataSnapshot.Child("turn").Value.ToString())
      {
        playerType = PlayerType.P1;
        currentTurn.text = "Your's Turn";
      }
      else
      {
        playerType = PlayerType.P2;
        currentTurn.text = "Opponents Turn";

      }
    }
  }

  public override void RegisterEvents()
  {
    EventManager.Instance.AddListener<PlayerTurnChangedTo>(playerTurnChanged);

  }


  public override void UnRegisterEvents()
  {
    EventManager.Instance.RemoveListener<PlayerTurnChangedTo>(playerTurnChanged);

  }

  private void playerTurnChanged(PlayerTurnChangedTo e)
  {
    if (e.playerType == playerType)
    {
      currentTurn.text = "Your's Turn";
    }
    else
    {
      currentTurn.text = "Opponents Turn";
    }
  }

}
