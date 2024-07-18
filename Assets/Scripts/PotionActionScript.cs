using System;
using System.Collections;
using UnityEngine;

public class PotionActionScript : MonoBehaviour
{
    [TagSelector] [SerializeField] private string thisTagFloor;
    [TagSelector] [SerializeField] private string thisTagWall;

    [SerializeField] private GameObject bouncyPotion;
    [SerializeField] private GameObject slipPotion;
    [SerializeField] private GameObject stickyPotion;
    private RaycastHit rayHitInfo;
    RaycastHit hit;

    private Vector3 _glassBreakPosition;
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(thisTagFloor) || other.CompareTag(thisTagWall))
        {
            hitFloor();
        }
    }

    private bool hitFloor()
    {
        StartCoroutine(BreakBouncyPotion());
        return true;
    }
    
    IEnumerator BreakBouncyPotion()
    {
        if (Physics.Raycast(this.gameObject.transform.position,  this.gameObject.transform.forward, out rayHitInfo, 1))
        {
            GameObject instantiatedObject = Instantiate(bouncyPotion, hit.point, Quaternion.identity, hit.transform);
            instantiatedObject.transform.rotation = Quaternion.LookRotation(hit.normal);
        }
        else
        {
            Debug.Log("No hit");
        }

        Destroy(this.gameObject, 0.2f);

        yield break;
    }
}
