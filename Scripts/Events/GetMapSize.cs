using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Events
{
  public class GetMapSize : GameEvent
  {
    public Action<Vector2Int> getMapSize;

    public GetMapSize(Action<Vector2Int> getMapSize)
    {
      this.getMapSize = getMapSize;
    }
  }
}
