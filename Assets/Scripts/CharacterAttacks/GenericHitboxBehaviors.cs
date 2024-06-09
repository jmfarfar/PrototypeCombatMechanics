using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;

public class GenericHitboxBehaviors : MonoBehaviour
{
    public GameObject InstantiateHitbox(GameObject original, Vector3 position, Quaternion rotation, AttackData attackData, out Hitbox hitbox)
    {
        GameObject instance = GameObject.Instantiate(
            original, 
            position,
            rotation);

        Hitbox instanceHitbox = instance.GetComponent<Hitbox>();
        instanceHitbox.attackData = attackData;
        hitbox = instanceHitbox;
        return instance;
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

    public Action DestroyHitboxAfterSeconds(GameObject hitbox, float seconds)
    {
        float timer = 0f;

        return () => {
          timer += Time.deltaTime;
            if (timer >= seconds) Destroy(hitbox);
        };
    }
}
