using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class HologramTypeChoice
{
    public HologramType MyType;
    public GameObject HologramPrefab;
    public GameObject HologramDummy;
}

public class HologramTypeSwitch : MonoBehaviour
{
    public List<HologramTypeChoice> Choices = new List<HologramTypeChoice>();

    private HologramType _currentType;
    private HologramTypeChoice _currentChoice;

    private void Awake()
    {
        SetCurrentChoice();
    }

    internal GameObject GetCurrentPrefab()
    {
        SetCurrentChoice();

        return _currentChoice?.HologramPrefab;
    }

    internal GameObject GetSpecificPrefab(HologramType type)
    {
        return GetChoice(type)?.HologramPrefab;
    }

    internal GameObject GetCurrentDummy()
    {
        SetCurrentChoice();

        return _currentChoice?.HologramDummy;
    }


    private void SetCurrentChoice()
    {
        _currentChoice = GetChoice(_currentType);
    }

    internal void SetCurrentType(HologramType type)
    {
        _currentType = type;
    }

    private HologramTypeChoice GetChoice(HologramType type)
    {
        HologramTypeChoice choice = Choices.FirstOrDefault(v => v.MyType == type);
        if (choice == null)
        {
            Debug.LogError("Missing HologramTypeChoice");
            return null;
        }
        return choice;
    }
}
