using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct State
{
    public string Name;
    
    [Range(0, 1)] 
    public float TimeBetweenFrames;
    
    public List<Sprite> SpriteSheet;
}

