using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridAnimEvent : MonoBehaviour
{
    public Sprite TargetSprite;

    public void AnimEvent_FlipToSprite1()
    {
        GetComponentInChildren<SpriteRenderer>().sprite = TargetSprite;
    }
}
