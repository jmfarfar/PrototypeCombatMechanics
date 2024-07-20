using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    
    IEnumerator Start()
    {
        yield return new WaitForSeconds(3f);
        Destroy(this.gameObject);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
