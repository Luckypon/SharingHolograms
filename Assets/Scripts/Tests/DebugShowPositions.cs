using Microsoft.MixedReality.WorldLocking.Core;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class DebugShowPositions : MonoBehaviour
{
    public Transform TransformToLookAt;
    public TextMeshPro WorldPositionText;
    public TextMeshPro LocalPositionText;
    public TextMeshPro WorldLockedPositionText;
    public TextMeshPro LocalLockedPositionText;
    public TextMeshPro WorldFrozenPositionText;
    public TextMeshPro LocalFrozenPositionText;

    private Vector3 _WorldPosition;
    private Vector3 _LocalPosition;
    private Vector3 _WorldLockedPosition;
    private Vector3 _LocalLockedPosition;
    private Vector3 _WorldFrozenPosition;
    private Vector3 _LocalFrozenPosition;

    private void Update()
    {
        _WorldPosition = TransformToLookAt.position;
        _LocalPosition = TransformToLookAt.localPosition;
        
        _WorldLockedPosition = WorldLockingManager.GetInstance().LockedFromFrozen.Multiply(TransformToLookAt.GetGlobalPose()).position;
        _LocalLockedPosition = WorldLockingManager.GetInstance().LockedFromFrozen.Multiply(TransformToLookAt.GetLocalPose()).position;

        //_WorldFrozenPosition = WorldLockingManager.GetInstance().FrozenFromSpongy.Multiply(_WorldPosition);
        //_LocalFrozenPosition = WorldLockingManager.GetInstance().FrozenFromSpongy.Multiply(_LocalPosition);

        SetTexts();
    }

    private void SetTexts()
    {
        WorldPositionText.text = "World : " + _WorldPosition.ToString("n2");
        LocalPositionText.text = "Local : " + _LocalPosition.ToString("n2");
        WorldLockedPositionText.text = "Locked World : " + _WorldLockedPosition.ToString("n2");
        LocalLockedPositionText.text = "Locked Local : " + _LocalLockedPosition.ToString("n2");
        WorldFrozenPositionText.text = TransformToLookAt.name;
        //WorldFrozenPositionText.text = _WorldFrozenPosition.ToString("n2");
        //LocalFrozenPositionText.text = _LocalFrozenPosition.ToString("n2");
    }

}
