using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventType
{
    None,
    AngryAunty,
    KuihRain,
    SlipperyGround,
    Blackout
}



public class RandomEventManager : MonoBehaviour
{
    [Header("Event Settings")]
    [SerializeField] private float minTimeBetweenEvents = 15f;
    [SerializeField] private float maxTimeBetweenEvents = 30f;
    public static EventType CurrentEvent { get; private set; } = EventType.None;

    [Header("UI Announcement")]
    [SerializeField] private MovingAnnouncementUI announcementUI;

    [Header("Angry Makcik")]
    [SerializeField] private GameObject angryMakcikPrefab;
    [SerializeField] private Transform MakcikSpawnPooint;

    private float nextEventTime;

    private delegate void GameEvent();
    private List<GameEvent> eventList = new List<GameEvent>();

    public GameObject lighting;
    public GameObject firecrackerPrefab; // Reference to the firecracker prefab
    public GameObject photographerPrefab; // Reference to the photographer prefab

    void Start()
    {
        // Register available events
        eventList.Add(AngryAuntyEvent);
        //eventList.Add(KuihRainEvent);
        //eventList.Add(SlipperyGroundEvent);
        //eventList.Add(BlackoutEvent); // 🆕 Blackout
        //eventList.Add(TakingPictureEvent); // 🆕 Taking Picture
        ScheduleNextEvent();
    }


    void Update()
    {
        if (Time.time >= nextEventTime)
        {
            TriggerRandomEvent();
            ScheduleNextEvent();
        }
    }

    private void ScheduleNextEvent()
    {
        float timeUntilNext = Random.Range(minTimeBetweenEvents, maxTimeBetweenEvents);
        nextEventTime = Time.time + timeUntilNext;
    }

    private void TriggerRandomEvent()
    {
        int index = Random.Range(0, eventList.Count);
        eventList[index].Invoke();

        //RemoveFirecrackersIfNeeded(); // Remove firecrackers if needed
    }

    // 🎯 Event Implementations
    private void AngryAuntyEvent()
    {
        CurrentEvent = EventType.AngryAunty; // Set the current event type
        Debug.Log("Event: Angry Aunty");

        if (announcementUI != null)
        {
            announcementUI.StartAnnouncement(
                "💢 ANGRY AUNTY IS COMING! YOU BETTER RUN!! 💨",
                Color.red,
                () =>
                {
                    if (angryMakcikPrefab != null)
                    {
                        Vector2 offset2D = Random.insideUnitCircle * 10f;
                        Vector3 spawnPosition = MakcikSpawnPooint.position + new Vector3(offset2D.x, 0f, offset2D.y);

                        Instantiate(angryMakcikPrefab, spawnPosition, Quaternion.identity);
                    }
                }
            );
        }

        // TODO: spawn angry aunty here
    }

    private void KuihRainEvent()
    {
        CurrentEvent = EventType.KuihRain; // Set the current event type
        Debug.Log("Event: Kuih Rain");

        if (announcementUI != null)
        {
            announcementUI.StartAnnouncement(
                "🍪 FREE KUIH RAIN FROM THE SKY!",
                Color.yellow
            );
        }

        // TODO: spawn falling kuih
    }

    private void SlipperyGroundEvent()
    {
        Debug.Log("Event: Slippery Ground");

        if (announcementUI != null)
        {
            announcementUI.StartAnnouncement(
                "💧 SLIPPERY GROUND! BE CAREFUL!!",
                Color.cyan
               
            );
        }

        // TODO: change player movement physics temporarily
    }

    private void BlackoutEvent()
    {
        CurrentEvent = EventType.Blackout; // Set the current event type
        Debug.Log("Event: Blackout");

        if (announcementUI != null)
        {
            announcementUI.StartAnnouncement(
                "🌑 BLACKOUT! EVERYTHING IS DARK...",    
                Color.black,
                () =>
                {
                    if (lighting != null)
                    {
                        lighting.SetActive(false);
                        for (int i = 0; i < 3; i++)
                        {
                            Vector2 offset = Random.insideUnitCircle.normalized * Random.Range(5f, 15f);
                            Vector3 spawnPos = transform.position + new Vector3(offset.x,0f , offset.y);

                            GameObject cracker = Instantiate(firecrackerPrefab, spawnPos, Quaternion.identity);
                            Debug.Log($"🧨 Spawned firecracker at {spawnPos}");
                        }


                        StartCoroutine(RestoreLightingAfterDelay(15f)); // Turn lights back on after 5 seconds
                    }
                }
            );
        }
    }

    private IEnumerator RestoreLightingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (lighting != null)
        {
            lighting.SetActive(true);
            RemoveFirecrackersIfNeeded(); // Remove firecrackers after blackout
            Debug.Log("Lighting restored after blackout.");
        }
    }

    private void TakingPictureEvent()
    {
        Debug.Log("Event: Photographer Taking Picture");

        CurrentEvent = EventType.None;

        if (announcementUI != null)
        {
            announcementUI.StartAnnouncement(
                "📸 SAY CHEESE! A PHOTOGRAPHER IS HERE!",
                Color.magenta,
                () =>
                {
                    Vector3 spawnOffset = Random.insideUnitCircle.normalized * 10f;
                    Vector3 spawnPos = transform.position + new Vector3(spawnOffset.x, 0f, spawnOffset.y);

                    Instantiate(photographerPrefab, spawnPos, Quaternion.LookRotation(-spawnOffset.normalized));
                }
            );
        }
    }


    private void RemoveFirecrackersIfNeeded()
    {
        if (CurrentEvent != EventType.Blackout)
        {
            Firecracker[] firecrackers = FindObjectsOfType<Firecracker>();
            foreach (Firecracker firecracker in firecrackers)
            {
                Destroy(firecracker.gameObject);
            }

            Debug.Log("🔥 Firecrackers destroyed because the event is not blackout.");
        }
        else
        {
            Debug.Log("🕶️ Firecrackers remain because it's a blackout event.");
        }
    }

    [SerializeField] private float spawnRadius = 10f;
    [SerializeField] private Color gizmoColor = new Color(1f, 0f, 0f, 0.3f); // translucent red

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(transform.position, 0.3f); // Mark center
        Gizmos.DrawWireSphere(transform.position, spawnRadius); // Spawn area
    }

}
