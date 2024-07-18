using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ProjectDungeonCrawlerPJ15
{
    public class BasicCam : MonoBehaviour
    {
        [SerializeField, Range(0, 1000f)] private float _sensX = 400f, _sensY = 400f;

        [SerializeField] private Transform _BodyOrientation;

        private float _xRotation;

        // Start is called before the first frame update
        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // Update is called once per frame
        void Update()
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * _sensX;
            float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * _sensY;

            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -87f, 87f);

            transform.localRotation = Quaternion.Euler(_xRotation, 0, 0);
            _BodyOrientation.Rotate(Vector3.up * mouseX);
        }
    }
}
