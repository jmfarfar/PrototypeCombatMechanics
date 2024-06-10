using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditorInternal;
using UnityEngine;

public class PlayerStateMachine : StateMachine, IPlayerStateMachine
{
    #region Properties
    [field: SerializeField] public IPlayerInput PlayerInput { get; private set; }
    
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

    public AttackData[] attacks;
    [HideInInspector] public Collider colliderComponent => gameObject.GetComponent<Collider>();

    public float dashTime = 0.25f;
    public float dashCooldown = 1f;

    public float dashDistance = 1f;
    public float dashInvulnerabilityTime;
    public AnimationCurve dashAccelerationCurve;

    public Hitbox.ColliderStates colliderState;

    public Action<AttackData> shieldHit;
    public Action<Vector3> rollEvent;

    public float maxShieldEnergy = 1f;
    [HideInInspector] public float shieldEnergy;
    public bool isShieldDepleting;
    public bool isShieldDepleted;
    public bool isRolling;

    // Radial Menu
    [field: SerializeField] public GameObject[] RadialMenu { get; private set; }
    [field: SerializeField] public string[] SpellNames { get; private set; }

    public bool isRadialMenuActive;
    public Transform selectedObject;
    public Transform menuCenter;
    public TextMeshProUGUI highlightedSpellName;

    // Attack Manager
    [Header("Attack Manager")]
    public AttackManager AttackManager;


    #endregion

    private void Start()
    {
        PlayerInput = GetComponent<IPlayerInput>();

        SwitchState(new PlayerMovementState(this));

        shieldEnergy = maxShieldEnergy;
        isRadialMenuActive = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (colliderState == Hitbox.ColliderStates.INACTIVE || !other.TryGetComponent<Hitbox>(out Hitbox hitbox))
        {
            return;
        }

        else if (colliderState == Hitbox.ColliderStates.BLOCKING)
        {
            // Physics.IgnoreCollision(colliderComponent, other, true);
            colliderState = Hitbox.ColliderStates.SHIELDSTUN;
            shieldHit?.Invoke(hitbox.attackData);
        }

        else
        {
            for (int i = 0; i < hitbox.attackData.attackEffects.Length; i++)
            {
                if (hitbox.attackData.attackEffects[i] == AttackData.AttackEffects.KNOCKDOWN)
                {
                    Debug.Log("Collided and knocked down");
                    Health.DecreaseHealth(hitbox.attackData.damage);

                    if (Health.CurrentHealth <= 0f)
                    {
                        SwitchState(new PlayerKOState(this));
                        return;
                    }

                    // Physics.IgnoreCollision(colliderComponent, other, true);
                    SwitchState(new PlayerKnockdownState(this, 3f));
                    return;
                }
            }
            Debug.Log("Collided");
            Health.DecreaseHealth(hitbox.attackData.damage);

            // Physics.IgnoreCollision(colliderComponent, other, true);

            if (Health.CurrentHealth <= 0)
            {
                SwitchState(new PlayerKOState(this));
            }
            else
            {
                SwitchState(new PlayerStunState(this, hitbox.attackData.stunTime));
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
