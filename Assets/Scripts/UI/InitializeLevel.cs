using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeLevel : MonoBehaviour
{
    [SerializeField] private AudioClip combatClip;
    
    private void Awake()
    {
        AudioManager.Instance.StopBackgroundClip();
        
    }

    private void Start()
    {
        AudioManager.Instance.PlaySound(combatClip);
    }
}
