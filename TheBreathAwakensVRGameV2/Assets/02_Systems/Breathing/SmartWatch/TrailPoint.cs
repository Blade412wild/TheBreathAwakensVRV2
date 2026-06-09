using System;
using UnityEngine;

[Serializable]
public struct TrailPoint
{
    public int Id;
    public Vector2 Position;
    public RectTransform Transform;
    public bool IsActive;
}
