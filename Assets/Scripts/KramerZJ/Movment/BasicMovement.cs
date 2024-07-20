using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
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
        [SerializeField, Range(0.1f, 1f)] private float _checkLength = 0.1f;
        private Vector3 _horizontalVelocity = new Vector3(0f, 0f, 0f);
        private Vector3 _verticalVelocity = new Vector3(0f, 0f, 0f);

        [Header("Jump attributes")]
        [SerializeField, Range(0, 1f)] private float _jumpTime = 0.3f;
        [SerializeField, Range(0f, 1f)] private float _jumpCooldown = 0.3f;
        [SerializeField, Range(0f, 10f)] private float _jumpForce = 4f;//it's actually height
        [SerializeField, Range(0, 1f)] private float _airControl = 0.2f;
        //[SerializeField, Range(0f, 10f)] private float _jumpDistance = 4f;

        [Header("Drag for velocity")]
        [SerializeField, Range(0, 10f)] private float _groundDrag = 3f;
        [SerializeField, Range(0f, 5f)] private float _airDrag = 0.5f;
        [SerializeField, Range(0f, 1f)] private float _hitWallDrag = 0.5f;

        [Header("Collition Detect")]
        [SerializeField, Range(0f, 5f)] private float _collitionDistance = 1.5f;
        [SerializeField, Range(0f, 2f)] private float _moveUpStep = 0.25f;

  
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
        [ReadOnly][SerializeField] float _moveHorizontal;
        [ReadOnly][SerializeField] private float _moveVertical;
        [ReadOnly][SerializeField] private Vector3 _moveDirection;

        
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
                Jump();
                Invoke(nameof(ResetJump), _jumpCooldown);
            }
        }

        private void ResetJump()
        {
            _readyToJump = true;
        }


        private void Jump()
        {
            _verticalVelocity = Vector3.up * 2*_jumpForce/_jumpTime;
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
            //update position
            transform.position += _horizontalVelocity * Time.fixedDeltaTime + _verticalVelocity * Time.fixedDeltaTime;

            // calculate movement direction
            _moveDirection = transform.forward * _moveVertical + transform.right * _moveHorizontal;

            if (OnSlope())
            {
                //_rb.AddForce(GetSlopeMoveDirection() * _speed * _horsePower, ForceMode.Force);

                //if (_rb.velocity.y > 0) _rb.AddForce(Vector3.down * 8f * _horsePower, ForceMode.Force);
                _horizontalVelocity += GetSlopeMoveDirection() * _speed;
            }

            // on ground
            if (_isGround)
            //_rb.AddForce(_moveDirection.normalized * _speed * _horsePower, ForceMode.Force);
            {
                _horizontalVelocity += _moveDirection * _speed;

            }
            // in air
            else if (!_isGround)
            {
                _horizontalVelocity += _moveDirection * _speed * _airControl;
                //handle falling
                _verticalVelocity += Vector3.up * -2 * _jumpForce / (_jumpTime * _jumpTime) * Time.fixedDeltaTime;
            }
            CollideDetect();
            HandleDrag();
            //TODO: disable gravity on slope if needs
        }

        private void HandleDrag()
        {
            //handle drag
            if (_isGround && _horizontalVelocity.sqrMagnitude > 0f)
            {
                _horizontalVelocity = _horizontalVelocity * (10 - _groundDrag) / 10;
            }
            else
            {
                _horizontalVelocity = _horizontalVelocity * (10 - _airDrag) / 10;
            }
            //stop when hit's ground
            if (_isGround && _verticalVelocity.y < 0f)
            {
                _verticalVelocity *= 0f;
            }
            //stop infinite sliding
            if (_horizontalVelocity.sqrMagnitude > 0f && _horizontalVelocity.sqrMagnitude < 0.1f)
            {
                _horizontalVelocity *= 0f;
            }
        }

        private void CollideDetect()
        {
            RaycastHit collisionHit;
            
            for (int i=0;i<4;i++)
            {
                if (Physics.Raycast(transform.position + new Vector3(0f, _moveUpStep * 2, 0f), _horizontalVelocity.normalized, out collisionHit, _collitionDistance, _groundMask))
                {
                    _horizontalVelocity = Vector3.ProjectOnPlane(_horizontalVelocity, collisionHit.normal) * _hitWallDrag;
                }
                if (Physics.Raycast(transform.position + new Vector3(0f, _moveUpStep * 6, 0f), _horizontalVelocity.normalized, out collisionHit, _collitionDistance, _groundMask))
                {
                    _horizontalVelocity = Vector3.ProjectOnPlane(_horizontalVelocity, collisionHit.normal) * _hitWallDrag;
                }
            }
            if (Physics.Raycast(transform.position + new Vector3(0f, _moveUpStep * 2, 0f), _horizontalVelocity.normalized, out collisionHit, _collitionDistance, _groundMask))
            {
                _horizontalVelocity = Vector3.ProjectOnPlane(_horizontalVelocity, collisionHit.normal) * _hitWallDrag;
                return;
            }
            if (Physics.Raycast(transform.position, _horizontalVelocity.normalized, _collitionDistance, _groundMask))
            {
                if (_horizontalVelocity.sqrMagnitude>0f)
                {
                    _verticalVelocity += new Vector3(0f, _moveUpStep, 0f);
                }
            }
        }
        private void SpeedControl()
        {

            if (_horizontalVelocity.sqrMagnitude > _speed * _speed)
            {
                _horizontalVelocity = _horizontalVelocity.normalized * _speed;
            }
            if (_verticalVelocity.sqrMagnitude> _speed * _speed* 2 * 2)
            {
                _verticalVelocity = _verticalVelocity.normalized * 2*_speed;
                Debug.Log("speed limit");
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
            Gizmos.DrawRay(transform.position+new Vector3(0f, _moveUpStep*2, 0f), _horizontalVelocity);
            Gizmos.DrawRay(transform.position+new Vector3(0f, _moveUpStep, 0f), _horizontalVelocity);
            Gizmos.DrawRay(transform.position+new Vector3(0f, _moveUpStep*6, 0f), _horizontalVelocity);
            Gizmos.DrawRay(transform.position, _horizontalVelocity);
        }
    }
}
