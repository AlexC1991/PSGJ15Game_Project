using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ProjectDungeonCrawlerPJ15
{
    public class BasicMovement : MonoBehaviour
    {
        [SerializeField] private CharacterController _controller;
        [SerializeField, Range(0, 50f)] private float _speed = 10f;
        [SerializeField, Range(-20f, 0)] private float _gravity = -9.8f;
        [SerializeField, Range(0.4f, 1f)] private float _checkLength = 0.1f;
        [SerializeField, Range(0, 0.5f)] private float _jumpHeight = 0.1f;

        private Vector3 _velocity;

        [SerializeField] private Transform _groundCheck;
        [SerializeField] private LayerMask _groundMask;
        private bool _isGround;
         private float _mouseXposition;
         private float _mouseYposition;
         private float _mouseSensitivityX;
         private float _mouseSensitivityY;
        [SerializeField] private float downValue;
        [SerializeField] private float upValue;
        [SerializeField] private Camera _playerCamera;
         private float _moveHorizontal;
         private float _moveVertical;
        private Vector3 _moveDirection;

        public bool isGrounded { get => _isGround; }
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
            _mouseXposition += Input.GetAxis("Mouse X") * _mouseSensitivityX;
            _mouseYposition -= Input.GetAxis("Mouse Y") * _mouseSensitivityY;
            _mouseYposition = Mathf.Clamp(_mouseYposition, downValue, upValue);

            transform.rotation = Quaternion.Euler(_mouseYposition, _mouseXposition, 0f);
            _playerCamera.transform.rotation = Quaternion.Euler(_mouseYposition, _mouseXposition, 0f);
            
            _moveHorizontal = Input.GetAxis("Horizontal");
            _moveVertical = Input.GetAxis("Vertical");

            Vector3 movement = new Vector3(_moveHorizontal, 0f, _moveVertical);
            movement = transform.TransformDirection(movement) * _speed;
            
            _controller.Move((movement + _moveDirection) * Time.deltaTime);
            
            _isGround = Physics.CheckSphere(_groundCheck.position, _checkLength, _groundMask);
            
            if (Input.GetKeyUp(KeyCode.Space) && _isGround)//jump
            {
                _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
            }
        }

        private void FixedUpdate()
        {

            if (_isGround && _velocity.y < 0)
            {
                _velocity.y = -0.1f;
            }
            if (_velocity.y > 0)
            {
                _velocity.y += _gravity * Time.deltaTime / 2;
            }
            else
            {
                _velocity.y += _gravity * Time.deltaTime / 4;
            }

            _controller.Move(_velocity / 2);//fall

        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_groundCheck.position, _checkLength);
        }
    }
}
