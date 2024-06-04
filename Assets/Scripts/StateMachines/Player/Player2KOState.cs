using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2KOState : Player2BaseState
{
    private int KOHash = Animator.StringToHash("KOForward");
    public Player2KOState(Player2StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(KOHash, 0.1f);
    }

    public override void Exit()
    {
        
    }

    public override void Tick(float deltaTime)
    {
        stateMachine.CharacterController.Move(Vector3.zero * deltaTime);
        Debug.Log("Dead");
    }
}
