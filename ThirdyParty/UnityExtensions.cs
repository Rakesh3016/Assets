using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Linq;
using UnityEngine;

public static class UnityExtensions
{
  public static void InvokeMethodInChildrenAndSelf<T>(this GameObject gameObject, Action<T> handler)
  {
    var decendents = gameObject.DescendantsAndSelf();
    var components = decendents.ToArray(x => x.GetComponent<T>());

    int length = components.Length;
    for (int i = 0; i < length; i++)
    {
      if (components[i] != null)
      {
        handler.Invoke(components[i]);
      }
    }
  }
}
