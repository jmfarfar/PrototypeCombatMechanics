using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArdidAttacks : MonoBehaviour
{
    public GameObject necroticDartHitbox;
    public Transform enemy;

    [Header("Negative energy per attack")]
    [SerializeField] private float necroticDart_negativeEnergy;

    public void NecroticDart(AttackData attackData)
    {
        GameObject instance = GameObject.Instantiate(
            necroticDartHitbox, 
            transform.position,
            Quaternion.identity);
        
        Hitbox instanceHitbox = instance.GetComponent<Hitbox>();
        instanceHitbox.attackData = attackData;
        instanceHitbox.SetUpHitboxActions(
            () => {},
            FollowTargetOverTime(enemy, instance.transform,attackData.range, attackData.activeTime),
            () => {Destroy(instance);}
            );
        instance.layer = LayerMask.NameToLayer("Player1Hitbox");
    }

    public Action FollowTargetOverTime(Transform target, Transform objectToMove, float range, float activeTime)
    {
        float speed = range/activeTime; // estático
        return () => { // continuo
        Vector3 direction = (target.position - objectToMove.position).normalized;

        activeTime -= Time.deltaTime;

        if (activeTime >0f)
        {
            objectToMove.position += direction * speed * Time.deltaTime;
        }
        else
        {
            Destroy(objectToMove.gameObject);
        }
        };
    }
}
