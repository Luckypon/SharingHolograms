using Microsoft.MixedReality.QR;
using Microsoft.MixedReality.WorldLocking.Samples.Advanced.QRSpacePins;
using System.Collections.Generic;
using UnityEngine;

public class MarkersHandleEvents : MonoBehaviour
{
    public bool DoQRCode = true;
    public AMarkersManager MyMarkersManager;

    //QRCODES
    public QRCodeMiniManager QRRecogniser;
    private bool _enumerationFinished = false;


    private void OnEnable()
    {
        if (DoQRCode)
            QRSetUpCallbacks();
    }
    private void OnDestroy()
    {
        if (DoQRCode)
            QRTearDownCallbacks();
    }


    #region QR

    private void QRSetUpCallbacks()
    {
        QRRecogniser.OnQRAdded += OnQRCodeAdded;
        QRRecogniser.OnQRUpdated += OnQRCodeUpdated;
        QRRecogniser.OnQRRemoved += OnQRCodeRemoved;
        QRRecogniser.OnQREnumerated += OnQRCodeEnumerated;
    }

    /// <summary>
    /// Unregister from callbacks.
    /// </summary>
    private void QRTearDownCallbacks()
    {
        QRRecogniser.OnQRAdded -= OnQRCodeAdded;
        QRRecogniser.OnQRUpdated -= OnQRCodeUpdated;
        QRRecogniser.OnQRRemoved -= OnQRCodeRemoved;
        QRRecogniser.OnQREnumerated -= OnQRCodeEnumerated;
    }


    /// <summary>
    /// Process a newly added QR code.
    /// </summary>
    /// <param name="qrCode">The qr code to process.</param>
    private void OnQRCodeAdded(QRCode qrCode)
    {
        if (!DoQRCode) return;
        if (_enumerationFinished)
        {
            MyMarkersManager.UpdateQRCode(qrCode);
        }
    }

    /// <summary>
    /// Process a newly updated QR code.
    /// </summary>
    /// <param name="qrCode">The qr code to process.</param>
    private void OnQRCodeUpdated(QRCode qrCode)
    {
        if (!DoQRCode) return;
        if (_enumerationFinished)
        {
            MyMarkersManager.UpdateQRCode(qrCode);
        }
    }

    /// <summary>
    /// Process a newly removed QR code.
    /// </summary>
    /// <param name="qrCode">The qr code to process.</param>
    private void OnQRCodeRemoved(QRCode qrCode)
    {
        if (!DoQRCode) return;
        MyMarkersManager.ResetQRCode(qrCode);
    }

    private void OnQRCodeEnumerated(QRCode qrCode)
    {
        _enumerationFinished = true;
    }


    #endregion


}

