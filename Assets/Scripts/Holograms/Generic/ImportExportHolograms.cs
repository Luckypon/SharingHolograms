using System.Collections.Generic;
using UnityEngine;

public class ImportExportHolograms : ImportExportJSON<HologramData>
{
    public HologramsManager MyHologramsManager;

    internal void HologramHasChanged()
    {
        Export();
    }


    protected override void ActionWhenImportFail()
    {
        TryImportWithDummyData();
    }

    private void TryImportWithDummyData()
    {
        List<HologramData> dataList = new List<HologramData>();

        HologramData holo1 = HologramDataWrapper.CreateSimpleHologramData(
           "id1",
           new Vector3(0.2f, 0, -0.1f),
           new Quaternion(0, 1, 0, 0),
           Vector3.one);

        dataList.Add(holo1);

        MyHologramsManager.CreateOrUpdateHologramsFromJSON(dataList);
    }

    protected override string GetImportPathEnd()
    {
        return "/holograms.json";
    }
    protected override string GetExportPathEnd()
    {
        return "/holograms.json";
    }

    protected override void DoActionWithObtainedData(List<HologramData> dataList)
    {
        MyHologramsManager.CreateOrUpdateHologramsFromJSON(dataList);
    }

    protected override HologramData[] GetArrayOfData()
    {
        return MyHologramsManager.GetHologramsData()?.ToArray();
    }
}
