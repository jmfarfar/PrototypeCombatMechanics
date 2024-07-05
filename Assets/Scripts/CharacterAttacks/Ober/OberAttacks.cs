using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using StateMachines.Player;
using TreeEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;

public class OberAttacks : GenericHitboxBehaviors
{
    [SerializeField] private GameObject sphereHitbox;
    [SerializeField] private GameObject fireBreathHitbox;
    [SerializeField] private GameObject salamanderNest;
    [SerializeField] private GameObject lavaFloor;
    [SerializeField] private GameObject impactZone;
    [SerializeField] private GameObject firewall;
    [SerializeField] private GameObject bloodMoon;
    [SerializeField] private bool isFuryActive;
    [SerializeField] private Transform enemy;
    [SerializeField] private AttackManager manager;

    public List<BloodMoonConfig> projectiles;
    public List<salamanderConfig> salamanders;
    public PlacementStrategy placementStrategy;
    Spawn salamander;
    Spawn projectile;

    float timeCounter = 0f;

    public float maxSpawns = 3;

    [Serializable]
    public struct salamanderConfig
    {
        public GameObject spawnPrefab;
        public int health;
        public int damage;
        public float attackRange;
    };

    [Serializable]
    public struct BloodMoonConfig
    {
        public GameObject projectile;
        public int damage;
    }

    public void SampleAttack(AttackData attackData)
    {
        GameObject instance = InstantiateHitbox(sphereHitbox, transform.position, Quaternion.identity, attackData,
            out Hitbox hitbox);

        hitbox.SetUpHitboxActions(
            () =>
            {
                //manager.onActiveFramesEnd += () => { Destroy(instance); }; 
                if (isFuryActive)
                {
                    attackData.damage *= 2;
                }
            },
            FollowTargetOverTime(enemy, instance.transform, attackData.range, attackData.activeTime),
            (Collider other) => { },
            () =>
            {
                //manager.onActiveFramesEnd -= () => { Destroy(instance); }; 
            }
        );

        hitbox.gameObject.layer = AssignLayerToHitbox(gameObject.layer);
    }

    #region FIREBREATH

    public void FireBreath(AttackData attackData)
    {
        GameObject instance = InstantiateHitbox(fireBreathHitbox,
            transform.position + (transform.forward * attackData.range), transform.rotation, attackData,
            out Hitbox hitbox);
        AudioManager.Instance.PlaySound(attackData.attackSFX);

        hitbox.SetUpHitboxActions(
            () =>
            {
                if (isFuryActive)
                {
                    attackData.damage *= 2;
                }

                manager.onActiveFramesEnd += () => { Destroy(instance); };
            },
            () => { },
            (Collider other) =>
            {
                //StartCoroutine(BurnRoutine(3f, 10f, ));
                Debug.Log("Hit by Fire Breath");
            },
            () => { manager.onActiveFramesEnd -= () => { Destroy(instance); }; }
        );

        hitbox.gameObject.layer = AssignLayerToHitbox(gameObject.layer);
    }

    #endregion

    #region SALAMANDER NEST

    public void SalamanderNest(AttackData attackData)
    {
        GameObject nestInstance = InstantiateHitbox(salamanderNest, transform.position + (Vector3.forward * 2),
            Quaternion.identity, attackData, out Hitbox hitbox);

        hitbox.SetUpHitboxActions(
            () =>
            {
                //SpawnSalamanders(attackData);
                StartCoroutine(SpawnSalamanders(attackData, nestInstance));
            },
            () => { },
            (Collider other) => { }
        );

        hitbox.gameObject.layer = AssignLayerToHitbox(gameObject.layer);
    }

    IEnumerator SpawnSalamanders(AttackData attackData, GameObject nest)
    {
        for (int i = 0; i < maxSpawns; i++)
        {
            salamander = Create(salamanders[i % salamanders.Count], nest);
            salamander.transform.position = placementStrategy.SetPosition(nest.transform.position);

            // Salamander behavior
            salamander.GetComponent<Hitbox>().SetUpHitboxActions(
                () => { },
                FollowTargetOverTime(enemy, salamander.transform, attackData.range, attackData.activeTime),
                (Collider other) => { Destroy(salamander.gameObject); }
            );

            salamander.GetComponent<Hitbox>().gameObject.layer = AssignLayerToHitbox(gameObject.layer);
            yield return new WaitForSeconds(5);
        }

        Destroy(nest.gameObject);
    }

    private Spawn Create(salamanderConfig config, GameObject nest)
    {
        GameObject spawn = Instantiate(config.spawnPrefab, nest.transform.position, Quaternion.identity);
        Spawn spawnComponent = spawn.GetComponent<Spawn>();
        return spawnComponent;
    }

    #endregion

    #region IMPETUS

    // Impetus

    public void Impetus(AttackData attackData)
    {
        Debug.Log(attackData.activeTime);
        StartCoroutine(IncreaseMovementSpeed(attackData.activeTime));
    }

    IEnumerator IncreaseMovementSpeed(float duration)
    {
        Debug.Log("Enter Increase Movement Speed");
        PlayerStateMachine player = GetComponent<PlayerStateMachine>();
        float originalSpeed = player.Speed;
        player.Speed *= 2;
        yield return new WaitForSeconds(duration);
        player.Speed = originalSpeed;
        Debug.Log("Exit Increase Movement Speed");
    }

    #endregion

    #region LAVA_FLOOR

    public void LavaFloor(AttackData attackData)
    {
        GameObject lavaInstance = InstantiateHitbox(lavaFloor,
            transform.position + (transform.forward * attackData.range), Quaternion.identity, attackData,
            out Hitbox hitbox);

        hitbox.SetUpHitboxActions(
            () => { },
            () => { },
            (Collider other) => { },
            (Collider other) =>
            {
                if (other.TryGetComponent<PlayerStateMachine>(out PlayerStateMachine opponent))
                {
                    StartCoroutine(LavaFloorLifetime(attackData, lavaInstance.gameObject, opponent));
                    opponent.GetComponent<Health>().DecreaseHealth(attackData.damage);
                }
            },
            () => { }
        );

        hitbox.gameObject.layer = AssignLayerToHitbox(gameObject.layer);
    }

    IEnumerator LavaFloorLifetime(AttackData data, GameObject instance, PlayerStateMachine opponent)
    {
        //float start = Time.time;

        //while (Time.time < start + 1f)
        //{
        //    opponent.GetComponent<Health>().DecreaseHealth(data.damage);
        //    yield return new WaitForEndOfFrame();

        //}
        yield return new WaitForSeconds(data.activeTime);
        Destroy(instance);
    }

    #endregion

    #region JUMP

    public void Jump(AttackData attackData)
    {
        GameObject impactInstance = InstantiateHitbox(impactZone, transform.position, Quaternion.identity, attackData,
            out Hitbox hitbox);


        float time = attackData.activeTime;
        GameObject parent = new GameObject("rotationParent");
        parent.transform.position = transform.position;
        Vector3 midPoint = (transform.position + enemy.position - new Vector3(1f, 1f, 0f)) / 2;

        hitbox.SetUpHitboxActions(
            () =>
            {
                impactInstance.transform.parent = gameObject.transform;
                impactInstance.GetComponent<MeshRenderer>().enabled = false;
            },
            () =>
            {
                time -= Time.deltaTime;

                if (time > 0f)
                {
                    Debug.Log("Start");
                    parent.transform.RotateAround(midPoint, transform.right,
                        (175f * Time.deltaTime) / attackData.activeTime);

                    transform.position = parent.transform.position;
                }
                else
                {
                    Debug.Log("Complete");
                    impactInstance.GetComponent<MeshRenderer>().enabled = true;
                    manager.CompleteAttack();
                    impactInstance.transform.parent = null;
                }

                Debug.Log("Exit");
            },
            (Collider other) => { },
            (Collider other) =>
            {
                if (other.TryGetComponent<PlayerStateMachine>(out PlayerStateMachine opponent))
                {
                    StartCoroutine(LavaFloorLifetime(attackData, impactInstance.gameObject, opponent));
                }
                //Destroy(impactInstance);
            },
            () => { }
        );

        hitbox.gameObject.layer = AssignLayerToHitbox(gameObject.layer);
    }

    #endregion

    #region FURY

    public void Fury(AttackData attackData)
    {
        StartCoroutine(ToggleFury(attackData));
    }

    IEnumerator ToggleFury(AttackData data)
    {
        isFuryActive = true;
        yield return new WaitForSeconds(data.activeTime);
        isFuryActive = false;
    }

    #endregion

    #region SCALE_HARDNESS

    public void ScaleHardness(AttackData attackData)
    {
        StartCoroutine(ReduceDamage(attackData));
    }

    IEnumerator ReduceDamage(AttackData attackData)
    {
        if (TryGetComponent<PlayerStateMachine>(out PlayerStateMachine player))
        {
            AttackData[] opponentAttackData = player.Combatant.GetComponentInChildren<AttackManager>().attacks;
            int[] backup = new int[opponentAttackData.Length];

            for (int i = 0; i < opponentAttackData.Length; i++)
            {
                backup[i] = opponentAttackData[i].damage;
            }


            foreach (AttackData attack in opponentAttackData)
            {
                attack.damage = (int)Mathf.Round(attack.damage * 0.7f); // 30% damage reduction
            }

            yield return new WaitForSeconds(attackData.activeTime);

            for (int i = 0; i < backup.Length; i++)
            {
                opponentAttackData[i].damage = backup[i];
            }
        }
    }

    #endregion

    #region FIREWALL

    public void FireWall(AttackData attackData)
    {
        GameObject wallInstance = InstantiateHitbox(firewall, transform.position + Vector3.left,
            Quaternion.LookRotation(transform.position, Vector3.up), attackData, out Hitbox hitbox);

        hitbox.SetUpHitboxActions(
            () => { StartCoroutine(SpawnWall(attackData, wallInstance)); },
            () => { },
            (Collider other) =>
            {
                //Destroy(wallInstance);
            }
        );

        hitbox.gameObject.layer = LayerMask.NameToLayer("Player2Hitbox");
    }

    IEnumerator SpawnWall(AttackData data, GameObject instance)
    {
        yield return new WaitForSeconds(data.activeTime);
        Destroy(instance);
    }

    #endregion

    #region BLOODMOON

    public void BloodMoon(AttackData attackData)
    {
        GameObject moonInstance = InstantiateHitbox(bloodMoon, transform.position + (Vector3.up * 4),
            Quaternion.identity, attackData, out Hitbox hitbox);

        hitbox.SetUpHitboxActions(
            () => { StartCoroutine(SpawnProjectiles(attackData, moonInstance)); },
            () => { },
            (Collider other) => { }
        );

        hitbox.gameObject.layer = LayerMask.NameToLayer("Player2Hitbox");
    }

    IEnumerator SpawnProjectiles(AttackData attackData, GameObject bloodMoon)
    {
        for (int i = 0; i < maxSpawns; i++)
        {
            projectile = Create(projectiles[i % projectiles.Count], bloodMoon);

            projectile.transform.position = bloodMoon.transform.position;

            projectile.GetComponent<Hitbox>().SetUpHitboxActions(
                () => { },
                FollowTargetOverTime(enemy, projectile.transform, attackData.range, attackData.activeTime),
                (Collider other) => { Destroy(projectile.gameObject); }
            );

            projectile.GetComponent<Hitbox>().gameObject.layer = LayerMask.NameToLayer("Player2Hitbox");
            yield return new WaitForSeconds(5);
        }

        Destroy(bloodMoon);
    }

    private Spawn Create(BloodMoonConfig config, GameObject moon)
    {
        GameObject spawn = Instantiate(config.projectile, moon.transform.position + (Vector3.forward * 3),
            Quaternion.identity);
        Spawn spawnComponent = spawn.GetComponent<Spawn>();
        return spawnComponent;
    }

    #endregion

    #region CONCENTRATION

    public void Concentration(AttackData attackData)
    {
        // float chargeTime = timeCounter > 0f ? timeCounter : attackData.activeTime * 100f;

        PlayerStateMachine playerStateMachine = GetComponent<PlayerStateMachine>();

        Debug.Log("Before while");

        Debug.Log("Charging");
        
        // stop movement and enter charge state
        if (!playerStateMachine.IsChannelComplete)
        {
            playerStateMachine.SwitchState(new PlayerChannelState(playerStateMachine, attackData));
        }
        // suspend charge when changing to block state and return to movement state
        else
        {
            Debug.Log("Start coroutine");
            // StartCoroutine(FireConcentration(playerStateMachine, attackData));
            FireConcentration(playerStateMachine, attackData);
            playerStateMachine.ResetChargeTime();
        }
    }

    private void FireConcentration(PlayerStateMachine playerStateMachine, AttackData attackData)
    {
        Debug.Log("Begin coroutine");
        // float time = Time.time;

        Debug.Log("Fire Concentration");
        // playerStateMachine.Speed = 10f;

        GameObject instance = InstantiateHitbox(sphereHitbox, transform.position, Quaternion.identity, attackData,
            out Hitbox hitbox);
        //while (Time.time < time + attackData.activeTime + 1f)
        //{


        hitbox.SetUpHitboxActions(
            () => { },
            FollowTargetOverTime(enemy, instance.transform, attackData.range, attackData.activeTime),
            (Collider other) => { Destroy(instance); }
        );
        hitbox.gameObject.layer = AssignLayerToHitbox(gameObject.layer);

        //}
        // yield return new WaitForEndOfFrame();
    }

    #endregion


    #region PYROMANIAC

    public void Pyromaniac(AttackData attackData)
    {
        StartCoroutine(SpawnFirePyro(attackData));
    }

    IEnumerator SpawnFirePyro(AttackData attackData)
    {
        for (int i = 0; i < 10 * attackData.activeTime; i++)
        {
            GameObject instance = InstantiateHitbox(sphereHitbox, transform.position, Quaternion.identity, attackData,
                out Hitbox hitbox);
            //pyro.transform.position = transform.position;/* placementStrategy.SetPosition(transform.position);*/
            Vector3 midPoint = new Vector3();
            Vector3 randomPosition = placementStrategy.SetPosition(transform.position);
            Vector3 vectorRandPos = (transform.position - randomPosition).normalized;
            Vector3 offsetPoint = new Vector3();

            // Pyro behavior
            hitbox.SetUpHitboxActions(
                () =>
                {
                    midPoint = (transform.position + randomPosition) / 2;
                    offsetPoint = new Vector3(midPoint.x, -Vector3.Distance(midPoint, transform.position) + 1f,
                        midPoint.z);
                },
                () =>
                {
                    float angle = (180f * Time.deltaTime) / 0.5f;
                    Vector3 axis = Vector3.Cross(vectorRandPos, Vector3.up);
                    instance.transform.RotateAround(offsetPoint, axis, angle);
                },
                (Collider other) => { Destroy(instance); }
            );

            instance.layer = AssignLayerToHitbox(gameObject.layer);
            yield return new WaitForSeconds(0.1f);
        }
    }

    #endregion
}