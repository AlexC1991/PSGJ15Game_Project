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
        public bool isGrounded { get => _isGround; }
        // Update is called once per frame
        void Update()
        {
            _isGround = Physics.CheckSphere(_groundCheck.position, _checkLength, _groundMask);

            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;

            _controller.Move(move * _speed * Time.deltaTime);//basic movement

            if (Input.GetButtonDown("Jump") && _isGround)//jump
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
