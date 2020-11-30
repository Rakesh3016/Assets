using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum DragDirection
{
    None,
    UP,
    Down,
    Left,
    Right,
    UpLeft,
    UpRight,
    DownLeft,
    DownRight
}

public enum DragInputLayerMask
{
    None,
    BoyFriendMask,
    GirlFriendMask
}
public class InputManager
{

    EventSystem eventSystem;
    Vector2 initialClickPosition;
    Vector2 dragPosition;
    float swipeTime;
    float swipeLength;
    float swipeSpeed;
    DragDirection dragDirection = DragDirection.None;
    DragInputLayerMask dragInputLayerMask = DragInputLayerMask.None;
    public LayerMask boyFriendSwipeLayer;
    public LayerMask girlFriendSwipeLayer;
    private int boyfirendLayerMask;
    private int girlfirendLayerMask;
    private bool isDragEnded;
    float min = 0.5f, max = 1;

    public InputManager()
    {
        eventSystem = EventSystem.current;
        GameObject[] draginput = GameObject.FindGameObjectsWithTag("DragInput");
        boyfirendLayerMask = 8;
        girlfirendLayerMask = 1 << 9;
        foreach (GameObject obj in draginput)
            Debug.Log("DragInput "+obj.name);
        foreach (GameObject dragObjs in draginput)
        {
            if (dragObjs.layer == 8)
            {
                Debug.Log("DragLayer " + dragObjs.name);
                boyFriendSwipeLayer = dragObjs.layer;
            }
            else if (dragObjs.layer == 9)
            {
                Debug.Log("DragLayer " + dragObjs.name);
                girlFriendSwipeLayer = dragObjs.layer;
            }
        }
    }

    public void Update()
    {
        DetectSwipe();
    }

    void DetectSwipe()
    {
        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {

                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 10.5f, boyfirendLayerMask))
                {
                    Debug.Log("DetectedSwipe for boyFriendSwipeLayer");
                    onRayCastHit(touch, DragInputLayerMask.BoyFriendMask);
                }
                else if (Physics.Raycast(ray, out hit,10.5f, girlfirendLayerMask))
                {
                    Debug.Log("DetectedSwipe for girlFriendSwipeLayer");
                    onRayCastHit(touch, DragInputLayerMask.GirlFriendMask);
                }
                //else {
                //    Debug.Log("DetectedSwipe for none ");

                //    onRayCastHit(touch, boyFriendSwipeLayer);
                //}
               // Debug.Log("hit " + hit.collider.name);

            }
        }
    }

    private void onRayCastHit(Touch touch, DragInputLayerMask layerMask)
    {
       
        dragInputLayerMask = DragInputLayerMask.BoyFriendMask;
       
        if (touch.phase == TouchPhase.Began)
        {
            SetInitialTouchPosition(touch.position);
        }
        else if (touch.phase == TouchPhase.Moved)
        {
            UpdateTouchPosition(touch.position);
        }
        else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            EndTouch(touch.position);
        }
    }

    private void SetInitialTouchPosition(Vector2 position)
    {
        initialClickPosition = position;
        swipeTime = 0;
        swipeLength = 0;
        isDragEnded = false;
        swipeSpeed = 0;
    }

    private void UpdateTouchPosition(Vector2 position)
    {
        dragPosition = position;
        swipeLength = (initialClickPosition - dragPosition).magnitude;
        swipeTime += Time.deltaTime;
        Vector2 DragVectioDirection = (dragPosition - initialClickPosition).normalized;
        dragDirection =  GetSwipeDirection(DragVectioDirection);
        //Debug.LogError("DragDirection " + dragDirection);
        isDragEnded = false;

        //TODO:Swipe Speed Not Implemented
        Utils.EventAsync(new Events.DragEvent(swipeLength, dragDirection, swipeTime, isDragEnded, dragInputLayerMask));
    }
    
    private void EndTouch(Vector2 position)
    {
        dragPosition = position;
        swipeLength = (initialClickPosition - dragPosition).magnitude;
        swipeTime += Time.deltaTime;
        Vector2 DragVectioDirection = (dragPosition - initialClickPosition).normalized;
        dragDirection = GetSwipeDirection(DragVectioDirection);
        swipeSpeed = GetSwipeSpeed();

        Debug.LogError("DragDirection End" + dragDirection);
        isDragEnded = true;

        Utils.EventSync(new Events.DragEvent(swipeLength, dragDirection, swipeSpeed, isDragEnded, dragInputLayerMask));
        resetDragEvents();
    }

    private float GetSwipeSpeed()
    {
        float speedFactor = GEtResolutionIndependentValue(swipeLength) / swipeTime;
        speedFactor = Mathf.Clamp(speedFactor, min, max);
        return (speedFactor /= max);
    }

    private float GEtResolutionIndependentValue(float swipeLength)
    {
        float UpdatedValue = swipeLength / Screen.width;
        return UpdatedValue > 1 ? 1 : UpdatedValue;
    }

    private void resetDragEvents()
    {
        dragDirection = DragDirection.None;
        swipeTime = 0;
        swipeLength = 0;
    }

    private DragDirection GetSwipeDirection(Vector2 dragDirection)
    {
        //0.906 = cos(25)
        if (Vector2.Dot(dragDirection, new Vector2(0, 1)) > 0.906)
        {
            return DragDirection.UP;
        }
        else if (Vector2.Dot(dragDirection, new Vector2(0, -1)) > 0.906)
        {
            return DragDirection.Down;
        }
        else if (Vector2.Dot(dragDirection, new Vector2(-1, 0)) > 0.906)
        {
            return DragDirection.Left;
        }
        else if (Vector2.Dot(dragDirection, new Vector2(1, 0)) > 0.906)
        {
            return DragDirection.Right;
        }
        else if (Vector2.Dot(dragDirection, new Vector2(1, -1)) > 0.906)
        {
            return DragDirection.DownRight;
        }
        else if (Vector2.Dot(dragDirection, new Vector2(1, 1)) > 0.906)
        {
            return DragDirection.UpRight;
        }
        else if (Vector2.Dot(dragDirection, new Vector2(-1, -1)) > 0.906)
        {
            return DragDirection.DownLeft;
        }
        else if (Vector2.Dot(dragDirection, new Vector2(-1, 1)) > 0.906)
        {
            return DragDirection.UpLeft;
        }
        else
        {
            return DragDirection.None;
        }
    }

   
}
