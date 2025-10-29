using TMPro;
using UnityEngine;
using Managers;

namespace UI
{
    public class UIPoints : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PointsManager pointsManager;
        [SerializeField] private TMP_Text pointsText;

        [Header("Formatting")]
        [SerializeField] private int totalDigits = 12;

        private void OnEnable()
        {
            pointsManager.OnPointsChanged += UpdateText;
        }

        private void OnDisable()
        {
            pointsManager.OnPointsChanged -= UpdateText;
        }

        private void UpdateText(int total)
        {
            string padded = total.ToString().PadLeft(totalDigits, '0');
            pointsText.text = $"POINTS {padded}";
        }
    }
}
