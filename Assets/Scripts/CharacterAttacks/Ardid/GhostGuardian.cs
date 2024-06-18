using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostGuardian : GenericHitboxBehaviors
{
    [HideInInspector] public Transform owner;
    [HideInInspector] public Transform enemy;
    [HideInInspector] public GameObject sphereHitbox;
    [HideInInspector] public AttackData attackData;
    [SerializeField] private Transform hitboxLocation;
    Action updateAction;
    Animator animator;
    GameObject hitboxInstance;
    Vector3 vectorToEnemy;
    Vector3 desiredPosition;

    void Start()
    {
        animator = GetComponent<Animator>();
        updateAction = GuardingAction();
    }

    void Update()
    {
        vectorToEnemy = enemy.position - owner.position;
        vectorToEnemy.y = 0f;
        vectorToEnemy.Normalize();

        updateAction?.Invoke();
        
        transform.SetPositionAndRotation(Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 2f), Quaternion.LookRotation(vectorToEnemy));
    }

    Action GuardingAction()
    {
        return () => {
            Vector3 vectorToCenter = Vector3.zero - owner.position;
            vectorToCenter.y = 0f;
            vectorToCenter.Normalize();
            Vector3 vectorToOwner = transform.position - owner.position;
            vectorToOwner.y = 0f;
            vectorToOwner.Normalize();

            desiredPosition = owner.position + vectorToCenter + vectorToEnemy + vectorToOwner;

            desiredPosition.y = 0f;

            if (Vector3.Distance(transform.position, enemy.position) < attackData.range)
            {
                updateAction = AttackingAction();
            }
        };
    }

    Action AttackingAction()
    {
        animator.SetBool("isAttacking", true);
        return () => {
            desiredPosition = enemy.position + (transform.position - enemy.position).normalized;
        };
    }

    public void Attack()
    {
        hitboxInstance = InstantiateHitbox(sphereHitbox, hitboxLocation, attackData, out Hitbox hitbox);

        hitboxInstance.transform.localScale = Vector3.one * 2f;

        hitboxInstance.layer = AssignLayerToHitbox(gameObject.layer);
    }

    public void EndHitboxActiveFrames()
    {
        Destroy(hitboxInstance);
    }

    public void FinishAttack()
    {
        animator.SetBool("isAttacking", false);
        Destroy(gameObject);
    }
}
