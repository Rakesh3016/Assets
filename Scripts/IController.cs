using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IController
{
  void Initialize();
  void RegisterEvents();
  void UnRegisterEvents();
}
