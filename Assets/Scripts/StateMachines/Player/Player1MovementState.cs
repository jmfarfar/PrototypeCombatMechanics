using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1MovementState : Player1BaseState
{
    public Player1MovementState(Player1StateMachine stateMachine) : base(stateMachine)
    {
    }

    private int MovementStateHash = Animator.StringToHash("MovementState");
    private int MovementSpeedHash = Animator.StringToHash("MovementSpeed");

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(MovementStateHash, 0.1f);
    }

    public override void Exit()
    {

    }

    public override void Tick(float deltaTime)
    {
        Vector2 movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        UpdateAnimation(movementInput);

        Vector3 direction = new Vector3(movementInput.x, 0f, movementInput.y);

        if (!(direction == Vector3.zero))
        {
            Quaternion toRotate = Quaternion.LookRotation(direction, Vector3.up);
            stateMachine.transform.rotation = Quaternion.RotateTowards(stateMachine.transform.rotation, toRotate, Time.deltaTime * stateMachine.RotationSpeed);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            
            stateMachine.SwitchState(new Player1DashState(stateMachine));
        }

        else if (Input.GetKeyDown(KeyCode.J))
        {
            
            stateMachine.SwitchState(new Player1AttackState(stateMachine, 0));
        }

        else if (Input.GetKeyDown(KeyCode.K))
        {
            
            stateMachine.SwitchState(new Player1AttackState(stateMachine, 1));
        }

        else if (Input.GetKeyDown(KeyCode.L))
        {
            
            stateMachine.SwitchState(new Player1AttackState(stateMachine, 3));
        }

        else if (Input.GetKey(KeyCode.I) && !stateMachine.isShieldDepleted)
        {
            stateMachine.SwitchState(new Player1BlockState(stateMachine));
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
}
