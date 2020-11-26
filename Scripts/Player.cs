using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
  private bool moveToTarget = false;
  public float speed = 2f;
  private Vector3 target;
  public Animator animator;
  public void MoveTowards(Vector3 target)
  {
    this.target = target;
    moveToTarget = true;
  }
  public void Update()
  {
    if (moveToTarget)
    {
      float step = speed * Time.deltaTime;
      transform.position = Vector3.MoveTowards(transform.position, target, step);
      transform.LookAt(target);
      if (Vector3.Distance(transform.position, target) < 0.001f)
      {
        moveToTarget = false;
      }
    }
  }
}
