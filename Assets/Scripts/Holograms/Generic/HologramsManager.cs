using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HologramsManager : MonoBehaviour
{
    public HologramCreation MyHologramCreation;
    public HologramsChanges MyHologramsChanges;

    private List<HologramManager> _holograms = new List<HologramManager>();
    private readonly string _rootId = "HologramId";
    private int _currentNb = 0;



    internal void AddHologramToList(HologramManager hologramManager)
    {
        if (hologramManager == null) return;
        if (_holograms.Contains(hologramManager)) return;
        _holograms.Add(hologramManager);

        if (string.IsNullOrWhiteSpace(hologramManager.Id))
        {
            hologramManager.Id = GenerateHologramId();
        }
    }


    internal void RemoveHologramFromList(HologramManager hologramManager)
    {
        if (hologramManager == null) return;
        if (!_holograms.Contains(hologramManager)) return;
        _holograms.Remove(hologramManager);
    }


    internal List<HologramData> GetHologramsData()
    {
        _holograms.RemoveAll(i => i == null);

        List<HologramData> list = new List<HologramData>();

        foreach (var hologram in _holograms)
        {
            HologramData hologramData = hologram.GetHologramData();
            if (hologramData == null)
            {
                Debug.Log("Id or hologram is null");
                continue;
            }
            list.Add(hologramData);
        }

        return list;
    }



    internal void CreateOrUpdateHologramsFromJSON(List<HologramData> hologramsList)
    {
        _holograms.RemoveAll(i => i == null);

        for (int index = hologramsList.Count() - 1; index >= 0; index--)
        {
            HologramData hologramData = hologramsList[index];
            var hologramInList = _holograms.FirstOrDefault(i => i.Id == hologramData.Id);
            if (hologramInList == null)
            {
                hologramInList = CreateNewHologramFromJSON(hologramData);
                hologramInList.CreateHologramFromData(hologramData);
            }
            else
            {
                hologramInList.UpdateHologramFromData(hologramData);
            }
        }
    }



    private HologramManager CreateNewHologramFromJSON(HologramData hologramData)
    {
        GameObject hologram = MyHologramCreation.CreateNewHologramFromBatch(hologramData.Type);
        HologramManager hologramManager = hologram.GetComponent<HologramManager>();
        hologramManager.MyManager = this;
        return hologramManager;
    }


    internal HologramManager GetSelectedHologram()
    {
        for (int i = _holograms.Count - 1; i >= 0; i--)
        {
            HologramManager hologramManager = _holograms[i];
            if (!hologramManager.IsHologramGrabbed)
                continue;

            return hologramManager;
        }
        return null;
    }


    private string GenerateHologramId()
    {
        Guid guid = System.Guid.NewGuid();
        string newId = guid + _rootId + _currentNb;
        _currentNb++;

        return newId;
    }


    internal void ChangesDone()
    {
        MyHologramsChanges.ChangesDone();
    }

}

