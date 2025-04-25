using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(CharacterController))]
public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float punchTimeout = 1f;

    [Header("Punch Hitboxes")]
    [SerializeField] private GameObject rightFistHitboxObject;
    [SerializeField] private GameObject leftFistHitboxObject;

    [Header("Dash Settings")]
    [SerializeField] private float dashDistance = 5f;
    [SerializeField] private float dashCooldown = 2f;
    [SerializeField] private float dashDuration = 0.2f; // Total time to dash
    [SerializeField] private GameObject dashEffect; // Optional dash effect prefab


    [Header("Emote Settings")]
    [SerializeField] private Image emotePlaceHoder;
    [SerializeField] private List<Sprite> emoteSprites;
    [SerializeField] private float emoteDuration = 3f; // Duration for each emote
    [SerializeField] private CanvasGroup emoteCanvasGroup; // Optional canvas group for fading in/out
    [SerializeField] private GameObject playerMarker; // Optional player marker for emote position

    private float emoteFloatSpeed = 1f;
    private float emoteFloatHeight = 0.1f;
    [Header("Emote Display")]
    [SerializeField] private float emoteCooldown = 1f;
    private float lastEmoteTime = -Mathf.Infinity;

    private Vector3 emoteOriginalPos;
    private Quaternion emoteOriginalRot;
    private Vector3 emoteOriginalScale;
    private bool isScalingUp;
    private float emoteScaleTime;
    private float scaleDuration = 0.2f;


    private float emoteDisplayStartTime;
    private bool isEmoteShowing;

    private float lastDashTime;
    private bool isDashing;
    private float dashStartTime;
    private Vector3 dashStartPosition;
    private Vector3 dashEndPosition;

    private float lastPunchTime;
    private bool isPunching;

    private PunchHitbox rightFistHitbox;
    private PunchHitbox leftFistHitbox;
    private PlayerInput playerInput;
    private CharacterController characterController;

    private PlayerFoodConsumer playerFoodConsumer;
    private AudioManager audioManager; // Reference to AudioManager script

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        rightFistHitbox = rightFistHitboxObject.GetComponent<PunchHitbox>();
        leftFistHitbox = leftFistHitboxObject.GetComponent<PunchHitbox>();
        playerInput = GetComponent<PlayerInput>();
        characterController = GetComponent<CharacterController>();

        playerFoodConsumer = GetComponent<PlayerFoodConsumer>();

        emotePlaceHoder.enabled = false; // Hide emote initially

        playerMarker.SetActive(true); // Hide player marker initially
        audioManager = FindAnyObjectByType<AudioManager>();
    }

    public void OnAttack()
    {
        lastPunchTime = Time.time;

        if (!isPunching && !playerFoodConsumer.IsConsuming())
        {
            isPunching = true;
            animator.SetBool("isPunching", true);
        }
    }

    public void OnDash()
    {
        if (Time.time - lastDashTime < dashCooldown || isDashing) return;

        audioManager.PlaySFX(audioManager.Dash);
        lastDashTime = Time.time;
        dashStartTime = Time.time;

        dashStartPosition = transform.position;
        dashEndPosition = transform.position + transform.forward * dashDistance + Vector3.up * 1f; // You can adjust the 1f to control height


        Instantiate(dashEffect, transform.position, Quaternion.identity); // Optional dash effect
        isDashing = true;

        // animator.SetTrigger("Dash"); // optional animation
    }

    private void Update()
    {
        if (isPunching && Time.time - lastPunchTime > punchTimeout)
        {
            isPunching = false;
            animator.SetBool("isPunching", false);
        }

        if (isDashing)
        {
            float elapsed = Time.time - dashStartTime;
            float t = elapsed / dashDuration;

            if (t < 1f)
            {
                Vector3 nextPos = Vector3.Lerp(dashStartPosition, dashEndPosition, t);
                Vector3 movement = nextPos - transform.position;
                characterController.Move(movement);
            }
            else
            {
                isDashing = false;
            }
        }

        if (isEmoteShowing)
        {
            
            float elapsed = Time.time - emoteDisplayStartTime;

            // === Float up/down ===
            float floatOffset = Mathf.Sin(elapsed * emoteFloatSpeed) * emoteFloatHeight;
            emotePlaceHoder.transform.localPosition = emoteOriginalPos + new Vector3(0, floatOffset, 0);

            // === Tilt rotation ===
            float tiltAngle = Mathf.Sin(elapsed * 2f) * 5f;
            emotePlaceHoder.transform.localRotation = Quaternion.Euler(0f, 0f, tiltAngle);

            // === Pop-in scale ===
            if (isScalingUp)
            {
                emoteScaleTime += Time.deltaTime;
                float scaleProgress = Mathf.Clamp01(emoteScaleTime / scaleDuration);
                float easedScale = Mathf.Sin(scaleProgress * Mathf.PI * 0.5f); // easeOutSine
                emotePlaceHoder.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, easedScale);

                if (scaleProgress >= 1f)
                {
                    isScalingUp = false;
                }
            }

            // === Fade-out ===
            float fadeProgress = Mathf.Clamp01(elapsed / emoteDuration);
            if (emoteCanvasGroup != null)
                emoteCanvasGroup.alpha = 1f - fadeProgress;

            // === Done showing? ===
            if (elapsed > emoteDuration)
            {
                emotePlaceHoder.enabled = false;
                isEmoteShowing = false;

                if (emoteCanvasGroup != null)
                    emoteCanvasGroup.alpha = 0f;
            }
        }

        playerMarker.SetActive(!isEmoteShowing); // Show/hide player marker based on emote visibility

    }

    // Animation Events
    public void StartRightPunch() => rightFistHitbox?.StartPunch();
    public void EndRightPunch() => rightFistHitbox?.EndPunch();
    public void StartLeftPunch() => leftFistHitbox?.StartPunch();
    public void EndLeftPunch() => leftFistHitbox?.EndPunch();

    public void OnEmote1() => ShowEmote(0);
    public void OnEmote2() => ShowEmote(1);
    public void OnEmote3() => ShowEmote(2);
    public void OnEmote4() => ShowEmote(3);


    private void ShowEmote(int index)
    {
        if (index < 0 || index >= emoteSprites.Count) return;

        if (Time.time - lastEmoteTime < emoteCooldown) return; // 🔒 Cooldown check

        lastEmoteTime = Time.time; // ✅ Register emote time

        emotePlaceHoder.sprite = emoteSprites[index];
        emotePlaceHoder.enabled = true;

        if (emoteCanvasGroup != null)
            emoteCanvasGroup.alpha = 1f;

        emoteDisplayStartTime = Time.time;
        isEmoteShowing = true;

        // Save base state
        emoteOriginalPos = Vector3.zero;
        emoteOriginalRot = Quaternion.identity;
        emoteOriginalScale = Vector3.zero;
        emotePlaceHoder.transform.localPosition = emoteOriginalPos;
        emotePlaceHoder.transform.localRotation = emoteOriginalRot;
        emotePlaceHoder.transform.localScale = emoteOriginalScale;

        // Start manual pop-in scale
        isScalingUp = true;
        emoteScaleTime = 0f;
    }




}
