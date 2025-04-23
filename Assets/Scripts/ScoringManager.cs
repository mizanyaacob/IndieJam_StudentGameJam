using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoringManager : MonoBehaviour
{
    public static ScoringManager Instance;

    public int P1Score, P2Score, P3Score, P4Score;

    [Header("Player Score UI")]
    [SerializeField] private int maxScore = 100;
    [SerializeField] private Image[] playerScoreImages;
    [SerializeField] private TMP_Text[] score; 

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        foreach (var image in playerScoreImages)
        {
            if (image != null)
                image.fillAmount = 0f; // Initialize fill amount to 0
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


}
