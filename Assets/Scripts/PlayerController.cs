using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public Rigidbody2D rb;

    private float moveX;
    private Vector3 originalScale;



    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale; // Guardamos el tamaño original
    }

    // Update is called once per frame
    void Update()
    {
        // Detectar si estás en móvil o en ordenador
        if (Application.isMobilePlatform)
        {
            // Usar acelerómetro para el movimiento en móviles
            moveX = Input.acceleration.x * moveSpeed;
        }
        else
        {
            // Usar Input.GetAxis en ordenadores
            moveX = Input.GetAxis("Horizontal") * moveSpeed;
        }

        // Flip the character based on the direction
        if (moveX > 0)
        {
            // Look right
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
        else if (moveX < 0)
        {
            // Look left
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }

        CheckScreenBounds(); // Verificar si el personaje se sale de los límites

    }

    private void FixedUpdate()
    {
        Vector2 velocity = rb.velocity;
        velocity.x = moveX;
        rb.velocity = velocity;
    }

    private void CheckScreenBounds()
    {
        // Obtener las coordenadas de la cámara principal
        Camera mainCamera = Camera.main;
        Vector3 screenPosition = mainCamera.WorldToViewportPoint(transform.position);

        // Si el personaje se sale por el lado izquierdo o derecho, aparecerá en el lado opuesto
        if (screenPosition.x < 0) // Salió por el lado izquierdo
        {
            Vector3 newPosition = transform.position;
            newPosition.x = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x; // Aparece por la derecha
            transform.position = newPosition;
        }
        else if (screenPosition.x > 1) // Salió por el lado derecho
        {
            Vector3 newPosition = transform.position;
            newPosition.x = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x; // Aparece por la izquierda
            transform.position = newPosition;
        }

        // Si el personaje se sale por el borde inferior, regresa al menú principal
        if (screenPosition.y < 0) // Salió por abajo
        {

            LoadMainMenu();
        }
    }

    private void LoadMainMenu()
    {
        // Cargar la escena del menú principal
        SceneManager.LoadScene("MainMenuScene");
    }
}