using System.Collections;
using System.Collections.Generic;
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
}
