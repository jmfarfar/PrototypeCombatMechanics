using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;

public class GenericHitboxBehaviors : MonoBehaviour
{
    protected GameObject InstantiateHitbox(GameObject original, Vector3 position, Quaternion rotation, AttackData attackData, out Hitbox hitbox)
    {
        GameObject instance = GameObject.Instantiate(
            original,
            position,
            rotation);

        Hitbox instanceHitbox = instance.GetComponent<Hitbox>();
        instanceHitbox.attackData = attackData;
        instanceHitbox.Owner = this.gameObject;
        hitbox = instanceHitbox;

        return instance;
    }

    public Action FollowTargetOverTime(Transform target, Transform objectToMove, float range, float activeTime)
    {
        float speed = range / activeTime; // estático
        return () =>
        { // continuo
            Vector3 direction = (target.position - objectToMove.position).normalized;

            activeTime -= Time.deltaTime;

            if (activeTime > 0f)
            {
                objectToMove.position += direction * speed * Time.deltaTime;
            }
            else
            {
                Destroy(objectToMove.gameObject);
            }
        };
    }

    //public Action ParabolicJump(Transform objectToMove, Vector3 start, Vector3 end, float height, float time)
    //{
    //    float startTime = time;
    //    return () =>
    //    {

    //        time -= Time.deltaTime;
    //        Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

    //        if (time > 0f)
    //        {

    //            var mid = Vector3.Lerp(end, start, time);

    //            objectToMove.transform.position = new Vector3(mid.x, f(time) + Mathf.Lerp(end.y, start.y, time), mid.z);
    //            //Debug.Log(objectToMove.transform.position);
    //            Debug.Log(f(time));

    //        }
    //        //else
    //        //{
    //        //    Destroy(objectToMove.gameObject);
    //        //}

    //    };
    //}

    public Action DestroyHitboxAfterSeconds(GameObject hitbox, float seconds)
    {
        float timer = 0f;

        return () =>
        {
            timer += Time.deltaTime;
            if (timer >= seconds) Destroy(hitbox);
        };
    }

    public IEnumerator BurnRoutine(float activeTime, int totalDamage, Health targetHealth)
    {
        int damageGiven = 0;
        while (damageGiven < totalDamage)
        {
            targetHealth.DecreaseHealth(1);

            damageGiven++;

            yield return new WaitForSeconds(activeTime / totalDamage);
        }
    }


}
