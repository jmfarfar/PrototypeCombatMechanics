using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArdidArcherSkeleton : GenericHitboxBehaviors
{
    public Transform enemy;
    private AttackData attackData;
    [SerializeField] private GameObject sphereHitbox;
    private int shotCount;

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

    public void ShootArrow()
    {
        GameObject hitboxInstance = InstantiateHitbox(sphereHitbox, transform.position + Vector3.up, Quaternion.identity, attackData, out Hitbox hitbox);

        hitbox.SetUpHitboxActions(
            //Hitbox start action
            () => {},
            
            //Hitbox update action
            FollowTargetOverTime(enemy, hitboxInstance.transform, 10f, 0.5f),
            
            //Hitbox OnTriggerEnter action
            (Collider other) => {
                Destroy(hitboxInstance);
                }
            );
        hitboxInstance.layer = AssignLayerToHitbox(gameObject.layer);
        shotCount++;
    }

    public void DestroyAfterAllShots()
    {
        if (shotCount == 3)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy() 
    {
        
    }
}
