using UnityEngine;
using System.Collections;

public class AidKitSpawner : MonoBehaviour
{
    public GameObject aidKitPrefab;             // Reference to the aid kit prefab
    public Transform[] aidKitSpawnPoints;       // Array of spawn points for aid kits
    public float spawnInterval = 30f;           // Time interval between spawns in seconds
    public float fixedZPosition = 0f;           // Fixed Z position to ensure aid kit is visible

    void Start()
    {
        StartCoroutine(SpawnAidKitsRepeatedly());
    }

    IEnumerator SpawnAidKitsRepeatedly()
    {
        while (true)
        {
            SpawnAidKit();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void SpawnAidKit()
    {
        // Select a random spawn point for the aid kit
        Transform spawnPoint = aidKitSpawnPoints[Random.Range(0, aidKitSpawnPoints.Length)];
        if (aidKitPrefab != null) // Ensure the prefab is not null before instantiation
        {
            Vector3 spawnPosition = new Vector3(spawnPoint.position.x, spawnPoint.position.y, fixedZPosition);
            GameObject aidKit = Instantiate(aidKitPrefab, spawnPosition, Quaternion.identity);

            Debug.Log("Aid kit spawned at position: " + spawnPosition); // Debug log to verify position

            // Ensure aid kit is on the correct layer and sorting order if it's a 2D object
            SpriteRenderer spriteRenderer = aidKit.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = 10; // Adjust sorting order as needed
            }
        }
        else
        {
            Debug.LogError("Aid Kit Prefab is not assigned or is missing.");
        }
    }
}
