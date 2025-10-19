using System;
using UI.Buttons;
using UnityEngine;

namespace UI.Menus
{
    public static class UIMenuButtonFactory
    {
        public static UIButtonController Create(UIButtonController prefab, Transform parent, UIButtonConfig config)
        {
            var instance = GameObject.Instantiate(prefab, parent);
            instance.name = $"{config.MenuSource}_Btn";
            instance.Setup(config);
            return instance;
        }
    }
}