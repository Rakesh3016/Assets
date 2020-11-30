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
  public Sprite Extra2MovesImage;
  public Sprite ExtraMoveImage;
  public Sprite Robo;
  public PowerType powerType;
  public int powerCount = 0;

  public void SetPowerToggleWidgetData(PowerType powerType, int powerCount)
  {
    switch (powerType)
    {
      case PowerType.None:
        break;
      case PowerType.Plus1:
        toggleBackGround.sprite = ExtraMoveImage;
        toggleTarget.sprite = ExtraMoveImage;
        togglePowerCount.text = powerCount.ToString();
        break;
      //case PowerType.Plus2:
      //  toggleBackGround.sprite = Extra2MovesImage;
      //  toggleTarget.sprite = Extra2MovesImage;
      //  togglePowerCount.text = powerCount.ToString();
      //  this.powerType = powerType;
      //  break;
      case PowerType.Spawner:
        toggleBackGround.sprite = Robo;
        toggleTarget.sprite = Robo;
        togglePowerCount.text = powerCount.ToString();
        break;
    }
    this.powerCount = powerCount;
    this.powerType = powerType;
  }

  public void UserUsedThePower()
  {
    powerCount--;
    toggle.isOn = false;
    if (powerCount <= 0)
    {
      toggle.gameObject.SetActive(false);
    }
  }
}
