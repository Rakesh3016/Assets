using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Events
{
  public class PlayerTurnChangedTo : GameEvent
  {
    public PlayerType playerType;

    public PlayerTurnChangedTo(PlayerType playerType)
    {
      this.playerType = playerType;
    }
  }
}