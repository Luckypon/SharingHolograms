using System;
using System.Collections.Generic;
using UnityEngine;

public static class JsonSerialiserService
{
    public static string Serialyse<T>(T objectToSave, bool prettyPrint = false)
    {
        return JsonUtility.ToJson(objectToSave, prettyPrint);
    }

    public static T Deserialyse<T>(string savedObjectValue)
    {
        try
        {
            T content = JsonUtility.FromJson<T>(savedObjectValue);
            return content;
        }
        catch (Exception e)
        {
            return default;
        }
    }


    public static T[] DeserialyseArray<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper?.Items;
    }

    public static string SerialyseArray<T>(T[] array, bool prettyPrint = false)
    {
        Wrapper<T> wrapper = new Wrapper<T>
        {
            Items = array
        };
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }
}

[Serializable]
public class Wrapper<T>
{
    public Wrapper() { }

    public Wrapper(List<T> items)
    {
        Items = items.ToArray();
    }
    public T[] Items;
}

