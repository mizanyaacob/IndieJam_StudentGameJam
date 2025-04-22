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
    }
    public void StartAnnouncement(string message, Color textColor)
    {
        gameObject.SetActive(true);
        announcementText.text = message;
        announcementText.color = textColor;
        //if (backgroundImage != null)
            //backgroundImage.color = bgColor;

        float canvasWidth = ((RectTransform)announcementTransform.parent).rect.width;

        startPos = new Vector2(canvasWidth + 500, 0);
        midPos = new Vector2(0, 0);
        endPos = new Vector2(-canvasWidth - 500, 0);

        announcementTransform.anchoredPosition = startPos;

        StopAllCoroutines(); // In case something was already running
        StartCoroutine(AnimateAnnouncement());
    }

    private IEnumerator AnimateAnnouncement()
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
    }
}
