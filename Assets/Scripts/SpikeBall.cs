using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeBall : MonoBehaviour
{
    public Transform[] points; // Array de puntos (esquinas del camino)
    public float moveSpeed = 3f; // Velocidad de movimiento
    private int currentPointIndex = 0; // �ndice del punto actual

    void Start()
    {
        // Posicionar la bola en el primer punto
        transform.position = points[currentPointIndex].position;
    }

    void Update()
    {
        if (points.Length == 0) return;

        // Mover la bola hacia el siguiente punto
        MoveTowardsNextPoint();
    }

    private void MoveTowardsNextPoint()
    {
        // Punto objetivo actual
        Transform targetPoint = points[currentPointIndex];

        // Mover la bola hacia el punto objetivo
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);

        // Si ha llegado al punto objetivo
        if (Vector3.Distance(transform.position, targetPoint.position) <= 0.05f)
        {
            // Avanzar al siguiente punto (en bucle)
            currentPointIndex = (currentPointIndex + 1) % points.Length;
        }
    }
}
