using UnityEngine;

public class HologramsChanges : MonoBehaviour
{
    public ImportExportHolograms MyImportExportManager;

    private float _currentTime, _maxTimeToCheck = 1f;
    private bool _hologramChanged;


    internal void ChangesDone()
    {
        _hologramChanged = true;
        _currentTime = 0;
    }

    void Update()
    {
        if (!_hologramChanged) return;
        if (_currentTime < _maxTimeToCheck)
        {
            _currentTime += Time.deltaTime;
            return;
        }
        _currentTime = 0;
        _hologramChanged = false;
        MyImportExportManager.HologramHasChanged();


    }

}

