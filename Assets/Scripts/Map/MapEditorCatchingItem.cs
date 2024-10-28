using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditorCatchingItem : MonoBehaviour
{
    [HideInInspector]public MapPosition pos; 
    //[HideInInspector]public string name;
    
    //Vector2 is the position on the screen
    private Action<Vector2> _onMoving;

    private void Update()
    {
        _onMoving.Invoke(Input.mousePosition);
    }

    public void Set(Action<Vector2> onMoving)
    {
        _onMoving = onMoving;
    }
    
}
