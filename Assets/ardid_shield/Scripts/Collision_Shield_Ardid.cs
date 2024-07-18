using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Collision_Shield_Ardid : MonoBehaviour
{
    [SerializeField] private GameObject damageShieldVFX;
    [SerializeField] private GameObject hitVFX;

    private Material material;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            return;
        }
        var collisionShield = Instantiate(damageShieldVFX, transform) as GameObject;
        var particles = collisionShield.transform.GetChild(0).GetComponent<ParticleSystemRenderer>();
        material = particles.material;
        // material.SetVector("_SphereCenter", collision.contacts[0].point);

        Destroy(collisionShield, 2);

        var hit = Instantiate(hitVFX, transform.position, Quaternion.identity) as GameObject;

        Destroy(hit, 2);
    }
}