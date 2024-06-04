using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class Player2StunState : Player2BaseState
{
    private int StunHash = Animator.StringToHash("Stun");
    private float duration;

    public Player2StunState(Player2StateMachine stateMachine, float duration) : base(stateMachine)
    {
        this.duration = duration;
    }

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(StunHash, 0.1f);
    }

    public override void Exit()
    {

    }

    public override void Tick(float deltaTime)
    {
        stateMachine.CharacterController.Move(Vector3.zero * deltaTime);

        duration -= deltaTime;

        if (duration <= 0f)
        {
            stateMachine.SwitchState(new Player2MovementState(stateMachine));
        }
    }
}
