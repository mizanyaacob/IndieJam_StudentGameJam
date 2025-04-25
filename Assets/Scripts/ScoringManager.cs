using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class ScoringManager : MonoBehaviour
{
    public static ScoringManager Instance;

    public int P1Score, P2Score, P3Score, P4Score;

    [Header("Player Score UI")]
    [SerializeField] private int maxScore = 100;
    [SerializeField] private Image[] playerScoreImages;
    [SerializeField] private TMP_Text[] score;

    [Header("TIMER")]
    [SerializeField] private float timerDuration = 120f; // Duration in seconds
    [SerializeField] private TMP_Text timerText; // Reference to the UI Text for the timer
    private float remainingTime;
    private bool timerRunning = false;

    private AudioManager audioManager; // Reference to AudioManager script
    private bool hasPlayedCountdownSFX = false;




    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        foreach (var image in playerScoreImages)
        {
            if (image != null)
                image.fillAmount = 0f; // Initialize fill amount to 0
        }   

        audioManager = FindAnyObjectByType<AudioManager>();
    }

    private void Start()
    {
       timerText.text = timerDuration.ToString(); // Initialize the timer text
        // Initialize scores
        P1Score = 0;
        P2Score = 0;
        P3Score = 0;
        P4Score = 0;

        // Start the timer
        StartTimer();
    }
    private void Update()
    {
        if (!timerRunning) return;

        remainingTime -= Time.deltaTime;

        if (timerText != null)
        {
            timerText.text = Mathf.Ceil(remainingTime).ToString(); // Update the timer text
        }

        if (remainingTime <= 10f)
        {
            timerText.color = Color.red; // Change color to red when time is low
        }

        // Play countdown sound once at 5 seconds left
        if (!hasPlayedCountdownSFX && remainingTime <= 5f)
        {
            audioManager.PlaySFX(audioManager.Countdown);
            hasPlayedCountdownSFX = true;
        }

        if (remainingTime <= 0f)
        {
            timerRunning = false;
            HandleGameOver();
        }
    }



    public void AddScore(int playerIndex, int value)
    {
        switch (playerIndex)
        {
            case 0: P1Score = Mathf.Min(P1Score + value, maxScore); break;
            case 1: P2Score = Mathf.Min(P2Score + value, maxScore); break;
            case 2: P3Score = Mathf.Min(P3Score + value, maxScore); break;
            case 3: P4Score = Mathf.Min(P4Score + value, maxScore); break;
        }

        UpdateScoreUI(playerIndex);
        Debug.Log($"Player {playerIndex + 1} score: {GetScore(playerIndex)}");
    }


    public int GetScore(int playerIndex)
    {
        return playerIndex switch
        {
            0 => P1Score,
            1 => P2Score,
            2 => P3Score,
            3 => P4Score,
            _ => 0
        };
    }

    private void UpdateScoreUI(int playerIndex)
    {
        int currentScore = GetScore(playerIndex);

        // Update fill amount on score bar
        if (playerScoreImages != null && playerIndex >= 0 && playerIndex < playerScoreImages.Length)
        {
            float normalized = (float)currentScore / maxScore;
            playerScoreImages[playerIndex].fillAmount = normalized;
        }

        // Update numeric score text
        if (score != null && playerIndex >= 0 && playerIndex < score.Length)
        {
            score[playerIndex].text = currentScore.ToString();
        }
    }

    public void StartTimer()
    {
        remainingTime = timerDuration;
        timerRunning = true;
    }
    private void HandleGameOver()
    {
        Debug.Log("Time's up! Game Over.");
        audioManager.PlaySFX(audioManager.GameEnd);
        SceneManager.LoadScene("GAMEOVER"); // Make sure you add this scene to your Build Settings
    }

}
