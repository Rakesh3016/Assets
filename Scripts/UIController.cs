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

  //public PowerToggleWidget PowerUpToggle;

  [SerializeField]
  PowerPrefabDictionary player1AquiredPowersAndCountSerilized;

  public IDictionary<PowerType, PowerToggleWidget> Player1PowersAndCount
  {
    get { return player1AquiredPowersAndCountSerilized; }
    set { player1AquiredPowersAndCountSerilized.CopyFrom(value); }
  }

  [SerializeField]
  PowerPrefabDictionary player2AquiredPowersAndCountSerilized;

  public IDictionary<PowerType, PowerToggleWidget> Player2PowersAndCount
  {
    get { return player2AquiredPowersAndCountSerilized; }
    set { player2AquiredPowersAndCountSerilized.CopyFrom(value); }
  }
  //Dictionary<PowerType, PowerToggleWidget> player1AquiredPowersAndCount;
  //Dictionary<PowerType, PowerToggleWidget> player2AquiredPowersAndCount;

  public override void Initialize()
  {
    foreach (KeyValuePair<PowerType, PowerToggleWidget> Player1Power in player1AquiredPowersAndCountSerilized)
    {
      Player1Power.Value.toggle.onValueChanged.AddListener(delegate
          {
            onChangePowerToggle(Player1Power.Value, Player1Power.Value.toggle.isOn);
          });
    }
    foreach (KeyValuePair<PowerType, PowerToggleWidget> Player2Power in player2AquiredPowersAndCountSerilized)
    {
      Player2Power.Value.toggle.onValueChanged.AddListener(delegate
      {
        onChangePowerToggle(Player2Power.Value, Player2Power.Value.toggle.isOn);
      });
    }
    //player1AquiredPowersAndCount = new Dictionary<PowerType, PowerToggleWidget>();
    //player2AquiredPowersAndCount = new Dictionary<PowerType, PowerToggleWidget>();
  }

  public override void RegisterEvents()
  {
    EventManager.Instance.AddListener<UserAquiredPower>(onUserGotNewPower);
    EventManager.Instance.AddListener<PlayerTurnChanged>(onPlayerTurnChanged);
    EventManager.Instance.AddListener<UserUsedSelectedPower>(onUserUsedSelectedPower);

  }

  private void onPlayerTurnChanged(PlayerTurnChanged e)
  {
    //if (e.playerType == PlayerType.P1)
    //{
    //  Player1PowerParent.gameObject.SetActive(true);
    //  Player2PowerParent.gameObject.SetActive(false);
    //}
    //else
    //{
    //  Player1PowerParent.gameObject.SetActive(false);
    //  Player2PowerParent.gameObject.SetActive(true);
    //}
  }

  private void onUserUsedSelectedPower(UserUsedSelectedPower e)
  {
    if (e.type == PlayerType.P1)
    {
      PowerToggleWidget powerToggleWidget = Player1PowersAndCount[e.usedPower];
      powerToggleWidget.UserUsedThePower();
    }
    else
    {
      PowerToggleWidget powerToggleWidget = Player2PowersAndCount[e.usedPower];
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
      if (Player1PowersAndCount.ContainsKey(e.powerType))
      {
        Player1PowersAndCount[e.powerType].SetPowerToggleWidgetData(e.powerType, e.countOfPowerType);
      }
      else
      {
        //  PowerToggleWidget powerToggle = Instantiate(PowerUpToggle, Player1PowerParent);
        //  powerToggle.SetPowerToggleWidgetData(e.powerType, e.countOfPowerType);
        //  powerToggle.toggle.onValueChanged.AddListener(delegate
        //  {
        //    onChangePowerToggle(powerToggle, powerToggle.toggle.isOn);
        //  });
        //  player1AquiredPowersAndCount.Add(e.powerType, powerToggle);
      }
    }
    else
    {
      if (Player2PowersAndCount.ContainsKey(e.powerType))
      {
        Player2PowersAndCount[e.powerType].SetPowerToggleWidgetData(e.powerType, e.countOfPowerType);
      }
      else
      {
        //PowerToggleWidget powerToggle = Instantiate(PowerUpToggle, Player2PowerParent);
        //powerToggle.SetPowerToggleWidgetData(e.powerType, e.countOfPowerType);
        //player2AquiredPowersAndCount.Add(e.powerType, powerToggle);
      }
    }
    //Instantiate()
  }

  private void onChangePowerToggle(PowerToggleWidget powerToggle, bool isOn)
  {
    Utils.EventAsync(new Events.UserSelectedPower(powerToggle.powerType, isOn));
  }
}
