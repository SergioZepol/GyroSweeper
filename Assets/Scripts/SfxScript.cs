using System;
using Unity.VisualScripting;
using UnityEngine;

public class SfxScript : MonoBehaviour
{
    private static SfxScript instance = null;
    private AudioSource sfxSource;
    private float vol_aux = 0.5f;

    // Clips de efectos de sonido
    public AudioClip sfxButton1;
    public AudioClip sfxDead;
    public AudioClip sfxJump;
    public AudioClip sfxTrampoline;
    public AudioClip sfxFalling;
    public AudioClip sfxHelix;

    // Evento para reproducir efectos de sonido
    public static event Action<string> OnPlaySfx;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        sfxSource = GetComponent<AudioSource>();
        if (sfxSource == null)
        {
            Debug.LogError("No se encontró un AudioSource en el GameObject para SFX.");
        }

        vol_aux = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        sfxSource.volume = vol_aux;
    }

    void OnEnable()
    {
        // Suscribirse al evento
        OnPlaySfx += PlaySfx;
    }

    void OnDisable()
    {
        // Desuscribirse del evento
        OnPlaySfx -= PlaySfx;
    }

    // Método para invocar el evento desde otros scripts
    public static void TriggerSfx(string sfxName)
    {
        if (OnPlaySfx != null)
        {
            OnPlaySfx.Invoke(sfxName);
        }
    }

    // Método para reproducir efectos de sonido
    private void PlaySfx(string sfxName)
    {
        AudioClip clipToPlay = null;

        switch (sfxName)
        {
            case "SfxButton1":
                clipToPlay = sfxButton1;
                break;
            case "SfxJump":
                clipToPlay = sfxJump;
                break;
            case "SfxDead":
                clipToPlay = sfxDead;
                break;
            case "SfxTrampoline":
                clipToPlay = sfxTrampoline;
                break;
            case "SfxHelix":
                clipToPlay = sfxHelix;
                break;
            case "SfxFalling":
                clipToPlay = sfxFalling;
                break;
            default:
                Debug.LogWarning("Efecto de sonido no encontrado");
                return;
        }

        if (clipToPlay != null)
        {
            sfxSource.PlayOneShot(clipToPlay); // Usamos PlayOneShot para que no se interrumpan otros sonidos
        }
    }
}
