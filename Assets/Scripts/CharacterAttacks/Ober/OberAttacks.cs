using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

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

    public List<BloodMoonConfig> Projectiles;
    public List<salamanderConfig> Salamanders;
    public PlacementStrategy placementStrategy;
    Spawn salamander;
    Spawn projectile;

    public float maxSpawns = 10;

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
        GameObject instance = InstantiateHitbox(sphereHitbox, transform.position, Quaternion.identity, attackData, out Hitbox hitbox);

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

        hitbox.gameObject.layer = LayerMask.NameToLayer("Player2Hitbox");
    }

    public void FireBreath(AttackData attackData)
    {
        GameObject instance = InstantiateHitbox(fireBreathHitbox, transform.position + (transform.forward * attackData.range / 2), Quaternion.identity, attackData, out Hitbox hitbox);

        hitbox.SetUpHitboxActions(
            () =>
            {
                if (isFuryActive)
                {
                    attackData.damage *= 2;
                }

                manager.onActiveFramesEnd += () =>
                {
                    Destroy(instance);
                };
            },
            () =>
            {
            },
            (Collider other) =>
            {
                //StartCoroutine(BurnRoutine(3f, 10f, ));
                Debug.Log("Hit by Fire Breath");

            },
            () => { manager.onActiveFramesEnd -= () => { Destroy(instance); }; }
        );

        hitbox.gameObject.layer = LayerMask.NameToLayer("Player2Hitbox");
    }

    #region SALAMANDER NEST
    public void SalamanderNest(AttackData attackData)
    {
        GameObject nestInstance = InstantiateHitbox(salamanderNest, transform.position + (Vector3.forward * 2), Quaternion.identity, attackData, out Hitbox hitbox);

        hitbox.SetUpHitboxActions(
            () =>
            {
                //SpawnSalamanders(attackData);
                StartCoroutine(SpawnSalamanders(attackData, nestInstance));
            },
            () =>
            {
            },
            (Collider other) => { }
        );

        hitbox.gameObject.layer = LayerMask.NameToLayer("Player2Hitbox");
    }

    IEnumerator SpawnSalamanders(AttackData attackData, GameObject nest)
    {
        for (int i = 0; i < maxSpawns; i++)
        {
            salamander = Create(Salamanders[i % Salamanders.Count], nest);
            salamander.transform.position = placementStrategy.SetPosition(nest.transform.position);

            // Salamander behavior
            salamander.GetComponent<Hitbox>().SetUpHitboxActions(
                () => { },
                FollowTargetOverTime(enemy, salamander.transform, attackData.range, attackData.activeTime),
                (Collider other) =>
                {
                    Destroy(salamander.gameObject);
                }
            );

            salamander.GetComponent<Hitbox>().gameObject.layer = LayerMask.NameToLayer("Player2Hitbox");
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
        GameObject lavaInstance = InstantiateHitbox(lavaFloor, enemy.transform.position, Quaternion.identity, attackData, out Hitbox hitbox);

        hitbox.SetUpHitboxActions(
            () => { },
            () => { },
            (Collider other) =>
            {


                //other.GetComponent<PlayerStateMachine>().Combatant.GetComponent<Health>().DecreaseHealth(attackData.damage);

            },
            (Collider other) =>
            {
                if (other.TryGetComponent<PlayerStateMachine>(out PlayerStateMachine opponent))
                {
                    StartCoroutine(LavaFloorLifetime(attackData, lavaInstance.gameObject, opponent));
                }

            },

            () => { }
        );
    }

    IEnumerator LavaFloorLifetime(AttackData data, GameObject instance, PlayerStateMachine opponent)
    {
        float start = Time.time;

        while (Time.time < start + data.activeTime)
        {
            yield return new WaitForSeconds(1f);
            opponent.GetComponent<Health>().DecreaseHealth(data.damage);

        }
        Destroy(instance);
    }

    #endregion

    #region JUMP

    public void Jump(AttackData attackData)
    {
        GameObject impactInstance = InstantiateHitbox(impactZone, transform.position, Quaternion.identity, attackData, out Hitbox hitbox);
        float animation = 0;
        float time = attackData.activeTime;

        hitbox.SetUpHitboxActions(
            () =>
            {
                //impactInstance.SetActive(false);
            },

            //ParabolicJump(transform, transform.position, enemy.transform.position, 5f, attackData.activeTime),
            () =>
            {
                time -= Time.deltaTime;
                 
                if (time > 0f)
                {
                    animation += Time.deltaTime;

                    animation = animation % attackData.activeTime;

                    transform.position = Parabolic(transform.position, /*transform.forward * attackData.range*/ enemy.transform.position, 5f, animation / attackData.activeTime);

                }
            },

            //() =>
            //{
            //    Vector3 direction = enemy.transform.position - transform.position;
            //    Vector3 groundDirection = new Vector3(direction.x, 0, direction.z);
            //    Vector3 targetPos = new Vector3(groundDirection.magnitude, direction.y, 0);
            //    float height = targetPos.y + targetPos.magnitude / 2f;

            //    CalculatePathWithHeight(targetPos, height, out float v0, out float angle, out float time);

            //    StopAllCoroutines();
            //    StartCoroutine(ParabolicJump(groundDirection.normalized, v0, angle, time));

            //},
            (Collider other) =>
            {
                Destroy(impactInstance);
            }
        );

        hitbox.gameObject.layer = LayerMask.NameToLayer("Player2Hitbox");

    }

    IEnumerator Start()
    {
        float maxTime = 2f;
        float time = maxTime;
        Vector3 force = new Vector3(0f, 10f, 0f);
        while (time > 0f)
        {
            force.z = 5f;

            transform.Translate(force * Time.deltaTime);

            force.y -= 20f * Time.deltaTime;

            time -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    public static Vector3 Parabolic(Vector3 start, Vector3 end, float height, float time)
    {
        Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

        var mid = Vector3.Lerp(start, end, time);

        return new Vector3(mid.x, f(time) + Mathf.Lerp(start.y, end.y, time), mid.z);
    }

    IEnumerator ParabolicJump(Vector3 direction, float v0, float angle, float time)
    {
        float t = 0;
        while (t < time)
        {
            float x = v0 * t * Mathf.Cos(angle);
            float y = v0 * t * Mathf.Sin(angle) - (1f / 2f) * -Physics.gravity.y * Mathf.Pow(t, 2);

            transform.position = transform.position + direction * x + Vector3.up * y;
            t += Time.deltaTime;
            yield return null;
        }
    }

    private void CalculatePathWithHeight(Vector3 targetPos, float h, out float v0, out float angle, out float time)
    {
        float xt = targetPos.x;
        float yt = targetPos.y;
        float g = -Physics.gravity.y;

        float b = Mathf.Sqrt(2 * g * h);
        float a = (-0.5f * g);
        float c = -yt;

        float tplus = QuadraticEquation(a, b, c, 1);
        float tmin = QuadraticEquation(a, b, c, -1);
        time = tplus > tmin ? tplus : tmin;

        angle = Mathf.Atan(b * time / xt);

        v0 = b / Mathf.Sin(angle);
    }

    private float QuadraticEquation(float a, float b, float c, float sign)
    {
        return (-b + sign * Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);
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

            //foreach (AttackData attack in opponentAttackData)
            //{
            //    attack.damage = (int)Mathf.Ceil(attack.damage / 0.7f); // restore damage
            //}
        }

    }

    #endregion

    #region FIREWALL

    public void FireWall(AttackData attackData)
    {
        GameObject wallInstance = InstantiateHitbox(firewall, transform.position + Vector3.left, Quaternion.LookRotation(transform.position), attackData, out Hitbox hitbox);

        hitbox.SetUpHitboxActions(
            () =>
            {
                StartCoroutine(SpawnWall(attackData, wallInstance));
            },
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

        GameObject moonInstance = InstantiateHitbox(bloodMoon, transform.position + (Vector3.up * 4), Quaternion.identity, attackData, out Hitbox hitbox);

        hitbox.SetUpHitboxActions(
            () =>
            {
                StartCoroutine(SpawnProjectiles(attackData, moonInstance));
            },
            () => { },
            (Collider other) => { }


        );

        hitbox.gameObject.layer = LayerMask.NameToLayer("Player2Hitbox");
    }

    IEnumerator SpawnProjectiles(AttackData attackData, GameObject bloodMoon)
    {
        for (int i = 0; i < maxSpawns; i++)
        {
            projectile = Create(Projectiles[i % Projectiles.Count], bloodMoon);

            projectile.transform.position = bloodMoon.transform.position;

            projectile.GetComponent<Hitbox>().SetUpHitboxActions(
                () => { },
                FollowTargetOverTime(enemy, projectile.transform, attackData.range, attackData.activeTime),
                (Collider other) =>
                {
                    Destroy(projectile.gameObject);
                }
            );

            projectile.GetComponent<Hitbox>().gameObject.layer = LayerMask.NameToLayer("Player2Hitbox");
            yield return new WaitForSeconds(5);
        }
        Destroy(bloodMoon);
    }

    private Spawn Create(BloodMoonConfig config, GameObject moon)
    {
        GameObject spawn = Instantiate(config.projectile, moon.transform.position + (Vector3.forward * 3), Quaternion.identity);
        Spawn spawnComponent = spawn.GetComponent<Spawn>();
        return spawnComponent;
    }

    #endregion

    #region CONCENTRATION

    public void Concentration(AttackData attackData)
    {
        Debug.Log("Concentration");
    }

    #endregion


    #region PYROMANIAC

    public void Pyromaniac(AttackData attackData)
    {

        GameObject instance = InstantiateHitbox(sphereHitbox, transform.position, Quaternion.identity, attackData, out Hitbox hitbox);
        float time = attackData.activeTime;
        float animation = 0;

        hitbox.SetUpHitboxActions(
            () => { },
            () =>
            {

                time -= Time.deltaTime;

                if (time > 0f)
                {
                    animation += Time.deltaTime;

                    animation = animation % attackData.activeTime;

                    transform.position = Parabolic(transform.position, placementStrategy.SetPosition(transform.position), 5f, animation / attackData.activeTime);

                }
            },
            (Collider other) => { }
        );
    }

    #endregion




}
