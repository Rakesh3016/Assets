using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
  public static void EventAsync(GameEvent gameEvent)
  {
    EventManager.Instance.TriggerEvent(gameEvent);
  }
  public static void EventSync(GameEvent gameEvent)
  {
    EventManager.Instance.QueueEvent(gameEvent);
  }

  public static Vector2 GetCartesianCoordinates(DragDirection dragDirection)
  {
    if (dragDirection == DragDirection.UP)
    {
      return new Vector2(0, 1);
    }
    else if (dragDirection == DragDirection.Down)
    {
      return new Vector2(0, -1);
    }
    else if (dragDirection == DragDirection.Left)
    {
      return new Vector2(-1, 0);
    }
    else if (dragDirection == DragDirection.Right)
    {
      return new Vector2(1, 0);
    }
    else if (dragDirection == DragDirection.DownRight)
    {
      return new Vector2(1, -1);
    }
    else if (dragDirection == DragDirection.UpRight)
    {
      return new Vector2(1, 1);
    }
    else if (dragDirection == DragDirection.DownLeft)
    {
      return new Vector2(-1, -1);
    }
    else if (dragDirection == DragDirection.UpLeft)
    {
      return new Vector2(-1, 1);
    }
    else
    {
      return new Vector2(0, 0);
    }
  }
  //public static GameObject GetGameObjectInLayer(int LayerNumber)
  //{
  //    GameObject[] gameObjects = (GameObject)Transform.FindObjectsOfType(typeof(GameObject)); 
  //}
}