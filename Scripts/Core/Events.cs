using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public class DragEvent : GameEvent
    {
        public float swipeLength;
        public DragDirection dragDirection;
        public float swipeSpeed;
        public DragInputLayerMask layerMask;
        public bool isDragEnded;
        public DragEvent(float swipeLength, DragDirection dragDirection, float swipeSpeed, bool isDragEnded, DragInputLayerMask layerMask)
        {
            this.swipeLength = swipeLength;
            this.dragDirection = dragDirection;
            this.swipeSpeed = swipeSpeed;
            this.isDragEnded = isDragEnded;
            this.layerMask = layerMask;
        }
    }

    public class NothoingEvent : GameEvent
    {
        public string working;
        public NothoingEvent(string Working)
        {
            this.working = Working;
        }
    }
}
