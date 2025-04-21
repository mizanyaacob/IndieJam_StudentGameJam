using UnityEngine;
using UnityEngine.InputSystem;

public class FoodConsumer : MonoBehaviour
{
    [Header("Consume Settings")]
    [SerializeField] private LayerMask foodLayer;

    [Header("Score")]
    [SerializeField] private int currentScore = 0;

    private PlayerInput playerInput;
    private InputAction consumeAction;
    private ConsumableItem currentFoodItem;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        consumeAction = playerInput.actions["Interact"];

        // Add a trigger collider if none exists
        if (GetComponent<Collider>() == null)
        {
            var collider = gameObject.AddComponent<SphereCollider>();
            collider.isTrigger = true;
            collider.radius = 1.5f; // Adjust radius as needed
            Debug.Log("Added trigger collider to FoodConsumer");
        }
        else if (!GetComponent<Collider>().isTrigger)
        {
            GetComponent<Collider>().isTrigger = true;
            Debug.Log("Converted existing collider to trigger");
        }
    }

    private void OnEnable()
    {
        consumeAction.performed += OnConsume;
    }

    private void OnDisable()
    {
        consumeAction.performed -= OnConsume;
    }

    private void OnConsume(InputAction.CallbackContext context)
    {
        TryConsumeFood();
    }

    private void TryConsumeFood()
    {
        if (currentFoodItem != null)
        {
            ConsumeCurrentItem();
        }
        else
        {
            Debug.Log("No food in range to consume");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider is on the food layer and has a ConsumableItem component
        if (((1 << other.gameObject.layer) & foodLayer) != 0)
        {
            var foodItem = other.GetComponent<ConsumableItem>();
            if (foodItem != null)
            {
                currentFoodItem = foodItem;
                Debug.Log($"Food in range: {currentFoodItem.itemName} (Press interact to consume)");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (currentFoodItem != null && other.gameObject == currentFoodItem.gameObject)
        {
            Debug.Log($"Food left range: {currentFoodItem.itemName}");
            currentFoodItem = null;
        }
    }

    private void ConsumeCurrentItem()
    {
        if (currentFoodItem != null)
        {
            // Call the consumption method on the food item
            currentFoodItem.OnConsumed(this);
            currentFoodItem = null; // Clear reference after consumption
        }
    }

    public void AddToScore(int points)
    {
        currentScore += points;
        Debug.Log($"Consumed! Current Score: {currentScore}");
        // Update UI or game manager here
    }
}