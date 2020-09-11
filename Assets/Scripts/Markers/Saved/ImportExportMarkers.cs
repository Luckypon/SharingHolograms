using System;
using System.Collections.Generic;
using UnityEngine;

public class ImportExportMarkers : ImportExportJSON<MarkerData>
{
    public MarkersSpacePinsManager MyMarkersManager;


    protected override void ActionWhenImportFail()
    {
        // TODO : remove and replace, it's only for testing
        // InformationText.Show("Missing data. Contact an admin.");
        TryImportWithDummyData();
    }
    private void TryImportWithDummyData()
    {
        List<MarkerData> markersList = new List<MarkerData>
        {
            new MarkerData(true, "AQRCode_1", new Vector3(0, 0, 0), Quaternion.Euler(180,0,0)), // QR is inversed
            new MarkerData(true, "AQRCode_2", new Vector3(2.67f, 0, 0), Quaternion.Euler(180,0,0)) // QR is inversed
        };

        MyMarkersManager.AddOrUpdateMarkersByJSON(markersList);
    }


    protected override string GetImportPathEnd()
    {
        return "/markers.json";
    }
    protected override string GetExportPathEnd()
    {
        return "/markers.json";
    }

    protected override void DoActionWithObtainedData(List<MarkerData> dataList)
    {
        MyMarkersManager.AddOrUpdateMarkersByJSON(dataList);
    }

    protected override MarkerData[] GetArrayOfData()
    {
        return MyMarkersManager.GetMarkersForJSON()?.ToArray();
    }

    internal void ClearFileAndImportAgain()
    {
        DestroyFile();
        TryImportWithDummyData();
    }
}

