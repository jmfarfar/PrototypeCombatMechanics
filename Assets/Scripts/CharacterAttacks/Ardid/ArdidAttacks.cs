using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArdidAttacks : GenericHitboxBehaviors
{
    [SerializeField] private GameObject sphereHitbox;
    [SerializeField] private Transform enemy;
    [SerializeField] private NegativeEnergyManager negativeEnergyManager;
    [SerializeField] private AttackManager attackManager;

    [Header("Negative energy ratio per attack")]
    [SerializeField] private float necroticDartNegativeEnergy;
    [SerializeField] private float deathScytheNegativeEnergy;

    [Header("Drain ratio per attack")]
    [SerializeField] private float deathScytheDrainRatio;
    
    private Health health;

    private void Awake() 
    {
        health = GetComponent<Health>();
    }

    // GENERAL METHODS

    public void AddNegativeEnergyOnSuccessfulHit(Collider other, AttackData attackData, NegativeEnergyManager manager, float damageToEnergyRatio)
    {
        if (other.TryGetComponent(out PlayerStateMachine player))
        {
            if (player.colliderState == Hitbox.ColliderStates.ACTIVE)
            {
                manager.AddNegativeEnergy(Mathf.Ceil(attackData.damage * damageToEnergyRatio));
            }
        }
    }

    public void AddNegativeEnergyOnAnyHit(Collider other, AttackData attackData, NegativeEnergyManager manager, float damageToEnergyRatio)
    {
        if (other.TryGetComponent(out PlayerStateMachine player))
        {
            if (player.colliderState != Hitbox.ColliderStates.INACTIVE)
            {
                manager.AddNegativeEnergy(Mathf.Ceil(attackData.damage * damageToEnergyRatio));
            }
        }
    }

    public void DrainLifeOnSuccessfulHit(Collider other, AttackData attackData, float damageToHealthPointsRatio)
    {
        if (other.TryGetComponent(out PlayerStateMachine player))
        {
            if (player.colliderState == Hitbox.ColliderStates.ACTIVE)
            {
                health.IncreaseHealth((int)Mathf.Ceil(attackData.damage * damageToHealthPointsRatio));
            }
        }
    }

    public void DrainLifeOnAnyHit(Collider other, AttackData attackData, float damageToHealthPointsRatio)
    {
        if (other.TryGetComponent(out PlayerStateMachine player))
        {
            if (player.colliderState != Hitbox.ColliderStates.INACTIVE)
            {
                health.IncreaseHealth((int)Mathf.Ceil(attackData.damage * damageToHealthPointsRatio));
            }
        }
    }

    // NECROTIC DART
#region
    public void NecroticDart(AttackData attackData)
    {
        GameObject instance = InstantiateHitbox(
            sphereHitbox, 
            transform.position,
            Quaternion.identity,
            attackData,
            out Hitbox hitbox
            );
        
        hitbox.SetUpHitboxActions(
            //Hitbox start action
            () => {},
            
            //Hitbox update action
            FollowTargetOverTime(enemy, instance.transform, attackData.range, attackData.activeTime),
            
            //Hitbox OnTriggerEnter action
            (Collider other) => {
                AddNegativeEnergyOnSuccessfulHit(other, hitbox.attackData, negativeEnergyManager, necroticDartNegativeEnergy);
                Destroy(instance);
                }
            );
        instance.layer = LayerMask.NameToLayer("Player1Hitbox");
    }

#endregion

    // SOUL TRAP SCYTHE
#region 
    public void SoulTrapScythe(AttackData attackData)
    {
        GameObject hitboxInstance = InstantiateHitbox(
            sphereHitbox,
            transform.position + (transform.forward * attackData.range/2f),
            Quaternion.identity,
            attackData,
            out Hitbox hitbox
        );
        hitboxInstance.layer = LayerMask.NameToLayer("Player1Hitbox");

        hitboxInstance.transform.localScale = new Vector3(attackData.range/2f, attackData.range/2f, attackData.range/2f);

        hitbox.SetUpHitboxActions(
            () => {attackManager.onActiveFramesEnd += () => {Destroy(hitboxInstance);};},
            () => {},
            (Collider other) => {
                AddNegativeEnergyOnAnyHit(other, attackData, negativeEnergyManager, deathScytheNegativeEnergy);
                DrainLifeOnAnyHit(other, attackData,deathScytheDrainRatio);
                },
            () => {attackManager.onActiveFramesEnd -= () => {Destroy(hitboxInstance);};}
        );
    }

#endregion
    
}
