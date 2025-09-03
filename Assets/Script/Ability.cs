using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    [Tooltip("Higher priority abilities override lower ones")]
    public int priority = 0;

    [SerializeField] protected float cooldown = 0f;
    protected float cooldownTimer = 0f;

    protected virtual void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;
    }
    public abstract void Activate(PlayerInput input);

    public abstract bool IsActive();

    public virtual bool CanUse() => cooldownTimer <= 0f;
    protected void StartCooldown() => cooldownTimer = cooldown;
}
