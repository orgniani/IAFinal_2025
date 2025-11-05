using UnityEngine;
using Audio;
using UnityEngine.UI;

namespace UI.Buttons
{
    [RequireComponent(typeof(Button))]
    public class UIButtonTest : MonoBehaviour
    {
        [Header("Invoke Event")]
        [SerializeField] private AudioEvent onPlayAudio;
        [SerializeField] private AudioKey buttonClickAudioKey;

        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnButtonClick);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveAllListeners();
        }

        private void OnButtonClick()
        {
            if (onPlayAudio != null)
            {
                onPlayAudio.InvokeEvent(buttonClickAudioKey);
            }
        }
    }
}