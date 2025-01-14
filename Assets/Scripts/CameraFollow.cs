using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // El jugador u objetivo al que seguirá la cámara
    public SpriteRenderer backgroundRenderer; // Referencia al SpriteRenderer del background
    public Sprite[] backgroundSprites; // Array para almacenar los diferentes sprites de fondo
    public float heightInterval = 50f; // Intervalo de altura para cambiar de fondo
    public float fadeDuration = 1f; // Duración del fade in y fade out


    private int currentBackgroundIndex = 0; // Índice del fondo actual
    private float nextHeightThreshold = 0f; // Altura para cambiar al siguiente fondo
    private Coroutine fadeCoroutine; // Para asegurarnos de no ejecutar múltiples fades al mismo tiempo
    private bool isFirstBackground = true; // Control para evitar fade en el primer fondo


    private void Start()
    {
        if (backgroundSprites.Length > 0)
        {
            // Configurar el primer fondo y el primer umbral
            ChangeBackground(0);
            nextHeightThreshold = heightInterval;
        }
        else
        {
            Debug.LogWarning("No se asignaron sprites de background en el array.");
        }
    }

    private void LateUpdate()
    {
        // Seguir al objetivo en Y si se encuentra por encima de la cámara
        if (target.position.y > transform.position.y)
        {
            Vector3 newPosition = new Vector3(transform.position.x, target.position.y, transform.position.z);
            transform.position = newPosition;

            // Verificar si se ha alcanzado el siguiente umbral de altura
            if (target.position.y >= nextHeightThreshold)
            {
                currentBackgroundIndex = (currentBackgroundIndex + 1) % backgroundSprites.Length; // Cambiar al siguiente sprite
                ChangeBackground(currentBackgroundIndex);
                nextHeightThreshold += heightInterval; // Ajustar el siguiente umbral
            }
        }
    }

    private void ChangeBackground(int index, bool instant = false)
    {
        // Cambiar el sprite del fondo con o sin fade
        if (backgroundRenderer != null && index < backgroundSprites.Length)
        {
            if (instant || isFirstBackground)
            {
                // Cambiar de forma instantánea
                backgroundRenderer.sprite = backgroundSprites[index];
                SetAlpha(1f);
                isFirstBackground = false;
            }
            else
            {
                // Iniciar el fade out y fade in
                if (fadeCoroutine != null)
                    StopCoroutine(fadeCoroutine);

                fadeCoroutine = StartCoroutine(FadeBackground(index));
            }
        }
        else
        {
            Debug.LogWarning($"No se encontró un SpriteRenderer o índice fuera de rango: {index}");
        }
    }
    private IEnumerator FadeBackground(int newIndex)
    {
        // Desvanecer el fondo actual
        yield return StartCoroutine(FadeAlpha(0f));

        // Cambiar el sprite después del fade out
        backgroundRenderer.sprite = backgroundSprites[newIndex];

        // Hacer fade in del nuevo fondo
        yield return StartCoroutine(FadeAlpha(1f));
    }

    private IEnumerator FadeAlpha(float targetAlpha)
    {
        float startAlpha = backgroundRenderer.color.a; // Alpha actual
        float elapsedTime = 0f;

        // Lerp del canal alpha
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            SetAlpha(newAlpha);
            yield return null;
        }

        // Asegurarse de llegar al alpha objetivo
        SetAlpha(targetAlpha);
    }

    private void SetAlpha(float alpha)
    {
        // Actualizar el alpha del color en el SpriteRenderer
        if (backgroundRenderer != null)
        {
            Color color = backgroundRenderer.color;
            color.a = alpha;
            backgroundRenderer.color = color;
        }
    }
}
