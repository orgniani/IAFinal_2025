using System;
using UnityEngine;

namespace Events
{
    [CreateAssetMenu(menuName = "EventChannels/EmptyChannels", fileName = "EmptyChannel")]
    public class EmptyAction : ScriptableObject
    {
        [SerializeField] private ActionConfig _config;
        private Action _event = delegate { };

        public void Subscribe(Action action)
        {
            _event += action;
            if (_config.listenerEvent)
            {
                Debug.Log($"{name}: A listener({action}) was suscribed at Event.");
            }
        }

        public void Unsubscribe(Action action)
        {
            _event -= action;
            if (_config.listenerEvent)
            {
                Debug.Log($"{name}: A listener({action}) was unsuscribed at Event.");
            }
        }

        public void InvokeEvent()
        {
            _event?.Invoke();
            if (_config.eventLog)
            {
                Debug.Log($"{name}: The event was invoked.");
            }
        }
    }
}
