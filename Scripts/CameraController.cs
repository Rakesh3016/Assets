using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
  public Camera orthographicCamera;
  public Camera thirdPersonPrespectiveCamera;


  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.T))
    {
      orthographicCamera.enabled = false;
      thirdPersonPrespectiveCamera.enabled = true;
    }
    else if (Input.GetKeyDown(KeyCode.O))
    {
      orthographicCamera.enabled = true;
      thirdPersonPrespectiveCamera.enabled = false;
    }
  }

  //void Update()
  //{
  //  if (Input.GetKeyDown(KeyCode.UpArrow))
  //  {
  //    this.transform.position += Vector3.down;
  //  }
  //  else if (Input.GetKeyDown(KeyCode.DownArrow))
  //  {
  //    this.transform.position += Vector3.up;
  //  }
  //  else if (Input.GetKeyDown(KeyCode.LeftArrow))
  //  {
  //    this.transform.position += Vector3.right;
  //  }
  //  else if (Input.GetKeyDown(KeyCode.RightArrow))
  //  {
  //    this.transform.position += Vector3.left;
  //  }
  //  else if (Input.GetKeyDown(KeyCode.T))
  //  {
  //    this.transform.position += Vector3.forward;
  //  }
  //  else if (Input.GetKeyDown(KeyCode.G))
  //  {
  //    this.transform.position += Vector3.back;
  //  }
  //}
}
