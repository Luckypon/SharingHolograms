using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class HologramDataWrapper
{
    #region SIMPLE_HOLOGRAM
    internal static HologramData CreateSimpleHologramData(string id, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        var data = new HologramData(id, HologramType.Simple, position, rotation, scale)
        {
            Content = ""
        };
        return data;
    }
    #endregion

}

