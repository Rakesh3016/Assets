using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Events
{
  public class UserSelectedPower : GameEvent
  {
    public PowerType powerType;
    public bool isUserSelectedPower;
    public PlayerType playerType;

    public UserSelectedPower(PowerType powerType, bool isUserSelectedPower, PlayerType playerType)
    {
      this.powerType = powerType;
      this.isUserSelectedPower = isUserSelectedPower;
      this.playerType = playerType;
    }
  }

  public class UserUsedSelectedPower : GameEvent
  {
    public PowerType usedPower;
    public PlayerType type;

    public UserUsedSelectedPower(PowerType usedPower, PlayerType type)
    {
      this.usedPower = usedPower;
      this.type = type;
    }
  }
}
