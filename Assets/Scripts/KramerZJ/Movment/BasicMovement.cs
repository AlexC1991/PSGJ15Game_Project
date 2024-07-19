using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ProjectDungeonCrawlerPJ15
{
    public class BasicMovement : MonoBehaviour
    {
        
        [Header("Movement")]
        [SerializeField] private Rigidbody _rb;
        [SerializeField, Range(0, 50f)] private float _speed = 7f;
        [SerializeField, Range(0, 50f)] private float _walkSpeed = 7f;
        [SerializeField, Range(0, 50f)] private float _sprintSpeed = 10f;
        [SerializeField, Range(-20f, 0)] private float _gravity = -9.8f;
        [SerializeField, Range(0.4f, 1f)] private float _checkLength = 0.1f;
        [SerializeField, Range(0, 20f)] private float _jumpForce = 12f;
        [SerializeField, Range(0, 1f)] private float airMultiplier = 0.4f;
        [SerializeField, Range(0, 10f)] private float _groundDrag = 3;
        [SerializeField, Range(0f,20f)] private float _horsePower = 10f;
        [Header("Crouching")]
        [SerializeField, Range(0f, 7f)] private float _crouchSpeed = 3.5f;
        [SerializeField, Range(0f, 0.7f)] private float _crouchYScale = 0.5f;
        private float _startYScale;
        [Header("Ground Check")]
        [SerializeField] private Transform _groundCheck;
        [SerializeField] private LayerMask _groundMask;
        private bool _isGround;
         private float _mouseXposition;
         private float _mouseYposition;
         private float _mouseSensitivityX;
         private float _mouseSensitivityY;
        [Header("Camera")]
        [SerializeField] private float downValue;
        [SerializeField] private float upValue;
        [SerializeField] private Camera _playerCamera;

        [Header("Input")]
        private Transform _orientation;
        private float _moveHorizontal;
        private float _moveVertical;
        private Vector3 _moveDirection;

        
        [Header("Keybinds")]
        public KeyCode jumpKey = KeyCode.Space;
        public KeyCode sprintKey = KeyCode.LeftShift;
        public KeyCode crouchKey = KeyCode.LeftControl;


        public bool isGrounded { get => _isGround; }
        public enum MovementState
        {
            walking,
            springting,
            crouching,
            air
        }
        public MovementState state;

        // Update is called once per frame

        private void Start()
        {
            _rb = GetComponentInParent<Rigidbody>();
            _rb.freezeRotation = true;

            _mouseSensitivityY = 0.7f;
            _mouseSensitivityX = 1;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            _orientation = _playerCamera.transform;

            _startYScale = transform.localScale.y;
        }

        void Update()
        {
            _isGround = Physics.CheckSphere(_groundCheck.position, _checkLength, _groundMask);
            
            MyInput();
            SpeedControl();
            StateHandler();
            // handle drag
            if (_isGround)
                _rb.drag = _groundDrag;
            else
                _rb.drag = _groundDrag/2;
        }

        private void MyInput()
        {
            HandleCamera();
            _moveHorizontal = Input.GetAxis("Horizontal");
            _moveVertical = Input.GetAxis("Vertical");
            // when to jump
            if (Input.GetKeyUp(jumpKey)  && _isGround)
            {
                Jump();
            }
            //start crouch
            if (Input.GetKeyDown(crouchKey))
            {
                transform.localScale = new Vector3(transform.localScale.x, _crouchYScale, transform.localScale.z);
                _rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
            }
            //stop crouch
            if (Input.GetKeyUp(crouchKey))
            {
                transform.localScale = new Vector3(transform.localScale.x, _startYScale, transform.localScale.z);
            }
        }
        private void Jump()
        {
            // reset y velocity
            _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

            _rb.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
        }
        private void HandleCamera()
        {
            _mouseXposition += Input.GetAxis("Mouse X") * _mouseSensitivityX;
            _mouseYposition -= Input.GetAxis("Mouse Y") * _mouseSensitivityY;
            _mouseYposition = Mathf.Clamp(_mouseYposition, downValue, upValue);

            transform.rotation = Quaternion.Euler(_mouseYposition, _mouseXposition, 0f);
            _playerCamera.transform.rotation = Quaternion.Euler(_mouseYposition, _mouseXposition, 0f);
        }

        private void FixedUpdate()
        {
            MovePlayer();
        }
        private void MovePlayer()
        {
            // calculate movement direction
            _moveDirection = _orientation.forward * _moveVertical + _orientation.right * _moveHorizontal;

            // on ground
            if (_isGround)
                _rb.AddForce(_moveDirection.normalized * _speed * _horsePower, ForceMode.Force);

            // in air
            else if (!_isGround)
                _rb.AddForce(_moveDirection.normalized * _speed * 10f * airMultiplier, ForceMode.Force);

            //handle falling
            if (!_isGround)
                _rb.AddForce(transform.up * .25f * -_jumpForce, ForceMode.Acceleration);
        }
        private void SpeedControl()
        {
            Vector3 flatVel = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > _speed)
            {
                Vector3 limitedVel = flatVel.normalized * _speed;
                _rb.velocity = new Vector3(limitedVel.x, _rb.velocity.y, limitedVel.z);
            }
        }
        private void StateHandler()
        {
            if (Input.GetKey(crouchKey))
            {
                state = MovementState.crouching;
                _speed = _crouchSpeed;
                return;
            }

            if (_isGround&&Input.GetKey(sprintKey))
            {
                state = MovementState.springting;
                _speed = _sprintSpeed;
            }else if (!Input.GetKey(sprintKey))
            {
                state = MovementState.walking;
                _speed = _walkSpeed;
            }
            else
            {
                state = MovementState.air;
            }
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_groundCheck.position, _checkLength);
        }
    }
}
