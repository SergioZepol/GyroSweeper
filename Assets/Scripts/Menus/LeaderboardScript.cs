using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaderboardScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] scoreTexts; // Asigna 5 textos en el inspector

    private void Start()
    {
        DisplayScores();
    }

    private void DisplayScores()
    {
        for (int i = 0; i < scoreTexts.Length; i++)
        {
            int score = PlayerPrefs.GetInt($"HighScore{i}", 0);
            scoreTexts[i].text = $"#{i + 1}: {score}";
        }
    }

    public void PlayMenu()
    {
        SfxScript.TriggerSfx("SfxButton1");
        SceneManager.LoadScene("MainMenuScene");
    }
}
