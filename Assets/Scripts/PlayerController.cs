using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float fanLiftForce = 20f; // Fuerza de elevaci�n del ventilador
    public Rigidbody2D rb;
    public HUDScript hud;
    private bool dead = false;

    private float moveX;
    private Vector3 originalScale;
    private bool isInFanZone = false; // Indica si est� en la zona del ventilador
    private string microphoneDevice;
    private AudioClip micClip;
    private bool isBlowing = false;
    private bool isFanActivated = false; // Nuevo: Indica si el ventilador est� activado por clic/toque

    void Awake()
    {
        dead = false;
        Time.timeScale = 1f;
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale; // Guardamos el tama�o original
        /*
        // Configurar el micr�fono
        if (Microphone.devices.Length > 0)
        {
            microphoneDevice = Microphone.devices[0]; // Seleccionar el primer micr�fono disponible
            micClip = Microphone.Start(microphoneDevice, true, 1, 44100); // Iniciar grabaci�n
        }
        else
        {
            Debug.LogWarning("No se detect� ning�n micr�fono.");
        }
        */
    }

    void Update()
    {
        if (!dead)
        {
            // Detectar si est�s en m�vil o en ordenador
            if (Application.isMobilePlatform)
            {
                moveX = Input.acceleration.x * moveSpeed;
            }
            else
            {
                moveX = Input.GetAxis("Horizontal") * moveSpeed;
            }

            // Flip the character based on the direction
            if (moveX > 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
            }
            else if (moveX < 0)
            {
                transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
            }

            CheckScreenBounds(); // Verificar si el personaje se sale de los l�mites

            // Detectar si el usuario est� soplando al micr�fono
            //isBlowing = CheckBlowing();

            // Detectar clic o toque sobre objetos "Fan"
            //DetectFanClickOrTouch();
        }
    }

    private void FixedUpdate()
    {
        if (!dead)
        {
            Vector2 velocity = rb.velocity;
            velocity.x = moveX;

            // Si est� en la zona del ventilador y se activa (por soplo o clic/toque), a�adir fuerza hacia arriba
            if (isInFanZone && (isBlowing || isFanActivated))
            {
                velocity.y = fanLiftForce;
            }

            rb.velocity = velocity;
        }
        else
        {
            Vector2 velocity;
            velocity.x = 0;
            velocity.y = 0;
            rb.velocity = velocity;
            rb.gravityScale = 0;
        }
    }

    private void CheckScreenBounds()
    {
        Camera mainCamera = Camera.main;
        Vector3 screenPosition = mainCamera.WorldToViewportPoint(transform.position);

        if (screenPosition.x < 0)
        {
            Vector3 newPosition = transform.position;
            newPosition.x = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
            transform.position = newPosition;
        }
        else if (screenPosition.x > 1)
        {
            Vector3 newPosition = transform.position;
            newPosition.x = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
            transform.position = newPosition;
        }

        if (screenPosition.y < 0)
        {
            Dead();
        }
    }

    private void Dead()
    {
        dead = true;
        Animator animator = GetComponent<Animator>();
        animator.SetTrigger("IsDead");
        SfxScript.TriggerSfx("SfxDead");
        StartCoroutine(HandleDeathScreen()); // Iniciar corutina
    }
    private IEnumerator HandleDeathScreen()
    {
        yield return new WaitForSeconds(1f); // Esperar 1 segundo (ajusta el tiempo seg�n sea necesario)
        hud.DeadScreen(); // Mostrar la pantalla de muerte despu�s del retraso
        this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.gameObject.CompareTag("Da�o") && collision.relativeVelocity.y >= 0) || collision.gameObject.CompareTag("Da�oMuerte"))
        {
            Dead();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("FanAir"))
        {
            isInFanZone = true; // Estamos en la zona del ventilador
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("FanAir"))
        {
            isInFanZone = false; // Salimos de la zona del ventilador
        }
    }

    private bool CheckBlowing()
    {
        if (micClip == null || !Microphone.IsRecording(microphoneDevice)) return false;

        // Leer la posici�n del micr�fono
        int micPosition = Microphone.GetPosition(microphoneDevice);
        if (micPosition < 128) return false; // Asegurarse de que haya suficientes datos disponibles

        float[] micData = new float[128];

        // Calcular la posici�n de inicio, ajustando si es necesario
        int startPosition = micPosition - micData.Length;
        if (startPosition < 0)
        {
            startPosition += micClip.samples; // Ajustar para grabaci�n circular
        }

        micClip.GetData(micData, startPosition);

        // Calcular la amplitud promedio
        float averageAmplitude = 0f;
        foreach (float sample in micData)
        {
            averageAmplitude += Mathf.Abs(sample);
        }
        averageAmplitude /= micData.Length;

        // Determinar si se est� soplando
        return averageAmplitude > 0.1f; // Umbral ajustable
    }

    private void DetectFanClickOrTouch()
    {
        if (Input.GetMouseButtonDown(0)) // Detectar clic en PC
        {
            DetectFanHit(Input.mousePosition);
        }

        if (Input.touchCount > 0) // Detectar toque en m�vil
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                DetectFanHit(touch.position);
            }
        }
    }

    private void DetectFanHit(Vector3 screenPosition)
    {
        // Convertir la posici�n de pantalla a un rayo
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider != null && hit.collider.CompareTag("Fan"))
        {
            isFanActivated = true;
            StartCoroutine(DeactivateFanAfterDelay());
        }
    }

    private IEnumerator DeactivateFanAfterDelay()
    {
        yield return new WaitForSeconds(0.5f); // Tiempo que el ventilador estar� activado
        isFanActivated = false;
    }

    private void OnDestroy()
    {
        if (Microphone.IsRecording(microphoneDevice))
        {
            Microphone.End(microphoneDevice); // Detener el micr�fono al cerrar el juego
        }
    }
}
