using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Events
{
  public class GenerateMapEvent : GameEvent
  {
    public Action<Tile[,]> generatedTiles;

    public GenerateMapEvent(Action<Tile[,]> generatedTiles)
    {
      this.generatedTiles = generatedTiles;
    }
  }
}