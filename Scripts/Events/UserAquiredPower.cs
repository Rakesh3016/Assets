using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Events
{
  public class UserAquiredPower : GameEvent
  {
    public PlayerType playerWhoAquiredPower;
    public PowerType powerType;
    public int countOfPowerType;

    public UserAquiredPower(PlayerType playerWhoAquiredPower, PowerType powerType, int countOfPowerType)
    {
      this.playerWhoAquiredPower = playerWhoAquiredPower;
      this.powerType = powerType;
      this.countOfPowerType = countOfPowerType;
    }
  }
}