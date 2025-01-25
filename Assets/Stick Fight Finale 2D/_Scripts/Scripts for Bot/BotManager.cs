using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class BotManager : MonoBehaviour
{
    public GameObject botPrefab;
    public Transform[] spawnPoints;
    public int totalBots = 5;
    public int botsPerLevel = 5;
    private int currentBotCount;

    public TextMeshProUGUI botCounterText;
    public TextMeshProUGUI levelNumberText;
    public TextMeshProUGUI levelClearText; // UI for "Level Clear"

    public GameObject[] backgroundPrefabs; // Array of background prefabs
    private GameObject currentBackground; // Reference to the current background instance

    private int currentLevel = 1;
    private bool isPlayerAlive = true;
    private List<Transform> availableSpawnPoints;

    public Fader fader; // Reference to the Fader script

    // Define the background position (x=21.4, y=3, z=0)
    private Vector3 backgroundPosition = new Vector3(15f, 3f, 0f);
    private Quaternion backgroundRotation = Quaternion.identity; // Desired rotation for all backgrounds

    // Define the player spawn position (x=0, y=0, z=0)
    public Transform player;
    public Vector3 playerSpawnPosition = new Vector3(0f, 0f, 0f);

    void Start()
    {
        currentBotCount = totalBots;
        UpdateBotCounterUI();
        levelNumberText.text = "Level " + currentLevel;
        levelClearText.gameObject.SetActive(false); // Ensure the "Level Clear" text is hidden initially
        availableSpawnPoints = new List<Transform>(spawnPoints);
        StartCoroutine(SpawnBots(totalBots));

        PlayerHealth.OnPlayerDeath += StopAllBots;

        // Instantiate the first background
        InstantiateBackground();

        // Set the initial player position
        if (player != null)
        {
            player.position = playerSpawnPosition;
        }
    }

    void OnDestroy()
    {
        PlayerHealth.OnPlayerDeath -= StopAllBots;
    }

    public void OnBotKilled()
    {
        if (!isPlayerAlive) return;

        currentBotCount--;
        UpdateBotCounterUI();

        if (currentBotCount <= 0)
        {
            StartCoroutine(LevelClearSequence());
        }
    }

    private void UpdateBotCounterUI()
    {
        if (botCounterText != null)
        {
            botCounterText.text = "Enemy Left: " + currentBotCount;
        }
    }

    private IEnumerator LevelClearSequence()
    {
        // Display "Level Clear" message
        if (levelClearText != null)
        {
            levelClearText.gameObject.SetActive(true);
        }

        // Wait for a few seconds
        yield return new WaitForSeconds(2f);

        // Fade to black
        if (fader != null)
        {
            yield return StartCoroutine(fader.FadeIn());
        }

        // Proceed to the next level
        LoadNextLevel();

        // Fade back in
        if (fader != null)
        {
            yield return StartCoroutine(fader.FadeOut());
        }

        // Hide "Level Clear" message after a delay
        yield return new WaitForSeconds(1f);
        if (levelClearText != null)
        {
            levelClearText.gameObject.SetActive(false);
        }
    }

    private void LoadNextLevel()
    {
        currentLevel++;
        levelNumberText.text = "Level: " + currentLevel;

        // Change the environment background
        ChangeBackground();

        // Reset bot count for the new level
        totalBots += botsPerLevel;
        currentBotCount = totalBots;
        UpdateBotCounterUI();

        // Spawn new bots for the new level
        StartCoroutine(SpawnBots(totalBots));

        // Reposition the player
        if (player != null)
        {
            player.position = playerSpawnPosition;
        }
    }

    private void InstantiateBackground()
    {
        if (backgroundPrefabs.Length > 0)
        {
            currentBackground = Instantiate(backgroundPrefabs[0], backgroundPosition, backgroundRotation);
        }
        else
        {
            Debug.LogError("No background prefabs assigned.");
        }
    }

    private void ChangeBackground()
    {
        // Destroy the current background if it exists
        if (currentBackground != null)
        {
            Destroy(currentBackground);
        }

        // Instantiate the new background if it exists in the array
        if (currentLevel - 1 < backgroundPrefabs.Length)
        {
            currentBackground = Instantiate(backgroundPrefabs[currentLevel - 1], backgroundPosition, backgroundRotation);
        }
        else
        {
            Debug.LogError("No more backgrounds available for the next level.");
        }
    }

    private IEnumerator SpawnBots(int botCount)
    {
        for (int i = 0; i < botCount; i++)
        {
            if (availableSpawnPoints.Count == 0)
            {
                availableSpawnPoints = new List<Transform>(spawnPoints); // Reset the list if empty
            }

            Transform spawnPoint = availableSpawnPoints[Random.Range(0, availableSpawnPoints.Count)];
            if (botPrefab != null)
            {
                GameObject bot = Instantiate(botPrefab, spawnPoint.position, Quaternion.identity);
                BotHealth botHealth = bot.GetComponent<BotHealth>();
                if (botHealth != null)
                {
                    botHealth.botManager = this;
                }
                availableSpawnPoints.Remove(spawnPoint); // Remove used spawn point

                // Wait for a short duration before spawning the next bot
                yield return new WaitForSeconds(1f);
            }
            else
            {
                Debug.LogError("Bot Prefab is not assigned or is missing.");
            }
        }
    }

    private void StopAllBots()
    {
        isPlayerAlive = false;
        BotHealth[] bots = Object.FindObjectsByType<BotHealth>(FindObjectsSortMode.None);
        foreach (BotHealth bot in bots)
        {
            Destroy(bot.gameObject);
        }
    }
}
