using UnityEngine;

public class HologramCreation : MonoBehaviour
{
    public HologramTypeSwitch MyHologramTypeSwitch;
    public HologramsManager MyHologramsManager;
    public Transform HologramParent;

    internal void UserCreateNewHologram(Vector3 worldPosition, Quaternion worldRotation)
    {
        GameObject hologram = CreateNewHologram(GetCurrentPrefab());
        HologramManager hologramManager = hologram.GetComponent<HologramManager>();
        hologramManager.MyManager = MyHologramsManager;
        hologramManager.UserAddHologram(worldPosition, worldRotation);
    }

    internal void UserDeleteHologram(GameObject hologram)
    {
        if (hologram == null) return;
        HologramManager hologramManager = hologram.GetComponent<HologramManager>();
        if (hologramManager == null) return;

        hologramManager.UserRemoveHologram();

        DestroyHologram(hologram);
    }

    internal GameObject CreateNewHologramFromBatch(HologramType type)
    {
        return CreateNewHologram(GetSpecificPrefab(type));

    }
    private GameObject CreateNewHologram(GameObject prefab)
    {
        if (prefab == null) return null;

        GameObject obj = GameObject.Instantiate(prefab);
        obj.transform.SetParent(HologramParent);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;
        return obj;
        //return SimplePool.Spawn(HologramPrefab, Vector3.zero, Quaternion.identity, transform);
    }

    private void DestroyHologram(GameObject obj)
    {
        Destroy(obj);
        //SimplePool.Despawn(obj);
    }

    private GameObject GetCurrentPrefab()
    {
        return MyHologramTypeSwitch.GetCurrentPrefab();
    }

    private GameObject GetSpecificPrefab(HologramType type)
    {
        return MyHologramTypeSwitch.GetSpecificPrefab(type);
    }
}
