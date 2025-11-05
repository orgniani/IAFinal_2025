using Events;
using UnityEngine;

namespace Audio
{
    [CreateAssetMenu(menuName = "EventChannels/AudioChannel", fileName = "AudioEvent")]
    public class AudioEvent : EventChannels<AudioKey> { }
}