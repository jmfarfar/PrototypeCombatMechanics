using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using UnityEngine;

public class ArdidLichLord : GenericHitboxBehaviors
{
    public Transform enemy;
    public NegativeEnergyManager manager;
    private AttackData attackData;
    [SerializeField] private GameObject sphereHitbox;

    private void Update() 
    {
        Vector3 directionToEnemy = enemy.position - transform.position;
        directionToEnemy.y = 0;
        directionToEnemy.Normalize();

        transform.rotation = Quaternion.LookRotation(directionToEnemy);

    }

    public void SetUpAttackData(AttackData attackData)
    {
        this.attackData = attackData;
    }

    public void CastSpell()
    {
        Vector3 vectorToTarget = enemy.position - transform.position;
        vectorToTarget = Vector3.ClampMagnitude(vectorToTarget, attackData.range);

        GameObject hitboxInstance = InstantiateHitbox(sphereHitbox, transform.position + vectorToTarget, Quaternion.identity, attackData, out Hitbox hitbox);

        hitboxInstance.transform.localScale = Vector3.one * 2f;

        hitbox.SetUpHitboxActions(
            //Hitbox start action
            () => {},
            
            //Hitbox update action
            DestroyHitboxAfterSeconds(hitboxInstance, attackData.activeTime),
            
            //Hitbox OnTriggerEnter action
            (Collider other) => {}
            );
        hitboxInstance.layer = AssignLayerToHitbox(gameObject.layer);

        manager.SubtractNegativeEnergy(1);
    }

    public void DestroyOnEnergyDepleted()
    {
        if (manager.persistentNegativeEnergy == 0)
        {
            Destroy(gameObject);
        }
    }
}
