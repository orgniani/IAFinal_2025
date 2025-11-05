using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UI.Menus;

namespace UI.Buttons
{
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(Image))]
    public sealed class UIButtonController : MonoBehaviour
    {
        [SerializeField] private TMP_Text buttonText;
        [SerializeField] private Image buttonIcon;

        private Button _button;
        private Image _image;
        private UIButtonConfig _config;

        public event Action<UIButtonAction, UIMenuID?> OnClick;

        private void Reset()
        {
            GameObject child;

            if (transform.childCount < 1)
            {
                child = new GameObject("Text (TMP)");
                child.transform.SetParent(transform);
            }

            else child = transform.GetChild(0).gameObject;

            if (!child.TryGetComponent<TMP_Text>(out buttonText))
            {
                buttonText = child.AddComponent<TextMeshProUGUI>();
            }

            _button = GetComponent<Button>();
            _image = GetComponent<Image>();
        }

        private void Awake()
        {
            buttonText ??= GetComponent<TMP_Text>();
            _button ??= GetComponent<Button>();
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(HandleButtonClick);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(HandleButtonClick);
        }

        public void Setup(UIButtonConfig config)
        {
            _config = config;

            _button ??= GetComponent<Button>();
            _image ??= GetComponent<Image>();
            buttonText.SetText(config.Text);

            _image.color = config.Tint;

            if (_image) _image.sprite = config.Sprite;
            if (buttonIcon)
            {
                buttonIcon.sprite = config.Icon;
                buttonIcon.gameObject.SetActive(config.Icon != null);
            }
        }

        public void SetTint(Color tint)
        {
            if (_image != null)
                _image.color = tint;
        }

        public void SetText(string newText)
        {
            buttonText.SetText(newText);
        }

        private void HandleButtonClick()
        {
            var role = _config.MenuSource?.MenuID;
            OnClick?.Invoke(_config.Action, role);
        }
    }
}