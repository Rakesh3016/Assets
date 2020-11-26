using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingPipes : MonoBehaviour
{
  public Vector2Int frontTileIndex;
  public Vector2Int GetFrontTileIndex
  {
    get
    {
      return frontTileIndex;
    }
    set
    {
      frontTileIndex = value;
    }
  }


}
