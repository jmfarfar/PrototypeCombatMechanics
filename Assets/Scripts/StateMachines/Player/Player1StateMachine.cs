using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1StateMachine : StateMachine, IPlayerStateMachine
{
    #region Properties
    [field: SerializeField]
    public CharacterController CharacterController { get; set; }
    [field: SerializeField]
    public float Speed { get; set; }
    [field: SerializeField]
    public float DashForce { get; set; }
    [field: SerializeField] public Health Health { get; private set; }
    [field: SerializeField]
    public float RotationSpeed { get; set; }

    [field: SerializeField] public GameObject ProjectilePrefab { get; private set; }
    [field: SerializeField] public GameObject Combatant { get; private set; }
    [field: SerializeField] public Animator Animator { get; private set; }

    public AttackInfo[] attacks;
    [HideInInspector] public Collider colliderComponent => gameObject.GetComponent<Collider>();

    public float dashTime = 0.25f;
    public float dashCooldown = 1f;

    public float dashDistance = 1f;
    public float dashInvulnerabilityTime;
    public AnimationCurve dashAccelerationCurve;

    public Hitbox.ColliderStates colliderState;

    public Action<AttackInfo> shieldHit;
    public Action<Vector3> rollEvent;

    public float maxShieldEnergy = 1f;
    [HideInInspector] public float shieldEnergy;
    public bool isShieldDepleting;
    public bool isShieldDepleted;
    public bool isRolling;

    #endregion

    private void Start()
    {
        SwitchState(new Player1MovementState(this));

        shieldEnergy = maxShieldEnergy;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (colliderState == Hitbox.ColliderStates.INACTIVE || !other.TryGetComponent<Hitbox>(out Hitbox hitbox))
        {
            return;
        }

        else if (colliderState == Hitbox.ColliderStates.BLOCKING)
        {
            //SwitchState(new Player1StunState(this, hitbox.attackInfo.stunTime / 2));
            Physics.IgnoreCollision(colliderComponent, other, true);
            colliderState = Hitbox.ColliderStates.SHIELDSTUN;
            shieldHit?.Invoke(hitbox.attackInfo);

            if (hitbox.attackInfo.attackEffect == AttackInfo.AttackEffects.SHIELDBREAK)
            {
                shieldEnergy = 0f;
            }
        }

        else
        {
            if (hitbox.attackInfo.attackAnimation >= AttackInfo.AttackAnimation.APPEAR)
            {
                Debug.Log("Collided and knocked down");
                Health.decreaseHealth(hitbox.attackInfo.damage);


                Physics.IgnoreCollision(colliderComponent, other, true);
                hitbox.attackInfo.AttackEffectAction(hitbox);
                SwitchState(new Player1KnockdownState(this, 3f));

                if (Health.CurrentHealth <= 0f)
                {
                    SwitchState(new Player1KOState(this));
                }
            }
            else
            {
                Debug.Log("Collided");
                Health.decreaseHealth(hitbox.attackInfo.damage);


                Physics.IgnoreCollision(colliderComponent, other, true);
                hitbox.attackInfo.AttackEffectAction(hitbox);
                SwitchState(new Player1StunState(this, hitbox.attackInfo.stunTime));

                if (Health.CurrentHealth <= 0)
                {
                    SwitchState(new Player1KOState(this));
                }

            }
        }
    }

    private void Update()
    {
        base.Update();


        if (isShieldDepleting)
        {
            shieldEnergy = Mathf.Max(shieldEnergy - Time.deltaTime, 0f);
        }
        else
        {
            shieldEnergy = Mathf.Min(shieldEnergy + Time.deltaTime, maxShieldEnergy);
        }

        if (shieldEnergy == 0f)
        {
            isShieldDepleted = true;
        }

        if (shieldEnergy / maxShieldEnergy == 1f)
        {
            isShieldDepleted = false;
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.5f, 0.5f, 1f, 0.2f);
        Gizmos.DrawSphere(transform.position, shieldEnergy);

        float barHeight = (float)Health.CurrentHealth * 0.005f;
        Gizmos.color = new Color(0f, 1f, 0f, 1f);
        Gizmos.DrawCube(transform.position +
        Vector3.up + (Vector3.up * barHeight / 2), new Vector3(
            0.5f,
            barHeight,
            0.5f)
        );
    }
}
