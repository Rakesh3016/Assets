using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

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
}
