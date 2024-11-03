using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private FrameAnim anim;  
    public float moveSpeed = 5;
    public Vector2 bodySize = Vector2.one;
    
    public bool Dead { get; private set; } = false;
    
    private const float DeadCleanInSec = 1.2f;
    private float _elapsed = 0;

    private void Update()
    {
        if (Dead)
        {
            if (_elapsed >= DeadCleanInSec) return;
            _elapsed += Time.deltaTime;
            float p = Mathf.Clamp01(_elapsed / DeadCleanInSec);
            anim.SetAlpha(1 - p);
            if (_elapsed >= DeadCleanInSec) Destroy(gameObject);
        }
    }

    public Vector3 TryMove(MoveDirection wishDirection, float delta)
    {
        float moveInTick = delta * moveSpeed;
        Vector3 meAt = transform.position;
        Vector3 dest = wishDirection switch
        {
            MoveDirection.Up => meAt + Vector3.up * moveInTick,
            MoveDirection.Down => meAt + Vector3.down * moveInTick,
            MoveDirection.Left => meAt + Vector3.left * moveInTick,
            MoveDirection.Right => meAt + Vector3.right * moveInTick,
            _ => meAt
        };
        string dirId = wishDirection switch
        {
            MoveDirection.Up => "Up",
            MoveDirection.Down => "Down",
            MoveDirection.Left => "Left",
            MoveDirection.Right => "Right",
            _ => anim.Current.id
        };
        anim.Play(dirId, true);
        return dest;
    }
}

[Serializable]
public enum MoveDirection
{
    None,
    Up,
    Down,
    Left,
    Right
}