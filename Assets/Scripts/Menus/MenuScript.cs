using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{    public void PlayButton()
    {
        SfxScript.TriggerSfx("SfxButton1");
        SceneManager.LoadScene("GameScene");
    }
    public void PlayOptions()
    {
        SfxScript.TriggerSfx("SfxButton1");
        SceneManager.LoadScene("OptionsScene");
    }
    public void PlayLeaderboard()
    {
        SfxScript.TriggerSfx("SfxButton1");
        SceneManager.LoadScene("LeaderboardScene");
    }
    public void PlayMenu()
    {
        SfxScript.TriggerSfx("SfxButton1");
        SceneManager.LoadScene("MainMenuScene");
    }
    public void PlayQuit()
    {
        SfxScript.TriggerSfx("SfxButton1");
        Application.Quit();
    }
}
