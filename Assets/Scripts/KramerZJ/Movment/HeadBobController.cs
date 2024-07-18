using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ProjectDungeonCrawlerPJ15
{
    public class HeadBobController : MonoBehaviour
    {
        [SerializeField] private bool _enable = true;

        [SerializeField, Range(0, 0.1f)] private float _amplitude = 0.0075f;
        [SerializeField, Range(0, 30f)] private float _frenquency = 20.0f;

        [SerializeField] private Transform _camera = null;
        [SerializeField] private Transform _camera_holder = null;

        private float _speedThreshold = 3.0f;
        private Vector3 _startPos;
        private CharacterController _controller;
        private BasicMovement _movement;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _movement = GetComponent<BasicMovement>();
            _startPos = _camera.localPosition;
        }
        private Vector3 FootStepMotion()
        {
            Vector3 pos = Vector3.zero;
            pos.y += Mathf.Sin(Time.time * _frenquency) * _amplitude;
            pos.x += Mathf.Cos(Time.time * _frenquency / 2) * _amplitude * 2;
            return pos;
        }
        private void CheckMotion()
        {
            float speed = new Vector3(_controller.velocity.x, 0, _controller.velocity.z).magnitude;

            if (speed < _speedThreshold) return;
            if (!_movement.isGrounded) return;

            PlayMotion(FootStepMotion());
        }
        private void ResetPosition()
        {
            if (_camera.localPosition == _startPos) return;
            _camera.localPosition = Vector3.Lerp(_camera.localPosition, _startPos, Time.deltaTime);
        }
        private void PlayMotion(Vector3 motion)
        {
            _camera.localPosition += motion;
        }
        private Vector3 FocusTarget()
        {
            Vector3 pos = new Vector3(transform.position.x, transform.position.y + _camera_holder.localPosition.y, transform.position.z);
            pos += _camera_holder.forward * 15f;
            return pos;
        }
        // Update is called once per frame
        void Update()
        {
            if (!_enable) return;

            CheckMotion();
            ResetPosition();
            _camera.LookAt(FocusTarget());

        }
    }
}
