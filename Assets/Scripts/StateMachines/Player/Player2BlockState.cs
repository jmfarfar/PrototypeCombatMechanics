using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2BlockState : Player2BaseState
{
    int blockHash = Animator.StringToHash("Block");
    int stunHash = Animator.StringToHash("Stun");
    float startTime;

    public Player2BlockState(Player2StateMachine stateMachine) : base(stateMachine)
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
            if(!Input.GetKey(KeyCode.Joystick1Button4) || stateMachine.isShieldDepleted)
            {
                stateMachine.SwitchState(new Player2MovementState(stateMachine));
            }

            if (Input.GetKeyDown(KeyCode.Joystick1Button1))
            {
                stateMachine.SwitchState(new Player2AttackState(stateMachine, 2));
            }

            if (Input.GetKeyDown(KeyCode.Joystick1Button0))
            {
                stateMachine.SwitchState(new Player2DashState(stateMachine));
            } 
        }
    }

    void OnShieldHit(AttackData attackData)
    {
        startTime = Time.time;
        stateMachine.Animator.CrossFadeInFixedTime(stunHash, 0.1f);
        stateMachine.StartCoroutine(ShieldStun(attackData.stunTime / 2f));  

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
        while(Time.time < startTime + stunTime)
        {
            yield return new WaitForEndOfFrame();

        }
        stateMachine.colliderState = Hitbox.ColliderStates.BLOCKING;
        stateMachine.Animator.CrossFadeInFixedTime(blockHash, 0.1f);
    }
}
