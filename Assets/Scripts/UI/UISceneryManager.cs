using System.Collections;
using UnityEngine;
using Events;
using UnityEngine.UI;
using Helpers;

namespace UI
{
    public class UISceneryManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Canvas transitionCanvas;
        [SerializeField] private Slider loadBar;

        [Header("Parameters")]
        [Tooltip("Delay before hiding the transition canvas after scene load ends.")]
        [SerializeField] private float waitToDisableScreen = 1.5f;

        [Tooltip("Trigger name used to play the 'close' animation on the transition canvas.")]
        [SerializeField] private string closeAnimationTrigger = "close";

        [Tooltip("How long it takes for the load bar to reach the new value.")]
        [SerializeField] private float fillDuration = 0.5f;

        [Header("Subscribe to events")]
        [SerializeField] private EmptyAction onLoadStart;
        [SerializeField] private FloatAction onLoadProgress;
        [SerializeField] private EmptyAction onLoadEnd;

        private Animator _canvasAnimator;

        private Coroutine _fillCoroutine;
        private Coroutine _disablePopUpCoroutine;

        private void Awake()
        {
            _canvasAnimator = transitionCanvas.GetComponent<Animator>();          
            ValidateReferences();
        }

        private void OnEnable()
        {
            onLoadStart.Subscribe(EnableLoadingScreen);
            onLoadEnd.Subscribe(DisableLoadingScreen);

            onLoadProgress.Subscribe(UpdateLoadBarFill);
        }

        private void OnDisable()
        {
            onLoadStart.Unsubscribe(EnableLoadingScreen);
            onLoadEnd.Unsubscribe(DisableLoadingScreen);

            onLoadProgress.Unsubscribe(UpdateLoadBarFill);
        }

        private void EnableLoadingScreen()
        {
            loadBar.value = 0f;

            if (_disablePopUpCoroutine != null)
            {
                StopCoroutine(_disablePopUpCoroutine);
                _disablePopUpCoroutine = null;

                _canvasAnimator.ResetTrigger(closeAnimationTrigger);
                transitionCanvas.gameObject.SetActive(false);
            }

            transitionCanvas.gameObject.SetActive(true);
        }

        private void DisableLoadingScreen()
        {
            if(_disablePopUpCoroutine != null) return;
            _disablePopUpCoroutine = StartCoroutine(DisablePopUp());
        }

        private IEnumerator DisablePopUp()
        {
            _canvasAnimator.SetTrigger(closeAnimationTrigger);

            yield return new WaitForSeconds(waitToDisableScreen);

            transitionCanvas.gameObject.SetActive(false);
            _disablePopUpCoroutine = null;
        }

        private void UpdateLoadBarFill(float percentage)
        {
            if (_fillCoroutine != null)
                StopCoroutine(_fillCoroutine);

            _fillCoroutine = StartCoroutine(LerpFill(loadBar.value, percentage));
        }

        private IEnumerator LerpFill(float from, float to)
        {
            float startTime = Time.time;
            float startFillAmount = loadBar.value;

            while (Time.time < startTime + fillDuration)
            {
                float t = (Time.time - startTime) / fillDuration;
                loadBar.value = Mathf.Lerp(startFillAmount, to, t);
                yield return null;
            }

            loadBar.value = to;
        }

        private void ValidateReferences()
        {
            ReferenceValidator.Validate(transitionCanvas, nameof(transitionCanvas), this);
            ReferenceValidator.Validate(loadBar, nameof(loadBar), this);
            ReferenceValidator.Validate(_canvasAnimator, nameof(_canvasAnimator), this);

            ReferenceValidator.Validate(onLoadStart, nameof(onLoadStart), this);
            ReferenceValidator.Validate(onLoadEnd, nameof(onLoadEnd), this);
            ReferenceValidator.Validate(onLoadProgress, nameof(onLoadProgress), this);
        }
    }
}