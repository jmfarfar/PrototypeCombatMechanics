using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public Action<int> onHealthDecreased;
    
    [field: SerializeField] public int MaxHealth { get; private set; }
    [field: SerializeField] public int CurrentHealth { get; private set; }

    private void Awake() 
    {
        CurrentHealth = MaxHealth;
    }

    public void IncreaseHealth(int amount)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + amount, MaxHealth);
    }

    public void DecreaseHealth(int amount)
    {
        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
        onHealthDecreased?.Invoke(amount);
    }
}
