using UnityEngine;

public class DisableGameObjectInEditor : MonoBehaviour
{
#if UNITY_EDITOR
    private void Awake()
    {
        gameObject.SetActive(false);
    }
#endif
}

