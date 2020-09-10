using Microsoft.MixedReality.Toolkit.UI;
using System.Collections.Generic;
using UnityEngine;

public class EnableButtonsIfLocationDone : MonoBehaviour
{
    public List<Interactable> Buttons = new List<Interactable>();
    public GameObject HologramsParent;
    private bool _isEnabled;

    private void Start()
    {
        _isEnabled = HologramsParent.activeInHierarchy;
        ScanMessage();
        SetButtons();
    }

    private void ScanMessage()
    {
        if (!_isEnabled)
            InformationText.Show("Scan a QRCode.");
        else
            InformationText.Show();
    }

    private void Update()
    {
        if (_isEnabled == HologramsParent.activeInHierarchy) return;
        _isEnabled = !_isEnabled;
        ScanMessage();
        SetButtons();
    }

    private void SetButtons()
    {
        foreach (var button in Buttons)
        {
            button.IsEnabled = _isEnabled;
        }
    }

}

