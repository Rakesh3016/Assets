using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
namespace Virus
{
  public static class Utils
  {
    public static T DeepClone<T>(this T obj)
    {
      using (var ms = new MemoryStream())
      {
        var formatter = new BinaryFormatter();
        formatter.Serialize(ms, obj);
        ms.Position = 0;

        return (T)formatter.Deserialize(ms);
      }
    }



    public static Vector2Int GetNextTile(Vector2Int currentTilePosition, SwipeDirection direction, int mapSize)
    {
      Vector2Int returnPosition = new Vector2Int(-1, -1);
      switch (direction)
      {
        case SwipeDirection.Up:
          if (currentTilePosition.y == 0)
            returnPosition = new Vector2Int(-1, -1);
          else
            returnPosition = new Vector2Int(currentTilePosition.x, currentTilePosition.y - 1);
          break;
        case SwipeDirection.Down:
          if (currentTilePosition.y == mapSize - 1)
            returnPosition = new Vector2Int(-1, -1);
          else
            returnPosition = new Vector2Int(currentTilePosition.x, currentTilePosition.y + 1);
          break;
        case SwipeDirection.Left:
          if (currentTilePosition.x == mapSize - 1)
            returnPosition = new Vector2Int(-1, -1);
          else
            returnPosition = new Vector2Int(currentTilePosition.x + 1, currentTilePosition.y);
          break;
        case SwipeDirection.Right:
          if (currentTilePosition.x == 0)
            returnPosition = new Vector2Int(-1, -1);
          else
            returnPosition = new Vector2Int(currentTilePosition.x - 1, currentTilePosition.y);
          break;
      }
      return returnPosition;
    }
  }
}