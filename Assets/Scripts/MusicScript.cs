using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicScript : MonoBehaviour
{

    private static MusicScript instance = null; // Variable para mantener una �nica instancia
    private AudioSource Source;
    public float fadeInDuration = 2f; // Duraci�n del fade in en segundos
    private float vol_aux = 0.5f;

    // Clips de audio para cada escena
    public AudioClip menuMusic;
    public AudioClip gameMusic1;
    public AudioClip gameMusic2;
    public AudioClip gameMusic3;
    public AudioClip gameMusic4;
    public AudioClip gameMusic5;
    public AudioClip gameMusic6;
    public AudioClip gameMusic7;
    public AudioClip gameMusic8;

    void Awake()
    {
        // Verifica si ya existe una instancia de este objeto, si es as� destruye la nueva
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // No destruyas este objeto al cambiar de escena
        }

        Source = GetComponent<AudioSource>();
        if (Source == null)
        {
            Debug.LogError("No se encontr� un AudioSource en el GameObject.");
        }
    }

    void Start()
    {
        // Aseg�rate de que el volumen sea 0 para hacer el fade in despu�s
        Source.volume = 0f;
        // Reproducir la m�sica adecuada seg�n la escena inicial
        PlayMusicForScene(SceneManager.GetActiveScene().name);
    }

    void OnEnable()
    {
        // Subscribirse al evento de cambio de escena
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // Desubscribirse del evento de cambio de escena
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // M�todo que se llama cuando una nueva escena es cargada
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.name);
    }

    public void ChangeMusic()
    {
        AudioClip clipToPlay = null;
        AudioClip[] gameMusicClips = new AudioClip[]
{
                gameMusic1, gameMusic2, gameMusic3, gameMusic4,
                gameMusic5, gameMusic6, gameMusic7, gameMusic8
};

        // Seleccionar un clip aleatorio de la lista
        int randomIndex = Random.Range(0, gameMusicClips.Length);
        clipToPlay = gameMusicClips[randomIndex];
        // Verifica si el clip a reproducir es el mismo que ya est� en el AudioSource
        if (clipToPlay != null && Source.clip != clipToPlay)
        {
            // Asignar y reproducir la m�sica
            Source.clip = clipToPlay;
            Source.Play();

            // Siempre hacemos el fade in cuando se cambia la m�sica
            StartCoroutine(FadeIn(Source, fadeInDuration));
        }
    }

    // M�todo para ambiar la m�sica en funci�n de la escena
    void PlayMusicForScene(string sceneName)
    {
        AudioClip clipToPlay = null;

        // Usamos un switch para manejar las escenas espec�ficas como el men�
        switch (sceneName)
        {
            case "MainMenuScene":
                clipToPlay = menuMusic;
                break;
            case "GameScene":
                // Crear una lista de los clips de m�sica del juego
                AudioClip[] gameMusicClips = new AudioClip[]
                {
                gameMusic1, gameMusic2, gameMusic3, gameMusic4,
                gameMusic5, gameMusic6, gameMusic7, gameMusic8
                };

                // Seleccionar un clip aleatorio de la lista
                int randomIndex = Random.Range(0, gameMusicClips.Length);
                clipToPlay = gameMusicClips[randomIndex];
                break;
            default:
                Debug.Log("Escena sin asignar musica");
                break;
        }

        // Verifica si el clip a reproducir es el mismo que ya est� en el AudioSource
        if (clipToPlay != null && Source.clip != clipToPlay)
        {
            // Asignar y reproducir la m�sica
            Source.clip = clipToPlay;
            Source.Play();

            // Siempre hacemos el fade in cuando se cambia la m�sica
            StartCoroutine(FadeIn(Source, fadeInDuration));
        }
    }

    public void PlaySpecificMusic(AudioClip clip)
    {
        if (Source.clip == clip) return;

        Source.Stop();
        Source.clip = clip;
        Source.Play();
    }

    // M�todo est�tico para acceder al m�todo PlaySpecificMusic desde otros scripts
    public static void TriggerMusic(AudioClip clip)
    {
        if (instance != null)
        {
            instance.PlaySpecificMusic(clip);
        }
        else
        {
            Debug.LogWarning("No se encontr� la instancia de MusicScript.");
        }
    }

    // Corutina para hacer el fade in de volumen
    IEnumerator FadeIn(AudioSource audioSource, float duration)
    {
        audioSource.volume = 0f; // Empezamos con el volumen en 0
        float targetVolume = vol_aux; // El volumen al que queremos llegar

        float currentTime = 0f;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, targetVolume, currentTime / duration); // Lerp para un fade suave
            yield return null;
        }

        audioSource.volume = targetVolume; // Asegurarse de que el volumen sea exactamente 1 al final
    }
}