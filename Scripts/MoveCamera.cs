using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{

  void Update()
  {
    if (Input.GetKeyDown(KeyCode.UpArrow))
    {
      this.transform.position += Vector3.up;
    }
    else if (Input.GetKeyDown(KeyCode.DownArrow))
    {
      this.transform.position += Vector3.down;
    }
    else if (Input.GetKeyDown(KeyCode.LeftArrow))
    {
      this.transform.position += Vector3.left;
    }
    else if (Input.GetKeyDown(KeyCode.RightArrow))
    {
      this.transform.position += Vector3.right;
    }
  }
}
