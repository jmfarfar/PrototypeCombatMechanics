using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2DashState : Player2BaseState
{
    int dashHash = Animator.StringToHash("Dashing");
    public Player2DashState(Player2StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(dashHash, 0.1f);

        Vector2 movementInput = new Vector2(Input.GetAxisRaw("JHorizontal"), Input.GetAxisRaw("JVertical")).normalized;

        Vector3 direction = new Vector3(movementInput.x, 0f, movementInput.y);

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
            timer += Time.deltaTime;

            if (timer < stateMachine.dashTime * stateMachine.dashInvulnerabilityTime)
            {
                stateMachine.colliderState = Hitbox.ColliderStates.INACTIVE;
            }
            else
            {
                stateMachine.colliderState = Hitbox.ColliderStates.ACTIVE;
            }

            float speed = stateMachine.dashAccelerationCurve.Evaluate(timer/stateMachine.dashTime) * dashSpeed;

            stateMachine.CharacterController.Move(stateMachine.transform.forward * speed * Time.deltaTime);

            yield return new WaitForEndOfFrame();
        }

        stateMachine.SwitchState(new Player2MovementState(stateMachine));
    }
}
