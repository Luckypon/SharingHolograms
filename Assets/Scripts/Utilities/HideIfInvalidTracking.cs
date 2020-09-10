using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using UnityEngine;

public class HideIfInvalidTracking : MonoBehaviour
{
    [SerializeField]
    private SolverHandler _solverHandler;

    private void Awake()
    {
        _solverHandler = GetComponent<SolverHandler>();
        if (_solverHandler == null)
            Destroy(this);
    }

    private void Update()
    {
        if (_solverHandler == null)
        {
            Destroy(this);
            return;
        }
        GameObject gameObjectToHide = transform.GetChild(0)?.gameObject;
        if (gameObjectToHide == null) return;
        gameObjectToHide.SetActive(!_solverHandler.IsInvalidTracking());
    }
}
