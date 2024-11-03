using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    public CollectableItemType type;
    public Vector2Int GridPos { get; set; }
}

public enum CollectableItemType
{
    smallDot,
    bigDot
}
