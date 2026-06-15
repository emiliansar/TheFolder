using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;

public enum GameState
{
    MainMenu,
    RestartMenu,
    Playing,
    Levelcomplete
}

public class GameManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;

    public AudioClip playSound;
    public AudioClip spawnSound;
    public AudioClip takeSound;
    public AudioClip putSound;
    public AudioClip lossSound;

    public GameState state;
    public GameObject mainMenu;
    public GameObject gameUI;
    public GameObject gameOverPanel;

    public TMP_Text scoreText;
    public int currentScore = 0;
    public int oldScore = 0;
    public float spawnDelay = 0.1f;
    public float spawnTimer;

    public SpawnPoint[] spawnPoints;

    public GameObject paperPrefab;
    public ColorType[] colors;

    public float maxScore = 10;

    public TMP_Text paperCounterText;
    public int paperCounter = 0;

    void Start()
    {
        SetState(GameState.MainMenu);
    }

    public void StartGame()
    {
        currentScore = 0;
        paperCounter = 0;

        oldScore = PlayerPrefs.GetInt("Score", 0);

        spawnTimer = 0f;

        SetState(GameState.Playing);

        PlayPlaySound();

        UpdateUI();
    }

    public void AddProgress()
    {
        Debug.Log("Progress + 1");

        currentScore++;

        if (currentScore >= oldScore)
        {
            oldScore = currentScore;

            PlayerPrefs.SetInt("Score", currentScore);
            PlayerPrefs.Save();
        }

        UpdateUI();
    }

    public void UpdateUI()
    {
        scoreText.text = currentScore.ToString();

        paperCounterText.text = paperCounter.ToString() + "/10";
        Debug.Log("Листков: " + paperCounter.ToString() + "/10");
    }

    void UpdateSpawnDelay()
    {
        spawnDelay = 2f - (currentScore / 10f) * 0.1f;

        spawnDelay = Mathf.Clamp(spawnDelay, 0.3f, 2f);
    }

    void GameOver()
    {
        Debug.Log("GAME OVER");
        SetState(GameState.RestartMenu);

        PlayLossSound();

        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        SetState(GameState.Playing);

        Paper[] papers =
            FindObjectsByType<Paper>(FindObjectsSortMode.None);

        foreach (Paper paper in papers)
        {
            Destroy(paper.gameObject);
        }

        foreach (SpawnPoint point in spawnPoints)
        {
            point.isOccupied = false;
        }

        currentScore = 0;

        paperCounter = 0;

        spawnTimer = 0f;

        UpdateUI();
        StartGame();
    }

    void SetState(GameState newState)
    {
        state = newState;

        if (state == GameState.MainMenu)
        {
            mainMenu.SetActive(true);
            gameOverPanel.SetActive(false);
            gameUI.SetActive(false);
        }
        else if (state == GameState.RestartMenu)
        {
            gameOverPanel.SetActive(true);
            mainMenu.SetActive(false);
            gameUI.SetActive(false);
        }
        else if (state == GameState.Playing)
        {
            gameUI.SetActive(true);
            gameOverPanel.SetActive(false);
            mainMenu.SetActive(false);
        }
        else if (state == GameState.Levelcomplete)
        {
            mainMenu.SetActive(true);
            gameOverPanel.SetActive(false);
            gameUI.SetActive(false);
        }
    }

    public void SpawnNewPaper()
    {
        List<SpawnPoint> freePoints = new List<SpawnPoint>();

        foreach (SpawnPoint point in spawnPoints)
        {
            if (!point.isOccupied)
            {
                freePoints.Add(point);
            }
        }

        if (freePoints.Count == 0)
        {
            GameOver();
            return;
        }

        paperCounter++;

        UpdateUI();

        SpawnPoint randomPoint = freePoints[UnityEngine.Random.Range(0, freePoints.Count)];

        float randomRotation = UnityEngine.Random.Range(-45f, 45f);

        Quaternion rotation = Quaternion.Euler(0f, 0f, randomRotation);

        GameObject obj = Instantiate(
            paperPrefab,
            randomPoint.transform.position,
            rotation
        );

        Paper paper = obj.GetComponent<Paper>();

        paper.gameManager = this;

        paper.currentSpawnPoint = randomPoint;

        paper.SetColor(
            colors[UnityEngine.Random.Range(0, colors.Length)]
        );

        paper.sr.sortingOrder = UnityEngine.Random.Range(4, 100);
        paper.baseOrder = paper.sr.sortingOrder;

        randomPoint.isOccupied = true;

        PlaySpawnSound();
    }

    public void PlayPlaySound()
    {
        audioSource.PlayOneShot(playSound);
    }

    public void PlaySpawnSound()
    {
        audioSource.PlayOneShot(spawnSound);
    }

    public void PlayTakeSound()
    {
        audioSource.PlayOneShot(takeSound);
    }

    public void PlayPutSound()
    {
        audioSource.PlayOneShot(putSound);
    }

    public void PlayLossSound()
    {
        audioSource.PlayOneShot(lossSound);
    }

    void Update()
    {
        if (state != GameState.Playing) return;

        UpdateSpawnDelay();

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnDelay)
        {
            spawnTimer = 0f;

            SpawnNewPaper();
        }
    }
}
