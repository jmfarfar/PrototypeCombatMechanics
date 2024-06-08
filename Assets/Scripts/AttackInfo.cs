using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AttackInfo
{
#region
    [Serializable]
    public enum AttackAnimation 
    {
        TRAVEL,
        APPEAR,
        GROW
    }

    [Serializable]
    public enum AttackEffects 
    {
        NONE,
        SHIELDBREAK,
        BURN,
        SLOWDOWN,
        LIFESTEAL,
        POISON
    }

    public int damage;
    public AttackEffects attackEffect;
    public float startupTime;
    public float recoveryTime;
    public AttackAnimation attackAnimation;
    public float stunTime;
    [HideInInspector] public string animationState => SetAnimations();

    [Header("Attack behaviour")]
    public Hitbox.hitboxBehaviours attachBehaviour;
    
    public float range;
    
    [Header("Travel behaviour only")]
    public float speed;

    [Header("Appear and grow behaviour only")]
    public float radius;
    public float activeTime;

    [HideInInspector]
    public GameObject hitTarget;
#endregion

    private string SetAnimations()
    {
        switch (attackAnimation)
        {
            case AttackAnimation.TRAVEL:
            {
                return "AttackTravel";
            }

            case AttackAnimation.APPEAR:
            {
                return "AttackAppear";
            }

            case AttackAnimation.GROW:
            {
                return "AttackGrow";
            }

            default: return "";
        }
    }

    public void AttackEffectAction(Hitbox hitbox)
    {
        Health enemyHealth = hitbox.Target.GetComponent<Health>();

        switch (attackEffect)
        {
            case AttackEffects.BURN:
            enemyHealth.StartCoroutine(BurningCoroutine(enemyHealth));
            return;

            case AttackEffects.LIFESTEAL:
            StealLife(hitbox);
            return;

            case AttackEffects.POISON:
            enemyHealth.StartCoroutine(PoisonedCoroutine(enemyHealth));
            return;

            case AttackEffects.SLOWDOWN:
            return;

            default: return;
        }
    }

    private IEnumerator PoisonedCoroutine(Health health)
    {
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(1f);
            health.decreaseHealth(1);
        }
    }

    private void StealLife(Hitbox hitbox)
    {
        float startTime = Time.time;
        int stolenHPs = (int)Mathf.Ceil(hitbox.attackData.damage * 0.3f);
        hitbox.Owner.GetComponent<Health>().increaseHealth(stolenHPs);
    }

    private IEnumerator BurningCoroutine(Health health)
    {
        float startTime = Time.time;

        while (Time.time < startTime + 1f)
        {
            health.decreaseHealth(1);
            yield return new WaitForEndOfFrame();
        }
    }
}
