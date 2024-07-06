using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStunState : PlayerBaseState
{
    private int StunHash = Animator.StringToHash("Stun");
    private float duration;

    public PlayerStunState(PlayerStateMachine stateMachine, float duration) : base(stateMachine)
    {
        this.duration = duration;
    }

    public override void Enter()
    {
        stateMachine.Animator.fireEvents = false;
        stateMachine.Animator.CrossFadeInFixedTime(StunHash, 0.1f);
    }

    public override void Exit()
    {
        stateMachine.Animator.fireEvents = true;
    }

    public override void Tick(float deltaTime)
    {
        stateMachine.CharacterController.Move(Vector3.zero);

        duration -= deltaTime;

        if (duration <= 0f)
        {
            stateMachine.SwitchState(new PlayerMovementState(stateMachine));
        }
    }
}
