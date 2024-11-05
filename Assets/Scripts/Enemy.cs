using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    public float killRange = 1;
    public EnemyType type;
    
    
    //正在移动的方向
    [HideInInspector] public MoveDirection movingDir;
    
}

public enum EnemyType
{
    BlueGhost,
    RedGhost,
}
