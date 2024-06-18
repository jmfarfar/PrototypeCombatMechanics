using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OberAttacks : GenericHitboxBehaviors
{
    [SerializeField] private GameObject sphereHitbox;
    [SerializeField] private Transform enemy;
    [SerializeField] private AttackManager manager;
    public void SampleAttack(AttackData attackData)
    {
        GameObject instance = InstantiateHitbox(sphereHitbox, transform.position, Quaternion.identity, attackData, out Hitbox hitbox);

        hitbox.SetUpHitboxActions(
            () => {/*manager.onActiveFramesEnd += () => {Destroy(instance);};*/},
            FollowTargetOverTime(enemy, instance.transform, attackData.range, attackData.activeTime),
            (Collider other) => {Destroy(instance);},
            () => {/*manager.onActiveFramesEnd -= () => {Destroy(instance);};*/}
        );

        hitbox.gameObject.layer = LayerMask.NameToLayer("Player2Hitbox");
    }
}
