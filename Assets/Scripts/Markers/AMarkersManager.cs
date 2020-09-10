using Microsoft.MixedReality.QR;
using System.Collections.Generic;
using UnityEngine;

public abstract class AMarkersManager : MonoBehaviour
{
    internal abstract void UpdateQRCode(QRCode qrCode);
    internal abstract void ResetQRCode(QRCode qrCode);
}

