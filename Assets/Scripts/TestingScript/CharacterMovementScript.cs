using UnityEngine;
using Random = UnityEngine.Random;

namespace ProjectDungeonCrawlerPJ15
{

    public class CharacterMovementScript : MonoBehaviour
    {
        [Header("Inspector Changes")] [Range(-50, 50)] [SerializeField]
        private float gravitySlider;

        [Range(0, 30)] private float walkSpeed = 6;

        [SerializeField] private float downValue, upValue;

        /*[SerializeField] private LayerMask collisionLayer;*/
        [SerializeField] private float jumpHeight;
        [SerializeField] private GameObject[] weaponAndShield;
        [TagSelector] [SerializeField] private string thisTag;

        [Header("Internal Edits")] 
        [SerializeField] private Camera _playerCamera;
        public static CharacterController _controller;
        private bool _runFaster;
        [HideInInspector] public static float _mouseSensitivityY;
        [HideInInspector] public static float _mouseSensitivityX;
        private float _normalWalkSpeed;

        private float _mouseXposition,
            _moveHorizontal,
            _moveVertical,
            _mouseYposition;

        private Vector3 _moveDirection;
        private Animator _playersAnimation;
        private float _timeElapsed = 0;
        private float _duration = 3;
        private float idleTimer = 2;
        private float _idleResetTimer;
        private bool _playerIsJumping;
        private float _running;
        private bool _isRunning;
        private bool _weaponKey;
        private int _weaponButtonCounter;
        private float _randomAttack;
        private bool checkAttackNumber;
        private float storedAttackNumber;
        public static bool _playerIsBlocking;
        public static bool _playerIsAttacking;
        private bool _playerIsInAttackRange;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _playersAnimation = GetComponent<Animator>();
        }

        private void Start()
        {
            _running = walkSpeed * 2;
            Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
            Cursor.visible = false;
            _mouseSensitivityY = 0.7f;
            _mouseSensitivityX = 1;
            _normalWalkSpeed = walkSpeed;
            _runFaster = false;
            _idleResetTimer = idleTimer;
        }

        private void FixedUpdate()
        {
            CharacterGravity();
            CharacterMovementBase();
        }

        private void Update()
        {
            JumpMovement();
            RunningMovement();
        }

        private void CharacterMovementBase()
        {

            _mouseXposition += Input.GetAxis("Mouse X") * _mouseSensitivityX;
            _mouseYposition -= Input.GetAxis("Mouse Y") * _mouseSensitivityY;
            _mouseYposition = Mathf.Clamp(_mouseYposition, downValue, upValue);

            transform.rotation = Quaternion.Euler(_mouseYposition, _mouseXposition, 0f);
            _playerCamera.transform.rotation = Quaternion.Euler(_mouseYposition, _mouseXposition, 0f);
            Vector3
                movement = new Vector3(_moveHorizontal, 0f,
                    _moveVertical); // Allows the character to move forwards and backwards & left & right.
            movement = transform.TransformDirection(movement) * walkSpeed; // Gives the character movement speed.
            _controller.Move((movement + _moveDirection) *
                             Time.deltaTime); // Gets all the movement variables and moves the character.
        }


        private void CharacterGravity()
        {
            _moveDirection.y -= gravitySlider * Time.deltaTime;
            // Move the character controller with gravity
            _controller.Move(_moveDirection * Time.deltaTime);
        }

        private void JumpMovement()
        {
            if (_moveDirection.y > 0)
            {
                _playersAnimation.SetBool("IsJumping", true);
            }
            else
            {
                _playersAnimation.SetBool("IsJumping", false);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _playerIsJumping = true;
            }

            if (_playerIsJumping &&
                (_controller.isGrounded)) // If player hits the space bar and the character is touching the ground it will allow the character to jump.
            {
                _moveDirection.y = Mathf.Sqrt(5f * jumpHeight * gravitySlider);
                _moveDirection.y -= gravitySlider * Time.deltaTime;
                _playerIsJumping = false;
            }

        }

        private void RunningMovement()
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            {
                _isRunning = true;
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
            {
                _isRunning = false;
            }

            if (_isRunning)
            {
                walkSpeed = _running;
            }
            else
            {
                walkSpeed = _normalWalkSpeed;
            }
            
        }
    }

}

