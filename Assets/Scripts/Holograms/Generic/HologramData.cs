using System;
using UnityEngine;

public enum HologramType
{
    Simple
}

[Serializable]
public class HologramData
{   
    public string Id;
    public HologramType Type;
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 Scale;
    public string Content;

    public HologramData(string id, HologramType type, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        Id = id;
        Type = type;
        Position = position;
        Rotation = rotation;
        Scale = scale;
        Content = string.Empty;
    }

}
