using System.Collections;
using System.Collections.Generic;


public static class RanNum
{
  public static T[] RanNumGenerator<T>(T[] array, int seed)
  {
    System.Random prng = new System.Random(seed);
    int mapsize = GameManager.Instance.getMapSize.x;

    for (int i = 0; i < mapsize; i++)
    {
      int randomIndex = prng.Next(i, mapsize);
      T tempItem = array[randomIndex];
      array[randomIndex] = array[i];
      array[i] = tempItem;
    }
    return array;
  }

}
