using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ProjectDungeonCrawlerPJ15
{
    public class InputManager : MonoBehaviour
    {
        private PlayerControls playerControls;
        private void Awake()
        {
            playerControls = new PlayerControls();
        }
        private void OnEnable()
        {
            playerControls.Enable();
        }
        private void OnDisable()
        {
            playerControls.Disable();
        }
        public Vector2 GetPlayerMovement()
        {
            return playerControls.Playernormal.Movement.ReadValue<Vector2>();
        }
        public Vector2 GetMouseDelta()
        {
            return playerControls.Playernormal.Look.ReadValue<Vector2>();
        }
        public bool PlayerJumpedThisFrame()
        {
            return playerControls.Playernormal.Jump.triggered;
        }
    }
}
