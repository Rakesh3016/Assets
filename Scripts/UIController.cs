using Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : AbstractController
{
  public Transform Player1PowerParent;
  public Transform Player2PowerParent;

  public PowerToggleWidget PowerUpToggle;

  Dictionary<PowerType, PowerToggleWidget> player1AquiredPowersAndCount;
  Dictionary<PowerType, PowerToggleWidget> player2AquiredPowersAndCount;

  public override void Initialize()
  {
    player1AquiredPowersAndCount = new Dictionary<PowerType, PowerToggleWidget>();
    player2AquiredPowersAndCount = new Dictionary<PowerType, PowerToggleWidget>();
  }

  public override void RegisterEvents()
  {
    EventManager.Instance.AddListener<UserAquiredPower>(onUserGotNewPower);
    EventManager.Instance.AddListener<PlayerTurnChanged>(onPlayerTurnChanged);
    EventManager.Instance.AddListener<UserUsedSelectedPower>(onUserUsedSelectedPower);

  }

  private void onPlayerTurnChanged(PlayerTurnChanged e)
  {
    if (e.playerType == PlayerType.P1)
    {
      Player1PowerParent.gameObject.SetActive(true);
      Player2PowerParent.gameObject.SetActive(false);
    }
    else
    {
      Player1PowerParent.gameObject.SetActive(false);
      Player2PowerParent.gameObject.SetActive(true);
    }
  }

  private void onUserUsedSelectedPower(UserUsedSelectedPower e)
  {
    if (e.type == PlayerType.P1)
    {
      PowerToggleWidget powerToggleWidget = player1AquiredPowersAndCount[e.usedPower];
      powerToggleWidget.UserUsedThePower();
    }
  }

  public override void UnRegisterEvents()
  {
    EventManager.Instance.RemoveListener<UserAquiredPower>(onUserGotNewPower);
  }
  private void onUserGotNewPower(UserAquiredPower e)
  {
    if (e.playerWhoAquiredPower == PlayerType.P1)
    {
      if (player1AquiredPowersAndCount.ContainsKey(e.powerType))
      {
        player1AquiredPowersAndCount[e.powerType].SetPowerToggleWidgetData(e.powerType, e.countOfPowerType);
      }
      else
      {
        PowerToggleWidget powerToggle = Instantiate(PowerUpToggle, Player1PowerParent);
        powerToggle.SetPowerToggleWidgetData(e.powerType, e.countOfPowerType);
        powerToggle.toggle.onValueChanged.AddListener(delegate
        {
          onChangePowerToggle(powerToggle, powerToggle.toggle.isOn);
        });
        player1AquiredPowersAndCount.Add(e.powerType, powerToggle);
      }
    }
    else
    {
      if (player2AquiredPowersAndCount.ContainsKey(e.powerType))
      {
        player2AquiredPowersAndCount[e.powerType].SetPowerToggleWidgetData(e.powerType, e.countOfPowerType);
      }
      else
      {
        PowerToggleWidget powerToggle = Instantiate(PowerUpToggle, Player2PowerParent);
        powerToggle.SetPowerToggleWidgetData(e.powerType, e.countOfPowerType);
        player2AquiredPowersAndCount.Add(e.powerType, powerToggle);
      }
    }
    //Instantiate()
  }

  private void onChangePowerToggle(PowerToggleWidget powerToggle, bool isOn)
  {
    Utils.EventAsync(new Events.UserSelectedPower(powerToggle.powerType, isOn));
  }
}
