using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKnockdownState : PlayerBaseState
{
    private int KnockdownHash = Animator.StringToHash("Knockdown");
    private int GetupHash = Animator.StringToHash("GetUp");
    private int RollHash = Animator.StringToHash("Roll");
    private float duration;
    private Vector3 direction;
    float startTime;
    private Func<bool> currentRoutine;

    public PlayerKnockdownState(PlayerStateMachine stateMachine, float duration, Vector3 direction) : base(stateMachine)
    {
        this.duration = duration;
        this.direction = direction;
    }

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(KnockdownHash, 0.1f);
        currentRoutine = KnockdownSubroutine();
    }

    public override void Exit()
    {
        stateMachine.Animator.CrossFadeInFixedTime(GetupHash, 0.1f);
    }

    public override void Tick(float deltaTime)
    {
        bool hasFinished = currentRoutine.Invoke();
        if(hasFinished) stateMachine.SwitchState(new PlayerMovementState(stateMachine));
    }

    private Func<bool> KnockdownSubroutine()
    {
        float localDuration = duration;
        return () =>
        {
            if (localDuration > 0f)
            {
                stateMachine.CharacterController.Move(direction *
                    (stateMachine.Speed / 2f * Time.deltaTime));
            }
            else
            {
                currentRoutine = HitGroundSubroutine();
            }

            localDuration -= Time.deltaTime;
            return false;
        };
    }

    private Func<bool> HitGroundSubroutine()
    {
        Vector2 inputVector =
            stateMachine.PlayerInput.GetMovementInputVector();

        float localDuration = duration;

        // GET UP ROUTINE
        if (inputVector.sqrMagnitude < 0.1f)
        {
            stateMachine.Animator.CrossFadeInFixedTime(GetupHash, 0.1f);
            return () =>
            {
                localDuration -= Time.deltaTime;
                return !(localDuration > 0f);
            };
        }
        
        // ROLL ROUTINE
        Vector3 movementDirection = new(inputVector.x, 0f, inputVector.y);
        stateMachine.Animator.CrossFadeInFixedTime(RollHash, 0.1f);
        
        return () =>
        {
            stateMachine.CharacterController.Move(movementDirection.normalized *
                                                  stateMachine.Speed * Time.deltaTime);
            localDuration -= Time.deltaTime;
            return localDuration <= 0f;
        };
    }
}
