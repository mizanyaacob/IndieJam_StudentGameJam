using UnityEngine;
using TMPro;
using System.Linq;

public class GameOverUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text[] scoreTexts;

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

        // Assign animation ranks
        for (int i = 0; i < GameData.PlayerScores.Count && i < scoreTexts.Length; i++)
        {
            int currentScore = GameData.PlayerScores[i].score;
            int rank = sortedScores.IndexOf(currentScore) + 1; // Add 1 for 1-based ranking

            // Get the Animator from the parent of the scoreText and set the rank
            Animator animator = scoreTexts[i].GetComponentInParent<Animator>();
            if (animator != null)
            {
                animator.SetInteger("Anim", rank);
            }
        }
    }
}