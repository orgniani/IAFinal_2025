using UnityEngine;

namespace VFX
{
    public class ClickIndicatorController : MonoBehaviour
    {
        [SerializeField] private ParticleSystem indicatorParticle;

        public void SpawnIndicator(Vector3 position)
        {
            if (!indicatorParticle)
                return;

            indicatorParticle.transform.position = position;

            if (indicatorParticle.isPlaying)
                indicatorParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            indicatorParticle.Play();
        }
    }
}
