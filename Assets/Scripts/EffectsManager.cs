using UnityEngine;

public class EffectsManager : MonoBehaviour, IAnimationEffects
{
    private AttackData[] attackData;
    private PlayerStateMachine[] players;
    [field: SerializeField] public IPlayerInput PlayerInput { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        attackData = GetComponent<AttackManager>().attacks;
        PlayerInput = GetComponentInParent<IPlayerInput>();

        players = FindObjectsOfType<PlayerStateMachine>();

        foreach (PlayerStateMachine player in players)
        {
            player.onAttackStart += OnAttackStart;
        }
    }

    public void StartEffect(int test)
    {
        for (int i = 0; i < attackData.Length; i++)
        {
            AudioManager.Instance.PlaySound(attackData[i].attackSfx);
            attackData[i].attackVfx.Play();
        }

        PlayerInput.GetRadialMenuQuadrant();
    }

    public void OnAttackStart(int attackIndex)
    {
        AudioManager.Instance.PlaySound(attackData[attackIndex].attackSfx);
        attackData[attackIndex].attackVfx.Play();
    }

    public void OnAttackEnd(int attackIndex)
    {
        attackData[attackIndex].attackVfx.Stop();
        foreach (PlayerStateMachine player in players)
        {
            player.onAttackStart -= OnAttackStart;
        }
    }

    public void Attack1StartEffect()
    {
        AudioManager.Instance.PlaySound(attackData[0].attackSfx);
        attackData[0].attackVfx.Play();
    }

    public void Attack1EndEffect()
    {
        attackData[0].attackVfx.Stop();
    }

    public void Attack2StartEffect()
    {
        AudioManager.Instance.PlaySound(attackData[1].attackSfx);
        attackData[1].attackVfx.Play();
    }

    public void Attack2EndEffect()
    {
        attackData[1].attackVfx.Stop();
    }

    public void Attack3StartEffect()
    {
        AudioManager.Instance.PlaySound(attackData[2].attackSfx);
        attackData[2].attackVfx.Play();
    }

    public void Attack3EndEffect()
    {
        attackData[2].attackVfx.Stop();
    }

    public void Attack4StartEffect()
    {
        AudioManager.Instance.PlaySound(attackData[3].attackSfx);
        attackData[3].attackVfx.Play();
    }

    public void Attack4EndEffect()
    {
        attackData[3].attackVfx.Stop();
    }

    public void Attack5StartEffect()
    {
        AudioManager.Instance.PlaySound(attackData[4].attackSfx);
        attackData[4].attackVfx.Play();
    }

    public void Attack5EndEffect()
    {
        attackData[4].attackVfx.Stop();
    }

    public void Attack6StartEffect()
    {
        AudioManager.Instance.PlaySound(attackData[5].attackSfx);
        attackData[5].attackVfx.Play();
    }

    public void Attack6EndEffect()
    {
        attackData[5].attackVfx.Stop();
    }

    public void Attack7StartEffect()
    {
        AudioManager.Instance.PlaySound(attackData[6].attackSfx);
        attackData[6].attackVfx.Play();
    }

    public void Attack7EndEffect()
    {
        attackData[6].attackVfx.Stop();
    }

    public void Attack8StartEffect()
    {
        AudioManager.Instance.PlaySound(attackData[7].attackSfx);
        attackData[7].attackVfx.Play();
    }

    public void Attack8EndEffect()
    {
        attackData[7].attackVfx.Stop();
    }

    public void Attack9StartEffect()
    {
        AudioManager.Instance.PlaySound(attackData[8].attackSfx);
        attackData[8].attackVfx.Play();
    }

    public void Attack9EndEffect()
    {
        attackData[8].attackVfx.Stop();
    }

    public void Attack10StartEffect()
    {
        AudioManager.Instance.PlaySound(attackData[9].attackSfx);
        attackData[9].attackVfx.Play();
    }

    public void Attack10EndEffect()
    {
        attackData[9].attackVfx.Stop();
    }

    public void Attack11StartEffect()
    {
        AudioManager.Instance.PlaySound(attackData[10].attackSfx);
        attackData[10].attackVfx.Play();
    }

    public void Attack11EndEffect()
    {
        attackData[10].attackVfx.Stop();
    }

    public void Attack12StartEffect()
    {
        AudioManager.Instance.PlaySound(attackData[11].attackSfx);
        attackData[11].attackVfx.Play();
    }

    public void Attack12EndEffect()
    {
        attackData[11].attackVfx.Stop();
    }
}