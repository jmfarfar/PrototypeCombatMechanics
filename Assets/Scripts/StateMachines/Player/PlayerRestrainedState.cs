using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRestrainedState : PlayerBaseState
{
    Vector2 lastVector = Vector2.zero;
    float restrainValue;

    public PlayerRestrainedState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.colliderState = Hitbox.ColliderStates.RESTRAINED;

        restrainValue = stateMachine.restrainedStateExitThreshold;
    }

    public override void Tick(float deltaTime)
    {
        restrainValue -= Time.deltaTime * 5f;

        restrainValue -= CalculateInputDifference();

        if (restrainValue <= 0f)
        {
            stateMachine.SwitchState(new PlayerMovementState(stateMachine));
        }

        Debug.Log(restrainValue);
    }

    public override void Exit()
    {
        stateMachine.colliderState = Hitbox.ColliderStates.ACTIVE;
    }

    private float CalculateInputDifference()
    {
        Vector2 inputVector = stateMachine.PlayerInput.GetMovementInputVector();

        Vector2 simplifiedVector = new (
            Mathf.Round(inputVector.x),
            Mathf.Round(inputVector.y)
        );

        float inputDifference = Mathf.Floor((simplifiedVector - lastVector).magnitude);

        lastVector = simplifiedVector;

        return inputDifference;
    }
}
