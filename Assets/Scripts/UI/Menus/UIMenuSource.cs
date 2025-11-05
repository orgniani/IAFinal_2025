using UnityEngine;

namespace UI.Menus
{
    [CreateAssetMenu(menuName = "DataSources/Menu", fileName = "MS_")]
    public class UIMenuSource : ScriptableObject
    {
        public UIMenu DataInstance { get; set; }

        [SerializeField] private UIMenuType menuType;
        [SerializeField] private UIMenuID menuID;

        [Tooltip("Enable this if the menu should return to the previous menu when 'Back' is pressed.")]
        [SerializeField] private bool supportsBackNavigation = false;

        public UIMenuID MenuID => menuID;
        public UIMenuType MenuType => menuType;
        public bool SupportsBackNavigation => supportsBackNavigation;
    }
}