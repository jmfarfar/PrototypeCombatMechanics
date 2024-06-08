using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Hitbox : MonoBehaviour
{
    [Serializable]
    public enum hitboxBehaviours 
    {
        APPEAR,
        TRAVEL,
        GROW
    }

    public enum ColliderStates
    {
        ACTIVE,
        INACTIVE,
        BLOCKING,
        SHIELDSTUN
    }

    [HideInInspector] public AttackData attackData;
    public GameObject Owner;
    public GameObject Target;

    Action triggerEnterAction;

    Action hitboxAction;
    Action startAction;

    private void Start() 
    {
        startAction?.Invoke();
    }

    private void Update() 
    {
        hitboxAction?.Invoke();
    }

    public void SetUpHitboxActions(Action startAction, Action updateAction, Action triggerAction)
    {
        this.startAction = startAction;
        this.hitboxAction = updateAction;
        this.triggerEnterAction = triggerAction;
    }

    private void OnTriggerEnter(Collider other)
    {
        triggerEnterAction?.Invoke();
    }
}
