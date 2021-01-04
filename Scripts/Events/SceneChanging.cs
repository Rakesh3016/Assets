using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChangingTo : GameEvent
{
  public string sceneName;

  public SceneChangingTo(string sceneName)
  {
    this.sceneName = sceneName;
  }
}
