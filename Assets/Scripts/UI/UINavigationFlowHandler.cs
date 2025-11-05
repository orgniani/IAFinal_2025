using Events;
using System;
using System.Collections;
using UI.Menus;
using UnityEngine;

namespace UI
{
    public class UINavigationFlowHandler
    {
        private readonly MonoBehaviour _runner;
        private readonly EmptyAction _onLoadEndEvent;

        public UINavigationFlowHandler(MonoBehaviour runner, EmptyAction onLoadEnd)
        {
            _runner = runner;
            _onLoadEndEvent = onLoadEnd;
        }

        /// <summary>
        /// Begins a coroutine that waits for level load to complete, then invokes the callback.
        /// </summary>
        public void Begin(UIMenuID? menuID, Action flowAction, Action onComplete)
        {
            flowAction?.Invoke();
            _runner.StartCoroutine(WaitForLoadEnd(menuID, onComplete));
        }

        /// <summary>
        /// Waits until the onLoadEnd event is triggered, then calls the provided onComplete action.
        /// Subscribes a temporary callback that sets a flag when the event is fired,
        /// yielding until that flag becomes true. Ensures the callback is unsubscribed afterward.
        /// </summary>
        private IEnumerator WaitForLoadEnd(UIMenuID? pendingMenuID, Action onComplete)
        {
            bool completed = false;
            void Callback() => completed = true;

            _onLoadEndEvent.Subscribe(Callback);

            yield return new WaitUntil(() => completed);

            _onLoadEndEvent.Unsubscribe(Callback);

            onComplete?.Invoke();
        }
    }
}
