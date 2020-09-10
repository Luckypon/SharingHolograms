using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.UI
{
    [Serializable]
    public class HologramTypeChoiceButton
    {
        public HologramType MyType;
        public Interactable ToggleButton;
    }

    public class OnToggleCreationClick : MonoBehaviour
    {
        public HologramTypeSwitch MyHologramTypeSwitch;
        public UserInteractions MyUserInteractions;
        public GameObject ChoiceButtonsParent;
        public Interactable ToggleModeButton;
        public List<HologramTypeChoiceButton> Buttons = new List<HologramTypeChoiceButton>();

        private bool _isOn;
        private bool _refuseEvent;

        private void Awake()
        {
            _isOn = false;
            ShowButton(_isOn);
            SetOnlyOneToggle(HologramType.Simple);
        }
        internal void TriggerOnClick()
        {
            ToggleModeButton.TriggerOnClick();
        }

        private void ShowButton(bool isOn)
        {
            ChoiceButtonsParent.SetActive(isOn);
        }

        private void SetChoice(HologramType type)
        {
            MyHologramTypeSwitch.SetCurrentType(type);
            MyUserInteractions.ChoiceChange();
            SetOnlyOneToggle(type);
        }

        private bool IsToggle(HologramType type)
        {
            HologramTypeChoiceButton button = GetChoice(type);
            if (button == null) return false;
            return button.ToggleButton.IsToggled;
        }

        private HologramTypeChoiceButton GetChoice(HologramType type)
        {
            HologramTypeChoiceButton button = Buttons.FirstOrDefault(v => v.MyType == type);
            if (button == null)
            {
                Debug.LogError("Missing HologramTypeChoiceButton");
                return null;
            }
            return button;
        }

        private void SetOnlyOneToggle(HologramType type)
        {
            _refuseEvent = true;
            foreach (var button in Buttons)
            {
                if (button.MyType == type)
                {
                    button.ToggleButton.IsToggled = true;
                }
                else
                {
                    button.ToggleButton.IsToggled = false;
                }
            }
            _refuseEvent = false;
        }
        #region CALLED_BY_BUTTONS

        public void ToggleMode()
        {
            _isOn = !_isOn;

            ShowButton(_isOn);

            MyUserInteractions.ToggleMode(_isOn);
        }


        public void SetSimpleChoice()
        {
            if (_refuseEvent) return;
            if (!IsToggle(HologramType.Simple)) return; // can not desactive
            SetChoice(HologramType.Simple);
        }

       

        #endregion
    }
}
