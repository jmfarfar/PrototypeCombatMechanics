using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Player1AttackState : Player1BaseState
{
    int index;

    public Player1AttackState(Player1StateMachine stateMachine, int attackIndex) : base(stateMachine)
    {
        index = attackIndex;
    }

    private int AttackHash => Animator.StringToHash(stateMachine.AttackManager.attacks[index].name);

    public override void Enter()
    {
        stateMachine.AttackManager.StartAttack(index);
        stateMachine.AttackManager.attackCompletedEvent += ReturnToMovingState;

        stateMachine.StartCoroutine(RotateTowardsCombatant());

        stateMachine.Animator.CrossFadeInFixedTime(AttackHash, 0.1f);
    }

    public override void Exit()
    {
        stateMachine.AttackManager.attackCompletedEvent -= ReturnToMovingState;
        stateMachine.AttackManager.CancelAttack();
        stateMachine.StopAllCoroutines();
    }

    public override void Tick(float deltaTime)
    {
    }

    private IEnumerator RotateTowardsCombatant()
    {
        float startTime = Time.time;
        float rotationTime = 0.5f;

        Vector3 vectorToTarget = (stateMachine.Combatant.transform.position -
            stateMachine.transform.position);
        
        vectorToTarget.y = 0f;

        vectorToTarget.Normalize();
        
        Quaternion targetRotation = Quaternion.LookRotation(
                vectorToTarget,
                Vector3.up
            );
        
        while (Time.time < startTime + rotationTime)
        {
            stateMachine.transform.rotation = Quaternion.Lerp(
                stateMachine.transform.rotation,
                targetRotation,
                Time.deltaTime * 10f
            );

            yield return new WaitForEndOfFrame();
        }
    }

    private void ReturnToMovingState()
    {
        stateMachine.SwitchState(new Player1MovementState(stateMachine));
    }
}
