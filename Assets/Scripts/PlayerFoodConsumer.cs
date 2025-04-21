using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerInput))]
public class PlayerFoodConsumer : MonoBehaviour
{
    [Header("Food Consumption")]
    [SerializeField] private float consumeCooldown = 0.5f;
    [SerializeField] private float eatingTime = 5f;
    private GameObject foodInRange;
    private bool isNearFood = false;
    private float lastConsumeTime;
    private int foodConsumed = 0;
    private bool isConsuming = false;

    [Header("Consuming UI")]
    [SerializeField] private GameObject consumingTimerUI;
    [SerializeField] private Image timerSlider;
    private float currentEatingTime;

    private void Start()
    {
        if (consumingTimerUI != null && timerSlider == null)
            timerSlider = consumingTimerUI.GetComponent<Image>();

        currentEatingTime = eatingTime;

        // Hide the UI at start
        if (consumingTimerUI != null)
            consumingTimerUI.SetActive(false);
    }

    private void Update()
    {
        // Handle the eating timer
        if (isConsuming)
        {
            currentEatingTime -= Time.deltaTime;

            if (timerSlider != null)
                timerSlider.fillAmount = currentEatingTime / eatingTime;

            if (currentEatingTime <= 0)
            {
                currentEatingTime = 0;
                isConsuming = false;

                if (consumingTimerUI != null)
                    consumingTimerUI.SetActive(false);
            }
        }
    }

    // Input Method for Send Message Input System
    public void OnConsume()
    {
        if (Time.time - lastConsumeTime < consumeCooldown) return;

        if (isNearFood && foodInRange != null)
        {
            ConsumeFood();
            lastConsumeTime = Time.time;
            isConsuming = true;
            currentEatingTime = eatingTime;

            if (consumingTimerUI != null)
                consumingTimerUI.SetActive(true);
        }
    }

    private void ConsumeFood()
    {
        // Destroy or disable the food object
        Destroy(foodInRange);
        foodConsumed++;
        Debug.Log($"{gameObject.name} consumed food! Total consumed: {foodConsumed}");
        foodInRange = null;
        isNearFood = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food") && other.GetComponent<KuihProperties>()) 
        {
            isNearFood = true;
            foodInRange = other.gameObject;
            eatingTime = other.GetComponent<KuihProperties>().consumeTime; // Set the eating time from the food properties
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            if (other.gameObject == foodInRange)
            {
                isNearFood = false;
                foodInRange = null;
            }
        }
    }

    public bool IsConsuming()
    {
        return isConsuming;
    }

    public void SetIsConsuming(bool value)
    {
        isConsuming = value;
        if (!value && consumingTimerUI != null)
            consumingTimerUI.SetActive(false);
    }
}