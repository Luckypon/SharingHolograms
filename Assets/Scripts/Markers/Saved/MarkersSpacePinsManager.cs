using Microsoft.MixedReality.QR;
using Microsoft.MixedReality.WorldLocking.Core;
using Microsoft.MixedReality.WorldLocking.Samples.Advanced.QRSpacePins;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MarkersSpacePinsManager : AMarkersManager // TODO : refactoriser, pour séparer dans plusieurs classes codes QR, images et database
{
    public bool AddUnknownQrCode = true;
    public GameObject MarkerPrefab;
    public GameObject SceneToShow;
    public GameObject MarkersParent;
    public AlignSubtree MyAlignSubtree;

    public PositionMarkerHelper MyPositionMarkerHelper;

    public ImportExportMarkers MyImportExportMarkers;
    //QRCODES
    private QRSpatialCoord _tmpCoordinateSystem = null;

    //The virtual poses in the scene to be matched with the poses of the markers in the physical world.
    [SerializeField]
    private List<MarkerSpacePinManager> _markers = new List<MarkerSpacePinManager>();
    private void Awake()
    {
        _tmpCoordinateSystem = new QRSpatialCoord();
    }
    private void OnEnable()
    {
        ShowHolograms();
    }

    // reset the view, but the markers are still saved
    public void ResetAllMarkers() // call from button
    {
        if (_markers.Count > 0)
        {
            for (int i = 0; i < _markers.Count; i++)
            {
                _markers[i].ResetSpacePin();
            }
        }
        ShowHolograms();
    }

    internal void RemoveAMarker(MarkerSpacePinManager marker)
    {
        if (marker == null) return;

        marker.ResetSpacePin();
        marker.RemoveToAlignSubTree(ref MyAlignSubtree);
        if (_markers.Contains(marker))
            _markers.Remove(marker);

        Destroy(marker.gameObject);
        ShowHolograms();

        MyImportExportMarkers.Export();
    }

    public void DeleteAllAddedMarkers() // call from button
    {
        ResetAllMarkers();

        for (int i = _markers.Count - 1; i >= 0; i--)
        {
            Destroy(_markers[i].gameObject);
        }
        _markers.Clear();

        MyAlignSubtree.ClearOwnedPins();
        MyAlignSubtree.ClaimPinOwnership();

        MyImportExportMarkers.ClearFileAndImportAgain();
    }

    #region QR Codes (virtual will move to fit physical)
    internal override void UpdateQRCode(QRCode qrCode)
    {
        MarkerSpacePinManager currentMarker = GetMarkerByQRCode(qrCode);
        if (currentMarker == null)
        {
            if (!AddUnknownQrCode || !AtLeastOneMarkerDetected())
            {
                Debug.Log("You must first scan a marker that is known in the database.");
                return;
            }
            var markerData = GetMarkerDataByQrCode(qrCode);
            currentMarker = CreateNewVirtualMarkerByScan(markerData);

            if (currentMarker != null)
            {
                currentMarker.UpdateByQRJustAdded(qrCode);
                MyImportExportMarkers.Export();
            }
        }
        else
        {
            currentMarker.UpdateByQR(qrCode);
        }

        ShowHolograms();
    }

    internal override void ResetQRCode(QRCode qrCode)
    {
        MarkerSpacePinManager currentMarker = GetMarkerByQRCode(qrCode);
        if (currentMarker == null)
        {
            //Debug.Log(qrCode.Data + " is not registered. TODO : add a qr code in json");
        }
        else
        {
            currentMarker.ResetSpacePin();
        }
        ShowHolograms();
    }

    private MarkerSpacePinManager GetMarkerByQRCode(QRCode qrCode)
    {
        string data = qrCode.Data;

        if (string.IsNullOrWhiteSpace(data))
            return null;

        if (_markers.Count == 0)
        {
            // TODO : msg to contact admin
            Debug.Log("No registered markers");
            return null;
        }

        return _markers.FirstOrDefault(v => v.Id == data);
    }

    private MarkerData GetMarkerDataByQrCode(QRCode qrCode)
    {
        _tmpCoordinateSystem.SpatialNodeId = qrCode.SpatialGraphNodeId;

        if (_tmpCoordinateSystem.ComputePose(out Pose spongyPose))
        {
            //Pose frozenPose = WorldLockingManager.GetInstance().FrozenFromSpongy
            MyPositionMarkerHelper.SetGlobalPose(spongyPose);

            Pose localPose = MyPositionMarkerHelper.GetLocalPose();

            return new MarkerData(
                true,
                qrCode.Data,
                localPose.position,
                localPose.rotation,
                qrCode.PhysicalSideLength
                );

        }
        return null;


    }

    #endregion

    #region JSON (set up virtual markers)

    internal List<MarkerData> GetMarkersForJSON()
    {
        List<MarkerData> list = new List<MarkerData>();

        foreach (var marker in _markers)
        {
            MarkerData markerData = marker.GetMarkerData();
            if (markerData == null)
            {
                continue;
            }
            list.Add(markerData);
        }

        return list;
    }
    internal void AddOrUpdateMarkersByJSON(List<MarkerData> markersJSON)
    {
        Debug.Log("JSON add/update " + markersJSON.Count + "markers");

        foreach (var markerData in markersJSON)
        {
            Debug.Log($"Marker={markerData.Id}: position={markerData.Position}, rotationEuler={markerData.Rotation.eulerAngles}");
            var markerInList = _markers.FirstOrDefault(i => i.Id == markerData.Id);
            if (markerInList == null)
            {
                markerInList = CreateIdleMarker(markerData);
            }
            markerInList.UpdateByJSON(markerData);
        }
        SetUpAlignSubTree();
        ResetAllMarkers();
    }

    #endregion




    private MarkerSpacePinManager CreateNewVirtualMarkerByScan(MarkerData markerData)
    {
        if (markerData == null) return null;

        MarkerSpacePinManager newMarker = CreateIdleMarker(markerData);
        newMarker.UpdateByJSON(markerData);

        newMarker.AddToAlignSubTree(ref MyAlignSubtree);
        MyAlignSubtree.ClaimPinOwnership();

        newMarker.ResetSpacePin();
        return newMarker;
    }

    private MarkerSpacePinManager CreateIdleMarker(MarkerData markerData)
    {
        GameObject marker = GameObject.Instantiate(MarkerPrefab, Vector3.zero, Quaternion.identity);
        marker.transform.SetParent(MarkersParent.transform, true);
        marker.transform.localPosition = Vector3.zero;
        marker.transform.localRotation = Quaternion.identity;

        MarkerSpacePinManager currentMarker = marker.GetComponent<MarkerSpacePinManager>();
        currentMarker.Create(markerData.Id, markerData.IsQR, this, markerData.SizeMeters);
        _markers.Add(currentMarker);
        return currentMarker;
    }


    private void SetUpAlignSubTree()
    {
        MyAlignSubtree.ClearOwnedPins();
        int added = 0;
        foreach (var marker in _markers)
        {
            bool addsuccess = marker.AddToAlignSubTree(ref MyAlignSubtree);
            if (addsuccess) added++;
        }
        if (added > 0)
        {
            MyAlignSubtree.ClaimPinOwnership();
            MyAlignSubtree.Load();
        }

    }

    private void ShowHolograms()
    {
        SceneToShow.SetActive(AtLeastOneMarkerDetected());
    }

    private bool AtLeastOneMarkerDetected()
    {
        return _markers.Where(v => v.IsDetectedInWorld).Count() > 0;
    }


}

