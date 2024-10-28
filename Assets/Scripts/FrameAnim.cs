using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class FrameAnim : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private FrameAnimData[] data;
    
    public FrameAnimData Current { get; private set; }

    private float _elapsed = 0;
    private bool _playing = false;
    public bool updateByMono = true;
    private bool _doLoop = true;

    private void Start()
    {
        if (data.Length > 0) Play(data[0].id, _doLoop);
    }

    private void Update()
    {
        if (updateByMono && _playing && Current.Valid)
        {
            DoUpdate(Time.deltaTime);
        }
    }

    /// <summary>
    /// Update the animation
    /// </summary>
    /// <param name="delta"></param>
    /// <returns>if it is looped</returns>
    public bool DoUpdate(float delta)
    {
        if (!Current.Valid || !sprite || !gameObject) return true;

        int inTick = Mathf.FloorToInt(_elapsed / Current.frameRate);
        bool res = inTick >= Current.sprites.Length;
        if (res)
            inTick = _doLoop ? (inTick % Current.sprites.Length) : (Current.sprites.Length - 1);
        sprite.sprite = Current.sprites[inTick];

        _elapsed += delta;
        return res;
    }
    

    public void SetAlpha(float a)
    {
        sprite.color = new Color(1, 1, 1, a);
    }

    public void Play(string id, bool loop, bool instantPlayOnChange = true, bool forceChangeWhileSame = false)
    {
        if (HasAnim(id, out int aIndex))
        {
            if (Current.id == data[aIndex].id && !forceChangeWhileSame) return;

            Current = data[aIndex];
            _playing = instantPlayOnChange;
            _doLoop = loop;
            _elapsed = 0;

            DoUpdate(0);
        }
    }

    public bool HasAnim(string id, out int index)
    {
        index = -1;
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i].id == id)
            {
                index = i;
                return true;
            }
        }
        return false;
    }

    public void Pause()=> _playing = false;

    public void Resume() => _playing = true;

    public void DoLoop(bool loop) => _doLoop = loop;
}

[Serializable]
public struct FrameAnimData
{
    public string id;
    public Sprite[] sprites;
    public float frameRate;

    public bool Valid => sprites?.Length > 0 && frameRate > 0;
}