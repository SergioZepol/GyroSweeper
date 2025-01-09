using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject platformPrefab;
    public GameObject Ground;
    public int initialPlatformCount = 20; // Cantidad de plataformas iniciales
    public float spawnInterval = 0.1f; // Intervalo más rápido para generar plataformas
    public float minYDistance = 1f; // Distancia mínima vertical entre plataformas
    public float maxYDistance = 2f; // Distancia máxima vertical entre plataformas

    private float lastSpawnY = -3.5f; // Última posición Y donde se generó una plataforma
    private Transform cameraTransform;
    private float screenHalfWidth; // Mitad del ancho de la pantalla en unidades del mundo
    private float despawnYThreshold = -10f; // Margen para eliminar plataformas fuera de la cámara
    private List<GameObject> activePlatforms = new List<GameObject>(); // Lista de plataformas activas

    void Start()
    {
        cameraTransform = Camera.main.transform;
        float screenHeight = 2f * Camera.main.orthographicSize; // Altura visible
        screenHalfWidth = screenHeight * Camera.main.aspect / 2; // Ancho visible / 2

        // Precarga de plataformas iniciales
        PreloadPlatforms();
        activePlatforms.Add(Ground);
    }

    void Update()
    {
        // Genera plataformas cuando el jugador se acerca a la parte superior
        while (lastSpawnY < cameraTransform.position.y + 15f)
        {
            SpawnPlatform();
        }

        // Elimina plataformas que están fuera del campo de visión
        CleanupPlatforms();
    }

    void PreloadPlatforms()
    {
        for (int i = 0; i < initialPlatformCount; i++)
        {
            SpawnPlatform();
        }
    }

    void SpawnPlatform()
    {
        Vector3 spawnPosition = new Vector3();
        spawnPosition.y = lastSpawnY + Random.Range(minYDistance, maxYDistance);

        // Obtener el tamaño de la plataforma
        float platformWidth = platformPrefab.GetComponent<Renderer>().bounds.size.x;

        // Ajustar los límites de generación horizontal para permitir "a ras"
        float minX = -screenHalfWidth + (platformWidth / 2); // Límite izquierdo
        float maxX = screenHalfWidth - (platformWidth / 2);  // Límite derecho

        // Generar posición X dentro de los límites ajustados
        spawnPosition.x = Random.Range(minX, maxX);

        GameObject platform = Instantiate(platformPrefab, spawnPosition, Quaternion.identity);
        activePlatforms.Add(platform);

        lastSpawnY = spawnPosition.y;
    }

    void CleanupPlatforms()
    {
        for (int i = activePlatforms.Count - 1; i >= 0; i--)
        {
            if (activePlatforms[i].transform.position.y < cameraTransform.position.y + despawnYThreshold)
            {
                Destroy(activePlatforms[i]);
                activePlatforms.RemoveAt(i);
            }
        }
    }
}
