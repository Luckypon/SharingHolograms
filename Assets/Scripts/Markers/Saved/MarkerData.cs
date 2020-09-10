using System;
using UnityEngine;

[Serializable]
public class MarkerData
{
    public string Id;
    public Vector3 Position;
    public Quaternion Rotation;
    public float SizeMeters;
    public bool IsQR; // true : Qr, false : ...

    public MarkerData(bool isQr, string id, Vector3 position, Quaternion rotation, float sizeMeters = 0)
    {
        Id = id;
        Position = position;
        Rotation = rotation;
        SizeMeters = sizeMeters;
        IsQR = isQr;
    }
}
