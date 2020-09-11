using Microsoft.MixedReality.WorldLocking.Core;
using UnityEngine;

public class PositionMarkerHelper : MonoBehaviour
{
    public GameObject DummyTransformPrefab;
    private Transform _dummyTransform;

    private void Awake()
    {
        CreateDummyTransform();
    }

    internal void MoveLocalPose(Vector3 localPos, Quaternion localRot, Transform parent = null)
    {
        _dummyTransform.transform.SetParent(parent != null ? parent : transform, true);
        _dummyTransform.transform.localPosition = localPos;
        _dummyTransform.transform.localRotation = localRot;
        ResetParent();
    }

    internal Pose GetGlobalPose()
    {
        return _dummyTransform.GetGlobalPose();
    }

    internal void SetGlobalPose(Pose pose)
    {
        _dummyTransform.SetGlobalPose(pose);
    }

    internal Pose GetLocalPose()
    {
        return _dummyTransform.GetLocalPose();
    }


    private void CreateDummyTransform()
    {
        if (_dummyTransform == null)
        {
            //_dummyTransform = new GameObject().transform;
            //_dummyTransform = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            if (DummyTransformPrefab != null)
                _dummyTransform = GameObject.Instantiate(DummyTransformPrefab).transform;
            else
                _dummyTransform = new GameObject().transform;
            _dummyTransform.name = "_dummyTransform";
            _dummyTransform.localScale = new Vector3(0.02f, 0.05f, 0.01f);
            // _dummyTransform.hideFlags = HideFlags.HideInHierarchy;
            ResetParent();
            MoveLocalPose(Vector3.zero, Quaternion.identity);
        }
    }

    private void ResetParent()
    {
        _dummyTransform.transform.SetParent(transform, true);
    }
}
