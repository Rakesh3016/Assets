using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractController : MonoBehaviour, IController
{
  public abstract void Initialize();


  public abstract void RegisterEvents();

  public abstract void UnRegisterEvents();
}
