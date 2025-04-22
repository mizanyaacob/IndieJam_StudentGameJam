using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEventManager : MonoBehaviour
{
    [Header("Event Settings")]
    [SerializeField] private float minTimeBetweenEvents = 15f;
    [SerializeField] private float maxTimeBetweenEvents = 30f;

    [Header("UI Announcement")]
    [SerializeField] private MovingAnnouncementUI announcementUI;

    private float nextEventTime;

    private delegate void GameEvent();
    private List<GameEvent> eventList = new List<GameEvent>();

    void Start()
    {
        // Register available events
        eventList.Add(AngryAuntyEvent);
        eventList.Add(KuihRainEvent);
        eventList.Add(SlipperyGroundEvent);

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
    }

    // 🎯 Event Implementations
    private void AngryAuntyEvent()
    {
        Debug.Log("Event: Angry Aunty");

        if (announcementUI != null)
        {
            announcementUI.StartAnnouncement(
                "💢 ANGRY AUNTY IS COMING! YOU BETTER RUN!! 💨",
                Color.red
            );
        }

        // TODO: spawn angry aunty here
    }

    private void KuihRainEvent()
    {
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
}
