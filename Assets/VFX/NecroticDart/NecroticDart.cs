using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NecroticDart : MonoBehaviour
{
    [SerializeField] GameObject NecroticDart_VFX;


    void OnTriggerEnter(Collider collision)
    {
        GameObject instance = Instantiate(NecroticDart_VFX, transform.position, transform.rotation);
    }
}