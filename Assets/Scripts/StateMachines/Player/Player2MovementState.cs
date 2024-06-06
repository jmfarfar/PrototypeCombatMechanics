using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2MovementState : Player2BaseState
{
    public Player2MovementState(Player2StateMachine stateMachine) : base(stateMachine)
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
        Vector2 movementInput = new Vector2(Input.GetAxisRaw("JHorizontal"), Input.GetAxisRaw("JVertical")).normalized;

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

        if (Input.GetKeyDown(KeyCode.JoystickButton0)) // gamepad button south
        {
            stateMachine.SwitchState(new Player2DashState(stateMachine));
        }

        else if (Input.GetKeyDown(KeyCode.Joystick1Button1)) // gamepad button east
        {

            stateMachine.SwitchState(new Player2AttackState(stateMachine, 0));
        }

        else if (Input.GetKeyDown(KeyCode.Joystick1Button2))
        {
            stateMachine.SwitchState(new Player2AttackState(stateMachine, 1));
        }

        else if (Input.GetKeyDown(KeyCode.Joystick1Button3))
        {
            stateMachine.SwitchState(new Player2AttackState(stateMachine, 3));
        }

        else if (Input.GetKey(KeyCode.Joystick1Button4) && !stateMachine.isShieldDepleted)
        {
            stateMachine.SwitchState(new Player2BlockState(stateMachine));
        }

        stateMachine.CharacterController.Move(direction * stateMachine.Speed * Time.deltaTime);
    }

    private void UpdateAnimation(Vector2 movementInput)
    {
        float movementFactor = Mathf.Ceil(movementInput.sqrMagnitude);

        stateMachine.Animator.SetFloat(MovementSpeedHash, Mathf.Lerp(stateMachine.Animator.GetFloat(MovementSpeedHash), movementFactor, Time.deltaTime * 10f));
    }
}
