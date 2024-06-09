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

    Action<Collider> triggerEnterAction;

    Action updateAction;
    Action startAction;
    Action onDestroyAction;

    private void Start() 
    {
        startAction?.Invoke();
    }

    private void Update() 
    {
        updateAction?.Invoke();
    }

    public void SetUpHitboxActions(Action startAction, Action updateAction, Action<Collider> triggerAction)
    {
        this.startAction = startAction;
        this.updateAction = updateAction;
        this.triggerEnterAction = triggerAction;
    }
    public void SetUpHitboxActions(Action startAction, Action updateAction, Action<Collider> triggerEnterAction, Action onDestroyAction)
    {
        this.startAction = startAction;
        this.updateAction = updateAction;
        this.triggerEnterAction = triggerEnterAction;
        this.onDestroyAction = onDestroyAction;
    }

    private void OnTriggerEnter(Collider other)
    {
        triggerEnterAction?.Invoke(other);
    }

    private void OnDestroy() 
    {
        onDestroyAction?.Invoke();
    }
}
