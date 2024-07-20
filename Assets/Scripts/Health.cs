using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public Action<int> onHealthDecreased;
    
    [field: SerializeField] public int MaxHealth { get; private set; }
    [field: SerializeField] public int CurrentHealth { get; private set; }
    [FormerlySerializedAs("HPBar")] [SerializeField] private Image hpBar;

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
        hpBar.fillAmount = (float)CurrentHealth / MaxHealth;
        onHealthDecreased?.Invoke(amount);
    }
}
