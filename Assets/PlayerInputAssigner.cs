using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class PlayerInputAssigner : MonoBehaviour
{
    [SerializeField] private GameObject[] playerCharacters; // List of player character prefabs or instances

    private void Start()
    {
        AssignControllersToPlayers();
    }

    private void AssignControllersToPlayers()
    {
        var gamepads = Gamepad.all;

        for (int i = 0; i < playerCharacters.Length; i++)
        {
            if (i >= gamepads.Count)
            {
                Debug.LogWarning($"Not enough gamepads for player {i + 1}");
                continue;
            }

            var playerObj = playerCharacters[i];
            var input = playerObj.GetComponent<PlayerInput>();

            if (input == null)
            {
                Debug.LogError($"Player {i + 1} has no PlayerInput component.");
                continue;
            }

            // Assign the device to the player
            InputUser user = InputUser.PerformPairingWithDevice(gamepads[i], input.user);

            // Optional: assign player index if you're using that
            //input.playerIndex = i;

            Debug.Log($"Assigned Gamepad {i + 1} to Player {i + 1}");
        }
    }
}
