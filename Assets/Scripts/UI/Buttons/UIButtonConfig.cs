using UnityEngine;
using UI.Menus;

namespace UI.Buttons
{
    [CreateAssetMenu(menuName = "Config/Button", fileName = "BtnCfg", order = 0)]
    public class UIButtonConfig : ScriptableObject
    {
        [field: SerializeField] public UIMenuSource MenuSource { get; private set; }

        [field: SerializeField] public UIButtonAction Action { get; private set; }

        [field: SerializeField] public string Text { get; private set; }

        [field: SerializeField] public Sprite Sprite { get; set; }

        [field: SerializeField] public Sprite Icon { get; set; }

        [field: SerializeField] public Color Tint { get; set; } = Color.white;
    }
}