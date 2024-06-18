using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class NegativeEnergyManager : MonoBehaviour
{
    public float temporalNegativeEnergy;
    public int persistentNegativeEnergy;
    public int summoningSlots = 3;
    [SerializeField] private float energyDecayRate = 1f;
    public Action<int> onAddPersistentNegativeEnergy;

    private void Update() 
    {
        if (temporalNegativeEnergy >= 1f)
        {
            AddPersistentNegativeEnergy();
            temporalNegativeEnergy = 0f;
        }

        temporalNegativeEnergy = Mathf.Max(temporalNegativeEnergy - Time.deltaTime * energyDecayRate, 0f);
    }

    private void AddPersistentNegativeEnergy()
    {
        persistentNegativeEnergy += (int)temporalNegativeEnergy;
        persistentNegativeEnergy = Mathf.Min(persistentNegativeEnergy, summoningSlots);
        onAddPersistentNegativeEnergy?.Invoke((int)temporalNegativeEnergy);
    }

    public void AddNegativeEnergy(float amount){temporalNegativeEnergy += amount;}
    public void SubtractNegativeEnergy(int cost){
        persistentNegativeEnergy = Mathf.Max(persistentNegativeEnergy-= cost, 0);
        }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.blue;
        for (int i = 0; i < persistentNegativeEnergy; i++)
        {
            Gizmos.DrawCube(transform.position + transform.right + (Vector3.up/summoningSlots * i), Vector3.one * (1f / (summoningSlots * 2)));
        }
    }
}
