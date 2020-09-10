using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

public class HologramChangesEvent : MonoBehaviour
{
    public HologramManager MyHologramManager;
    public ObjectManipulator MyObjectManipulator;
    public BoundingBox MyBoundingBox;

    private bool _isGrabbed;

    private void Awake()
    {
        MyObjectManipulator.OnManipulationStarted.AddListener(OnManipulationStarted);
        MyObjectManipulator.OnManipulationEnded.AddListener(OnManipulationEnded);
        MyBoundingBox.RotateStopped.AddListener(ChangesDone);
        MyBoundingBox.ScaleStopped.AddListener(ChangesDone);
    }
    private void OnDestroy()
    {
        MyObjectManipulator.OnManipulationStarted.RemoveListener(OnManipulationStarted);
        MyObjectManipulator.OnManipulationEnded.RemoveListener(OnManipulationEnded);
        MyBoundingBox.RotateStopped.RemoveListener(ChangesDone);
        MyBoundingBox.ScaleStopped.RemoveListener(ChangesDone);
    }


    private void OnManipulationStarted(ManipulationEventData arg0)
    {
        _isGrabbed = true;
        HandleManipulation();
    }
    private void OnManipulationEnded(ManipulationEventData arg0)
    {
        _isGrabbed = false;
        HandleManipulation();
    }

    private void HandleManipulation()
    {
        MyHologramManager.HologramIsGrabbed(_isGrabbed);
        ChangesDone();
    }

    private void ChangesDone()
    {
        MyHologramManager.ChangesDone();
    }


}
