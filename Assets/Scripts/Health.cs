using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    [field: SerializeField] public int CurrentHealth { get; private set; }
    
    private void Awake() 
    {
        CurrentHealth = maxHealth;
    }

    public void increaseHealth(int amount)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + amount, maxHealth);
    }

    public void decreaseHealth(int amount)
    {
        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
    }
}
