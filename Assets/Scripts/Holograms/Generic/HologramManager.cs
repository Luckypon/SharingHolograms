using UnityEngine;

public abstract class HologramManager : MonoBehaviour
{
    public GameObject Hologram;

    internal HologramsManager MyManager;

    protected string _id;
    protected bool _isHologramGrabbed;

    public string Id { get => _id; set => _id = value; }
    public bool IsHologramGrabbed { get => _isHologramGrabbed; }

    internal void UserAddHologram(Vector3 worldPosition, Quaternion worldRotation)
    {
        _id = string.Empty;
        ResetHologram();
        MyManager.AddHologramToList(this);
        Hologram.name = _id;
        Hologram.transform.position = worldPosition;
        Hologram.transform.rotation = worldRotation;

        ChangesDone();
    }

    internal void UserRemoveHologram()
    {
        _id = string.Empty;
        ResetHologram();
        MyManager.RemoveHologramFromList(this);
        ChangesDone();
    }
    internal void CreateHologramFromData(HologramData hologramData)
    {
        _id = hologramData.Id;
        Hologram.name = _id;
        ResetHologram();
        MyManager.AddHologramToList(this);

        UpdateHologramFromData(hologramData);
    }


    #region HOLOGRAM DATA

    internal abstract HologramData GetHologramData();

    internal virtual void UpdateHologramFromData(HologramData data)
    {
        Hologram.transform.localPosition = data.Position;
        Hologram.transform.localRotation = data.Rotation;
        Hologram.transform.localScale = data.Scale;
    }

    internal virtual void ResetHologram()
    {
        Hologram.transform.localPosition = Vector3.zero;
        Hologram.transform.localRotation = Quaternion.identity;
        Hologram.transform.localScale = Vector3.one;
        _isHologramGrabbed = false;
    }

    internal void ChangesDone()
    {
        MyManager.ChangesDone();
    }

    internal void HologramIsGrabbed(bool isGrabbed)
    {
        _isHologramGrabbed = isGrabbed;
    }

    #endregion

}
