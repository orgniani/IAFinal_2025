using System;

namespace UI.Menus
{
    public interface IMenu
    {
        void Setup();
        void ToggleVisibility(bool isVisible);
        bool IsVisible { get; }
        bool SupportsBackNavigation { get; }
        UIMenuID MenuID { get; }
        UIMenuType MenuType { get; }

    }
}