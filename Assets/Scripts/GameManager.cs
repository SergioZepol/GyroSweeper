using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public struct PlatformPrefab
    {
        public GameObject prefab;
        public float probability; // Probabilidad de aparición
    }

    public List<PlatformPrefab> platforms; // Lista de prefabs con sus probabilidades
    public int initialPlatformCount = 20; // Cantidad de plataformas iniciales
    public float spawnInterval = 0.1f; // Intervalo más rápido para generar plataformas
    public float minYDistance = 1f; // Distancia mínima vertical entre plataformas
    public float maxYDistance = 2f; // Distancia máxima vertical entre plataformas

    private float lastSpawnY = -3.5f; // Última posición Y donde se generó una plataforma
    private Transform cameraTransform;
    private float screenHalfWidth; // Mitad del ancho de la pantalla en unidades del mundo
    private float despawnYThreshold = -10f; // Margen para eliminar plataformas fuera de la cámara
    private List<GameObject> activePlatforms = new List<GameObject>(); // Lista de plataformas activas

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        float screenHeight = 2f * Camera.main.orthographicSize; // Altura visible
        screenHalfWidth = screenHeight * Camera.main.aspect / 2; // Ancho visible / 2

        // Precarga de plataformas iniciales
        PreloadPlatforms();
    }

    private void Update()
    {
        // Genera plataformas cuando el jugador se acerca a la parte superior
        while (lastSpawnY < cameraTransform.position.y + 15f)
        {
            SpawnPlatform();
        }

        // Elimina plataformas que están fuera del campo de visión
        CleanupPlatforms();
    }

    private void PreloadPlatforms()
    {
        for (int i = 0; i < initialPlatformCount; i++)
        {
            SpawnPlatform();
        }
    }

    private void SpawnPlatform()
    {
        Vector3 spawnPosition = new Vector3();
        spawnPosition.y = lastSpawnY + Random.Range(minYDistance, maxYDistance);

        // Obtener el tamaño de la plataforma
        float platformWidth = platforms[0].prefab.GetComponent<Renderer>().bounds.size.x;

        // Ajustar los límites de generación horizontal para permitir "a ras"
        float minX = -screenHalfWidth + (platformWidth / 2); // Límite izquierdo
        float maxX = screenHalfWidth - (platformWidth / 2);  // Límite derecho

        // Generar posición X dentro de los límites ajustados
        spawnPosition.x = Random.Range(minX, maxX);

        // Seleccionar una plataforma aleatoriamente según la probabilidad
        GameObject selectedPlatform = SelectRandomPlatform();

        GameObject platform = Instantiate(selectedPlatform, spawnPosition, Quaternion.identity);
        activePlatforms.Add(platform);

        lastSpawnY = spawnPosition.y;
    }

    private void CleanupPlatforms()
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

    private GameObject SelectRandomPlatform()
    {
        float totalProbability = 0f;
        foreach (var platform in platforms)
        {
            totalProbability += platform.probability;
        }

        float randomValue = Random.value * totalProbability;
        float cumulativeProbability = 0f;

        foreach (var platform in platforms)
        {
            cumulativeProbability += platform.probability;
            if (randomValue <= cumulativeProbability)
            {
                return platform.prefab;
            }
        }

        return platforms[0].prefab; // Retorno por defecto si no se encuentra ninguno
    }
}
