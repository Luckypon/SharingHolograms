using Microsoft.MixedReality.QR;
using Microsoft.MixedReality.WorldLocking.Core;
using Microsoft.MixedReality.WorldLocking.Samples.Advanced.QRSpacePins;
using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PositionMarkerHelper))]
public class MarkerSpacePinManager : AMarkerManager
{
    public enum DoAfterScan
    {
        Nothing,
        UpdateWorld,
        UpdateMarker
    }

    public Transform VirtualMarker;
    public GameObject HighlightProxy;
    public GameObject UpdateWorldButton, UpdatePoseButton, DeleteButton;

    internal static int index = 0;

    private PositionMarkerHelper _positionMarkerHelper;
    private SpacePinOrientable _spacePin = null;
    private Coroutine m_myCoroutineRef;
    private float _sizeMeters = 1.0f;
    private MarkersSpacePinsManager _myManager;

    private string _id = "";
    private bool _isQR;
    private Pose _lastLockedPose = Pose.identity;
    private DoAfterScan _doAfterScan = DoAfterScan.Nothing;

    public string Id { get => _id; }
    public bool IsQR { get => _isQR; }
    internal DoAfterScan ToDoAfterScan { get => _doAfterScan; }

    // QRCODES
    private QRSpatialCoord _coordinateSystem = null;

    private void Awake()
    {
        _positionMarkerHelper = GetComponent<PositionMarkerHelper>();
    }

    private void OnDestroy()
    {
        ResetSpacePin();
        if (m_myCoroutineRef != null)
            StopCoroutine(m_myCoroutineRef);
        m_myCoroutineRef = null;
    }

    internal bool IsSpacePinActive()
    {
        if (_spacePin == null) return false;
        return _spacePin.PinActive;
    }


    internal void SetAgainLockedPose()
    {
        if (!_spacePin.PinActive) return;
        if (_lastLockedPose == Pose.identity) return;
        _spacePin.SetLockedPose(_lastLockedPose);
    }

    internal void Create(string id, bool isQR, MarkersSpacePinsManager manager, float sizeMeters)
    {
        _id = id;
        transform.name += _id;
        VirtualMarker.transform.name += _id;
        _isQR = isQR;
        _sizeMeters = sizeMeters;
        _myManager = manager;
        if (_isQR)
        {
            _coordinateSystem = new QRSpatialCoord();
        }

        _spacePin = VirtualMarker.gameObject.AddComponent<SpacePinOrientable>();
        _spacePin.Orienter = _myManager.MyOrienter;

        _spacePin.ResetModelingPose();
        ShowHighlightProxy(false);
    }


    internal void ResetSpacePin()
    {
        if (_spacePin != null)
            _spacePin.Reset();
        ShowHighlightProxy(false);
    }



    internal void UpdateIfActive()
    {
        Debug.Log($"PinActive={_spacePin.PinActive}: id={_id}");
        if (!_spacePin.PinActive) return;

        _lastLockedPose = WorldLockingManager.GetInstance().LockedFromFrozen.Multiply(_spacePin.transform.GetGlobalPose());
        ShowHighlightProxy(true);
        Debug.Log($"_lastLockedPose={_lastLockedPose}");
    }

    #region QR CODES

    internal void UpdateByQR(QRCode qrCode)
    {
        if (!UpdateByQRHelper(qrCode, out Pose frozenPose)) return;

        UpdatePose(frozenPose);
    }

    internal void UpdateByQRJustAdded(QRCode qrCode)
    {
        if (!UpdateByQRHelper(qrCode, out Pose frozenPose)) return;

        WaitThenUpdateQR(frozenPose);
    }

    private bool UpdateByQRHelper(QRCode qrCode, out Pose frozenPose)
    {
        _coordinateSystem.SpatialNodeId = qrCode.SpatialGraphNodeId;
        _sizeMeters = qrCode.PhysicalSideLength;
        if (!_coordinateSystem.ComputePose(out Pose spongyPose))
        {
            frozenPose = new Pose();
            return false;
        }
        frozenPose = WorldLockingManager.GetInstance().FrozenFromSpongy.Multiply(spongyPose);
        return true;
    }

    private void WaitThenUpdateQR(Pose frozenPose)
    {
        if (m_myCoroutineRef != null)
        {
            StopCoroutine(m_myCoroutineRef);
            m_myCoroutineRef = null;
        }
        m_myCoroutineRef = StartCoroutine(UpdatePoseAfterWait(frozenPose));
    }


    private IEnumerator UpdatePoseAfterWait(Pose frozenPose)
    {
        yield return null; // wait for start
        yield return null; // wait for start

        ResetModelingPoseAtOrigin();

        UpdatePose(frozenPose);
    }

    private void UpdatePose(Pose frozenPose)
    {
        Pose lockedPose = WorldLockingManager.GetInstance().LockedFromFrozen.Multiply(frozenPose);
        if (NeedCommit(lockedPose))
        {
            Debug.Log($"NeedCommit={lockedPose}");
            _spacePin.SetFrozenPose(frozenPose);

            ShowHighlightProxy(true);

            _lastLockedPose = lockedPose;

            _positionMarkerHelper.SetGlobalPose(_spacePin.transform.GetGlobalPose());
        }
        _doAfterScan = DoAfterScan.Nothing;
    }

    private bool NeedCommit(Pose lockedPose)
    {
        if (_doAfterScan == DoAfterScan.UpdateMarker) return true;
        if (_doAfterScan == DoAfterScan.UpdateWorld) return true;
        if (!IsSpacePinActive()) return true;
        float RefreshThreshold = 0.01f; // one cm?
        float distance = Vector3.Distance(lockedPose.position, _lastLockedPose.position);
        if (distance > RefreshThreshold)
        {
            return true;
        }
        return false;
    }

    #endregion

    #region JSON
    internal void UpdateByJSON(MarkerData data)
    {
        _doAfterScan = DoAfterScan.Nothing;
        MoveDummyByJSON(data);

        _spacePin.transform.SetGlobalPose(_positionMarkerHelper.GetGlobalPose());
        _spacePin.ResetModelingPose();
    }


    private void MoveDummyByJSON(MarkerData data)
    {
        Vector3 position = data.Position;
        Quaternion rotation = data.Rotation;

        // Debug.Log("position and scale : " + position.ToString() + " " + scale);

        _positionMarkerHelper.MoveLocalPose(position, rotation);

    }

    internal MarkerData GetMarkerData()
    {
        return new MarkerData(_isQR, _id, _spacePin.transform.localPosition, _spacePin.transform.localRotation, _sizeMeters);
    }

    #endregion

    internal bool AddToAlignSubTree(ref AlignSubtree alignSubtree)
    {
        return alignSubtree.AddOwnedPin(_spacePin);
    }
    internal bool RemoveToAlignSubTree(ref AlignSubtree alignSubtree)
    {
        return alignSubtree.RemoveOwnedPin(_spacePin);
    }
    private void ResetModelingPoseAtOrigin()
    {
        Vector3 localPos = _spacePin.transform.localPosition;
        Quaternion localRot = _spacePin.transform.localRotation;
        Transform parent = transform;

        _spacePin.transform.SetParent(null);
        _spacePin.transform.position = localPos;
        _spacePin.transform.rotation = localRot;

        _spacePin.ResetModelingPose();

        _spacePin.transform.SetParent(parent);
        _spacePin.transform.localPosition = localPos;
        _spacePin.transform.localRotation = localRot;
    }




    private void ShowHighlightProxy(bool arg0)
    {
        HighlightProxy.SetActive(arg0);
        UpdatePoseButton.SetActive(arg0);
        UpdateWorldButton.SetActive(arg0);
        DeleteButton.SetActive(arg0);
        if (arg0)
        {
            if (_isQR)
            {
                Vector3 scale = new Vector3(_sizeMeters, _sizeMeters, _sizeMeters * 0.1f);
                Vector3 offset = scale * 0.5f;
                HighlightProxy.transform.localScale = scale;
                HighlightProxy.transform.localPosition = offset;

                MoveButton(UpdatePoseButton, new Vector3(offset.x - 0.04f, offset.y, offset.z + 0.0634f), Quaternion.Euler(180, 0, 0));
                MoveButton(UpdateWorldButton, new Vector3(offset.x, offset.y, offset.z + 0.0634f), Quaternion.Euler(180, 0, 0));
                MoveButton(DeleteButton, new Vector3(offset.x + 0.04f, offset.y, offset.z + 0.0634f), Quaternion.Euler(180, 0, 0));

                DebugShowPositions tmpDebugShowPositions = GetComponentInChildren<DebugShowPositions>();
                if (tmpDebugShowPositions != null)
                    tmpDebugShowPositions.transform.localRotation = Quaternion.Euler(180, 0, 0);
            }
            else
            {
                Vector3 scaleInCm = VirtualMarker.transform.localScale * _sizeMeters;
                scaleInCm = new Vector3(scaleInCm.x, scaleInCm.y, 0.01f);
                HighlightProxy.transform.localScale = scaleInCm;
                HighlightProxy.transform.localPosition = new Vector3(0, 0, 0);

                MoveButton(UpdatePoseButton, new Vector3(-0.04f, 0, -0.0634f), Quaternion.identity);
                MoveButton(UpdateWorldButton, new Vector3(0, 0, -0.0634f), Quaternion.identity);
                MoveButton(DeleteButton, new Vector3(0.04f, 0, -0.0634f), Quaternion.identity);

                var collider = _spacePin.transform.GetComponent<BoxCollider>();
                if (collider != null)
                {
                    collider.center = Vector3.zero;
                    collider.size = scaleInCm;
                }
            }
        }
    }

    private void MoveButton(GameObject button, Vector3 position, Quaternion rotation)
    {
        button.transform.localPosition = position;
        button.transform.localRotation = rotation;

    }

    #region CALLED_BY_BUTTONS
    public void DeleteThisMarker()
    {
        if (_myManager != null)
            _myManager.RemoveAMarker(this);
    }
    public void UpdateMarkerPose() // only move the marker, don't move the virtual world with the new scan
    {
        if (_spacePin != null)
            _spacePin.Reset();
        _doAfterScan = DoAfterScan.UpdateMarker;
    }

    public void UpdateWorldWithMarker() // move the virtual world with the new scan
    {
        if (_spacePin != null)
            _spacePin.Reset();
        _doAfterScan = DoAfterScan.UpdateWorld;

    }
    #endregion
}

