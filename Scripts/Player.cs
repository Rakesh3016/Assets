using Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
  public PlayerType playerType;
  private bool moveToTarget = false;
  public float speed = 2f;
  private Vector3 target;
  public Animator animator;

  public GameObject shieldPower;

  public void Start()
  {
    EventManager.Instance.AddListener<UserSelectedPower>(onUserSelectedPower);
    EventManager.Instance.AddListener<UserUsedSelectedPower>(onUserUsedSelectedPower);

  }

  private void onUserUsedSelectedPower(UserUsedSelectedPower e)
  {
    if (e.type == playerType && e.usedPower == PowerType.Shield)
    {
      shieldPower.SetActive(false);
    }
  }

  private void onUserSelectedPower(UserSelectedPower e)
  {
    if (e.playerType == playerType && e.powerType == PowerType.Shield)
    {
      shieldPower.SetActive(true);
    }
  }
  private void OnDestroy()
  {
    EventManager.Instance.RemoveListener<UserSelectedPower>(onUserSelectedPower);
    EventManager.Instance.RemoveListener<UserUsedSelectedPower>(onUserUsedSelectedPower);
  }

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
