public class SimpleHologramManager : HologramManager
{
    #region HOLOGRAM DATA

    internal override HologramData GetHologramData()
    {
        if (string.IsNullOrWhiteSpace(Id) || Hologram == null)
        {
            return null;
        }
        return HologramDataWrapper.CreateSimpleHologramData(Id, Hologram.transform.localPosition, Hologram.transform.localRotation, Hologram.transform.localScale);
    }

   

    internal override void ResetHologram()
    {
        base.ResetHologram();
        _isHologramGrabbed = false;
    }


    #endregion
 

}
