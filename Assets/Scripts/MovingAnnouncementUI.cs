using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MovingAnnouncementUI : MonoBehaviour
{
    [Header("UI Elements")]
    public RectTransform announcementTransform;
    public TextMeshProUGUI announcementText; // Link to your TMP text
    public Image backgroundImage; // Optional: for coloring based on event

    [Header("Animation Settings")]
    public float moveSpeed = 800f;
    public float stopDuration = 2f;

    private Vector2 startPos;
    private Vector2 midPos;
    private Vector2 endPos;


    private void Start()
    {
        backgroundImage.gameObject.SetActive(false); // Hide background image by default
        gameObject.SetActive(false); // Hide the announcement UI at start
        announcementText.gameObject.SetActive(false); // Ensure text is active
    }
    public void StartAnnouncement(string message, Color textColor, System.Action onComplete = null)
    {
        announcementText.gameObject.SetActive(true);
        backgroundImage.gameObject.SetActive(true);

        gameObject.SetActive(true);
        announcementText.text = message;
        announcementText.color = textColor;

        float canvasWidth = ((RectTransform)announcementTransform.parent).rect.width;

        startPos = new Vector2(canvasWidth + 500, 0);
        midPos = new Vector2(0, 0);
        endPos = new Vector2(-canvasWidth - 500, 0);

        announcementTransform.anchoredPosition = startPos;

        StopAllCoroutines(); // Stop any previous animation
        StartCoroutine(AnimateAnnouncement(onComplete));
    }

    private IEnumerator AnimateAnnouncement(System.Action onComplete)
    {
        while (Vector2.Distance(announcementTransform.anchoredPosition, midPos) > 10f)
        {
            announcementTransform.anchoredPosition = Vector2.MoveTowards(announcementTransform.anchoredPosition, midPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(stopDuration);

        while (Vector2.Distance(announcementTransform.anchoredPosition, endPos) > 10f)
        {
            announcementTransform.anchoredPosition = Vector2.MoveTowards(announcementTransform.anchoredPosition, endPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        gameObject.SetActive(false);

        onComplete?.Invoke(); // ✅ Trigger event logic AFTER announcement ends
    }


    //private IEnumerator AnimateAnnouncement()
    //{
    //    while (Vector2.Distance(announcementTransform.anchoredPosition, midPos) > 10f)
    //    {
    //        announcementTransform.anchoredPosition = Vector2.MoveTowards(announcementTransform.anchoredPosition, midPos, moveSpeed * Time.deltaTime);
    //        yield return null;
    //    }

    //    yield return new WaitForSeconds(stopDuration);

    //    while (Vector2.Distance(announcementTransform.anchoredPosition, endPos) > 10f)
    //    {
    //        announcementTransform.anchoredPosition = Vector2.MoveTowards(announcementTransform.anchoredPosition, endPos, moveSpeed * Time.deltaTime);
    //        yield return null;
    //    }

    //    gameObject.SetActive(false);
    //}
}
