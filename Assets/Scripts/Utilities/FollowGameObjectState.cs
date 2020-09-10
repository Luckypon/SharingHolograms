using System.Collections.Generic;
using UnityEngine;

public class FollowGameObjectState : MonoBehaviour
{
    public List<GameObject> GameObjectsToChange = new List<GameObject>();
    public GameObject GameObjectToFollowState;
    private bool _isEnabled;

    private void Start()
    {
        _isEnabled = GameObjectToFollowState.activeInHierarchy;
        SetGameObjectsState();
    }

    private void Update()
    {
        if (_isEnabled == GameObjectToFollowState.activeInHierarchy) return;
        _isEnabled = !_isEnabled;
        SetGameObjectsState();
    }

    private void SetGameObjectsState()
    {
        foreach (var go in GameObjectsToChange)
        {
            go.SetActive(_isEnabled);
        }
    }

}
