using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class HUDScript : MonoBehaviour
{

    //Score text
    private Transform playerTransform; // Transform del jugador
    public GameObject text; // Objeto que contiene el TextMeshProUGUI
    public GameObject deadText;
    private TextMeshProUGUI textCanvas; // Referencia al componente TextMeshProUGUI
    private TextMeshProUGUI textDead;
    public GameObject Fox; // Referencia al jugador
    private float initialY; // Posición inicial del jugador en Y
    private int highestScore = 0; // Puntaje más alto alcanzado

    //Pause Button
    public GameObject pauseButton;
    public GameObject pauseObject;
    public bool paused = false;

    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    public bool mute = false;
    public GameObject MuteObject;
    public GameObject UnMuteObject;

    private float savedMusicVolume;
    private float savedSfxVolume;

    //Game Over
    public GameObject deadObject;

    private void Start()
    {
        // Obtén la referencia al componente TextMeshProUGUI
        textCanvas = text.GetComponent<TextMeshProUGUI>();
        textDead = deadText.GetComponent<TextMeshProUGUI>();

        // Obtén la referencia al Transform del jugador
        playerTransform = Fox.GetComponent<Transform>();

        // Guarda la posición inicial en Y del jugador
        initialY = playerTransform.position.y;

        // Configura el marcador inicial en 0
        textCanvas.text = "Score: 0";
    }

    private void Update()
    {
        // Calcula el puntaje relativo a la posición inicial
        int currentScore = Mathf.FloorToInt(playerTransform.position.y - initialY);

        // Solo actualiza el marcador si el puntaje actual supera el puntaje más alto
        if (currentScore > highestScore)
        {
            highestScore = currentScore;
            textCanvas.text = "Score: " + highestScore;
        }
    }

    private void SaveScore(int score)
    {
        // Cargar los puntajes existentes
        int[] scores = new int[5];
        for (int i = 0; i < 5; i++)
        {
            scores[i] = PlayerPrefs.GetInt($"HighScore{i}", 0);
        }

        // Agregar el nuevo puntaje y ordenar
        scores[4] = score;
        System.Array.Sort(scores);
        System.Array.Reverse(scores);

        // Guardar los 5 mejores puntajes
        for (int i = 0; i < 5; i++)
        {
            PlayerPrefs.SetInt($"HighScore{i}", scores[i]);
        }

        PlayerPrefs.Save();
    }

    public void Pause()
    {
        SfxScript.TriggerSfx("SfxButton1");
        pauseButton.SetActive(false);
        pauseObject.SetActive(true);
        paused = true;
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        SfxScript.TriggerSfx("SfxButton1");
        pauseObject.SetActive(false);
        pauseButton.SetActive(true);
        paused = false;
        Time.timeScale = 1f;
    }

    public void DeadScreen()
    {
        Time.timeScale = 0f;
        pauseButton.SetActive(false);
        textCanvas.gameObject.SetActive(false);
        deadObject.SetActive(true);

        textDead.text = "Your Score\n was: " + highestScore;
    }

    public void PlayMenu()
    {
        SfxScript.TriggerSfx("SfxButton1");
        SceneManager.LoadScene("MainMenuScene");
        Time.timeScale = 1f;
        // Guarda el puntaje actual
        SaveScore(highestScore);

        textDead.text = "Your Score\n was: " + highestScore;
    }

    public void Replay()
    {
        SfxScript.TriggerSfx("SfxButton1");
        pauseButton.SetActive(true);
        textCanvas.gameObject.SetActive(true);
        deadObject.SetActive(false);
        SceneManager.LoadScene("GameScene");

        // Guarda el puntaje actual
        SaveScore(highestScore);

    }
    public void Mute()
    {
        SfxScript.TriggerSfx("SfxButton1");
        if (!mute)
        {
            mute = true;

            // Guardar los volúmenes actuales
            myMixer.GetFloat("music", out savedMusicVolume);
            myMixer.GetFloat("sfx", out savedSfxVolume);

            // Mutear configurando los volúmenes a -80 dB (silencio)
            myMixer.SetFloat("music", -80f);
            myMixer.SetFloat("sfx", -80f);

            MuteObject.SetActive(false);
            UnMuteObject.SetActive(true);
        }
        else
        {
            mute = false;

            // Restaurar los volúmenes guardados
            myMixer.SetFloat("music", savedMusicVolume);
            myMixer.SetFloat("sfx", savedSfxVolume);

            UnMuteObject.SetActive(false);
            MuteObject.SetActive(true);
        }
    }
    public void setMasterVolume()
    {
        float mast = masterSlider.value;
        myMixer.SetFloat("master", Mathf.Log10(mast) * 20);
        PlayerPrefs.SetFloat("masterVolumen", mast);
    }

    public void setMusicVolume()
    {
        float mus = musicSlider.value;
        myMixer.SetFloat("music", Mathf.Log10(mus) * 20);
        PlayerPrefs.SetFloat("musicVolumen", mus);
    }

    public void setSfxVolume()
    {
        float sfx = sfxSlider.value;
        myMixer.SetFloat("sfx", Mathf.Log10(sfx) * 20);
        PlayerPrefs.SetFloat("sfxVolumen", sfx);
    }
}
