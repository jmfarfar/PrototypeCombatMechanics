using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class NegativeEnergyManager : MonoBehaviour
{
    public float negativeEnergy;

    private void Update() 
    {
        negativeEnergy = Mathf.Max(negativeEnergy - Time.deltaTime, 0f);
    }

    public void AddNegativeEnergy(float amount)
    {
        negativeEnergy += amount;
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(transform.position + transform.right, new Vector3(0.5f, negativeEnergy, 0.5f));
    }
}
