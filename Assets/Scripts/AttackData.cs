using System;
using UnityEngine;

[Serializable]
public class AttackData
{
    public enum AttackEffects
    {
        NONE,
        SHIELDBREAK,
        BURN,
        SLOWDOWN,
        LIFESTEAL,
        POISON,
        KNOCKDOWN
    }

    public string name;
    public int damage;
    public float range;
    public float activeTime;
    public float stunTime;
    public AnimationClip attackAnimation;
    public AttackEffects[] attackEffects;
    public AttackEvent attackEvent;
}