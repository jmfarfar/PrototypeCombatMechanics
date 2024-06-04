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
        SHIELDSTUN
    }

    [HideInInspector] public AttackInfo attackInfo;
    public GameObject Owner;
    public GameObject Target;

    private Vector3 vectorToTarget;
    private Collider colliderComponent => gameObject.GetComponent<Collider>();

    Action triggerEnterAction;

    private void Start() 
    {
        switch (attackInfo.attachBehaviour)
        {
            case hitboxBehaviours.APPEAR:
            {
                StartCoroutine(AppearBehaviour());
                break;
            }

            case hitboxBehaviours.TRAVEL:
            {
                StartCoroutine(TravelBehaviour());
                break;
            }

            case hitboxBehaviours.GROW:
            {
                StartCoroutine(GrowBehaviour());
                break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEnterBehaviour(triggerEnterAction);
    }

    private IEnumerator TravelBehaviour()
    {
        Action action = () => 
        {
            StopAllCoroutines();
            Destroy(this.gameObject);
        };

        triggerEnterAction = action;

        float startTime = Time.time;

        transform.position += Owner.transform.forward;

        while (Time.time < startTime + (attackInfo.speed / attackInfo.range))
        {
            vectorToTarget = (Target.transform.position - transform.position).normalized;
            transform.Translate(vectorToTarget * attackInfo.speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        Destroy(this.gameObject);
    }

    private IEnumerator AppearBehaviour()
    {
        Vector3 vectorToTarget = Target.transform.position - Owner.transform.position;
        float range = Mathf.Min(vectorToTarget.magnitude, attackInfo.range);
        transform.position = Owner.transform.position + (vectorToTarget.normalized * range);
        transform.localScale = Vector3.one * attackInfo.radius * 2f;
        yield return new WaitForSeconds(attackInfo.activeTime);
        Destroy(this.gameObject);
    }

    private IEnumerator GrowBehaviour()
    {
        transform.position = Owner.transform.position + Vector3.up;
        transform.localScale = Vector3.one * attackInfo.radius * 2f;
        yield return new WaitForSeconds(attackInfo.activeTime);
        Destroy(this.gameObject);
    }

    private void OnTriggerEnterBehaviour(Action triggerBehaviour)
    {
        if (triggerBehaviour != null)
        {
            triggerBehaviour();
        }
    }
}
