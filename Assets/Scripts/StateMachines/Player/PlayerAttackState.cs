using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class PlayerAttackState : PlayerBaseState
{
    int index;
    AnimatorOverrideController animatorOverrideController;

    public PlayerAttackState(PlayerStateMachine stateMachine, int attackIndex) : base(stateMachine)
    {
        index = attackIndex;
    }

    private int AttackHash => Animator.StringToHash("AttackState");

    public override void Enter()
    {
        stateMachine.AttackManager.StartAttack(index);
        stateMachine.AttackManager.attackCompletedEvent += ReturnToMovingState;

        stateMachine.StartCoroutine(RotateTowardsCombatant());

        animatorOverrideController = new AnimatorOverrideController(stateMachine.Animator.runtimeAnimatorController);
        animatorOverrideController["BasicAttackAnimation"] = stateMachine.AttackManager.attacks[index].attackAnimation;
        stateMachine.Animator.runtimeAnimatorController = animatorOverrideController;

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
        stateMachine.SwitchState(new PlayerMovementState(stateMachine));
    }
}
