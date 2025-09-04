using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    [Tooltip("Higher priority abilities override lower ones")]
    public int priority = 0;

    [Tooltip("Cooldown time in seconds before the ability can be reused")]
    [SerializeField] private float cooldownTimer = 0f;

    public void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    public abstract void Activate(PlayerInput input);

    public abstract bool IsActive();

    public bool CanUse() => cooldownTimer <= 0f;


    public void StartCooldown(float customCooldown) 
    {
        cooldownTimer = customCooldown;
    }
}
