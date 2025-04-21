using UnityEngine;
using UnityEngine.UI;

public class EatingTimer : MonoBehaviour
{
    [SerializeField] private float eatingTime = 5f;
    [SerializeField] private Image slider;

    private float currentTime;
    private bool timerRunning = false;

    public PlayerFoodConsumer playerFoodConsumer;

    private void Start()
    {
        if (slider == null)
            slider = GetComponent<Image>();

        // Get the PlayerFoodConsumer in parent
        playerFoodConsumer = GetComponentInParent<PlayerFoodConsumer>();

        currentTime = eatingTime;
        gameObject.SetActive(false); // Hide at start
    }

    private void Update()
    {
        // Only run the timer if player is consuming
        if (playerFoodConsumer != null && playerFoodConsumer.IsConsuming())
        {
            if (!timerRunning)
            {
                timerRunning = true;
                currentTime = eatingTime;
                gameObject.SetActive(true); // Show UI
            }

            currentTime -= Time.deltaTime;
            slider.fillAmount = currentTime / eatingTime;

            if (currentTime <= 0)
            {
                currentTime = 0;
                playerFoodConsumer.SetIsConsuming(false);
                timerRunning = false;
                gameObject.SetActive(false); // Hide UI after done
            }
        }
    }
}
