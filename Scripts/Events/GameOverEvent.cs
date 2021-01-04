using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Events
{
  public class GameOverEvent : GameEvent
  {
    public PlayerType playerWon;

    public GameOverEvent(PlayerType playerWon)
    {
      this.playerWon = playerWon;
    }
  }

  public class OpponentDisconnected : GameEvent
  {
    public Action onClose;

    public OpponentDisconnected(Action onClose)
    {
      this.onClose = onClose;
    }
  }

  public class OutOfTime : GameEvent
  {
    public Action onClose;

    public OutOfTime(Action onClose)
    {
      this.onClose = onClose;
    }
  }

  //public class UpdateUserStatsToServer : GameEvent
  //{
  //  public bool isUserWon;

  //  public UpdateUserStatsToServer(bool isUserWon)
  //  {
  //    this.isUserWon = isUserWon;
  //  }
  //}
}