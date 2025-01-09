using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public float jumpForce = 10f; // Altura fija del salto
    public GameObject zorro; // Arrastra el objeto "Zorro" aqu� en el inspector.
    private Animator zorroAnimator;

    void Start()
    {
        zorro = GameObject.Find("Fox"); // Cambia "Zorro" por el nombre exacto del GameObject en tu escena.
        zorroAnimator = zorro.GetComponent<Animator>();
    }

    private void OnCollisionEnter2D (Collision2D collision)
    {


        if (collision.relativeVelocity.y <= 0) // Aseg�rate de que el personaje est� cayendo
        {
            zorroAnimator.SetTrigger("IsJumping");

            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Ajusta la velocidad vertical directamente para evitar acumulaci�n
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
        }
    }
}
