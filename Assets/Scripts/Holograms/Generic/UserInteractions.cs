using Assets.Scripts.UI;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

public class UserInteractions : MonoBehaviour
{
    public HologramCreation MyHologramCreation;
    public HologramsManager MyHologramsManager;
    public HologramTypeSwitch MyHologramTypeSwitch;
    public OnToggleCreationClick MyOnToggleCreationClick;
    public GameObject FollowIndexGameObject;
    public Transform DummyInAlignSubTree;

    [InspectorButton("BeginCreation", ButtonWidth = 150)]
    public bool Begin;
    [InspectorButton("EndCreation", ButtonWidth = 150)]
    public bool End;
    [InspectorButton("CreateNewHologram", ButtonWidth = 150)]
    public bool CreateHologram;
    [InspectorButton("DeleteSelectedHologram", ButtonWidth = 150)]
    public bool DeleteHologram;

    private bool _isCreating;
    private Transform _dummyParent;
    private GameObject _currentHologramDummy;

    private void Awake()
    {
        FollowIndex(false);
        _dummyParent = DummyInAlignSubTree.parent;
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButton(1))
        {
            CreateNewHologram();
        }
        if (Input.GetMouseButton(2))
        {
            DeleteSelectedHologram();
        }
#endif
    }


    private void BeginCreation()
    {
        FollowIndex(true);
    }

    private void EndCreation()
    {
        FollowIndex(false);
    }

    private void FollowIndex(bool arg0)
    {
        if (arg0)
        {
            ShowCurrentHologramDummy();
        }

        _isCreating = arg0;
        FollowIndexGameObject.SetActive(arg0);
    }

    private void ShowCurrentHologramDummy()
    {
        if (_currentHologramDummy != null)
        {
            Destroy(_currentHologramDummy);
        }
        GameObject dummyPrefab = MyHologramTypeSwitch.GetCurrentDummy();
        if (dummyPrefab == null)
        {
            return;
        }
        _currentHologramDummy = GameObject.Instantiate(dummyPrefab);
        _currentHologramDummy.transform.SetParent(FollowIndexGameObject.transform);
        _currentHologramDummy.transform.localPosition = Vector3.zero;
        _currentHologramDummy.transform.localRotation = Quaternion.identity;
        _currentHologramDummy.transform.localScale = Vector3.one;
    }

    private void CreateNewHologram()
    {
        if (!_isCreating) return;

        DummyInAlignSubTree.SetParent(FollowIndexGameObject.transform);
        DummyInAlignSubTree.localPosition = Vector3.zero;
        DummyInAlignSubTree.localRotation = Quaternion.identity;
        DummyInAlignSubTree.SetParent(_dummyParent);

        MyHologramCreation.UserCreateNewHologram(DummyInAlignSubTree.transform.position, DummyInAlignSubTree.transform.rotation);
        MyOnToggleCreationClick.TriggerOnClick();
    }

    private void DeleteSelectedHologram()
    {
        if (_isCreating) return;
        HologramManager hologramToDelete = MyHologramsManager.GetSelectedHologram();
        if (hologramToDelete == null) return;
        MyHologramCreation.UserDeleteHologram(hologramToDelete.gameObject);
    }
    internal void ToggleMode(bool toggle)
    {
        if (toggle)
        {
            BeginCreation();
        }
        else
        {
            EndCreation();
        }
    }

    #region CALLED_BY_BUTTONS


    public void Create()
    {
        CreateNewHologram();
    }

    public void Delete()
    {
        DeleteSelectedHologram();
    }

    public void ChoiceChange()
    {
        ShowCurrentHologramDummy();
    }

    #endregion
}

