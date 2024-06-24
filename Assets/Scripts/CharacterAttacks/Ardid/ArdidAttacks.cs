using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.ShaderGraph;
using UnityEditorInternal;
using UnityEngine;

public class ArdidAttacks : GenericHitboxBehaviors
{
    [Serializable] private struct GeneralReferences
    {
        public GameObject sphereHitbox;
        public GameObject cubeHitbox;
        public Transform enemy;
        public NegativeEnergyManager negativeEnergyManager;
        public AttackManager attackManager;
    }

    [Serializable] private struct SummoningReferences
    {
        public GameObject archerSkeletonObject;
        public GameObject skeletonWall;
        public GameObject spectrePortal;
        public GameObject lichLord;
        public GameObject ghostGuardian;
    }

    [Serializable] private struct EnergyCosts
    {
        public int archerSkeletonCost;
        public int skeletonWallCost;
        public int portalSpectreCost;
        public int lichLordCost;
        public int ghostGuardianCost;
        public int necroticMistCost;
    }

    [Serializable] private struct NegativeEnergyRatios
    {
        public float necroticDart;
        public float deathScythe;
        public float necroticMist;
        public float sacrifice;
    }

    [Serializable] private struct DrainRatios
    {
        public float deathScytheDrainRatio;
    }

    [Serializable] private struct MultihitRates
    {
        public float necroticMistTimeRate;
    }
    
    [Serializable] private struct AttacksExtraParameters
    {
        public int archerSkeletonShotCount;
    }

    [SerializeField] GeneralReferences generalReferences;
    [SerializeField] SummoningReferences summoningReferences;
    [SerializeField] private EnergyCosts negativeEnergyCosts;
    [SerializeField] private NegativeEnergyRatios negativeEnergyRatios;
    [SerializeField] private DrainRatios drainRatios;
    [SerializeField] private MultihitRates multihitRates;
    [SerializeField] private AttacksExtraParameters attacksExtraParameters;
    
    [Header("Negative energy subtract rates")]
    [SerializeField] private float lichLordNegativeEnergySubtractRate;
    
    private Health health;
    bool isPortalActive;
    public Action<AttackData> onSummonSpectre;
    private GameObject necroticMistInstance;
    private List<GhostGuardian> ghostGuardians = new List<GhostGuardian>();

    private void Awake() 
    {
        health = GetComponent<Health>();
    }

    // GENERAL METHODS

    public void AddNegativeEnergyOnSuccessfulHit(Collider other, NegativeEnergyManager manager, float negativeEnergyAmount)
    {
        if (other.TryGetComponent(out PlayerStateMachine player))
        {
            if (player.colliderState == Hitbox.ColliderStates.ACTIVE)
            {
                manager.AddNegativeEnergy(negativeEnergyAmount);
            }
        }
    }

    public void AddNegativeEnergyOnAnyHit(Collider other, NegativeEnergyManager manager, float negativeEnergyAmount)
    {
        if (other.TryGetComponent(out PlayerStateMachine player))
        {
            if (player.colliderState != Hitbox.ColliderStates.INACTIVE)
            {
                manager.AddNegativeEnergy(negativeEnergyAmount);
            }
        }
    }

    public void DrainLifeOnSuccessfulHit(Collider other, AttackData attackData, float damageToHealthPointsRatio)
    {
        if (other.TryGetComponent(out PlayerStateMachine player))
        {
            if (player.colliderState == Hitbox.ColliderStates.ACTIVE)
            {
                health.IncreaseHealth((int)Mathf.Ceil(attackData.damage * damageToHealthPointsRatio));
            }
        }
    }

    public void DrainLifeOnAnyHit(Collider other, AttackData attackData, float damageToHealthPointsRatio)
    {
        if (other.TryGetComponent(out PlayerStateMachine player))
        {
            if (player.colliderState != Hitbox.ColliderStates.INACTIVE)
            {
                health.IncreaseHealth((int)Mathf.Ceil(attackData.damage * damageToHealthPointsRatio));
            }
        }
    }

    // NECROTIC DART
    public void NecroticDart(AttackData attackData)
    {
        GameObject hitboxInstance = InstantiateHitbox(
            generalReferences.sphereHitbox, 
            transform.position,
            Quaternion.identity,
            attackData,
            out Hitbox hitbox
            );
        
        hitbox.SetUpHitboxActions(
            //Hitbox start action
            () => {},
            
            //Hitbox update action
            FollowTargetOverTime(generalReferences.enemy, hitboxInstance.transform, attackData.range, attackData.activeTime),
            
            //Hitbox OnTriggerEnter action
            (Collider other) => {
                AddNegativeEnergyOnSuccessfulHit(other, generalReferences.negativeEnergyManager, negativeEnergyRatios.necroticDart);
                Destroy(hitboxInstance);
                }
            );
        hitboxInstance.layer = AssignLayerToHitbox(gameObject.layer);
    }

    // SOUL TRAP SCYTHE
    public void SoulTrapScythe(AttackData attackData)
    {
        GameObject hitboxInstance = InstantiateHitbox(
            generalReferences.sphereHitbox,
            transform.position + (transform.forward * attackData.range/2f),
            Quaternion.identity,
            attackData,
            out Hitbox hitbox
        );
        hitboxInstance.layer = AssignLayerToHitbox(gameObject.layer);

        hitboxInstance.transform.localScale = new Vector3(attackData.range, attackData.range, attackData.range);

        hitbox.SetUpHitboxActions(
            () => {generalReferences.attackManager.onActiveFramesEnd += () => {Destroy(hitboxInstance);};},
            () => {},
            (Collider other) => {
                AddNegativeEnergyOnAnyHit(other, generalReferences.negativeEnergyManager, negativeEnergyRatios.deathScythe);
                DrainLifeOnAnyHit(other, attackData, drainRatios.deathScytheDrainRatio);
                },
            () => {generalReferences.attackManager.onActiveFramesEnd -= () => {Destroy(hitboxInstance);};}
        );
    }

    // SUMMON ARCHER SKELETON

    public void SummonArcherSkeleton(AttackData attackData)
    {
        if (generalReferences.negativeEnergyManager.persistentNegativeEnergy < negativeEnergyCosts.archerSkeletonCost){ return;}

        Vector3 spawnPosition = transform.position + (generalReferences.enemy.position - transform.position).normalized * 2f;

        spawnPosition.y = 0f;

        ArdidArcherSkeleton archerSkeleton = Instantiate(summoningReferences.archerSkeletonObject, spawnPosition, Quaternion.identity).GetComponent<ArdidArcherSkeleton>();

        archerSkeleton.SetUpAttackData(attackData);
        archerSkeleton.enemy = generalReferences.enemy;
        archerSkeleton.gameObject.layer = gameObject.layer;
        archerSkeleton.maxShots = attacksExtraParameters.archerSkeletonShotCount;

        generalReferences.negativeEnergyManager.SubtractNegativeEnergy(negativeEnergyCosts.archerSkeletonCost);
    }

    // SUMMON SKELETON WALL
    public void SummonSkeletonWall(AttackData attackData)
    {
        if (generalReferences.negativeEnergyManager.persistentNegativeEnergy < negativeEnergyCosts.skeletonWallCost){return;}

        Vector3 vectorToTarget = generalReferences.enemy.position - transform.position;
        vectorToTarget.y = 0f;
        vectorToTarget.Normalize();

        Quaternion wallRotation = Quaternion.LookRotation(vectorToTarget);

        Vector3 spawnPosition = transform.position + vectorToTarget * 2f;
        spawnPosition.y = 0f;

        GameObject skeletonWallInstance = Instantiate(summoningReferences.skeletonWall, spawnPosition, wallRotation);

        generalReferences.negativeEnergyManager.SubtractNegativeEnergy(negativeEnergyCosts.skeletonWallCost);

        StartCoroutine(WallDestroyRoutine(attackData.activeTime, skeletonWallInstance));
    }

    private IEnumerator WallDestroyRoutine(float time, GameObject instance)
    {
        yield return new WaitForSeconds(time);
        Destroy(instance);
    }

    // NECROTIC MIST
    public void NecroticMist(AttackData attackData)
    {
        if (necroticMistInstance != null){return;}

        if (generalReferences.negativeEnergyManager.persistentNegativeEnergy < negativeEnergyCosts.necroticMistCost)
        {
            return;
        }

        generalReferences.negativeEnergyManager.SubtractNegativeEnergy(negativeEnergyCosts.necroticMistCost);

        int attackDataDamage = attackData.damage;

        GameObject hitboxInstance = InstantiateHitbox(generalReferences.sphereHitbox, transform, attackData, out Hitbox hitbox);

        necroticMistInstance = hitboxInstance;

        hitboxInstance.transform.localScale = Vector3.one * attackData.range;

        hitbox.attackData.damage = 0;
        hitbox.attackData.stunTime = 0f;

        Dictionary<string, Health> enemies = new Dictionary<string, Health>();

        hitboxInstance.layer = AssignLayerToHitbox(gameObject.layer);
        
        hitbox.SetUpHitboxActions(
            // Start
            () => {
                health.onHealthDecreased += NecroticMistCancel;
                hitbox.StartCoroutine(NecroticMistRoutine(enemies, attackDataDamage));
            },
            
            // Update
            DestroyHitboxAfterSeconds(hitboxInstance, attackData.activeTime),
            
            // OnTriggerEnter
            (Collider other) => {
                if (other.TryGetComponent(out Health health))
                {
                    enemies.Add(health.name, health);
                }
            },
            
            // OnDestroy
            () => { 
                health.onHealthDecreased -= NecroticMistCancel;
                hitbox.StopAllCoroutines();
                hitbox.attackData.damage = attackDataDamage;
            },
            
            // OnTriggerExit
            (Collider other) => {
                if (other.TryGetComponent(out Health health))
                {
                    enemies.Remove(health.name);
                }
                if (enemies.Count == 0)
                {
                    hitbox.StopAllCoroutines();
                }
            }
        );
    }

    private void NecroticMistCancel(int damage)
    {
        if (necroticMistInstance)
        {
            Destroy(necroticMistInstance);
            necroticMistInstance = null;
        }
    }

    private IEnumerator NecroticMistRoutine(Dictionary<string,Health> enemies, int damage)
    {
        while (true)
        {
            foreach (KeyValuePair<string, Health> enemy in enemies)
            {
                if (enemy.Value.TryGetComponent(out PlayerStateMachine stateMachine))
                {
                    if (stateMachine.colliderState != Hitbox.ColliderStates.ACTIVE)
                    {
                        continue;
                    }
                }

                enemy.Value.DecreaseHealth(damage);
                generalReferences.negativeEnergyManager.AddNegativeEnergy(negativeEnergyRatios.necroticMist);
            }
            yield return new WaitForSeconds(multihitRates.necroticMistTimeRate);
        }
    }

    // PHANTOM ONSLAUGHT
    public void PhantomOnslaught(AttackData attackData)
    {
        Vector3 vectorToTarget = (generalReferences.enemy.position - transform.position).normalized;

        GameObject hitboxInstance = InstantiateHitbox(generalReferences.cubeHitbox, transform.position, Quaternion.LookRotation(vectorToTarget), attackData, out Hitbox hitbox);

        hitboxInstance.transform.localScale = new Vector3(5f, 0.5f, 1f);

        hitboxInstance.layer = AssignLayerToHitbox(gameObject.layer);

        float timer = attackData.activeTime;

        hitbox.SetUpHitboxActions(
            () => {},
            () => {
                hitboxInstance.transform.position += vectorToTarget * (attackData.range / attackData.activeTime) * Time.deltaTime;

                timer -= Time.deltaTime;

                if (timer <= 0)
                {
                    Destroy(hitboxInstance);
                }
            },
            (Collider other) => {}
        );
    }

    // SUMMON BONE STALKER
    public void SummonBoneStalker(AttackData attackData)
    {
        int originalDamage = attackData.damage;
        float originalStunTime = attackData.stunTime;

        GameObject hitboxInstance = InstantiateHitbox(generalReferences.sphereHitbox, transform.position, Quaternion.identity, attackData, out Hitbox hitbox);

        hitbox.attackData.damage = 0;
        hitbox.attackData.stunTime = 0f;

        hitboxInstance.layer = AssignLayerToHitbox(gameObject.layer);

        hitbox.SetUpHitboxActions(
            // Start
            () => {},

            // Update
            FollowTargetOverTime(generalReferences.enemy, hitboxInstance.transform, attackData.range, attackData.activeTime),
            
            // OnTriggerEnter
            (Collider other) => {
                Destroy(hitboxInstance);
            },

            // OnDestroy
            () => {
                InstantiateSubHitbox(hitboxInstance.transform, originalDamage, originalStunTime);
                attackData.damage = originalDamage;
                attackData.stunTime = originalStunTime;
                }
        );
    }

    private Hitbox InstantiateSubHitbox(Transform hitboxInstance, int damage, float stunTime)
    {
        Hitbox hitbox;
        AttackData subHitboxAttackData = new()
        {
            damage = damage,
            range = 5f,
            activeTime = 0.5f,
            stunTime = stunTime,
            attackEffects = new AttackData.AttackEffects[0]
        };

        GameObject subhitbox = InstantiateHitbox(generalReferences.sphereHitbox, hitboxInstance.position, Quaternion.identity, subHitboxAttackData, out hitbox);

        subhitbox.layer = AssignLayerToHitbox(gameObject.layer);

        subhitbox.transform.localScale = Vector3.one * subHitboxAttackData.range;

        hitbox.SetUpHitboxActions(
            () => { },
            DestroyHitboxAfterSeconds(subhitbox, subHitboxAttackData.activeTime),
            (Collider other) => { }
        );
        return hitbox;
    }

    // SACRIFICE
    public void Sacrifice(AttackData attackData)
    {
        StartCoroutine(SacrificeRoutine(health, attackData.activeTime));
    }

    private IEnumerator SacrificeRoutine(Health health, float time)
    {
        Action<int> action = (int amount) => {
            generalReferences.negativeEnergyManager.AddNegativeEnergy(amount * negativeEnergyRatios.sacrifice);
            };
        
        health.onHealthDecreased += action;

        yield return new WaitForSeconds(time);

        health.onHealthDecreased -= action;
    }

    // PORTAL / SPECTRE
    public void PortalSpectre(AttackData attackData)
    {
        onSummonSpectre?.Invoke(attackData);

        if(isPortalActive){return;}

        Vector3 spawnPosition = transform.position + Vector3.up * 2f;

        Vector3 vectorToTarget = generalReferences.enemy.position - transform.position;
        vectorToTarget.y = 0f;
        vectorToTarget.Normalize();

        GameObject portalInstance = Instantiate(summoningReferences.spectrePortal, spawnPosition, Quaternion.LookRotation(vectorToTarget));

        portalInstance.layer = gameObject.layer;

        ArdidSpectrePortal portalBehavior = portalInstance.GetComponent<ArdidSpectrePortal>();
        
        portalBehavior.negativeEnergyManager = generalReferences.negativeEnergyManager;
        portalBehavior.spectreCost = negativeEnergyCosts.portalSpectreCost;
        portalBehavior.enemy = generalReferences.enemy;

        portalBehavior.negativeEnergyAmount += Mathf.Min(generalReferences.negativeEnergyManager.persistentNegativeEnergy, negativeEnergyCosts.portalSpectreCost);

        onSummonSpectre += portalBehavior.SummonSpectre(attackData);

        portalBehavior.onDestroy += () => {
            isPortalActive = false;
            onSummonSpectre = null;
            };

        generalReferences.negativeEnergyManager.SubtractNegativeEnergy(Mathf.Min(generalReferences.negativeEnergyManager.persistentNegativeEnergy, negativeEnergyCosts.portalSpectreCost));

        isPortalActive = true;
    }

    // SUMMON LICH LORD
    public void SummonLichLord(AttackData attackData)
    {
        if (generalReferences.negativeEnergyManager.persistentNegativeEnergy < negativeEnergyCosts.lichLordCost){ return;}

        Vector3 spawnPosition = transform.position + (generalReferences.enemy.position - transform.position).normalized * 2f;

        spawnPosition.y = 0f;

        ArdidLichLord lichLord = Instantiate(summoningReferences.lichLord, spawnPosition, Quaternion.identity).GetComponent<ArdidLichLord>();

        lichLord.SetUpAttackData(attackData);
        lichLord.enemy = generalReferences.enemy;
        lichLord.manager = generalReferences.negativeEnergyManager;
        lichLord.gameObject.layer = gameObject.layer;

        generalReferences.negativeEnergyManager.SubtractNegativeEnergy(negativeEnergyCosts.lichLordCost);
    }

    // GRIP BEYOND THE GRAVE
    public void GripBeyondTheGrave(AttackData attackData)
    {
        Vector3 vectorToTarget = generalReferences.enemy.position - transform.position;

        Vector3 spawnPosition = transform.position + Vector3.ClampMagnitude(vectorToTarget, attackData.range);

        GameObject hitboxInstance = InstantiateHitbox(generalReferences.sphereHitbox, spawnPosition, Quaternion.identity, attackData, out Hitbox hitbox);

        hitboxInstance.layer = AssignLayerToHitbox(gameObject.layer);

        hitbox.SetUpHitboxActions(
            () => {},
            DestroyHitboxAfterSeconds(hitboxInstance, attackData.activeTime),
            (Collider other) => {}
        );
    }

    // GHOST GUARDIAN
    public void SummonGhostGuardian(AttackData attackData)
    {
        if (generalReferences.negativeEnergyManager.persistentNegativeEnergy < negativeEnergyCosts.ghostGuardianCost)
        {
            return;
        }

        if (ghostGuardians.Count >= 3){return;}
        
        Vector3 vectorToCenter = Vector3.zero - transform.position;
        vectorToCenter.y = 0f;
        vectorToCenter.Normalize();

        GameObject ghostGuardianInstance = Instantiate(summoningReferences.ghostGuardian, transform.position + vectorToCenter * 2f, Quaternion.identity);

        GhostGuardian ghostGuardian = ghostGuardianInstance.GetComponent<GhostGuardian>();

        ghostGuardians.Add(ghostGuardian);
        ghostGuardian.OnDestroyAction += () => {
            ghostGuardians.Remove(ghostGuardian);
            ghostGuardian.OnDestroyAction = null;
        };

        ghostGuardian.index = ghostGuardians.Count -1;

        GhostGuardian.ghostGuardianInstances = ghostGuardians.Count;

        ghostGuardian.enemy = generalReferences.enemy;
        ghostGuardian.owner = transform;
        ghostGuardian.attackData = attackData;
        ghostGuardian.sphereHitbox = generalReferences.sphereHitbox;

        ghostGuardianInstance.layer = gameObject.layer;

        generalReferences.negativeEnergyManager.SubtractNegativeEnergy(negativeEnergyCosts.ghostGuardianCost);
    }
}
