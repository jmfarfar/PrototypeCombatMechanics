using UnityEngine;

public class EffectsManager : MonoBehaviour, IAnimationEffects
{
    private AttackData[] attackData;

    // Start is called before the first frame update
    void Start()
    {
        attackData = GetComponent<AttackManager>().attacks;
    }

    public void Attack1StartEffect()
    {
        AudioManager.Instance.PlaySound(attackData[1].attackSfx);
        attackData[1].attackVfx.Play();
    }

    public void Attack1EndEffect()
    {
        attackData[1].attackVfx.Stop();
    }

    public void Attack2StartEffect()
    {
        AudioManager.Instance.PlaySound(attackData[2].attackSfx);
        attackData[2].attackVfx.Play();
    }

    public void Attack2EndEffect()
    {
        attackData[2].attackVfx.Stop();
    }

    public void Attack3StartEffect()
    {
        AudioManager.Instance.PlaySound(attackData[3].attackSfx);
        attackData[3].attackVfx.Play();
    }

    public void Attack3EndEffect()
    {
        attackData[3].attackVfx.Stop();
    }

    public void Attack4StartEffect()
    {
        AudioManager.Instance.PlaySound(attackData[4].attackSfx);
        attackData[4].attackVfx.Play();
    }

    public void Attack4EndEffect()
    {
        attackData[4].attackVfx.Stop();
    }

    public void Attack5StartEffect()
    {
        AudioManager.Instance.PlaySound(attackData[5].attackSfx);
        attackData[5].attackVfx.Play();
    }

    public void Attack5EndEffect()
    {
        attackData[5].attackVfx.Stop();
    }

    public void Attack6StartEffect()
    {
        AudioManager.Instance.PlaySound(attackData[6].attackSfx);
        attackData[6].attackVfx.Play();
    }

    public void Attack6EndEffect()
    {
        attackData[6].attackVfx.Stop();
    }

    public void Attack7StartEffect()
    {
        AudioManager.Instance.PlaySound(attackData[7].attackSfx);
        attackData[7].attackVfx.Play();
    }

    public void Attack7EndEffect()
    {
        attackData[7].attackVfx.Stop();
    }

    public void Attack8StartEffect()
    {
        AudioManager.Instance.PlaySound(attackData[8].attackSfx);
        attackData[8].attackVfx.Play();
    }

    public void Attack8EndEffect()
    {
        attackData[8].attackVfx.Stop();
    }

    public void Attack9StartEffect()
    {
        AudioManager.Instance.PlaySound(attackData[9].attackSfx);
        attackData[9].attackVfx.Play();
    }

    public void Attack9EndEffect()
    {
        attackData[9].attackVfx.Stop();
    }

    public void Attack10StartEffect()
    {
        AudioManager.Instance.PlaySound(attackData[10].attackSfx);
        attackData[10].attackVfx.Play();
    }

    public void Attack10EndEffect()
    {
        attackData[10].attackVfx.Stop();
    }

    public void Attack11StartEffect()
    {
        AudioManager.Instance.PlaySound(attackData[11].attackSfx);
        attackData[11].attackVfx.Play();
    }

    public void Attack11EndEffect()
    {
        attackData[11].attackVfx.Stop();
    }

    public void Attack12StartEffect()
    {
        AudioManager.Instance.PlaySound(attackData[12].attackSfx);
        attackData[12].attackVfx.Play();
    }

    public void Attack12EndEffect()
    {
        attackData[12].attackVfx.Stop();
    }
}