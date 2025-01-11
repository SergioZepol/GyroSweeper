using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public float jumpForce = 10f; // Altura fija del salto
    public GameObject zorro; // Arrastra el objeto "Zorro" aquí en el inspector.
    private Animator zorroAnimator;

    // Variables para el movimiento lateral
    public bool isMovingPlatform; // Activa si esta plataforma debe moverse
    public float moveSpeed = 2f; // Velocidad de movimiento
    private Camera mainCamera; // Referencia a la cámara principal
    private bool movingRight = true; // Indica si la plataforma se mueve a la derecha o izquierda

    void Start()
    {
        zorro = GameObject.Find("Fox"); // Cambia "Zorro" por el nombre exacto del GameObject en tu escena.
        zorroAnimator = zorro.GetComponent<Animator>();
        mainCamera = Camera.main; // Obtiene la referencia a la cámara principal
    }

    void Update()
    {
        if (isMovingPlatform)
        {
            MovePlatform();
            CheckScreenLimits(); // Verifica los límites de la pantalla
        }
    }

    private void MovePlatform()
    {
        // Mueve la plataforma hacia la derecha o izquierda dependiendo de la dirección
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
        // Convierte la posición de la plataforma al espacio de la pantalla (viewport)
        Vector3 screenPosition = mainCamera.WorldToViewportPoint(transform.position);

        // Define un margen dentro de los límites de la pantalla
        float margin = 0.08f;

        // Si la plataforma se acerca al borde derecho, cambia la dirección a la izquierda
        if (screenPosition.x > 1 - margin)
        {
            movingRight = false;
        }
        // Si la plataforma se acerca al borde izquierdo, cambia la dirección a la derecha
        else if (screenPosition.x < margin)
        {
            movingRight = true;
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.y <= 0) // Asegúrate de que el personaje está cayendo
        {
            zorroAnimator.SetTrigger("IsJumping");

            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Ajusta la velocidad vertical directamente para evitar acumulación
                if (this.gameObject.tag == "Trampoline")
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce * 1.5f);
                }
                else
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                }
            }
        }
    }
}
