using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerBlockState : PlayerBaseState
{
    int blockHash = Animator.StringToHash("Block");
    int stunHash = Animator.StringToHash("Stun");
    float startTime;


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
        stateMachine.colliderState = Hitbox.ColliderStates.ACTIVE;
        stateMachine.shieldHit -= OnShieldHit;

        stateMachine.isShieldDepleting = false;
    }

    public override void Tick(float deltaTime)
    {
        if (stateMachine.colliderState != Hitbox.ColliderStates.SHIELDSTUN)
        {
            if (!stateMachine.PlayerInput.GetBlockButton() || stateMachine.isShieldDepleted)
            {
                stateMachine.SwitchState(new PlayerMovementState(stateMachine));
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

}
