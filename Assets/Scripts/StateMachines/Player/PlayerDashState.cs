using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerBaseState
{
    int dashHash = Animator.StringToHash("Dashing");
    public PlayerDashState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(dashHash, 0.1f);

        Vector2 movementInput = stateMachine.PlayerInput.GetMovementInputVector();

        Vector3 direction = Camera.main.transform.forward * movementInput.y +
            Camera.main.transform.right * movementInput.x;
        direction.y = 0f;
        direction.Normalize();

        if (!(direction == Vector3.zero))
        {
            Quaternion toRotate = Quaternion.LookRotation(direction, Vector3.up);
            stateMachine.transform.rotation = toRotate;
        }

        stateMachine.StartCoroutine(Dash());
    }

    public override void Exit()
    {

    }

    public override void Tick(float deltaTime)
    {


    }

    private IEnumerator Dash()
    {
        float timer = 0f;

        float dashSpeed = stateMachine.Speed * stateMachine.DashForce;

        while (timer < stateMachine.dashTime)
        {
            if (timer < stateMachine.dashTime * stateMachine.dashInvulnerabilityTime)
            {
                stateMachine.colliderState = Hitbox.ColliderStates.INACTIVE;
            }
            else
            {
                stateMachine.colliderState = Hitbox.ColliderStates.ACTIVE;
            }

            if (timer < stateMachine.dashTime * 0.5f)
            {
                stateMachine.ActivateAttackInput();
            }

            float speed = stateMachine.dashAccelerationCurve.Evaluate(timer/stateMachine.dashTime) * dashSpeed;

            stateMachine.CharacterController.Move(stateMachine.transform.forward * speed * Time.deltaTime);

            timer += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        stateMachine.SwitchState(new PlayerMovementState(stateMachine));
    }
}
