using UnityEngine;

[CreateAssetMenu(menuName = "Stats/Shell")]
public class ShellStats : ScriptableObject
{

    [SerializeField]
    private float movementSpeed;
    [SerializeField]
    private bool canAttack;
    [SerializeField]
    private bool canBlock;
    [SerializeField]
    private bool canPickupShells;

    #region Properties

    public float MovementSpeed { get { return movementSpeed; } }
    public bool CanAttack { get { return canAttack; } }
    public bool CanBlock { get { return canBlock; } }
    public bool CanPickupShells { get { return canPickupShells; } }

    #endregion
}