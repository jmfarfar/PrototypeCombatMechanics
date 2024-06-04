using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Player1BlockState : Player1BaseState
{
    int blockHash = Animator.StringToHash("Block");
    int stunHash = Animator.StringToHash("Stun");
    float startTime;


    public Player1BlockState(Player1StateMachine stateMachine) : base(stateMachine)
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
            if (!Input.GetKey(KeyCode.I) || stateMachine.isShieldDepleted)
            {
                stateMachine.SwitchState(new Player1MovementState(stateMachine));
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                stateMachine.SwitchState(new Player1AttackState(stateMachine, 2));
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                stateMachine.SwitchState(new Player1DashState(stateMachine));
            }
        }
    }

    void OnShieldHit(AttackInfo attackInfo)
    {
        startTime = Time.time;
        stateMachine.Animator.CrossFadeInFixedTime(stunHash, 0.1f);
        stateMachine.StartCoroutine(ShieldStun(attackInfo.stunTime / 2));

        if (attackInfo.attackEffect == AttackInfo.AttackEffects.SHIELDBREAK)
        {
            stateMachine.shieldEnergy = 0f;
        }
        else
        {
            stateMachine.shieldEnergy -= attackInfo.damage * 0.05f;
        }
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
