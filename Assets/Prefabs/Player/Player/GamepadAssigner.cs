using UnityEngine;
using UnityEngine.InputSystem;

public class GamepadAssigner : MonoBehaviour
{
    public PlayerInput playerInput; // Reference to PlayerInput on the character
    public int playerIndex; // Index of the player (0, 1, 2...)

    void Start()
    {
        // Automatically find PlayerInput if not assigned
        if (playerInput == null)
        {
            playerInput = GetComponent<PlayerInput>();
        }

        AssignControlScheme();
    }

    private void AssignControlScheme()
    {
        // Get all connected gamepads
        var gamepads = Gamepad.all;

        if (playerIndex < gamepads.Count)
        {
            // Assign the gamepad at the given index
            playerInput.SwitchCurrentControlScheme("Gamepad", gamepads[playerIndex]);
            Debug.Log($"Player {playerIndex} assigned to Gamepad: {gamepads[playerIndex].displayName}");
        }
        else
        {
            // Fallback to Keyboard & Mouse
            var keyboard = Keyboard.current;
            var mouse = Mouse.current;

            if (keyboard != null && mouse != null)
            {
                playerInput.SwitchCurrentControlScheme("Keyboard&Mouse", keyboard, mouse);
                Debug.Log($"Player {playerIndex} assigned to Keyboard & Mouse");
            }
            else
            {
                Debug.LogError("No suitable input device found for player " + playerIndex);
            }
        }
    }
}
