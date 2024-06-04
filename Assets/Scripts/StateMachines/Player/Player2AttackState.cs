using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Player2AttackState : Player2BaseState
{
    int index;

    public Player2AttackState(Player2StateMachine stateMachine, int attackIndex) : base(stateMachine)
    {
        index = attackIndex;
    }

    private int AttackHash => Animator.StringToHash(stateMachine.attacks[index].animationState);

    public override void Enter()
    {
        stateMachine.StartCoroutine(
            AttackStartUp(stateMachine.attacks[index].startupTime));

        stateMachine.StartCoroutine(RotateTowardsCombatant());

        stateMachine.Animator.CrossFadeInFixedTime(AttackHash, 0.1f);
    }

    public override void Exit()
    {
        stateMachine.StopAllCoroutines();
    }

    public override void Tick(float deltaTime)
    {
    }

    private IEnumerator RotateTowardsCombatant()
    {
        float startTime = Time.time;
        float rotationTime = 0.5f;

        Vector3 vectorToTarget = (stateMachine.Combatant.transform.position - stateMachine.transform.position);

        vectorToTarget.y = 0f;
        vectorToTarget.Normalize();

        Quaternion targetRotation = Quaternion.LookRotation(vectorToTarget, Vector3.up);

        while (Time.time < startTime + rotationTime)
        {
            stateMachine.transform.rotation = Quaternion.Lerp(stateMachine.transform.rotation, targetRotation, Time.deltaTime * 10f);

            yield return new WaitForEndOfFrame();
        }
    }
    private IEnumerator AttackStartUp(float time)
    {
        yield return new WaitForSeconds(time);
        ExecuteAttack();
    }

    private void ExecuteAttack()
    {
        GameObject instance = GameObject.Instantiate(
            stateMachine.ProjectilePrefab,
            stateMachine.transform.position,
            Quaternion.identity);

        instance.GetComponent<Hitbox>().attackInfo = stateMachine.attacks[index];
        instance.GetComponent<Hitbox>().Target = stateMachine.Combatant;
        instance.GetComponent<Hitbox>().Owner = stateMachine.gameObject;
        instance.layer = LayerMask.NameToLayer("Player2Hitbox");

        stateMachine.StartCoroutine(AttackRecovery(stateMachine.attacks[index].recoveryTime));
    }

    private IEnumerator AttackRecovery(float time)
    {
        yield return new WaitForSeconds(time);

        stateMachine.SwitchState(new Player2MovementState(stateMachine));
    }
}
