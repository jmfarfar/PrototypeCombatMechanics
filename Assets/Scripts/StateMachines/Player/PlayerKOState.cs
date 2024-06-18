using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKOState : PlayerBaseState
{
    private int KOHash = Animator.StringToHash("KOBack");
    public PlayerKOState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(KOHash, 0.1f);
        stateMachine.Health.enabled = false;
        stateMachine.CharacterController.enabled = false;
        GameManager.Instance.OnPlayerDefeated(stateMachine.gameObject.layer);
    }

    public override void Exit()
    {
    }

    public override void Tick(float deltaTime)
    {
    }
}
