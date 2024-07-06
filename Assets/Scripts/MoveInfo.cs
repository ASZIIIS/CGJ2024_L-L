using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveInfo
{
    public enum Directions{
        Up, RightUp, RightDown, Down, LeftDown, LeftUp
    }
    public Vector3 gridTarget, firstStart, firstEnd, secondStart, secondEnd;
    public MoveInfo(
        Vector3 gridTarget,
        Vector3 firstStart,
        Vector3 firstEnd,
        Vector3 secondStart,
        Vector3 secondEnd
        ){
        this.gridTarget=gridTarget;
        this.firstStart=firstStart;
        this.firstEnd=firstEnd;
        this.secondStart=secondStart;
        this.secondEnd=secondEnd;
    }
}
