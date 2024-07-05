using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerBlockState : PlayerBaseState
{
    readonly int blockHash = Animator.StringToHash("Block");
    readonly int stunHash = Animator.StringToHash("Stun");
    readonly int movementStateHash = Animator.StringToHash("MovementState");
    float startTime;
    bool isShieldReleasing;


    public PlayerBlockState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(blockHash, 0.1f);
        stateMachine.colliderState = Hitbox.ColliderStates.BLOCKING;

        stateMachine.shieldHit += OnShieldHit;

        stateMachine.isShieldDepleting = true;
    }

    public override void Exit()
    {
        // stateMachine.StopCoroutine(OnShieldReleaseRoutine());
        stateMachine.StopAllCoroutines();
    }

    public override void Tick(float deltaTime)
    {
        if (isShieldReleasing) { return; }

        if (stateMachine.colliderState != Hitbox.ColliderStates.SHIELDSTUN)
        {
            if (!stateMachine.PlayerInput.GetBlockButton() || stateMachine.isShieldDepleted)
            {
                stateMachine.StartCoroutine(OnShieldReleaseRoutine());
            }

            // if (Input.GetKeyDown(KeyCode.J))
            // {
            //     stateMachine.SwitchState(new Player1AttackState(stateMachine, 2));
            // }

            if (stateMachine.PlayerInput.GetDashButton())
            {
                stateMachine.SwitchState(new PlayerDashState(stateMachine));
            }
        }
    }

    void OnShieldHit(AttackData attackData)
    {
        startTime = Time.time;
        stateMachine.Animator.CrossFadeInFixedTime(stunHash, 0.1f);
        stateMachine.StartCoroutine(ShieldStun(attackData.stunTime / 2));

        for (int i = 0; i < attackData.attackEffects.Length; i++)
        {
            if (attackData.attackEffects[i] == AttackData.AttackEffects.SHIELDBREAK)
            {
                stateMachine.shieldEnergy = 0f;
            }
        }

        stateMachine.shieldEnergy -= attackData.damage * 0.05f;
    }

    IEnumerator ShieldStun(float stunTime)
    {
        while (Time.time < startTime + stunTime)
        {
            yield return new WaitForEndOfFrame();

        }
        stateMachine.colliderState = Hitbox.ColliderStates.BLOCKING;
        stateMachine.Animator.CrossFadeInFixedTime(blockHash, 0.1f);
    }

    IEnumerator OnShieldReleaseRoutine()
    {
        float timer = 0f;

        isShieldReleasing = true;

        stateMachine.shieldHit -= OnShieldHit;

        stateMachine.isShieldDepleting = false;

        stateMachine.Animator.CrossFadeInFixedTime(movementStateHash, stateMachine.ShieldReleaseTime);

        while (timer < stateMachine.ShieldReleaseTime)
        {
            if (timer < stateMachine.ShieldReleaseTime * stateMachine.parryTimePercentage)
            {
                stateMachine.colliderState = Hitbox.ColliderStates.PARRY;
            }
            else
            {
                stateMachine.colliderState = Hitbox.ColliderStates.ACTIVE;
            }

            if (timer < stateMachine.ShieldReleaseTime * 0.5f)
            {
                stateMachine.ActivateAttackInput();
            }

            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        stateMachine.SwitchState(new PlayerMovementState(stateMachine));
    }
}