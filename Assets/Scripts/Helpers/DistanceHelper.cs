using UnityEngine;

namespace Helpers
{
    public static class DistanceHelper
    {
        public static bool IsWithinRange(Vector3 a, Vector3 b, float range)
        {
            float sqrDistance = (a - b).sqrMagnitude;
            float sqrRange = range * range;
            bool withinRange = sqrDistance <= sqrRange;

            //Debug.Log($"[DistanceHelper] IsWithinRange: sqrDistance={sqrDistance:F2}, sqrRange={sqrRange:F2}, withinRange={withinRange}");

            return withinRange;
        }

        public static bool IsBeyondRange(Vector3 a, Vector3 b, float range)
        {
            float sqrDistance = (a - b).sqrMagnitude;
            float sqrRange = range * range;
            bool beyondRange = sqrDistance > sqrRange;

            //Debug.Log($"[DistanceHelper] IsBeyondRange: sqrDistance={sqrDistance:F2}, sqrRange={sqrRange:F2}, beyondRange={beyondRange}");

            return beyondRange;
        }

        public static float GetSqrDistance(Vector3 a, Vector3 b)
        {
            float sqrDistance = (a - b).sqrMagnitude;
            //Debug.Log($"[DistanceHelper] GetSqrDistance: sqrDistance={sqrDistance:F2}");
            return sqrDistance;
        }

        public static float GetDistance(Vector3 a, Vector3 b)
        {
            float distance = Mathf.Sqrt((a - b).sqrMagnitude);
            //Debug.Log($"[DistanceHelper] GetDistance: distance={distance:F2}");
            return distance;
        }
    }
}
