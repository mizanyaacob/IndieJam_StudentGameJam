using System.Collections.Generic;
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

    // List to keep track of all food items in range
    private List<GameObject> foodItemsInRange = new List<GameObject>();

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

        // If we don't have a current food item, but there are food items in range,
        // select the closest one
        if (!isNearFood && foodItemsInRange.Count > 0)
        {
            SelectClosestFood();
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
        // Remove from our tracking list before destroying
        foodItemsInRange.Remove(foodInRange);

        // Destroy or disable the food object
        Destroy(foodInRange);
        foodConsumed++;
        Debug.Log($"{gameObject.name} consumed food! Total consumed: {foodConsumed}");

        // Reset current food reference
        foodInRange = null;
        isNearFood = false;

        // After consuming, check if there's another food in range to focus on
        SelectClosestFood();
    }

    private void SelectClosestFood()
    {
        // Clean up any null references that might exist in our list
        foodItemsInRange.RemoveAll(item => item == null);

        if (foodItemsInRange.Count == 0)
        {
            foodInRange = null;
            isNearFood = false;
            return;
        }

        // Find the closest food item
        GameObject closest = null;
        float closestDistance = float.MaxValue;

        foreach (GameObject food in foodItemsInRange)
        {
            if (food != null)
            {
                float distance = Vector3.Distance(transform.position, food.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = food;
                }
            }
        }

        // Set the closest as our current target
        if (closest != null)
        {
            foodInRange = closest;
            isNearFood = true;

            // Update eating time based on the food's properties
            KuihProperties kuihProps = closest.GetComponent<KuihProperties>();
            if (kuihProps != null)
            {
                eatingTime = kuihProps.consumeTime;
            }

            Debug.Log($"Selected {foodInRange.name} as the current food to consume");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food") && other.GetComponent<KuihProperties>())
        {
            // Add this food to our tracking list
            if (!foodItemsInRange.Contains(other.gameObject))
            {
                foodItemsInRange.Add(other.gameObject);
                Debug.Log($"Added {other.gameObject.name} to food items in range. Total: {foodItemsInRange.Count}");
            }

            // If we don't currently have a food in focus, select this one
            if (!isNearFood || foodInRange == null)
            {
                foodInRange = other.gameObject;
                isNearFood = true;
                eatingTime = other.GetComponent<KuihProperties>().consumeTime;
                Debug.Log($"Selected {foodInRange.name} as the current food to consume");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            // Remove from our tracking list
            if (foodItemsInRange.Contains(other.gameObject))
            {
                foodItemsInRange.Remove(other.gameObject);
                Debug.Log($"Removed {other.gameObject.name} from food items in range. Total: {foodItemsInRange.Count}");
            }

            // If this was our current focus, find a new one
            if (other.gameObject == foodInRange)
            {
                foodInRange = null;
                isNearFood = false;
                SelectClosestFood();
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

    // Debug method to show which food item is currently focused
    public string GetCurrentFoodName()
    {
        return foodInRange != null ? foodInRange.name : "None";
    }
}