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
    [SerializeField] private float necroticDart_negativeEnergy;

    // GENERAL METHODS

    public void AddNegativeEnergyOnSuccessfulHit(Collider other, AttackData attackData, NegativeEnergyManager manager, float energyToDamageRatio)
    {
        if (other.TryGetComponent<PlayerStateMachine>(out PlayerStateMachine player))
        {
            if (player.colliderState == Hitbox.ColliderStates.ACTIVE)
            {
                manager.AddNegativeEnergy(Mathf.Ceil(attackData.damage * energyToDamageRatio));
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
            FollowTargetOverTime(enemy, instance.transform,attackData.range, attackData.activeTime),
            
            //Hitbox OnTriggerEnter action
            (Collider other) => {
                AddNegativeEnergyOnSuccessfulHit(other, hitbox.attackData, negativeEnergyManager, necroticDart_negativeEnergy);
                Destroy(instance);
                }
            );
        instance.layer = LayerMask.NameToLayer("Player1Hitbox");
    }

#endregion

    // DEATH SCYTHE
#region 
    public void DeathScythe(AttackData attackData)
    {
        GameObject hitboxInstance = InstantiateHitbox(
            sphereHitbox,
            transform.position + (transform.forward * attackData.range/2f),
            Quaternion.identity,
            attackData,
            out Hitbox hitbox
        );

        hitboxInstance.transform.localScale = new Vector3(attackData.range/2f, attackData.range/2f, attackData.range/2f);

        hitbox.SetUpHitboxActions(
            () => {attackManager.hitboxActiveFramesEnd += () => {Destroy(hitboxInstance);};},
            () => {},
            (Collider other) => {},
            () => {attackManager.hitboxActiveFramesEnd -= () => {Destroy(hitboxInstance);};}
        );
    }

#endregion
    
}
