using UnityEngine;
using TMPro;
using System.Linq;
using Unity.VisualScripting;

public class GameOverUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text[] scoreTexts;
    [SerializeField] private GameObject[] crowns; // Array of crown GameObjects (1 per player)

    private void Awake()
    {
        // Ensure all crowns are inactive at the start
        foreach (var crown in crowns)
        {
            crown.SetActive(false);
        }
    }
    private void Start()
    {
        // Set the score texts
        for (int i = 0; i < GameData.PlayerScores.Count && i < scoreTexts.Length; i++)
        {
            scoreTexts[i].text = $"{GameData.PlayerScores[i].score}";
        }

        // Extract scores and sort them to determine ranks
        var scores = GameData.PlayerScores.Select(player => player.score).ToList();
        var sortedScores = scores.OrderByDescending(score => score).ToList();

        // Find the highest score (rank 1)
        int highestScore = sortedScores[0];

        // Activate crown only for the player(s) with the highest score
        for (int i = 0; i < GameData.PlayerScores.Count && i < crowns.Length; i++)
        {
            // Check if this player has the highest score
            bool isWinner = (GameData.PlayerScores[i].score == highestScore);
            crowns[i].SetActive(isWinner); // Enable crown if winner

            // Optional: Set animation rank (if needed)
            Animator animator = scoreTexts[i].GetComponentInParent<Animator>();
            if (animator != null)
            {
                animator.SetInteger("Anim", isWinner ? 1 : 2); // 1 for winner, 2+ for others
            }
        }
    }
}