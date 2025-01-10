using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class FanScript : MonoBehaviour
{
    public float blowThreshold = 0.1f; // Sensibilidad del soplido
    public float pushForce = 5f; // Fuerza con la que empuja hacia arriba
    private BoxCollider2D fanCollider;
    private bool isBlowing = false;

    private string microphone;
    private AudioClip micClip;
    private bool isMicInitialized = false;

    void Start()
    {
        // Inicializar el micrófono
        if (Microphone.devices.Length > 0)
        {
            microphone = Microphone.devices[0];
            micClip = Microphone.Start(microphone, true, 10, AudioSettings.outputSampleRate);
            isMicInitialized = true;
        }
        else
        {
            Debug.LogError("No se detectó un micrófono en el dispositivo.");
        }

        // Obtener el BoxCollider2D del ventilador
        fanCollider = GetComponent<BoxCollider2D>();
        fanCollider.isTrigger = true; // Asegurarse de que sea un Trigger
    }

    void Update()
    {
        if (isMicInitialized)
        {
            float micLevel = GetMicrophoneLevel();
            isBlowing = micLevel > blowThreshold;
        }
    }

    private float GetMicrophoneLevel()
    {
        if (!Microphone.IsRecording(microphone)) return 0;

        int micPosition = Microphone.GetPosition(microphone);
        if (micPosition <= 0 || micClip == null) return 0; // Validación para evitar errores

        float[] samples = new float[256];
        micClip.GetData(samples, Mathf.Max(0, micPosition - samples.Length)); // Asegurarse de no ir fuera de los límites
        float sum = 0;
        foreach (float sample in samples)
        {
            sum += sample * sample;
        }
        return Mathf.Sqrt(sum / samples.Length);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (isBlowing)
        {
            // Aplicar fuerza hacia arriba si el objeto tiene Rigidbody2D
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(Vector2.up * pushForce, ForceMode2D.Force);
            }
        }
    }

    void OnDestroy()
    {
        if (isMicInitialized)
        {
            Microphone.End(microphone);
        }
    }
}