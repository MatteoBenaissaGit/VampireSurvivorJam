namespace PlateformCharacterController2D
{
    using System;
    using UnityEngine;


    [Serializable]
    public struct CollisionBox
    {
        [Range(0, 5)] public float CollisionBoxWidth;
        [Range(0, 5)] public float CollisionBoxHeight;
        [Range(-5, 5)] public float CollisionBoxOffsetX;
        [Range(-5, 5)] public float CollisionBoxOffsetY;
    }

    [Serializable]
    public struct Walk
    {
        [Range(0, 20)] public float MaximumSpeed;
        [Range(0, 200)] public float Acceleration;
        [Range(0, 200)] public float Deceleration;
        [Range(0, 10)] public float ApexBonus;
    }

    [Serializable]
    public struct Jump
    {
        [Range(0, 50)] public float JumpHeight;
        [Range(0, 50)] public float JumpApexThreshold;
        [Range(0, 1)] public float JumpBuffer;
    }

    [Serializable]
    public struct Inputs
    {
        public float X;
        public bool Jump;
    }

    [Serializable]
    public struct Gravity
    {
        [Range(-50, 0)] public float _fallClamp;
        [Range(0, 100)] public float _minFallSpeed;
        [Range(0, 200)] public float _maxFallSpeed;
    }

    [Serializable]
    public struct RayRange
    {
        public RayRange(float x1, float y1, float x2, float y2, Vector2 direction)
        {
            Start = new Vector2(x1, y1);
            End = new Vector2(x2, y2);
            Direction = direction;
        }

        public readonly Vector2 Start, End, Direction;
    }
}