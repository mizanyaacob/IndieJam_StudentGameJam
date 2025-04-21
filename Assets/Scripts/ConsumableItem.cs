using UnityEngine;

public class ConsumableItem : MonoBehaviour
{
    [Header("Item Identification")]
    [Tooltip("Name of this food/kuih item")]
    public string itemName = "Food";

    [Header("Consumption Settings")]
    [Tooltip("Score added when consumed")]
    public int scoreValue = 1;

    [Tooltip("Time in seconds required to consume")]
    public float consumeTime = 1f;

    [Header("Visual/Audio Effects")]
    [Tooltip("Particle effect to play when consumed")]
    public GameObject consumeEffect;

    [Tooltip("Sound to play when consumed")]
    public AudioClip consumeSound;

    [Header("Advanced Settings")]
    [Tooltip("Should this item be destroyed after consumption?")]
    public bool destroyOnConsume = true;

    public void OnConsumed(FoodConsumer consumer)
    {
        // Add score to the consumer
        if (consumer != null)
        {
            consumer.AddToScore(scoreValue);
        }

        // Play effects if they exist
        if (consumeEffect != null)
        {
            Instantiate(consumeEffect, transform.position, Quaternion.identity);
        }

        if (consumeSound != null)
        {
            AudioSource.PlayClipAtPoint(consumeSound, transform.position);
        }

        // Destroy this food item
        Destroy(gameObject);
    }

    // You can add additional kuih-specific methods here if needed
    public float GetConsumeTime()
    {
        return consumeTime;
    }

    public string GetItemName()
    {
        return itemName;
    }
}