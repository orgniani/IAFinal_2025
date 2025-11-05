using TMPro;
using UnityEngine;

namespace UI
{
    public class UIHighScore : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_Text highScoreText;

        [Header("Formatting")]
        [SerializeField] private int totalDigits = 12;

        private const string HIGH_SCORE_KEY = "HighScore";

        private void OnEnable()
        {
            int highScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
            string padded = highScore.ToString().PadLeft(totalDigits, '0');
            highScoreText.text = $"HIGH SCORE {padded}";
        }
    }
}
