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
        SHIELDSTUN,
        RESTRAINED
    }

    [HideInInspector] public AttackData attackData;
    public GameObject Owner;
    public GameObject Target;
    public Collider collider => GetComponent<Collider>();

    Action<Collider> triggerEnterAction;
    Action<Collider> triggerExitAction;

    Action updateAction;
    Action startAction;
    Action onDestroyAction;

    private void Start() 
    {
        startAction?.Invoke();
    }

    private void FixedUpdate() 
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

    public void SetUpHitboxActions(Action startAction, Action updateAction, Action<Collider> triggerEnterAction, Action onDestroyAction, Action<Collider> triggerExitAction)
    {
        this.startAction = startAction;
        this.updateAction = updateAction;
        this.triggerEnterAction = triggerEnterAction;
        this.onDestroyAction = onDestroyAction;
        this.triggerExitAction = triggerExitAction;
    }

    private void OnTriggerEnter(Collider other)
    {
        triggerEnterAction?.Invoke(other);
    }

    private void OnTriggerExit(Collider other) 
    {
        triggerExitAction?.Invoke(other);
    }

    private void OnDestroy() 
    {
        onDestroyAction?.Invoke();
    }
}
