using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerToggleWidget : MonoBehaviour
{
  public Toggle toggle;
  public Image toggleBackGround;
  public Image toggleTarget;
  public Text togglePowerCount;
  public PowerType powerType;
  public int powerCount = 0;

  public void SetPowerToggleWidgetData(PowerType powerType, int powerCount)
  {
    switch (powerType)
    {
      case PowerType.None:
        break;
      case PowerType.Plus1:
        //toggleBackGround.sprite = ExtraMoveImage;
        //toggleTarget.sprite = ExtraMoveImage;
        toggle.interactable = true;
        togglePowerCount.text = powerCount.ToString();
        break;
      //case PowerType.Plus2:
      //  toggleBackGround.sprite = Extra2MovesImage;
      //  toggleTarget.sprite = Extra2MovesImage;
      //  togglePowerCount.text = powerCount.ToString();
      //  this.powerType = powerType;
      //  break;
      case PowerType.Spawner:
        //toggleBackGround.sprite = Robo;
        //toggleTarget.sprite = Robo;
        toggle.interactable = true;
        togglePowerCount.text = powerCount.ToString();
        break;
      case PowerType.Shield:
        toggle.interactable = true;
        togglePowerCount.text = powerCount.ToString();
        break;
    }
    this.powerCount = powerCount;
    this.powerType = powerType;
  }

  public void UserUsedThePower()
  {
    powerCount--;
    togglePowerCount.text = powerCount.ToString();
    toggle.isOn = false;
    if (powerCount <= 0)
    {
      togglePowerCount.text = "";
      toggle.interactable = false;
    }
  }
}
