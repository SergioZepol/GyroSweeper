using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsScene : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    public bool mute = false;
    public GameObject MuteObject;
    public GameObject UnMuteObject;

    private float savedMusicVolume;
    private float savedSfxVolume;
    public void PlayMenu()
    {
        SfxScript.TriggerSfx("SfxButton1");
        SceneManager.LoadScene("MainMenuScene");
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

    private void Awake()
    {
        if (PlayerPrefs.HasKey("masterVolumen") || PlayerPrefs.HasKey("musicVolumen") || PlayerPrefs.HasKey("sfxVolumen"))
        {
            LoadVolume();
        }
        setMasterVolume();
        setMusicVolume();
        setSfxVolume();
    }

    public void LoadVolume()
    {
        masterSlider.value = PlayerPrefs.GetFloat("masterVolumen");
        musicSlider.value = PlayerPrefs.GetFloat("musicVolumen");
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVolumen");
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
