using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Events
{
  public class GenerateMapEvent : GameEvent
  {
    public Action<Tile[,]> generatedTiles;
    public Vector2Int mapSize;
    public int seed;
    public int powerSeed;

    public GenerateMapEvent(Vector2Int mapSize, int seed, int powerSeed, Action<Tile[,]> generatedTiles)
    {
      this.generatedTiles = generatedTiles;
      this.mapSize = mapSize;
      this.seed = seed;
      this.powerSeed = powerSeed;
    }
  }

  public class GetStartingPipeGameObject : GameEvent
  {
    public int ID;
    public Action<GameObject> action;

    public GetStartingPipeGameObject(int iD, Action<GameObject> action)
    {
      ID = iD;
      this.action = action;
    }
  }
}