using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
  public List<AbstractController> controllers;

  private void Awake()
  {
    Initialize();
  }
  private void Initialize()
  {
    foreach (IController controller in controllers)
    {
      controller.Initialize();
      controller.RegisterEvents();
    }
  }

}
