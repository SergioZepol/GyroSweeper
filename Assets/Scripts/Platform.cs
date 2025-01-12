using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public float jumpForce = 10f; // Altura fija del salto
    public GameObject zorro; // Arrastra el objeto "Zorro" aqu� en el inspector.
    public GameObject trampoline;
    private Animator zorroAnimator;
    private Animator platformAnimator;

    // Variables para el movimiento lateral
    public bool isMovingPlatform; // Activa si esta plataforma debe moverse
    public float moveSpeed = 2f; // Velocidad de movimiento
    private Camera mainCamera; // Referencia a la c�mara principal
    private bool movingRight = true; // Indica si la plataforma se mueve a la derecha o izquierda

    // Variables para plataformas que caen
    public bool isFallingPlatform; // Identifica si esta es una plataforma que cae
    private bool isFalling = false; // Indica si la plataforma ya est� cayendo

    void Start()
    {
        zorro = GameObject.Find("Fox"); // Cambia "Zorro" por el nombre exacto del GameObject en tu escena.
        trampoline = GameObject.FindGameObjectWithTag("Trampoline");
        zorroAnimator = zorro.GetComponent<Animator>();
        mainCamera = Camera.main; // Obtiene la referencia a la c�mara principal
    }

    void Update()
    {
        if (isMovingPlatform)
        {
            MovePlatform();
            CheckScreenLimits(); // Verifica los l�mites de la pantalla
        }

        trampoline = GameObject.FindGameObjectWithTag("Trampoline");
        platformAnimator = trampoline.GetComponent<Animator>();

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

            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Ajusta la velocidad vertical directamente para evitar acumulaci�n
                if (this.gameObject.tag == "Trampoline")
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce * 1.5f);
                    platformAnimator.SetTrigger("IsJumping");
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
        isFalling = true; // Marca la plataforma como cayendo
        Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>(); // Agrega un Rigidbody2D para simular la ca�da
        rb.gravityScale = 2f; // Ajusta la gravedad para una ca�da m�s r�pida
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Congela la rotaci�n para evitar que gire
    }
}
