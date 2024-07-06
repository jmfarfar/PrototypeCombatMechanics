using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class GhostGuardian : GenericHitboxBehaviors
{
    [HideInInspector] public Transform owner;
    [HideInInspector] public Transform enemy;
    [HideInInspector] public GameObject sphereHitbox;
    [HideInInspector] public AttackData attackData;
    public Action OnDestroyAction;
    public static int ghostGuardianInstances;
    public int index;
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
        

        updateAction?.Invoke();
        
        transform.SetPositionAndRotation(Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 5f), Quaternion.LookRotation(vectorToEnemy));
    }

    Action GuardingAction()
    {
        return () => {
            Vector3 vectorToOwner = transform.position - owner.position;
            vectorToOwner.y = 1f;
            vectorToOwner.Normalize();
            vectorToEnemy = enemy.position - owner.position;
            vectorToEnemy.y = 1f;
            vectorToEnemy.Normalize();

            Vector3 assignedVector = index switch
            {
                0 => vectorToOwner,
                1 => Vector3.Cross(vectorToEnemy, owner.up),
                2 => -Vector3.Cross(vectorToEnemy, owner.up),
                _ => vectorToOwner
            };

            desiredPosition = owner.position + vectorToEnemy + assignedVector;

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
            vectorToEnemy = enemy.position - transform.position;
            vectorToEnemy.y = 0f;
            vectorToEnemy.Normalize();

            desiredPosition = enemy.position + (transform.position - enemy.position).normalized * 2f;
        };
    }

    public void Attack()
    {
        hitboxInstance = InstantiateHitbox(sphereHitbox, hitboxLocation, attackData, out Hitbox hitbox);

        hitboxInstance.transform.localScale = Vector3.one * 3f;

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

    void OnDestroy()
    {
        OnDestroyAction?.Invoke();
    }
}
