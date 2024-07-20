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
    public SphereCollider ShieldCollider { get; set; }
    [field: SerializeField]
    public float Speed { get; set; }
    [field: SerializeField]
    public float DashForce { get; set; }
    [field: SerializeField] public Health Health { get; private set; }
    [field: SerializeField]
    public float RotationSpeed { get; set; }

    [field: SerializeField] public GameObject Combatant { get; private set; }
    [field: SerializeField] public Animator Animator { get; private set; }

    public float dashTime = 0.25f;
    public float dashCooldown = 1f;

    public float dashDistance = 1f;
    public float dashInvulnerabilityTime;
    public AnimationCurve dashAccelerationCurve;

    public Hitbox.ColliderStates colliderState;

    public Action<AttackData> shieldHit;
    public Action<Vector3> rollEvent;
    public Action<LayerMask> playerKO;

    public float maxShieldEnergy = 1f;
    [HideInInspector] public float shieldEnergy;
    public bool isShieldDepleting;
    public bool isShieldDepleted;
    public bool isRolling;
    [field: SerializeField] public bool IsChannelComplete { get; private set; }
    [field: SerializeField] public float ChargeTime { get; private set; } = 10f;

    public bool isRadialMenuActive;

    // Attack Manager
    [Header("Attack Manager")]
    public AttackManager AttackManager;

    [Header("Restrained State Exit threshold")]
    public float restrainedStateExitThreshold = 50f;

    [HideInInspector] public Action<LayerMask, string, int> onRadialMenuExecuted;
    [HideInInspector] public Action<LayerMask, int, bool> onRadialMenuActivated;

    [field: SerializeField] public float ShieldReleaseTime { get; private set; }
    [field: SerializeField] public int ParryFrameWindow { get; private set; } = 2;
    public bool HasParried {get; private set;}

    private Action processAttackInput;
    public Action bufferedAttack;
    private int menuSelected;
    
    #endregion

    private void Start()
    {
        PlayerInput = GetComponent<IPlayerInput>();

        AttackManager.activateAttackInput += ActivateAttackInput;

        GameManager.Instance.gameStart += OnGameStart;
        GameManager.Instance.onPlayerDefeated += OnPlayerDefeated;

        shieldEnergy = maxShieldEnergy;
        isRadialMenuActive = false;
    }

    private void OnPlayerDefeated(LayerMask layerMask)
    {
        PlayerInput = new EmptyPlayerInput();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (colliderState == Hitbox.ColliderStates.INACTIVE || !other.TryGetComponent<Hitbox>(out Hitbox hitbox))
        {
            return;
        }

        if (colliderState == Hitbox.ColliderStates.PARRY)
        {
            Debug.Log($"Parried {hitbox.attackData.name}");
            Physics.IgnoreCollision(GetComponent<Collider>(), other);
            GameManager.Instance.HitPause();
            SwitchState(new PlayerMovementState(this));
            HasParried = true;
            colliderState = Hitbox.ColliderStates.INACTIVE;
            Invoke(nameof(FinishParryInvulnerability), 0.1f);
        }
        else if (colliderState == Hitbox.ColliderStates.RESTRAINED)
        {
            Health.DecreaseHealth(hitbox.attackData.damage);
        }
        else if (colliderState == Hitbox.ColliderStates.BLOCKING)
        {
            colliderState = Hitbox.ColliderStates.SHIELDSTUN;
            shieldHit?.Invoke(hitbox.attackData);

            for (int i = 0; i < hitbox.attackData.attackEffects.Length; i++)
            {
                if (hitbox.attackData.attackEffects[i] == AttackData.AttackEffects.RESTRAIN)
                {
                    SwitchState(new PlayerRestrainedState(this));
                    return;
                }
            }
        }
        else
        {
            for (int i = 0; i < hitbox.attackData.attackEffects.Length; i++)
            {
                if (hitbox.attackData.attackEffects[i] == AttackData.AttackEffects.KNOCKDOWN)
                {
                    Health.DecreaseHealth(hitbox.attackData.damage);

                    if (Health.CurrentHealth <= 0f)
                    {
                        SwitchState(new PlayerKOState(this));
                        return;
                    }

                    Vector3 hitDirection =
                        transform.position - other.transform.position;
                    hitDirection.y = 0f;

                    SwitchState(new PlayerKnockdownState(this, 1f, hitDirection.normalized));
                    return;
                }

                if (hitbox.attackData.attackEffects[i] == AttackData.AttackEffects.RESTRAIN)
                {
                    SwitchState(new PlayerRestrainedState(this));
                    return;
                }
            }
            Health.DecreaseHealth(hitbox.attackData.damage);

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

    private void OnGameStart()
    {
        SwitchState(new PlayerMovementState(this));
    }

    private void OnDestroy()
    {
        GameManager.Instance.gameStart -= OnGameStart;
        GameManager.Instance.onPlayerDefeated -= OnPlayerDefeated;
        AttackManager.activateAttackInput -= ActivateAttackInput;
    }

    private void Update()
    {
        base.Update();

        processAttackInput?.Invoke();

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
        if (colliderState == Hitbox.ColliderStates.PARRY)
        {
            Gizmos.color = new(1f, 1f, 1f, 0.3f);
        }
        Gizmos.DrawSphere(transform.position, shieldEnergy);

        float barHeight = (float)Health.CurrentHealth / (float)Health.MaxHealth * 2f;
        Gizmos.color = new Color(0f, 1f, 0f, 1f);
        Gizmos.DrawCube(transform.position +
        Vector3.up + (Vector3.up * barHeight / 2), new Vector3(
            0.3f,
            barHeight,
            0.3f)
        );
    }

    public void ActivateAttackInput()
    {

        if (processAttackInput != null) { return; }
        processAttackInput = GetActivateRadialMenu();
    }

    public void DeactivateAttackInput()
    {
        processAttackInput = null;
    }

    private Action GetActivateRadialMenu()
    {
        return () =>
        {
            if (PlayerInput.GetFirstRadialMenuButton())
            {
                ActivateRadialMenu(0);
                processAttackInput = GetExecuteRadialMenu();
            }
            else if (PlayerInput.GetSecondRadialMenuButton())
            {
                ActivateRadialMenu(1);
                processAttackInput = GetExecuteRadialMenu();
            }
            else if (PlayerInput.GetThirdRadialMenuButton())
            {
                ActivateRadialMenu(2);
                processAttackInput = GetExecuteRadialMenu();
            }
        };
    }

    private void ActivateRadialMenu(int menuIndex)
    {
        isRadialMenuActive = !isRadialMenuActive;

        if (isRadialMenuActive)
        {
            onRadialMenuActivated?.Invoke(gameObject.layer, menuIndex, true);
            menuSelected = menuIndex;
        }
    }

    private void ExecuteRadialMenu(int menuIndex)
    {
        int cuadrant = PlayerInput.GetRadialMenuQuadrant();

        int currentSpell = menuIndex switch
        {
            0 => 0, // spells for menu X
            1 => 4, // spells for menu Y
            2 => 8, // spells for menu B
            _ => 0,
        };

        int spellNumber = cuadrant + currentSpell;

        if (spellNumber > currentSpell + 3) return;

        onRadialMenuExecuted?.Invoke(gameObject.layer, AttackManager.attacks[spellNumber].name, cuadrant);

        bufferedAttack = () => {
            SwitchState(new PlayerAttackState(this, spellNumber));
        };

    }

    private Action GetExecuteRadialMenu()
    {
        return () =>
        {
            ExecuteRadialMenu(menuSelected);
        };
    }

    public void ChannelComplete()
    {
        IsChannelComplete = true;
    }

    public void ResetChargeTime()
    {
        ChargeTime = 10f;
        IsChannelComplete = false;
    }

    public void DecreaseChargeTime()
    {
        ChargeTime -= Time.deltaTime;
    }

    void FinishParryInvulnerability()
    {
        HasParried = false;
        colliderState = Hitbox.ColliderStates.ACTIVE;
    }
}

