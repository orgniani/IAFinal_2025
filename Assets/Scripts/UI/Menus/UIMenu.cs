using System;
using System.Collections.Generic;
using UnityEngine;
using UI.Buttons;

namespace UI.Menus
{
    public class UIMenu : MonoBehaviour, IMenu
    {
        [Header("References")]
        [SerializeField] private UIMenuSource selfSource;

        [Header("Buttons")]
        [SerializeField] private UIButtonController buttonPrefab;
        [SerializeField] private List<ButtonGroup> buttons = new();

        private readonly Dictionary<UIButtonConfig, UIButtonController> _buttonInstances = new();

        public event Action<UIButtonAction, UIMenuID?> OnButtonAction;

        public UIMenuID MenuID => selfSource?.MenuID ?? UIMenuID.Main;
        public UIMenuType MenuType => selfSource?.MenuType ?? UIMenuType.Full;
        public bool SupportsBackNavigation => selfSource?.SupportsBackNavigation ?? false;
        public bool IsVisible => gameObject.activeSelf;

        public void Setup()
        {
            foreach (var group in buttons)
            {
                if (group.ButtonParent == null) continue;

                foreach (var config in group.ButtonConfigs)
                {
                    if (config == null) continue;

                    var btn = Instantiate(buttonPrefab, group.ButtonParent);
                    btn.Setup(config);

                    btn.OnClick += (action, menuID) => OnButtonAction?.Invoke(action, menuID);

                    _buttonInstances[config] = btn;
                }
            }
        }


        public void ToggleVisibility(bool isVisible) => gameObject.SetActive(isVisible);

        [Serializable]
        public struct ButtonGroup
        {
            [field: SerializeField] public List<UIButtonConfig> ButtonConfigs { get; set; }
            [field: SerializeField] public Transform ButtonParent { get; set; }
        }
    }
}