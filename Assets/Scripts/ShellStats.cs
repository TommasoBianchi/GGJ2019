using UnityEngine;

[CreateAssetMenu(menuName = "Stats/Shell")]
public class ShellStats : ScriptableObject
{

    [SerializeField]
    private float movementSpeed;
    [SerializeField]
    private float hitDamage;
    [SerializeField]
    private float attackCooldown;
    [SerializeField]
    private float shellHealth;

    [SerializeField]
    private bool canAttack;
    [SerializeField]
    private bool canBlock;
    [SerializeField]
    private bool canPickupShells;

    [SerializeField]
    private Transform modelPrefab;

    [SerializeField]
    private bool isRanged;
    [SerializeField]
    private Projectile projectilePrefab;
    [SerializeField]
    private float projectileSpeed;

    #region Properties

    public float MovementSpeed { get { return movementSpeed; } }
    public float HitDamage { get { return hitDamage; } }
    public float AttackCooldown { get { return attackCooldown; } }
    public float ShellHealth { get { return shellHealth; } }

    public bool CanAttack { get { return canAttack; } }
    public bool CanBlock { get { return canBlock; } }
    public bool CanPickupShells { get { return canPickupShells; } }

    public Transform ModelPrefab { get { return modelPrefab; } }

    public bool IsRanged { get { return isRanged; } }
    public Projectile ProjectilePrefab { get { return projectilePrefab; } }
    public float ProjectileSpeed { get { return projectileSpeed; } }

    #endregion
}