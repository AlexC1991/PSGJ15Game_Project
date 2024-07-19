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
        [SerializeField, Range(0, 50f)] private float _speed = 7f;
        [SerializeField, Range(-20f, 0)] private float _gravity = -9.8f;
        [SerializeField, Range(0.4f, 1f)] private float _checkLength = 0.1f;
        [SerializeField, Range(0, 20f)] private float _jumpForce = 12f;
        [SerializeField, Range(0, 1f)] private float _jumpTime = 0.3f;
        [SerializeField, Range(0f, 1f)] private float _jumpCooldown = 0.3f;
        [SerializeField, Range(0f, 10f)] private float _jumpHeight = 4f;
        [SerializeField, Range(0, 1f)] private float _airControl = 0.4f;
        [SerializeField, Range(0, 10f)] private float _groundDrag = 3f;
        [SerializeField, Range(0f, 5f)] private float _airDrag = 0.5f;
        [SerializeField, Range(0f,20f)] private float _horsePower = 10f;

  
        private float fallingTime = 0f;
        private Vector3 _velocity;

        [Header("Ground Check")]
        [SerializeField] private Transform _groundCheck;
        [SerializeField] private LayerMask _groundMask;
        private bool _isGround;

        [Header("Slope Handling")]
        [SerializeField, Range(0f, 7f)] private float maxSlopeAngle;
        [SerializeField, Range(0f, 7f)] private RaycastHit slopeHit;

        private float _mouseXposition;
         private float _mouseYposition;
         private float _mouseSensitivityX;
         private float _mouseSensitivityY;
        [Header("Camera")]
        [SerializeField] private float downValue;
        [SerializeField] private float upValue;
        [SerializeField] private Camera _playerCamera;
        [SerializeField] private Transform _cameraFollow;

        [Header("Input")]
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
            onGround,
            OnWall,
        }
        public MovementState state;
        
        [SerializeField]private bool _readyToJump = true;

        // Update is called once per frame

        private void Start()
        {

            _mouseSensitivityY = 0.7f;
            _mouseSensitivityX = 1;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

        }

        void Update()
        {
            _isGround = Physics.CheckSphere(_groundCheck.position, _checkLength, _groundMask);
            
            MyInput();
            SpeedControl();

            //TODO: handle drag somewhere 
        }

        private void MyInput()
        {
            HandleCamera();
            _moveHorizontal = Input.GetAxis("Horizontal");
            _moveVertical = Input.GetAxis("Vertical");
            // when to jump
            if (Input.GetKey(jumpKey)  && _isGround&& _readyToJump)
            {
                _readyToJump = false;
                StartCoroutine(Jump());
                Invoke(nameof(ResetJump), _jumpCooldown);
            }
        }

        private void ResetJump()
        {
            _readyToJump = true;
        }


        private IEnumerator Jump()
        {
            float timer = 0;
            float jumpFactor = _jumpTime / 30;
            while (timer< _jumpTime && jumpFactor< _jumpTime)
            {
                transform.position = transform.position + Vector3.up * _jumpForce* -Mathf.Log(jumpFactor/ _jumpTime,30);//gravity is a negative number
                timer += Time.fixedDeltaTime;
                jumpFactor+= Time.fixedDeltaTime;
                yield return new WaitForEndOfFrame();
            }
            
        }
        private void HandleCamera()
        {
            _mouseXposition += Input.GetAxis("Mouse X") * _mouseSensitivityX;
            _mouseYposition -= Input.GetAxis("Mouse Y") * _mouseSensitivityY;
            _mouseYposition = Mathf.Clamp(_mouseYposition, downValue, upValue);

            transform.rotation = Quaternion.Euler(0f, _mouseXposition, 0f);
            _cameraFollow.rotation = Quaternion.Euler(_mouseYposition, _mouseXposition, 0f);
            _playerCamera.transform.rotation = Quaternion.Euler(_mouseYposition, _mouseXposition, 0f);
        }

        private void FixedUpdate()
        {
            MovePlayer();
        }
        private void MovePlayer()
        {
            // calculate movement direction
            _moveDirection = transform.forward * _moveVertical + transform.right * _moveHorizontal;
            

            if (OnSlope())
            {
                //_rb.AddForce(GetSlopeMoveDirection() * _speed * _horsePower, ForceMode.Force);

                //if (_rb.velocity.y > 0) _rb.AddForce(Vector3.down * 8f * _horsePower, ForceMode.Force);
                transform.position = transform.position + GetSlopeMoveDirection() *_speed * Time.fixedDeltaTime;
            }

            // on ground
            if (_isGround)
            //_rb.AddForce(_moveDirection.normalized * _speed * _horsePower, ForceMode.Force);
            {
                transform.position = transform.position + _moveDirection * _speed * Time.fixedDeltaTime;
                fallingTime = 0;
            }
            // in air
            else if (!_isGround)
            {
                transform.position = transform.position + _moveDirection * _speed * _airControl * Time.fixedDeltaTime;
                //handle falling
                fallingTime += Time.fixedDeltaTime;
                if (fallingTime>0)
                {
                    transform.position = transform.position + Vector3.up * _gravity * Mathf.Log10(fallingTime+1);//gravity is a negative number
                }
            }
            


            //TODO: disable gravity on slope if needs
        }
        private void SpeedControl()
        {
            // limiting speed on slope
            if (OnSlope())
            {
                //TODO: Handle speed in transform
            }
            else// limiting speed on gournd or air
            {
                //TODO: Handle speed in transform
            }
        }
        private bool OnSlope()
        {
            if (Physics.Raycast(_groundCheck.position, Vector3.down, out slopeHit, 0.3f, _groundMask))
            {
                float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
                return angle < maxSlopeAngle && angle != 0;
            }
            return false;
        }
        private Vector3 GetSlopeMoveDirection()
        {
            return Vector3.ProjectOnPlane(_moveDirection, slopeHit.normal).normalized;
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_groundCheck.position, _checkLength);
        }
    }
}
