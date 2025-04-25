using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(CharacterController))]
public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private PlayerFoodConsumer playerFoodConsumer;

    [Header("Punch Settings")]
    [SerializeField] private float punchTimeout = 1f;
    [SerializeField] private GameObject rightFistHitboxObject;
    [SerializeField] private GameObject leftFistHitboxObject;

    [Header("Dash Settings")]
    [SerializeField] private float dashDistance = 5f;
    [SerializeField] private float dashCooldown = 2f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private GameObject dashEffect;

    [Header("Emote Settings")]
    [SerializeField] private Image emotePlaceholder;
    [SerializeField] private List<Sprite> emoteSprites;
    [SerializeField] private float emoteDuration = 3f;
    [SerializeField] private float emoteCooldown = 1f;
    [SerializeField] private CanvasGroup emoteCanvasGroup;
    [SerializeField] private GameObject playerMarker;

    [Header("Emote Animation")]
    [SerializeField] private float emoteFloatSpeed = 1f;
    [SerializeField] private float emoteFloatHeight = 0.1f;
    [SerializeField] private float emoteScaleDuration = 0.2f;

    // Punch
    private float lastPunchTime;
    private bool isPunching;
    private PunchHitbox rightFistHitbox;
    private PunchHitbox leftFistHitbox;

    // Dash
    private float lastDashTime;
    private bool isDashing;
    private float dashStartTime;
    private Vector3 dashStartPosition;
    private Vector3 dashEndPosition;

    // Emote
    private float lastEmoteTime = -Mathf.Infinity;
    private bool isEmoteShowing;
    private float emoteDisplayStartTime;
    private Vector3 emoteOriginalPos;
    private Quaternion emoteOriginalRot;
    private bool isScalingUp;
    private float emoteScaleTime;

    private AudioManager audioManager;

    private void Awake()
    {
        // Initialize references
        if (!animator) animator = GetComponent<Animator>();
        if (!characterController) characterController = GetComponent<CharacterController>();
        if (!playerFoodConsumer) playerFoodConsumer = GetComponent<PlayerFoodConsumer>();

        rightFistHitbox = rightFistHitboxObject.GetComponent<PunchHitbox>();
        leftFistHitbox = leftFistHitboxObject.GetComponent<PunchHitbox>();

        audioManager = FindObjectOfType<AudioManager>();

        // Initialize emote
        emotePlaceholder.enabled = false;
        if (playerMarker) playerMarker.SetActive(false);
    }

    private void Update()
    {
        HandlePunchTimeout();
        HandleDashMovement();
        HandleEmoteAnimation();
    }

    private void HandlePunchTimeout()
    {
        if (isPunching && Time.time - lastPunchTime > punchTimeout)
        {
            isPunching = false;
            animator.SetBool("isPunching", false);
        }
    }

    private void HandleDashMovement()
    {
        if (!isDashing) return;

        float elapsed = Time.time - dashStartTime;
        float t = elapsed / dashDuration;

        if (t < 1f)
        {
            Vector3 nextPos = Vector3.Lerp(dashStartPosition, dashEndPosition, t);
            characterController.Move(nextPos - transform.position);
        }
        else
        {
            isDashing = false;
        }
    }

    private void HandleEmoteAnimation()
    {
        if (!isEmoteShowing) return;

        float elapsed = Time.time - emoteDisplayStartTime;

        // Float up/down
        float floatOffset = Mathf.Sin(elapsed * emoteFloatSpeed) * emoteFloatHeight;
        emotePlaceholder.transform.localPosition = emoteOriginalPos + new Vector3(0, floatOffset, 0);

        // Tilt rotation
        float tiltAngle = Mathf.Sin(elapsed * 2f) * 5f;
        emotePlaceholder.transform.localRotation = Quaternion.Euler(0f, 0f, tiltAngle);

        // Pop-in scale
        if (isScalingUp)
        {
            emoteScaleTime += Time.deltaTime;
            float scaleProgress = Mathf.Clamp01(emoteScaleTime / emoteScaleDuration);
            float easedScale = Mathf.Sin(scaleProgress * Mathf.PI * 0.5f);
            emotePlaceholder.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, easedScale);

            if (scaleProgress >= 1f) isScalingUp = false;
        }

        // Fade-out
        if (emoteCanvasGroup)
            emoteCanvasGroup.alpha = 1f - Mathf.Clamp01(elapsed / emoteDuration);

        // End emote display
        if (elapsed > emoteDuration)
        {
            emotePlaceholder.enabled = false;
            isEmoteShowing = false;
            if (playerMarker) playerMarker.SetActive(true);
        }
    }

    // === Input Callbacks ===
    public void OnAttack()
    {
        if (isPunching || playerFoodConsumer.IsConsuming()) return;

        lastPunchTime = Time.time;
        isPunching = true;
        animator.SetBool("isPunching", true);
    }

    public void OnDash()
    {
        if (Time.time - lastDashTime < dashCooldown || isDashing) return;

        audioManager?.PlaySFX(audioManager.Dash);
        lastDashTime = Time.time;
        dashStartTime = Time.time;

        dashStartPosition = transform.position;
        dashEndPosition = transform.position + transform.forward * dashDistance;

        if (dashEffect) Instantiate(dashEffect, transform.position, Quaternion.identity);
        isDashing = true;
    }

    public void OnEmote1() => ShowEmote(0);
    public void OnEmote2() => ShowEmote(1);
    public void OnEmote3() => ShowEmote(2);
    public void OnEmote4() => ShowEmote(3);

    private void ShowEmote(int index)
    {
        if (index < 0 || index >= emoteSprites.Count || Time.time - lastEmoteTime < emoteCooldown)
            return;

        lastEmoteTime = Time.time;

        // Play emote SFX
        switch (index)
        {
            case 0: audioManager?.PlaySFX(audioManager.EmoteAngry); break;
            case 1: audioManager?.PlaySFX(audioManager.EmoteHappy); break;
            case 2: audioManager?.PlaySFX(audioManager.EmoteSad); break;
            case 3: audioManager?.PlaySFX(audioManager.EmoteWow); break;
        }

        // Set emote sprite
        emotePlaceholder.sprite = emoteSprites[index];
        emotePlaceholder.enabled = true;
        if (playerMarker) playerMarker.SetActive(false);

        // Reset animation state
        emoteOriginalPos = Vector3.zero;
        emoteOriginalRot = Quaternion.identity;
        emotePlaceholder.transform.localPosition = emoteOriginalPos;
        emotePlaceholder.transform.localRotation = emoteOriginalRot;
        emotePlaceholder.transform.localScale = Vector3.zero;

        // Start animation
        isEmoteShowing = true;
        isScalingUp = true;
        emoteScaleTime = 0f;
        emoteDisplayStartTime = Time.time;

        if (emoteCanvasGroup) emoteCanvasGroup.alpha = 1f;
    }

    // === Animation Events ===
    public void StartRightPunch() => rightFistHitbox?.StartPunch();
    public void EndRightPunch() => rightFistHitbox?.EndPunch();
    public void StartLeftPunch() => leftFistHitbox?.StartPunch();
    public void EndLeftPunch() => leftFistHitbox?.EndPunch();
}