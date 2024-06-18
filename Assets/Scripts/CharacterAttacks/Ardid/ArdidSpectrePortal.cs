using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class ArdidSpectrePortal : GenericHitboxBehaviors
{
    [SerializeField] private GameObject spectrePrefab;
    public Transform enemy;
    public NegativeEnergyManager negativeEnergyManager;
    public int negativeEnergyAmount;
    public int spectreCost;
    public int summonedSpectres;
    Action<int> transferNegativeEnergy;
    public Action onDestroy;

    void Start()
    {
        transferNegativeEnergy = (int amount) => {
        negativeEnergyManager.SubtractNegativeEnergy(Mathf.Min(spectreCost - negativeEnergyAmount, amount));
        negativeEnergyAmount = Mathf.Min(negativeEnergyAmount += amount, spectreCost);
        };
        
        negativeEnergyManager.onAddPersistentNegativeEnergy += transferNegativeEnergy;
    }

    void Update()
    {
        transform.localScale = Vector3.one * (negativeEnergyAmount * 0.5f + 1);
    }

    void OnDestroy()
    {
        negativeEnergyManager.onAddPersistentNegativeEnergy -= transferNegativeEnergy;
        onDestroy?.Invoke();
    }

    public Action<AttackData> SummonSpectre(AttackData attackData)
    {
        return (AttackData AttackData) => {
            if (negativeEnergyAmount == 0){Destroy(gameObject); return;}

            for (int i = 0; i < negativeEnergyAmount; i++)
            {
                StartCoroutine(SpectreRoutine(i * 0.2f, AttackData));
            }
        };
    }

    IEnumerator SpectreRoutine(float time, AttackData attackData)
    {
        yield return new WaitForSeconds(time);

        Vector3 vectorToTarget = (enemy.position - transform.position).normalized;

        GameObject spectreInstance = InstantiateHitbox(spectrePrefab, transform.position, Quaternion.LookRotation(vectorToTarget), attackData, out Hitbox hitbox);

        hitbox.gameObject.layer = AssignLayerToHitbox(gameObject.layer);

        hitbox.SetUpHitboxActions(
            () => {},
            () => {
                spectreInstance.transform.position += vectorToTarget * (attackData.range / attackData.activeTime) * Time.deltaTime;
                } + DestroyHitboxAfterSeconds(spectreInstance, attackData.activeTime),
            (Collider other) => {Destroy(spectreInstance);}
        );

        summonedSpectres++;

        if (summonedSpectres == negativeEnergyAmount)
        {
            Destroy(gameObject);
        }
    }
}
