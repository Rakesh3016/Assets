using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum PlayerType
{
  P1, P2
}

public enum PlayerSymbol
{
  Walkable = 0,
  Blocker = 1,
  P1 = 2,
  P2 = 3
}

public enum SwipeDirection
{
  Up,
  Down,
  Left,
  Right
}