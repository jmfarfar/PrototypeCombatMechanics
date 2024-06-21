using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovementState : PlayerBaseState
{

    public PlayerMovementState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    private int MovementStateHash = Animator.StringToHash("MovementState");
    private int MovementSpeedHash = Animator.StringToHash("MovementSpeed");

    private int menuSelected;
    Action inputAction;


    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(MovementStateHash, 0.1f);
        stateMachine.ActivateAttackInput();
        ExecuteBufferedAttack();
    }

    public override void Exit()
    {
        stateMachine.isRadialMenuActive = false;
        stateMachine.onRadialMenuActivated?.Invoke(stateMachine.gameObject.layer, menuSelected, false);
        stateMachine.DeactivateAttackInput();

    }

    public override void Tick(float deltaTime)
    {
        ExecuteBufferedAttack();

        Vector2 movementInput = stateMachine.PlayerInput.GetMovementInputVector();

        UpdateAnimation(movementInput);

        Vector3 direction = Camera.main.transform.forward * movementInput.y +
            Camera.main.transform.right * movementInput.x;
        direction.y = 0f;
        direction.Normalize();

        //Vector3 direction = new Vector3(movementInput.x, 0f, movementInput.y);


        if (!(direction == Vector3.zero))
        {
            Quaternion toRotate = Quaternion.LookRotation(direction, Vector3.up);
            stateMachine.transform.rotation = Quaternion.RotateTowards(stateMachine.transform.rotation, toRotate, Time.deltaTime * stateMachine.RotationSpeed);
        }

        if (stateMachine.PlayerInput.GetDashButton())
        {

            stateMachine.SwitchState(new PlayerDashState(stateMachine));
        }
        else if (stateMachine.PlayerInput.GetBlockButton() && !stateMachine.isShieldDepleted)
        {
            stateMachine.SwitchState(new PlayerBlockState(stateMachine));
        }

        stateMachine.CharacterController.Move(direction * stateMachine.Speed * Time.deltaTime);
    }

    private void UpdateAnimation(Vector2 movementInput)
    {
        float movementFactor = Mathf.Ceil(movementInput.sqrMagnitude);

        stateMachine.Animator.SetFloat(MovementSpeedHash,
        Mathf.Lerp(stateMachine.Animator.GetFloat(MovementSpeedHash),
        movementFactor,
        Time.deltaTime * 10f
        ));
    }

    private void ExecuteBufferedAttack()
    {
        stateMachine.bufferedAttack?.Invoke();
        stateMachine.bufferedAttack = null;
    }
}
