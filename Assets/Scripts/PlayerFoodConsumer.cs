using System.Collections;
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

    [Header("Inverted Control Settings")]
    [SerializeField] private float invertedDuration = 5f;
    [SerializeField] private string normalActionMap = "Player";
    [SerializeField] private string invertedActionMap = "Inverted";
    [SerializeField] private GameObject invertedEffect;
    [SerializeField] private Transform effectPoint; // Optional vomit effect prefab
    [SerializeField] private GameObject PlayerMarker; // Optional player marker for emote position

    [Header("PLayer Score")]
    [SerializeField] private int playerIndex = 0; // Assuming player index is set somewhere else

    private ScoringManager scoringManager;

    private List<GameObject> foodItemsInRange = new List<GameObject>();
    private PlayerInput playerInput;
    private Coroutine invertCoroutine;

    private void Start()
    {
        if (consumingTimerUI != null && timerSlider == null)
            timerSlider = consumingTimerUI.GetComponent<Image>();

        currentEatingTime = eatingTime;

        if (consumingTimerUI != null)
            consumingTimerUI.SetActive(false);

        playerInput = GetComponent<PlayerInput>();
        scoringManager = FindObjectOfType<ScoringManager>();
    }

    private void Update()
    {
        if (isConsuming)
        {
            currentEatingTime -= Time.deltaTime;
            if (timerSlider != null)
                timerSlider.fillAmount = currentEatingTime / eatingTime;

            if (currentEatingTime <= 0)
            {
                currentEatingTime = 0;
                isConsuming = false;

                // ✅ Score gets added after eating completes
                if (foodInRange != null)
                {
                    KuihProperties kuihProps = foodInRange.GetComponent<KuihProperties>();
                    if (kuihProps != null)
                    {
                        if (kuihProps.CurrentState == KuihProperties.KuihState.Bad)
                        {
                            Debug.Log($"{gameObject.name} finished consuming a BAD kuih! Switching to Inverted controls.");

                            if (playerInput != null && playerInput.currentActionMap.name != invertedActionMap)
                            {
                                if (invertCoroutine != null)
                                    StopCoroutine(invertCoroutine);

                                GameObject vomitEffect = Instantiate(invertedEffect, effectPoint.position, Quaternion.identity);
                                vomitEffect.transform.SetParent(effectPoint);
                                invertCoroutine = StartCoroutine(TemporarilyInvertControls(vomitEffect));
                            }
                        }

                        scoringManager.AddScore(playerIndex, kuihProps.scoreValue);
                    }

                    foodItemsInRange.Remove(foodInRange);
                    Destroy(foodInRange);
                    foodConsumed++;

                    Debug.Log($"{gameObject.name} finished consuming food! Total consumed: {foodConsumed}");

                    foodInRange = null;
                    isNearFood = false;

                    SelectClosestFood();
                }

                if (consumingTimerUI != null)
                    consumingTimerUI.SetActive(false);
            }
        }

        if (!isNearFood && foodItemsInRange.Count > 0)
        {
            SelectClosestFood();
        }
    }


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
        if (foodInRange == null) return;

        isConsuming = true;
        currentEatingTime = eatingTime;

        if (consumingTimerUI != null)
            consumingTimerUI.SetActive(true);

        Debug.Log($"{gameObject.name} started consuming {foodInRange.name}");
    }



    private IEnumerator TemporarilyInvertControls(GameObject effect)
    {
        PlayerMarker.SetActive(false); // ✅ Hide player marker during inverted controls
        playerInput.SwitchCurrentActionMap(invertedActionMap);
        yield return new WaitForSeconds(invertedDuration);
        playerInput.SwitchCurrentActionMap(normalActionMap);
        Debug.Log($"{gameObject.name} controls reverted to normal.");
        Destroy(effect);
        PlayerMarker.SetActive(true); // ✅ Show player marker again
    }


    private void SelectClosestFood()
    {
        foodItemsInRange.RemoveAll(item => item == null);
        if (foodItemsInRange.Count == 0)
        {
            foodInRange = null;
            isNearFood = false;
            return;
        }

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

        if (closest != null)
        {
            foodInRange = closest;
            isNearFood = true;

            KuihProperties kuihProps = closest.GetComponent<KuihProperties>();
            if (kuihProps != null)
                eatingTime = kuihProps.consumeTime;

            Debug.Log($"Selected {foodInRange.name} as the current food to consume");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food") && other.GetComponent<KuihProperties>())
        {
            if (!foodItemsInRange.Contains(other.gameObject))
            {
                foodItemsInRange.Add(other.gameObject);
                Debug.Log($"Added {other.gameObject.name} to food items in range. Total: {foodItemsInRange.Count}");
            }

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
            if (foodItemsInRange.Contains(other.gameObject))
            {
                foodItemsInRange.Remove(other.gameObject);
                Debug.Log($"Removed {other.gameObject.name} from food items in range. Total: {foodItemsInRange.Count}");
            }

            if (other.gameObject == foodInRange)
            {
                foodInRange = null;
                isNearFood = false;
                SelectClosestFood();
            }
        }
    }

    public bool IsConsuming() => isConsuming;

    public void SetIsConsuming(bool value)
    {
        isConsuming = value;
        if (!value && consumingTimerUI != null)
            consumingTimerUI.SetActive(false);
    }

    public string GetCurrentFoodName()
    {
        return foodInRange != null ? foodInRange.name : "None";
    }

    // Add this to PlayerFoodConsumer.cs
    public int GetFoodConsumed()
    {
        return foodConsumed;
    }
}
