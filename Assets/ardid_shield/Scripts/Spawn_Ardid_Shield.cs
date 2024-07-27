using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn_Ardid_Shield : MonoBehaviour
{
    [SerializeField]private GameObject shieldVFX;
    private Vector3 shieldOffset; 
    [SerializeField]private float shieldActiveTime;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var vfx = Instantiate(shieldVFX, transform) as GameObject;
            vfx.transform.position += shieldOffset;
            Destroy(vfx, shieldActiveTime);
        }        
    } 

}
