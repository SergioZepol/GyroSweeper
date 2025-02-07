using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public float jumpForce = 10f; // Altura fija del salto
    public GameObject zorro; // Arrastra el objeto "Zorro" aqu� en el inspector.
    private Animator zorroAnimator;

    // Variables para el movimiento lateral
    public bool isMovingPlatform; // Activa si esta plataforma debe moverse
    public float moveSpeed = 2f; // Velocidad de movimiento
    private Camera mainCamera; // Referencia a la c�mara principal
    private bool movingRight = true; // Indica si la plataforma se mueve a la derecha o izquierda

    // Variables para plataformas que caen
    public bool isFallingPlatform; // Identifica si esta es una plataforma que cae
    private bool isFalling = false; // Indica si la plataforma ya est� cayendo
    private bool animada = false;

    // Variables de audio
    public AudioSource audioSource; // Fuente de audio

    void Start()
    {
        zorro = GameObject.Find("Fox"); // Cambia "Zorro" por el nombre exacto del GameObject en tu escena.
        zorroAnimator = zorro.GetComponent<Animator>();
        mainCamera = Camera.main; // Obtiene la referencia a la c�mara principal
        if (isFallingPlatform)
        {
            audioSource = this.GetComponent<AudioSource>();
            audioSource.Play();
        }
    }

    void Update()
    {
        if (isMovingPlatform)
        {
            MovePlatform();
            CheckScreenLimits(); // Verifica los l�mites de la pantalla
        }

        // Controla el sonido de "helix"
        if (isFallingPlatform)
        {
            HandleHelixSfx();
        }
    }

    private void MovePlatform()
    {
        // Mueve la plataforma hacia la derecha o izquierda dependiendo de la direcci�n
        if (movingRight)
        {
            transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
        }
    }

    private void CheckScreenLimits()
    {
        // Convierte la posici�n de la plataforma al espacio de la pantalla (viewport)
        Vector3 screenPosition = mainCamera.WorldToViewportPoint(transform.position);

        // Define un margen dentro de los l�mites de la pantalla
        float margin = 0.08f;

        // Si la plataforma se acerca al borde derecho, cambia la direcci�n a la izquierda
        if (screenPosition.x > 1 - margin)
        {
            movingRight = false;
        }
        // Si la plataforma se acerca al borde izquierdo, cambia la direcci�n a la derecha
        else if (screenPosition.x < margin)
        {
            movingRight = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.y <= 0) // Aseg�rate de que el personaje est� cayendo
        {
            zorroAnimator.SetTrigger("IsJumping");
            SfxScript.TriggerSfx("SfxJump");
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Ajusta la velocidad vertical directamente para evitar acumulaci�n
                if (this.gameObject.tag == "Trampoline")
                {
                    Animator platformAnimator = this.GetComponent<Animator>();
                    platformAnimator.SetTrigger("IsJumping");
                    SfxScript.TriggerSfx("SfxTrampoline");
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce * 1.5f);
                }
                else
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                }
                if (isFallingPlatform && !isFalling)
                {
                    StartFalling();
                }
            }
        }
    }

    private void StartFalling()
    {
        if (!animada)
        {
            Animator platformAnimator = this.GetComponent<Animator>();
            platformAnimator.SetTrigger("IsFalling");
            animada = true;
        }
        isFalling = true; // Marca la plataforma como cayendo
        SfxScript.TriggerSfx("SfxFalling");
        Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>(); // Agrega un Rigidbody2D para simular la ca�da
        rb.gravityScale = 2f; // Ajusta la gravedad para una ca�da m�s r�pida
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Congela la rotaci�n para evitar que gire
        EdgeCollider2D ec = gameObject.GetComponent<EdgeCollider2D>();
        ec.enabled = false; //Desactiva las colisiones para que al caer no choque
        // Det�n el sonido de "helix" al iniciar la ca�da
        if (audioSource.isPlaying)
        {
            this.audioSource.Stop();
            this.GetComponent<AudioSource>().enabled = false;
        }
    }

    private void HandleHelixSfx()
    {
        // Convierte la posici�n de la plataforma al espacio de la pantalla (viewport)
        Vector3 screenPosition = mainCamera.WorldToViewportPoint(transform.position);

        // Reproduce o detiene el sonido "helix" basado en la posici�n y estado de la plataforma
        if (!isFalling && screenPosition.x > 0 && screenPosition.x < 1 && screenPosition.y > 0 && screenPosition.y < 1)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
}
