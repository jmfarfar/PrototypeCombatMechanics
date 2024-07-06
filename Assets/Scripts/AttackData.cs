using System;
using System.Drawing;
using UnityEngine;
using UnityEngine.VFX;

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
        KNOCKDOWN,
        RESTRAIN
    }

    public string name;
    public int damage;
    public float range;
    public float activeTime;
    public float stunTime;
    public AnimationClip attackAnimation;
    public AudioClip attackSfx;
    public VisualEffect attackVfx;
    public AttackEffects[] attackEffects;
    public AttackEvent attackEvent;
    public Sprite attackIcon;
}