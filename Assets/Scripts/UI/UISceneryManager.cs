using System.Collections;
using UnityEngine;
using Events;

namespace UI
{
    public class UISceneryManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Canvas transitionCanvas;

        [Tooltip("Delay before hiding the transition canvas after scene load ends.")]
        [SerializeField] private float waitToDisableScreen = 1.5f;

        [Tooltip("Trigger name used to play the 'close' animation on the transition canvas.")]
        [SerializeField] private string closeAnimationTrigger = "close";

        [Header("Subscribe to events")]
        [SerializeField] private EmptyAction onLoadStart;
        [SerializeField] private EmptyAction onLoadEnd;

        private Animator _canvasAnimator;
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
        }

        private void OnDisable()
        {
            onLoadStart.Unsubscribe(EnableLoadingScreen);
            onLoadEnd.Unsubscribe(DisableLoadingScreen);
        }

        private void EnableLoadingScreen()
        {
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
        private void ValidateReferences()
        {
            if (!ValidateReference(transitionCanvas, nameof(transitionCanvas))) return;
            if (!ValidateReference(_canvasAnimator, nameof(_canvasAnimator))) return;
            if (!ValidateReference(onLoadStart, nameof(onLoadStart))) return;
            if (!ValidateReference(onLoadEnd, nameof(onLoadEnd))) return;
        }

        private bool ValidateReference(Object reference, string referenceName)
        {
            if (reference != null) return true;

            Debug.LogError($"{name}: {referenceName} is null!" +
                           $"\nDisabling component to avoid errors.");
            enabled = false;
            return false;
        }
    }
}