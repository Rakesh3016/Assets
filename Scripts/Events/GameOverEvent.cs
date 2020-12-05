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
}