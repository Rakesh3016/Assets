using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Events
{
  public class PlayerTurnChanged : GameEvent
  {
    public PlayerType playerType;

    public PlayerTurnChanged(PlayerType playerType)
    {
      this.playerType = playerType;
    }
  }
}