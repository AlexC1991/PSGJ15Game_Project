using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ProjectDungeonCrawlerPJ15
{
    public class BasicAimAndFire : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private Gun gun;

        // Start is called before the first frame update
        void Start()
        {
            if (_animator == null)
            {
                Debug.LogError("Assign the animator to the script! YOU *@&$#% !!! JK btw");
                if (gun == null)
                {
                    Destroy(this);
                }
            }
            if (gun == null)
            {
                Debug.LogError("Assign the Gun script to the script! YOU *@&$#% !!! JK btw");
            }
        }

        // Update is called once per frame
        void Update()
        {
            Aim();
            Fire();
        }
        private void Aim()
        {
            if (Input.GetButtonDown("Fire2"))
            {
                _animator.SetBool("Aiming", !_animator.GetBool("Aiming"));//change to true if hold to aim
            }
            //if (Input.GetButtonUp("Fire2"))  //if you want to hold to aim
            //{
            //    animController.SetBool("Aiming", false);
            //}
        }
        private void Fire()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                gun.Shoot();
            }
        }
    }
}